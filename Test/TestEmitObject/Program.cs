using IBatisNetSelf.Common.Utilities.Objects;

namespace TestEmitObject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FactoryBuilder _factoryBuilder = new FactoryBuilder();

            IFactory _factory= _factoryBuilder.CreateFactory(typeof(MyClass), new Type[] { typeof(string)});

            _factoryBuilder.SaveDll();

            MyClass _my = (MyClass)_factory.CreateInstance(new object[] {"1" });


            Console.WriteLine($"{_my.id}  {_my.name}") ;
            Console.ReadKey();
        }
    }

    public class MyClass
    {
        public string id { set; get; }
        public string name { set; get; }

        public MyClass(string id) 
        {
            id = Guid.NewGuid().ToString();
            name = "Test";
        }
    }
}
