using System;
using Xunit;
using Moq;



namespace FileLoggerKata
{

    public class FileLoggerKataTests //Class for all FileLogger tests
    {

        private FileLogger filelogger { get; } //To Access filelogger class being tested

        private const string messagetoTest = "success"; //Test message

        private const string logExtension = "txt";

        private string weekendLogFile => $"weekend.{logExtension}";//weekend file with extension

        private readonly DateTime Saturday = new DateTime(2022, 4, 2);//Creating date and time for Saturday
        private readonly DateTime Sunday = new DateTime(2022, 4, 3);//Creating date and time for Sunday

        private DateTime now => new DateTime(2022, 3, 28);//Creating date and time for during the week


        private string appendMessageWeek => $"{now:yyyy-MM-dd HH:mm:ss}" + messagetoTest;

        private string appendMessageSaturday => $"{Saturday:yyyy-MM-dd HH:mm:ss}" + messagetoTest;

        private string appendMessageSunday => $"{Sunday:yyyy-MM-dd HH:mm:ss}" + messagetoTest;

        private string logFileName_test => $"log{now:yyyyMMdd}.{logExtension}";



        private Mock<IDateProvider> mockDateProvider { get; }//Gives access to IDateProvider interface and properties

        private Mock<IFileSystem> mockFileSystem { get; }//Gives access to IFileSystem interface and properties



        public FileLoggerKataTests()//Mocks for IDateprovider and IFileSystem actions
        {
            mockDateProvider = new Mock<IDateProvider>(MockBehavior.Strict);

            mockDateProvider.Setup(day => day.Today).Returns(now);

            mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);

            //setup for FileSystem methods
            mockFileSystem.Setup(file => file.Append(It.IsNotNull<string>(), It.IsNotNull<string>()));
            mockFileSystem.Setup(file => file.Create(It.IsNotNull<string>()));
            mockFileSystem.Setup(file => file.Exists(It.IsNotNull<string>())).Returns(true);
            mockFileSystem.Setup(file => file.GetLastWriteTime(It.IsNotNull<string>())).Returns(DateTime.Now);
            mockFileSystem.Setup(file => file.Rename(It.IsNotNull<string>(), It.IsNotNull<string>()));

            //mock objects for filelogger
            filelogger = new FileLogger(mockFileSystem.Object, mockDateProvider.Object);




        }

        [Fact]

        public void appendsMessage()//Checks if message is appended correctly
        {
            filelogger.Log(messagetoTest);

            mockFileSystem.Verify(file => file.Append(logFileName_test, appendMessageWeek), Times.Once);


        }


        [Fact]
        public void validateLogFile()//Check for file
        {
            mockFileSystem.Setup(file => file.Exists(logFileName_test)).Returns(false);

            filelogger.Log(messagetoTest);

            mockFileSystem.Verify(file => file.Exists(logFileName_test), Times.Once);

            mockFileSystem.Verify(file => file.Create(logFileName_test), Times.Once);

            mockFileSystem.Verify(file => file.Append(logFileName_test, appendMessageWeek), Times.Once);

        }
        [Fact]
        public void logFileNotFound()
        {
            mockFileSystem.Setup(file => file.Exists(logFileName_test)).Returns(true);

            filelogger.Log(messagetoTest);

            mockFileSystem.Verify(file => file.Exists(logFileName_test), Times.Once);

            mockFileSystem.Verify(file => file.Create(logFileName_test), Times.Never);

            mockFileSystem.Verify(file => file.Append(logFileName_test, appendMessageWeek), Times.Once, "not appended");

        }

        [Fact]
        public void sundayLogCheck()
        {
            mockFileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(true);

            mockDateProvider.Setup(day => day.Today).Returns(Sunday);

            filelogger.Log(messagetoTest);

            mockFileSystem.Verify(file => file.Exists(weekendLogFile), Times.AtLeastOnce);

            mockFileSystem.Verify(file => file.Create(weekendLogFile), Times.Never);

            mockFileSystem.Verify(file => file.Append(weekendLogFile, appendMessageSunday), Times.Once, "not appended");
        }

        [Fact]
        public void saturdayLogCheck()
        {
            mockFileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(false);

            mockDateProvider.Setup(day => day.Today).Returns(Saturday);

            filelogger.Log(messagetoTest);

            mockDateProvider.VerifyGet(day => day.Today, Times.AtLeastOnce, "not fetched");

            mockFileSystem.Verify(file => file.Exists(weekendLogFile), Times.AtLeastOnce);

            mockFileSystem.Verify(file => file.Create(weekendLogFile), Times.Once);

            mockFileSystem.Verify(file => file.Append(weekendLogFile, appendMessageSaturday), Times.Once, "not appended");
        }

        [Fact]

        //checks for weekend file
        public void sundayLogAppends()
        {
            mockFileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(false);

            mockDateProvider.Setup(day => day.Today).Returns(Sunday);

            filelogger.Log(messagetoTest);

            mockDateProvider.VerifyGet(day => day.Today, Times.AtLeastOnce, "not fetched");


            mockFileSystem.Verify(file => file.Exists(weekendLogFile), Times.AtLeastOnce);

            mockFileSystem.Verify(file => file.Create(weekendLogFile), Times.Once);

            mockFileSystem.Verify(file => file.Append(weekendLogFile, appendMessageSunday), Times.Once, "not appended");
        }




    }

}