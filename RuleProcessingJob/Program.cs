using System;
using System.Threading.Tasks;


namespace RuleProcessingJob
{
    class Program
    {
        static void Main(string[] args)
        {
           IFileProcessor processor = new FileProcessor();
           Task t = processor.Process();

            Console.WriteLine("RuleJob Waiting to be finished...");

            Task.WhenAll(t).Wait();
        }
        
    }
}
