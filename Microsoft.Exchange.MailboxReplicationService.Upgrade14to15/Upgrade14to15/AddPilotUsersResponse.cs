using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "AddPilotUsersResponse", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class AddPilotUsersResponse : IExtensibleDataObject
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
		public int AddPilotUsersResult
		{
			get
			{
				return this.AddPilotUsersResultField;
			}
			set
			{
				this.AddPilotUsersResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private int AddPilotUsersResultField;
	}
}
