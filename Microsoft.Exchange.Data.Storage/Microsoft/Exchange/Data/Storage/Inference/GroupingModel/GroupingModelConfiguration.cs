using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	public class GroupingModelConfiguration
	{
		[XmlElement]
		public int CurrentVersion { get; set; }

		[XmlElement]
		public int MinimumSupportedVersion { get; set; }

		public IGroupingModelConfiguration AsReadOnly()
		{
			return new GroupingModelConfigurationWrapper(this);
		}

		public static GroupingModelConfiguration LoadFrom(string groupingModelConfigurationDefinition)
		{
			GroupingModelConfiguration result;
			using (TextReader textReader = new StringReader(groupingModelConfigurationDefinition))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GroupingModelConfiguration));
				result = (GroupingModelConfiguration)xmlSerializer.Deserialize(textReader);
			}
			return result;
		}

		public static GroupingModelConfiguration LoadFromFile()
		{
			string filepath = Path.Combine(ExchangeSetupContext.BinPath, "GroupingModelConfiguration.Xml");
			return GroupingModelConfiguration.LoadFromFile(filepath);
		}

		public static GroupingModelConfiguration LoadFromFile(string filepath)
		{
			GroupingModelConfiguration result;
			using (XmlReader xmlReader = new XmlTextReader(filepath))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GroupingModelConfiguration));
				result = (GroupingModelConfiguration)xmlSerializer.Deserialize(xmlReader);
			}
			return result;
		}

		internal static string WriteTo(GroupingModelConfiguration groupingModelConfiguration)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (TextWriter textWriter = new StringWriter(stringBuilder))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GroupingModelConfiguration));
				xmlSerializer.Serialize(textWriter, groupingModelConfiguration);
			}
			return stringBuilder.ToString();
		}

		public const string FileName = "GroupingModelConfiguration.Xml";
	}
}
