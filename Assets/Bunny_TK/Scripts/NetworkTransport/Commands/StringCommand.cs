namespace Bunny_TK.Net
{
    public class StringCommand : BaseCommand
    {
        public string name;
        public string value;

        public StringCommand(string name, string value)
            : base(CommandType.String, "127.0.0.1", -1, -1)
        {
            this.name = name;
            this.value = value;
        }
    }
}