using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MoveJobItemSubscriptionSettings : JobItemSubscriptionSettingsBase
	{
		public string TargetDatabase { get; private set; }

		public string TargetArchiveDatabase { get; private set; }

		public Unlimited<int>? BadItemLimit { get; private set; }

		public Unlimited<int>? LargeItemLimit { get; private set; }

		public bool? PrimaryOnly { get; private set; }

		public bool? ArchiveOnly { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return MoveJobItemSubscriptionSettings.MoveJobItemSubscriptionSettingsPropertyDefinitions;
			}
		}

		protected override bool IsEmpty
		{
			get
			{
				return base.IsEmpty && this.BadItemLimit == null && this.LargeItemLimit == null && this.TargetDatabase == null && this.TargetArchiveDatabase == null && this.PrimaryOnly == null && this.ArchiveOnly == null;
			}
		}

		public override JobItemSubscriptionSettingsBase Clone()
		{
			return new MoveJobItemSubscriptionSettings
			{
				TargetDatabase = this.TargetDatabase,
				TargetArchiveDatabase = this.TargetArchiveDatabase,
				BadItemLimit = this.BadItemLimit,
				LargeItemLimit = this.LargeItemLimit,
				PrimaryOnly = this.PrimaryOnly,
				ArchiveOnly = this.ArchiveOnly,
				LastModifiedTime = base.LastModifiedTime
			};
		}

		public override void UpdateFromDataRow(IMigrationDataRow request)
		{
			bool flag = false;
			MoveMigrationDataRow moveMigrationDataRow = request as MoveMigrationDataRow;
			if (moveMigrationDataRow == null)
			{
				throw new ArgumentException("expected a MoveMigrationDataRow", "request");
			}
			if (!object.Equals(this.TargetDatabase, moveMigrationDataRow.TargetDatabase))
			{
				this.TargetDatabase = moveMigrationDataRow.TargetDatabase;
				flag = true;
			}
			if (!object.Equals(this.TargetArchiveDatabase, moveMigrationDataRow.TargetArchiveDatabase))
			{
				this.TargetArchiveDatabase = moveMigrationDataRow.TargetArchiveDatabase;
				flag = true;
			}
			if (!object.Equals(this.BadItemLimit, moveMigrationDataRow.BadItemLimit))
			{
				this.BadItemLimit = moveMigrationDataRow.BadItemLimit;
				flag = true;
			}
			if (!object.Equals(this.LargeItemLimit, moveMigrationDataRow.LargeItemLimit))
			{
				this.LargeItemLimit = moveMigrationDataRow.LargeItemLimit;
				flag = true;
			}
			if (!object.Equals(this.PrimaryOnly, moveMigrationDataRow.PrimaryOnly) || !object.Equals(this.ArchiveOnly, moveMigrationDataRow.ArchiveOnly))
			{
				this.PrimaryOnly = moveMigrationDataRow.PrimaryOnly;
				this.ArchiveOnly = moveMigrationDataRow.ArchiveOnly;
				JobSubscriptionSettingsBase.ValidatePrimaryArchiveExclusivity(this.PrimaryOnly, this.ArchiveOnly);
				flag = true;
			}
			if (flag || base.LastModifiedTime == ExDateTime.MinValue)
			{
				base.LastModifiedTime = ExDateTime.UtcNow;
			}
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			MigrationHelper.WriteOrDeleteNullableProperty<string[]>(message, MigrationBatchMessageSchema.MigrationJobTargetDatabase, (this.TargetDatabase == null) ? null : new string[]
			{
				this.TargetDatabase
			});
			MigrationHelper.WriteOrDeleteNullableProperty<string[]>(message, MigrationBatchMessageSchema.MigrationJobTargetArchiveDatabase, (this.TargetArchiveDatabase == null) ? null : new string[]
			{
				this.TargetArchiveDatabase
			});
			MigrationHelper.WriteUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobBadItemLimit, this.BadItemLimit);
			MigrationHelper.WriteUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobLargeItemLimit, this.LargeItemLimit);
			MigrationHelper.WriteOrDeleteNullableProperty<bool?>(message, MigrationBatchMessageSchema.MigrationJobPrimaryOnly, this.PrimaryOnly);
			MigrationHelper.WriteOrDeleteNullableProperty<bool?>(message, MigrationBatchMessageSchema.MigrationJobArchiveOnly, this.ArchiveOnly);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.BadItemLimit = MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobBadItemLimit);
			this.LargeItemLimit = MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobLargeItemLimit);
			string[] valueOrDefault = message.GetValueOrDefault<string[]>(MigrationBatchMessageSchema.MigrationJobTargetDatabase, null);
			this.TargetDatabase = ((valueOrDefault == null || valueOrDefault.Length == 0) ? null : valueOrDefault[0]);
			string[] valueOrDefault2 = message.GetValueOrDefault<string[]>(MigrationBatchMessageSchema.MigrationJobTargetArchiveDatabase, null);
			this.TargetArchiveDatabase = ((valueOrDefault2 == null || valueOrDefault2.Length == 0) ? null : valueOrDefault2[0]);
			this.PrimaryOnly = message.GetValueOrDefault<bool?>(MigrationBatchMessageSchema.MigrationJobPrimaryOnly, null);
			this.ArchiveOnly = message.GetValueOrDefault<bool?>(MigrationBatchMessageSchema.MigrationJobArchiveOnly, null);
			JobSubscriptionSettingsBase.ValidatePrimaryArchiveExclusivity(this.PrimaryOnly, this.ArchiveOnly);
			return base.ReadFromMessageItem(message);
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			parent.Add(new object[]
			{
				new XElement("TargetDatabase", this.TargetDatabase),
				new XElement("TargetArchiveDatabase", this.TargetArchiveDatabase),
				new XElement("BadItemLimit", this.BadItemLimit),
				new XElement("LargeItemLimit", this.LargeItemLimit),
				new XElement("PrimaryOnly", this.PrimaryOnly),
				new XElement("ArchiveOnly", this.ArchiveOnly)
			});
		}

		public static readonly PropertyDefinition[] MoveJobItemSubscriptionSettingsPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobTargetDatabase,
				MigrationBatchMessageSchema.MigrationJobTargetArchiveDatabase,
				MigrationBatchMessageSchema.MigrationJobBadItemLimit,
				MigrationBatchMessageSchema.MigrationJobLargeItemLimit,
				MigrationBatchMessageSchema.MigrationJobPrimaryOnly,
				MigrationBatchMessageSchema.MigrationJobArchiveOnly,
				MigrationBatchMessageSchema.MigrationJobTargetDeliveryDomain
			},
			SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions
		});
	}
}
