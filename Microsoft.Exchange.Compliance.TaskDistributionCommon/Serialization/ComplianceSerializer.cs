using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization
{
	public static class ComplianceSerializer
	{
		public static T DeSerialize<T>(ComplianceSerializationDescription<T> description, byte[] blob) where T : class, new()
		{
			T result = default(T);
			FaultDefinition faultDefinition = null;
			if (!ComplianceSerializer.TryDeserialize<T>(description, blob, out result, out faultDefinition, "DeSerialize", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Serialization\\ComplianceSerializer.cs", 77))
			{
				throw new BadStructureFormatException();
			}
			return result;
		}

		public static byte[] Serialize<T>(ComplianceSerializationDescription<T> description, T inputObject) where T : class, new()
		{
			if (inputObject == null)
			{
				return null;
			}
			int num = 1;
			num = num + 1 + description.TotalByteFields;
			num = num + 1 + 2 * description.TotalShortFields;
			num = num + 1 + 4 * description.TotalIntegerFields;
			num = num + 1 + 8 * description.TotalLongFields;
			num = num + 1 + 8 * description.TotalDoubleFields;
			num = num + 1 + 16 * description.TotalGuidFields;
			num = num + 1 + 4 * description.TotalStringFields;
			List<Tuple<int, byte[]>> variableWidthMemberBytes = ComplianceSerializer.GetVariableWidthMemberBytes<T>(description, inputObject, ComplianceSerializer.VariableWidthType.String);
			foreach (Tuple<int, byte[]> tuple in variableWidthMemberBytes)
			{
				num += tuple.Item1;
			}
			num = num + 1 + 4 * description.TotalBlobFields;
			List<Tuple<int, byte[]>> variableWidthMemberBytes2 = ComplianceSerializer.GetVariableWidthMemberBytes<T>(description, inputObject, ComplianceSerializer.VariableWidthType.Blob);
			foreach (Tuple<int, byte[]> tuple2 in variableWidthMemberBytes2)
			{
				num += tuple2.Item1;
			}
			num++;
			List<ComplianceSerializer.CollectionField> list = new List<ComplianceSerializer.CollectionField>();
			byte b = 0;
			while ((int)b < description.TotalCollectionFields)
			{
				CollectionItemType itemType = CollectionItemType.NotDefined;
				if (description.TryGetCollectionPropertyItemType(b, out itemType))
				{
					IEnumerable<object> collectionItems = description.GetCollectionItems(inputObject, b);
					ComplianceSerializer.CollectionField collectionField = ComplianceSerializer.CollectionField.GetCollectionField(itemType, collectionItems);
					list.Add(collectionField);
					num += collectionField.GetSizeOfSerializedCollectionField();
				}
				b += 1;
			}
			byte[] array = new byte[num];
			array[0] = description.ComplianceStructureId;
			int num2 = ComplianceSerializer.WriteFixedWidthFieldsToBlob<T>(ref description, ref inputObject, array, 1, 1, ComplianceSerializer.FixedWidthType.Byte, description.TotalByteFields);
			num2 = ComplianceSerializer.WriteFixedWidthFieldsToBlob<T>(ref description, ref inputObject, array, num2, 2, ComplianceSerializer.FixedWidthType.Short, description.TotalShortFields);
			num2 = ComplianceSerializer.WriteFixedWidthFieldsToBlob<T>(ref description, ref inputObject, array, num2, 4, ComplianceSerializer.FixedWidthType.Int, description.TotalIntegerFields);
			num2 = ComplianceSerializer.WriteFixedWidthFieldsToBlob<T>(ref description, ref inputObject, array, num2, 8, ComplianceSerializer.FixedWidthType.Long, description.TotalLongFields);
			num2 = ComplianceSerializer.WriteFixedWidthFieldsToBlob<T>(ref description, ref inputObject, array, num2, 8, ComplianceSerializer.FixedWidthType.Double, description.TotalDoubleFields);
			num2 = ComplianceSerializer.WriteFixedWidthFieldsToBlob<T>(ref description, ref inputObject, array, num2, 16, ComplianceSerializer.FixedWidthType.Guid, description.TotalGuidFields);
			num2 = ComplianceSerializer.WriteVariableWidthFieldsToBlob(array, num2, variableWidthMemberBytes);
			num2 = ComplianceSerializer.WriteVariableWidthFieldsToBlob(array, num2, variableWidthMemberBytes2);
			num2 = ComplianceSerializer.WriteCollectionsToBlob(array, num2, list);
			return array;
		}

		public static bool TryDeserialize<T>(ComplianceSerializationDescription<T> description, byte[] blob, out T parsedObject, out FaultDefinition faultDefinition, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0) where T : class, new()
		{
			parsedObject = Activator.CreateInstance<T>();
			int totalLength = blob.Length;
			if (description.ComplianceStructureId != blob[0])
			{
				faultDefinition = FaultDefinition.FromErrorString("Parsing wrong structure", callerMember, callerFilePath, callerLineNumber);
				return false;
			}
			int startIndex = 1;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			flag = ComplianceSerializer.TryWriteFixedWidthFieldsToObject<T>(ref description, ref parsedObject, blob, startIndex, 1, ComplianceSerializer.FixedWidthType.Byte, totalLength, flag, out startIndex, ref stringBuilder);
			flag = ComplianceSerializer.TryWriteFixedWidthFieldsToObject<T>(ref description, ref parsedObject, blob, startIndex, 2, ComplianceSerializer.FixedWidthType.Short, totalLength, flag, out startIndex, ref stringBuilder);
			flag = ComplianceSerializer.TryWriteFixedWidthFieldsToObject<T>(ref description, ref parsedObject, blob, startIndex, 4, ComplianceSerializer.FixedWidthType.Int, totalLength, flag, out startIndex, ref stringBuilder);
			flag = ComplianceSerializer.TryWriteFixedWidthFieldsToObject<T>(ref description, ref parsedObject, blob, startIndex, 8, ComplianceSerializer.FixedWidthType.Long, totalLength, flag, out startIndex, ref stringBuilder);
			flag = ComplianceSerializer.TryWriteFixedWidthFieldsToObject<T>(ref description, ref parsedObject, blob, startIndex, 8, ComplianceSerializer.FixedWidthType.Double, totalLength, flag, out startIndex, ref stringBuilder);
			flag = ComplianceSerializer.TryWriteFixedWidthFieldsToObject<T>(ref description, ref parsedObject, blob, startIndex, 16, ComplianceSerializer.FixedWidthType.Guid, totalLength, flag, out startIndex, ref stringBuilder);
			flag = ComplianceSerializer.TryWriteVariableWidthMembersToObject<T>(ref description, ref parsedObject, blob, startIndex, totalLength, ComplianceSerializer.VariableWidthType.String, flag, out startIndex, ref stringBuilder);
			flag = ComplianceSerializer.TryWriteVariableWidthMembersToObject<T>(ref description, ref parsedObject, blob, startIndex, totalLength, ComplianceSerializer.VariableWidthType.Blob, flag, out startIndex, ref stringBuilder);
			if (flag)
			{
				flag = ComplianceSerializer.TryWriteCollectionsToObject<T>(ref description, ref parsedObject, blob, startIndex, totalLength, out startIndex, ref stringBuilder);
			}
			if (flag)
			{
				faultDefinition = null;
			}
			else
			{
				faultDefinition = FaultDefinition.FromErrorString(stringBuilder.ToString(), callerMember, callerFilePath, callerLineNumber);
			}
			return flag;
		}

		private static void WriteShortToBlob(byte[] blob, int index, short shortValue)
		{
			blob[index] = (byte)(shortValue >> 8);
			blob[index + 1] = (byte)shortValue;
		}

		private static void WriteIntToBlob(byte[] blob, int index, int intValue)
		{
			blob[index] = (byte)(intValue >> 24);
			blob[index + 1] = (byte)(intValue >> 16);
			blob[index + 2] = (byte)(intValue >> 8);
			blob[index + 3] = (byte)intValue;
		}

		private static void WriteLongToBlob(byte[] blob, int index, long longValue)
		{
			blob[index] = (byte)(longValue >> 56);
			blob[index + 1] = (byte)(longValue >> 48);
			blob[index + 2] = (byte)(longValue >> 40);
			blob[index + 3] = (byte)(longValue >> 32);
			blob[index + 4] = (byte)(longValue >> 24);
			blob[index + 5] = (byte)(longValue >> 16);
			blob[index + 6] = (byte)(longValue >> 8);
			blob[index + 7] = (byte)longValue;
		}

		private unsafe static void WriteDoubleToBlob(byte[] blob, int index, double doubleValue)
		{
			ulong num = (ulong)(*(long*)(&doubleValue));
			blob[index] = (byte)(num >> 56);
			blob[index + 1] = (byte)(num >> 48);
			blob[index + 2] = (byte)(num >> 40);
			blob[index + 3] = (byte)(num >> 32);
			blob[index + 4] = (byte)(num >> 24);
			blob[index + 5] = (byte)(num >> 16);
			blob[index + 6] = (byte)(num >> 8);
			blob[index + 7] = (byte)num;
		}

		private static void WriteGuidToBlob(byte[] blob, int index, Guid guidValue)
		{
			byte[] array = guidValue.ToByteArray();
			Array.Copy(array, 0, blob, index, array.Length);
		}

		private static int WriteFixedWidthFieldsToBlob<T>(ref ComplianceSerializationDescription<T> description, ref T inputObject, byte[] blob, int startIndex, int width, ComplianceSerializer.FixedWidthType widthType, int totalFields) where T : class, new()
		{
			byte b = Convert.ToByte(totalFields);
			blob[startIndex] = b;
			int num = startIndex + 1;
			for (byte b2 = 0; b2 < b; b2 += 1)
			{
				switch (widthType)
				{
				case ComplianceSerializer.FixedWidthType.Byte:
				{
					byte b3 = 0;
					description.TryGetByteProperty(inputObject, b2, out b3);
					blob[num] = b3;
					break;
				}
				case ComplianceSerializer.FixedWidthType.Short:
				{
					short shortValue = 0;
					description.TryGetShortProperty(inputObject, b2, out shortValue);
					ComplianceSerializer.WriteShortToBlob(blob, num, shortValue);
					break;
				}
				case ComplianceSerializer.FixedWidthType.Int:
				{
					int intValue = 0;
					description.TryGetIntegerProperty(inputObject, b2, out intValue);
					ComplianceSerializer.WriteIntToBlob(blob, num, intValue);
					break;
				}
				case ComplianceSerializer.FixedWidthType.Long:
				{
					long longValue = 0L;
					description.TryGetLongProperty(inputObject, b2, out longValue);
					ComplianceSerializer.WriteLongToBlob(blob, num, longValue);
					break;
				}
				case ComplianceSerializer.FixedWidthType.Double:
				{
					double doubleValue = 0.0;
					description.TryGetDoubleProperty(inputObject, b2, out doubleValue);
					ComplianceSerializer.WriteDoubleToBlob(blob, num, doubleValue);
					break;
				}
				case ComplianceSerializer.FixedWidthType.Guid:
				{
					Guid empty = Guid.Empty;
					description.TryGetGuidProperty(inputObject, b2, out empty);
					ComplianceSerializer.WriteGuidToBlob(blob, num, empty);
					break;
				}
				default:
					throw new ArgumentException("widthType");
				}
				num += width;
			}
			return num;
		}

		private static int WriteVariableWidthFieldsToBlob(byte[] blob, int index, List<Tuple<int, byte[]>> fieldMembers)
		{
			blob[index++] = (byte)fieldMembers.Count;
			foreach (Tuple<int, byte[]> tuple in fieldMembers)
			{
				blob[index++] = (byte)(tuple.Item1 >> 24);
				blob[index++] = (byte)(tuple.Item1 >> 16);
				blob[index++] = (byte)(tuple.Item1 >> 8);
				blob[index++] = (byte)tuple.Item1;
			}
			foreach (Tuple<int, byte[]> tuple2 in fieldMembers)
			{
				if (tuple2.Item1 > 0)
				{
					byte[] item = tuple2.Item2;
					int num = item.Length;
					Array.Copy(item, 0, blob, index, num);
					index += num;
				}
			}
			return index;
		}

		private static int WriteCollectionsToBlob(byte[] blob, int index, List<ComplianceSerializer.CollectionField> collections)
		{
			blob[index++] = (byte)collections.Count;
			foreach (ComplianceSerializer.CollectionField collectionField in collections)
			{
				index = collectionField.WriteFieldToBlob(blob, index);
			}
			return index;
		}

		private static List<Tuple<int, byte[]>> GetVariableWidthMemberBytes<T>(ComplianceSerializationDescription<T> description, T inputObject, ComplianceSerializer.VariableWidthType widthType) where T : class, new()
		{
			List<Tuple<int, byte[]>> list = new List<Tuple<int, byte[]>>();
			byte b = (byte)description.TotalStringFields;
			if (widthType == ComplianceSerializer.VariableWidthType.Blob)
			{
				b = (byte)description.TotalBlobFields;
			}
			for (byte b2 = 0; b2 < b; b2 += 1)
			{
				int item = 0;
				byte[] array = null;
				string text;
				if (widthType == ComplianceSerializer.VariableWidthType.Blob)
				{
					if (inputObject != null && description.TryGetBlobProperty(inputObject, b2, out array) && array != null)
					{
						item = array.Length;
					}
				}
				else if (inputObject != null && description.TryGetStringProperty(inputObject, b2, out text) && text != null)
				{
					array = Encoding.UTF8.GetBytes(text);
					item = array.Length;
				}
				list.Add(new Tuple<int, byte[]>(item, array));
			}
			return list;
		}

		private static short ReadShortFromBlob(byte[] blob, int index)
		{
			return (short)((int)blob[index] << 8 | (int)blob[index + 1]);
		}

		private static int ReadIntFromBlob(byte[] blob, int index)
		{
			return (int)blob[index] << 24 | (int)blob[index + 1] << 16 | (int)blob[index + 2] << 8 | (int)blob[index + 3];
		}

		private static long ReadLongFromBlob(byte[] blob, int index)
		{
			int num = (int)blob[index] << 24 | (int)blob[index + 1] << 16 | (int)blob[index + 2] << 8 | (int)blob[index + 3];
			int num2 = (int)blob[index + 4] << 24 | (int)blob[index + 5] << 16 | (int)blob[index + 6] << 8 | (int)blob[index + 7];
			return (long)((ulong)num2 | (ulong)((ulong)((long)num) << 32));
		}

		private unsafe static double ReadDoubleFromBlob(byte[] blob, int index)
		{
			uint num = (uint)((int)blob[index] << 24 | (int)blob[index + 1] << 16 | (int)blob[index + 2] << 8 | (int)blob[index + 3]);
			uint num2 = (uint)((int)blob[index + 4] << 24 | (int)blob[index + 5] << 16 | (int)blob[index + 6] << 8 | (int)blob[index + 7]);
			ulong num3 = (ulong)num2 | (ulong)num << 32;
			return *(double*)(&num3);
		}

		private static Guid ReadGuidFromBlob(byte[] blob, int index)
		{
			byte[] array = new byte[16];
			Array.Copy(blob, index, array, 0, 16);
			return new Guid(array);
		}

		private static bool TryWriteFixedWidthFieldsToObject<T>(ref ComplianceSerializationDescription<T> description, ref T parsedObject, byte[] blob, int startIndex, int width, ComplianceSerializer.FixedWidthType widthType, int totalLength, bool continueDeserialization, out int index, ref StringBuilder errorBuilder) where T : class, new()
		{
			if (!continueDeserialization)
			{
				index = startIndex;
				return continueDeserialization;
			}
			index = startIndex;
			if (startIndex >= totalLength)
			{
				errorBuilder.AppendFormat("StartIndex:{0} is bigger than blob length:{1}", startIndex, totalLength);
				return false;
			}
			byte b = blob[startIndex];
			index++;
			for (byte b2 = 0; b2 < b; b2 += 1)
			{
				if (index + width > totalLength)
				{
					errorBuilder.AppendFormat("Blob length:{0} is not sufficient to read the field from index:{1}.", totalLength, index);
					return false;
				}
				switch (widthType)
				{
				case ComplianceSerializer.FixedWidthType.Byte:
					description.TrySetByteProperty(parsedObject, b2, blob[index]);
					break;
				case ComplianceSerializer.FixedWidthType.Short:
				{
					short value = ComplianceSerializer.ReadShortFromBlob(blob, index);
					description.TrySetShortProperty(parsedObject, b2, value);
					break;
				}
				case ComplianceSerializer.FixedWidthType.Int:
				{
					int value2 = ComplianceSerializer.ReadIntFromBlob(blob, index);
					description.TrySetIntegerProperty(parsedObject, b2, value2);
					break;
				}
				case ComplianceSerializer.FixedWidthType.Long:
				{
					long value3 = ComplianceSerializer.ReadLongFromBlob(blob, index);
					description.TrySetLongProperty(parsedObject, b2, value3);
					break;
				}
				case ComplianceSerializer.FixedWidthType.Double:
				{
					double value4 = ComplianceSerializer.ReadDoubleFromBlob(blob, index);
					description.TrySetDoubleProperty(parsedObject, b2, value4);
					break;
				}
				case ComplianceSerializer.FixedWidthType.Guid:
				{
					Guid value5 = ComplianceSerializer.ReadGuidFromBlob(blob, index);
					description.TrySetGuidProperty(parsedObject, b2, value5);
					break;
				}
				default:
					throw new ArgumentException("widthType");
				}
				index += width;
			}
			return true;
		}

		private static bool TryWriteVariableWidthMembersToObject<T>(ref ComplianceSerializationDescription<T> description, ref T parsedObject, byte[] blob, int startIndex, int totalLength, ComplianceSerializer.VariableWidthType widthType, bool continueDeserialization, out int index, ref StringBuilder errorBuilder) where T : class, new()
		{
			if (!continueDeserialization)
			{
				index = startIndex;
				return continueDeserialization;
			}
			index = startIndex;
			if (startIndex >= totalLength)
			{
				errorBuilder.AppendFormat("StartIndex:{0} is bigger than blob length:{1}", startIndex, totalLength);
				return false;
			}
			byte b = blob[index++];
			if (b > 0)
			{
				List<int> list = new List<int>();
				for (byte b2 = 0; b2 < b; b2 += 1)
				{
					if (index + 4 > totalLength)
					{
						errorBuilder.AppendFormat("Blob length:{0} is not sufficient to read the field-width from index:{1}.", totalLength, index);
						return false;
					}
					int item = ComplianceSerializer.ReadIntFromBlob(blob, index);
					list.Add(item);
					index += 4;
				}
				byte b3 = 0;
				foreach (int num in list)
				{
					if (num > 0)
					{
						if (index + num > totalLength)
						{
							errorBuilder.AppendFormat("Blob length:{0} is not sufficient to read the field of size:{1} from index:{2}.", totalLength, num, index);
							return false;
						}
						if (widthType == ComplianceSerializer.VariableWidthType.String)
						{
							string @string = Encoding.UTF8.GetString(blob, index, num);
							description.TrySetStringProperty(parsedObject, b3, @string);
							index += num;
						}
						else
						{
							byte[] array = new byte[num];
							Array.Copy(blob, index, array, 0, num);
							description.TrySetBlobProperty(parsedObject, b3, array);
							index += num;
						}
					}
					b3 += 1;
				}
				return true;
			}
			return true;
		}

		private static bool TryWriteCollectionsToObject<T>(ref ComplianceSerializationDescription<T> description, ref T parsedObject, byte[] blob, int startIndex, int totalLength, out int index, ref StringBuilder errorBuilder) where T : class, new()
		{
			index = startIndex;
			if (startIndex >= totalLength)
			{
				errorBuilder.AppendFormat("StartIndex:{0} is bigger than blob length:{1}", startIndex, totalLength);
				return false;
			}
			byte b = blob[index++];
			for (byte b2 = 0; b2 < b; b2 += 1)
			{
				Type typeFromHandle = typeof(CollectionItemType);
				CollectionItemType collectionItemType = (CollectionItemType)Enum.ToObject(typeFromHandle, blob[index++]);
				if (!Enum.IsDefined(typeFromHandle, collectionItemType))
				{
					errorBuilder.AppendFormat("Byte value:{0} at index:{1} does not represent a valid CollectionItemType", collectionItemType, index - 1);
					return false;
				}
				if (index + 4 > totalLength)
				{
					errorBuilder.AppendFormat("Blob length:{0} is not sufficient to read the field count:{1} at index:{2}.", totalLength, b, index);
					return false;
				}
				int num = ComplianceSerializer.ReadIntFromBlob(blob, index);
				index += 4;
				List<object> list = new List<object>();
				for (int i = 0; i < num; i++)
				{
					ComplianceSerializer.CollectionItem collectionItem = null;
					if (!ComplianceSerializer.CollectionItem.TryGetCollectionItemFromBlob(collectionItemType, blob, index, totalLength, out collectionItem, ref errorBuilder))
					{
						return false;
					}
					list.Add(ComplianceSerializer.CollectionItem.GetObject(collectionItemType, collectionItem));
					index += collectionItem.GetSerializedSize();
				}
				description.TrySetCollectionItems(parsedObject, b2, list);
			}
			return true;
		}

		private const int SizeOfGuid = 16;

		private enum FixedWidthType
		{
			Byte,
			Short,
			Int,
			Long,
			Double,
			Guid
		}

		private enum VariableWidthType
		{
			String,
			Blob
		}

		private class CollectionField
		{
			public CollectionItemType CollectionItemType { get; set; }

			public int NumberItems { get; set; }

			public ICollection<ComplianceSerializer.CollectionItem> CollectionItems
			{
				get
				{
					return this.collectionItems;
				}
			}

			public static ComplianceSerializer.CollectionField GetCollectionField(CollectionItemType itemType, IEnumerable<object> objects)
			{
				ComplianceSerializer.CollectionField collectionField = new ComplianceSerializer.CollectionField();
				collectionField.CollectionItemType = itemType;
				int num = 0;
				foreach (object obj in objects)
				{
					ComplianceSerializer.CollectionItem collectionItemFromObject = ComplianceSerializer.CollectionItem.GetCollectionItemFromObject(itemType, obj);
					num++;
					collectionField.CollectionItems.Add(collectionItemFromObject);
				}
				collectionField.NumberItems = num;
				return collectionField;
			}

			public int GetSizeOfSerializedCollectionField()
			{
				int num = 0;
				foreach (ComplianceSerializer.CollectionItem collectionItem in this.CollectionItems)
				{
					num += collectionItem.GetSerializedSize();
				}
				return 5 + num;
			}

			public int WriteFieldToBlob(byte[] blob, int index)
			{
				blob[index++] = (byte)this.CollectionItemType;
				ComplianceSerializer.WriteIntToBlob(blob, index, this.NumberItems);
				index += 4;
				foreach (ComplianceSerializer.CollectionItem collectionItem in this.CollectionItems)
				{
					index = collectionItem.WriteItemToBlob(blob, index);
				}
				return index;
			}

			private List<ComplianceSerializer.CollectionItem> collectionItems = new List<ComplianceSerializer.CollectionItem>();
		}

		private class CollectionItem
		{
			public bool IsFixedWidth { get; private set; }

			public byte[] ItemBlob { get; private set; }

			public static bool TryGetCollectionItemFromBlob(CollectionItemType itemType, byte[] blob, int index, int totalLength, out ComplianceSerializer.CollectionItem item, ref StringBuilder errorBuilder)
			{
				item = new ComplianceSerializer.CollectionItem();
				int num = 0;
				int num2 = 0;
				switch (itemType)
				{
				case CollectionItemType.Short:
					item.IsFixedWidth = true;
					num = 2;
					break;
				case CollectionItemType.Int:
					item.IsFixedWidth = true;
					num = 4;
					break;
				case CollectionItemType.Long:
					item.IsFixedWidth = true;
					num = 8;
					break;
				case CollectionItemType.Double:
					item.IsFixedWidth = true;
					num = 8;
					break;
				case CollectionItemType.Guid:
					item.IsFixedWidth = true;
					num = 16;
					break;
				case CollectionItemType.String:
				case CollectionItemType.Blob:
					item.IsFixedWidth = false;
					num2 = 4;
					if (index + num2 > totalLength)
					{
						errorBuilder.AppendFormat("Blob length:{0} is not sufficient to read the collection item width at index:{1}", totalLength, index);
						return false;
					}
					num = ComplianceSerializer.ReadIntFromBlob(blob, index);
					break;
				}
				item.ItemBlob = new byte[num];
				if (index + num2 + num > totalLength)
				{
					errorBuilder.AppendFormat("Blob length:{0} is not sufficient to read the collection item at index:{1}", totalLength, index + num2);
					return false;
				}
				Array.Copy(blob, index + num2, item.ItemBlob, 0, num);
				return true;
			}

			public static ComplianceSerializer.CollectionItem GetCollectionItemFromObject(CollectionItemType itemType, object obj)
			{
				ComplianceSerializer.CollectionItem collectionItem = new ComplianceSerializer.CollectionItem();
				switch (itemType)
				{
				case CollectionItemType.Short:
					collectionItem.IsFixedWidth = true;
					collectionItem.ItemBlob = new byte[2];
					ComplianceSerializer.WriteShortToBlob(collectionItem.ItemBlob, 0, (short)obj);
					break;
				case CollectionItemType.Int:
					collectionItem.IsFixedWidth = true;
					collectionItem.ItemBlob = new byte[4];
					ComplianceSerializer.WriteIntToBlob(collectionItem.ItemBlob, 0, (int)obj);
					break;
				case CollectionItemType.Long:
					collectionItem.IsFixedWidth = true;
					collectionItem.ItemBlob = new byte[8];
					ComplianceSerializer.WriteLongToBlob(collectionItem.ItemBlob, 0, (long)obj);
					break;
				case CollectionItemType.Double:
					collectionItem.IsFixedWidth = true;
					collectionItem.ItemBlob = new byte[8];
					ComplianceSerializer.WriteDoubleToBlob(collectionItem.ItemBlob, 0, (double)obj);
					break;
				case CollectionItemType.Guid:
					collectionItem.IsFixedWidth = true;
					collectionItem.ItemBlob = new byte[16];
					ComplianceSerializer.WriteGuidToBlob(collectionItem.ItemBlob, 0, (Guid)obj);
					break;
				case CollectionItemType.String:
					collectionItem.IsFixedWidth = false;
					collectionItem.ItemBlob = Encoding.UTF8.GetBytes((string)obj);
					break;
				case CollectionItemType.Blob:
					collectionItem.IsFixedWidth = false;
					collectionItem.ItemBlob = (byte[])obj;
					break;
				}
				return collectionItem;
			}

			public static object GetObject(CollectionItemType itemType, ComplianceSerializer.CollectionItem item)
			{
				switch (itemType)
				{
				case CollectionItemType.Short:
					return ComplianceSerializer.ReadShortFromBlob(item.ItemBlob, 0);
				case CollectionItemType.Int:
					return ComplianceSerializer.ReadIntFromBlob(item.ItemBlob, 0);
				case CollectionItemType.Long:
					return ComplianceSerializer.ReadLongFromBlob(item.ItemBlob, 0);
				case CollectionItemType.Double:
					return ComplianceSerializer.ReadDoubleFromBlob(item.ItemBlob, 0);
				case CollectionItemType.Guid:
					return ComplianceSerializer.ReadGuidFromBlob(item.ItemBlob, 0);
				case CollectionItemType.String:
					return Encoding.UTF8.GetString(item.ItemBlob);
				case CollectionItemType.Blob:
					return item.ItemBlob;
				default:
					return null;
				}
			}

			public int GetSerializedSize()
			{
				if (this.IsFixedWidth)
				{
					return this.ItemBlob.Length;
				}
				return this.ItemBlob.Length + 4;
			}

			public int WriteItemToBlob(byte[] blob, int index)
			{
				if (this.IsFixedWidth)
				{
					Array.Copy(this.ItemBlob, 0, blob, index, this.ItemBlob.Length);
				}
				else
				{
					ComplianceSerializer.WriteIntToBlob(blob, index, this.ItemBlob.Length);
					Array.Copy(this.ItemBlob, 0, blob, index + 4, this.ItemBlob.Length);
				}
				return index + this.GetSerializedSize();
			}
		}
	}
}
