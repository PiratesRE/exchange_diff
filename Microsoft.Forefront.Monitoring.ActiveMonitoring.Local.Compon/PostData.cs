using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class PostData : Dictionary<string, string>
	{
		public PostData()
		{
		}

		public PostData(Dictionary<string, string> dictionary) : base(dictionary)
		{
		}

		public static PostData FromXml(XmlNode workContext)
		{
			PostData postData = new PostData();
			using (XmlNodeList xmlNodeList = workContext.SelectNodes("PostData"))
			{
				foreach (object obj in xmlNodeList)
				{
					XmlElement xmlElement = (XmlElement)obj;
					string key = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Key"), "PostData Key");
					XmlAttribute xmlAttribute = xmlElement.Attributes["Value"];
					string value;
					if (xmlAttribute != null && !string.IsNullOrWhiteSpace(xmlAttribute.Value))
					{
						value = xmlAttribute.Value;
					}
					else
					{
						XmlNode xmlNode = xmlElement.SelectSingleNode("Value");
						Utils.CheckNode(xmlNode, "PostData Value");
						value = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "PostData Value");
					}
					postData.Add(key, value);
				}
			}
			return postData;
		}

		public void AddRange(Dictionary<string, string> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("Value of parameter dictionary cannot be null.");
			}
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				if (!base.ContainsKey(keyValuePair.Key))
				{
					base.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public override string ToString()
		{
			return this.ToString(false);
		}

		public string ToString(bool formEncoded)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in this)
			{
				if (formEncoded)
				{
					string str = HttpUtility.HtmlDecode(keyValuePair.Value);
					stringBuilder.AppendFormat("{0}={1}&", keyValuePair.Key, HttpUtility.UrlEncode(str));
				}
				else
				{
					stringBuilder.Append(keyValuePair.Value);
				}
			}
			if (!formEncoded)
			{
				return stringBuilder.ToString();
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'&'
			});
		}
	}
}
