using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ComponentDataPool
	{
		public int InternalVersion
		{
			get
			{
				return this.internalVersion;
			}
			set
			{
				this.internalVersion = value;
			}
		}

		public int ExternalVersion
		{
			get
			{
				return this.externalVersion;
			}
			set
			{
				this.externalVersion = value;
			}
		}

		public BinaryReader ConstStringDataReader
		{
			get
			{
				if (this.constStringDataReader == null)
				{
					this.constStringDataReader = new BinaryReader(new MemoryStream(128));
				}
				return this.constStringDataReader;
			}
		}

		public byte[] CopyBuffer
		{
			get
			{
				if (this.copyBuffer == null)
				{
					this.copyBuffer = new byte[128];
				}
				return this.copyBuffer;
			}
		}

		public DateTimeData GetDateTimeDataInstance()
		{
			if (this.dateTimeData == null)
			{
				this.dateTimeData = new DateTimeData();
			}
			return this.dateTimeData;
		}

		public NullableDateTimeData GetNullableDateTimeDataInstance()
		{
			if (this.nullableDateTimeData == null)
			{
				this.nullableDateTimeData = new NullableDateTimeData();
			}
			return this.nullableDateTimeData;
		}

		public StringData GetStringDataInstance()
		{
			if (this.stringData == null)
			{
				this.stringData = new StringData();
			}
			return this.stringData;
		}

		public ConstStringData GetConstStringDataInstance()
		{
			if (this.constStringData == null)
			{
				this.constStringData = new ConstStringData();
			}
			return this.constStringData;
		}

		public ByteData GetByteDataInstance()
		{
			if (this.byteData == null)
			{
				this.byteData = new ByteData();
			}
			return this.byteData;
		}

		public ArrayData<NullableData<Int32Data, int>, int?> GetNullableInt32ArrayInstance()
		{
			if (this.nullableInt32ArrayData == null)
			{
				this.nullableInt32ArrayData = new ArrayData<NullableData<Int32Data, int>, int?>();
			}
			return this.nullableInt32ArrayData;
		}

		public ArrayData<Int32Data, int> GetInt32ArrayInstance()
		{
			if (this.int32ArrayData == null)
			{
				this.int32ArrayData = new ArrayData<Int32Data, int>();
			}
			return this.int32ArrayData;
		}

		public ByteArrayData GetByteArrayInstance()
		{
			if (this.byteArrayData == null)
			{
				this.byteArrayData = new ByteArrayData();
			}
			return this.byteArrayData;
		}

		public BooleanData GetBooleanDataInstance()
		{
			if (this.booleanData == null)
			{
				this.booleanData = new BooleanData();
			}
			return this.booleanData;
		}

		public StoreObjectIdData GetStoreObjectIdDataInstance()
		{
			if (this.storeObjectIdData == null)
			{
				this.storeObjectIdData = new StoreObjectIdData();
			}
			return this.storeObjectIdData;
		}

		public ADObjectIdData GetADObjectIdDataInstance()
		{
			if (this.adObjectIdData == null)
			{
				this.adObjectIdData = new ADObjectIdData();
			}
			return this.adObjectIdData;
		}

		public Int32Data GetInt32DataInstance()
		{
			if (this.int32Data == null)
			{
				this.int32Data = new Int32Data();
			}
			return this.int32Data;
		}

		public UInt32Data GetUInt32DataInstance()
		{
			if (this.uint32Data == null)
			{
				this.uint32Data = new UInt32Data();
			}
			return this.uint32Data;
		}

		public Int64Data GetInt64DataInstance()
		{
			if (this.int64Data == null)
			{
				this.int64Data = new Int64Data();
			}
			return this.int64Data;
		}

		public DerivedData<ISyncWatermark> GetISyncWatermarkDataInstance()
		{
			if (this.syncWatermarkDataInstance == null)
			{
				this.syncWatermarkDataInstance = new DerivedData<ISyncWatermark>();
			}
			return this.syncWatermarkDataInstance;
		}

		public DerivedData<ISyncItemId> GetISyncItemIdDataInstance()
		{
			if (this.syncItemIdDataInstance == null)
			{
				this.syncItemIdDataInstance = new DerivedData<ISyncItemId>();
			}
			return this.syncItemIdDataInstance;
		}

		public ConversationIdData GetConversationIdDataInstance()
		{
			if (this.conversationIdDataInstance == null)
			{
				this.conversationIdDataInstance = new ConversationIdData();
			}
			return this.conversationIdDataInstance;
		}

		private int internalVersion;

		private int externalVersion;

		private DateTimeData dateTimeData;

		private StringData stringData;

		private NullableDateTimeData nullableDateTimeData;

		private ConstStringData constStringData;

		private ByteData byteData;

		private ArrayData<NullableData<Int32Data, int>, int?> nullableInt32ArrayData;

		private ArrayData<Int32Data, int> int32ArrayData;

		private ByteArrayData byteArrayData;

		private BooleanData booleanData;

		private StoreObjectIdData storeObjectIdData;

		private ADObjectIdData adObjectIdData;

		private Int32Data int32Data;

		private UInt32Data uint32Data;

		private Int64Data int64Data;

		private DerivedData<ISyncWatermark> syncWatermarkDataInstance;

		private DerivedData<ISyncItemId> syncItemIdDataInstance;

		private ConversationIdData conversationIdDataInstance;

		private BinaryReader constStringDataReader;

		private byte[] copyBuffer;
	}
}
