using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DomainFederationSettings", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class DomainFederationSettings : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public string ActiveLogOnUri
		{
			get
			{
				return this.ActiveLogOnUriField;
			}
			set
			{
				this.ActiveLogOnUriField = value;
			}
		}

		[DataMember]
		public string FederationBrandName
		{
			get
			{
				return this.FederationBrandNameField;
			}
			set
			{
				this.FederationBrandNameField = value;
			}
		}

		[DataMember]
		public string IssuerUri
		{
			get
			{
				return this.IssuerUriField;
			}
			set
			{
				this.IssuerUriField = value;
			}
		}

		[DataMember]
		public string LogOffUri
		{
			get
			{
				return this.LogOffUriField;
			}
			set
			{
				this.LogOffUriField = value;
			}
		}

		[DataMember]
		public string MetadataExchangeUri
		{
			get
			{
				return this.MetadataExchangeUriField;
			}
			set
			{
				this.MetadataExchangeUriField = value;
			}
		}

		[DataMember]
		public string NextSigningCertificate
		{
			get
			{
				return this.NextSigningCertificateField;
			}
			set
			{
				this.NextSigningCertificateField = value;
			}
		}

		[DataMember]
		public string PassiveLogOnUri
		{
			get
			{
				return this.PassiveLogOnUriField;
			}
			set
			{
				this.PassiveLogOnUriField = value;
			}
		}

		[DataMember]
		public AuthenticationProtocol? PreferredAuthenticationProtocol
		{
			get
			{
				return this.PreferredAuthenticationProtocolField;
			}
			set
			{
				this.PreferredAuthenticationProtocolField = value;
			}
		}

		[DataMember]
		public string SigningCertificate
		{
			get
			{
				return this.SigningCertificateField;
			}
			set
			{
				this.SigningCertificateField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string ActiveLogOnUriField;

		private string FederationBrandNameField;

		private string IssuerUriField;

		private string LogOffUriField;

		private string MetadataExchangeUriField;

		private string NextSigningCertificateField;

		private string PassiveLogOnUriField;

		private AuthenticationProtocol? PreferredAuthenticationProtocolField;

		private string SigningCertificateField;
	}
}
