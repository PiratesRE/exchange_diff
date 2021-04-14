using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "Domain", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class Domain : IExtensibleDataObject
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
		public DomainAuthenticationType? Authentication
		{
			get
			{
				return this.AuthenticationField;
			}
			set
			{
				this.AuthenticationField = value;
			}
		}

		[DataMember]
		public DomainCapabilities? Capabilities
		{
			get
			{
				return this.CapabilitiesField;
			}
			set
			{
				this.CapabilitiesField = value;
			}
		}

		[DataMember]
		public bool? IsDefault
		{
			get
			{
				return this.IsDefaultField;
			}
			set
			{
				this.IsDefaultField = value;
			}
		}

		[DataMember]
		public bool? IsInitial
		{
			get
			{
				return this.IsInitialField;
			}
			set
			{
				this.IsInitialField = value;
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.NameField;
			}
			set
			{
				this.NameField = value;
			}
		}

		[DataMember]
		public string RootDomain
		{
			get
			{
				return this.RootDomainField;
			}
			set
			{
				this.RootDomainField = value;
			}
		}

		[DataMember]
		public DomainStatus? Status
		{
			get
			{
				return this.StatusField;
			}
			set
			{
				this.StatusField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private DomainAuthenticationType? AuthenticationField;

		private DomainCapabilities? CapabilitiesField;

		private bool? IsDefaultField;

		private bool? IsInitialField;

		private string NameField;

		private string RootDomainField;

		private DomainStatus? StatusField;
	}
}
