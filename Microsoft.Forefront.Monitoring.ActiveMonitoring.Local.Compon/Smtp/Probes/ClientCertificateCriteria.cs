using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class ClientCertificateCriteria
	{
		public StoreLocation StoreLocation { get; internal set; }

		public StoreName StoreName { get; internal set; }

		public X509FindType FindType { get; internal set; }

		public string FindValue { get; internal set; }

		public string TransportCertificateName { get; internal set; }

		public string TransportCertificateFqdn { get; internal set; }

		internal WildcardMatchType TransportWildcardMatchType { get; set; }

		public static ClientCertificateCriteria FromXml(XmlNode workContext, out bool validCertificate)
		{
			XmlElement xmlElement = workContext as XmlElement;
			ClientCertificateCriteria clientCertificateCriteria = null;
			validCertificate = false;
			if (xmlElement != null)
			{
				clientCertificateCriteria = new ClientCertificateCriteria();
				validCertificate = true;
				XmlNode xmlNode = workContext.SelectSingleNode("StoreLocation");
				if (xmlNode != null)
				{
					clientCertificateCriteria.StoreLocation = Utils.GetEnumValue<StoreLocation>(xmlNode.InnerText, "ClientCertificate StoreLocation");
				}
				xmlNode = workContext.SelectSingleNode("StoreName");
				if (xmlNode != null)
				{
					clientCertificateCriteria.StoreName = Utils.GetEnumValue<StoreName>(xmlNode.InnerText, "ClientCertificate StoreName");
				}
				xmlNode = workContext.SelectSingleNode("FindType");
				if (xmlNode != null)
				{
					clientCertificateCriteria.FindType = Utils.GetEnumValue<X509FindType>(xmlNode.InnerText, "ClientCertificate FindType");
				}
				xmlNode = workContext.SelectSingleNode("FindValue");
				if (xmlNode != null)
				{
					clientCertificateCriteria.FindValue = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "ClientCertificate FindValue");
				}
				xmlNode = workContext.SelectSingleNode("TransportCertificateName");
				if (xmlNode != null)
				{
					clientCertificateCriteria.TransportCertificateName = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "ClientCertificate TransportCertificateName");
				}
				xmlNode = workContext.SelectSingleNode("TransportCertificateFqdn");
				if (xmlNode != null)
				{
					clientCertificateCriteria.TransportCertificateFqdn = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "ClientCertificate TransportCertificateFqdn");
				}
				xmlNode = workContext.SelectSingleNode("TransportWildcardMatchType");
				if (xmlNode != null)
				{
					clientCertificateCriteria.TransportWildcardMatchType = Utils.GetEnumValue<WildcardMatchType>(xmlNode.InnerText, "ClientCertificate TransportWildcardMatchType");
				}
			}
			return clientCertificateCriteria;
		}
	}
}
