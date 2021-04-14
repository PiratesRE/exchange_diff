using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.com.IPC.WSService
{
	[DataContract(Name = "ActiveFederationFault", Namespace = "http://microsoft.com/IPC/WSService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	public class ActiveFederationFault : IExtensibleDataObject
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
		public RmsErrors ErrorCode
		{
			get
			{
				return this.ErrorCodeField;
			}
			set
			{
				this.ErrorCodeField = value;
			}
		}

		[DataMember]
		public bool IsPermanentFailure
		{
			get
			{
				return this.IsPermanentFailureField;
			}
			set
			{
				this.IsPermanentFailureField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private RmsErrors ErrorCodeField;

		private bool IsPermanentFailureField;
	}
}
