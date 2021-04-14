using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using DataContractSerializerProject;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.Diagnostics
{
	public class EntityLogger
	{
		public static string GetLoggingDetails(IPropertyChangeTracker<PropertyDefinition> entity)
		{
			DataContractSerializer dataContractSerializer = new DataContractSerializer(entity.GetType(), EntityLogger.LoggingSerializerSettings);
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (EntityXmlTextWriter entityXmlTextWriter = new EntityXmlTextWriter(stringWriter))
				{
					dataContractSerializer.WriteObject(entityXmlTextWriter, entity);
					result = stringWriter.ToString();
				}
			}
			return result;
		}

		private static readonly DataContractSerializerSettings LoggingSerializerSettings = new DataContractSerializerSettings
		{
			DataContractSurrogate = PropertyChangeTrackingObject.LoggingSurrogate,
			PreserveObjectReferences = true,
			DataContractResolver = new EntityLogger.EntityLoggerDataContractResolver(),
			RootName = new XmlDictionaryString(XmlDictionary.Empty, "Entity", 0)
		};

		private class EntityLoggerDataContractResolver : DataContractResolver
		{
			public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
			{
				string value = XmlConvert.EncodeName(type.Name);
				typeName = new XmlDictionaryString(XmlDictionary.Empty, value, 0);
				typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, string.Empty, 0);
				return true;
			}

			public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
			{
				throw new NotImplementedException();
			}
		}
	}
}
