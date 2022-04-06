- Project tested using Xunit and Moq

# References

## https://pnguyen.io/posts/verify-ilogger-call-in-dotnet-core/

## https://im5tu.io/article/2020/11/using-moq-with-ref-in-and-out-parameters-in-c/

## https://medium.com/@HansikaWanniarachchi/get-started-net-core-application-unit-testing-with-xunit-and-moq-83c7ee0d15f3

## https://daedtech.com/introduction-to-unit-testing-part-6-test-doubles/

## https://makolyte.com/event-driven-dotnet-how-to-unit-test-code-that-depends-on-an-event/

# File logger

## What does the application do?

The application has a class 'FileLogger' with a method, Log(string message).

When this method is called, it should append the message to the end of a file, "log.txt", located in the same folder as the running application (or tests).

Messages should default to having a prefix of "YYYY-MM-DD HH:MM:SS " in front of the message. For example, Log("test") would yield "2020-07-14 10:25:23 test". Time can be in local timezone and should use 24-hour clock.

If the file doesn't exist, create it. If it does exist, use it and append to it.

Now update the method so that it writes to a file called logYYYYMMDD.txt, where YYYYMMDD corresponds to the current date. Note: It's not unusual for the logger instance to run for a long period of time, and the log messages should go into the appropriate file based on when they are logged, not when the logger was created.

Verify that a new file is created if it doesn't exist on each new day.

The IT manager doesn't want to have to open multiple files on Mondays. Any time logging is occurring on a Saturday or Sunday, have it log to a file called "weekend.txt". If it already exists, it can just append to it.

Actually, the manager just gave us new requirements. The first time you log to a file on a new weekend, make sure you start with a fresh "weekend.txt" file. Rename the old one based on the date of the Saturday of that weekend, e.g. weekend-YYYYMMDD.txt.

## Example of final weekend behavior

Let's say the month starts on Saturday the 1st (e.g. 1 Feb 2020) and currently there are no log files present. Logging on Saturday the 1st goes to a file weekend.txt. Logging on Sunday the 2nd continues to go to weekend.txt. Throughout the week, new files are created for each date log20200203.txt, log20200204.txt, etc. Then Saturday 8 Feb 2020 rolls around and the last requirements comes into play. The existing weekend.txt file has metadata indicating it was created or last modified on Feb 1st/2nd (respectively). Thus, the logger renames the file to weekend-20200201.txt corresponding to the Saturday of that weekend log file. It then creates a new weekend.txt which is used for the rest of the 8th. And again on the 9th. That file will be renamed on 15 Feb to weekend-20200208.txt.

## Note

An existing weekend.txt file isn't necessarily from the previous weekend (6 days ago). It could be that nothing was logged for several weeks. It's necessary to inspect the file to see when it was created/lastmodified to know what it should be renamed to.

## The Task

Write Unit Tests for the File Logger class to ensure the class behaves as described above.

### Hint

You will need to use test doubles in order to test certain scenarios.
