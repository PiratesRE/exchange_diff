using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "LocatorServiceFault", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class LocatorServiceFault : IExtensibleDataObject
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
		public bool CanRetry
		{
			get
			{
				return this.CanRetryField;
			}
			set
			{
				this.CanRetryField = value;
			}
		}

		[DataMember]
		public int IntErrorCode
		{
			get
			{
				return this.IntErrorCodeField;
			}
			set
			{
				this.IntErrorCodeField = value;
			}
		}

		public ErrorCode ErrorCode
		{
			get
			{
				return (ErrorCode)this.IntErrorCode;
			}
		}

		[DataMember]
		public string Message
		{
			get
			{
				return this.MessageField;
			}
			set
			{
				this.MessageField = value;
			}
		}

		[DataMember]
		public string[] MissingData
		{
			get
			{
				return this.MissingDataField;
			}
			set
			{
				this.MissingDataField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private bool CanRetryField;

		private int IntErrorCodeField;

		private string MessageField;

		private string[] MissingDataField;
	}
}
