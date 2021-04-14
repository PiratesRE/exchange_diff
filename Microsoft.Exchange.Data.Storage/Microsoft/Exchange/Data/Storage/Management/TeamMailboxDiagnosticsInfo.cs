using System;
using System.Text;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class TeamMailboxDiagnosticsInfo : ConfigurableObject
	{
		public TeamMailboxDiagnosticsInfo(string displayName) : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			if (string.IsNullOrEmpty(displayName))
			{
				throw new ArgumentNullException("displayName");
			}
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = new SyncInfoId();
			this.propertyBag.ResetChangeTracking();
			this.DisplayName = displayName;
		}

		public SyncInfo HierarchySyncInfo
		{
			get
			{
				return (SyncInfo)this[TeamMailboxDiagnosticsInfoSchema.DocLibSyncInfo];
			}
			set
			{
				this[TeamMailboxDiagnosticsInfoSchema.DocLibSyncInfo] = value;
			}
		}

		public SyncInfo MembershipSyncInfo
		{
			get
			{
				return (SyncInfo)this[TeamMailboxDiagnosticsInfoSchema.MembershipSyncInfo];
			}
			set
			{
				this[TeamMailboxDiagnosticsInfoSchema.MembershipSyncInfo] = value;
			}
		}

		public SyncInfo MaintenanceSyncInfo
		{
			get
			{
				return (SyncInfo)this[TeamMailboxDiagnosticsInfoSchema.MaintenanceSyncInfo];
			}
			set
			{
				this[TeamMailboxDiagnosticsInfoSchema.MaintenanceSyncInfo] = value;
			}
		}

		public MultiValuedProperty<SyncInfo> DocLibSyncInfos { get; set; }

		public string DisplayName
		{
			get
			{
				return (string)this[TeamMailboxDiagnosticsInfoSchema.DisplayName];
			}
			private set
			{
				this[TeamMailboxDiagnosticsInfoSchema.DisplayName] = value;
			}
		}

		public TeamMailboxSyncStatus Status
		{
			get
			{
				return (TeamMailboxSyncStatus)this[TeamMailboxDiagnosticsInfoSchema.Status];
			}
			internal set
			{
				this[TeamMailboxDiagnosticsInfoSchema.Status] = value;
			}
		}

		public string LastDocumentSyncCycleLog
		{
			get
			{
				return (string)this[TeamMailboxDiagnosticsInfoSchema.LastDocumentSyncCycleLog];
			}
			internal set
			{
				this[TeamMailboxDiagnosticsInfoSchema.LastDocumentSyncCycleLog] = value;
			}
		}

		public string LastMembershipSyncCycleLog
		{
			get
			{
				return (string)this[TeamMailboxDiagnosticsInfoSchema.LastMembershipSyncCycleLog];
			}
			internal set
			{
				this[TeamMailboxDiagnosticsInfoSchema.LastMembershipSyncCycleLog] = value;
			}
		}

		public string LastMaintenanceSyncCycleLog
		{
			get
			{
				return (string)this[TeamMailboxDiagnosticsInfoSchema.LastMaintenanceSyncCycleLog];
			}
			internal set
			{
				this[TeamMailboxDiagnosticsInfoSchema.LastMaintenanceSyncCycleLog] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TeamMailboxDiagnosticsInfo.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("==================== Diagnostic Information For Site Mailbox: \"{0}\" ====================", this.DisplayName);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("1. Hierarchy Synchronization Information:");
			stringBuilder.AppendLine();
			TeamMailboxDiagnosticsInfo.AppendSyncInfo(stringBuilder, this.HierarchySyncInfo);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("2. Document Library Synchronization Information:");
			stringBuilder.AppendLine();
			foreach (SyncInfo info in this.DocLibSyncInfos)
			{
				TeamMailboxDiagnosticsInfo.AppendSyncInfo(stringBuilder, info);
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("3. Membership Synchronization Information:");
			stringBuilder.AppendLine();
			TeamMailboxDiagnosticsInfo.AppendSyncInfo(stringBuilder, this.MembershipSyncInfo);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("4. Maintenance Synchronization Information:");
			stringBuilder.AppendLine();
			TeamMailboxDiagnosticsInfo.AppendSyncInfo(stringBuilder, this.MaintenanceSyncInfo);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("5. Document Synchronization Log:");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(this.LastDocumentSyncCycleLog ?? "N/A");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("6. Membership Synchronization Log:");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(this.LastMembershipSyncCycleLog ?? "N/A");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("7. Maintenance Synchronization Log:");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(this.LastMaintenanceSyncCycleLog ?? "N/A");
			return stringBuilder.ToString();
		}

		private static void AppendSyncInfo(StringBuilder result, SyncInfo info)
		{
			if (info != null)
			{
				result.AppendFormat("DisplayName:{0}", info.DisplayName);
				result.AppendLine();
				result.AppendFormat("Url:{0}", info.Url);
				result.AppendLine();
				result.AppendFormat("LastFailedSyncTime:{0}", info.LastFailedSyncTime);
				result.AppendLine();
				result.AppendFormat("LastSyncFailure:{0}", info.LastSyncFailure);
				result.AppendLine();
				result.AppendFormat("FirstAttemptedSyncTime:{0}", info.FirstAttemptedSyncTime);
				result.AppendLine();
				result.AppendFormat("LastAttemptedSyncTime:{0}", info.LastAttemptedSyncTime);
				result.AppendLine();
				result.AppendFormat("LastSuccessfulSyncTime:{0}", info.LastSuccessfulSyncTime);
				result.AppendLine();
				result.AppendFormat("LastFailedSyncEmailTime:{0}", info.LastFailedSyncEmailTime);
				result.AppendLine();
				return;
			}
			result.AppendLine("N/A");
			result.AppendLine();
		}

		private static readonly TeamMailboxDiagnosticsInfoSchema schema = ObjectSchema.GetInstance<TeamMailboxDiagnosticsInfoSchema>();
	}
}
