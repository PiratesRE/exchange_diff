using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "GetEmailByTrackingIdResponse", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetEmailByTrackingIdResponse : IExtensibleDataObject
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
		public EmailDefinition GetEmailByTrackingIdResult
		{
			get
			{
				return this.GetEmailByTrackingIdResultField;
			}
			set
			{
				this.GetEmailByTrackingIdResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private EmailDefinition GetEmailByTrackingIdResultField;
	}
}
