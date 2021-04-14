using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "PickWorkItems", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class PickWorkItems : IExtensibleDataObject
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
		public int pageSize
		{
			get
			{
				return this.pageSizeField;
			}
			set
			{
				this.pageSizeField = value;
			}
		}

		[DataMember]
		public TimeSpan visibilityTimeout
		{
			get
			{
				return this.visibilityTimeoutField;
			}
			set
			{
				this.visibilityTimeoutField = value;
			}
		}

		[DataMember(Order = 2)]
		public byte[] bookmark
		{
			get
			{
				return this.bookmarkField;
			}
			set
			{
				this.bookmarkField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private int pageSizeField;

		private TimeSpan visibilityTimeoutField;

		private byte[] bookmarkField;
	}
}
