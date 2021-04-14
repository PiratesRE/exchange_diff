using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Exchange.DxStore.Common
{
	public static class DxSerializationUtil
	{
		public static bool UseBinarySerialize { get; set; } = true;

		public static MemoryStream Serialize<T>(T obj) where T : class
		{
			MemoryStream result;
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				if (DxSerializationUtil.UseBinarySerialize)
				{
					DxBinarySerializationUtil.Serialize(memoryStream, obj);
				}
				else
				{
					DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T), null, int.MaxValue, false, false, null, new DxSerializationUtil.SharedTypeResolver());
					dataContractSerializer.WriteObject(memoryStream, obj);
				}
				result = memoryStream;
			}
			catch (Exception ex)
			{
				EventLogger.LogErr("Serialize<T> err: {0}", new object[]
				{
					ex
				});
				throw new DxStoreSerializeException(ex.Message, ex);
			}
			return result;
		}

		public static MemoryStream SerializeMessage(HttpRequest msg)
		{
			return DxSerializationUtil.Serialize<HttpRequest>(msg);
		}

		public static T Deserialize<T>(Stream stream) where T : class
		{
			T result;
			try
			{
				if (DxSerializationUtil.UseBinarySerialize)
				{
					result = (T)((object)DxBinarySerializationUtil.Deserialize(stream));
				}
				else
				{
					DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T), null, int.MaxValue, false, false, null, new DxSerializationUtil.SharedTypeResolver());
					result = (T)((object)dataContractSerializer.ReadObject(stream));
				}
			}
			catch (Exception ex)
			{
				EventLogger.LogErr("Deserialize<T> err: {0}", new object[]
				{
					ex
				});
				throw new DxStoreSerializeException(ex.Message, ex);
			}
			return result;
		}

		public static T TryDeserialize<T>(Stream stream, out Exception ex) where T : class
		{
			ex = null;
			try
			{
				return DxSerializationUtil.Deserialize<T>(stream);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			return default(T);
		}

		public class SharedTypeResolver : DataContractResolver
		{
			public override bool TryResolveType(Type dataContractType, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
			{
				if (!knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace))
				{
					XmlDictionary xmlDictionary = new XmlDictionary();
					typeName = xmlDictionary.Add(dataContractType.FullName);
					typeNamespace = xmlDictionary.Add(dataContractType.Assembly.FullName);
				}
				return true;
			}

			public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
			{
				Type type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
				if (type == null)
				{
					if (typeNamespace.StartsWith("http:", StringComparison.OrdinalIgnoreCase))
					{
						return null;
					}
					try
					{
						string typeName2 = typeName + ", " + typeNamespace;
						type = Type.GetType(typeName2);
					}
					catch (Exception ex)
					{
						EventLogger.LogErr("ResolveName err: {0}", new object[]
						{
							ex
						});
					}
				}
				return type;
			}
		}
	}
}
