using System;
using Xunit;
using Moq;



namespace FileLoggerKata
{

    public class FileLoggerKataTests //Class for all FileLogger tests
    {

        private FileLogger filelogger { get; } //To Access filelogger class being tested

        private string message = "well"; //Test message


        private string weekendLogFile = $"weekend.{LogExtension}";//weekend with extension

        private readonly DateTime Saturday = new DateTime(2022, 03, 26);//Creating date and time for Saturday
        private readonly DateTime Sunday = new DateTime(2022, 03, 27);//Creating date and time for Sunday

        private DateTime Today => new DateTime(2022, 03, 27);//Creating date and time for during the week


        public string appendMessageWeek => $"{Today:YYYY-MM-dd HH:MM:SS}" + message;

        public string appendMessageSaturday => $"{Saturday:YYYY-MM-dd HH:MM:SS}" + message;

        public string appendMessageSunday => $"{Sunday:YYYY-MM-dd HH:MM:SS}" + message;

        private const string LogExtension = "txt";
        public string logFileName_test => $"{Today:YYYY-MM-dd HH:MM:SS}.{LogExtension}" + message;



        private Mock<IDateProvider> DateProvider { get; }//Gives access to IDateProvider interface and properties

        private Mock<IFileSystem> FileSystem { get; }//Gives access to IFileSystem interface and properties



        public FileLoggerKataTests()//Mocks for IDateprovider and IFileSystem actions
        {
            DateProvider = new Mock<IDateProvider>();

            DateProvider.Setup(day => day.Today).Returns(Today);

            FileSystem = new Mock<IFileSystem>();

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

            FileSystem.Verify(file => file.Exists(logFileName_test));

            FileSystem.Verify(file => file.Create(logFileName_test));

            FileSystem.Verify(file => file.Append(logFileName_test, appendMessageWeek));

        }
        [Fact]
        public void LogFileNotFound()
        {
            FileSystem.Setup(file => file.Exists(logFileName_test)).Returns(true);

            filelogger.Log(message);

            FileSystem.Verify(file => file.Exists(logFileName_test));

            FileSystem.Verify(file => file.Create(logFileName_test));

            FileSystem.Verify(file => file.Append(logFileName_test, appendMessageWeek), "not appended");

        }


    }

}