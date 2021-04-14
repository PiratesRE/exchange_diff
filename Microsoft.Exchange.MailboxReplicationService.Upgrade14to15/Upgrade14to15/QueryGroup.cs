using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "QueryGroup", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	public class QueryGroup : IExtensibleDataObject
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
