using IBatisNetSelf.Common.Logging;

namespace TestLog
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            logger.Info("Test");

            Console.ReadLine();
        }
    }
}