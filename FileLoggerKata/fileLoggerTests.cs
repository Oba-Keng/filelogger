using System;
using Xunit;
using Moq;



namespace FileLoggerKata
{

    public class FileLoggerKataTests //Class for all FileLogger tests
    {

        private FileLogger filelogger { get; } //To Access filelogger class being tested

        private string message = "well.txt"; //Test string

        public string messageAppendToFile => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}" + message;

        private string weekendLogFile = "weekend";

        private Mock<IDateProvider> DateProvider { get; }//Gives access to IDaterovider interface and properties

        private Mock<IFileSystem> IFileSystem { get; }//Gives access to IFileSystem interface and properties







        [Fact]
        public void validateLog()
        {
            var mockIDateProvider = new Mock<IDateProvider>();
            // filelogger.Log("Hello");
            // filelogger.Log(messageAppendToFile);
            Console.WriteLine(mockIDateProvider);
            Assert.Contains("well", "this is it");
            // Assert.IsTrue(filelogger);
        }
    }

}