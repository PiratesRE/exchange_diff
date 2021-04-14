using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public static class Utilities
	{
		public static void SerializeObjectToFile(object obj, string fileName)
		{
			string directoryName = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read, 8, FileOptions.WriteThrough))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, Encoding.Unicode))
				{
					xmlSerializer.Serialize(xmlTextWriter, obj);
					xmlTextWriter.Close();
				}
				fileStream.Flush(true);
				fileStream.Close();
			}
		}

		public static T DeserializeObjectFromFile<T>(string fileName)
		{
			T result = default(T);
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				result = (T)((object)xmlSerializer.Deserialize(fileStream));
				fileStream.Close();
			}
			return result;
		}

		public static string NormalizeStringToValidFileOrRegistryKeyName(string input)
		{
			return Regex.Replace(input, "[<>|:*?\\/]", "-");
		}

		public static bool IsSequenceNullOrEmpty<T>(IEnumerable<T> sequence)
		{
			return sequence == null || !sequence.Any<T>();
		}
	}
}
