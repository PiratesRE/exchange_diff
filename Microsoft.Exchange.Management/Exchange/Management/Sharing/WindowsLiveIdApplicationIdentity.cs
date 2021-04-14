using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Sharing
{
	[Serializable]
	public sealed class WindowsLiveIdApplicationIdentity : ConfigurableObject
	{
		public WindowsLiveIdApplicationIdentity() : base(new SimpleProviderPropertyBag())
		{
		}

		public LiveIdInstanceType InstanceType
		{
			get
			{
				return (LiveIdInstanceType)this.propertyBag[WindowsLiveIdApplicationIdentitySchema.InstanceType];
			}
			set
			{
				this.propertyBag[WindowsLiveIdApplicationIdentitySchema.InstanceType] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this.propertyBag[WindowsLiveIdApplicationIdentitySchema.Name];
			}
			set
			{
				this.propertyBag[WindowsLiveIdApplicationIdentitySchema.Name] = value;
			}
		}

		public string Id
		{
			get
			{
				return this.propertyBag[WindowsLiveIdApplicationIdentitySchema.Id] as string;
			}
			set
			{
				this.propertyBag[WindowsLiveIdApplicationIdentitySchema.Id] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return this.propertyBag[SimpleProviderObjectSchema.Identity] as ObjectId;
			}
		}

		public string Status
		{
			get
			{
				return this.propertyBag[WindowsLiveIdApplicationIdentitySchema.Status] as string;
			}
			set
			{
				this.propertyBag[WindowsLiveIdApplicationIdentitySchema.Status] = value;
			}
		}

		public MultiValuedProperty<Uri> UriCollection
		{
			get
			{
				return this.propertyBag[WindowsLiveIdApplicationIdentitySchema.UriCollection] as MultiValuedProperty<Uri>;
			}
			set
			{
				this.propertyBag[WindowsLiveIdApplicationIdentitySchema.UriCollection] = value;
			}
		}

		public MultiValuedProperty<WindowsLiveIdApplicationCertificate> CertificateCollection
		{
			get
			{
				return this.propertyBag[WindowsLiveIdApplicationIdentitySchema.CertificateCollection] as MultiValuedProperty<WindowsLiveIdApplicationCertificate>;
			}
			set
			{
				this.propertyBag[WindowsLiveIdApplicationIdentitySchema.CertificateCollection] = value;
			}
		}

		public string RawXml
		{
			get
			{
				return this.propertyBag[WindowsLiveIdApplicationIdentitySchema.RawXml] as string;
			}
			set
			{
				this.propertyBag[WindowsLiveIdApplicationIdentitySchema.RawXml] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return WindowsLiveIdApplicationIdentity.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal static WindowsLiveIdApplicationIdentity ParseXml(LiveIdInstanceType liveIdInstanceType, XmlDocument xml)
		{
			WindowsLiveIdApplicationIdentity windowsLiveIdApplicationIdentity = new WindowsLiveIdApplicationIdentity();
			windowsLiveIdApplicationIdentity.InstanceType = liveIdInstanceType;
			XmlNode xmlNode = xml.SelectSingleNode("AppidData/Application/Name");
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				windowsLiveIdApplicationIdentity.Name = xmlNode.InnerText;
			}
			xmlNode = xml.SelectSingleNode("AppidData/Application/ID");
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				windowsLiveIdApplicationIdentity.propertyBag[SimpleProviderObjectSchema.Identity] = new WindowsLiveIdIdentity(xmlNode.InnerText);
				windowsLiveIdApplicationIdentity.Id = xmlNode.InnerText;
			}
			xmlNode = xml.SelectSingleNode(WindowsLiveIdApplicationIdentity.GetPropertyXPath("Status"));
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				windowsLiveIdApplicationIdentity.Status = xmlNode.InnerText;
			}
			using (XmlNodeList xmlNodeList = xml.SelectNodes("AppidData/Application/URIs/URI/URIAddress"))
			{
				if (xmlNodeList != null)
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode2 = (XmlNode)obj;
						if (!string.IsNullOrEmpty(xmlNode2.InnerText))
						{
							Uri item = null;
							if (Uri.TryCreate(xmlNode2.InnerText, UriKind.RelativeOrAbsolute, out item))
							{
								windowsLiveIdApplicationIdentity.UriCollection.Add(item);
							}
						}
					}
				}
			}
			using (XmlNodeList xmlNodeList2 = xml.SelectNodes("AppidData/Application/Certificates/Certificate"))
			{
				if (xmlNodeList2 != null)
				{
					foreach (object obj2 in xmlNodeList2)
					{
						XmlNode xmlNode3 = (XmlNode)obj2;
						string name = string.Empty;
						X509Certificate2 x509Certificate = null;
						bool isCurrent = false;
						XmlNode xmlNode4 = xmlNode3.SelectSingleNode("CertificateName");
						if (xmlNode4 != null)
						{
							name = xmlNode4.InnerText;
						}
						xmlNode4 = xmlNode3.SelectSingleNode("CertificateData");
						if (xmlNode4 != null && !string.IsNullOrEmpty(xmlNode4.InnerText))
						{
							x509Certificate = WindowsLiveIdApplicationCertificate.CertificateFromBase64(xmlNode4.InnerText);
						}
						xmlNode4 = xmlNode3.SelectSingleNode("CertificateIsCurrent");
						if (xmlNode4 != null && !string.IsNullOrEmpty(xmlNode4.InnerText) && !bool.TryParse(xmlNode4.InnerText, out isCurrent))
						{
							isCurrent = false;
						}
						if (x509Certificate != null)
						{
							windowsLiveIdApplicationIdentity.CertificateCollection.Add(new WindowsLiveIdApplicationCertificate(name, isCurrent, x509Certificate));
						}
					}
				}
			}
			xmlNode = xml.SelectSingleNode(WindowsLiveIdApplicationIdentity.GetPropertyXPath("Key"));
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
			{
				xmlNode.InnerText = "*****";
			}
			windowsLiveIdApplicationIdentity.RawXml = xml.DocumentElement.InnerXml;
			return windowsLiveIdApplicationIdentity;
		}

		private static string GetPropertyXPath(string propertyName)
		{
			return string.Format("AppidData/Application/Properties/{0}", propertyName);
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<WindowsLiveIdApplicationIdentitySchema>();
	}
}
