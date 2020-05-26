using System;
using System.Linq;
using System.Collections.Generic;

namespace Jay.Xtend
{
    public static class XtendIEnumerable
    {
        public static void ForEach<T>(this IEnumerable<T> container, Action<T> action) 
        {
            foreach(T val in container) {
                action(val);
            }
        }   

        public static IEnumerable<T> Slice<T>(this List<T> source, int start)
            => source.Skip(start).Take(source.Count - start);

        public static IEnumerable<T> Slice<T>(this List<T> source, int start, int len) 
            => source.Skip(start).Take(len);
    }
}