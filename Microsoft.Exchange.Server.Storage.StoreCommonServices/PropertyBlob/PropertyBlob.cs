using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PropertyBlob
{
	public static class PropertyBlob
	{
		private static bool TryFindDictionaryEntry(byte[] blob, int startOffset, int propertyCount, ushort id, out int index)
		{
			int num = 0;
			int i = propertyCount - 1;
			while (i >= num)
			{
				index = (num + i) / 2;
				int num2 = id.CompareTo((ushort)ParseSerialize.ParseInt16(blob, startOffset + 8 + index * 8 + 2));
				if (num2 < 0)
				{
					i = index - 1;
				}
				else
				{
					if (num2 <= 0)
					{
						return true;
					}
					num = index + 1;
				}
			}
			index = num;
			return false;
		}

		private static ushort IdFromTag(uint tag)
		{
			return (ushort)(tag >> 16);
		}

		private static PropertyType PropertyTypeFromTag(uint tag)
		{
			return (PropertyType)(tag & 65535U);
		}

		internal static PropertyBlob.CompressedPropertyType GetCompressedType(PropertyType propertyType)
		{
			if (propertyType <= PropertyType.Actions)
			{
				if (propertyType <= PropertyType.Unicode)
				{
					switch (propertyType)
					{
					case PropertyType.Null:
						return PropertyBlob.CompressedPropertyType.Null;
					case PropertyType.Int16:
						return PropertyBlob.CompressedPropertyType.Int16;
					case PropertyType.Int32:
						return PropertyBlob.CompressedPropertyType.Int32;
					case PropertyType.Real32:
						return PropertyBlob.CompressedPropertyType.Real32;
					case PropertyType.Real64:
						return PropertyBlob.CompressedPropertyType.Real64;
					case PropertyType.Currency:
						return PropertyBlob.CompressedPropertyType.Currency;
					case PropertyType.AppTime:
						return PropertyBlob.CompressedPropertyType.AppTime;
					case (PropertyType)8:
					case (PropertyType)9:
					case PropertyType.Error:
					case (PropertyType)12:
						break;
					case PropertyType.Boolean:
						return PropertyBlob.CompressedPropertyType.Boolean;
					case PropertyType.Object:
						return PropertyBlob.CompressedPropertyType.Object;
					default:
						if (propertyType == PropertyType.Int64)
						{
							return PropertyBlob.CompressedPropertyType.Int64;
						}
						if (propertyType == PropertyType.Unicode)
						{
							return PropertyBlob.CompressedPropertyType.Unicode;
						}
						break;
					}
				}
				else
				{
					if (propertyType == PropertyType.SysTime)
					{
						return PropertyBlob.CompressedPropertyType.SysTime;
					}
					if (propertyType == PropertyType.Guid)
					{
						return PropertyBlob.CompressedPropertyType.Guid;
					}
					switch (propertyType)
					{
					case PropertyType.SvrEid:
						return PropertyBlob.CompressedPropertyType.SvrEid;
					case PropertyType.SRestriction:
						return PropertyBlob.CompressedPropertyType.SRestriction;
					case PropertyType.Actions:
						return PropertyBlob.CompressedPropertyType.Actions;
					}
				}
			}
			else if (propertyType <= PropertyType.MVInt64)
			{
				if (propertyType == PropertyType.Binary)
				{
					return PropertyBlob.CompressedPropertyType.Binary;
				}
				switch (propertyType)
				{
				case PropertyType.MVInt16:
					return PropertyBlob.CompressedPropertyType.MVInt16;
				case PropertyType.MVInt32:
					return PropertyBlob.CompressedPropertyType.MVInt32;
				case PropertyType.MVReal32:
					return PropertyBlob.CompressedPropertyType.MVReal32;
				case PropertyType.MVReal64:
					return PropertyBlob.CompressedPropertyType.MVReal64;
				case PropertyType.MVCurrency:
					return PropertyBlob.CompressedPropertyType.MVCurrency;
				case PropertyType.MVAppTime:
					return PropertyBlob.CompressedPropertyType.MVAppTime;
				default:
					if (propertyType == PropertyType.MVInt64)
					{
						return PropertyBlob.CompressedPropertyType.MVInt64;
					}
					break;
				}
			}
			else if (propertyType <= PropertyType.MVSysTime)
			{
				if (propertyType == PropertyType.MVUnicode)
				{
					return PropertyBlob.CompressedPropertyType.MVUnicode;
				}
				if (propertyType == PropertyType.MVSysTime)
				{
					return PropertyBlob.CompressedPropertyType.MVSysTime;
				}
			}
			else
			{
				if (propertyType == PropertyType.MVGuid)
				{
					return PropertyBlob.CompressedPropertyType.MVGuid;
				}
				if (propertyType == PropertyType.MVBinary)
				{
					return PropertyBlob.CompressedPropertyType.MVBinary;
				}
			}
			throw new ArgumentException(string.Format("invalid or unexpected property type: {0:X}", (ushort)propertyType));
		}

		internal static bool CompatibleTypes(PropertyBlob.CompressedPropertyType persistedCompressedType, PropertyBlob.CompressedPropertyType desiredCompressedType)
		{
			if (persistedCompressedType > PropertyBlob.CompressedPropertyType.Real32)
			{
				if (persistedCompressedType <= PropertyBlob.CompressedPropertyType.SRestriction)
				{
					if (persistedCompressedType != PropertyBlob.CompressedPropertyType.Object && persistedCompressedType != PropertyBlob.CompressedPropertyType.SRestriction)
					{
						return false;
					}
				}
				else if (persistedCompressedType != PropertyBlob.CompressedPropertyType.Actions && persistedCompressedType != PropertyBlob.CompressedPropertyType.SvrEid)
				{
					return false;
				}
				return desiredCompressedType == PropertyBlob.CompressedPropertyType.Binary;
			}
			if (persistedCompressedType == PropertyBlob.CompressedPropertyType.Int16)
			{
				return desiredCompressedType == PropertyBlob.CompressedPropertyType.Int32 || desiredCompressedType == PropertyBlob.CompressedPropertyType.Int64;
			}
			if (persistedCompressedType == PropertyBlob.CompressedPropertyType.Int32)
			{
				return desiredCompressedType == PropertyBlob.CompressedPropertyType.Int64;
			}
			if (persistedCompressedType == PropertyBlob.CompressedPropertyType.Real32)
			{
				return desiredCompressedType == PropertyBlob.CompressedPropertyType.Real64;
			}
			return false;
		}

		private static SerializedValue.ValueFormat GetValueFormat(PropertyBlob.CompressedPropertyType compressedType)
		{
			return (SerializedValue.ValueFormat)(compressedType & (PropertyBlob.CompressedPropertyType)248);
		}

		private static PropertyType GetPropertyType(PropertyBlob.CompressedPropertyType compressedType)
		{
			if (compressedType <= PropertyBlob.CompressedPropertyType.Object)
			{
				if (compressedType <= PropertyBlob.CompressedPropertyType.Real32)
				{
					if (compressedType <= PropertyBlob.CompressedPropertyType.Int16)
					{
						if (compressedType == PropertyBlob.CompressedPropertyType.Null)
						{
							return PropertyType.Null;
						}
						if (compressedType == PropertyBlob.CompressedPropertyType.Boolean)
						{
							return PropertyType.Boolean;
						}
						if (compressedType == PropertyBlob.CompressedPropertyType.Int16)
						{
							return PropertyType.Int16;
						}
					}
					else if (compressedType <= PropertyBlob.CompressedPropertyType.Int64)
					{
						if (compressedType == PropertyBlob.CompressedPropertyType.Int32)
						{
							return PropertyType.Int32;
						}
						if (compressedType == PropertyBlob.CompressedPropertyType.Int64)
						{
							return PropertyType.Int64;
						}
					}
					else
					{
						if (compressedType == PropertyBlob.CompressedPropertyType.Currency)
						{
							return PropertyType.Currency;
						}
						if (compressedType == PropertyBlob.CompressedPropertyType.Real32)
						{
							return PropertyType.Real32;
						}
					}
				}
				else if (compressedType <= PropertyBlob.CompressedPropertyType.SysTime)
				{
					if (compressedType == PropertyBlob.CompressedPropertyType.Real64)
					{
						return PropertyType.Real64;
					}
					if (compressedType == PropertyBlob.CompressedPropertyType.AppTime)
					{
						return PropertyType.AppTime;
					}
					if (compressedType == PropertyBlob.CompressedPropertyType.SysTime)
					{
						return PropertyType.SysTime;
					}
				}
				else if (compressedType <= PropertyBlob.CompressedPropertyType.Unicode)
				{
					if (compressedType == PropertyBlob.CompressedPropertyType.Guid)
					{
						return PropertyType.Guid;
					}
					if (compressedType == PropertyBlob.CompressedPropertyType.Unicode)
					{
						return PropertyType.Unicode;
					}
				}
				else
				{
					if (compressedType == PropertyBlob.CompressedPropertyType.Binary)
					{
						return PropertyType.Binary;
					}
					if (compressedType == PropertyBlob.CompressedPropertyType.Object)
					{
						return PropertyType.Object;
					}
				}
			}
			else if (compressedType <= PropertyBlob.CompressedPropertyType.MVCurrency)
			{
				if (compressedType <= PropertyBlob.CompressedPropertyType.SvrEid)
				{
					if (compressedType == PropertyBlob.CompressedPropertyType.SRestriction)
					{
						return PropertyType.SRestriction;
					}
					if (compressedType == PropertyBlob.CompressedPropertyType.Actions)
					{
						return PropertyType.Actions;
					}
					if (compressedType == PropertyBlob.CompressedPropertyType.SvrEid)
					{
						return PropertyType.SvrEid;
					}
				}
				else if (compressedType <= PropertyBlob.CompressedPropertyType.MVInt32)
				{
					if (compressedType == PropertyBlob.CompressedPropertyType.MVInt16)
					{
						return PropertyType.MVInt16;
					}
					if (compressedType == PropertyBlob.CompressedPropertyType.MVInt32)
					{
						return PropertyType.MVInt32;
					}
				}
				else
				{
					if (compressedType == PropertyBlob.CompressedPropertyType.MVInt64)
					{
						return PropertyType.MVInt64;
					}
					if (compressedType == PropertyBlob.CompressedPropertyType.MVCurrency)
					{
						return PropertyType.MVCurrency;
					}
				}
			}
			else if (compressedType <= PropertyBlob.CompressedPropertyType.MVAppTime)
			{
				if (compressedType == PropertyBlob.CompressedPropertyType.MVReal32)
				{
					return PropertyType.MVReal32;
				}
				if (compressedType == PropertyBlob.CompressedPropertyType.MVReal64)
				{
					return PropertyType.MVReal64;
				}
				if (compressedType == PropertyBlob.CompressedPropertyType.MVAppTime)
				{
					return PropertyType.MVAppTime;
				}
			}
			else if (compressedType <= PropertyBlob.CompressedPropertyType.MVGuid)
			{
				if (compressedType == PropertyBlob.CompressedPropertyType.MVSysTime)
				{
					return PropertyType.MVSysTime;
				}
				if (compressedType == PropertyBlob.CompressedPropertyType.MVGuid)
				{
					return PropertyType.MVGuid;
				}
			}
			else
			{
				if (compressedType == PropertyBlob.CompressedPropertyType.MVUnicode)
				{
					return PropertyType.MVUnicode;
				}
				if (compressedType == PropertyBlob.CompressedPropertyType.MVBinary)
				{
					return PropertyType.MVBinary;
				}
			}
			throw new InvalidBlobException("invalid dictionary entry - compressedType");
		}

		[Conditional("DEBUG")]
		private static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				throw new Exception(message);
			}
		}

		public static byte[] BuildBlob(Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties)
		{
			return PropertyBlob.BuildBlob(properties, null);
		}

		public static byte[] BuildBlob(Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties, HashSet<ushort> defaultPromotedPropertyIds)
		{
			if (properties == null || properties.Count == 0)
			{
				return null;
			}
			List<ushort> list = new List<ushort>(properties.Count);
			int num = 8;
			foreach (KeyValuePair<ushort, KeyValuePair<StorePropTag, object>> keyValuePair in properties)
			{
				if (keyValuePair.Value.Value != null || defaultPromotedPropertyIds == null || !defaultPromotedPropertyIds.Contains(keyValuePair.Key))
				{
					int serializedSize = PropertyBlob.GetSerializedSize(keyValuePair.Value.Key.PropTag, keyValuePair.Value.Value);
					PropertyBlob.ThrowIfBlobOverflow(num, serializedSize + 8);
					num += 8 + serializedSize;
					list.Add(keyValuePair.Value.Key.PropId);
				}
			}
			if (num == 8)
			{
				return null;
			}
			byte[] array = new byte[num];
			int num2 = 0;
			int num3 = 8 + list.Count * 8;
			ParseSerialize.SerializeInt32(1349481040, array, 0);
			ParseSerialize.SerializeInt16(768, array, 4);
			ParseSerialize.SerializeInt16((short)list.Count, array, 6);
			list.Sort();
			foreach (ushort key in list)
			{
				KeyValuePair<StorePropTag, object> keyValuePair2 = properties[key];
				PropertyBlob.AddProperty(array, ref num2, ref num3, keyValuePair2.Key.PropTag, keyValuePair2.Value);
			}
			return array;
		}

		public static byte[] BuildBlob(IList<StorePropTag> propTags, IList<object> propValues)
		{
			int num = 8 + propTags.Count * 8;
			for (int i = 0; i < propTags.Count; i++)
			{
				int serializedSize = PropertyBlob.GetSerializedSize(propTags[i].PropTag, propValues[i]);
				PropertyBlob.ThrowIfBlobOverflow(num, serializedSize);
				num += serializedSize;
			}
			byte[] array = new byte[num];
			int num2 = 0;
			int num3 = 8 + propTags.Count * 8;
			ParseSerialize.SerializeInt32(1349481040, array, 0);
			ParseSerialize.SerializeInt16(768, array, 4);
			ParseSerialize.SerializeInt16((short)propTags.Count, array, 6);
			for (int j = 0; j < propTags.Count; j++)
			{
				PropertyBlob.AddProperty(array, ref num2, ref num3, propTags[j].PropTag, propValues[j]);
			}
			return array;
		}

		public static byte[] BuildBlob(IList<uint> propTags, IList<object> propValues)
		{
			int num = 8 + propTags.Count * 8;
			for (int i = 0; i < propTags.Count; i++)
			{
				int serializedSize = PropertyBlob.GetSerializedSize(propTags[i], propValues[i]);
				PropertyBlob.ThrowIfBlobOverflow(num, serializedSize);
				num += serializedSize;
			}
			byte[] array = new byte[num];
			int num2 = 0;
			int num3 = 8 + propTags.Count * 8;
			ParseSerialize.SerializeInt32(1349481040, array, 0);
			ParseSerialize.SerializeInt16(768, array, 4);
			ParseSerialize.SerializeInt16((short)propTags.Count, array, 6);
			for (int j = 0; j < propTags.Count; j++)
			{
				PropertyBlob.AddProperty(array, ref num2, ref num3, propTags[j], propValues[j]);
			}
			return array;
		}

		public static byte[] PromoteProperties(byte[] onPageBlob, byte[] offPageBlob, HashSet<ushort> additionalPromotedProperties)
		{
			PropertyBlob.BlobReader onPageBlobReader = new PropertyBlob.BlobReader(onPageBlob, 0);
			if (!PropertyBlob.IsPromotionNecessary(onPageBlobReader, additionalPromotedProperties))
			{
				return onPageBlob;
			}
			PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(offPageBlob, 0);
			Dictionary<ushort, KeyValuePair<StorePropTag, object>> dictionary = new Dictionary<ushort, KeyValuePair<StorePropTag, object>>(onPageBlobReader.PropertyCount + additionalPromotedProperties.Count);
			List<ushort> list = new List<ushort>(onPageBlobReader.PropertyCount + additionalPromotedProperties.Count);
			int num = 8;
			for (int i = 0; i < onPageBlobReader.PropertyCount; i++)
			{
				uint propertyTag = onPageBlobReader.GetPropertyTag(i);
				StorePropTag key = StorePropTag.CreateWithoutInfo(propertyTag);
				object propertyValue = onPageBlobReader.GetPropertyValue(i);
				if (propertyValue != null || additionalPromotedProperties.Contains(key.PropId) || blobReader.TestIfPropertyPresent(key.PropId))
				{
					dictionary.Add(key.PropId, new KeyValuePair<StorePropTag, object>(key, propertyValue));
					int serializedSize = PropertyBlob.GetSerializedSize(key.PropTag, propertyValue);
					PropertyBlob.ThrowIfBlobOverflow(num, serializedSize + 8);
					num += 8 + serializedSize;
					list.Add(key.PropId);
				}
			}
			int num2 = num;
			int count = list.Count;
			if (num < 3110)
			{
				HashSet<ushort> hashSet = null;
				for (;;)
				{
					num = num2;
					list.RemoveRange(count, list.Count - count);
					int num3 = 0;
					ushort item = ushort.MaxValue;
					foreach (ushort num4 in additionalPromotedProperties)
					{
						if (!onPageBlobReader.TestIfPropertyPresent(num4) && (hashSet == null || !hashSet.Contains(num4)))
						{
							int index;
							StorePropTag key2;
							object value;
							if (blobReader.TryFindPropertyById(num4, out index))
							{
								int propertyValueSize = blobReader.GetPropertyValueSize(index);
								if (propertyValueSize >= 512)
								{
									if (hashSet == null)
									{
										hashSet = new HashSet<ushort>();
									}
									hashSet.Add(num4);
									continue;
								}
								key2 = StorePropTag.CreateWithoutInfo(blobReader.GetPropertyTag(index));
								value = blobReader.GetPropertyValue(index);
							}
							else
							{
								key2 = StorePropTag.CreateWithoutInfo(num4, PropertyType.Null);
								value = null;
							}
							dictionary[num4] = new KeyValuePair<StorePropTag, object>(key2, value);
							int serializedSize2 = PropertyBlob.GetSerializedSize(key2.PropTag, value);
							if (serializedSize2 + 8 > num3)
							{
								num3 = serializedSize2 + 8;
								item = num4;
							}
							PropertyBlob.ThrowIfBlobOverflow(num, serializedSize2 + 8);
							num += 8 + serializedSize2;
							if (num > 3110)
							{
								break;
							}
							list.Add(num4);
						}
					}
					if (num <= 3110)
					{
						break;
					}
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(num3 != 0, "We must have some evictable property here");
					if (hashSet == null)
					{
						hashSet = new HashSet<ushort>();
					}
					hashSet.Add(item);
				}
			}
			if (num == num2)
			{
				return onPageBlob;
			}
			byte[] array = new byte[num];
			int num5 = 0;
			int num6 = 8 + list.Count * 8;
			ParseSerialize.SerializeInt32(1349481040, array, 0);
			ParseSerialize.SerializeInt16(768, array, 4);
			ParseSerialize.SerializeInt16((short)list.Count, array, 6);
			list.Sort();
			foreach (ushort key3 in list)
			{
				KeyValuePair<StorePropTag, object> keyValuePair = dictionary[key3];
				PropertyBlob.AddProperty(array, ref num5, ref num6, keyValuePair.Key.PropTag, keyValuePair.Value);
			}
			return array;
		}

		private static void AddProperty(byte[] blob, ref int currentPropertyCount, ref int currentValueOffset, uint tag, object value)
		{
			PropertyBlob.DictionaryEntry dictionaryEntry = PropertyBlob.BuildEntry(tag, value, currentValueOffset);
			int num;
			PropertyBlob.TryFindDictionaryEntry(blob, 0, currentPropertyCount, PropertyBlob.IdFromTag(tag), out num);
			int num2 = 8 + num * 8;
			if (num != currentPropertyCount)
			{
				Buffer.BlockCopy(blob, num2, blob, num2 + 8, (currentPropertyCount - num) * 8);
			}
			ParseSerialize.SerializeInt16((short)dictionaryEntry.Id, blob, num2 + 2);
			blob[num2] = (byte)dictionaryEntry.CompressedType;
			blob[num2 + 1] = (byte)dictionaryEntry.Format;
			ParseSerialize.SerializeInt32(dictionaryEntry.Offset, blob, num2 + 4);
			currentPropertyCount++;
			SerializedValue.ValueFormat format = dictionaryEntry.Format;
			if (format <= SerializedValue.ValueFormat.Binary)
			{
				if (format <= SerializedValue.ValueFormat.Int64)
				{
					if (format <= SerializedValue.ValueFormat.Boolean)
					{
						if (format != SerializedValue.ValueFormat.FormatModifierShift && format != SerializedValue.ValueFormat.Boolean)
						{
							return;
						}
						return;
					}
					else
					{
						if (format == SerializedValue.ValueFormat.Int16 || format == SerializedValue.ValueFormat.Int32)
						{
							return;
						}
						if (format != SerializedValue.ValueFormat.Int64)
						{
							return;
						}
					}
				}
				else if (format <= SerializedValue.ValueFormat.DateTime)
				{
					if (format != SerializedValue.ValueFormat.Single && format != SerializedValue.ValueFormat.Double && format != SerializedValue.ValueFormat.DateTime)
					{
						return;
					}
				}
				else if (format != SerializedValue.ValueFormat.Guid && format != SerializedValue.ValueFormat.String && format != SerializedValue.ValueFormat.Binary)
				{
					return;
				}
			}
			else if (format <= SerializedValue.ValueFormat.MVInt64)
			{
				if (format <= (SerializedValue.ValueFormat)124)
				{
					if (format != SerializedValue.ValueFormat.Reference)
					{
						if (format != (SerializedValue.ValueFormat)124)
						{
							return;
						}
						return;
					}
				}
				else if (format != SerializedValue.ValueFormat.MVInt16 && format != SerializedValue.ValueFormat.MVInt32 && format != SerializedValue.ValueFormat.MVInt64)
				{
					return;
				}
			}
			else if (format <= SerializedValue.ValueFormat.MVDateTime)
			{
				if (format != SerializedValue.ValueFormat.MVSingle && format != SerializedValue.ValueFormat.MVDouble && format != SerializedValue.ValueFormat.MVDateTime)
				{
					return;
				}
			}
			else if (format != SerializedValue.ValueFormat.MVGuid && format != SerializedValue.ValueFormat.MVString && format != SerializedValue.ValueFormat.MVBinary)
			{
				return;
			}
			SerializedValue.Serialize(dictionaryEntry.Format, value, blob, ref currentValueOffset);
		}

		private static PropertyBlob.DictionaryEntry BuildEntry(uint tag, object value, int currentValueOffset)
		{
			PropertyBlob.CompressedPropertyType compressedType = PropertyBlob.GetCompressedType(PropertyBlob.PropertyTypeFromTag(tag));
			SerializedValue.ValueFormat valueFormat;
			int offsetOrValue;
			if (value == null)
			{
				valueFormat = SerializedValue.ValueFormat.FormatModifierShift;
				offsetOrValue = 0;
			}
			else if (value is ValueReference)
			{
				if (((ValueReference)value).IsZero)
				{
					valueFormat = (SerializedValue.ValueFormat)124;
					offsetOrValue = 0;
				}
				else
				{
					valueFormat = SerializedValue.ValueFormat.Reference;
					offsetOrValue = currentValueOffset;
				}
			}
			else
			{
				valueFormat = PropertyBlob.GetValueFormat(compressedType);
				SerializedValue.ValueFormat valueFormat2 = valueFormat;
				if (valueFormat2 <= SerializedValue.ValueFormat.Int16)
				{
					if (valueFormat2 == SerializedValue.ValueFormat.Boolean)
					{
						offsetOrValue = (((bool)value) ? 1 : 0);
						goto IL_88;
					}
					if (valueFormat2 == SerializedValue.ValueFormat.Int16)
					{
						offsetOrValue = (int)((short)value);
						goto IL_88;
					}
				}
				else
				{
					if (valueFormat2 == SerializedValue.ValueFormat.Int32)
					{
						offsetOrValue = (int)value;
						goto IL_88;
					}
					if (valueFormat2 == SerializedValue.ValueFormat.Reserved2 || valueFormat2 == SerializedValue.ValueFormat.Reserved1)
					{
						valueFormat = SerializedValue.ValueFormat.Binary;
					}
				}
				offsetOrValue = currentValueOffset;
			}
			IL_88:
			return new PropertyBlob.DictionaryEntry(PropertyBlob.IdFromTag(tag), compressedType, valueFormat, offsetOrValue);
		}

		private static int GetSerializedSize(uint tag, object value)
		{
			if (value == null)
			{
				return 0;
			}
			SerializedValue.ValueFormat valueFormat;
			if (value is ValueReference)
			{
				if (((ValueReference)value).IsZero)
				{
					return 0;
				}
				valueFormat = SerializedValue.ValueFormat.Reference;
			}
			else
			{
				PropertyType propertyType = PropertyBlob.PropertyTypeFromTag(tag);
				PropertyBlob.CompressedPropertyType compressedType = PropertyBlob.GetCompressedType(propertyType);
				valueFormat = PropertyBlob.GetValueFormat(compressedType);
			}
			SerializedValue.ValueFormat valueFormat2 = valueFormat;
			if (valueFormat2 <= SerializedValue.ValueFormat.Binary)
			{
				if (valueFormat2 <= SerializedValue.ValueFormat.Single)
				{
					if (valueFormat2 <= SerializedValue.ValueFormat.Int16)
					{
						if (valueFormat2 == SerializedValue.ValueFormat.Boolean)
						{
							return 0;
						}
						if (valueFormat2 != SerializedValue.ValueFormat.Int16)
						{
							goto IL_12C;
						}
						return 0;
					}
					else
					{
						if (valueFormat2 == SerializedValue.ValueFormat.Int32)
						{
							return 0;
						}
						if (valueFormat2 != SerializedValue.ValueFormat.Int64 && valueFormat2 != SerializedValue.ValueFormat.Single)
						{
							goto IL_12C;
						}
					}
				}
				else if (valueFormat2 <= SerializedValue.ValueFormat.DateTime)
				{
					if (valueFormat2 != SerializedValue.ValueFormat.Double && valueFormat2 != SerializedValue.ValueFormat.DateTime)
					{
						goto IL_12C;
					}
				}
				else if (valueFormat2 != SerializedValue.ValueFormat.Guid && valueFormat2 != SerializedValue.ValueFormat.String && valueFormat2 != SerializedValue.ValueFormat.Binary)
				{
					goto IL_12C;
				}
			}
			else if (valueFormat2 <= SerializedValue.ValueFormat.MVInt64)
			{
				if (valueFormat2 <= SerializedValue.ValueFormat.Reserved1)
				{
					if (valueFormat2 != SerializedValue.ValueFormat.Reserved2 && valueFormat2 != SerializedValue.ValueFormat.Reserved1)
					{
						goto IL_12C;
					}
					valueFormat = SerializedValue.ValueFormat.Binary;
				}
				else if (valueFormat2 != SerializedValue.ValueFormat.MVInt16 && valueFormat2 != SerializedValue.ValueFormat.MVInt32 && valueFormat2 != SerializedValue.ValueFormat.MVInt64)
				{
					goto IL_12C;
				}
			}
			else if (valueFormat2 <= SerializedValue.ValueFormat.MVDateTime)
			{
				if (valueFormat2 != SerializedValue.ValueFormat.MVSingle && valueFormat2 != SerializedValue.ValueFormat.MVDouble && valueFormat2 != SerializedValue.ValueFormat.MVDateTime)
				{
					goto IL_12C;
				}
			}
			else if (valueFormat2 != SerializedValue.ValueFormat.MVGuid && valueFormat2 != SerializedValue.ValueFormat.MVString && valueFormat2 != SerializedValue.ValueFormat.MVBinary)
			{
				goto IL_12C;
			}
			return SerializedValue.ComputeSize(valueFormat, value);
			IL_12C:
			throw new ArgumentException("invalid or unexpected property type");
		}

		private static bool IsPromotionNecessary(PropertyBlob.BlobReader onPageBlobReader, HashSet<ushort> additionalPromotedProperties)
		{
			foreach (ushort id in additionalPromotedProperties)
			{
				if (!onPageBlobReader.TestIfPropertyPresent(id))
				{
					return true;
				}
			}
			return false;
		}

		internal static void ThrowIfBlobOverflow(int blobSize, int propertySize)
		{
			if (propertySize < 0 || blobSize + propertySize < blobSize)
			{
				throw new InvalidOperationException("Invalid size");
			}
		}

		public static void BuildTwoBlobs(Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties, HashSet<ushort> alwaysPromotedProperties, HashSet<ushort> defaultPromotedProperties, HashSet<ushort> additionalPromotedProperties, out byte[] onPageBlob, out object offPageBlob)
		{
			onPageBlob = null;
			offPageBlob = null;
			int val = (properties == null) ? 0 : properties.Count;
			int num = (defaultPromotedProperties == null) ? 0 : defaultPromotedProperties.Count;
			int num2 = (additionalPromotedProperties == null) ? 0 : additionalPromotedProperties.Count;
			List<ushort> list = new List<ushort>(Math.Max(val, num + num2));
			HashSet<ushort> hashSet = null;
			int num3;
			for (;;)
			{
				num3 = 8;
				list.Clear();
				int num4 = 0;
				ushort item = ushort.MaxValue;
				if (properties != null && defaultPromotedProperties != null)
				{
					foreach (ushort num5 in defaultPromotedProperties)
					{
						KeyValuePair<StorePropTag, object> keyValuePair;
						if (properties.TryGetValue(num5, out keyValuePair))
						{
							object value = keyValuePair.Value;
							bool flag = true;
							if (hashSet != null && hashSet.Contains(num5))
							{
								value = ValueReference.Zero;
								flag = false;
							}
							else if (alwaysPromotedProperties != null && alwaysPromotedProperties.Contains(keyValuePair.Key.PropId))
							{
								flag = false;
							}
							else if (ValueTypeHelper.ValueSize(PropertyTypeHelper.GetExtendedTypeCode(keyValuePair.Key.PropType), value) >= 512)
							{
								if (hashSet == null)
								{
									hashSet = new HashSet<ushort>();
								}
								hashSet.Add(num5);
								value = ValueReference.Zero;
								flag = false;
							}
							int serializedSize = PropertyBlob.GetSerializedSize(keyValuePair.Key.PropTag, value);
							if (serializedSize > num4 && flag)
							{
								num4 = serializedSize;
								item = num5;
							}
							PropertyBlob.ThrowIfBlobOverflow(num3, serializedSize + 8);
							num3 += 8 + serializedSize;
							if (num3 > 3110 && num4 != 0)
							{
								break;
							}
							list.Add(num5);
						}
					}
				}
				if ((num3 <= 3110 || num4 == 0) && additionalPromotedProperties != null)
				{
					foreach (ushort num6 in additionalPromotedProperties)
					{
						if (hashSet == null || !hashSet.Contains(num6))
						{
							KeyValuePair<StorePropTag, object> keyValuePair2;
							StorePropTag storePropTag;
							object value2;
							if (properties == null || !properties.TryGetValue(num6, out keyValuePair2))
							{
								storePropTag = StorePropTag.CreateWithoutInfo(num6, PropertyType.Null);
								value2 = null;
							}
							else
							{
								if (ValueTypeHelper.ValueSize(PropertyTypeHelper.GetExtendedTypeCode(keyValuePair2.Key.PropType), keyValuePair2.Value) >= 512)
								{
									if (hashSet == null)
									{
										hashSet = new HashSet<ushort>();
									}
									hashSet.Add(num6);
									continue;
								}
								storePropTag = keyValuePair2.Key;
								value2 = keyValuePair2.Value;
							}
							int serializedSize2 = PropertyBlob.GetSerializedSize(storePropTag.PropTag, value2);
							if (serializedSize2 + 8 > num4)
							{
								num4 = serializedSize2 + 8;
								item = num6;
							}
							PropertyBlob.ThrowIfBlobOverflow(num3, serializedSize2 + 8);
							num3 += 8 + serializedSize2;
							if (num3 > 3110 && num4 != 0)
							{
								break;
							}
							list.Add(num6);
						}
					}
				}
				if (num3 <= 3110 || num4 == 0)
				{
					break;
				}
				if (hashSet == null)
				{
					hashSet = new HashSet<ushort>();
				}
				hashSet.Add(item);
			}
			if (list.Count != 0)
			{
				onPageBlob = new byte[num3];
				int num7 = 0;
				int num8 = 8 + list.Count * 8;
				ParseSerialize.SerializeInt32(1349481040, onPageBlob, 0);
				ParseSerialize.SerializeInt16(768, onPageBlob, 4);
				ParseSerialize.SerializeInt16((short)list.Count, onPageBlob, 6);
				list.Sort();
				foreach (ushort num9 in list)
				{
					KeyValuePair<StorePropTag, object> keyValuePair3;
					if (properties != null && properties.TryGetValue(num9, out keyValuePair3))
					{
						if (hashSet != null && hashSet.Contains(num9))
						{
							PropertyBlob.AddProperty(onPageBlob, ref num7, ref num8, keyValuePair3.Key.PropTag, ValueReference.Zero);
						}
						else
						{
							PropertyBlob.AddProperty(onPageBlob, ref num7, ref num8, keyValuePair3.Key.PropTag, keyValuePair3.Value);
						}
					}
					else
					{
						StorePropTag storePropTag2 = StorePropTag.CreateWithoutInfo(num9, PropertyType.Null);
						PropertyBlob.AddProperty(onPageBlob, ref num7, ref num8, storePropTag2.PropTag, null);
					}
				}
			}
			if (properties != null)
			{
				num3 = 8;
				list.Clear();
				foreach (KeyValuePair<ushort, KeyValuePair<StorePropTag, object>> keyValuePair4 in properties)
				{
					ushort key = keyValuePair4.Key;
					if (((defaultPromotedProperties == null || !defaultPromotedProperties.Contains(key)) && (additionalPromotedProperties == null || !additionalPromotedProperties.Contains(key))) || (hashSet != null && hashSet.Contains(key)))
					{
						StorePropTag key2 = keyValuePair4.Value.Key;
						object value3 = keyValuePair4.Value.Value;
						int serializedSize3 = PropertyBlob.GetSerializedSize(key2.PropTag, value3);
						PropertyBlob.ThrowIfBlobOverflow(num3, serializedSize3 + 8);
						num3 += 8 + serializedSize3;
						list.Add(key);
					}
				}
				if (list.Count != 0)
				{
					if (num3 < 65536)
					{
						byte[] array = new byte[num3];
						int num10 = 0;
						int num11 = 8 + list.Count * 8;
						ParseSerialize.SerializeInt32(1349481040, array, 0);
						ParseSerialize.SerializeInt16(768, array, 4);
						ParseSerialize.SerializeInt16((short)list.Count, array, 6);
						list.Sort();
						foreach (ushort key3 in list)
						{
							KeyValuePair<StorePropTag, object> keyValuePair5 = properties[key3];
							PropertyBlob.AddProperty(array, ref num10, ref num11, keyValuePair5.Key.PropTag, keyValuePair5.Value);
						}
						offPageBlob = array;
						return;
					}
					Stream stream = null;
					try
					{
						stream = TempStream.CreateInstance();
						PropertyBlob.WriteBlobToStream(list, properties, stream);
						offPageBlob = stream;
						stream = null;
					}
					finally
					{
						if (stream != null)
						{
							stream.Dispose();
						}
					}
				}
			}
		}

		internal static void WriteBlobToStream(List<ushort> propertyIds, Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties, Stream blobStream)
		{
			propertyIds.Sort();
			PropertyBlob.DictionaryEntry[] array = new PropertyBlob.DictionaryEntry[propertyIds.Count];
			int num = 8 + propertyIds.Count * 8;
			for (int i = 0; i < propertyIds.Count; i++)
			{
				KeyValuePair<StorePropTag, object> keyValuePair = properties[propertyIds[i]];
				array[i] = PropertyBlob.BuildEntry(keyValuePair.Key.PropTag, keyValuePair.Value, num);
				num += PropertyBlob.GetSerializedSize(keyValuePair.Key.PropTag, keyValuePair.Value);
			}
			byte[] array2 = null;
			BufferPool bufferPool = DataRow.GetBufferPool(Factory.GetOptimalStreamChunkSize());
			try
			{
				array2 = bufferPool.Acquire();
				ParseSerialize.SerializeInt32(1349481040, array2, 0);
				ParseSerialize.SerializeInt16(768, array2, 4);
				ParseSerialize.SerializeInt16((short)propertyIds.Count, array2, 6);
				blobStream.Write(array2, 0, 8);
				for (int j = 0; j < array.Length; j++)
				{
					ParseSerialize.SerializeInt16((short)array[j].Id, array2, 2);
					array2[0] = (byte)array[j].CompressedType;
					array2[1] = (byte)array[j].Format;
					ParseSerialize.SerializeInt32(array[j].Offset, array2, 4);
					blobStream.Write(array2, 0, 8);
				}
				for (int k = 0; k < array.Length; k++)
				{
					if (!array[k].IsValueInline)
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail((long)array[k].Offset == blobStream.Position, "Stream position and entry offset mismatch");
						KeyValuePair<StorePropTag, object> keyValuePair2 = properties[propertyIds[k]];
						int serializedSize = PropertyBlob.GetSerializedSize(keyValuePair2.Key.PropTag, keyValuePair2.Value);
						byte[] buffer = array2;
						if (array2.Length < serializedSize)
						{
							byte[] array3 = keyValuePair2.Value as byte[];
							if (array3 != null)
							{
								PropertyBlob.WriteLargeBinaryValueToStream(array3, array2, blobStream);
								goto IL_203;
							}
							buffer = new byte[serializedSize];
						}
						int count = 0;
						SerializedValue.Serialize(array[k].Format, keyValuePair2.Value, buffer, ref count);
						blobStream.Write(buffer, 0, count);
					}
					IL_203:;
				}
			}
			finally
			{
				if (array2 != null)
				{
					bufferPool.Release(array2);
				}
			}
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail((long)num == blobStream.Position, "size mismatch");
		}

		private static void WriteLargeBinaryValueToStream(byte[] value, byte[] tempBuffer, Stream blobStream)
		{
			if (value.Length <= 65535)
			{
				SerializedValue.ValueFormat valueFormat = (SerializedValue.ValueFormat)82;
				tempBuffer[0] = (byte)valueFormat;
				ParseSerialize.SerializeInt16((short)value.Length, tempBuffer, 1);
				blobStream.Write(tempBuffer, 0, 3);
			}
			else
			{
				SerializedValue.ValueFormat valueFormat2 = (SerializedValue.ValueFormat)83;
				tempBuffer[0] = (byte)valueFormat2;
				ParseSerialize.SerializeInt32(value.Length, tempBuffer, 1);
				blobStream.Write(tempBuffer, 0, 5);
			}
			blobStream.Write(value, 0, value.Length);
		}

		private const short CurrentBlobFormatVersion = 768;

		private const int BlobMagicNumber = 1349481040;

		private const int BlobHeaderMagicOffset = 0;

		private const int BlobHeaderMagicLength = 4;

		private const int BlobHeaderVersionOffset = 4;

		private const int BlobHeaderVersionLength = 2;

		private const int BlobHeaderPropertyCountOffset = 6;

		private const int BlobHeaderPropertyCountLength = 2;

		private const int BlobHeaderLength = 8;

		private const int DictionaryEntryCompressedTypeOffset = 0;

		private const int DictionaryEntryCompressedTypeLength = 1;

		private const int DictionaryEntryFormatOffset = 1;

		private const int DictionaryEntryFormatLength = 1;

		private const int DictionaryEntryIdOffset = 2;

		private const int DictionaryEntryIdLength = 2;

		private const int DictionaryEntryOffsetOrValueOffset = 4;

		private const int DictionaryEntryOffsetOrValueLength = 4;

		private const int DictionaryEntryLength = 8;

		public const int MaxOnPageBlobSize = 3110;

		public const int MaxOnPageProperties = 388;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct BlobHeader
		{
			public uint Magic;

			public ushort Version;

			public ushort PropertyCount;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct DictionaryEntry
		{
			public DictionaryEntry(byte[] blob, int dictionaryEntryOffset)
			{
				this.compressedType = blob[dictionaryEntryOffset];
				this.format = blob[dictionaryEntryOffset + 1];
				this.id = (ushort)ParseSerialize.ParseInt16(blob, dictionaryEntryOffset + 2);
				this.offsetOrValue = ParseSerialize.ParseInt32(blob, dictionaryEntryOffset + 4);
			}

			public DictionaryEntry(ushort id, PropertyBlob.CompressedPropertyType compressedType, SerializedValue.ValueFormat format, int offsetOrValue)
			{
				this.id = id;
				this.compressedType = (byte)compressedType;
				this.format = (byte)format;
				this.offsetOrValue = offsetOrValue;
			}

			public PropertyBlob.CompressedPropertyType CompressedType
			{
				get
				{
					return (PropertyBlob.CompressedPropertyType)(this.compressedType & 252);
				}
			}

			public AdditionalPropertyInfo AdditionalInfo
			{
				get
				{
					return (AdditionalPropertyInfo)(this.compressedType & 3);
				}
			}

			public SerializedValue.ValueFormat Format
			{
				get
				{
					return (SerializedValue.ValueFormat)this.format;
				}
			}

			public int Offset
			{
				get
				{
					return this.offsetOrValue;
				}
			}

			public int Value
			{
				get
				{
					return this.offsetOrValue;
				}
			}

			public ushort Id
			{
				get
				{
					return this.id;
				}
			}

			public bool IsValueInline
			{
				get
				{
					SerializedValue.ValueFormat valueFormat = this.Format;
					if (valueFormat <= SerializedValue.ValueFormat.Boolean)
					{
						if (valueFormat != SerializedValue.ValueFormat.FormatModifierShift && valueFormat != SerializedValue.ValueFormat.Boolean)
						{
							return false;
						}
					}
					else if (valueFormat != SerializedValue.ValueFormat.Int16 && valueFormat != SerializedValue.ValueFormat.Int32 && valueFormat != (SerializedValue.ValueFormat)124)
					{
						return false;
					}
					return true;
				}
			}

			public uint GetPropertyTag()
			{
				return (uint)((int)this.id << 16 | (int)PropertyBlob.GetPropertyType(this.CompressedType));
			}

			public PropertyType GetPropertyType()
			{
				return PropertyBlob.GetPropertyType(this.CompressedType);
			}

			private byte compressedType;

			private byte format;

			private ushort id;

			private int offsetOrValue;
		}

		public struct BlobReader
		{
			public BlobReader(byte[] blob, int startOffset)
			{
				if (blob == null || blob.Length == 0)
				{
					this.blob = blob;
					this.startOffset = startOffset;
					this.version = 1;
					this.propertyCount = 0;
					return;
				}
				if (blob.Length - startOffset < 8 || ParseSerialize.ParseInt32(blob, startOffset) != 1349481040)
				{
					throw new InvalidBlobException("blob is too short or blob magic number is incorrect");
				}
				ushort num = (ushort)ParseSerialize.ParseInt16(blob, startOffset + 4);
				ushort num2 = (ushort)ParseSerialize.ParseInt16(blob, startOffset + 6);
				if ((num & 65280) != 768)
				{
					throw new InvalidBlobException("invalid blob format version");
				}
				if (blob.Length - startOffset < (int)(8 + num2 * 8))
				{
					throw new InvalidBlobException("invalid property count");
				}
				this.blob = blob;
				this.startOffset = startOffset;
				this.version = num;
				this.propertyCount = num2;
			}

			public int PropertyCount
			{
				get
				{
					return (int)this.propertyCount;
				}
			}

			public bool TryFindPropertyById(ushort id, out int index)
			{
				if (this.propertyCount == 0)
				{
					index = -1;
					return false;
				}
				return PropertyBlob.TryFindDictionaryEntry(this.blob, this.startOffset, (int)this.propertyCount, id, out index);
			}

			public uint GetPropertyTag(int index)
			{
				if (index < 0 || index >= (int)this.propertyCount)
				{
					throw new IndexOutOfRangeException();
				}
				return this.GetDictionaryEntry(index).GetPropertyTag();
			}

			public object GetPropertyValue(int index)
			{
				if (index < 0 || index >= (int)this.propertyCount)
				{
					throw new IndexOutOfRangeException();
				}
				PropertyBlob.DictionaryEntry dictionaryEntry = this.GetDictionaryEntry(index);
				return this.GetPropertyValue(dictionaryEntry);
			}

			public int GetPropertyValueSize(int index)
			{
				if (index < 0 || index >= (int)this.propertyCount)
				{
					throw new IndexOutOfRangeException();
				}
				PropertyBlob.DictionaryEntry dictionaryEntry = this.GetDictionaryEntry(index);
				return this.GetPropertyValueSize(dictionaryEntry);
			}

			public AdditionalPropertyInfo GetPropertyAdditionalInfo(int index)
			{
				if (index < 0 || index >= (int)this.propertyCount)
				{
					throw new IndexOutOfRangeException();
				}
				return this.GetDictionaryEntry(index).AdditionalInfo;
			}

			public bool IsPropertyValueNull(int index)
			{
				if (index < 0 || index >= (int)this.propertyCount)
				{
					throw new IndexOutOfRangeException();
				}
				return this.GetDictionaryEntry(index).Format == SerializedValue.ValueFormat.FormatModifierShift;
			}

			public bool IsPropertyValueReference(int index)
			{
				if (index < 0 || index >= (int)this.propertyCount)
				{
					throw new IndexOutOfRangeException();
				}
				PropertyBlob.DictionaryEntry dictionaryEntry = this.GetDictionaryEntry(index);
				return dictionaryEntry.Format == SerializedValue.ValueFormat.Reference || dictionaryEntry.Format == (SerializedValue.ValueFormat)124;
			}

			public KeyValuePair<uint, object> GetProperty(int index)
			{
				if (index < 0 || index >= (int)this.propertyCount)
				{
					throw new IndexOutOfRangeException();
				}
				PropertyBlob.DictionaryEntry dictionaryEntry = this.GetDictionaryEntry(index);
				return new KeyValuePair<uint, object>(dictionaryEntry.GetPropertyTag(), this.GetPropertyValue(dictionaryEntry));
			}

			public object GetPropertyValueByTag(uint tag)
			{
				int num;
				object result;
				if (this.TryGetPropertyValueByTag(tag, out num, out result))
				{
					return result;
				}
				return null;
			}

			public bool TryGetPropertyValueByTag(uint tag, out int index, out object value)
			{
				if (!this.TryFindPropertyById(PropertyBlob.IdFromTag(tag), out index))
				{
					value = null;
					return false;
				}
				PropertyBlob.DictionaryEntry dictionaryEntry = this.GetDictionaryEntry(index);
				PropertyType propertyType = PropertyBlob.PropertyTypeFromTag(tag);
				if (dictionaryEntry.GetPropertyType() == propertyType)
				{
					value = this.GetPropertyValue(dictionaryEntry);
					return true;
				}
				PropertyBlob.CompressedPropertyType compressedType = PropertyBlob.GetCompressedType(propertyType);
				if (PropertyBlob.CompatibleTypes(dictionaryEntry.CompressedType, compressedType))
				{
					value = this.GetPropertyValue(dictionaryEntry, compressedType);
					return true;
				}
				value = null;
				return true;
			}

			public bool TestIfPropertyPresent(ushort id)
			{
				int num;
				return this.TryFindPropertyById(id, out num);
			}

			public bool TryGetPropertyById(ushort id, out KeyValuePair<uint, object> property)
			{
				int index;
				if (this.TryFindPropertyById(id, out index))
				{
					PropertyBlob.DictionaryEntry dictionaryEntry = this.GetDictionaryEntry(index);
					property = new KeyValuePair<uint, object>(dictionaryEntry.GetPropertyTag(), this.GetPropertyValue(dictionaryEntry));
					return true;
				}
				property = default(KeyValuePair<uint, object>);
				return false;
			}

			private PropertyBlob.DictionaryEntry GetDictionaryEntry(int index)
			{
				return new PropertyBlob.DictionaryEntry(this.blob, this.startOffset + 8 + index * 8);
			}

			private object GetPropertyValue(PropertyBlob.DictionaryEntry dictionaryEntry)
			{
				if (dictionaryEntry.Format != SerializedValue.ValueFormat.FormatModifierShift && dictionaryEntry.Format != SerializedValue.ValueFormat.Reference && dictionaryEntry.Format != (SerializedValue.ValueFormat)124)
				{
					SerializedValue.ValueFormat valueFormat = PropertyBlob.GetValueFormat(dictionaryEntry.CompressedType);
					if (valueFormat != dictionaryEntry.Format && (dictionaryEntry.Format != SerializedValue.ValueFormat.Binary || (valueFormat != SerializedValue.ValueFormat.Reserved1 && valueFormat != SerializedValue.ValueFormat.Reserved2)))
					{
						throw new InvalidBlobException("invalid dictionary entry - format should match type");
					}
				}
				SerializedValue.ValueFormat format = dictionaryEntry.Format;
				if (format <= SerializedValue.ValueFormat.Binary)
				{
					if (format <= SerializedValue.ValueFormat.Int64)
					{
						if (format <= SerializedValue.ValueFormat.Boolean)
						{
							if (format == SerializedValue.ValueFormat.FormatModifierShift)
							{
								return null;
							}
							if (format != SerializedValue.ValueFormat.Boolean)
							{
								goto IL_1D5;
							}
							if (dictionaryEntry.Value == 0)
							{
								return SerializedValue.BoxedFalse;
							}
							return SerializedValue.BoxedTrue;
						}
						else
						{
							if (format == SerializedValue.ValueFormat.Int16)
							{
								return (short)dictionaryEntry.Value;
							}
							if (format == SerializedValue.ValueFormat.Int32)
							{
								return SerializedValue.GetBoxedInt32(dictionaryEntry.Value);
							}
							if (format != SerializedValue.ValueFormat.Int64)
							{
								goto IL_1D5;
							}
						}
					}
					else if (format <= SerializedValue.ValueFormat.DateTime)
					{
						if (format != SerializedValue.ValueFormat.Single && format != SerializedValue.ValueFormat.Double && format != SerializedValue.ValueFormat.DateTime)
						{
							goto IL_1D5;
						}
					}
					else if (format != SerializedValue.ValueFormat.Guid && format != SerializedValue.ValueFormat.String && format != SerializedValue.ValueFormat.Binary)
					{
						goto IL_1D5;
					}
				}
				else if (format <= SerializedValue.ValueFormat.MVInt64)
				{
					if (format <= (SerializedValue.ValueFormat)124)
					{
						if (format != SerializedValue.ValueFormat.Reference)
						{
							if (format != (SerializedValue.ValueFormat)124)
							{
								goto IL_1D5;
							}
							return ValueReference.Zero;
						}
					}
					else if (format != SerializedValue.ValueFormat.MVInt16 && format != SerializedValue.ValueFormat.MVInt32 && format != SerializedValue.ValueFormat.MVInt64)
					{
						goto IL_1D5;
					}
				}
				else if (format <= SerializedValue.ValueFormat.MVDateTime)
				{
					if (format != SerializedValue.ValueFormat.MVSingle && format != SerializedValue.ValueFormat.MVDouble && format != SerializedValue.ValueFormat.MVDateTime)
					{
						goto IL_1D5;
					}
				}
				else if (format != SerializedValue.ValueFormat.MVGuid && format != SerializedValue.ValueFormat.MVString && format != SerializedValue.ValueFormat.MVBinary)
				{
					goto IL_1D5;
				}
				object zero;
				if (!SerializedValue.TryParse(dictionaryEntry.Format, this.blob, this.startOffset + dictionaryEntry.Offset, out zero))
				{
					throw new InvalidBlobException("value parsing error");
				}
				if (dictionaryEntry.AdditionalInfo == AdditionalPropertyInfo.Truncated)
				{
					zero = ValueReference.Zero;
				}
				return zero;
				IL_1D5:
				throw new InvalidBlobException("invalid dictionary entry - format");
			}

			private int GetPropertyValueSize(PropertyBlob.DictionaryEntry dictionaryEntry)
			{
				if (dictionaryEntry.Format != SerializedValue.ValueFormat.FormatModifierShift && dictionaryEntry.Format != SerializedValue.ValueFormat.Reference && dictionaryEntry.Format != (SerializedValue.ValueFormat)124)
				{
					SerializedValue.ValueFormat valueFormat = PropertyBlob.GetValueFormat(dictionaryEntry.CompressedType);
					if (valueFormat != dictionaryEntry.Format && (dictionaryEntry.Format != SerializedValue.ValueFormat.Binary || (valueFormat != SerializedValue.ValueFormat.Reserved1 && valueFormat != SerializedValue.ValueFormat.Reserved2)))
					{
						throw new InvalidBlobException("invalid dictionary entry - format should match type");
					}
				}
				SerializedValue.ValueFormat format = dictionaryEntry.Format;
				if (format <= SerializedValue.ValueFormat.Binary)
				{
					if (format <= SerializedValue.ValueFormat.Int64)
					{
						if (format <= SerializedValue.ValueFormat.Boolean)
						{
							if (format == SerializedValue.ValueFormat.FormatModifierShift)
							{
								return 0;
							}
							if (format != SerializedValue.ValueFormat.Boolean)
							{
								goto IL_188;
							}
							return 1;
						}
						else
						{
							if (format == SerializedValue.ValueFormat.Int16)
							{
								return 2;
							}
							if (format == SerializedValue.ValueFormat.Int32)
							{
								return 4;
							}
							if (format != SerializedValue.ValueFormat.Int64)
							{
								goto IL_188;
							}
						}
					}
					else if (format <= SerializedValue.ValueFormat.DateTime)
					{
						if (format != SerializedValue.ValueFormat.Single && format != SerializedValue.ValueFormat.Double && format != SerializedValue.ValueFormat.DateTime)
						{
							goto IL_188;
						}
					}
					else if (format != SerializedValue.ValueFormat.Guid && format != SerializedValue.ValueFormat.String && format != SerializedValue.ValueFormat.Binary)
					{
						goto IL_188;
					}
				}
				else if (format <= SerializedValue.ValueFormat.MVInt64)
				{
					if (format <= (SerializedValue.ValueFormat)124)
					{
						if (format != SerializedValue.ValueFormat.Reference)
						{
							if (format != (SerializedValue.ValueFormat)124)
							{
								goto IL_188;
							}
							return 0;
						}
					}
					else if (format != SerializedValue.ValueFormat.MVInt16 && format != SerializedValue.ValueFormat.MVInt32 && format != SerializedValue.ValueFormat.MVInt64)
					{
						goto IL_188;
					}
				}
				else if (format <= SerializedValue.ValueFormat.MVDateTime)
				{
					if (format != SerializedValue.ValueFormat.MVSingle && format != SerializedValue.ValueFormat.MVDouble && format != SerializedValue.ValueFormat.MVDateTime)
					{
						goto IL_188;
					}
				}
				else if (format != SerializedValue.ValueFormat.MVGuid && format != SerializedValue.ValueFormat.MVString && format != SerializedValue.ValueFormat.MVBinary)
				{
					goto IL_188;
				}
				int result;
				if (!SerializedValue.TryGetSize(dictionaryEntry.Format, this.blob, this.startOffset + dictionaryEntry.Offset, out result))
				{
					throw new InvalidBlobException("value parsing error");
				}
				return result;
				IL_188:
				throw new InvalidBlobException("invalid dictionary entry - format");
			}

			private object GetPropertyValue(PropertyBlob.DictionaryEntry dictionaryEntry, PropertyBlob.CompressedPropertyType desiredCompressedType)
			{
				SerializedValue.ValueFormat format = dictionaryEntry.Format;
				if (format <= SerializedValue.ValueFormat.Int32)
				{
					if (format == SerializedValue.ValueFormat.FormatModifierShift)
					{
						return null;
					}
					if (format != SerializedValue.ValueFormat.Int16)
					{
						if (format == SerializedValue.ValueFormat.Int32)
						{
							int value = dictionaryEntry.Value;
							if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int32)
							{
								return value;
							}
							if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int64)
							{
								return (long)value;
							}
						}
					}
					else
					{
						short num = (short)dictionaryEntry.Value;
						if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int32)
						{
							return (int)num;
						}
						if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int64)
						{
							return (long)num;
						}
					}
				}
				else if (format <= SerializedValue.ValueFormat.Single)
				{
					if (format != SerializedValue.ValueFormat.Int64)
					{
						if (format == SerializedValue.ValueFormat.Single)
						{
							if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Real64)
							{
								object obj;
								if (!SerializedValue.TryParse(dictionaryEntry.Format, this.blob, this.startOffset + dictionaryEntry.Offset, out obj))
								{
									throw new InvalidBlobException("value parsing error");
								}
								return (double)((float)obj);
							}
						}
					}
					else
					{
						long num2 = (long)dictionaryEntry.Value;
						if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int64)
						{
							return num2;
						}
					}
				}
				else if (format != SerializedValue.ValueFormat.Binary)
				{
					if (format == (SerializedValue.ValueFormat)124)
					{
						return ValueReference.Zero;
					}
				}
				else if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Binary)
				{
					object result;
					if (!SerializedValue.TryParse(dictionaryEntry.Format, this.blob, this.startOffset + dictionaryEntry.Offset, out result))
					{
						throw new InvalidBlobException("value parsing error");
					}
					return result;
				}
				throw new InvalidBlobException("invalid property value format");
			}

			private ushort version;

			private ushort propertyCount;

			private int startOffset;

			private byte[] blob;
		}

		internal enum CompressedPropertyType : byte
		{
			CompressedPropertyTypeMask = 252,
			AdditionalInfoMask = 3,
			Null = 0,
			Boolean = 8,
			Int16 = 16,
			Int32 = 24,
			Int64 = 32,
			Currency = 36,
			Real32 = 40,
			Real64 = 48,
			AppTime = 52,
			SysTime = 56,
			Guid = 64,
			Unicode = 72,
			Binary = 80,
			Object = 84,
			SvrEid = 112,
			SRestriction = 104,
			Actions = 108,
			MVInt16 = 144,
			MVInt32 = 152,
			MVInt64 = 160,
			MVCurrency = 164,
			MVReal32 = 168,
			MVReal64 = 176,
			MVAppTime = 180,
			MVSysTime = 184,
			MVGuid = 192,
			MVUnicode = 200,
			MVBinary = 208
		}

		public struct BlobStreamReader
		{
			public BlobStreamReader(Stream blobStream)
			{
				this.blobStream = blobStream;
			}

			public IEnumerable<KeyValuePair<uint, object>> LoadProperties(bool loadValues, bool includeNullValues)
			{
				byte[] tempBuffer = null;
				BufferPool pool = DataRow.GetBufferPool(Factory.GetOptimalStreamChunkSize());
				try
				{
					tempBuffer = pool.Acquire();
					PropertyBlob.DictionaryEntry[] entries = this.LoadBlobDictionary(tempBuffer, loadValues);
					for (int i = 0; i < entries.Length; i++)
					{
						PropertyBlob.DictionaryEntry dictionaryEntry = entries[i];
						if (dictionaryEntry.Format != SerializedValue.ValueFormat.FormatModifierShift && dictionaryEntry.Format != SerializedValue.ValueFormat.Reference && dictionaryEntry.Format != (SerializedValue.ValueFormat)124)
						{
							SerializedValue.ValueFormat valueFormat = PropertyBlob.GetValueFormat(dictionaryEntry.CompressedType);
							if (valueFormat != dictionaryEntry.Format && (dictionaryEntry.Format != SerializedValue.ValueFormat.Binary || (valueFormat != SerializedValue.ValueFormat.Reserved1 && valueFormat != SerializedValue.ValueFormat.Reserved2)))
							{
								throw new StoreException((LID)36112U, ErrorCodeValue.CorruptData, "Invalid dictionary entry - format should match type.");
							}
						}
						if (loadValues)
						{
							if (dictionaryEntry.IsValueInline)
							{
								yield return new KeyValuePair<uint, object>(dictionaryEntry.GetPropertyTag(), this.GetInlinePropertyValue(dictionaryEntry));
							}
							else
							{
								int maxValueLength = checked((int)(this.blobStream.Length - this.blobStream.Position));
								if (i + 1 < entries.Length)
								{
									maxValueLength = entries[i + 1].Offset - dictionaryEntry.Offset;
								}
								yield return new KeyValuePair<uint, object>(dictionaryEntry.GetPropertyTag(), this.GetPropertyValueFromStream(dictionaryEntry, maxValueLength, tempBuffer));
							}
						}
						else if (includeNullValues || dictionaryEntry.Format != SerializedValue.ValueFormat.FormatModifierShift)
						{
							yield return new KeyValuePair<uint, object>(dictionaryEntry.GetPropertyTag(), null);
						}
					}
				}
				finally
				{
					if (tempBuffer != null)
					{
						pool.Release(tempBuffer);
					}
				}
				yield break;
			}

			public object GetPropertyValueByTag(uint tag)
			{
				byte[] array = null;
				BufferPool bufferPool = DataRow.GetBufferPool(Factory.GetOptimalStreamChunkSize());
				object result;
				try
				{
					array = bufferPool.Acquire();
					PropertyBlob.DictionaryEntry[] array2 = this.LoadBlobDictionary(array, true);
					int num = 0;
					while (num < array2.Length && array2[num].Id != PropertyBlob.IdFromTag(tag))
					{
						num++;
					}
					if (num == array2.Length)
					{
						result = null;
					}
					else
					{
						PropertyType propertyType = PropertyBlob.PropertyTypeFromTag(tag);
						PropertyBlob.CompressedPropertyType compressedType = PropertyBlob.GetCompressedType(propertyType);
						if (array2[num].GetPropertyType() == propertyType || PropertyBlob.CompatibleTypes(array2[num].CompressedType, compressedType))
						{
							object obj;
							if (array2[num].IsValueInline)
							{
								obj = this.GetInlinePropertyValue(array2[num]);
							}
							else
							{
								this.blobStream.Position = (long)array2[num].Offset;
								int maxValueLength = checked((int)(this.blobStream.Length - this.blobStream.Position));
								if (num + 1 < array2.Length)
								{
									maxValueLength = array2[num + 1].Offset - array2[num].Offset;
								}
								obj = this.GetPropertyValueFromStream(array2[num], maxValueLength, array);
							}
							if (array2[num].CompressedType != compressedType)
							{
								obj = this.ConvertPropertyValue(array2[num], compressedType, obj);
							}
							result = obj;
						}
						else
						{
							result = null;
						}
					}
				}
				finally
				{
					if (array != null)
					{
						bufferPool.Release(array);
					}
				}
				return result;
			}

			private PropertyBlob.DictionaryEntry[] LoadBlobDictionary(byte[] tempBuffer, bool sortEntries)
			{
				this.Read(tempBuffer, 8);
				if (ParseSerialize.ParseInt32(tempBuffer, 0) != 1349481040)
				{
					throw new StoreException((LID)52496U, ErrorCodeValue.CorruptData, "Blob magic number is invalid.");
				}
				ushort num = (ushort)ParseSerialize.ParseInt16(tempBuffer, 4);
				ushort num2 = (ushort)ParseSerialize.ParseInt16(tempBuffer, 6);
				if ((num & 65280) != 768)
				{
					throw new StoreException((LID)46352U, ErrorCodeValue.CorruptData, "Invalid blob format version.");
				}
				PropertyBlob.DictionaryEntry[] array = new PropertyBlob.DictionaryEntry[(int)num2];
				int num3 = (int)(8 + num2 * 8);
				for (int i = 0; i < (int)num2; i++)
				{
					this.Read(tempBuffer, 8);
					array[i] = new PropertyBlob.DictionaryEntry(tempBuffer, 0);
					if (!array[i].IsValueInline && (array[i].Offset < num3 || this.blobStream.Length < (long)array[i].Offset))
					{
						throw new StoreException((LID)62736U, ErrorCodeValue.CorruptData, "Invalid entry offset.");
					}
				}
				if (sortEntries)
				{
					Array.Sort<PropertyBlob.DictionaryEntry>(array, delegate(PropertyBlob.DictionaryEntry x, PropertyBlob.DictionaryEntry y)
					{
						if (x.IsValueInline && !y.IsValueInline)
						{
							return -1;
						}
						if (!x.IsValueInline && y.IsValueInline)
						{
							return 1;
						}
						return x.Offset.CompareTo(y.Offset);
					});
				}
				return array;
			}

			private void Read(byte[] buffer, int size)
			{
				if (this.blobStream.Length - this.blobStream.Position < (long)size)
				{
					throw new StoreException((LID)38160U, ErrorCodeValue.CorruptData, "Unexpected end of blob.");
				}
				int num = this.blobStream.Read(buffer, 0, size);
				if (num != size)
				{
					throw new StoreException((LID)54544U, ErrorCodeValue.CorruptData, "Unexpected number of bytes read from the blob stream.");
				}
			}

			private object GetInlinePropertyValue(PropertyBlob.DictionaryEntry dictionaryEntry)
			{
				SerializedValue.ValueFormat format = dictionaryEntry.Format;
				if (format <= SerializedValue.ValueFormat.Boolean)
				{
					if (format == SerializedValue.ValueFormat.FormatModifierShift)
					{
						return null;
					}
					if (format == SerializedValue.ValueFormat.Boolean)
					{
						if (dictionaryEntry.Value == 0)
						{
							return SerializedValue.BoxedFalse;
						}
						return SerializedValue.BoxedTrue;
					}
				}
				else
				{
					if (format == SerializedValue.ValueFormat.Int16)
					{
						return (short)dictionaryEntry.Value;
					}
					if (format == SerializedValue.ValueFormat.Int32)
					{
						return dictionaryEntry.Value;
					}
					if (format == (SerializedValue.ValueFormat)124)
					{
						return ValueReference.Zero;
					}
				}
				throw new StoreException((LID)42256U, ErrorCodeValue.CorruptData, "Invalid dictionary entry - format.");
			}

			private object GetPropertyValueFromStream(PropertyBlob.DictionaryEntry dictionaryEntry, int maxValueLength, byte[] tempBuffer)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail((long)dictionaryEntry.Offset == this.blobStream.Position, "Stream position and entry offset mismatch");
				byte[] buffer = tempBuffer;
				if (tempBuffer.Length < maxValueLength)
				{
					if (dictionaryEntry.Format == SerializedValue.ValueFormat.Binary)
					{
						return this.GetBinaryValueFromStream(dictionaryEntry, maxValueLength, tempBuffer);
					}
					buffer = new byte[maxValueLength];
				}
				this.Read(buffer, maxValueLength);
				object result;
				if (!SerializedValue.TryParse(dictionaryEntry.Format, buffer, 0, out result))
				{
					throw new StoreException((LID)58640U, ErrorCodeValue.CorruptData, "Value parsing error.");
				}
				return result;
			}

			private byte[] GetBinaryValueFromStream(PropertyBlob.DictionaryEntry dictionaryEntry, int maxValueLength, byte[] tempBuffer)
			{
				this.Read(tempBuffer, 1);
				maxValueLength--;
				SerializedValue.ValueFormat valueFormat = (SerializedValue.ValueFormat)tempBuffer[0];
				if ((byte)(dictionaryEntry.Format & SerializedValue.ValueFormat.TypeMask) != (byte)(valueFormat & SerializedValue.ValueFormat.TypeMask))
				{
					throw new StoreException((LID)34064U, ErrorCodeValue.CorruptData, "Value parsing error.");
				}
				int num2;
				int num = this.ParseLength(valueFormat, tempBuffer, out num2);
				maxValueLength -= num2;
				if (num > maxValueLength)
				{
					throw new StoreException((LID)50448U, ErrorCodeValue.CorruptData, "Value parsing error.");
				}
				byte[] array = new byte[num];
				this.Read(array, num);
				return array;
			}

			private int ParseLength(SerializedValue.ValueFormat format, byte[] tempBuffer, out int sizeOfLength)
			{
				switch ((byte)(format & SerializedValue.ValueFormat.TypeShift))
				{
				case 0:
					sizeOfLength = 0;
					return 0;
				case 1:
					sizeOfLength = 1;
					this.Read(tempBuffer, sizeOfLength);
					return (int)tempBuffer[0];
				case 2:
					sizeOfLength = 2;
					this.Read(tempBuffer, sizeOfLength);
					return (int)((ushort)ParseSerialize.ParseInt16(tempBuffer, 0));
				case 3:
					sizeOfLength = 4;
					this.Read(tempBuffer, sizeOfLength);
					return ParseSerialize.ParseInt32(tempBuffer, 0);
				default:
					throw new StoreException((LID)47376U, ErrorCodeValue.CorruptData, "Value parsing error.");
				}
			}

			private object ConvertPropertyValue(PropertyBlob.DictionaryEntry dictionaryEntry, PropertyBlob.CompressedPropertyType desiredCompressedType, object value)
			{
				SerializedValue.ValueFormat format = dictionaryEntry.Format;
				if (format <= SerializedValue.ValueFormat.Int32)
				{
					if (format == SerializedValue.ValueFormat.FormatModifierShift)
					{
						return null;
					}
					if (format != SerializedValue.ValueFormat.Int16)
					{
						if (format == SerializedValue.ValueFormat.Int32)
						{
							int num = (int)value;
							if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int32)
							{
								return num;
							}
							if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int64)
							{
								return (long)num;
							}
						}
					}
					else
					{
						short num2 = (short)value;
						if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int32)
						{
							return (int)num2;
						}
						if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int64)
						{
							return (long)num2;
						}
					}
				}
				else if (format <= SerializedValue.ValueFormat.Single)
				{
					if (format != SerializedValue.ValueFormat.Int64)
					{
						if (format == SerializedValue.ValueFormat.Single)
						{
							if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Real64)
							{
								return (double)((float)value);
							}
						}
					}
					else
					{
						long num3 = (long)value;
						if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Int64)
						{
							return num3;
						}
					}
				}
				else if (format != SerializedValue.ValueFormat.Binary)
				{
					if (format == (SerializedValue.ValueFormat)124)
					{
						return ValueReference.Zero;
					}
				}
				else if (desiredCompressedType == PropertyBlob.CompressedPropertyType.Binary)
				{
					return value;
				}
				throw new StoreException((LID)63760U, ErrorCodeValue.CorruptData, "Invalid property value format.");
			}

			private Stream blobStream;
		}
	}
}
