using System;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Etw
{
	public sealed class GCPerHeapHistoryTraceEvent : TraceEvent
	{
		internal unsafe GCPerHeapHistoryTraceEvent(Guid providerGuid, string providerName, EtwTraceNativeComponents.EVENT_RECORD* rawData) : base(providerGuid, providerName, rawData)
		{
		}

		public int MemoryPressure
		{
			get
			{
				if (this.V4_0)
				{
					return base.GetInt32At(this.SizeOfGenData * 5);
				}
				return base.GetInt32At(this.SizeOfGenData * 5 + 8);
			}
		}

		public int GenCondemnedReasons
		{
			get
			{
				if (this.V4_0)
				{
					return base.GetInt32At(this.SizeOfGenData * 5 + 16);
				}
				return base.GetInt32At(this.SizeOfGenData * 5);
			}
		}

		public int GenCondemnedReasonsEx
		{
			get
			{
				return base.GetInt32At(this.SizeOfGenData * 5 + 4);
			}
		}

		public int HeapIndex
		{
			get
			{
				return base.GetInt32At(base.EventDataLength - 5);
			}
		}

		public int ClrInstanceID
		{
			get
			{
				return base.GetByteAt(base.EventDataLength - 1);
			}
		}

		public int EntriesInGenData
		{
			get
			{
				if (this.V4_5_Beta)
				{
					return 8;
				}
				return 10;
			}
		}

		public int SizeOfGenData
		{
			get
			{
				return base.HostSizePtr(this.EntriesInGenData);
			}
		}

		public int TotalSizeOfGenData
		{
			get
			{
				return this.SizeOfGenData * 5;
			}
		}

		public bool V4_0
		{
			get
			{
				if (base.Version == 0)
				{
					int num = base.HostSizePtr(10) * 5 + 25;
					return base.EventDataLength == num || base.EventDataLength == num - 4;
				}
				return false;
			}
		}

		public bool V4_5_Beta
		{
			get
			{
				if (base.Version == 0)
				{
					int num = base.HostSizePtr(8) * 5 + 25;
					return base.EventDataLength == num;
				}
				return false;
			}
		}

		public bool V4_5
		{
			get
			{
				return base.Version == 2;
			}
		}

		public bool VersionRecognized
		{
			get
			{
				return this.V4_0 || this.V4_5_Beta || this.V4_5;
			}
		}

		public GCPerHeapHistoryTraceEvent.GCPerHeapHistoryGenData GenData(Gens genNumber)
		{
			return new GCPerHeapHistoryTraceEvent.GCPerHeapHistoryGenData(this, this.SizeOfGenData * (int)genNumber);
		}

		private const int MaxxGenData = 5;

		public sealed class GCPerHeapHistoryGenData
		{
			internal GCPerHeapHistoryGenData(GCPerHeapHistoryTraceEvent container, int offset)
			{
				this.container = container;
				this.startOffset = offset;
			}

			public long SizeBefore
			{
				get
				{
					return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(0));
				}
			}

			public long SizeAfter
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(3));
					}
					return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(1));
				}
			}

			public long ObjSpaceBefore
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.SizeBefore - this.FreeListSpaceBefore - this.FreeObjSpaceBefore;
					}
					return -1L;
				}
			}

			public long Fragmentation
			{
				get
				{
					if (this.container.V4_0)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(4));
					}
					return this.FreeListSpaceAfter + this.FreeObjSpaceAfter;
				}
			}

			public long ObjSizeAfter
			{
				get
				{
					return this.SizeAfter - this.Fragmentation;
				}
			}

			public long FreeListSpaceBefore
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(1));
					}
					return -1L;
				}
			}

			public long FreeObjSpaceBefore
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(2));
					}
					return -1L;
				}
			}

			public long FreeListSpaceAfter
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(4));
					}
					if (this.container.V4_5_Beta)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(2));
					}
					return -1L;
				}
			}

			public long FreeObjSpaceAfter
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(5));
					}
					if (this.container.V4_5_Beta)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(3));
					}
					return -1L;
				}
			}

			public long In
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(6));
					}
					if (this.container.V4_5_Beta)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(4));
					}
					return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(5));
				}
			}

			public long Out
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(7));
					}
					if (this.container.V4_5_Beta)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(5));
					}
					return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(6));
				}
			}

			public long Budget
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(8));
					}
					if (this.container.V4_5_Beta)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(6));
					}
					return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(7));
				}
			}

			public long SurvRate
			{
				get
				{
					if (this.container.V4_5)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(9));
					}
					if (this.container.V4_5_Beta)
					{
						return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(7));
					}
					return this.container.GetIntPtrAt(this.startOffset + this.container.HostSizePtr(8));
				}
			}

			private readonly int startOffset;

			private GCPerHeapHistoryTraceEvent container;
		}
	}
}
