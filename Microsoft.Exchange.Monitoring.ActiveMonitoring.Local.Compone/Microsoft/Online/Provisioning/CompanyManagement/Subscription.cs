using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Provisioning.CompanyManagement
{
	[DebuggerStepThrough]
	[DataContract(Name = "Subscription", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Provisioning.CompanyManagement")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class Subscription : IExtensibleDataObject
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

		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		public Guid AccountId
		{
			get
			{
				return this.AccountIdField;
			}
			set
			{
				this.AccountIdField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public int AllowedOverageUnits
		{
			get
			{
				return this.AllowedOverageUnitsField;
			}
			set
			{
				this.AllowedOverageUnitsField = value;
			}
		}

		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		public Guid ContextId
		{
			get
			{
				return this.ContextIdField;
			}
			set
			{
				this.ContextIdField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public DateTime LifecycleNextStateChangeDate
		{
			get
			{
				return this.LifecycleNextStateChangeDateField;
			}
			set
			{
				this.LifecycleNextStateChangeDateField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public SubscriptionState LifecycleState
		{
			get
			{
				return this.LifecycleStateField;
			}
			set
			{
				this.LifecycleStateField = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string OfferType
		{
			get
			{
				return this.OfferTypeField;
			}
			set
			{
				this.OfferTypeField = value;
			}
		}

		[DataMember]
		public string PartNumber
		{
			get
			{
				return this.PartNumberField;
			}
			set
			{
				this.PartNumberField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public int PrepaidUnits
		{
			get
			{
				return this.PrepaidUnitsField;
			}
			set
			{
				this.PrepaidUnitsField = value;
			}
		}

		[DataMember]
		public Guid SkuId
		{
			get
			{
				return this.SkuIdField;
			}
			set
			{
				this.SkuIdField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public DateTime StartDate
		{
			get
			{
				return this.StartDateField;
			}
			set
			{
				this.StartDateField = value;
			}
		}

		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		public Guid SubscriptionId
		{
			get
			{
				return this.SubscriptionIdField;
			}
			set
			{
				this.SubscriptionIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid AccountIdField;

		private int AllowedOverageUnitsField;

		private Guid ContextIdField;

		private DateTime LifecycleNextStateChangeDateField;

		private SubscriptionState LifecycleStateField;

		private string OfferTypeField;

		private string PartNumberField;

		private int PrepaidUnitsField;

		private Guid SkuIdField;

		private DateTime StartDateField;

		private Guid SubscriptionIdField;
	}
}
