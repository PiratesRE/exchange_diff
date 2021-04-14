using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class ThrottlePolicy : DirectoryObject
	{
		public DirectoryPropertyStringSingleLength1To256 DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public DirectoryPropertyXmlThrottleLimit ThrottleLimits
		{
			get
			{
				return this.throttleLimitsField;
			}
			set
			{
				this.throttleLimitsField = value;
			}
		}

		public DirectoryPropertyGuidSingle ThrottlePolicyId
		{
			get
			{
				return this.throttlePolicyIdField;
			}
			set
			{
				this.throttlePolicyIdField = value;
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

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyXmlThrottleLimit throttleLimitsField;

		private DirectoryPropertyGuidSingle throttlePolicyIdField;

		private XmlAttribute[] anyAttrField;
	}
}
