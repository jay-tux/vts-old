# VTS - Jay.VTS.XtendIEnumerable
The ``XtendIEnumerable`` class is a static class which adds some methods to the C# ``IEnumerable`` type.
## Extension methods
 - ``public static void ForEach<T>(this IEnumerable<T> container, Action<T> action)``;  
 Extends the ``List<T>.ForEach<T>(Action<T>)`` method to the other ``IEnumerable<T>`` types. Uses the ``foreach(T val in container)`` construction for enumeration.
 - ``public static IEnumerable<T> Slice<T>(this List<T> source, int start)``;  
 Quick slice from index ``start`` to the end of the ``List<T>`` using the ``source.Skip(start).Take(source.Count - start)`` construction.
 - ``public static IEnumerable<T> Slice<T>(this List<T> source, int start, int len)``;  
 Quick slice from index ``start`` with length ``count`` using the ``source.Skip(start).Take(len)`` construction.
