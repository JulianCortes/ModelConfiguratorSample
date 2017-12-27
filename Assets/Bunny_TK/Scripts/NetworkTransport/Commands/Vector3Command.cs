using UnityEngine;

namespace Bunny_TK.Net
{
    public class Vector3Command : BaseCommand
    {
        public string name;
        public Vector3 targetPosition = Vector3.zero;

        public Vector3Command()
           : base(CommandType.Vector3, "127.0.0.1", -1, -1)
        {
            name = "";
            targetPosition = Vector3.zero;
        }

        public Vector3Command(string name, Vector3 targetPosition) : this()
        {
            this.name = name;
            this.targetPosition = targetPosition;
        }
    }
}