using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "QueryCapacityResponse", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	public class QueryCapacityResponse : IExtensibleDataObject
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
		public GroupCapacity[] QueryCapacityResult
		{
			get
			{
				return this.QueryCapacityResultField;
			}
			set
			{
				this.QueryCapacityResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private GroupCapacity[] QueryCapacityResultField;
	}
}
