using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.Common.Utilities.Objects.Members;

namespace EmitDllSave
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region ObjectFactory
            //FactoryBuilder _factoryBuilder = new FactoryBuilder();

            //IFactory _factory = _factoryBuilder.CreateFactory(typeof(TestClass), Type.EmptyTypes);

            //TestClass _object = (TestClass)_factory.CreateInstance(null);

            //Console.WriteLine($"{_object.Id}  {_object.Name}");

            // 保存动态程序集到磁盘，即保持为dll，方便反编译观察程序
            //var _generator = new Lokad.ILPack.AssemblyGenerator();

            //var _assembly = _factoryBuilder.ModuleBuilder.Assembly;

            //_generator.GenerateAssembly(_assembly, AppDomain.CurrentDomain.BaseDirectory + _factoryBuilder.ModuleBuilder.Assembly.GetName().Name + ".dll");

            #endregion


            #region SetAccessorFactory
            SetAccessorFactory _factory = new SetAccessorFactory(true);
            ISetAccessor _accessor =_factory.CreateSetAccessor(typeof(TestClass),"Id");

            TestClass _obj = new TestClass();

            _accessor.Set(_obj, "10");

            Console.WriteLine(_obj.Id);


            // 保存动态程序集到磁盘，即保持为dll，方便反编译观察程序

            var _generator = new Lokad.ILPack.AssemblyGenerator();

            var _assembly = _factory.ModuleBuilder.Assembly;

            _generator.GenerateAssembly(_assembly, AppDomain.CurrentDomain.BaseDirectory + _factory.ModuleBuilder.Assembly.GetName().Name + ".dll");

            #endregion

            Console.ReadKey();
        }

    }
    public class TestClass
    {
        public string Id { set; get; }
        public string Name { set; get; }


        public TestClass()
        {
            Id = Guid.NewGuid().ToString();
            Name = "Test";
        }
    }
}