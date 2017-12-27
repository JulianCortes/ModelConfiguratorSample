using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Bunny_TK.Net
{
    public class NetworkTransportManager : MonoBehaviour
    {
        private bool _isServer = false;
        private bool _isStarted = false;

        public string hostIP = "10.1.1.51";
        public int port = 7075;
        private int _connectionID = 0;

        private int _genericHostId = 0;
        private int _webSocketHostId = 0;
        private int _communicationChannel = 0;
        private ConnectionConfig _config = null;

        //Host
        private List<int> _partyIDs = new List<int>();

        private List<string> _partyIPs = new List<string>();

        //Client
        private int _hostID = 0;

        private int _host_ConnectionID = 0;

        //Personal
        private string _myIp = "127.0.0.1";

        private int commandsSent = 0;
        private int commandsReceived = 0;
        private CommandType _lastCommandReceived;
        private CommandType _lastCommandSent;
        private Status _status = Status.None;
        private string _lastMessageError = "";
        private string _lastCommandJson = "";

        #region EventCommands

        public bool ingoreTestCommands = false;

        public delegate void CommandHandler(object sender, NetworkTransportEventArgs args);

        public event CommandHandler OnReceivedCommand;

        public event CommandHandler OnSentCommand;

        #endregion EventCommands

        #region Properties

        public int ConnectionID { get { return _connectionID; } }
        public int HostID { get { return _hostID; } }
        public int HostConnectionID { get { return _host_ConnectionID; } }

        public string MyIP { get { return _myIp; } }

        public int CommandsSent { get { return commandsSent; } }
        public int CommandsReceived { get { return commandsReceived; } }

        public Status CurrentStatus { get { return _status; } }

        public CommandType LastCommandReceived { get { return _lastCommandReceived; } }
        public CommandType LastCommandSent { get { return _lastCommandSent; } }

        public string LastMessageError { get { return _lastMessageError; } }

        public string LastCommandJson { get { return _lastCommandJson; } }

        #endregion Properties

        #region Unity Stuff

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (_isServer) Host_CheckForNetworkEvents();
            else Client_CheckForNetworkEvents();
        }

        #endregion Unity Stuff

        #region Host Functionalities

        //Inizializzazione del host
        public void StartHosting(int port)
        {
            this.port = port;
            _partyIDs = new List<int>();
            _partyIPs = new List<string>();

            NetworkTransport.Init();
            HostTopology topology = new HostTopology(_config, 24);
            _webSocketHostId = NetworkTransport.AddWebsocketHost(topology, port, null);// <- Probabilmente non neccessario
            _genericHostId = NetworkTransport.AddHost(topology, port, null);

            _myIp = GetLocalIPAddress();

            commandsSent = 0;
            commandsReceived = 0;

            _isServer = true;
            _isStarted = true;
            _status = Status.Hosting;
        }

        //Ricevuto un comando di richiesta inserimento al party da parte di un client
        private void ReceivedRequestToParty(int _receiverConnectionID, PartyCommand receivedPartyCommand)
        {
            if (!_partyIDs.Exists(id => id == _receiverConnectionID))
                _partyIDs.Add(_receiverConnectionID);

            int index = _partyIDs.IndexOf(_receiverConnectionID);

            //Save ip
            if (!_partyIPs.Exists(ip => ip == receivedPartyCommand.ipSender))
            {
                _partyIPs.Add(receivedPartyCommand.ipSender);
            }
            else
            {
                if (_partyIPs[index] != receivedPartyCommand.ipSender)
                    _partyIPs[index] = receivedPartyCommand.ipSender;
            }

            //Send Message of request accepted
            PartyCommand partyCommand = new PartyCommand(_myIp, _connectionID, _receiverConnectionID, false);
            SendCommand(partyCommand);
        }

        //Chiamato in Update, una volta inizializzato come server, rimane in ascolto di eventuali eventi.
        //In relazione all'evento vengono effettuate le funzioni relative.
        private void Host_CheckForNetworkEvents()
        {
            if (!_isStarted) return;

            int recHostId;
            int connectionId;
            int channelId;
            int dataSize;
            int bufferSize = 1024;
            byte error;
            byte[] recBuffer = new byte[1024];

            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

            switch (recData)
            {
                case NetworkEventType.Nothing:
                    {
                        break;
                    }

                case NetworkEventType.ConnectEvent:
                    {
                        break;
                    }

                case NetworkEventType.DataEvent:
                    {
                        CommandType commandType;
                        object o = DecodeJsonCommand(recBuffer, out commandType);
                        _lastCommandReceived = commandType;

                        switch (commandType)
                        {
                            case CommandType.Party:
                                {
                                    PartyCommand partyCommand = (PartyCommand)o;
                                    if (partyCommand.isRequest)
                                        ReceivedRequestToParty(connectionId, partyCommand);
                                    commandsReceived++;
                                    break;
                                }

                            case CommandType.Test:
                                {
                                    commandsReceived++;
                                    break;
                                }

                            case CommandType.None:
                                {
                                    commandsReceived++;
                                    break;
                                }
                        }

                        //Catch exception
                        OnReceived((BaseCommand)o);
                        break;
                    }

                case NetworkEventType.DisconnectEvent:
                    {
                        break;
                    }
            }
        }

        #endregion Host Functionalities

        #region Client Functionalities

        //Inizializzazione Client, richiesta connessione al host.
        //Il client si collega al host all'indirizzo IP e Port definito.
        //Una volta collegato rimane in attesa di avvenuta connessione per poi richiedere la richiesta di partecipazione al "Party"
        public void StartClienting(string hostIP, int port)
        {
            _isServer = false;
            _isStarted = true;
            NetworkTransport.Init();

            this.hostIP = hostIP;
            this.port = port;

            _myIp = GetLocalIPAddress();

            commandsSent = 0;
            commandsReceived = 0;

            _genericHostId = NetworkTransport.AddHost(new HostTopology(_config, 24), 0); //?

            byte error;
            _connectionID = NetworkTransport.Connect(_genericHostId, hostIP, port, 0, out error);
            NetworkError nError = (NetworkError)error;
            if (nError != NetworkError.WrongChannel)
            {
                Debug.LogError(nError.ToString());
                _lastMessageError = nError.ToString();
            }
        }

        //Invio al host, dopo avvenuta connessione, di richiesta inserimento al party
        private void Send_Request_ToParty(int hostID, int receiverConnectionID)
        {
            _hostID = hostID;
            _host_ConnectionID = receiverConnectionID;
            PartyCommand partyCommand = new PartyCommand(_myIp, _connectionID, _host_ConnectionID, true);
            SendCommand(partyCommand);
        }

        private void Client_CheckForNetworkEvents()
        {
            if (!_isStarted) return;

            int recHostId;
            int connectionId;
            int channelId;
            int dataSize;
            int bufferSize = 1024;
            byte error;
            byte[] recBuffer = new byte[1024];

            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

            switch (recData)
            {
                case NetworkEventType.Nothing:
                    {
                        break;
                    }

                case NetworkEventType.ConnectEvent:
                    {
                        Send_Request_ToParty(recHostId, connectionId);
                        _hostID = recHostId;
                        _status = Status.Connected;
                        break;
                    }

                case NetworkEventType.DataEvent:
                    {
                        CommandType commandType;
                        object o = DecodeJsonCommand(recBuffer, out commandType);
                        _lastCommandReceived = commandType;
                        switch (commandType)
                        {
                            case CommandType.Party:
                                {
                                    PartyCommand partyCommand = (PartyCommand)o;
                                    commandsReceived++;
                                    _status = Status.Partying;
                                    break;
                                }
                            case CommandType.Test:
                                commandsReceived++;
                                break;

                            case CommandType.None:
                                commandsReceived++;
                                break;
                        }

                        OnReceived((BaseCommand)o);
                        break;
                    }

                case NetworkEventType.DisconnectEvent:
                    {
                        _lastMessageError = "" + (NetworkError)error;
                        _status = Status.Disconnected;
                        StartClienting(hostIP, port);
                        break;
                    }
            }
        }

        #endregion Client Functionalities

        #region Common Functionalities

        //Inizializzazione impostazioni comuni
        private void Init()
        {
            _config = new ConnectionConfig();
            _communicationChannel = _config.AddChannel(QosType.Reliable);
            _status = Status.Initialized;
        }

        //Invio disconnesione del party/dal party e shutdown del NetworkTransport
        public void Stop()
        {
            if (!_isStarted) return;

            _isStarted = false;
            _isServer = false;

            byte error;
            if (_isServer)
                NetworkTransport.DisconnectNetworkHost(_genericHostId, out error); //Da verificare che evento manda e a chi
            else
                NetworkTransport.Disconnect(_genericHostId, _connectionID, out error); //Da verificare che evento manda e a chi

            NetworkTransport.Shutdown();
            _status = Status.Disconnected;
        }

        #endregion Common Functionalities

        #region General Utilities

        private string GetLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public bool SendCommand(BaseCommand command)
        {
            if (!_isStarted) return false;

            byte error;
            byte[] bytes = Encoding.UTF32.GetBytes(JsonUtility.ToJson(command));
            NetworkTransport.Send(_hostID, command.idReceiver, _communicationChannel, bytes, bytes.Length, out error);

            NetworkError nError = (NetworkError)error;
            if (nError != NetworkError.Ok)
            {
                Debug.LogError(nError.ToString());
                _lastMessageError = nError.ToString() + " HID:" + _hostID + " RID:" + command.idReceiver + " CH" + _communicationChannel;
                NetworkTransport.Send(_hostID, command.idReceiver, _communicationChannel == 0 ? 1 : 0, bytes, bytes.Length, out error); //Remove this
                return false;
            }
            _lastMessageError = nError.ToString() + " HID:" + _hostID + " RID:" + command.idReceiver + " CH" + _communicationChannel;

            _lastCommandSent = command.commandType;
            commandsSent++;

            return true;
        }

        public void Host_SendCommand_ToParty(BaseCommand baseCommand)
        {
            if (!_isStarted) return;
            if (!_isServer) return;

            baseCommand.idSender = _connectionID;
            baseCommand.ipSender = MyIP;

            foreach (int id in _partyIDs)
            {
                byte error;
                byte[] bytes = Encoding.UTF32.GetBytes(JsonUtility.ToJson(baseCommand));
                NetworkTransport.Send(_hostID, id, _communicationChannel, bytes, bytes.Length, out error);

                NetworkError nError = (NetworkError)error;
                if (nError != NetworkError.Ok)
                {
                    Debug.LogError(nError.ToString());
                    _lastMessageError = nError.ToString() + " HID:" + _hostID + " RID:" + id + " CH" + _communicationChannel;
                }
                else
                {
                    _lastCommandSent = baseCommand.commandType;
                    commandsSent++;
                }
            }
        }

        public byte[] EncodeJson(object o)
        {
            return Encoding.UTF32.GetBytes(JsonUtility.ToJson(o));
        }

        public object DecodeJsonCommand(byte[] bytes, out CommandType commandType)
        {
            BaseCommand bCommand = JsonUtility.FromJson<BaseCommand>(Encoding.UTF32.GetString(bytes));
            commandType = bCommand.commandType;

            switch (bCommand.commandType)
            {
                case CommandType.Party:
                    {
                        _lastCommandJson = Encoding.UTF32.GetString(bytes);
                        return JsonUtility.FromJson<PartyCommand>(Encoding.UTF32.GetString(bytes));
                    }
                case CommandType.Vector3:
                    {
                        _lastCommandJson = Encoding.UTF32.GetString(bytes);
                        return JsonUtility.FromJson<Vector3Command>(Encoding.UTF32.GetString(bytes));
                    }

                case CommandType.String:
                    {
                        _lastCommandJson = Encoding.UTF32.GetString(bytes);
                        return JsonUtility.FromJson<StringCommand>(Encoding.UTF32.GetString(bytes));
                    }
            }

            //if None or Test
            return bCommand;
        }

        //Invia un Comando di test, se Server lo invia al party, se Client lo invia al server
        public void SendTestCommand()
        {
            if (!_isStarted) return;

            if (_isServer)
            {
                foreach (int id in _partyIDs)
                    SendCommand(new BaseCommand(CommandType.Test, MyIP, ConnectionID, id));
            }
            else
            {
                SendCommand(new BaseCommand(CommandType.Test, MyIP, ConnectionID, _host_ConnectionID));
            }
        }

        public enum Status
        {
            None,
            Initialized,
            Hosting,

            Connected,
            Partying,

            Disconnected
        }

        private void OnSent(BaseCommand baseCommand)
        {
            if (OnSentCommand != null)
                OnSentCommand(this, new NetworkTransportEventArgs(baseCommand));
        }

        private void OnReceived(BaseCommand baseCommand)
        {
            if (OnReceivedCommand != null)
                OnReceivedCommand(this, new NetworkTransportEventArgs(baseCommand));
        }

        #endregion General Utilities
    }
}