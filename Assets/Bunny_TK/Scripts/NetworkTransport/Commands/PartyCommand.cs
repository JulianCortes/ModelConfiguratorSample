using System;

namespace Bunny_TK.Net
{
    [Serializable]
    public class PartyCommand : BaseCommand
    {
        public bool isRequest;

        public PartyCommand(string ipSender, int idSender, int idReceiver, bool isRequest)
            : base(CommandType.Party, ipSender, idSender, idReceiver)
        {
            this.isRequest = isRequest;
        }
    }
}