using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	internal sealed class ResultsCache<T> : IEnumerable<Result<!0>>, IEnumerable
	{
		public ResultsCache()
		{
			this.cache = new ResultsCache<T>.Cache<Result<T>>.EmptyCache(1);
			this.isComplete = false;
		}

		private ResultsCache(ResultsCache<T>.ICache<Result<T>> cache, bool isComplete)
		{
			this.cache = cache;
			this.isComplete = isComplete;
		}

		public int Count
		{
			get
			{
				return this.cache.ResultCount;
			}
		}

		public bool IsComplete
		{
			get
			{
				return this.isComplete;
			}
		}

		public ResultsCache<T> AsCompleted()
		{
			if (this.IsComplete)
			{
				return this;
			}
			return new ResultsCache<T>(this.cache, true);
		}

		public ResultsCache<T> Add(Result<T> item)
		{
			if (this.isComplete)
			{
				throw new InvalidOperationException();
			}
			return new ResultsCache<T>(this.cache.Add(item), this.isComplete);
		}

		public IEnumerable<Result<T>> Skip(int count)
		{
			return this.cache.Skip(count);
		}

		public IEnumerator<Result<T>> GetEnumerator()
		{
			return this.cache.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.cache.GetEnumerator();
		}

		private readonly ResultsCache<T>.ICache<Result<T>> cache;

		private readonly bool isComplete;

		private interface ICache<U> : IEnumerable<U>, IEnumerable
		{
			int ResultCount { get; }

			int ItemsPerListlette { get; }

			ResultsCache<T>.ICache<U> Add(U item);

			IEnumerable<U> Skip(int count);
		}

		private sealed class Cache<U> : ResultsCache<T>.ICache<U>, IEnumerable<U>, IEnumerable
		{
			public Cache(ResultsCache<T>.Cache<U>.Listlette head, ResultsCache<T>.ICache<ResultsCache<T>.Cache<U>.Listlette> tail, int itemsPerListlette)
			{
				this.head = head;
				this.tail = tail;
				this.itemsPerListlette = itemsPerListlette;
			}

			public int ResultCount
			{
				get
				{
					return this.tail.ResultCount + this.head.Count * this.ItemsPerListlette;
				}
			}

			public int ItemsPerListlette
			{
				get
				{
					return this.itemsPerListlette;
				}
			}

			public ResultsCache<T>.ICache<U> Add(U item)
			{
				if (this.head.Count < 4)
				{
					return new ResultsCache<T>.Cache<U>(this.head.Add(item), this.tail, this.itemsPerListlette);
				}
				return new ResultsCache<T>.Cache<U>(new ResultsCache<T>.Cache<U>.One(item), this.tail.Add(this.head), this.itemsPerListlette);
			}

			public IEnumerable<U> Skip(int count)
			{
				if (count > this.ResultCount)
				{
					throw new ArgumentOutOfRangeException();
				}
				int tailCount = this.tail.ResultCount;
				if (count < tailCount)
				{
					bool isFirstListlette = true;
					foreach (ResultsCache<T>.Cache<U>.Listlette listlette in this.tail.Skip(count))
					{
						if (isFirstListlette)
						{
							isFirstListlette = false;
							int tailSkip = count % this.tail.ItemsPerListlette / this.itemsPerListlette;
							switch (listlette.Count)
							{
							case 1:
								switch (tailSkip)
								{
								case 0:
									yield return listlette.Item1;
									break;
								}
								break;
							case 2:
								switch (tailSkip)
								{
								case 0:
									yield return listlette.Item1;
									yield return listlette.Item2;
									break;
								case 1:
									yield return listlette.Item2;
									break;
								}
								break;
							case 3:
								switch (tailSkip)
								{
								case 0:
									yield return listlette.Item1;
									yield return listlette.Item2;
									yield return listlette.Item3;
									break;
								case 1:
									yield return listlette.Item2;
									yield return listlette.Item3;
									break;
								case 2:
									yield return listlette.Item3;
									break;
								}
								break;
							case 4:
								switch (tailSkip)
								{
								case 0:
									yield return listlette.Item1;
									yield return listlette.Item2;
									yield return listlette.Item3;
									yield return listlette.Item4;
									break;
								case 1:
									yield return listlette.Item2;
									yield return listlette.Item3;
									yield return listlette.Item4;
									break;
								case 2:
									yield return listlette.Item3;
									yield return listlette.Item4;
									break;
								case 3:
									yield return listlette.Item4;
									break;
								}
								break;
							}
						}
						else
						{
							switch (listlette.Count)
							{
							case 1:
								yield return listlette.Item1;
								break;
							case 2:
								yield return listlette.Item1;
								yield return listlette.Item2;
								break;
							case 3:
								yield return listlette.Item1;
								yield return listlette.Item2;
								yield return listlette.Item3;
								break;
							case 4:
								yield return listlette.Item1;
								yield return listlette.Item2;
								yield return listlette.Item3;
								yield return listlette.Item4;
								break;
							}
						}
					}
				}
				int headSkipCount = (count < tailCount) ? 0 : ((count - tailCount) / this.itemsPerListlette);
				switch (this.head.Count)
				{
				case 1:
					switch (headSkipCount)
					{
					case 0:
						yield return this.head.Item1;
						break;
					}
					break;
				case 2:
					switch (headSkipCount)
					{
					case 0:
						yield return this.head.Item1;
						yield return this.head.Item2;
						break;
					case 1:
						yield return this.head.Item2;
						break;
					}
					break;
				case 3:
					switch (headSkipCount)
					{
					case 0:
						yield return this.head.Item1;
						yield return this.head.Item2;
						yield return this.head.Item3;
						break;
					case 1:
						yield return this.head.Item2;
						yield return this.head.Item3;
						break;
					case 2:
						yield return this.head.Item3;
						break;
					}
					break;
				case 4:
					switch (headSkipCount)
					{
					case 0:
						yield return this.head.Item1;
						yield return this.head.Item2;
						yield return this.head.Item3;
						yield return this.head.Item4;
						break;
					case 1:
						yield return this.head.Item2;
						yield return this.head.Item3;
						yield return this.head.Item4;
						break;
					case 2:
						yield return this.head.Item3;
						yield return this.head.Item4;
						break;
					case 3:
						yield return this.head.Item4;
						break;
					}
					break;
				}
				yield break;
			}

			public IEnumerator<U> GetEnumerator()
			{
				return this.Skip(0).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public override string ToString()
			{
				return string.Format("CACHE(Count:{0} IPL:{1} Tail:{2} Head:{3})", new object[]
				{
					this.ResultCount,
					this.ItemsPerListlette,
					this.tail,
					this.head
				});
			}

			private readonly ResultsCache<T>.Cache<U>.Listlette head;

			private readonly ResultsCache<T>.ICache<ResultsCache<T>.Cache<U>.Listlette> tail;

			private readonly int itemsPerListlette;

			public sealed class EmptyCache : ResultsCache<T>.ICache<U>, IEnumerable<!1>, IEnumerable
			{
				public EmptyCache(int itemsPerListlette)
				{
					this.itemsPerListlette = itemsPerListlette;
				}

				public int ResultCount
				{
					get
					{
						return 0;
					}
				}

				public int ItemsPerListlette
				{
					get
					{
						return this.itemsPerListlette;
					}
				}

				public ResultsCache<T>.ICache<U> Add(U item)
				{
					return new ResultsCache<T>.Cache<U>(new ResultsCache<T>.Cache<U>.One(item), new ResultsCache<T>.Cache<ResultsCache<T>.Cache<U>.Listlette>.EmptyCache(this.itemsPerListlette * 4), this.itemsPerListlette);
				}

				public IEnumerable<U> Skip(int count)
				{
					if (count != 0)
					{
						throw new InvalidOperationException();
					}
					yield break;
				}

				public IEnumerator<U> GetEnumerator()
				{
					yield break;
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					yield break;
				}

				public override string ToString()
				{
					return string.Format("EMPTY(IPL:{0})", this.ItemsPerListlette);
				}

				private readonly int itemsPerListlette;
			}

			public abstract class Listlette
			{
				public abstract U Item1 { get; }

				public abstract U Item2 { get; }

				public abstract U Item3 { get; }

				public abstract U Item4 { get; }

				public abstract int Count { get; }

				public abstract ResultsCache<T>.Cache<U>.Listlette Add(U item);
			}

			public sealed class One : ResultsCache<T>.Cache<U>.Listlette
			{
				public One(U item1)
				{
					this.item1 = item1;
				}

				public override U Item1
				{
					get
					{
						return this.item1;
					}
				}

				public override U Item2
				{
					get
					{
						throw new IndexOutOfRangeException();
					}
				}

				public override U Item3
				{
					get
					{
						throw new IndexOutOfRangeException();
					}
				}

				public override U Item4
				{
					get
					{
						throw new IndexOutOfRangeException();
					}
				}

				public override int Count
				{
					get
					{
						return 1;
					}
				}

				public override ResultsCache<T>.Cache<U>.Listlette Add(U item)
				{
					return new ResultsCache<T>.Cache<U>.Two(this.item1, item);
				}

				public override string ToString()
				{
					return string.Format("ONE({0})", this.Item1);
				}

				private readonly U item1;
			}

			public sealed class Two : ResultsCache<T>.Cache<U>.Listlette
			{
				public Two(U item1, U item2)
				{
					this.item1 = item1;
					this.item2 = item2;
				}

				public override U Item1
				{
					get
					{
						return this.item1;
					}
				}

				public override U Item2
				{
					get
					{
						return this.item2;
					}
				}

				public override U Item3
				{
					get
					{
						throw new IndexOutOfRangeException();
					}
				}

				public override U Item4
				{
					get
					{
						throw new IndexOutOfRangeException();
					}
				}

				public override int Count
				{
					get
					{
						return 2;
					}
				}

				public override ResultsCache<T>.Cache<U>.Listlette Add(U item)
				{
					return new ResultsCache<T>.Cache<U>.Three(this.item1, this.item2, item);
				}

				public override string ToString()
				{
					return string.Format("TWO({0},{1})", this.Item1, this.Item2);
				}

				private readonly U item1;

				private readonly U item2;
			}

			public sealed class Three : ResultsCache<T>.Cache<U>.Listlette
			{
				public Three(U item1, U item2, U item3)
				{
					this.item1 = item1;
					this.item2 = item2;
					this.item3 = item3;
				}

				public override U Item1
				{
					get
					{
						return this.item1;
					}
				}

				public override U Item2
				{
					get
					{
						return this.item2;
					}
				}

				public override U Item3
				{
					get
					{
						return this.item3;
					}
				}

				public override U Item4
				{
					get
					{
						throw new IndexOutOfRangeException();
					}
				}

				public override int Count
				{
					get
					{
						return 3;
					}
				}

				public override ResultsCache<T>.Cache<U>.Listlette Add(U item)
				{
					return new ResultsCache<T>.Cache<U>.Four(this.item1, this.item2, this.item3, item);
				}

				public override string ToString()
				{
					return string.Format("THREE({0},{1},{2})", this.Item1, this.Item2, this.Item3);
				}

				private readonly U item1;

				private readonly U item2;

				private readonly U item3;
			}

			public sealed class Four : ResultsCache<T>.Cache<U>.Listlette
			{
				public Four(U item1, U item2, U item3, U item4)
				{
					this.item1 = item1;
					this.item2 = item2;
					this.item3 = item3;
					this.item4 = item4;
				}

				public override U Item1
				{
					get
					{
						return this.item1;
					}
				}

				public override U Item2
				{
					get
					{
						return this.item2;
					}
				}

				public override U Item3
				{
					get
					{
						return this.item3;
					}
				}

				public override U Item4
				{
					get
					{
						return this.item4;
					}
				}

				public override int Count
				{
					get
					{
						return 4;
					}
				}

				public override ResultsCache<T>.Cache<U>.Listlette Add(U item)
				{
					throw new InvalidOperationException();
				}

				public override string ToString()
				{
					return string.Format("FOUR({0},{1},{2},{3})", new object[]
					{
						this.Item1,
						this.Item2,
						this.Item3,
						this.Item4
					});
				}

				private readonly U item1;

				private readonly U item2;

				private readonly U item3;

				private readonly U item4;
			}
		}
	}
}
