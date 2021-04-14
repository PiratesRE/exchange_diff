using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "QueryGroupResponse", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class QueryGroupResponse : IExtensibleDataObject
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
		public Group[] QueryGroupResult
		{
			get
			{
				return this.QueryGroupResultField;
			}
			set
			{
				this.QueryGroupResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Group[] QueryGroupResultField;
	}
}
