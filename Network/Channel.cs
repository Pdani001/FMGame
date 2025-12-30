namespace ReFMGame.Network
{
    public class Channel
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public string[] Clients { get; set; }
    }
}
