using System;
using Xunit;
using Moq;



namespace FileLoggerKata
{

    public class FileLoggerKataTests //Class for all FileLogger tests
    {

        private FileLogger filelogger { get; } //To Access filelogger class and its methods

        private const string messageToAppend = " this is the message appended to end of file";
        private string messagetoTest => $"{now:yyyy-MM-dd HH:mm:ss}" + messageToAppend; //Test message with date 

        private const string logExtension = "txt";

        private string weekendLogFile => $"weekend.{logExtension}";//weekend file with extension

        private static readonly DateTime Saturday = new DateTime(2022, 4, 2, 22, 06, 36);//Creating date and time for Saturday
        private static readonly DateTime Sunday = new DateTime(2022, 4, 3, 16, 15, 40);//Creating date and time for Sunday

        private string messageToAppendSaturday => $"{Saturday:yyyy-MM-dd HH:mm:ss}" + messageToAppend;

        private string messageToAppendSunday => $"{Sunday:yyyy-MM-dd HH:mm:ss}" + messageToAppend;


        private DateTime now => new DateTime(2022, 3, 28, 23, 33, 33);//Creating date and time for during the week

        private string logFileName_test => $"log{now:yyyyMMdd}.{logExtension}";

        private Mock<IDateProvider> mockDateProvider { get; }//Gives access to IDateProvider interface and properties

        private Mock<IFileSystem> mockFileSystem { get; }//Gives access to IFileSystem interface and properties

        private DateTime passedDate => new DateTime(2022, 03, 26);

        private string weekendMessage = "weekend";
        public FileLoggerKataTests()//Mocks for IDateprovider and IFileSystem actions
        {
            mockDateProvider = new Mock<IDateProvider>(MockBehavior.Strict);

            mockDateProvider.Setup(day => day.Now).Returns(now);

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

            mockFileSystem.Verify(file => file.Append(logFileName_test, messagetoTest), Times.Once);


        }

        [Fact]
        public void validateLogFile()//Check if log.txt file exists,if not creates and appends to it
        {
            mockFileSystem.Setup(file => file.Exists(logFileName_test)).Returns(false);

            filelogger.Log(messagetoTest);

            mockFileSystem.Verify(file => file.Exists(logFileName_test), Times.Once);

            mockFileSystem.Verify(file => file.Create(logFileName_test), Times.Once);

            mockFileSystem.Verify(file => file.Append(logFileName_test, messagetoTest), Times.Once);

        }

        [Fact]
        public void logFileNotFound()
        {
            mockFileSystem.Setup(file => file.Exists(logFileName_test)).Returns(true);

            filelogger.Log(messagetoTest);

            mockFileSystem.Verify(file => file.Exists(logFileName_test), Times.Once);

            mockFileSystem.Verify(file => file.Create(logFileName_test), Times.Never);

            mockFileSystem.Verify(file => file.Append(logFileName_test, messagetoTest), Times.Once, "not appended");

        }

        [Fact]
        public void sundayLogCheck()
        {
            mockFileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(true);

            mockDateProvider.Setup(day => day.Now).Returns(Sunday);

            filelogger.Log(messageToAppendSunday);

            mockFileSystem.Verify(file => file.Exists(weekendLogFile), Times.AtLeastOnce);

            mockFileSystem.Verify(file => file.Create(weekendLogFile), Times.Never);

            mockFileSystem.Verify(file => file.Append(weekendLogFile, messageToAppendSunday), Times.Once, "not appended");
        }

        [Fact]
        public void saturdayLogCheck()
        {
            mockFileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(false);

            mockDateProvider.Setup(day => day.Now).Returns(Saturday);

            filelogger.Log(messageToAppendSaturday);

            mockDateProvider.VerifyGet(day => day.Now, Times.AtLeastOnce, "not fetched");

            mockFileSystem.Verify(file => file.Exists(weekendLogFile), Times.AtLeastOnce);

            mockFileSystem.Verify(file => file.Create(weekendLogFile), Times.Once);

            mockFileSystem.Verify(file => file.Append(weekendLogFile, messageToAppendSaturday), Times.Once, "not appended");
        }

        [Fact]
        public void sundayLogAppends()//checks for weekend file
        {
            mockFileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(false);

            mockDateProvider.Setup(day => day.Now).Returns(Sunday);

            filelogger.Log(messageToAppendSunday);

            mockDateProvider.VerifyGet(day => day.Now, Times.AtLeastOnce, "not fetched");


            mockFileSystem.Verify(file => file.Exists(weekendLogFile), Times.AtLeastOnce);

            mockFileSystem.Verify(file => file.Create(weekendLogFile), Times.Once);

            mockFileSystem.Verify(file => file.Append(weekendLogFile, messageToAppendSunday), Times.Once, "not appended");
        }

        [Fact]
        public void logsToWeekend()
        {
            mockDateProvider.Setup(day => day.Now).Returns(Sunday);

            filelogger.Log(messageToAppendSunday);

            mockDateProvider.VerifyGet(day => day.Now, Times.AtLeastOnce);

            mockFileSystem.Verify(file => file.Append(weekendLogFile, messageToAppendSunday), Times.Once);
        }

        [Fact]

        public void weekendLogFileSaturday()
        {
            var passedDateLogFile = $"{weekendMessage}-{passedDate:yyyyMMdd}.{logExtension}";

            mockDateProvider.Setup(day => day.Now).Returns(Saturday);

            mockFileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(true);

            mockFileSystem.Setup(file => file.GetLastWriteTime(weekendLogFile)).Returns(passedDate);

            filelogger.Log(messageToAppendSaturday);

            mockFileSystem.Verify(file => file.Rename(weekendLogFile, passedDateLogFile), Times.Once);

            mockFileSystem.Verify(file => file.Append(weekendLogFile, messageToAppendSaturday), Times.Once);


        }

        [Fact]

        public void weekendLogFileSunday()
        {
            var passedDateLogFile = $"{weekendMessage}-{passedDate:yyyyMMdd}.{logExtension}";

            mockDateProvider.Setup(day => day.Now).Returns(Sunday);

            mockFileSystem.Setup(file => file.Exists(weekendLogFile)).Returns(true);

            mockFileSystem.Setup(file => file.GetLastWriteTime(weekendLogFile)).Returns(passedDate);

            filelogger.Log(messageToAppendSunday);

            mockFileSystem.Verify(file => file.Rename(weekendLogFile, passedDateLogFile), Times.Once);

            mockFileSystem.Verify(file => file.Append(weekendLogFile, messageToAppendSunday), Times.Once);


        }

    }

}