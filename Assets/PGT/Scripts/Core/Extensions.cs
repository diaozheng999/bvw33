using System.Collections.Generic;
using UnityEngine;

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

        public static Transform FindRec(this Transform transform, string child){
            var n = transform.childCount;
            var attempt = transform.Find(child);
            if (attempt != null) return attempt;
            for(int i=0; i<n; ++i){
                attempt = transform.GetChild(i).FindRec(child);
                if(attempt != null) return attempt;
            }
            return null;
        }
    }
}
