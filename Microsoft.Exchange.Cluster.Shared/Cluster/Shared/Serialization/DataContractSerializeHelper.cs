using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Shared.Serialization
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DataContractSerializeHelper
	{
		public static Exception SerializeToXmlString(object toSerialize, out string xmlString)
		{
			xmlString = null;
			string tmpStr = null;
			Exception ex = DataContractSerializeHelper.HandleException(delegate
			{
				XmlWriterSettings settings = new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "\t"
				};
				DataContractSerializer dataContractSerializer = new DataContractSerializer(toSerialize.GetType());
				StringBuilder stringBuilder = new StringBuilder();
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
				{
					using (XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter))
					{
						dataContractSerializer.WriteObject(xmlDictionaryWriter, toSerialize);
						xmlDictionaryWriter.Flush();
						tmpStr = stringBuilder.ToString();
					}
				}
			});
			if (ex == null)
			{
				xmlString = tmpStr;
			}
			return ex;
		}

		public static Exception DeserializeFromXmlString<T>(string xml, out T deserializedObj)
		{
			deserializedObj = default(T);
			T tmpObj = default(T);
			Exception ex = DataContractSerializeHelper.HandleException(delegate
			{
				DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
				using (StringReader stringReader = new StringReader(xml))
				{
					using (XmlReader xmlReader = XmlReader.Create(stringReader))
					{
						tmpObj = (T)((object)dataContractSerializer.ReadObject(xmlReader));
					}
				}
			});
			if (ex == null)
			{
				deserializedObj = tmpObj;
			}
			return ex;
		}

		public static Exception SerializeToXmlFile(object toSerialize, string fileFullPath)
		{
			return DataContractSerializeHelper.HandleException(delegate
			{
				string directoryName = Path.GetDirectoryName(fileFullPath);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				if (File.Exists(fileFullPath))
				{
					File.Delete(fileFullPath);
				}
				using (FileStream fileStream = File.OpenWrite(fileFullPath))
				{
					XmlWriterSettings settings = new XmlWriterSettings
					{
						Indent = true,
						IndentChars = "\t",
						Encoding = Encoding.UTF8
					};
					DataContractSerializer dataContractSerializer = new DataContractSerializer(toSerialize.GetType());
					using (XmlWriter xmlWriter = XmlWriter.Create(fileStream, settings))
					{
						using (XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter))
						{
							dataContractSerializer.WriteObject(xmlDictionaryWriter, toSerialize);
							xmlDictionaryWriter.Flush();
						}
					}
				}
			});
		}

		public static Exception DeserializeFromXmlFile<T>(string fileFullPath, out T deserializedObj)
		{
			deserializedObj = default(T);
			T tmpObj = default(T);
			Exception ex = DataContractSerializeHelper.HandleException(delegate
			{
				using (FileStream fileStream = File.OpenRead(fileFullPath))
				{
					using (XmlDictionaryReader xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas()))
					{
						DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
						tmpObj = (T)((object)dataContractSerializer.ReadObject(xmlDictionaryReader, true));
					}
				}
			});
			if (ex == null)
			{
				deserializedObj = tmpObj;
			}
			return ex;
		}

		public static Exception HandleException(Action operation)
		{
			Exception result = null;
			try
			{
				operation();
			}
			catch (IOException ex)
			{
				result = ex;
			}
			catch (SecurityException ex2)
			{
				result = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				result = ex3;
			}
			catch (InvalidDataContractException ex4)
			{
				result = ex4;
			}
			catch (SerializationException ex5)
			{
				result = ex5;
			}
			catch (QuotaExceededException ex6)
			{
				result = ex6;
			}
			catch (XmlException ex7)
			{
				result = ex7;
			}
			return result;
		}
	}
}
