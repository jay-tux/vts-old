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
    }
}