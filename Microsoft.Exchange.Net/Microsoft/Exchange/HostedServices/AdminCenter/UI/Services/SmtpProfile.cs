using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DebuggerStepThrough]
	[DataContract(Name = "SmtpProfile", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class SmtpProfile : IExtensibleDataObject
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
		internal SmtpProfileEntry[] IPList
		{
			get
			{
				return this.IPListField;
			}
			set
			{
				this.IPListField = value;
			}
		}

		[DataMember]
		internal string ProfileName
		{
			get
			{
				return this.ProfileNameField;
			}
			set
			{
				this.ProfileNameField = value;
			}
		}

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private SmtpProfileEntry[] IPListField;

		[OptionalField]
		private string ProfileNameField;
	}
}
