using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	internal sealed class EnumerableWrapper<T> : IEnumerable<!0>, IEnumerable
	{
		private EnumerableWrapper(IList<IEnumerable<T>> enumerables, IList<IEnumerableFilter<T>> filters)
		{
			if (enumerables != null)
			{
				this.enumerables = new List<IEnumerable<T>>(enumerables);
			}
			if (filters != null)
			{
				this.filters = new List<IEnumerableFilter<T>>(filters);
			}
		}

		private static IList<TData> GetArrayIfNotNull<TData>(TData obj)
		{
			if (obj == null)
			{
				return null;
			}
			return new TData[]
			{
				obj
			};
		}

		public static EnumerableWrapper<T> GetWrapper(IEnumerable<T> enumerable)
		{
			return EnumerableWrapper<T>.GetWrapper(EnumerableWrapper<T>.GetArrayIfNotNull<IEnumerable<T>>(enumerable), null);
		}

		public static EnumerableWrapper<T> GetWrapper(IEnumerable<T> enumerable, IEnumerableFilter<T> filter)
		{
			return EnumerableWrapper<T>.GetWrapper(EnumerableWrapper<T>.GetArrayIfNotNull<IEnumerable<T>>(enumerable), EnumerableWrapper<T>.GetArrayIfNotNull<IEnumerableFilter<T>>(filter));
		}

		public static EnumerableWrapper<T> GetWrapper(IList<IEnumerable<T>> enumerables)
		{
			return EnumerableWrapper<T>.GetWrapper(enumerables, null);
		}

		public static EnumerableWrapper<T> GetWrapper(IList<IEnumerable<T>> enumerables, IEnumerableFilter<T> filter)
		{
			return EnumerableWrapper<T>.GetWrapper(enumerables, EnumerableWrapper<T>.GetArrayIfNotNull<IEnumerableFilter<T>>(filter));
		}

		public static EnumerableWrapper<T> GetWrapper(IEnumerable<T> enumerable, IList<IEnumerableFilter<T>> filters)
		{
			return EnumerableWrapper<T>.GetWrapper(EnumerableWrapper<T>.GetArrayIfNotNull<IEnumerable<T>>(enumerable), filters);
		}

		public static EnumerableWrapper<T> GetWrapper(IList<IEnumerable<T>> enumerables, IList<IEnumerableFilter<T>> filters)
		{
			if (enumerables == null || enumerables.Count == 0)
			{
				return EnumerableWrapper<T>.Empty;
			}
			if (enumerables.Count == 1)
			{
				EnumerableWrapper<T> enumerableWrapper = enumerables[0] as EnumerableWrapper<T>;
				if (enumerableWrapper != null)
				{
					if (filters == null || !enumerableWrapper.HasElements())
					{
						return enumerableWrapper;
					}
					if (enumerableWrapper.filters != null)
					{
						HashSet<IEnumerableFilter<T>> hashSet = new HashSet<IEnumerableFilter<T>>(enumerableWrapper.filters);
						HashSet<IEnumerableFilter<T>> other = new HashSet<IEnumerableFilter<T>>(filters);
						if (hashSet.IsSupersetOf(other))
						{
							return enumerableWrapper;
						}
					}
				}
				else
				{
					ICollection<T> collection = enumerables[0] as ICollection<T>;
					if (collection != null && collection.Count == 0)
					{
						return EnumerableWrapper<T>.Empty;
					}
				}
			}
			return new EnumerableWrapper<T>(enumerables, filters);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.ResolveEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.ResolveEnumerator();
		}

		private IEnumerator<T> ResolveEnumerator()
		{
			if (this.enumerator == null && this.consumedEnumerator && !this.hasElements)
			{
				return EnumerableWrapper<T>.EmptyList.GetEnumerator();
			}
			return this.Enumerator;
		}

		private EnumerableWrapper<T>.EnumeratorWrapper<T> Enumerator
		{
			get
			{
				if (this.enumerator == null)
				{
					if (this.enumerables != null)
					{
						this.enumerator = new EnumerableWrapper<T>.EnumeratorWrapper<T>(this.enumerables.GetEnumerator(), this.filters);
					}
					else
					{
						this.enumerator = new EnumerableWrapper<T>.EnumeratorWrapper<T>(null, null);
					}
				}
				return this.enumerator;
			}
		}

		internal bool IsEnumerableAlreadyConsumed()
		{
			return this.consumedEnumerator;
		}

		public bool HasElements()
		{
			this.InitHasElements();
			return this.hasElements;
		}

		public bool HasUnfilteredElements()
		{
			this.InitHasElements();
			return this.hasUnfilteredElements;
		}

		private void InitHasElements()
		{
			if (!this.consumedEnumerator)
			{
				try
				{
					EnumerableWrapper<T>.EnumeratorWrapper<T> enumeratorWrapper = this.Enumerator;
					this.hasElements = enumeratorWrapper.HasElements();
					this.hasUnfilteredElements = enumeratorWrapper.HasUnfilteredElements();
				}
				finally
				{
					if (!this.hasElements)
					{
						this.DisposeEnumerator();
					}
					this.consumedEnumerator = true;
				}
			}
		}

		private void DisposeEnumerator()
		{
			if (this.enumerator != null)
			{
				this.enumerator.Dispose();
			}
			this.enumerator = null;
		}

		private List<IEnumerable<T>> enumerables;

		private List<IEnumerableFilter<T>> filters;

		private EnumerableWrapper<T>.EnumeratorWrapper<T> enumerator;

		private bool consumedEnumerator;

		private bool hasElements;

		private bool hasUnfilteredElements;

		private static List<T> EmptyList = new List<T>();

		internal static EnumerableWrapper<T> Empty = new EnumerableWrapper<T>(null, null);

		private sealed class EnumeratorWrapper<TData> : IEnumerator<!1>, IEnumerator, IDisposeTrackable, IDisposable
		{
			internal EnumeratorWrapper(IEnumerator<IEnumerable<TData>> elements, IList<IEnumerableFilter<TData>> filters)
			{
				this.enumerables = elements;
				if (filters != null && filters.Count > 0)
				{
					this.filterEnabled = true;
					if (filters.Count == 1)
					{
						this.filter = filters[0];
					}
					else
					{
						this.filters = filters;
					}
				}
				this.disposeTracker = this.GetDisposeTracker();
			}

			public DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<EnumerableWrapper<T>.EnumeratorWrapper<TData>>(this);
			}

			public void SuppressDisposeTracker()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Suppress();
				}
			}

			private bool MoveNextElement()
			{
				if (this.firstRead)
				{
					this.firstRead = false;
					return this.hasElements.Value;
				}
				if (this.enumerator != null)
				{
					if (this.enumerator.MoveNext())
					{
						this.hasElements = new bool?(true);
						return true;
					}
					this.DisposeCurrentEnumerator();
				}
				if (this.enumerables != null)
				{
					while (this.enumerables.MoveNext())
					{
						EnumerableWrapper<T> enumerableWrapper = this.enumerables.Current as EnumerableWrapper<T>;
						if (enumerableWrapper != null)
						{
							this.hasUnfilteredElements |= enumerableWrapper.HasUnfilteredElements();
						}
						this.enumerator = this.enumerables.Current.GetEnumerator();
						if (this.enumerator.MoveNext())
						{
							this.hasElements = new bool?(true);
							return true;
						}
						this.DisposeCurrentEnumerator();
					}
				}
				if (this.enumerator != null)
				{
					this.DisposeCurrentEnumerator();
				}
				return false;
			}

			public bool MoveNext()
			{
				while (this.MoveNextElement())
				{
					this.hasUnfilteredElements = true;
					if (!this.filterEnabled || this.AcceptCurrent(this.Current))
					{
						this.hasElements = new bool?(true);
						return true;
					}
				}
				return false;
			}

			private bool AcceptCurrent(TData element)
			{
				if (this.filter != null)
				{
					return this.filter.AcceptElement(element);
				}
				foreach (IEnumerableFilter<TData> enumerableFilter in this.filters)
				{
					if (!enumerableFilter.AcceptElement(element))
					{
						return false;
					}
				}
				return true;
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}

			public TData Current
			{
				get
				{
					return this.GetCurrent();
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.GetCurrent();
				}
			}

			private TData GetCurrent()
			{
				if (this.enumerator != null)
				{
					return this.enumerator.Current;
				}
				throw new InvalidOperationException(DataStrings.InvalidOperationCurrentProperty);
			}

			public bool HasElements()
			{
				this.InitHasMethods();
				return this.hasElements.Value;
			}

			public bool HasUnfilteredElements()
			{
				this.InitHasMethods();
				return this.hasUnfilteredElements;
			}

			private void InitHasMethods()
			{
				if (this.hasElements == null)
				{
					this.hasElements = new bool?(this.MoveNext());
					this.firstRead = true;
				}
			}

			private void DisposeCurrentEnumerator()
			{
				this.enumerator.Dispose();
				this.enumerator = null;
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (!this.disposed && disposing)
				{
					if (this.enumerator != null)
					{
						this.enumerator.Dispose();
					}
					while (this.enumerables != null && this.enumerables.MoveNext())
					{
						IEnumerator<TData> enumerator = this.enumerables.Current.GetEnumerator();
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
					}
				}
				this.disposed = true;
			}

			private IEnumerator<TData> enumerator;

			private IEnumerator<IEnumerable<TData>> enumerables;

			private bool? hasElements;

			private bool hasUnfilteredElements;

			private bool firstRead;

			private bool filterEnabled;

			private IList<IEnumerableFilter<TData>> filters;

			private IEnumerableFilter<TData> filter;

			private DisposeTracker disposeTracker;

			private bool disposed;
		}
	}
}
