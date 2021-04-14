using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Transport.QueueViewer
{
	internal static class Serialization
	{
		public static byte[] ObjectToBytes(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			byte[] buffer;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, obj);
				buffer = memoryStream.GetBuffer();
			}
			return buffer;
		}

		public static object BytesToObject(byte[] mBinaryData)
		{
			if (mBinaryData == null || mBinaryData.Length == 0)
			{
				return null;
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
			object result;
			using (MemoryStream memoryStream = new MemoryStream(mBinaryData, false))
			{
				result = binaryFormatter.Deserialize(memoryStream);
			}
			return result;
		}

		public static byte[] InputObjectToBytes(QueueViewerInputObject input)
		{
			byte[] result = new byte[100];
			int num = 0;
			int num2 = 6;
			if (input.QueryFilter != null)
			{
				num2++;
			}
			if (input.SortOrder != null)
			{
				num2++;
			}
			string value = string.Empty;
			if (input.BookmarkObject != null)
			{
				if (input.BookmarkObject is PropertyBagBasedQueueInfo)
				{
					value = "PropertyBagBasedQueueInfo";
					num2++;
				}
				else if (input.BookmarkObject is PropertyBagBasedMessageInfo)
				{
					value = "PropertyBagBasedMessageInfo";
					num2++;
				}
			}
			PropertyStreamWriter.WritePropertyKeyValue("Version", StreamPropertyType.String, Serialization.Version.ToString(), ref result, ref num);
			PropertyStreamWriter.WritePropertyKeyValue("NumProperties", StreamPropertyType.Int32, num2, ref result, ref num);
			PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.BookmarkIndex.Name, StreamPropertyType.Int32, input.BookmarkIndex, ref result, ref num);
			PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.IncludeBookmark.Name, StreamPropertyType.Bool, input.IncludeBookmark, ref result, ref num);
			PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.IncludeComponentLatencyInfo.Name, StreamPropertyType.Bool, input.IncludeComponentLatencyInfo, ref result, ref num);
			PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.IncludeDetails.Name, StreamPropertyType.Bool, input.IncludeDetails, ref result, ref num);
			PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.PageSize.Name, StreamPropertyType.Int32, input.PageSize, ref result, ref num);
			PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.SearchForward.Name, StreamPropertyType.Bool, input.SearchForward, ref result, ref num);
			if (input.QueryFilter != null)
			{
				PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.QueryFilter.Name, StreamPropertyType.String, input.QueryFilter.GenerateInfixString(FilterLanguage.Monad), ref result, ref num);
			}
			if (input.SortOrder != null)
			{
				PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.SortOrder.Name, StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array, (from o in input.SortOrder
				select o.ToString()).ToArray<string>(), ref result, ref num);
			}
			if (input.BookmarkObject != null && !string.IsNullOrEmpty(value))
			{
				PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.BookmarkObject.Name, StreamPropertyType.String, value, ref result, ref num);
				if (input.BookmarkObject is PropertyBagBasedQueueInfo)
				{
					((PropertyBagBasedQueueInfo)input.BookmarkObject).ToByteArray(ref result, ref num);
				}
				else
				{
					if (!(input.BookmarkObject is PropertyBagBasedMessageInfo))
					{
						throw new InvalidOperationException(string.Format("Unexpected bookmark object type found: {0}", input.BookmarkObject.GetType().Name));
					}
					((PropertyBagBasedMessageInfo)input.BookmarkObject).ToByteArray(Serialization.Version, ref result, ref num);
				}
			}
			return result;
		}

		public static QueueViewerInputObject BytesToInputObject(QVObjectType objectType, byte[] binaryData, out Version sourceVersion)
		{
			if (binaryData == null)
			{
				throw new SerializationException("input bytes is null");
			}
			PropertyStreamReader propertyStreamReader = new PropertyStreamReader(new MemoryStream(binaryData));
			int bookmarkIndex = -1;
			bool includeBookmark = false;
			bool includeComponentLatencyInfo = false;
			bool includeDetails = false;
			int pageSize = 0;
			bool searchForward = false;
			QueryFilter queryFilter = null;
			QueueViewerSortOrderEntry[] array = null;
			IConfigurable bookmarkObject = null;
			sourceVersion = Serialization.CheckVersion(propertyStreamReader, false);
			int num = Serialization.ReadPropertyValue<int>(propertyStreamReader, "NumProperties");
			for (int i = 0; i < num; i++)
			{
				KeyValuePair<string, object> keyValuePair;
				propertyStreamReader.Read(out keyValuePair);
				if (string.Equals(QueueViewerInputObjectSchema.BookmarkIndex.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					bookmarkIndex = (int)keyValuePair.Value;
				}
				else if (string.Equals(QueueViewerInputObjectSchema.IncludeBookmark.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					includeBookmark = (bool)keyValuePair.Value;
				}
				else if (string.Equals(QueueViewerInputObjectSchema.IncludeComponentLatencyInfo.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					includeComponentLatencyInfo = (bool)keyValuePair.Value;
				}
				else if (string.Equals(QueueViewerInputObjectSchema.IncludeDetails.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					includeDetails = (bool)keyValuePair.Value;
				}
				else if (string.Equals(QueueViewerInputObjectSchema.PageSize.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					pageSize = (int)keyValuePair.Value;
				}
				else if (string.Equals(QueueViewerInputObjectSchema.SearchForward.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					searchForward = (bool)keyValuePair.Value;
				}
				else
				{
					if (string.Equals(QueueViewerInputObjectSchema.QueryFilter.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
					{
						try
						{
							if (objectType == QVObjectType.QueueInfo)
							{
								queryFilter = new MonadFilter((string)keyValuePair.Value, null, ObjectSchema.GetInstance<ExtensibleQueueInfoSchema>()).InnerFilter;
							}
							else
							{
								queryFilter = new MonadFilter((string)keyValuePair.Value, null, ObjectSchema.GetInstance<ExtensibleMessageInfoSchema>()).InnerFilter;
							}
							goto IL_2E3;
						}
						catch (ParsingNonFilterablePropertyException arg)
						{
							ExTraceGlobals.QueuingTracer.TraceWarning<ParsingNonFilterablePropertyException>(0L, "Unrecognized filter property found {0}", arg);
							goto IL_2E3;
						}
						catch (ParsingException arg2)
						{
							ExTraceGlobals.QueuingTracer.TraceWarning<ParsingException>(0L, "parse error in filter {0}", arg2);
							goto IL_2E3;
						}
					}
					if (string.Equals(QueueViewerInputObjectSchema.SortOrder.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
					{
						string[] array2 = (string[])keyValuePair.Value;
						array = new QueueViewerSortOrderEntry[array2.Length];
						int num2 = 0;
						foreach (string s in array2)
						{
							array[num2++] = QueueViewerSortOrderEntry.Parse(s);
						}
					}
					else if (string.Equals(QueueViewerInputObjectSchema.BookmarkObject.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
					{
						string text = (string)keyValuePair.Value;
						if (string.Equals("PropertyBagBasedQueueInfo", text, StringComparison.InvariantCultureIgnoreCase))
						{
							bookmarkObject = PropertyBagBasedQueueInfo.CreateFromByteStream(propertyStreamReader);
						}
						else if (string.Equals("PropertyBagBasedMessageInfo", text, StringComparison.InvariantCultureIgnoreCase))
						{
							bookmarkObject = PropertyBagBasedMessageInfo.CreateFromByteStream(propertyStreamReader, sourceVersion);
						}
						else
						{
							ExTraceGlobals.QueuingTracer.TraceWarning<string>(0L, "Unrecognized bookmark type found {0}", text);
						}
					}
					else
					{
						ExTraceGlobals.QueuingTracer.TraceWarning<string>(0L, "Unrecognized property found {0}", keyValuePair.Key);
					}
				}
				IL_2E3:;
			}
			return new QueueViewerInputObject(bookmarkIndex, bookmarkObject, includeBookmark, includeComponentLatencyInfo, includeDetails, pageSize, queryFilter, searchForward, array);
		}

		public static byte[] PagedObjectToBytes(PagedDataObject[] pagedObjects, Version targetVersion)
		{
			if (pagedObjects == null || pagedObjects.Length == 0)
			{
				return new byte[0];
			}
			if (pagedObjects[0] is QueueInfo || pagedObjects[0] is MessageInfo)
			{
				throw new InvalidOperationException("LegacyQueueInfo and LegacyMessageInfo should use ObjectToBytes method");
			}
			byte[] result = new byte[pagedObjects.Length * 100];
			int num = 0;
			PropertyStreamWriter.WritePropertyKeyValue("Version", StreamPropertyType.String, targetVersion.ToString(), ref result, ref num);
			if (pagedObjects[0] is PropertyBagBasedQueueInfo)
			{
				PropertyStreamWriter.WritePropertyKeyValue("ObjectType", StreamPropertyType.String, "PropertyBagBasedQueueInfo", ref result, ref num);
				PropertyStreamWriter.WritePropertyKeyValue("NumObjects", StreamPropertyType.Int32, pagedObjects.Length, ref result, ref num);
				foreach (PagedDataObject pagedDataObject in pagedObjects)
				{
					PropertyBagBasedQueueInfo propertyBagBasedQueueInfo = (PropertyBagBasedQueueInfo)pagedDataObject;
					propertyBagBasedQueueInfo.ToByteArray(ref result, ref num);
				}
			}
			else
			{
				if (!(pagedObjects[0] is PropertyBagBasedMessageInfo))
				{
					throw new SerializationException(string.Format("Dont know how to serialize objects of type '{0}'", pagedObjects[0].GetType().Name));
				}
				PropertyStreamWriter.WritePropertyKeyValue("ObjectType", StreamPropertyType.String, "PropertyBagBasedMessageInfo", ref result, ref num);
				PropertyStreamWriter.WritePropertyKeyValue("NumObjects", StreamPropertyType.Int32, pagedObjects.Length, ref result, ref num);
				foreach (PagedDataObject pagedDataObject2 in pagedObjects)
				{
					PropertyBagBasedMessageInfo propertyBagBasedMessageInfo = (PropertyBagBasedMessageInfo)pagedDataObject2;
					propertyBagBasedMessageInfo.ToByteArray(targetVersion, ref result, ref num);
				}
			}
			return result;
		}

		public static ExtensibleQueueInfo[] BytesToPagedQueueObject(byte[] binaryData)
		{
			if (binaryData == null)
			{
				return new ExtensibleQueueInfo[0];
			}
			PropertyStreamReader reader = new PropertyStreamReader(new MemoryStream(binaryData));
			Serialization.CheckVersion(reader, true);
			string text = Serialization.ReadPropertyValue<string>(reader, "ObjectType");
			int num = Serialization.ReadPropertyValue<int>(reader, "NumObjects");
			List<ExtensibleQueueInfo> list = new List<ExtensibleQueueInfo>();
			if (string.Equals("PropertyBagBasedQueueInfo", text, StringComparison.InvariantCultureIgnoreCase))
			{
				for (int i = 0; i < num; i++)
				{
					PropertyBagBasedQueueInfo item = PropertyBagBasedQueueInfo.CreateFromByteStream(reader);
					list.Add(item);
				}
				return list.ToArray();
			}
			throw new SerializationException(string.Format("Unexpected objectType encountered: '{0}'", text));
		}

		public static ExtensibleMessageInfo[] BytesToPagedMessageObject(byte[] binaryData)
		{
			if (binaryData == null)
			{
				return new ExtensibleMessageInfo[0];
			}
			PropertyStreamReader reader = new PropertyStreamReader(new MemoryStream(binaryData));
			Version sourceVersion = Serialization.CheckVersion(reader, true);
			string text = Serialization.ReadPropertyValue<string>(reader, "ObjectType");
			int num = Serialization.ReadPropertyValue<int>(reader, "NumObjects");
			List<ExtensibleMessageInfo> list = new List<ExtensibleMessageInfo>();
			if (string.Equals("PropertyBagBasedMessageInfo", text, StringComparison.InvariantCultureIgnoreCase))
			{
				for (int i = 0; i < num; i++)
				{
					PropertyBagBasedMessageInfo item = PropertyBagBasedMessageInfo.CreateFromByteStream(reader, sourceVersion);
					list.Add(item);
				}
				return list.ToArray();
			}
			throw new SerializationException(string.Format("Unexpected objectType encountered: '{0}'", text));
		}

		public static ExtensibleMessageInfo BytesToSingleMessageObject(byte[] binaryData)
		{
			if (binaryData == null)
			{
				return null;
			}
			PropertyStreamReader reader = new PropertyStreamReader(new MemoryStream(binaryData));
			Version sourceVersion = Serialization.CheckVersion(reader, true);
			return PropertyBagBasedMessageInfo.CreateFromByteStream(reader, sourceVersion);
		}

		public static byte[] SingleMessageObjectToBytes(PagedDataObject properties, Version targetVersion)
		{
			if (properties == null)
			{
				return new byte[0];
			}
			if (!(properties is PropertyBagBasedMessageInfo))
			{
				throw new InvalidOperationException("not a propertyBagBasedMessageInfo object");
			}
			byte[] result = new byte[100];
			int num = 0;
			PropertyBagBasedMessageInfo propertyBagBasedMessageInfo = (PropertyBagBasedMessageInfo)properties;
			PropertyStreamWriter.WritePropertyKeyValue("Version", StreamPropertyType.String, targetVersion.ToString(), ref result, ref num);
			propertyBagBasedMessageInfo.ToByteArray(targetVersion, ref result, ref num);
			return result;
		}

		public static MessageIdentity BytesToMessageId(byte[] binaryData)
		{
			if (binaryData == null)
			{
				return null;
			}
			PropertyStreamReader propertyStreamReader = new PropertyStreamReader(new MemoryStream(binaryData));
			Version sourceVersion = Serialization.CheckVersion(propertyStreamReader, true);
			KeyValuePair<string, object> pair;
			propertyStreamReader.Read(out pair);
			return MessageIdentity.Create(sourceVersion, pair, propertyStreamReader);
		}

		public static byte[] MessageIdToBytes(MessageIdentity identity, Version targetVersion)
		{
			if (identity == null)
			{
				return new byte[0];
			}
			byte[] result = new byte[100];
			int num = 0;
			PropertyStreamWriter.WritePropertyKeyValue("Version", StreamPropertyType.String, targetVersion.ToString(), ref result, ref num);
			identity.ToByteArray(targetVersion, ref result, ref num);
			return result;
		}

		public static QueryFilter BytesToFilterObject(byte[] binaryData)
		{
			PropertyStreamReader propertyStreamReader = new PropertyStreamReader(new MemoryStream(binaryData));
			Serialization.CheckVersion(propertyStreamReader, false);
			QueryFilter result = null;
			int num = Serialization.ReadPropertyValue<int>(propertyStreamReader, "NumProperties");
			for (int i = 0; i < num; i++)
			{
				KeyValuePair<string, object> keyValuePair;
				propertyStreamReader.Read(out keyValuePair);
				if (string.Equals(QueueViewerInputObjectSchema.QueryFilter.Name, keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					result = new MonadFilter((string)keyValuePair.Value, null, ObjectSchema.GetInstance<ExtensibleMessageInfoSchema>()).InnerFilter;
					break;
				}
			}
			return result;
		}

		public static byte[] FilterObjectToBytes(QueryFilter filter)
		{
			if (filter == null)
			{
				return new byte[0];
			}
			byte[] result = new byte[100];
			int num = 0;
			PropertyStreamWriter.WritePropertyKeyValue("Version", StreamPropertyType.String, Serialization.Version.ToString(), ref result, ref num);
			PropertyStreamWriter.WritePropertyKeyValue("NumProperties", StreamPropertyType.Int32, 1, ref result, ref num);
			PropertyStreamWriter.WritePropertyKeyValue(QueueViewerInputObjectSchema.QueryFilter.Name, StreamPropertyType.String, filter.GenerateInfixString(FilterLanguage.Monad), ref result, ref num);
			return result;
		}

		private static T ReadPropertyValue<T>(PropertyStreamReader reader, string expectedPropertyName)
		{
			KeyValuePair<string, object> keyValuePair;
			reader.Read(out keyValuePair);
			if (!string.Equals(keyValuePair.Key, expectedPropertyName, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SerializationException(string.Format("Unexpected property name found {0}, expected {1}", keyValuePair.Key, expectedPropertyName));
			}
			return (T)((object)keyValuePair.Value);
		}

		private static Version CheckVersion(PropertyStreamReader reader, bool clientSide)
		{
			string text = Serialization.ReadPropertyValue<string>(reader, "Version");
			Version version;
			if (!Version.TryParse(text, out version))
			{
				throw new SerializationException(string.Format("Cannot parse version {0}", text));
			}
			if (clientSide)
			{
				if (version != Serialization.Version)
				{
					throw new SerializationException(string.Format("client received bytes with version {0} even though client has version {1}", text, Serialization.Version));
				}
			}
			else if (version > Serialization.Version)
			{
				throw new SerializationException(string.Format("Cannot understand version {0}. Current version is {1}", text, Serialization.Version));
			}
			return version;
		}

		private const string VersionProperty = "Version";

		private const string NumProperties = "NumProperties";

		private const string NumObjects = "NumObjects";

		private const string ObjectType = "ObjectType";

		private const string PropertyBagBasedQueueInfoType = "PropertyBagBasedQueueInfo";

		private const string PropertyBagBasedMessageInfoType = "PropertyBagBasedMessageInfo";

		internal static readonly Version Version = new Version(1, 1);
	}
}
