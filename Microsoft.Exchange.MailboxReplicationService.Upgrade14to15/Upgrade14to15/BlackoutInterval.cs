using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "BlackoutInterval", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.Common.DataContract")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class BlackoutInterval : IExtensibleDataObject
	{
		public BlackoutInterval(DateTime startDate, DateTime endDate, string reason)
		{
			this.StartDate = startDate;
			this.EndDate = endDate;
			this.Reason = reason;
		}

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
		public DateTime EndDate
		{
			get
			{
				return this.EndDateField;
			}
			set
			{
				this.EndDateField = value;
			}
		}

		[DataMember]
		public string Reason
		{
			get
			{
				return this.ReasonField;
			}
			set
			{
				this.ReasonField = value;
			}
		}

		[DataMember]
		public DateTime StartDate
		{
			get
			{
				return this.StartDateField;
			}
			set
			{
				this.StartDateField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private DateTime EndDateField;

		private string ReasonField;

		private DateTime StartDateField;
	}
}
