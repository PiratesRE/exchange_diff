using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ComputedElementCollection<TSource1, TSource2, TResult> : ReadOnlyDelegatingCollection<TResult>
	{
		public ComputedElementCollection(Func<TSource1, TSource2, TResult> computeElementDelegate, IEnumerable<TSource1> source1Enumerable, IEnumerable<TSource2> source2Enumerable, int elementCount)
		{
			this.elementCount = elementCount;
			this.source1Enumerable = source1Enumerable;
			this.source2Enumerable = source2Enumerable;
			this.computeElementDelegate = computeElementDelegate;
		}

		public override int Count
		{
			get
			{
				return this.elementCount;
			}
		}

		public override IEnumerator<TResult> GetEnumerator()
		{
			return new ComputedElementCollection<TSource1, TSource2, TResult>.Enumerator<TSource1, TSource2, TResult>(this.computeElementDelegate, this.source1Enumerable, this.source2Enumerable);
		}

		private readonly int elementCount;

		private readonly IEnumerable<TSource1> source1Enumerable;

		private readonly IEnumerable<TSource2> source2Enumerable;

		private readonly Func<TSource1, TSource2, TResult> computeElementDelegate;

		private struct Enumerator<EnumTSource1, EnumTSource2, EnumTResult> : IEnumerator<EnumTResult>, IDisposable, IEnumerator
		{
			internal Enumerator(Func<EnumTSource1, EnumTSource2, EnumTResult> computeElementDelegate, IEnumerable<EnumTSource1> source1Enumerable, IEnumerable<EnumTSource2> source2Enumerable)
			{
				this.source1Enumerator = source1Enumerable.GetEnumerator();
				this.source2Enumerator = source2Enumerable.GetEnumerator();
				this.computeElementDelegate = computeElementDelegate;
			}

			public EnumTResult Current
			{
				get
				{
					return this.computeElementDelegate(this.source1Enumerator.Current, this.source2Enumerator.Current);
				}
			}

			public void Dispose()
			{
				Util.DisposeIfPresent(this.source1Enumerator);
				Util.DisposeIfPresent(this.source2Enumerator);
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				bool result = this.source1Enumerator.MoveNext();
				this.source2Enumerator.MoveNext();
				return result;
			}

			public void Reset()
			{
				this.source1Enumerator.Reset();
				this.source2Enumerator.Reset();
			}

			private readonly IEnumerator<EnumTSource1> source1Enumerator;

			private readonly IEnumerator<EnumTSource2> source2Enumerator;

			private readonly Func<EnumTSource1, EnumTSource2, EnumTResult> computeElementDelegate;
		}
	}
}
