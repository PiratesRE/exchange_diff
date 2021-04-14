using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.Serialization
{
	public class EntitySerializer
	{
		public static void Serialize<T>(T obj, Stream stream)
		{
			DataContractSerializer dataContractSerializer = new DataContractSerializer(obj.GetType(), EntitySerializer.DefaultSerializerSettings);
			dataContractSerializer.WriteObject(stream, obj);
		}

		public static T Deserialize<T>(Stream stream)
		{
			DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T), EntitySerializer.DefaultSerializerSettings);
			return (T)((object)dataContractSerializer.ReadObject(stream));
		}

		public static string Serialize<T>(T obj)
		{
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EntitySerializer.Serialize<T>(obj, memoryStream);
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return @string;
		}

		public static T Deserialize<T>(string serializedObject)
		{
			T result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedObject)))
			{
				result = EntitySerializer.Deserialize<T>(memoryStream);
			}
			return result;
		}

		private static readonly DataContractSerializerSettings DefaultSerializerSettings = new DataContractSerializerSettings
		{
			DataContractSurrogate = PropertyChangeTrackingObject.DataContractSurrogate,
			PreserveObjectReferences = true,
			DataContractResolver = new EntitySerializer.EntityDataContractResolver()
		};

		private class EntityDataContractResolver : DataContractResolver
		{
			public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
			{
				typeName = new XmlDictionaryString(XmlDictionary.Empty, type.FullName, 0);
				string name = type.Assembly.GetName().Name;
				if (name == EntitySerializer.EntityDataContractResolver.MscorlibAssemblyName)
				{
					typeNamespace = XmlDictionaryString.Empty;
				}
				else
				{
					typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, type.Assembly.FullName, 0);
				}
				return true;
			}

			public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
			{
				Type type = Type.GetType(typeName, false);
				if (type == null)
				{
					Assembly assembly = Assembly.Load(typeNamespace);
					type = assembly.GetType(typeName, false);
				}
				return type ?? knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
			}

			private static readonly string MscorlibAssemblyName = typeof(int).Assembly.GetName().Name;
		}
	}
}
