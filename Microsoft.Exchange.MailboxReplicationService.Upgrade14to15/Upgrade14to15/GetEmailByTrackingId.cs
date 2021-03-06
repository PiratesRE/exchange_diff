using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "GetEmailByTrackingId", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetEmailByTrackingId : IExtensibleDataObject
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
		public Guid trackingId
		{
			get
			{
				return this.trackingIdField;
			}
			set
			{
				this.trackingIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid trackingIdField;
	}
}
