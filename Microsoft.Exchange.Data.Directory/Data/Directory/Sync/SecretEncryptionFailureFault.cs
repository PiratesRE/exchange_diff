using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DataContract(Name = "SecretEncryptionFailureFault", Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class SecretEncryptionFailureFault : IExtensibleDataObject
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

		[DataMember(EmitDefaultValue = false)]
		public string Location
		{
			get
			{
				return this.LocationField;
			}
			set
			{
				this.LocationField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string LocationField;
	}
}
