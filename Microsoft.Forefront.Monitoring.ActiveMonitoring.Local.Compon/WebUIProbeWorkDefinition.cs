using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class WebUIProbeWorkDefinition
	{
		public WebUIProbeWorkDefinition(string xml)
		{
			this.LoadFromContext(xml);
		}

		public ProcessCookies ProcessCookies
		{
			get
			{
				return this.processCookies;
			}
			set
			{
				this.processCookies = value;
			}
		}

		public ICollection<EndPoint> EndPoints
		{
			get
			{
				return this.endPoints;
			}
			set
			{
				this.endPoints = value;
			}
		}

		public TimeSpan PageLoadTimeout
		{
			get
			{
				return this.pageLoadTimeout;
			}
			set
			{
				this.pageLoadTimeout = value;
			}
		}

		private void LoadFromContext(string xml)
		{
			if (string.IsNullOrWhiteSpace(xml))
			{
				throw new ArgumentException("Work Definition XML is not valid.");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(xml);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//ProcessCookies");
			this.ProcessCookies = ProcessCookies.All;
			if (xmlNode != null)
			{
				this.ProcessCookies = Utils.GetEnumValue<ProcessCookies>(xmlNode.InnerText, "ProcessCookies");
			}
			xmlNode = xmlDocument.SelectSingleNode("//PageLoadTimeout");
			this.PageLoadTimeout = TimeSpan.MaxValue;
			if (xmlNode != null)
			{
				this.PageLoadTimeout = TimeSpan.FromMilliseconds((double)Utils.GetPositiveInteger(xmlNode.InnerText, "PageLoadTimeout"));
			}
			this.EndPoints = EndPoint.FromXml(xmlDocument, this.PageLoadTimeout);
		}

		private ProcessCookies processCookies;

		private ICollection<EndPoint> endPoints;

		private TimeSpan pageLoadTimeout;
	}
}
