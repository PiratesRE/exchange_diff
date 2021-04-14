using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "QueryCapacity", Namespace = "http://tempuri.org/")]
	public class QueryCapacity : IExtensibleDataObject
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
		public string[] groupNames
		{
			get
			{
				return this.groupNamesField;
			}
			set
			{
				this.groupNamesField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] groupNamesField;
	}
}
