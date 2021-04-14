using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class SliceInstance : DirectoryObject
	{
		public DirectoryPropertyStringSingleLength1To20 BuildNumber
		{
			get
			{
				return this.buildNumberField;
			}
			set
			{
				this.buildNumberField = value;
			}
		}

		public DirectoryPropertyInt32Single SliceId
		{
			get
			{
				return this.sliceIdField;
			}
			set
			{
				this.sliceIdField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 SliceStatus
		{
			get
			{
				return this.sliceStatusField;
			}
			set
			{
				this.sliceStatusField = value;
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

		public DirectoryPropertyBinarySingleLength1To256 TenantRange
		{
			get
			{
				return this.tenantRangeField;
			}
			set
			{
				this.tenantRangeField = value;
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

		private DirectoryPropertyStringSingleLength1To20 buildNumberField;

		private DirectoryPropertyInt32Single sliceIdField;

		private DirectoryPropertyInt32SingleMin0 sliceStatusField;

		private DirectoryPropertyXmlServiceEndpoint serviceEndpointsField;

		private DirectoryPropertyBinarySingleLength1To256 tenantRangeField;

		private XmlAttribute[] anyAttrField;
	}
}
