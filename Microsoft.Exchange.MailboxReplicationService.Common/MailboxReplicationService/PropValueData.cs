using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(long[]))]
	[KnownType(typeof(bool))]
	[DataContract]
	[KnownType(typeof(string))]
	[KnownType(typeof(byte[][]))]
	[KnownType(typeof(int))]
	[KnownType(typeof(int[]))]
	[KnownType(typeof(string[]))]
	[KnownType(typeof(byte[]))]
	[KnownType(typeof(byte))]
	[KnownType(typeof(long))]
	[KnownType(typeof(bool[]))]
	internal sealed class PropValueData
	{
		public PropValueData()
		{
		}

		[DataMember(IsRequired = true)]
		public int PropTag { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public object Value { get; set; }

		public PropValueData(PropTag propTag, object value)
		{
			if (value == null)
			{
				propTag = propTag.ChangePropType(PropType.Null);
			}
			if (value is PropertyError)
			{
				propTag = propTag.ChangePropType(PropType.Error);
				value = (int)((PropertyError)value).PropertyErrorCode;
			}
			else if (value is ErrorCode)
			{
				propTag = propTag.ChangePropType(PropType.Error);
				value = (int)((ErrorCode)value);
			}
			if (value is ExDateTime)
			{
				value = (DateTime)((ExDateTime)value);
			}
			this.PropTag = (int)propTag;
			this.Value = value;
		}

		public override string ToString()
		{
			return TraceUtils.DumpPropVal(new PropValue((PropTag)this.PropTag, this.Value));
		}

		internal int GetApproximateSize()
		{
			int num = 4;
			if (this.Value is string)
			{
				num += 2 * ((string)this.Value).Length;
			}
			else if (this.Value is int)
			{
				num += 4;
			}
			else if (this.Value is byte[])
			{
				num += ((byte[])this.Value).Length;
			}
			else if (this.Value is bool || this.Value is byte)
			{
				num++;
			}
			else if (this.Value is long)
			{
				num += 8;
			}
			else if (this.Value is string[])
			{
				string[] array = (string[])this.Value;
				foreach (string text in array)
				{
					num += 2 * text.Length;
				}
			}
			else if (this.Value is int[])
			{
				num += 4 * ((int[])this.Value).Length;
			}
			else if (this.Value is byte[][])
			{
				byte[][] array3 = (byte[][])this.Value;
				foreach (byte[] array5 in array3)
				{
					num += array5.Length;
				}
			}
			else if (this.Value is bool[])
			{
				num += ((bool[])this.Value).Length;
			}
			else if (this.Value is long[])
			{
				num += 8 * ((long[])this.Value).Length;
			}
			return num;
		}
	}
}
