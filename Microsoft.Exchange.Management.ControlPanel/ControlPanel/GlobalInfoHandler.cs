using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class GlobalInfoHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
			if (section != null && section.ChildNodes.Count > 0)
			{
				foreach (object obj in section.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.Attributes["lang"] != null && !string.IsNullOrEmpty(xmlNode.Attributes["lang"].Value))
					{
						string[] array = new string[2];
						string value = xmlNode.Attributes["lang"].Value;
						if (xmlNode.Attributes["name"] != null && !string.IsNullOrEmpty(xmlNode.Attributes["name"].Value))
						{
							array[0] = xmlNode.Attributes["name"].Value;
						}
						else
						{
							array[0] = string.Empty;
						}
						if (xmlNode.Attributes["address"] != null && !string.IsNullOrEmpty(xmlNode.Attributes["address"].Value))
						{
							array[1] = xmlNode.Attributes["address"].Value;
						}
						else
						{
							array[1] = string.Empty;
						}
						string[] array2 = value.Split(new char[]
						{
							','
						});
						foreach (string text in array2)
						{
							string text2 = text.Trim();
							if (!string.IsNullOrEmpty(text2))
							{
								dictionary[text2] = array;
							}
						}
					}
				}
			}
			return dictionary;
		}
	}
}
