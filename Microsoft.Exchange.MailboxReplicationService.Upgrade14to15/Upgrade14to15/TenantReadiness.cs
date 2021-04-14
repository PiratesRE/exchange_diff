using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "TenantReadiness", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.Common.DataContract")]
	public class TenantReadiness : IExtensibleDataObject
	{
		public TenantReadiness(string[] constraints, string groupName, bool isReady, Guid tenantId, int upgradeUnits, bool useDefaultCapacity)
		{
			this.ConstraintsField = constraints;
			this.GroupNameField = groupName;
			this.IsReadyField = isReady;
			this.TenantIdField = tenantId;
			this.UpgradeUnitsField = upgradeUnits;
			this.UseDefaultCapacityField = useDefaultCapacity;
		}

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
		public string[] Constraints
		{
			get
			{
				return this.ConstraintsField;
			}
			set
			{
				this.ConstraintsField = value;
			}
		}

		[DataMember]
		public string GroupName
		{
			get
			{
				return this.GroupNameField;
			}
			set
			{
				this.GroupNameField = value;
			}
		}

		[DataMember]
		public bool IsReady
		{
			get
			{
				return this.IsReadyField;
			}
			set
			{
				this.IsReadyField = value;
			}
		}

		[DataMember]
		public Guid TenantId
		{
			get
			{
				return this.TenantIdField;
			}
			set
			{
				this.TenantIdField = value;
			}
		}

		[DataMember]
		public int UpgradeUnits
		{
			get
			{
				return this.UpgradeUnitsField;
			}
			set
			{
				this.UpgradeUnitsField = value;
			}
		}

		[DataMember]
		public bool UseDefaultCapacity
		{
			get
			{
				return this.UseDefaultCapacityField;
			}
			set
			{
				this.UseDefaultCapacityField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] ConstraintsField;

		private string GroupNameField;

		private bool IsReadyField;

		private Guid TenantIdField;

		private int UpgradeUnitsField;

		private bool UseDefaultCapacityField;
	}
}
