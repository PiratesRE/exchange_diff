using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DomainSearchFilter", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	public class DomainSearchFilter : IExtensibleDataObject
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
		public DomainCapabilities? Capability
		{
			get
			{
				return this.CapabilityField;
			}
			set
			{
				this.CapabilityField = value;
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

		private DomainCapabilities? CapabilityField;

		private DomainStatus? StatusField;
	}
}
