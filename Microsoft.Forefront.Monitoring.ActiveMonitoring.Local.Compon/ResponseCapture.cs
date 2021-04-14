using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class ResponseCapture
	{
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Pattern
		{
			get
			{
				return this.pattern;
			}
			set
			{
				this.pattern = value;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public CaptureType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public string PersistInStateAttributeName
		{
			get
			{
				return this.persistInStateAttributeName;
			}
			set
			{
				this.persistInStateAttributeName = value;
			}
		}

		public static ICollection<ResponseCapture> FromXml(XmlNode workContext)
		{
			List<ResponseCapture> list = new List<ResponseCapture>();
			using (XmlNodeList xmlNodeList = workContext.SelectNodes("Capture"))
			{
				foreach (object obj in xmlNodeList)
				{
					XmlElement xmlElement = (XmlElement)obj;
					ResponseCapture responseCapture = new ResponseCapture();
					responseCapture.Name = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Name"), "Capture Name");
					XmlAttribute xmlAttribute = xmlElement.Attributes["Pattern"];
					if (xmlAttribute != null && !string.IsNullOrWhiteSpace(xmlAttribute.Value))
					{
						responseCapture.Pattern = xmlAttribute.Value;
					}
					else
					{
						XmlNode xmlNode = xmlElement.SelectSingleNode("Pattern");
						Utils.CheckNode(xmlNode, "Capture Pattern");
						responseCapture.Pattern = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "Capture Pattern");
					}
					responseCapture.Type = CaptureType.ResponseText;
					string attribute = xmlElement.GetAttribute("Type");
					if (!string.IsNullOrWhiteSpace(attribute))
					{
						responseCapture.Type = Utils.GetEnumValue<CaptureType>(attribute, "Capture Type");
					}
					responseCapture.persistInStateAttributeName = Utils.GetOptionalXmlAttribute<string>(xmlElement, "PersistInStateAttributeName", null);
					responseCapture.Value = string.Empty;
					list.Add(responseCapture);
				}
			}
			return list;
		}

		private string name;

		private string pattern;

		private string value;

		private CaptureType type;

		private string persistInStateAttributeName;
	}
}
