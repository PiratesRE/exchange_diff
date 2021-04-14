using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class ExpectedResult
	{
		public ExpectedResultType Type
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

		public bool IsRegEx
		{
			get
			{
				return this.isRegEx;
			}
			set
			{
				this.isRegEx = value;
			}
		}

		public static ICollection<ExpectedResult> FromXml(XmlNode workContext)
		{
			List<ExpectedResult> list = new List<ExpectedResult>();
			using (XmlNodeList xmlNodeList = workContext.SelectNodes("ExpectedResult"))
			{
				foreach (object obj in xmlNodeList)
				{
					XmlElement xmlElement = (XmlElement)obj;
					ExpectedResult expectedResult = new ExpectedResult();
					ExpectedResultType expectedResultType = ExpectedResultType.Body;
					string attribute = xmlElement.GetAttribute("Type");
					if (string.IsNullOrWhiteSpace(attribute) || !Enum.TryParse<ExpectedResultType>(attribute, true, out expectedResultType))
					{
						throw new ArgumentException("Expected result type is a required argument but was not specified or was not a valid value.");
					}
					expectedResult.Type = expectedResultType;
					XmlAttribute xmlAttribute = xmlElement.Attributes["Value"];
					if (xmlAttribute != null && !string.IsNullOrWhiteSpace(xmlAttribute.Value))
					{
						expectedResult.Value = xmlAttribute.Value;
					}
					else
					{
						XmlNode xmlNode = xmlElement.SelectSingleNode("Value");
						Utils.CheckNode(xmlNode, "ExpectedResult Value");
						expectedResult.Value = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "ExpectedResult Value");
					}
					attribute = xmlElement.GetAttribute("IsRegEx");
					expectedResult.IsRegEx = false;
					bool flag;
					if (!string.IsNullOrWhiteSpace(attribute) && bool.TryParse(attribute, out flag))
					{
						expectedResult.IsRegEx = flag;
					}
					list.Add(expectedResult);
				}
			}
			return list;
		}

		private ExpectedResultType type;

		private string value;

		private bool isRegEx;
	}
}
