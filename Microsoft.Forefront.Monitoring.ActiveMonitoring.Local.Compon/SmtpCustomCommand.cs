using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class SmtpCustomCommand
	{
		public string Name
		{
			get
			{
				return this.name;
			}
			internal set
			{
				this.name = value;
			}
		}

		public string Arguments
		{
			get
			{
				return this.arguments;
			}
			internal set
			{
				this.arguments = value;
			}
		}

		public CustomCommandRunPoint CustomCommandRunPoint
		{
			get
			{
				return this.runPoint;
			}
			internal set
			{
				this.runPoint = value;
			}
		}

		public SmtpExpectedResponse ExpectedResponse
		{
			get
			{
				return this.expectedResponse;
			}
			internal set
			{
				this.expectedResponse = value;
			}
		}

		public static ICollection<SmtpCustomCommand> FromXml(XmlNode workContext)
		{
			List<SmtpCustomCommand> list = new List<SmtpCustomCommand>();
			using (XmlNodeList xmlNodeList = workContext.SelectNodes("//CustomCommand"))
			{
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					SmtpCustomCommand smtpCustomCommand = new SmtpCustomCommand();
					XmlElement xmlElement = xmlNode as XmlElement;
					smtpCustomCommand.name = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Name"), "CustomCommand Name");
					smtpCustomCommand.runPoint = Utils.GetEnumValue<CustomCommandRunPoint>(xmlElement.GetAttribute("RunPoint"), "CustomCommand RunPoint");
					smtpCustomCommand.expectedResponse = SmtpExpectedResponse.FromXml(xmlNode.SelectSingleNode("ExpectedResponse"), "ExpectedResponse", SimpleSmtpClient.SmtpResponseCode.OK, false);
					string attribute = xmlElement.GetAttribute("Arguments");
					if (!string.IsNullOrWhiteSpace(attribute))
					{
						smtpCustomCommand.arguments = attribute;
					}
					list.Add(smtpCustomCommand);
				}
			}
			return list;
		}

		private string name;

		private string arguments;

		private CustomCommandRunPoint runPoint;

		private SmtpExpectedResponse expectedResponse;
	}
}
