using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DebuggerStepThrough]
	[KnownType(typeof(CompanyResponseInfo))]
	[DataContract(Name = "ResponseInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[KnownType(typeof(DomainResponseInfo))]
	[Serializable]
	internal class ResponseInfo : IExtensibleDataObject
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
		internal ServiceFault Fault
		{
			get
			{
				return this.FaultField;
			}
			set
			{
				this.FaultField = value;
			}
		}

		[DataMember]
		internal ResponseStatus Status
		{
			get
			{
				return this.StatusField;
			}
			set
			{
				this.StatusField = value;
			}
		}

		[DataMember]
		internal TargetObject Target
		{
			get
			{
				return this.TargetField;
			}
			set
			{
				this.TargetField = value;
			}
		}

		[DataMember]
		internal string[] TargetValue
		{
			get
			{
				return this.TargetValueField;
			}
			set
			{
				this.TargetValueField = value;
			}
		}

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private ServiceFault FaultField;

		[OptionalField]
		private ResponseStatus StatusField;

		[OptionalField]
		private TargetObject TargetField;

		[OptionalField]
		private string[] TargetValueField;
	}
}
