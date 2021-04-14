using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq
{
	public static class LinqNamespaceExtensionMethods
	{
		public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.All(predicate);
		}

		public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Any(predicate);
		}

		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
		{
			return source.Contains(value);
		}

		public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
		{
			return source.Cast<TResult>();
		}

		public static int Count<TSource>(this IEnumerable<TSource> source)
		{
			return source.Count<TSource>();
		}

		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Except(second);
		}

		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			return source.FirstOrDefault<TSource>();
		}

		public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Intersect(second);
		}

		public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderBy(keySelector);
		}

		public static IEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderByDescending(keySelector);
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Select(selector);
		}

		public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
		{
			return source.Take(count);
		}

		public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
		{
			return source.ToArray<TSource>();
		}

		public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			return source.ToList<TSource>();
		}

		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Union(second);
		}

		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
		{
			return source.Distinct<TSource>();
		}

		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate);
		}
	}
}
