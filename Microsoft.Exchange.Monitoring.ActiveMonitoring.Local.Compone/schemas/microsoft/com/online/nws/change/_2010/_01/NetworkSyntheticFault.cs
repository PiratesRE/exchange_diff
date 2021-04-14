using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.online.nws.change._2010._01
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "NetworkSyntheticFault", Namespace = "http://schemas.microsoft.com/online/nws/change/2010/01")]
	[DebuggerStepThrough]
	public class NetworkSyntheticFault : IExtensibleDataObject
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
		public string ComponentName
		{
			get
			{
				return this.ComponentNameField;
			}
			set
			{
				this.ComponentNameField = value;
			}
		}

		[DataMember]
		public string ErrorDescription
		{
			get
			{
				return this.ErrorDescriptionField;
			}
			set
			{
				this.ErrorDescriptionField = value;
			}
		}

		[DataMember]
		public string ServerName
		{
			get
			{
				return this.ServerNameField;
			}
			set
			{
				this.ServerNameField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string ComponentNameField;

		private string ErrorDescriptionField;

		private string ServerNameField;
	}
}
