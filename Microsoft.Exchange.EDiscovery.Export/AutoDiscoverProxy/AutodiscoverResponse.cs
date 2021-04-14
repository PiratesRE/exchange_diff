using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[XmlInclude(typeof(DomainResponse))]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[XmlInclude(typeof(GetDomainSettingsResponse))]
	[XmlInclude(typeof(UserResponse))]
	[XmlInclude(typeof(GetUserSettingsResponse))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(GetOrganizationRelationshipSettingsResponse))]
	[XmlInclude(typeof(GetFederationInformationResponse))]
	[DebuggerStepThrough]
	[Serializable]
	public class AutodiscoverResponse
	{
		public ErrorCode ErrorCode
		{
			get
			{
				return this.errorCodeField;
			}
			set
			{
				this.errorCodeField = value;
			}
		}

		[XmlIgnore]
		public bool ErrorCodeSpecified
		{
			get
			{
				return this.errorCodeFieldSpecified;
			}
			set
			{
				this.errorCodeFieldSpecified = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string ErrorMessage
		{
			get
			{
				return this.errorMessageField;
			}
			set
			{
				this.errorMessageField = value;
			}
		}

		private ErrorCode errorCodeField;

		private bool errorCodeFieldSpecified;

		private string errorMessageField;
	}
}
