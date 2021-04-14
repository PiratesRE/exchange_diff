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
	internal class ExchangeJobSubscriptionSettings : JobSubscriptionSettingsBase
	{
		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return ExchangeJobSubscriptionSettings.ExchangeJobSubscriptionSettingsPropertyDefinitions;
			}
		}

		public ExDateTime? StartAfter { get; private set; }

		protected override bool IsEmpty
		{
			get
			{
				return base.IsEmpty && this.StartAfter == null;
			}
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			MigrationHelper.WriteOrDeleteNullableProperty<ExDateTime?>(message, MigrationBatchMessageSchema.MigrationJobStartAfter, this.StartAfter);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.StartAfter = message.GetValueOrDefault<ExDateTime?>(MigrationBatchMessageSchema.MigrationJobStartAfter, null);
			return base.ReadFromMessageItem(message);
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			parent.Add(new XElement("StartAfter", this.StartAfter));
		}

		public override void WriteToBatch(MigrationBatch batch)
		{
			if (this.StartAfter != null)
			{
				batch.StartAfter = (DateTime?)MigrationHelper.GetLocalizedDateTime(this.StartAfter, batch.UserTimeZone.ExTimeZone);
				batch.StartAfterUTC = (DateTime?)MigrationHelper.GetUniversalDateTime(this.StartAfter);
			}
		}

		protected override void InitalizeFromBatch(MigrationBatch batch)
		{
			this.StartAfter = MigrationHelper.GetUniversalDateTime((ExDateTime?)batch.StartAfter);
		}

		public static readonly PropertyDefinition[] ExchangeJobSubscriptionSettingsPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobStartAfter
			},
			SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions
		});
	}
}
