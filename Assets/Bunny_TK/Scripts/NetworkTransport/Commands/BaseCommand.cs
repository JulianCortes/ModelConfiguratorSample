namespace Bunny_TK.Net
{
    [System.Serializable]
    public class BaseCommand
    {
        public CommandType commandType = CommandType.None;
        public string ipSender;
        public int idSender;
        public int idReceiver;

        public BaseCommand(CommandType commandType, string ipSender, int idSender, int idReceiver)
        {
            this.commandType = commandType;
            this.ipSender = ipSender;
            this.idSender = idSender;
            this.idReceiver = idReceiver;
        }
    }

    [System.Serializable]
    public enum CommandType
    {
        None = 0,
        Test = 1,
        Party = 2,

        String,

        Vector3,
    }
}