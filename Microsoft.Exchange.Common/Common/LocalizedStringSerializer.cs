using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common
{
	public static class LocalizedStringSerializer
	{
		public static bool TrySerialize(LocalizedString localizedString, out string serializedString)
		{
			bool result;
			try
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
					{
						DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(LocalizedString), new Type[]
						{
							typeof(object[])
						});
						dataContractSerializer.WriteObject(xmlWriter, localizedString);
						xmlWriter.Flush();
						serializedString = stringWriter.ToString();
						result = true;
					}
				}
			}
			catch (InvalidDataContractException)
			{
				serializedString = null;
				result = false;
			}
			catch (SerializationException)
			{
				serializedString = null;
				result = false;
			}
			return result;
		}

		public static bool TryDeserialize(string serializedString, out LocalizedString localizedString)
		{
			bool result;
			using (StringReader stringReader = new StringReader(serializedString))
			{
				using (XmlReader xmlReader = XmlReader.Create(stringReader))
				{
					DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(LocalizedString), new Type[]
					{
						typeof(object[])
					});
					object obj = dataContractSerializer.ReadObject(xmlReader);
					if (obj != null)
					{
						localizedString = (LocalizedString)obj;
						result = true;
					}
					else
					{
						localizedString = LocalizedString.Empty;
						result = false;
					}
				}
			}
			return result;
		}
	}
}
