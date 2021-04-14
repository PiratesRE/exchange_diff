using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class EnumerableFunctions
	{
		public static ICollection<T> Union<T>(this ICollection<T> first, ICollection<T> second)
		{
			Util.ThrowOnNullArgument(first, "first");
			ICollection<T> result;
			if (EnumerableFunctions.TryPickWinner<T>(first, second, out result))
			{
				return result;
			}
			if (first == second)
			{
				return first;
			}
			HashSet<T> hashSet = new HashSet<T>(first);
			hashSet.UnionWith(second);
			return hashSet;
		}

		internal static ICollection<T> Concat<T>(this ICollection<T> first, ICollection<T> second)
		{
			ICollection<T> result;
			if (EnumerableFunctions.TryPickWinner<T>(first, second, out result))
			{
				return result;
			}
			return new EnumerableFunctions.ConcatCollection<T>(first, second);
		}

		public static IEnumerable<TResult> Cast<TSource, TResult>(this IEnumerable<TSource> source) where TSource : PropertyDefinition where TResult : PropertyDefinition
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return EnumerableFunctions.CastIterator<TResult, TSource>(source);
		}

		public static ICollection<TResult> Select<TSource, TResult>(this ICollection<TSource> first, Func<TSource, TResult> selector)
		{
			return new EnumerableFunctions.SelectCollection<TSource, TResult>(first, selector);
		}

		private static bool TryPickWinner<T>(ICollection<T> first, ICollection<T> second, out ICollection<T> winner)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null || second.Count == 0)
			{
				winner = first;
				return true;
			}
			if (first.Count == 0)
			{
				winner = second;
				return true;
			}
			winner = null;
			return false;
		}

		private static IEnumerable<TResult> CastIterator<TResult, TSource>(IEnumerable<TSource> source) where TResult : PropertyDefinition where TSource : PropertyDefinition
		{
			foreach (TSource element in source)
			{
				TResult temp = element as TResult;
				if (temp == null)
				{
					throw new InvalidCastException(string.Format("Can't convert {0} to {1}", element, typeof(TSource)));
				}
				yield return temp;
			}
			yield break;
		}

		private sealed class ConcatCollection<T> : ReadOnlyDelegatingCollection<T>
		{
			public ConcatCollection(ICollection<T> first, ICollection<T> second)
			{
				Util.ThrowOnNullArgument(first, "first");
				Util.ThrowOnNullArgument(second, "second");
				this.first = first;
				this.second = second;
			}

			public override bool Contains(T item)
			{
				return this.first.Contains(item) || this.second.Contains(item);
			}

			public override void CopyTo(T[] array, int arrayIndex)
			{
				this.first.CopyTo(array, arrayIndex);
				this.second.CopyTo(array, arrayIndex + this.first.Count);
			}

			public override int Count
			{
				get
				{
					return this.first.Count + this.second.Count;
				}
			}

			public override IEnumerator<T> GetEnumerator()
			{
				return this.first.Concat(this.second).GetEnumerator();
			}

			private readonly ICollection<T> first;

			private readonly ICollection<T> second;
		}

		private sealed class SelectCollection<TSource, TResult> : ReadOnlyDelegatingCollection<TResult>
		{
			public SelectCollection(ICollection<TSource> source, Func<TSource, TResult> selector)
			{
				Util.ThrowOnNullArgument(source, "source");
				Util.ThrowOnNullArgument(selector, "selector");
				this.source = source;
				this.selector = selector;
			}

			public override int Count
			{
				get
				{
					return this.source.Count;
				}
			}

			public override IEnumerator<TResult> GetEnumerator()
			{
				return this.source.Select(this.selector).GetEnumerator();
			}

			private readonly ICollection<TSource> source;

			private readonly Func<TSource, TResult> selector;
		}
	}
}
