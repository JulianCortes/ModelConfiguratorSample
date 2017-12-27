using System;

namespace Bunny_TK.Net
{
    public class NetworkTransportEventArgs : EventArgs
    {
        public BaseCommand baseCommand;

        public NetworkTransportEventArgs(BaseCommand baseCommand)
            : base()
        {
            this.baseCommand = baseCommand;
        }
    }
}