using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.LiveServices;

namespace Microsoft.Exchange.Management.Sharing
{
	[Serializable]
	public sealed class WindowsLiveIdNamespace : ConfigurableObject
	{
		public WindowsLiveIdNamespace() : base(new SimpleProviderPropertyBag())
		{
		}

		public LiveIdInstanceType InstanceType
		{
			get
			{
				return (LiveIdInstanceType)this.propertyBag[WindowsLiveIdNamespaceSchema.InstanceType];
			}
			set
			{
				this.propertyBag[WindowsLiveIdNamespaceSchema.InstanceType] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this.propertyBag[WindowsLiveIdNamespaceSchema.Name];
			}
			set
			{
				this.propertyBag[WindowsLiveIdNamespaceSchema.Name] = value;
			}
		}

		public string ID
		{
			get
			{
				return this.propertyBag[WindowsLiveIdNamespaceSchema.ID] as string;
			}
			set
			{
				this.propertyBag[WindowsLiveIdNamespaceSchema.ID] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return this.propertyBag[SimpleProviderObjectSchema.Identity] as ObjectId;
			}
		}

		public string SiteID
		{
			get
			{
				return this.propertyBag[WindowsLiveIdNamespaceSchema.SiteID] as string;
			}
			set
			{
				this.propertyBag[WindowsLiveIdNamespaceSchema.SiteID] = value;
			}
		}

		public string AppID
		{
			get
			{
				return this.propertyBag[WindowsLiveIdNamespaceSchema.AppID] as string;
			}
			set
			{
				this.propertyBag[WindowsLiveIdNamespaceSchema.AppID] = value;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.propertyBag[WindowsLiveIdNamespaceSchema.URI] as Uri;
			}
			set
			{
				this.propertyBag[WindowsLiveIdNamespaceSchema.URI] = value;
			}
		}

		public X509Certificate2 Certificate
		{
			get
			{
				return this.propertyBag[WindowsLiveIdNamespaceSchema.Certificate] as X509Certificate2;
			}
			set
			{
				this.propertyBag[WindowsLiveIdNamespaceSchema.Certificate] = value;
			}
		}

		public X509Certificate2 NextCertificate
		{
			get
			{
				return this.propertyBag[WindowsLiveIdNamespaceSchema.NextCertificate] as X509Certificate2;
			}
			set
			{
				this.propertyBag[WindowsLiveIdNamespaceSchema.NextCertificate] = value;
			}
		}

		public string RawXml
		{
			get
			{
				return this.propertyBag[WindowsLiveIdNamespaceSchema.RawXml] as string;
			}
			set
			{
				this.propertyBag[WindowsLiveIdNamespaceSchema.RawXml] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return WindowsLiveIdNamespace.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal static WindowsLiveIdNamespace ParseXml(LiveIdInstanceType liveIdInstanceType, XmlDocument xml)
		{
			WindowsLiveIdNamespace windowsLiveIdNamespace = new WindowsLiveIdNamespace();
			windowsLiveIdNamespace.InstanceType = liveIdInstanceType;
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xml.NameTable);
			LiveServicesHelper.AddNamespaces(xml, xmlNamespaceManager);
			XmlNode xmlNode = xml.SelectSingleNode("p:Namespace/p:name", xmlNamespaceManager);
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				windowsLiveIdNamespace.Name = xmlNode.InnerText;
			}
			xmlNode = xml.SelectSingleNode("p:Namespace/p:ID", xmlNamespaceManager);
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				windowsLiveIdNamespace.propertyBag[SimpleProviderObjectSchema.Identity] = new WindowsLiveIdIdentity(xmlNode.InnerText);
				windowsLiveIdNamespace.ID = xmlNode.InnerText;
			}
			xmlNode = xml.SelectSingleNode(WindowsLiveIdNamespace.GetPropertyXPath("SiteID"), xmlNamespaceManager);
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				windowsLiveIdNamespace.SiteID = xmlNode.InnerText;
			}
			xmlNode = xml.SelectSingleNode(WindowsLiveIdNamespace.GetPropertyXPath("AppID"), xmlNamespaceManager);
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				windowsLiveIdNamespace.AppID = xmlNode.InnerText;
			}
			xmlNode = xml.SelectSingleNode(WindowsLiveIdNamespace.GetPropertyXPath("URI"), xmlNamespaceManager);
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				Uri uri = null;
				if (Uri.TryCreate(xmlNode.InnerText, UriKind.RelativeOrAbsolute, out uri))
				{
					windowsLiveIdNamespace.Uri = uri;
				}
			}
			xmlNode = xml.SelectSingleNode(WindowsLiveIdNamespace.GetPropertyXPath("Certificate"), xmlNamespaceManager);
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				windowsLiveIdNamespace.Certificate = WindowsLiveIdApplicationCertificate.CertificateFromBase64(xmlNode.InnerText);
			}
			xmlNode = xml.SelectSingleNode(WindowsLiveIdNamespace.GetPropertyXPath("NextCertificate"), xmlNamespaceManager);
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				windowsLiveIdNamespace.NextCertificate = WindowsLiveIdApplicationCertificate.CertificateFromBase64(xmlNode.InnerText);
			}
			windowsLiveIdNamespace.RawXml = xml.DocumentElement.InnerXml;
			return windowsLiveIdNamespace;
		}

		private static string GetPropertyXPath(string propertyName)
		{
			return string.Format("p:Namespace/p:property[@name=\"{0}\"]", propertyName);
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<WindowsLiveIdNamespaceSchema>();
	}
}
