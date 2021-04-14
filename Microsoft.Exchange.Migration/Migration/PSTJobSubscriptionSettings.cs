using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PSTJobSubscriptionSettings : JobSubscriptionSettingsBase
	{
		public Unlimited<int>? BadItemLimit { get; private set; }

		public Unlimited<int>? LargeItemLimit { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return PSTJobSubscriptionSettings.JobSubscriptionSettingsPropertyDefinitions;
			}
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			MigrationHelper.WriteUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobBadItemLimit, this.BadItemLimit);
			MigrationHelper.WriteUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobLargeItemLimit, this.LargeItemLimit);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.BadItemLimit = MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobBadItemLimit);
			this.LargeItemLimit = MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobLargeItemLimit);
			return base.ReadFromMessageItem(message);
		}

		public override void WriteToBatch(MigrationBatch batch)
		{
			batch.BadItemLimit = (this.BadItemLimit ?? Unlimited<int>.UnlimitedValue);
			batch.LargeItemLimit = (this.LargeItemLimit ?? Unlimited<int>.UnlimitedValue);
		}

		protected override void InitalizeFromBatch(MigrationBatch batch)
		{
			this.BadItemLimit = new Unlimited<int>?(batch.BadItemLimit);
			this.LargeItemLimit = new Unlimited<int>?(batch.LargeItemLimit);
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			parent.Add(new object[]
			{
				new XElement("BadItemLimit", this.BadItemLimit),
				new XElement("LargeItemLimit", this.LargeItemLimit)
			});
		}

		public static readonly PropertyDefinition[] JobSubscriptionSettingsPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobBadItemLimit,
				MigrationBatchMessageSchema.MigrationJobLargeItemLimit
			},
			SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions
		});
	}
}
