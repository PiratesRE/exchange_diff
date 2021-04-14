using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "QueryConstraintResponse", Namespace = "http://tempuri.org/")]
	public class QueryConstraintResponse : IExtensibleDataObject
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
		public Constraint[] QueryConstraintResult
		{
			get
			{
				return this.QueryConstraintResultField;
			}
			set
			{
				this.QueryConstraintResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Constraint[] QueryConstraintResultField;
	}
}
