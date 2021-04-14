using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PSTJobItemSubscriptionSettings : JobItemSubscriptionSettingsBase
	{
		internal PSTJobItemSubscriptionSettings()
		{
		}

		public string PstFilePath { get; private set; }

		public bool? PrimaryOnly { get; private set; }

		public bool? ArchiveOnly { get; private set; }

		public string SourceRootFolder { get; private set; }

		public string TargetRootFolder { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return PSTJobItemSubscriptionSettings.PSTJobItemSubscriptionSettingsPropertyDefinitions;
			}
		}

		protected override bool IsEmpty
		{
			get
			{
				return base.IsEmpty && string.IsNullOrEmpty(this.PstFilePath) && this.PrimaryOnly == null && this.ArchiveOnly == null && string.IsNullOrEmpty(this.SourceRootFolder) && string.IsNullOrEmpty(this.TargetRootFolder);
			}
		}

		internal static PSTJobItemSubscriptionSettings CreateFromProperties(string pstFilePath)
		{
			return new PSTJobItemSubscriptionSettings
			{
				PstFilePath = pstFilePath,
				PrimaryOnly = new bool?(true),
				LastModifiedTime = ExDateTime.UtcNow
			};
		}

		public override JobItemSubscriptionSettingsBase Clone()
		{
			return new PSTJobItemSubscriptionSettings
			{
				PstFilePath = this.PstFilePath,
				PrimaryOnly = this.PrimaryOnly,
				ArchiveOnly = this.ArchiveOnly,
				SourceRootFolder = this.SourceRootFolder,
				TargetRootFolder = this.TargetRootFolder,
				LastModifiedTime = base.LastModifiedTime
			};
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			if (!string.IsNullOrEmpty(this.PstFilePath))
			{
				message[MigrationBatchMessageSchema.MigrationPSTFilePath] = this.PstFilePath;
			}
			if (!string.IsNullOrEmpty(this.SourceRootFolder))
			{
				message[MigrationBatchMessageSchema.MigrationSourceRootFolder] = this.SourceRootFolder;
			}
			if (!string.IsNullOrEmpty(this.TargetRootFolder))
			{
				message[MigrationBatchMessageSchema.MigrationTargetRootFolder] = this.TargetRootFolder;
			}
			MigrationHelper.WriteOrDeleteNullableProperty<bool?>(message, MigrationBatchMessageSchema.MigrationJobPrimaryOnly, this.PrimaryOnly);
			MigrationHelper.WriteOrDeleteNullableProperty<bool?>(message, MigrationBatchMessageSchema.MigrationJobArchiveOnly, this.ArchiveOnly);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.PstFilePath = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationPSTFilePath, null);
			this.PrimaryOnly = message.GetValueOrDefault<bool?>(MigrationBatchMessageSchema.MigrationJobPrimaryOnly, null);
			this.ArchiveOnly = message.GetValueOrDefault<bool?>(MigrationBatchMessageSchema.MigrationJobArchiveOnly, null);
			JobSubscriptionSettingsBase.ValidatePrimaryArchiveExclusivity(this.PrimaryOnly, this.ArchiveOnly);
			this.SourceRootFolder = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationSourceRootFolder, null);
			this.TargetRootFolder = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationTargetRootFolder, null);
			return base.ReadFromMessageItem(message);
		}

		public override void UpdateFromDataRow(IMigrationDataRow request)
		{
			bool flag = false;
			PSTMigrationDataRow pstmigrationDataRow = request as PSTMigrationDataRow;
			if (pstmigrationDataRow == null)
			{
				throw new ArgumentException("expected an PSTMigrationDataRow", "request");
			}
			if (!string.Equals(this.PstFilePath, pstmigrationDataRow.PSTFilePath))
			{
				this.PstFilePath = pstmigrationDataRow.PSTFilePath;
				flag = true;
			}
			bool flag2 = false;
			bool flag3 = false;
			if (pstmigrationDataRow.TargetMailboxType == MigrationMailboxType.PrimaryAndArchive)
			{
				flag2 = true;
				flag3 = true;
			}
			else if (pstmigrationDataRow.TargetMailboxType == MigrationMailboxType.PrimaryOnly)
			{
				flag2 = true;
			}
			else
			{
				flag3 = true;
			}
			if (!object.Equals(this.PrimaryOnly, flag2) || !object.Equals(this.ArchiveOnly, flag3))
			{
				this.PrimaryOnly = new bool?(flag2);
				this.ArchiveOnly = new bool?(flag3);
				JobSubscriptionSettingsBase.ValidatePrimaryArchiveExclusivity(this.PrimaryOnly, this.ArchiveOnly);
				flag = true;
			}
			if (!string.Equals(this.SourceRootFolder, pstmigrationDataRow.SourceRootFolder))
			{
				this.SourceRootFolder = pstmigrationDataRow.SourceRootFolder;
				flag = true;
			}
			if (!string.Equals(this.TargetRootFolder, pstmigrationDataRow.TargetRootFolder))
			{
				this.TargetRootFolder = pstmigrationDataRow.TargetRootFolder;
				flag = true;
			}
			if (flag || base.LastModifiedTime == ExDateTime.MinValue)
			{
				base.LastModifiedTime = ExDateTime.UtcNow;
			}
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			string content = string.IsNullOrEmpty(this.PstFilePath) ? string.Empty : this.PstFilePath;
			string content2 = string.IsNullOrEmpty(this.SourceRootFolder) ? string.Empty : this.SourceRootFolder;
			string content3 = string.IsNullOrEmpty(this.TargetRootFolder) ? string.Empty : this.TargetRootFolder;
			parent.Add(new object[]
			{
				new XElement("PSTFilePath", content),
				new XElement("PrimaryOnly", this.PrimaryOnly),
				new XElement("ArchiveOnly", this.ArchiveOnly),
				new XElement("SourceRootFolder", content2),
				new XElement("TargetRootFolder", content3)
			});
		}

		public static readonly PropertyDefinition[] PSTJobItemSubscriptionSettingsPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationPSTFilePath,
				MigrationBatchMessageSchema.MigrationJobPrimaryOnly,
				MigrationBatchMessageSchema.MigrationJobArchiveOnly,
				MigrationBatchMessageSchema.MigrationSourceRootFolder,
				MigrationBatchMessageSchema.MigrationTargetRootFolder
			},
			SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions
		});
	}
}
