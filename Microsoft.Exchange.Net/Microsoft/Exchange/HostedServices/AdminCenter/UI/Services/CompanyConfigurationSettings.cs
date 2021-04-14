using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DebuggerStepThrough]
	[DataContract(Name = "CompanyConfigurationSettings", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class CompanyConfigurationSettings : ConfigurationSettings
	{
		[DataMember]
		internal Guid? CompanyGuid
		{
			get
			{
				return this.CompanyGuidField;
			}
			set
			{
				this.CompanyGuidField = value;
			}
		}

		[DataMember]
		internal InboundIPListConfig InboundIPList
		{
			get
			{
				return this.InboundIPListField;
			}
			set
			{
				this.InboundIPListField = value;
			}
		}

		[DataMember]
		internal InheritanceSettings InheritFromParent
		{
			get
			{
				return this.InheritFromParentField;
			}
			set
			{
				this.InheritFromParentField = value;
			}
		}

		[DataMember]
		internal Guid? MicrosoftOnlineId
		{
			get
			{
				return this.MicrosoftOnlineIdField;
			}
			set
			{
				this.MicrosoftOnlineIdField = value;
			}
		}

		[DataMember]
		internal PropagationSettings PropagationSetting
		{
			get
			{
				return this.PropagationSettingField;
			}
			set
			{
				this.PropagationSettingField = value;
			}
		}

		[DataMember]
		internal int SeatCount
		{
			get
			{
				return this.SeatCountField;
			}
			set
			{
				this.SeatCountField = value;
			}
		}

		[DataMember]
		internal ServicePlan ServicePlanId
		{
			get
			{
				return this.ServicePlanIdField;
			}
			set
			{
				this.ServicePlanIdField = value;
			}
		}

		[OptionalField]
		private Guid? CompanyGuidField;

		[OptionalField]
		private InboundIPListConfig InboundIPListField;

		[OptionalField]
		private InheritanceSettings InheritFromParentField;

		[OptionalField]
		private Guid? MicrosoftOnlineIdField;

		[OptionalField]
		private PropagationSettings PropagationSettingField;

		[OptionalField]
		private int SeatCountField;

		[OptionalField]
		private ServicePlan ServicePlanIdField;
	}
}
