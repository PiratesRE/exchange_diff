using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace System.Collections.Concurrent
{
	[__DynamicallyInvokable]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public static class Partitioner
	{
		[__DynamicallyInvokable]
		public static OrderablePartitioner<TSource> Create<TSource>(IList<TSource> list, bool loadBalance)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (loadBalance)
			{
				return new Partitioner.DynamicPartitionerForIList<TSource>(list);
			}
			return new Partitioner.StaticIndexRangePartitionerForIList<TSource>(list);
		}

		[__DynamicallyInvokable]
		public static OrderablePartitioner<TSource> Create<TSource>(TSource[] array, bool loadBalance)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (loadBalance)
			{
				return new Partitioner.DynamicPartitionerForArray<TSource>(array);
			}
			return new Partitioner.StaticIndexRangePartitionerForArray<TSource>(array);
		}

		[__DynamicallyInvokable]
		public static OrderablePartitioner<TSource> Create<TSource>(IEnumerable<TSource> source)
		{
			return Partitioner.Create<TSource>(source, EnumerablePartitionerOptions.None);
		}

		[__DynamicallyInvokable]
		public static OrderablePartitioner<TSource> Create<TSource>(IEnumerable<TSource> source, EnumerablePartitionerOptions partitionerOptions)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((partitionerOptions & ~EnumerablePartitionerOptions.NoBuffering) != EnumerablePartitionerOptions.None)
			{
				throw new ArgumentOutOfRangeException("partitionerOptions");
			}
			return new Partitioner.DynamicPartitionerForIEnumerable<TSource>(source, partitionerOptions);
		}

		[__DynamicallyInvokable]
		public static OrderablePartitioner<Tuple<long, long>> Create(long fromInclusive, long toExclusive)
		{
			int num = 3;
			if (toExclusive <= fromInclusive)
			{
				throw new ArgumentOutOfRangeException("toExclusive");
			}
			long num2 = (toExclusive - fromInclusive) / (long)(PlatformHelper.ProcessorCount * num);
			if (num2 == 0L)
			{
				num2 = 1L;
			}
			return Partitioner.Create<Tuple<long, long>>(Partitioner.CreateRanges(fromInclusive, toExclusive, num2), EnumerablePartitionerOptions.NoBuffering);
		}

		[__DynamicallyInvokable]
		public static OrderablePartitioner<Tuple<long, long>> Create(long fromInclusive, long toExclusive, long rangeSize)
		{
			if (toExclusive <= fromInclusive)
			{
				throw new ArgumentOutOfRangeException("toExclusive");
			}
			if (rangeSize <= 0L)
			{
				throw new ArgumentOutOfRangeException("rangeSize");
			}
			return Partitioner.Create<Tuple<long, long>>(Partitioner.CreateRanges(fromInclusive, toExclusive, rangeSize), EnumerablePartitionerOptions.NoBuffering);
		}

		private static IEnumerable<Tuple<long, long>> CreateRanges(long fromInclusive, long toExclusive, long rangeSize)
		{
			bool shouldQuit = false;
			long i = fromInclusive;
			while (i < toExclusive && !shouldQuit)
			{
				long item = i;
				long num;
				try
				{
					num = checked(i + rangeSize);
				}
				catch (OverflowException)
				{
					num = toExclusive;
					shouldQuit = true;
				}
				if (num > toExclusive)
				{
					num = toExclusive;
				}
				yield return new Tuple<long, long>(item, num);
				i += rangeSize;
			}
			yield break;
		}

		[__DynamicallyInvokable]
		public static OrderablePartitioner<Tuple<int, int>> Create(int fromInclusive, int toExclusive)
		{
			int num = 3;
			if (toExclusive <= fromInclusive)
			{
				throw new ArgumentOutOfRangeException("toExclusive");
			}
			int num2 = (toExclusive - fromInclusive) / (PlatformHelper.ProcessorCount * num);
			if (num2 == 0)
			{
				num2 = 1;
			}
			return Partitioner.Create<Tuple<int, int>>(Partitioner.CreateRanges(fromInclusive, toExclusive, num2), EnumerablePartitionerOptions.NoBuffering);
		}

		[__DynamicallyInvokable]
		public static OrderablePartitioner<Tuple<int, int>> Create(int fromInclusive, int toExclusive, int rangeSize)
		{
			if (toExclusive <= fromInclusive)
			{
				throw new ArgumentOutOfRangeException("toExclusive");
			}
			if (rangeSize <= 0)
			{
				throw new ArgumentOutOfRangeException("rangeSize");
			}
			return Partitioner.Create<Tuple<int, int>>(Partitioner.CreateRanges(fromInclusive, toExclusive, rangeSize), EnumerablePartitionerOptions.NoBuffering);
		}

		private static IEnumerable<Tuple<int, int>> CreateRanges(int fromInclusive, int toExclusive, int rangeSize)
		{
			bool shouldQuit = false;
			int i = fromInclusive;
			while (i < toExclusive && !shouldQuit)
			{
				int item = i;
				int num;
				try
				{
					num = checked(i + rangeSize);
				}
				catch (OverflowException)
				{
					num = toExclusive;
					shouldQuit = true;
				}
				if (num > toExclusive)
				{
					num = toExclusive;
				}
				yield return new Tuple<int, int>(item, num);
				i += rangeSize;
			}
			yield break;
		}

		private static int GetDefaultChunkSize<TSource>()
		{
			int result;
			if (typeof(TSource).IsValueType)
			{
				if (typeof(TSource).StructLayoutAttribute.Value == LayoutKind.Explicit)
				{
					result = Math.Max(1, 512 / Marshal.SizeOf(typeof(TSource)));
				}
				else
				{
					result = 128;
				}
			}
			else
			{
				result = 512 / IntPtr.Size;
			}
			return result;
		}

		private const int DEFAULT_BYTES_PER_CHUNK = 512;

		private abstract class DynamicPartitionEnumerator_Abstract<TSource, TSourceReader> : IEnumerator<KeyValuePair<long, TSource>>, IDisposable, IEnumerator
		{
			protected DynamicPartitionEnumerator_Abstract(TSourceReader sharedReader, Partitioner.SharedLong sharedIndex) : this(sharedReader, sharedIndex, false)
			{
			}

			protected DynamicPartitionEnumerator_Abstract(TSourceReader sharedReader, Partitioner.SharedLong sharedIndex, bool useSingleChunking)
			{
				this.m_sharedReader = sharedReader;
				this.m_sharedIndex = sharedIndex;
				this.m_maxChunkSize = (useSingleChunking ? 1 : Partitioner.DynamicPartitionEnumerator_Abstract<TSource, TSourceReader>.s_defaultMaxChunkSize);
			}

			protected abstract bool GrabNextChunk(int requestedChunkSize);

			protected abstract bool HasNoElementsLeft { get; set; }

			public abstract KeyValuePair<long, TSource> Current { get; }

			public abstract void Dispose();

			public void Reset()
			{
				throw new NotSupportedException();
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
				if (this.m_localOffset == null)
				{
					this.m_localOffset = new Partitioner.SharedInt(-1);
					this.m_currentChunkSize = new Partitioner.SharedInt(0);
					this.m_doublingCountdown = 3;
				}
				if (this.m_localOffset.Value < this.m_currentChunkSize.Value - 1)
				{
					this.m_localOffset.Value++;
					return true;
				}
				int requestedChunkSize;
				if (this.m_currentChunkSize.Value == 0)
				{
					requestedChunkSize = 1;
				}
				else if (this.m_doublingCountdown > 0)
				{
					requestedChunkSize = this.m_currentChunkSize.Value;
				}
				else
				{
					requestedChunkSize = Math.Min(this.m_currentChunkSize.Value * 2, this.m_maxChunkSize);
					this.m_doublingCountdown = 3;
				}
				this.m_doublingCountdown--;
				if (this.GrabNextChunk(requestedChunkSize))
				{
					this.m_localOffset.Value = 0;
					return true;
				}
				return false;
			}

			protected readonly TSourceReader m_sharedReader;

			protected static int s_defaultMaxChunkSize = Partitioner.GetDefaultChunkSize<TSource>();

			protected Partitioner.SharedInt m_currentChunkSize;

			protected Partitioner.SharedInt m_localOffset;

			private const int CHUNK_DOUBLING_RATE = 3;

			private int m_doublingCountdown;

			protected readonly int m_maxChunkSize;

			protected readonly Partitioner.SharedLong m_sharedIndex;
		}

		private class DynamicPartitionerForIEnumerable<TSource> : OrderablePartitioner<TSource>
		{
			internal DynamicPartitionerForIEnumerable(IEnumerable<TSource> source, EnumerablePartitionerOptions partitionerOptions) : base(true, false, true)
			{
				this.m_source = source;
				this.m_useSingleChunking = ((partitionerOptions & EnumerablePartitionerOptions.NoBuffering) > EnumerablePartitionerOptions.None);
			}

			public override IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount)
			{
				if (partitionCount <= 0)
				{
					throw new ArgumentOutOfRangeException("partitionCount");
				}
				IEnumerator<KeyValuePair<long, TSource>>[] array = new IEnumerator<KeyValuePair<long, TSource>>[partitionCount];
				IEnumerable<KeyValuePair<long, TSource>> enumerable = new Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerable(this.m_source.GetEnumerator(), this.m_useSingleChunking, true);
				for (int i = 0; i < partitionCount; i++)
				{
					array[i] = enumerable.GetEnumerator();
				}
				return array;
			}

			public override IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions()
			{
				return new Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerable(this.m_source.GetEnumerator(), this.m_useSingleChunking, false);
			}

			public override bool SupportsDynamicPartitions
			{
				get
				{
					return true;
				}
			}

			private IEnumerable<TSource> m_source;

			private readonly bool m_useSingleChunking;

			private class InternalPartitionEnumerable : IEnumerable<KeyValuePair<long, TSource>>, IEnumerable, IDisposable
			{
				internal InternalPartitionEnumerable(IEnumerator<TSource> sharedReader, bool useSingleChunking, bool isStaticPartitioning)
				{
					this.m_sharedReader = sharedReader;
					this.m_sharedIndex = new Partitioner.SharedLong(-1L);
					this.m_hasNoElementsLeft = new Partitioner.SharedBool(false);
					this.m_sourceDepleted = new Partitioner.SharedBool(false);
					this.m_sharedLock = new object();
					this.m_useSingleChunking = useSingleChunking;
					if (!this.m_useSingleChunking)
					{
						int num = (PlatformHelper.ProcessorCount > 4) ? 4 : 1;
						this.m_FillBuffer = new KeyValuePair<long, TSource>[num * Partitioner.GetDefaultChunkSize<TSource>()];
					}
					if (isStaticPartitioning)
					{
						this.m_activePartitionCount = new Partitioner.SharedInt(0);
						return;
					}
					this.m_activePartitionCount = null;
				}

				public IEnumerator<KeyValuePair<long, TSource>> GetEnumerator()
				{
					if (this.m_disposed)
					{
						throw new ObjectDisposedException(Environment.GetResourceString("PartitionerStatic_CanNotCallGetEnumeratorAfterSourceHasBeenDisposed"));
					}
					return new Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerator(this.m_sharedReader, this.m_sharedIndex, this.m_hasNoElementsLeft, this.m_sharedLock, this.m_activePartitionCount, this, this.m_useSingleChunking);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				private void TryCopyFromFillBuffer(KeyValuePair<long, TSource>[] destArray, int requestedChunkSize, ref int actualNumElementsGrabbed)
				{
					actualNumElementsGrabbed = 0;
					KeyValuePair<long, TSource>[] fillBuffer = this.m_FillBuffer;
					if (fillBuffer == null)
					{
						return;
					}
					if (this.m_FillBufferCurrentPosition >= this.m_FillBufferSize)
					{
						return;
					}
					Interlocked.Increment(ref this.m_activeCopiers);
					int num = Interlocked.Add(ref this.m_FillBufferCurrentPosition, requestedChunkSize);
					int num2 = num - requestedChunkSize;
					if (num2 < this.m_FillBufferSize)
					{
						actualNumElementsGrabbed = ((num < this.m_FillBufferSize) ? num : (this.m_FillBufferSize - num2));
						Array.Copy(fillBuffer, num2, destArray, 0, actualNumElementsGrabbed);
					}
					Interlocked.Decrement(ref this.m_activeCopiers);
				}

				internal bool GrabChunk(KeyValuePair<long, TSource>[] destArray, int requestedChunkSize, ref int actualNumElementsGrabbed)
				{
					actualNumElementsGrabbed = 0;
					if (this.m_hasNoElementsLeft.Value)
					{
						return false;
					}
					if (this.m_useSingleChunking)
					{
						return this.GrabChunk_Single(destArray, requestedChunkSize, ref actualNumElementsGrabbed);
					}
					return this.GrabChunk_Buffered(destArray, requestedChunkSize, ref actualNumElementsGrabbed);
				}

				internal bool GrabChunk_Single(KeyValuePair<long, TSource>[] destArray, int requestedChunkSize, ref int actualNumElementsGrabbed)
				{
					object sharedLock = this.m_sharedLock;
					bool result;
					lock (sharedLock)
					{
						if (this.m_hasNoElementsLeft.Value)
						{
							result = false;
						}
						else
						{
							try
							{
								if (this.m_sharedReader.MoveNext())
								{
									this.m_sharedIndex.Value = checked(this.m_sharedIndex.Value + 1L);
									destArray[0] = new KeyValuePair<long, TSource>(this.m_sharedIndex.Value, this.m_sharedReader.Current);
									actualNumElementsGrabbed = 1;
									result = true;
								}
								else
								{
									this.m_sourceDepleted.Value = true;
									this.m_hasNoElementsLeft.Value = true;
									result = false;
								}
							}
							catch
							{
								this.m_sourceDepleted.Value = true;
								this.m_hasNoElementsLeft.Value = true;
								throw;
							}
						}
					}
					return result;
				}

				internal bool GrabChunk_Buffered(KeyValuePair<long, TSource>[] destArray, int requestedChunkSize, ref int actualNumElementsGrabbed)
				{
					this.TryCopyFromFillBuffer(destArray, requestedChunkSize, ref actualNumElementsGrabbed);
					if (actualNumElementsGrabbed == requestedChunkSize)
					{
						return true;
					}
					if (this.m_sourceDepleted.Value)
					{
						this.m_hasNoElementsLeft.Value = true;
						this.m_FillBuffer = null;
						return actualNumElementsGrabbed > 0;
					}
					object sharedLock = this.m_sharedLock;
					lock (sharedLock)
					{
						if (this.m_sourceDepleted.Value)
						{
							return actualNumElementsGrabbed > 0;
						}
						try
						{
							if (this.m_activeCopiers > 0)
							{
								SpinWait spinWait = default(SpinWait);
								while (this.m_activeCopiers > 0)
								{
									spinWait.SpinOnce();
								}
							}
							while (actualNumElementsGrabbed < requestedChunkSize)
							{
								if (!this.m_sharedReader.MoveNext())
								{
									this.m_sourceDepleted.Value = true;
									break;
								}
								this.m_sharedIndex.Value = checked(this.m_sharedIndex.Value + 1L);
								destArray[actualNumElementsGrabbed] = new KeyValuePair<long, TSource>(this.m_sharedIndex.Value, this.m_sharedReader.Current);
								actualNumElementsGrabbed++;
							}
							KeyValuePair<long, TSource>[] fillBuffer = this.m_FillBuffer;
							if (!this.m_sourceDepleted.Value && fillBuffer != null && this.m_FillBufferCurrentPosition >= fillBuffer.Length)
							{
								for (int i = 0; i < fillBuffer.Length; i++)
								{
									if (!this.m_sharedReader.MoveNext())
									{
										this.m_sourceDepleted.Value = true;
										this.m_FillBufferSize = i;
										break;
									}
									this.m_sharedIndex.Value = checked(this.m_sharedIndex.Value + 1L);
									fillBuffer[i] = new KeyValuePair<long, TSource>(this.m_sharedIndex.Value, this.m_sharedReader.Current);
								}
								this.m_FillBufferCurrentPosition = 0;
							}
						}
						catch
						{
							this.m_sourceDepleted.Value = true;
							this.m_hasNoElementsLeft.Value = true;
							throw;
						}
					}
					return actualNumElementsGrabbed > 0;
				}

				public void Dispose()
				{
					if (!this.m_disposed)
					{
						this.m_disposed = true;
						this.m_sharedReader.Dispose();
					}
				}

				private readonly IEnumerator<TSource> m_sharedReader;

				private Partitioner.SharedLong m_sharedIndex;

				private volatile KeyValuePair<long, TSource>[] m_FillBuffer;

				private volatile int m_FillBufferSize;

				private volatile int m_FillBufferCurrentPosition;

				private volatile int m_activeCopiers;

				private Partitioner.SharedBool m_hasNoElementsLeft;

				private Partitioner.SharedBool m_sourceDepleted;

				private object m_sharedLock;

				private bool m_disposed;

				private Partitioner.SharedInt m_activePartitionCount;

				private readonly bool m_useSingleChunking;
			}

			private class InternalPartitionEnumerator : Partitioner.DynamicPartitionEnumerator_Abstract<TSource, IEnumerator<TSource>>
			{
				internal InternalPartitionEnumerator(IEnumerator<TSource> sharedReader, Partitioner.SharedLong sharedIndex, Partitioner.SharedBool hasNoElementsLeft, object sharedLock, Partitioner.SharedInt activePartitionCount, Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerable enumerable, bool useSingleChunking) : base(sharedReader, sharedIndex, useSingleChunking)
				{
					this.m_hasNoElementsLeft = hasNoElementsLeft;
					this.m_sharedLock = sharedLock;
					this.m_enumerable = enumerable;
					this.m_activePartitionCount = activePartitionCount;
					if (this.m_activePartitionCount != null)
					{
						Interlocked.Increment(ref this.m_activePartitionCount.Value);
					}
				}

				protected override bool GrabNextChunk(int requestedChunkSize)
				{
					if (this.HasNoElementsLeft)
					{
						return false;
					}
					if (this.m_localList == null)
					{
						this.m_localList = new KeyValuePair<long, TSource>[this.m_maxChunkSize];
					}
					return this.m_enumerable.GrabChunk(this.m_localList, requestedChunkSize, ref this.m_currentChunkSize.Value);
				}

				protected override bool HasNoElementsLeft
				{
					get
					{
						return this.m_hasNoElementsLeft.Value;
					}
					set
					{
						this.m_hasNoElementsLeft.Value = true;
					}
				}

				public override KeyValuePair<long, TSource> Current
				{
					get
					{
						if (this.m_currentChunkSize == null)
						{
							throw new InvalidOperationException(Environment.GetResourceString("PartitionerStatic_CurrentCalledBeforeMoveNext"));
						}
						return this.m_localList[this.m_localOffset.Value];
					}
				}

				public override void Dispose()
				{
					if (this.m_activePartitionCount != null && Interlocked.Decrement(ref this.m_activePartitionCount.Value) == 0)
					{
						this.m_enumerable.Dispose();
					}
				}

				private KeyValuePair<long, TSource>[] m_localList;

				private readonly Partitioner.SharedBool m_hasNoElementsLeft;

				private readonly object m_sharedLock;

				private readonly Partitioner.SharedInt m_activePartitionCount;

				private Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerable m_enumerable;
			}
		}

		private abstract class DynamicPartitionerForIndexRange_Abstract<TSource, TCollection> : OrderablePartitioner<TSource>
		{
			protected DynamicPartitionerForIndexRange_Abstract(TCollection data) : base(true, false, true)
			{
				this.m_data = data;
			}

			protected abstract IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions_Factory(TCollection data);

			public override IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount)
			{
				if (partitionCount <= 0)
				{
					throw new ArgumentOutOfRangeException("partitionCount");
				}
				IEnumerator<KeyValuePair<long, TSource>>[] array = new IEnumerator<KeyValuePair<long, TSource>>[partitionCount];
				IEnumerable<KeyValuePair<long, TSource>> orderableDynamicPartitions_Factory = this.GetOrderableDynamicPartitions_Factory(this.m_data);
				for (int i = 0; i < partitionCount; i++)
				{
					array[i] = orderableDynamicPartitions_Factory.GetEnumerator();
				}
				return array;
			}

			public override IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions()
			{
				return this.GetOrderableDynamicPartitions_Factory(this.m_data);
			}

			public override bool SupportsDynamicPartitions
			{
				get
				{
					return true;
				}
			}

			private TCollection m_data;
		}

		private abstract class DynamicPartitionEnumeratorForIndexRange_Abstract<TSource, TSourceReader> : Partitioner.DynamicPartitionEnumerator_Abstract<TSource, TSourceReader>
		{
			protected DynamicPartitionEnumeratorForIndexRange_Abstract(TSourceReader sharedReader, Partitioner.SharedLong sharedIndex) : base(sharedReader, sharedIndex)
			{
			}

			protected abstract int SourceCount { get; }

			protected override bool GrabNextChunk(int requestedChunkSize)
			{
				while (!this.HasNoElementsLeft)
				{
					long num = Volatile.Read(ref this.m_sharedIndex.Value);
					if (this.HasNoElementsLeft)
					{
						return false;
					}
					long num2 = Math.Min((long)(this.SourceCount - 1), num + (long)requestedChunkSize);
					if (Interlocked.CompareExchange(ref this.m_sharedIndex.Value, num2, num) == num)
					{
						this.m_currentChunkSize.Value = (int)(num2 - num);
						this.m_localOffset.Value = -1;
						this.m_startIndex = (int)(num + 1L);
						return true;
					}
				}
				return false;
			}

			protected override bool HasNoElementsLeft
			{
				get
				{
					return Volatile.Read(ref this.m_sharedIndex.Value) >= (long)(this.SourceCount - 1);
				}
				set
				{
				}
			}

			public override void Dispose()
			{
			}

			protected int m_startIndex;
		}

		private class DynamicPartitionerForIList<TSource> : Partitioner.DynamicPartitionerForIndexRange_Abstract<TSource, IList<TSource>>
		{
			internal DynamicPartitionerForIList(IList<TSource> source) : base(source)
			{
			}

			protected override IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions_Factory(IList<TSource> m_data)
			{
				return new Partitioner.DynamicPartitionerForIList<TSource>.InternalPartitionEnumerable(m_data);
			}

			private class InternalPartitionEnumerable : IEnumerable<KeyValuePair<long, TSource>>, IEnumerable
			{
				internal InternalPartitionEnumerable(IList<TSource> sharedReader)
				{
					this.m_sharedReader = sharedReader;
					this.m_sharedIndex = new Partitioner.SharedLong(-1L);
				}

				public IEnumerator<KeyValuePair<long, TSource>> GetEnumerator()
				{
					return new Partitioner.DynamicPartitionerForIList<TSource>.InternalPartitionEnumerator(this.m_sharedReader, this.m_sharedIndex);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				private readonly IList<TSource> m_sharedReader;

				private Partitioner.SharedLong m_sharedIndex;
			}

			private class InternalPartitionEnumerator : Partitioner.DynamicPartitionEnumeratorForIndexRange_Abstract<TSource, IList<TSource>>
			{
				internal InternalPartitionEnumerator(IList<TSource> sharedReader, Partitioner.SharedLong sharedIndex) : base(sharedReader, sharedIndex)
				{
				}

				protected override int SourceCount
				{
					get
					{
						return this.m_sharedReader.Count;
					}
				}

				public override KeyValuePair<long, TSource> Current
				{
					get
					{
						if (this.m_currentChunkSize == null)
						{
							throw new InvalidOperationException(Environment.GetResourceString("PartitionerStatic_CurrentCalledBeforeMoveNext"));
						}
						return new KeyValuePair<long, TSource>((long)(this.m_startIndex + this.m_localOffset.Value), this.m_sharedReader[this.m_startIndex + this.m_localOffset.Value]);
					}
				}
			}
		}

		private class DynamicPartitionerForArray<TSource> : Partitioner.DynamicPartitionerForIndexRange_Abstract<TSource, TSource[]>
		{
			internal DynamicPartitionerForArray(TSource[] source) : base(source)
			{
			}

			protected override IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions_Factory(TSource[] m_data)
			{
				return new Partitioner.DynamicPartitionerForArray<TSource>.InternalPartitionEnumerable(m_data);
			}

			private class InternalPartitionEnumerable : IEnumerable<KeyValuePair<long, TSource>>, IEnumerable
			{
				internal InternalPartitionEnumerable(TSource[] sharedReader)
				{
					this.m_sharedReader = sharedReader;
					this.m_sharedIndex = new Partitioner.SharedLong(-1L);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				public IEnumerator<KeyValuePair<long, TSource>> GetEnumerator()
				{
					return new Partitioner.DynamicPartitionerForArray<TSource>.InternalPartitionEnumerator(this.m_sharedReader, this.m_sharedIndex);
				}

				private readonly TSource[] m_sharedReader;

				private Partitioner.SharedLong m_sharedIndex;
			}

			private class InternalPartitionEnumerator : Partitioner.DynamicPartitionEnumeratorForIndexRange_Abstract<TSource, TSource[]>
			{
				internal InternalPartitionEnumerator(TSource[] sharedReader, Partitioner.SharedLong sharedIndex) : base(sharedReader, sharedIndex)
				{
				}

				protected override int SourceCount
				{
					get
					{
						return this.m_sharedReader.Length;
					}
				}

				public override KeyValuePair<long, TSource> Current
				{
					get
					{
						if (this.m_currentChunkSize == null)
						{
							throw new InvalidOperationException(Environment.GetResourceString("PartitionerStatic_CurrentCalledBeforeMoveNext"));
						}
						return new KeyValuePair<long, TSource>((long)(this.m_startIndex + this.m_localOffset.Value), this.m_sharedReader[this.m_startIndex + this.m_localOffset.Value]);
					}
				}
			}
		}

		private abstract class StaticIndexRangePartitioner<TSource, TCollection> : OrderablePartitioner<TSource>
		{
			protected StaticIndexRangePartitioner() : base(true, true, true)
			{
			}

			protected abstract int SourceCount { get; }

			protected abstract IEnumerator<KeyValuePair<long, TSource>> CreatePartition(int startIndex, int endIndex);

			public override IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount)
			{
				if (partitionCount <= 0)
				{
					throw new ArgumentOutOfRangeException("partitionCount");
				}
				int num2;
				int num = Math.DivRem(this.SourceCount, partitionCount, out num2);
				IEnumerator<KeyValuePair<long, TSource>>[] array = new IEnumerator<KeyValuePair<long, TSource>>[partitionCount];
				int num3 = -1;
				for (int i = 0; i < partitionCount; i++)
				{
					int num4 = num3 + 1;
					if (i < num2)
					{
						num3 = num4 + num;
					}
					else
					{
						num3 = num4 + num - 1;
					}
					array[i] = this.CreatePartition(num4, num3);
				}
				return array;
			}
		}

		private abstract class StaticIndexRangePartition<TSource> : IEnumerator<KeyValuePair<long, TSource>>, IDisposable, IEnumerator
		{
			protected StaticIndexRangePartition(int startIndex, int endIndex)
			{
				this.m_startIndex = startIndex;
				this.m_endIndex = endIndex;
				this.m_offset = startIndex - 1;
			}

			public abstract KeyValuePair<long, TSource> Current { get; }

			public void Dispose()
			{
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}

			public bool MoveNext()
			{
				if (this.m_offset < this.m_endIndex)
				{
					this.m_offset++;
					return true;
				}
				this.m_offset = this.m_endIndex + 1;
				return false;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			protected readonly int m_startIndex;

			protected readonly int m_endIndex;

			protected volatile int m_offset;
		}

		private class StaticIndexRangePartitionerForIList<TSource> : Partitioner.StaticIndexRangePartitioner<TSource, IList<TSource>>
		{
			internal StaticIndexRangePartitionerForIList(IList<TSource> list)
			{
				this.m_list = list;
			}

			protected override int SourceCount
			{
				get
				{
					return this.m_list.Count;
				}
			}

			protected override IEnumerator<KeyValuePair<long, TSource>> CreatePartition(int startIndex, int endIndex)
			{
				return new Partitioner.StaticIndexRangePartitionForIList<TSource>(this.m_list, startIndex, endIndex);
			}

			private IList<TSource> m_list;
		}

		private class StaticIndexRangePartitionForIList<TSource> : Partitioner.StaticIndexRangePartition<TSource>
		{
			internal StaticIndexRangePartitionForIList(IList<TSource> list, int startIndex, int endIndex) : base(startIndex, endIndex)
			{
				this.m_list = list;
			}

			public override KeyValuePair<long, TSource> Current
			{
				get
				{
					if (this.m_offset < this.m_startIndex)
					{
						throw new InvalidOperationException(Environment.GetResourceString("PartitionerStatic_CurrentCalledBeforeMoveNext"));
					}
					return new KeyValuePair<long, TSource>((long)this.m_offset, this.m_list[this.m_offset]);
				}
			}

			private volatile IList<TSource> m_list;
		}

		private class StaticIndexRangePartitionerForArray<TSource> : Partitioner.StaticIndexRangePartitioner<TSource, TSource[]>
		{
			internal StaticIndexRangePartitionerForArray(TSource[] array)
			{
				this.m_array = array;
			}

			protected override int SourceCount
			{
				get
				{
					return this.m_array.Length;
				}
			}

			protected override IEnumerator<KeyValuePair<long, TSource>> CreatePartition(int startIndex, int endIndex)
			{
				return new Partitioner.StaticIndexRangePartitionForArray<TSource>(this.m_array, startIndex, endIndex);
			}

			private TSource[] m_array;
		}

		private class StaticIndexRangePartitionForArray<TSource> : Partitioner.StaticIndexRangePartition<TSource>
		{
			internal StaticIndexRangePartitionForArray(TSource[] array, int startIndex, int endIndex) : base(startIndex, endIndex)
			{
				this.m_array = array;
			}

			public override KeyValuePair<long, TSource> Current
			{
				get
				{
					if (this.m_offset < this.m_startIndex)
					{
						throw new InvalidOperationException(Environment.GetResourceString("PartitionerStatic_CurrentCalledBeforeMoveNext"));
					}
					return new KeyValuePair<long, TSource>((long)this.m_offset, this.m_array[this.m_offset]);
				}
			}

			private volatile TSource[] m_array;
		}

		private class SharedInt
		{
			internal SharedInt(int value)
			{
				this.Value = value;
			}

			internal volatile int Value;
		}

		private class SharedBool
		{
			internal SharedBool(bool value)
			{
				this.Value = value;
			}

			internal volatile bool Value;
		}

		private class SharedLong
		{
			internal SharedLong(long value)
			{
				this.Value = value;
			}

			internal long Value;
		}
	}
}
