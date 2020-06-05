# VTS - Jay.Logging
The preprocessor-driven logging system; when no flags are defined, the compiled code for this class (and whole file) is almost nothing. Depending on which flags are defined in the preprocessor (before compiling VTS), certain messages are shown or skipped.
## Flags
 - ``EXECUTION``, for execution related messages,
 - ``DEBUG``, for debugging messages (way too much messages to be really useful, only good when searching for a crash reason),
 - ``MESSAGE``, for generic messages,
 - ``PARSING``, for parsing related messages,
 - ``WARNING``, for warning messages,
 - ``STRUTURAL``, (*yes, typo in the code*) for structural messages (parsed code structure).

## Enum
The ``LogType`` enum defines all types of messages which can be used. All but one of these types relate to the flags, and the ``NOIGNORE`` type is always shown (that is, if the ``VERBOSE`` flag is set).

## Logger Class
The Logger class contains two methods and one field. Refactoring should make this class ``static``.

### Logger Fields
 - ``public static bool Enabled``;  
 Is the Logger enabled (field only exists when the ``VERBOSE`` flag is set).

### Logger Methods
 - ``public static void Log(string message, LogType type)``;  
 If the ``VERBOSE`` flag and the flag corresponding to the Logtype are set, logs the given message to stdout.
 - ``public static void Log(object message, LogType type)``;  
 Calls the other Log method with the string representation of the message.
