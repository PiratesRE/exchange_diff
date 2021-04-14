using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.com.IPC.WSService
{
	[DataContract(Name = "VersionData", Namespace = "http://microsoft.com/IPC/WSService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	public class VersionData : IExtensibleDataObject
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
		public string MinimumVersion
		{
			get
			{
				return this.MinimumVersionField;
			}
			set
			{
				this.MinimumVersionField = value;
			}
		}

		[DataMember(Order = 1)]
		public string MaximumVersion
		{
			get
			{
				return this.MaximumVersionField;
			}
			set
			{
				this.MaximumVersionField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string MinimumVersionField;

		private string MaximumVersionField;
	}
}
