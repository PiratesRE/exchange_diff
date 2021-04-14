using System;
using System.Xml;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class SmtpExpectedResponse
	{
		public ExpectedResponseType Type
		{
			get
			{
				return this.type;
			}
			internal set
			{
				this.type = value;
			}
		}

		public SimpleSmtpClient.SmtpResponseCode ResponseCode
		{
			get
			{
				return this.responseCode;
			}
			internal set
			{
				this.responseCode = value;
			}
		}

		public string ResponseText
		{
			get
			{
				return this.responseText;
			}
			internal set
			{
				this.responseText = value;
			}
		}

		public static SmtpExpectedResponse FromXml(XmlNode workContext, string nodePath, SimpleSmtpClient.SmtpResponseCode defaultValue, bool isRequired = false)
		{
			XmlElement xmlElement = workContext as XmlElement;
			SmtpExpectedResponse smtpExpectedResponse = new SmtpExpectedResponse
			{
				type = ExpectedResponseType.ResponseCode,
				responseCode = defaultValue
			};
			if (xmlElement != null)
			{
				string attribute = xmlElement.GetAttribute("Type");
				if (!string.IsNullOrEmpty(attribute))
				{
					smtpExpectedResponse.type = Utils.GetEnumValue<ExpectedResponseType>(attribute, string.Format("{0} Type", nodePath));
				}
				switch (smtpExpectedResponse.type)
				{
				case ExpectedResponseType.ResponseCode:
					smtpExpectedResponse.responseCode = Utils.GetEnumValue<SimpleSmtpClient.SmtpResponseCode>(xmlElement.InnerText, nodePath);
					break;
				case ExpectedResponseType.ResponseText:
					smtpExpectedResponse.responseText = Utils.CheckNullOrWhiteSpace(xmlElement.InnerText, nodePath);
					break;
				default:
					throw new ArgumentException(string.Format("Expected Response Type {0} is not supported.", smtpExpectedResponse.Type));
				}
			}
			else if (isRequired)
			{
				throw new ArgumentException(string.Format("The ExpectedResponse node ({0}) is required.", nodePath));
			}
			return smtpExpectedResponse;
		}

		private ExpectedResponseType type;

		private SimpleSmtpClient.SmtpResponseCode responseCode;

		private string responseText;
	}
}
