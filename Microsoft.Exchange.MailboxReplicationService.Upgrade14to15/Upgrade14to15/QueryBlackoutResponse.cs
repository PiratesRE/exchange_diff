using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "QueryBlackoutResponse", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class QueryBlackoutResponse : IExtensibleDataObject
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
		public GroupBlackout[] QueryBlackoutResult
		{
			get
			{
				return this.QueryBlackoutResultField;
			}
			set
			{
				this.QueryBlackoutResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private GroupBlackout[] QueryBlackoutResultField;
	}
}
