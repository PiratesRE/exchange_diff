using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "UpdateBlackout", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class UpdateBlackout : IExtensibleDataObject
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
		public string workloadName
		{
			get
			{
				return this.workloadNameField;
			}
			set
			{
				this.workloadNameField = value;
			}
		}

		[DataMember(Order = 1)]
		public GroupBlackout blackout
		{
			get
			{
				return this.blackoutField;
			}
			set
			{
				this.blackoutField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string workloadNameField;

		private GroupBlackout blackoutField;
	}
}
