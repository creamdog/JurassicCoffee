namespace JurassicCoffee.Console
{
    class Program
    {
        static void Main(string[] args)
        {

           new Core.Compiler().Compile("test.coffee");

        }
    }
}
