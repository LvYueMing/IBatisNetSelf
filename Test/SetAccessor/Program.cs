using IBatisNetSelf.Common.Utilities.Objects.Members;

namespace SetAccessor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SetAccessorFactory factory = new SetAccessorFactory(true);

            //for (int i = 0; i < 10000; i++)
            //{
            //    ISetAccessor accessor = factory.CreateSetAccessor(typeof(TestClass), "id");
            //    accessor.Set(new TestClass(), "123");
            //}

            ISetAccessor accessor = factory.CreateSetAccessor(typeof(TestClass), "id");

            object obj = new TestClass();

            accessor.Set(obj, "123");

            Console.WriteLine(((TestClass)obj).id);

            Console.ReadKey();
        }
    }

    public class TestClass
    {
        public string id { set; get; }
        public string name { set; get; }


        public TestClass()
        {
            id = Guid.NewGuid().ToString();
            name = "Test";
        }

        public TestClass(string id)
        {
            id = Guid.NewGuid().ToString();
            name = "Test";
        }
    }
}
