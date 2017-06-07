namespace TestOpenTK
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.Instance.Run(60.0, 60.0); // Updates and renders frames 60 times a second
        }
    }
}
