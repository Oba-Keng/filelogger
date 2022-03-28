using System;
using Xunit;
using Moq;



namespace FileLoggerKata
{

    public class FileLoggerKataTests //Class for all FileLogger tests
    {

        private FileLogger filelogger { get; } //To Access filelogger class being tested

        private const string message = "success"; //Test message

        private const string logExtension = "txt";

        private string weekendLogFile => $"weekend.{logExtension}";//weekend with extension

        private readonly DateTime Saturday = new DateTime(2022, 03, 26);//Creating date and time for Saturday
        private readonly DateTime Sunday = new DateTime(2022, 03, 27);//Creating date and time for Sunday

        private DateTime Now => new DateTime(2022, 03, 28);//Creating date and time for during the week


        public string appendMessageWeek => $"{Now:yyyy-MM-dd HH:mm:ss} " + message;

        public string appendMessageSaturday => $"{Saturday:yyyy-MM-dd HH:mm:ss} " + message;

        public string appendMessageSunday => $"{Sunday:yyyy-MM-dd HH:mm:ss} " + message;

        public string logFileName_test => $"log{Now:yyyyMMdd}.{logExtension}";



        private Mock<IDateProvider> DateProvider { get; }//Gives access to IDateProvider interface and properties

        private Mock<IFileSystem> FileSystem { get; }//Gives access to IFileSystem interface and properties



        public FileLoggerKataTests()//Mocks for IDateprovider and IFileSystem actions
        {
            DateProvider = new Mock<IDateProvider>(MockBehavior.Strict);

            DateProvider.Setup(day => day.Now).Returns(Now);

            FileSystem = new Mock<IFileSystem>(MockBehavior.Strict);

            //setup for FileSystem methods
            FileSystem.Setup(file => file.Exists(It.IsNotNull<string>())).Returns(true);
            FileSystem.Setup(file => file.Create(It.IsNotNull<string>()));
            FileSystem.Setup(file => file.Append(It.IsNotNull<string>(), It.IsNotNull<string>()));
            FileSystem.Setup(file => file.GetLastWriteTime(It.IsNotNull<string>())).Returns(DateTime.Now);
            FileSystem.Setup(file => file.Rename(It.IsNotNull<string>(), It.IsNotNull<string>()));

            //mock objects for filelogger
            filelogger = new FileLogger(FileSystem.Object, DateProvider.Object);




        }

        [Fact]

        public void appendsMessage()
        {
            filelogger.Log(message);

            FileSystem.Verify(file => file.Append(logFileName_test, appendMessageWeek), Times.Once, "Error");

        }


        [Fact]
        public void validateLogFile()
        {
            FileSystem.Setup(file => file.Exists(logFileName_test)).Returns(false);

            filelogger.Log(message);

            FileSystem.Verify(file => file.Exists(logFileName_test), Times.Once);

            FileSystem.Verify(file => file.Create(logFileName_test), Times.Once);

            FileSystem.Verify(file => file.Append(logFileName_test, appendMessageWeek), Times.Once);

        }
        [Fact]
        public void logFileNotFound()
        {
            FileSystem.Setup(file => file.Exists(logFileName_test)).Returns(true);

            filelogger.Log(message);

            FileSystem.Verify(file => file.Exists(logFileName_test), Times.Once);

            FileSystem.Verify(file => file.Create(logFileName_test), Times.Never);

            FileSystem.Verify(file => file.Append(logFileName_test, appendMessageWeek), Times.Once, "not appended");

        }

        [Fact]
        public void sundayLogCheck()
        {
            FileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(true);

            DateProvider.Setup(day => day.Now).Returns(Sunday);

            filelogger.Log(message);

            FileSystem.Verify(file => file.Exists(weekendLogFile), Times.AtLeastOnce);

            FileSystem.Verify(file => file.Create(weekendLogFile), Times.Never);

            FileSystem.Verify(file => file.Append(weekendLogFile, appendMessageSunday), Times.Once, "not appended");
        }

        [Fact]
        public void saturdayLogCheck()
        {
            FileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(false);

            DateProvider.Setup(day => day.Now).Returns(Saturday);

            filelogger.Log(message);

            FileSystem.Verify(file => file.Exists(weekendLogFile), Times.AtLeastOnce);

            FileSystem.Verify(file => file.Create(weekendLogFile), Times.Once);

            FileSystem.Verify(file => file.Append(weekendLogFile, appendMessageSaturday), Times.Once, "not appended");
        }




    }

}