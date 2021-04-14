using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class Datacenter : DirectoryObject
	{
		public DirectoryPropertyXmlDatacenterRedirection DatacenterRedirections
		{
			get
			{
				return this.datacenterRedirectionsField;
			}
			set
			{
				this.datacenterRedirectionsField = value;
			}
		}

		public DirectoryPropertyXmlGeographicLocation GeographicLocation
		{
			get
			{
				return this.geographicLocationField;
			}
			set
			{
				this.geographicLocationField = value;
			}
		}

		public DirectoryPropertyXmlServiceEndpoint ServiceEndpoints
		{
			get
			{
				return this.serviceEndpointsField;
			}
			set
			{
				this.serviceEndpointsField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 SiteName
		{
			get
			{
				return this.siteNameField;
			}
			set
			{
				this.siteNameField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return this.anyAttrField;
			}
			set
			{
				this.anyAttrField = value;
			}
		}

		private DirectoryPropertyXmlDatacenterRedirection datacenterRedirectionsField;

		private DirectoryPropertyXmlGeographicLocation geographicLocationField;

		private DirectoryPropertyXmlServiceEndpoint serviceEndpointsField;

		private DirectoryPropertyStringSingleLength1To64 siteNameField;

		private XmlAttribute[] anyAttrField;
	}
}
