﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace LinqTools {
	
	public static class Enumerable {
		
		public static int Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, int> selector) {
			int sum = 0;
	
			foreach (TSource obj in source) {
				sum += selector(obj);
			}
	
			return sum;
		} 
	
		public static float Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector) {
			float sum = 0;
	
			foreach (TSource obj in source) {
				sum += selector(obj);
			}
	
			return sum;
		} 
	
		public static TResult Aggregate<TSource, TResult> (this IEnumerable<TSource> source, TResult seed, Func<TResult, TSource, TResult> aggregator) {
			TResult result = seed;
	
			foreach (TSource obj in source) {
				result = aggregator(result, obj);
			}
	
			return result;
		} 
	
		public static TResult Aggregate<TSource, TResult> (this IEnumerable<TSource> source, Func<TResult, TSource, TResult> aggregator) {
			TResult result = default(TResult);
	
			foreach (TSource obj in source) {
				result = aggregator(result, obj);
			}
	
			return result;
		}
	
		public static IEnumerable<TSource> Where<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			List<TSource> result = new List<TSource>();
	
			foreach (TSource obj in source) {
				if (predicate(obj))
					result.Add(obj);
			}
	
			return result;
		}
	
		public static List<TSource> ToList<TSource> (this IEnumerable<TSource> source) {
			List<TSource> result = new List<TSource>();
	
			foreach (TSource obj in source) {
				result.Add(obj);
			}
	
			return result;
		}
	
		public static TSource[] ToArray<TSource> (this IEnumerable<TSource> source) {
			List<TSource> list = source.ToList();
			TSource[] result = new TSource[list.Count];
	
			for (int i = 0; i != result.Length; i++) {
				result[i] = list[i];
			}
	
			return result;
		}
	
		public static TSource First<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			return source.Where(predicate).First();
		}
	
		public static TSource First<TSource> (this IEnumerable<TSource> source) {
			IList<TSource> list = source as IList<TSource>;
	
			if (list == null) {
				throw new Exception("IList == null!");
			}
	
			if (list.Count == 0) {
				throw new Exception("No elements satisfying the predicate!");
			}
	
			return list[0];
		}
	
		public static TSource FirstOrDefault<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			return source.Where(predicate).FirstOrDefault();
		}
	
		public static TSource FirstOrDefault<TSource> (this IEnumerable<TSource> source) {
			IList<TSource> list = source as IList<TSource>;
	
			if (list == null) {
				throw new Exception("IList == null!");
			}
	
			if (list.Count == 0) {
				return default(TSource);
			}
	
			return list[0];
		}
		
		public static TSource SingleOrDefault<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			return source.Where(predicate).SingleOrDefault();
		}
	
		public static TSource SingleOrDefault<TSource> (this IEnumerable<TSource> source) {
			IList<TSource> list = source as IList<TSource>;
	
			if (list == null) {
				throw new Exception("IList == null!");
			}
			
			if (list.Count > 1) {
				throw new Exception("IList count > 1");
			}
	
			if (list.Count == 0) {
				return default(TSource);
			}
	
			return list[0];
		}
		
		public static TSource Last<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			return source.Where(predicate).Last();
		}
	
		public static TSource Last<TSource> (this IEnumerable<TSource> source) {
			IList<TSource> list = source as IList<TSource>;
	
			if (list == null) {
				throw new Exception("IList == null!");
			}
	
			if (list.Count == 0) {
				throw new Exception("No elements satisfying the predicate!");
			}
	
			return list[list.Count-1];
		}
		
		public static int Count<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			return source.Where(predicate).Count();
		}
	
		public static int Count<TSource> (this IEnumerable<TSource> source) {
			IList<TSource> list = source as IList<TSource>;
	
			if (list == null) {
				throw new Exception("IList == null!");
			}
	
			return list.Count;
		}
	
		public static IEnumerable<TResult> Select<TSource, TResult> (this IEnumerable<TSource> source, Func<TSource, TResult> selector) {
			List<TResult> result = new List<TResult>();
	
			foreach (TSource obj in source) {
				result.Add(selector(obj));
			}
	
			return result;
		}
	
		public static bool All<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			foreach (TSource obj in source) {
				if (!predicate(obj))
					return false;
			}
	
			return true;
		}
	
		public static bool Any<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			foreach (TSource obj in source) {
				if (predicate(obj))
					return true;
			}
	
			return false;
		}
	
		public static bool Any<TSource> (this IEnumerable<TSource> source) {
			return source.Any(src => true);
		}
	
		public static IEnumerable<TResult> SelectMany<TSource, TResult> (this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector) {
			IList<TResult> result = new List<TResult>();
	
			foreach (TSource obj in source) {
				foreach (TResult rObj in selector(obj)) {
					result.Add(rObj);
				}
			}
	
			return result;
		}
	
		public static int Min<TSource> (this IEnumerable<TSource> source, Func<TSource, int> selector) {
			int min = int.MaxValue;
	
			foreach (TSource obj in source) {
				int value = selector(obj);
	
				if (value < min)
					min = value;
			}
	
			return min;
		}
	
		public static int Min (this IEnumerable<int> source) {
			return source.Min(i => i);
		}
	
		public static float Min<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector) {
			float min = float.PositiveInfinity;
	
			foreach (TSource obj in source) {
				float value = selector(obj);
	
				if (value < min)
					min = value;
			}
	
			return min;
		}
	
		public static float Min (this IEnumerable<float> source) {
			return source.Min(i => i);
		}
	
		public static int Max<TSource> (this IEnumerable<TSource> source, Func<TSource, int> selector) {
			int max = int.MinValue;
	
			foreach (TSource obj in source) {
				int value = selector(obj);
	
				if (value > max)
					max = value;
			}
	
			return max;
		}
	
		public static int Max (this IEnumerable<int> source) {
			return source.Max(i => i);
		}
	
		public static float Max<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector) {
			float max = float.MinValue;
	
			foreach (TSource obj in source) {
				float value = selector(obj);
	
				if (value > max)
					max = value;
			}
	
			return max;
		}
	
		public static float Max (this IEnumerable<float> source) {
			return source.Max(i => i);
		}
	
		public static IEnumerable<TSource> Except<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second) {
			IList<TSource> result = new List<TSource>();
			IList<TSource> lSecond = second as IList<TSource>;
	
			if (lSecond == null)
				throw new Exception("IList == null!");
				
	
			foreach (TSource obj in first) {
				if (lSecond.IndexOf(obj) == -1)
					result.Add(obj);
			}
	
			return result;
		}
		
		public static IEnumerable<TSource> Intersect<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second) {
			IList<TSource> result = new List<TSource>();
			IList<TSource> lSecond = second as IList<TSource>;
	
			if (lSecond == null)
				throw new Exception("IList == null!");
				
	
			foreach (TSource obj in first) {
				if (lSecond.IndexOf(obj) != -1)
					result.Add(obj);
			}
	
			return result;
		}
	
		public static IEnumerable<TResult> Cast<TResult> (this IEnumerable source) {
			IList<TResult> result = new List<TResult>();
	
			foreach (object obj in source) {
				result.Add((TResult) obj);
			}
	
			return result;
		}
		
		public static IEnumerable<TResult> OfType<TResult> (this IEnumerable source) {
			List<TResult> result = new List<TResult>();
	
			foreach (object obj in source) {
				if ((typeof(TResult).IsAssignableFrom(obj.GetType())))
					result.Add((TResult) obj);
			}
	
			return result;
		}
	
		public static float Average<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector) {
			IEnumerable<TSource> enumerable = source.ToList();
			return enumerable.Sum(selector) / enumerable.Count();
		}
	
		public static TSource ElementAt<TSource> (this IEnumerable<TSource> source, int index) {
			int i = 0;
			
			foreach (TSource obj in source) {
				if (i == index)
					return obj;
	
				i++;
			}
	
			throw new Exception("No such element!");
		}
		
		public static int IndexOf<TSource> (this IEnumerable<TSource> source, TSource element) {
			int i = 0;
			
			foreach (TSource obj in source) {
				if (obj.Equals(element))
					return i;
	
				i++;
			}
	
			return -1;
		}
		
		public static bool Contains<TSource> (this IEnumerable<TSource> source, TSource element) {
			foreach (TSource obj in source) {
				if (obj.Equals(element))
					return true;
			}
			
			return false;
		}
		
		public static IEnumerable<TSource> OrderBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
			List<TSource> result = source.ToList();
			if (typeof(IComparable).IsAssignableFrom(typeof(TKey))) {
				result.Sort((x, y) => (keySelector(x) as IComparable).CompareTo((keySelector(y) as IComparable)));
			}
			else {
				throw new Exception("OrderBy - order by non-comparable type");
			}
			return result;
		}
		
		public static IEnumerable<TSource> OrderByDescending<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
			List<TSource> result = source.ToList();
			if (typeof(IComparable).IsAssignableFrom(typeof(TKey))) {
				result.Sort((x, y) => (keySelector(y) as IComparable).CompareTo((keySelector(x) as IComparable)));
			}
			else {
				throw new Exception("OrderByDescending - order by non-comparable type");
			}
			return result;
		}
		
		public static IEnumerable<TResult> Repeat<TResult> (TResult element, int count) {
			List<TResult> result = new List<TResult>();
			
			if (count < 0) {
				throw new Exception("Count < 0");
			}
			
			for (int i = 0; i < count; i++) {
				result.Add(element);
			}
			
			return result;
		}
		
	}
	
}
