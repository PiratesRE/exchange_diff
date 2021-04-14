using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "AccountSkuDetails", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class AccountSkuDetails : IExtensibleDataObject
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
		public string AccountName
		{
			get
			{
				return this.AccountNameField;
			}
			set
			{
				this.AccountNameField = value;
			}
		}

		[DataMember]
		public Guid AccountObjectId
		{
			get
			{
				return this.AccountObjectIdField;
			}
			set
			{
				this.AccountObjectIdField = value;
			}
		}

		[DataMember]
		public string AccountSkuId
		{
			get
			{
				return this.AccountSkuIdField;
			}
			set
			{
				this.AccountSkuIdField = value;
			}
		}

		[DataMember]
		public int ActiveUnits
		{
			get
			{
				return this.ActiveUnitsField;
			}
			set
			{
				this.ActiveUnitsField = value;
			}
		}

		[DataMember]
		public int ConsumedUnits
		{
			get
			{
				return this.ConsumedUnitsField;
			}
			set
			{
				this.ConsumedUnitsField = value;
			}
		}

		[DataMember]
		public ServiceStatus[] ServiceStatus
		{
			get
			{
				return this.ServiceStatusField;
			}
			set
			{
				this.ServiceStatusField = value;
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

		[DataMember]
		public string SkuPartNumber
		{
			get
			{
				return this.SkuPartNumberField;
			}
			set
			{
				this.SkuPartNumberField = value;
			}
		}

		[DataMember]
		public Guid[] SubscriptionIds
		{
			get
			{
				return this.SubscriptionIdsField;
			}
			set
			{
				this.SubscriptionIdsField = value;
			}
		}

		[DataMember]
		public int SuspendedUnits
		{
			get
			{
				return this.SuspendedUnitsField;
			}
			set
			{
				this.SuspendedUnitsField = value;
			}
		}

		[DataMember]
		public SkuTargetClass TargetClass
		{
			get
			{
				return this.TargetClassField;
			}
			set
			{
				this.TargetClassField = value;
			}
		}

		[DataMember]
		public int WarningUnits
		{
			get
			{
				return this.WarningUnitsField;
			}
			set
			{
				this.WarningUnitsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string AccountNameField;

		private Guid AccountObjectIdField;

		private string AccountSkuIdField;

		private int ActiveUnitsField;

		private int ConsumedUnitsField;

		private ServiceStatus[] ServiceStatusField;

		private Guid SkuIdField;

		private string SkuPartNumberField;

		private Guid[] SubscriptionIdsField;

		private int SuspendedUnitsField;

		private SkuTargetClass TargetClassField;

		private int WarningUnitsField;
	}
}
