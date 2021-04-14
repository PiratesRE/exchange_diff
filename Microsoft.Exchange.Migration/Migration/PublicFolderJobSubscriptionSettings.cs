using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class PublicFolderJobSubscriptionSettings : JobSubscriptionSettingsBase
	{
		public Unlimited<int>? BadItemLimit { get; private set; }

		public Unlimited<int>? LargeItemLimit { get; private set; }

		public string SourcePublicFolderDatabase { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return PublicFolderJobSubscriptionSettings.DefaultPropertyDefinitions;
			}
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			parent.Add(new object[]
			{
				new XElement("BadItemLimit", this.BadItemLimit),
				new XElement("LargeItemLimit", this.LargeItemLimit),
				new XElement("SourcePublicFolderDatabase", this.SourcePublicFolderDatabase)
			});
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			MigrationHelper.WriteUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobBadItemLimit, this.BadItemLimit);
			MigrationHelper.WriteUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobLargeItemLimit, this.LargeItemLimit);
			MigrationHelper.WriteOrDeleteNullableProperty<string>(message, MigrationBatchMessageSchema.MigrationJobSourcePublicFolderDatabase, this.SourcePublicFolderDatabase);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.BadItemLimit = MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobBadItemLimit);
			this.LargeItemLimit = MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationJobLargeItemLimit);
			this.SourcePublicFolderDatabase = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobSourcePublicFolderDatabase, null);
			return base.ReadFromMessageItem(message);
		}

		public override void WriteToBatch(MigrationBatch batch)
		{
			batch.BadItemLimit = (this.BadItemLimit ?? Unlimited<int>.UnlimitedValue);
			batch.LargeItemLimit = (this.LargeItemLimit ?? Unlimited<int>.UnlimitedValue);
			batch.SourcePublicFolderDatabase = this.SourcePublicFolderDatabase;
		}

		protected override void InitalizeFromBatch(MigrationBatch batch)
		{
			this.BadItemLimit = new Unlimited<int>?(batch.BadItemLimit);
			this.LargeItemLimit = new Unlimited<int>?(batch.LargeItemLimit);
			this.SourcePublicFolderDatabase = batch.SourcePublicFolderDatabase;
		}

		public static readonly PropertyDefinition[] DefaultPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions,
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobBadItemLimit,
				MigrationBatchMessageSchema.MigrationJobLargeItemLimit,
				MigrationBatchMessageSchema.MigrationJobSourcePublicFolderDatabase
			}
		});
	}
}
