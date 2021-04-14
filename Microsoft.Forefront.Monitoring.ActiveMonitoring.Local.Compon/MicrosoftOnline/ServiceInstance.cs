using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class ServiceInstance : DirectoryObject
	{
		public DirectoryPropertyStringLength1To1024 AuthorizedIdentity
		{
			get
			{
				return this.authorizedIdentityField;
			}
			set
			{
				this.authorizedIdentityField = value;
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

		public DirectoryPropertyBinarySingleLength1To4000 SecretEncryptionCertificate
		{
			get
			{
				return this.secretEncryptionCertificateField;
			}
			set
			{
				this.secretEncryptionCertificateField = value;
			}
		}

		public DirectoryPropertyXmlServiceInstanceInfo ServiceInstanceConfig
		{
			get
			{
				return this.serviceInstanceConfigField;
			}
			set
			{
				this.serviceInstanceConfigField = value;
			}
		}

		public DirectoryPropertyXmlServiceInstanceInfo ServiceInstanceInfo
		{
			get
			{
				return this.serviceInstanceInfoField;
			}
			set
			{
				this.serviceInstanceInfoField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 ServiceInstanceName
		{
			get
			{
				return this.serviceInstanceNameField;
			}
			set
			{
				this.serviceInstanceNameField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 ServiceInstanceOptions
		{
			get
			{
				return this.serviceInstanceOptionsField;
			}
			set
			{
				this.serviceInstanceOptionsField = value;
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

		private DirectoryPropertyStringLength1To1024 authorizedIdentityField;

		private DirectoryPropertyXmlGeographicLocation geographicLocationField;

		private DirectoryPropertyBinarySingleLength1To4000 secretEncryptionCertificateField;

		private DirectoryPropertyXmlServiceInstanceInfo serviceInstanceConfigField;

		private DirectoryPropertyXmlServiceInstanceInfo serviceInstanceInfoField;

		private DirectoryPropertyStringSingleLength1To256 serviceInstanceNameField;

		private DirectoryPropertyInt32SingleMin0 serviceInstanceOptionsField;

		private XmlAttribute[] anyAttrField;
	}
}
