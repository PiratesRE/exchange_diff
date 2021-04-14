using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public static class ServiceInstanceMapSerializer
	{
		public static string ConvertServiceInstanceMapToXml(ServiceInstanceMapValue map)
		{
			if (map != null)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServiceInstanceMapValue));
				using (StringWriter stringWriter = new StringWriter())
				{
					xmlSerializer.Serialize(stringWriter, map);
					return stringWriter.ToString();
				}
			}
			return string.Empty;
		}

		public static ServiceInstanceMapValue ConvertXmlToServiceInstanceMap(string xml)
		{
			if (string.IsNullOrEmpty(xml))
			{
				return null;
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServiceInstanceMapValue));
			ServiceInstanceMapValue result;
			using (StringReader stringReader = new StringReader(xml))
			{
				try
				{
					result = (xmlSerializer.Deserialize(stringReader) as ServiceInstanceMapValue);
				}
				catch (InvalidOperationException ex)
				{
					if (ex.InnerException is XmlException)
					{
						throw new InvalidServiceInstanceMapXmlFormatException(ex.InnerException.Message, ex);
					}
					if (ex.InnerException is InvalidOperationException)
					{
						throw new InvalidServiceInstanceMapXmlFormatException(string.Format("{0} {1}", ex.InnerException.Message, ex.Message), ex);
					}
					throw;
				}
			}
			return result;
		}
	}
}
