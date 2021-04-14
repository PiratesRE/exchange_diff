using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class UserIdType
	{
		public string SID
		{
			get
			{
				return this.sIDField;
			}
			set
			{
				this.sIDField = value;
			}
		}

		public string PrimarySmtpAddress
		{
			get
			{
				return this.primarySmtpAddressField;
			}
			set
			{
				this.primarySmtpAddressField = value;
			}
		}

		public string DisplayName
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

		public DistinguishedUserType DistinguishedUser
		{
			get
			{
				return this.distinguishedUserField;
			}
			set
			{
				this.distinguishedUserField = value;
			}
		}

		[XmlIgnore]
		public bool DistinguishedUserSpecified
		{
			get
			{
				return this.distinguishedUserFieldSpecified;
			}
			set
			{
				this.distinguishedUserFieldSpecified = value;
			}
		}

		public string ExternalUserIdentity
		{
			get
			{
				return this.externalUserIdentityField;
			}
			set
			{
				this.externalUserIdentityField = value;
			}
		}

		private string sIDField;

		private string primarySmtpAddressField;

		private string displayNameField;

		private DistinguishedUserType distinguishedUserField;

		private bool distinguishedUserFieldSpecified;

		private string externalUserIdentityField;
	}
}
