using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class TransportPropertyStreamReader : PropertyStreamReader
	{
		public TransportPropertyStreamReader(Stream stream) : base(stream)
		{
		}

		protected override string ConvertTypedKey(TypedValue key)
		{
			return KeySerializer.Deserialize(key);
		}

		protected override object ReadRawValue(StreamPropertyType propId)
		{
			if (propId <= (StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array))
			{
				switch (propId)
				{
				case StreamPropertyType.RoutingAddress:
				{
					base.Read(this.buffer, 4);
					int num = BitConverter.ToInt32(this.buffer, 0);
					if (num == 0)
					{
						return new RoutingAddress(string.Empty);
					}
					byte[] array = new byte[num];
					base.Read(array, num);
					return new RoutingAddress(array);
				}
				case StreamPropertyType.ADObjectId:
				case StreamPropertyType.ADObjectIdUTF8:
				{
					base.Read(this.buffer, 4);
					int num2 = BitConverter.ToInt32(this.buffer, 0);
					if (num2 < 16)
					{
						throw new FormatException("Extended property stream. Invalid ADObjectId content.");
					}
					byte[] array2 = new byte[num2];
					base.Read(array2, num2);
					Encoding encoding = (propId == StreamPropertyType.ADObjectId) ? Encoding.Unicode : Encoding.UTF8;
					return new ADObjectId(array2, encoding);
				}
				case StreamPropertyType.RecipientType:
					base.Read(this.buffer, 4);
					return (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)BitConverter.ToInt32(this.buffer, 0);
				case StreamPropertyType.ADObjectIdWithString:
				{
					base.Read(this.buffer, 4);
					int num3 = BitConverter.ToInt32(this.buffer, 0);
					byte[] array3 = new byte[num3];
					base.Read(array3, num3);
					return new ADObjectIdWithString(array3);
				}
				case StreamPropertyType.ProxyAddress:
				{
					base.Read(this.buffer, 4);
					int num4 = BitConverter.ToInt32(this.buffer, 0);
					byte[] array4 = new byte[num4];
					base.Read(array4, num4);
					return ProxyAddress.Parse(Encoding.UTF8.GetString(array4, 0, num4));
				}
				default:
					switch (propId)
					{
					case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					{
						RoutingAddress[] result;
						base.ReadArray<RoutingAddress>(propId, out result);
						return result;
					}
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					{
						ADObjectId[] result2;
						base.ReadArray<ADObjectId>(propId, out result2);
						return result2;
					}
					case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
					{
						ADObjectIdWithString[] result3;
						base.ReadArray<ADObjectIdWithString>(propId, out result3);
						return result3;
					}
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
					{
						ProxyAddress[] result4;
						base.ReadArray<ProxyAddress>(propId, out result4);
						return result4;
					}
					}
					break;
				}
			}
			else
			{
				switch (propId)
				{
				case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
				{
					List<RoutingAddress> result5;
					base.ReadList<RoutingAddress>(propId, out result5);
					return result5;
				}
				case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
				{
					List<ADObjectId> result6;
					base.ReadList<ADObjectId>(propId, out result6);
					return result6;
				}
				case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
					break;
				case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
				{
					List<ADObjectIdWithString> result7;
					base.ReadList<ADObjectIdWithString>(propId, out result7);
					return result7;
				}
				case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
				{
					List<ProxyAddress> result8;
					base.ReadList<ProxyAddress>(propId, out result8);
					return result8;
				}
				default:
					switch (propId)
					{
					case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					{
						List<RoutingAddress> result9;
						base.ReadMultiValuedProperty<RoutingAddress>(propId, out result9);
						return result9;
					}
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					{
						List<ADObjectId> result10;
						base.ReadMultiValuedProperty<ADObjectId>(propId, out result10);
						return result10;
					}
					case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					{
						List<ADObjectIdWithString> result11;
						base.ReadMultiValuedProperty<ADObjectIdWithString>(propId, out result11);
						return result11;
					}
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					{
						List<ProxyAddress> result12;
						base.ReadMultiValuedProperty<ProxyAddress>(propId, out result12);
						return result12;
					}
					}
					break;
				}
			}
			return base.ReadRawValue(propId);
		}
	}
}
