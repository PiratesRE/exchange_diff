using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "PilotUserInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.WorkloadService")]
	[DebuggerStepThrough]
	public class PilotUserInfo : IExtensibleDataObject
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
		public Guid PilotUserId
		{
			get
			{
				return this.PilotUserIdField;
			}
			set
			{
				this.PilotUserIdField = value;
			}
		}

		[DataMember]
		public string Upn
		{
			get
			{
				return this.UpnField;
			}
			set
			{
				this.UpnField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid PilotUserIdField;

		private string UpnField;
	}
}
