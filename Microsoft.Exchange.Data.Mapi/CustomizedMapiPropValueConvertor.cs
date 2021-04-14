using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal static class CustomizedMapiPropValueConvertor
	{
		public static object ExtractNullableEnhancedTimeSpanFromDays(PropValue value, MapiPropertyDefinition propertyDefinition)
		{
			if (typeof(EnhancedTimeSpan?) == propertyDefinition.Type)
			{
				object obj = null;
				if (MapiPropValueConvertor.TryCastValueToExtract(value, typeof(double), out obj))
				{
					double num = (double)obj;
					return (0.0 == num || -1.0 == num) ? null : new EnhancedTimeSpan?(EnhancedTimeSpan.FromDays((double)obj));
				}
			}
			throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static PropValue PackNullableEnhancedTimeSpanIntoDays(object value, MapiPropertyDefinition propertyDefinition)
		{
			if (value == null || typeof(EnhancedTimeSpan) == value.GetType())
			{
				PropType type = propertyDefinition.PropertyTag.ValueType();
				object value2 = null;
				if (MapiPropValueConvertor.TryCastValueToPack((value == null) ? 0.0 : ((EnhancedTimeSpan)value).TotalDays, type, out value2))
				{
					return new PropValue(propertyDefinition.PropertyTag, value2);
				}
			}
			throw MapiPropValueConvertor.ConstructPackingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static object ExtractMultiAnsiStringsFromBytes(PropValue value, MapiPropertyDefinition propertyDefinition)
		{
			if (typeof(string) == propertyDefinition.Type && propertyDefinition.IsMultivalued)
			{
				byte[] bytes = value.GetBytes();
				List<string> list = new List<string>();
				int num = 0;
				int num2 = 0;
				while (bytes.Length > num2)
				{
					if (bytes[num2] == 0)
					{
						list.Add(Encoding.ASCII.GetString(bytes, num, num2 - num));
						num = 1 + num2;
					}
					num2++;
				}
				return new MultiValuedProperty<string>(propertyDefinition.IsReadOnly, propertyDefinition, list);
			}
			throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static PropValue PackMultiAnsiStringsIntoBytes(object value, MapiPropertyDefinition propertyDefinition)
		{
			if (typeof(MultiValuedProperty<string>) == value.GetType())
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)value;
				List<byte> list = new List<byte>();
				foreach (string s in multiValuedProperty)
				{
					byte[] bytes = Encoding.ASCII.GetBytes(s);
					list.AddRange(bytes);
					if (bytes[bytes.Length - 1] != 0)
					{
						list.Add(0);
					}
				}
				return new PropValue(propertyDefinition.PropertyTag, list.ToArray());
			}
			throw MapiPropValueConvertor.ConstructPackingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static object ExtractNullableEnhancedTimeSpanFromSeconds(PropValue value, MapiPropertyDefinition propertyDefinition)
		{
			if (typeof(EnhancedTimeSpan?) == propertyDefinition.Type)
			{
				object obj = null;
				if (MapiPropValueConvertor.TryCastValueToExtract(value, typeof(double), out obj))
				{
					return new EnhancedTimeSpan?(EnhancedTimeSpan.FromSeconds((double)obj));
				}
			}
			throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static PropValue PackNullableEnhancedTimeSpanIntoSeconds(object value, MapiPropertyDefinition propertyDefinition)
		{
			if (value != null && typeof(EnhancedTimeSpan) == value.GetType())
			{
				object value2 = null;
				if (MapiPropValueConvertor.TryCastValueToPack(((EnhancedTimeSpan)value).TotalSeconds, propertyDefinition.PropertyTag.ValueType(), out value2))
				{
					return new PropValue(propertyDefinition.PropertyTag, value2);
				}
			}
			throw MapiPropValueConvertor.ConstructPackingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static object ExtractNullableUnlimitedByteQuantifiedSizeFromKilobytes(PropValue value, MapiPropertyDefinition propertyDefinition)
		{
			if (typeof(Unlimited<ByteQuantifiedSize>?) == propertyDefinition.Type)
			{
				object obj = null;
				if (MapiPropValueConvertor.TryCastValueToExtract(value, typeof(long), out obj))
				{
					long num = (long)obj;
					if (0L > num)
					{
						return new Unlimited<ByteQuantifiedSize>?(Unlimited<ByteQuantifiedSize>.UnlimitedValue);
					}
					return new Unlimited<ByteQuantifiedSize>?(new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(checked((ulong)num))));
				}
			}
			throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static PropValue PackNullableUnlimitedByteQuantifiedSizeIntoKilobytes(object value, MapiPropertyDefinition propertyDefinition)
		{
			if (value == null || typeof(Unlimited<ByteQuantifiedSize>) == value.GetType())
			{
				long num = -1L;
				if (value != null)
				{
					Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)value;
					if (unlimited.IsUnlimited)
					{
						num = -1L;
					}
					else
					{
						num = checked((long)unlimited.Value.ToKB());
					}
				}
				object value2 = null;
				if (MapiPropValueConvertor.TryCastValueToPack(num, propertyDefinition.PropertyTag.ValueType(), out value2))
				{
					return new PropValue(propertyDefinition.PropertyTag, value2);
				}
			}
			throw MapiPropValueConvertor.ConstructPackingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static object ExtractIpV4StringFromIpV6Bytes(PropValue value, MapiPropertyDefinition propertyDefinition)
		{
			if (!(typeof(string) == propertyDefinition.Type))
			{
				throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ConstantNa);
			}
			byte[] bytes = value.GetBytes();
			if (16 == bytes.Length)
			{
				return string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					bytes[4],
					bytes[5],
					bytes[6],
					bytes[7]
				});
			}
			throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ErrorByteArrayLength(16.ToString(), bytes.Length.ToString()));
		}

		public static object ExtractMacAddressStringFromBytes(PropValue value, MapiPropertyDefinition propertyDefinition)
		{
			if (!(typeof(string) == propertyDefinition.Type))
			{
				throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ConstantNa);
			}
			byte[] bytes = value.GetBytes();
			if (6 == bytes.Length)
			{
				return string.Format("{0:X2}-{1:X2}-{2:X2}-{3:X2}-{4:X2}-{5:X2}", new object[]
				{
					bytes[0],
					bytes[1],
					bytes[2],
					bytes[3],
					bytes[4],
					bytes[5]
				});
			}
			throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ErrorByteArrayLength(6.ToString(), bytes.Length.ToString()));
		}

		public static object ExtractUnlimitedByteQuantifiedSizeFromBytes(PropValue value, MapiPropertyDefinition propertyDefinition)
		{
			if (typeof(Unlimited<ByteQuantifiedSize>) == propertyDefinition.Type)
			{
				object obj = null;
				if (MapiPropValueConvertor.TryCastValueToExtract(value, typeof(long), out obj))
				{
					long num = (long)obj;
					if (0L > num)
					{
						return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
					}
					return new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(checked((ulong)num)));
				}
			}
			throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static object ExtractUnlimitedByteQuantifiedSizeFromPages(PropValue value, MapiPropertyDefinition propertyDefinition)
		{
			if (typeof(Unlimited<ByteQuantifiedSize>) == propertyDefinition.Type)
			{
				object obj = null;
				if (MapiPropValueConvertor.TryCastValueToExtract(value, typeof(int), out obj))
				{
					long num = (long)((int)obj);
					if (0L > num)
					{
						return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
					}
					return new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(checked((ulong)num * 32UL * 1024UL)));
				}
			}
			throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ConstantNa);
		}

		public static PropValue PackNullableValueToBool(object value, MapiPropertyDefinition propertyDefinition)
		{
			if (value == null || typeof(bool) == value.GetType())
			{
				PropType type = propertyDefinition.PropertyTag.ValueType();
				object value2 = null;
				if (MapiPropValueConvertor.TryCastValueToPack((value == null) ? false : value, type, out value2))
				{
					return new PropValue(propertyDefinition.PropertyTag, value2);
				}
			}
			throw MapiPropValueConvertor.ConstructPackingException(value, propertyDefinition, Strings.ConstantNa);
		}
	}
}
