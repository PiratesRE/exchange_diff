using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class MarkupParser
	{
		public MarkupParser()
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml("<linkLabel></linkLabel>");
			this.xml = safeXmlDocument.DocumentElement;
		}

		public string Markup
		{
			get
			{
				return this.xml.InnerXml;
			}
			set
			{
				this.xml.InnerXml = value;
			}
		}

		public string Text
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj in this.xml.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					stringBuilder.Append(xmlNode.InnerText);
				}
				return stringBuilder.ToString();
			}
		}

		public XmlNodeList Nodes
		{
			get
			{
				return this.xml.ChildNodes;
			}
		}

		public void ReplaceAnchorValues(object dataSource, string listSeparator)
		{
			if (dataSource != null)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(dataSource);
				foreach (object obj in this.xml.SelectNodes("a"))
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (!string.IsNullOrEmpty(xmlNode.InnerText))
					{
						string value = xmlNode.Attributes["id"].Value;
						PropertyDescriptor propertyDescriptor = properties[value];
						if (propertyDescriptor != null)
						{
							object value2 = propertyDescriptor.GetValue(dataSource);
							if (!WinformsHelper.IsEmptyValue(value2))
							{
								xmlNode.InnerText = MarkupParser.ValueToString(value2, listSeparator);
							}
						}
					}
				}
			}
		}

		public Dictionary<string, bool> GetEditingProperties()
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (object obj in this.xml.SelectNodes("a"))
			{
				XmlNode xmlNode = (XmlNode)obj;
				string value = xmlNode.Attributes["id"].Value;
				if (!string.IsNullOrEmpty(value) && !dictionary.ContainsKey(value))
				{
					dictionary.Add(value, !string.IsNullOrEmpty(xmlNode.InnerText));
				}
			}
			return dictionary;
		}

		public static string ValueToString(object value, string listSeparator)
		{
			string result = string.Empty;
			listSeparator = ((!string.IsNullOrEmpty(listSeparator)) ? listSeparator : CultureInfo.CurrentCulture.TextInfo.ListSeparator);
			if (value != null)
			{
				result = value.ToUserFriendText(listSeparator, new ObjectExtension.IsQuotationRequiredDelegate(MarkupParser.IsQuotationRequiredType));
			}
			return result;
		}

		private static bool IsQuotationRequiredType(object value)
		{
			Type type = typeof(ICollection);
			if (value.GetType().IsGenericType)
			{
				type = typeof(ICollection<>).MakeGenericType(value.GetType().GetGenericArguments());
			}
			return !type.IsAssignableFrom(value.GetType());
		}

		private XmlElement xml;
	}
}
