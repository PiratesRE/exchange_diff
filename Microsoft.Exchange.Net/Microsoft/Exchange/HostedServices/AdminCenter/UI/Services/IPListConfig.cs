using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "IPListConfig", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class IPListConfig : IExtensibleDataObject
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
		internal string[] IPList
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
		internal TargetAction TargetAction
		{
			get
			{
				return this.TargetActionField;
			}
			set
			{
				this.TargetActionField = value;
			}
		}

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private string[] IPListField;

		[OptionalField]
		private TargetAction TargetActionField;
	}
}
