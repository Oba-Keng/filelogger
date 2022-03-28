using System;

namespace FileLoggerKata
{

    public class FileLogger

    {


        private const string LogExtension = "txt";
        private const string LogFileName = "log";
        private const string WeekendLogFileName = "weekend";

        private IDateProvider DateProvider { get; }
        private IFileSystem FileSystem { get; }


        public FileLogger(IFileSystem fileSystem, IDateProvider dateProvider = null)
        {
            DateProvider = dateProvider ?? DefaultDataProvider.Instance;
            FileSystem = fileSystem;


        }


        public void Log(string message)
        {
            var logDate = DateProvider.Now;

            var messageToAppend = $"{logDate:yyyy-MM-dd HH:mm:ss}" + message;

            var logFileName = GetLogFileName();



            if (ShouldRotateWeekendLogs())
            {
                ArchiveWeekendLog();
            }

            if (!FileSystem.Exists(logFileName))
            {
                FileSystem.Create(logFileName);
            }



            FileSystem.Append(logFileName, messageToAppend);

            bool ShouldRotateWeekendLogs()
            {
                return IsWeekend(DateProvider.Now) &&
                       FileSystem.Exists(logFileName) &&
                       (DateProvider.Now - FileSystem.GetLastWriteTime(logFileName)).Days > 2;
            }
        }

        public string GetLogFileName()
        {
            var today = DateProvider.Now;



            return IsWeekend(today)
                ? $"{WeekendLogFileName}.{LogExtension}"
                : $"{LogFileName}{ToFileDateFormat(today)}.{LogExtension}";
        }

        private void ArchiveWeekendLog()
        {
            var lastWriteTime = FileSystem.GetLastWriteTime($"{WeekendLogFileName}.{LogExtension}");

            if (lastWriteTime.DayOfWeek == DayOfWeek.Sunday)
            {
                lastWriteTime = lastWriteTime.AddDays(-1);
            }

            var archivedFileName = $"{WeekendLogFileName}-{ToFileDateFormat(lastWriteTime)}.{LogExtension}";
            FileSystem.Rename($"{WeekendLogFileName}.{LogExtension}", archivedFileName);
        }

        private static string ToFileDateFormat(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");

        }

        private static bool IsWeekend(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;





    }




}

