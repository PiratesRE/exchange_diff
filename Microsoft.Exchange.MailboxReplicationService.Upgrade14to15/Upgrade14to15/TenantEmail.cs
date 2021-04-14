using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "TenantEmail", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.ManagementService")]
	[DebuggerStepThrough]
	public class TenantEmail : IExtensibleDataObject
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
		public string Email
		{
			get
			{
				return this.EmailField;
			}
			set
			{
				this.EmailField = value;
			}
		}

		[DataMember]
		public EmailType EmailType
		{
			get
			{
				return this.EmailTypeField;
			}
			set
			{
				this.EmailTypeField = value;
			}
		}

		[DataMember]
		public Guid TenantEmailId
		{
			get
			{
				return this.TenantEmailIdField;
			}
			set
			{
				this.TenantEmailIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string EmailField;

		private EmailType EmailTypeField;

		private Guid TenantEmailIdField;
	}
}
