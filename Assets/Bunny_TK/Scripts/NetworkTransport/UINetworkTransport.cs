using UnityEngine;
using UnityEngine.UI;
using Bunny_TK.Net;

namespace Ferrari.Demo
{
    public class UINetworkTransport : MonoBehaviour
    {
        #region Fields

        [Header("General")]
        [SerializeField]
        private bool isVisible = false;

        #region Network

        [Header("Network UI")]
        public NetworkTransportManager NetworkManager;

        [SerializeField]
        private GameObject network_Panel;

        [SerializeField]
        private InputField hostIp_Input;

        [SerializeField]
        private InputField port_Input;

        [Header("Network UI: Texts")]
        [SerializeField]
        private Text myIP_txt;

        [SerializeField]
        private Text status_txt;

        [SerializeField]
        private Text lastErrorMessage_txt;

        [SerializeField]
        private Text lastMessage_sent_txt;

        [SerializeField]
        private Text lastMessage_received_txt;

        [SerializeField]
        private Text countMessage_sent_txt;

        [SerializeField]
        private Text countMessage_received_txt;

        [SerializeField]
        private Text lastCommandJson_received_txt;

        #endregion Network

        #endregion Fields

        #region Props

        public bool IsVisible { get { return isVisible; } }

        #endregion Props

        #region Unity Stuff

        private void Start()
        {
            if (NetworkManager == null) NetworkManager = FindObjectOfType<NetworkTransportManager>();
            HideAllUI();
        }

        private void Update()
        {
            if (!isVisible) return;

            if (network_Panel.activeSelf)
            {
                myIP_txt.text = NetworkManager.MyIP;
                status_txt.text = "" + NetworkManager.CurrentStatus;

                countMessage_sent_txt.text = "" + NetworkManager.CommandsSent;
                countMessage_received_txt.text = "" + NetworkManager.CommandsReceived;

                lastMessage_received_txt.text = "" + NetworkManager.LastCommandReceived;
                lastMessage_sent_txt.text = "" + NetworkManager.LastCommandSent;

                if (lastErrorMessage_txt != null)
                    lastErrorMessage_txt.text = NetworkManager.LastMessageError;

                if (lastCommandJson_received_txt != null)
                    lastCommandJson_received_txt.text = NetworkManager.LastCommandJson;
            }
        }

        #endregion Unity Stuff

        #region NetworkUI

        public void SetVisibleUI_Network(bool isVisible)
        {
            //Should hide others
            this.isVisible = isVisible;
            network_Panel.SetActive(isVisible);
        }

        public void Client_StartClienting()
        {
            NetworkManager.StartClienting(hostIp_Input.text, int.Parse(port_Input.text));
        }

        public void Host_StartHosting()
        {
            //NetworkManager.StartHosting(int.Parse(port_Input.text));
            NetworkManager.StartHosting(7075);
        }

        public void Stop()
        {
            NetworkManager.Stop();
        }

        public void ToggleNetworkUI()
        {
            SetVisibleUI_Network(!isVisible);
        }

        #endregion NetworkUI

        #region Common

        public void SendTestCommand()
        {
            NetworkManager.SendTestCommand();
        }

        public void HideAllUI()
        {
            isVisible = false;
            network_Panel.SetActive(false);
        }

        public void LoadSettings(string hostIP, int port)
        {
            hostIp_Input.text = hostIP;
            port_Input.text = "" + port;
        }

        #endregion Common
    }
}