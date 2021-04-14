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
	internal class MoveJobSubscriptionSettings : JobSubscriptionSettingsBase
	{
		public MoveJobSubscriptionSettings(bool isLocalMove)
		{
			this.isLocalMove = isLocalMove;
		}

		public string[] TargetDatabases { get; private set; }

		public string[] TargetArchiveDatabases { get; private set; }

		public string TargetDeliveryDomain { get; private set; }

		public Unlimited<int>? BadItemLimit { get; private set; }

		public Unlimited<int>? LargeItemLimit { get; private set; }

		public bool? PrimaryOnly { get; private set; }

		public bool? ArchiveOnly { get; private set; }

		public ExDateTime? StartAfter { get; private set; }

		public ExDateTime? CompleteAfter { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return MoveJobSubscriptionSettings.MoveJobSubscriptionSettingsPropertyDefinitions;
			}
		}

		protected override bool IsEmpty
		{
			get
			{
				return base.IsEmpty && this.BadItemLimit == null && this.LargeItemLimit == null && this.TargetDatabases == null && this.TargetArchiveDatabases == null && this.PrimaryOnly == null && this.ArchiveOnly == null;
			}
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			MigrationHelper.WriteOrDeleteNullableProperty<string[]>(message, MigrationBatchMessageSchema.MigrationJobTargetDatabase, this.TargetDatabases);
			MigrationHelper.WriteOrDeleteNullableProperty<string[]>(message, MigrationBatchMessageSchema.MigrationJobTargetArchiveDatabase, this.TargetArchiveDatabases);
			MigrationHelper.WriteOrDeleteNullableProperty<string>(message, MigrationBatchMessageSchema.MigrationJobTargetDeliveryDomain, this.TargetDeliveryDomain);
			MigrationHelper.WriteUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobBadItemLimit, this.BadItemLimit);
			MigrationHelper.WriteUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobLargeItemLimit, this.LargeItemLimit);
			MigrationHelper.WriteOrDeleteNullableProperty<bool?>(message, MigrationBatchMessageSchema.MigrationJobPrimaryOnly, this.PrimaryOnly);
			MigrationHelper.WriteOrDeleteNullableProperty<bool?>(message, MigrationBatchMessageSchema.MigrationJobArchiveOnly, this.ArchiveOnly);
			MigrationHelper.WriteOrDeleteNullableProperty<ExDateTime?>(message, MigrationBatchMessageSchema.MigrationJobStartAfter, this.StartAfter);
			MigrationHelper.WriteOrDeleteNullableProperty<ExDateTime?>(message, MigrationBatchMessageSchema.MigrationJobCompleteAfter, this.CompleteAfter);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.BadItemLimit = MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobBadItemLimit);
			this.LargeItemLimit = MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobLargeItemLimit);
			this.TargetDatabases = message.GetValueOrDefault<string[]>(MigrationBatchMessageSchema.MigrationJobTargetDatabase, null);
			this.TargetArchiveDatabases = message.GetValueOrDefault<string[]>(MigrationBatchMessageSchema.MigrationJobTargetArchiveDatabase, null);
			this.PrimaryOnly = message.GetValueOrDefault<bool?>(MigrationBatchMessageSchema.MigrationJobPrimaryOnly, null);
			this.ArchiveOnly = message.GetValueOrDefault<bool?>(MigrationBatchMessageSchema.MigrationJobArchiveOnly, null);
			this.TargetDeliveryDomain = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobTargetDeliveryDomain, null);
			this.StartAfter = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobStartAfter);
			this.CompleteAfter = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobCompleteAfter);
			JobSubscriptionSettingsBase.ValidatePrimaryArchiveExclusivity(this.PrimaryOnly, this.ArchiveOnly);
			return base.ReadFromMessageItem(message);
		}

		public override void WriteToBatch(MigrationBatch batch)
		{
			batch.TargetDatabases = this.TargetDatabases;
			batch.TargetArchiveDatabases = this.TargetArchiveDatabases;
			batch.BadItemLimit = (this.BadItemLimit ?? Unlimited<int>.UnlimitedValue);
			batch.LargeItemLimit = (this.LargeItemLimit ?? Unlimited<int>.UnlimitedValue);
			batch.PrimaryOnly = this.PrimaryOnly;
			batch.ArchiveOnly = this.ArchiveOnly;
			batch.TargetDeliveryDomain = this.TargetDeliveryDomain;
			if (this.StartAfter != null)
			{
				batch.StartAfter = (DateTime?)MigrationHelper.GetLocalizedDateTime(this.StartAfter, batch.UserTimeZone.ExTimeZone);
				batch.StartAfterUTC = (DateTime?)MigrationHelper.GetUniversalDateTime(this.StartAfter);
			}
			if (this.CompleteAfter != null)
			{
				batch.CompleteAfter = (DateTime?)MigrationHelper.GetLocalizedDateTime(this.CompleteAfter, batch.UserTimeZone.ExTimeZone);
				batch.CompleteAfterUTC = (DateTime?)MigrationHelper.GetUniversalDateTime(this.CompleteAfter);
			}
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			parent.Add(new object[]
			{
				new XElement("TargetDatabases", string.Join(",", this.TargetDatabases ?? new string[0])),
				new XElement("TargetArchiveDatabases", string.Join(",", this.TargetArchiveDatabases ?? new string[0])),
				new XElement("BadItemLimit", this.BadItemLimit),
				new XElement("LargeItemLimit", this.LargeItemLimit),
				new XElement("PrimaryOnly", this.PrimaryOnly),
				new XElement("ArchiveOnly", this.ArchiveOnly),
				new XElement("TargetDeliveryDomain", this.TargetDeliveryDomain),
				new XElement("StartAfter", this.StartAfter),
				new XElement("CompleteAfter", this.CompleteAfter)
			});
		}

		protected override void InitalizeFromBatch(MigrationBatch batch)
		{
			this.TargetDatabases = (batch.TargetDatabases ?? new string[0]).ToArray();
			this.TargetArchiveDatabases = (batch.TargetArchiveDatabases ?? new string[0]).ToArray();
			this.BadItemLimit = new Unlimited<int>?(batch.BadItemLimit);
			this.LargeItemLimit = (this.isLocalMove ? null : new Unlimited<int>?(batch.LargeItemLimit));
			this.PrimaryOnly = batch.PrimaryOnly;
			this.ArchiveOnly = batch.ArchiveOnly;
			this.TargetDeliveryDomain = batch.TargetDeliveryDomain;
			this.StartAfter = MigrationHelper.GetUniversalDateTime((ExDateTime?)batch.StartAfter);
			this.CompleteAfter = MigrationHelper.GetUniversalDateTime((ExDateTime?)batch.CompleteAfter);
			JobSubscriptionSettingsBase.ValidatePrimaryArchiveExclusivity(this.PrimaryOnly, this.ArchiveOnly);
		}

		public static readonly PropertyDefinition[] MoveJobSubscriptionSettingsPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobTargetDatabase,
				MigrationBatchMessageSchema.MigrationJobTargetArchiveDatabase,
				MigrationBatchMessageSchema.MigrationJobBadItemLimit,
				MigrationBatchMessageSchema.MigrationJobLargeItemLimit,
				MigrationBatchMessageSchema.MigrationJobPrimaryOnly,
				MigrationBatchMessageSchema.MigrationJobArchiveOnly,
				MigrationBatchMessageSchema.MigrationJobTargetDeliveryDomain,
				MigrationBatchMessageSchema.MigrationJobStartAfter,
				MigrationBatchMessageSchema.MigrationJobCompleteAfter
			},
			SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions
		});

		private readonly bool isLocalMove;
	}
}
