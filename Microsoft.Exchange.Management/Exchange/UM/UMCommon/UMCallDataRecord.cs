using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	public class UMCallDataRecord : UMCallReportBase
	{
		public UMCallDataRecord(ObjectId identity) : base(identity)
		{
		}

		public DateTime Date
		{
			get
			{
				return (DateTime)this[UMCallDataRecordSchema.Date];
			}
			internal set
			{
				this[UMCallDataRecordSchema.Date] = value;
			}
		}

		public TimeSpan Duration
		{
			get
			{
				return (TimeSpan)this[UMCallDataRecordSchema.Duration];
			}
			internal set
			{
				this[UMCallDataRecordSchema.Duration] = value;
			}
		}

		public string AudioCodec
		{
			get
			{
				return (string)this[UMCallDataRecordSchema.AudioCodec];
			}
			internal set
			{
				this[UMCallDataRecordSchema.AudioCodec] = value;
			}
		}

		public string DialPlan
		{
			get
			{
				return (string)this[UMCallDataRecordSchema.DialPlan];
			}
			internal set
			{
				this[UMCallDataRecordSchema.DialPlan] = value;
			}
		}

		public string CallType
		{
			get
			{
				return (string)this[UMCallDataRecordSchema.CallType];
			}
			internal set
			{
				this[UMCallDataRecordSchema.CallType] = value;
			}
		}

		public string CallingNumber
		{
			get
			{
				return (string)this[UMCallDataRecordSchema.CallingNumber];
			}
			internal set
			{
				this[UMCallDataRecordSchema.CallingNumber] = value;
			}
		}

		public string CalledNumber
		{
			get
			{
				return (string)this[UMCallDataRecordSchema.CalledNumber];
			}
			internal set
			{
				this[UMCallDataRecordSchema.CalledNumber] = value;
			}
		}

		public string Gateway
		{
			get
			{
				return (string)this[UMCallDataRecordSchema.Gateway];
			}
			internal set
			{
				this[UMCallDataRecordSchema.Gateway] = value;
			}
		}

		public string UserMailboxName
		{
			get
			{
				return (string)this[UMCallDataRecordSchema.UserMailboxName];
			}
			internal set
			{
				this[UMCallDataRecordSchema.UserMailboxName] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return UMCallDataRecord.schema;
			}
		}

		private static UMCallDataRecordSchema schema = ObjectSchema.GetInstance<UMCallDataRecordSchema>();
	}
}
