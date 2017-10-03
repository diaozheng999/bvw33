using System.Collections.Generic;

namespace PGT.Core
{
    public static class Extensions
    {
        /// <summary>
        /// Returns a reversed iterator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> Rev<T>(this IList<T> list)
        {
            int count = list.Count;
            for(int i=count-1; i>=0; i--)
            {
                yield return list[i];
            }
        }

    }
}
