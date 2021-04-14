using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "GetPilotUsersResponse", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetPilotUsersResponse : IExtensibleDataObject
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
		public UserWorkloadStatusInfo[] GetPilotUsersResult
		{
			get
			{
				return this.GetPilotUsersResultField;
			}
			set
			{
				this.GetPilotUsersResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private UserWorkloadStatusInfo[] GetPilotUsersResultField;
	}
}
