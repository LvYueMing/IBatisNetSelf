using IBatisNetSelf.Common.Utilities.Objects;

namespace TestEmitObject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FactoryBuilder _factoryBuilder = new FactoryBuilder();

            IFactory _factory= _factoryBuilder.CreateFactory(typeof(MyClass), Type.EmptyTypes);

            _factoryBuilder.SaveToDll();

            MyClass _my = (MyClass)_factory.CreateInstance(null);


            Console.WriteLine($"{_my.id}  {_my.name}") ;
            Console.ReadKey();
        }
    }

    public class MyClass
    {
        public string id { set; get; }
        public string name { set; get; }


        public MyClass()
        {
            id = Guid.NewGuid().ToString();
            name = "Test";
        }

        public MyClass(string id) 
        {
            id = Guid.NewGuid().ToString();
            name = "Test";
        }
    }
}
