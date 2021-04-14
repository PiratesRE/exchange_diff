using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "AddWorkItemResponse", Namespace = "http://tempuri.org/")]
	public class AddWorkItemResponse : IExtensibleDataObject
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
		public string AddWorkItemResult
		{
			get
			{
				return this.AddWorkItemResultField;
			}
			set
			{
				this.AddWorkItemResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string AddWorkItemResultField;
	}
}
