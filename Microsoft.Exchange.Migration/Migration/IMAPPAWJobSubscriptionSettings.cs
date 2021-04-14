using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IMAPPAWJobSubscriptionSettings : IMAPJobSubscriptionSettings
	{
		public ExDateTime? StartAfter { get; private set; }

		public ExDateTime? CompleteAfter { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return IMAPPAWJobSubscriptionSettings.IMAPJobSubscriptionSettingsPropertyDefinitions;
			}
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			MigrationHelper.WriteOrDeleteNullableProperty<ExDateTime?>(message, MigrationBatchMessageSchema.MigrationJobStartAfter, this.StartAfter);
			MigrationHelper.WriteOrDeleteNullableProperty<ExDateTime?>(message, MigrationBatchMessageSchema.MigrationJobCompleteAfter, this.CompleteAfter);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.StartAfter = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobStartAfter);
			this.CompleteAfter = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobCompleteAfter);
			return base.ReadFromMessageItem(message);
		}

		public override void WriteToBatch(MigrationBatch batch)
		{
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
			base.WriteToBatch(batch);
		}

		protected override void InitalizeFromBatch(MigrationBatch batch)
		{
			base.InitalizeFromBatch(batch);
			this.StartAfter = MigrationHelper.GetUniversalDateTime((ExDateTime?)batch.StartAfter);
			this.CompleteAfter = MigrationHelper.GetUniversalDateTime((ExDateTime?)batch.CompleteAfter);
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			base.AddDiagnosticInfoToElement(dataProvider, parent, argument);
			parent.Add(new object[]
			{
				new XElement("StartAfter", this.StartAfter),
				new XElement("CompleteAfter", this.CompleteAfter)
			});
		}

		public static readonly PropertyDefinition[] IMAPJobSubscriptionSettingsPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobStartAfter,
				MigrationBatchMessageSchema.MigrationJobCompleteAfter
			},
			SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions
		});
	}
}
