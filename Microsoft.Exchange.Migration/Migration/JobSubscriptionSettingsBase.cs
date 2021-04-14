using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal abstract class JobSubscriptionSettingsBase : SubscriptionSettingsBase, IJobSubscriptionSettings, ISubscriptionSettings, IMigrationSerializable
	{
		public static void ValidatePrimaryArchiveExclusivity(bool? primaryOnly, bool? archiveOnly)
		{
			if ((primaryOnly != null || archiveOnly != null) && (primaryOnly == null || archiveOnly == null || (primaryOnly == archiveOnly && primaryOnly.Value)))
			{
				throw new ArgumentException("PrimaryOnly and ArchiveOnly must only be specified together, and if they are specified they must be mutually exclusive or both false.");
			}
		}

		public abstract void WriteToBatch(MigrationBatch batch);

		public virtual void WriteExtendedProperties(PersistableDictionary dictionary)
		{
		}

		public virtual bool ReadExtendedProperties(PersistableDictionary dictionary)
		{
			return false;
		}

		internal static JobSubscriptionSettingsBase Create(MigrationType migrationType, bool isPAW)
		{
			if (migrationType <= MigrationType.ExchangeRemoteMove)
			{
				if (migrationType != MigrationType.IMAP)
				{
					if (migrationType == MigrationType.ExchangeOutlookAnywhere)
					{
						return new ExchangeJobSubscriptionSettings();
					}
					if (migrationType != MigrationType.ExchangeRemoteMove)
					{
						goto IL_56;
					}
				}
				else
				{
					if (!isPAW)
					{
						return new IMAPJobSubscriptionSettings();
					}
					return new IMAPPAWJobSubscriptionSettings();
				}
			}
			else if (migrationType != MigrationType.ExchangeLocalMove)
			{
				if (migrationType == MigrationType.PSTImport)
				{
					return new PSTJobSubscriptionSettings();
				}
				if (migrationType != MigrationType.PublicFolder)
				{
					goto IL_56;
				}
				return new PublicFolderJobSubscriptionSettings();
			}
			return new MoveJobSubscriptionSettings(migrationType == MigrationType.ExchangeLocalMove);
			IL_56:
			return null;
		}

		internal static PropertyDefinition[] GetPropertyDefinitions(MigrationType migrationType, bool isPAW = false)
		{
			if (migrationType <= MigrationType.ExchangeRemoteMove)
			{
				if (migrationType != MigrationType.IMAP)
				{
					if (migrationType == MigrationType.ExchangeOutlookAnywhere)
					{
						return ExchangeJobSubscriptionSettings.ExchangeJobSubscriptionSettingsPropertyDefinitions;
					}
					if (migrationType != MigrationType.ExchangeRemoteMove)
					{
						goto IL_51;
					}
				}
				else
				{
					if (!isPAW)
					{
						return SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions;
					}
					return IMAPPAWJobSubscriptionSettings.IMAPJobSubscriptionSettingsPropertyDefinitions;
				}
			}
			else if (migrationType != MigrationType.ExchangeLocalMove)
			{
				if (migrationType == MigrationType.PSTImport)
				{
					return PSTJobSubscriptionSettings.JobSubscriptionSettingsPropertyDefinitions;
				}
				if (migrationType != MigrationType.PublicFolder)
				{
					goto IL_51;
				}
				return PublicFolderJobSubscriptionSettings.DefaultPropertyDefinitions;
			}
			return MoveJobSubscriptionSettings.MoveJobSubscriptionSettingsPropertyDefinitions;
			IL_51:
			return null;
		}

		internal static IJobSubscriptionSettings CreateFromBatch(MigrationBatch batch, bool isPAW)
		{
			JobSubscriptionSettingsBase jobSubscriptionSettingsBase = JobSubscriptionSettingsBase.Create(batch.MigrationType, isPAW);
			if (jobSubscriptionSettingsBase == null)
			{
				return null;
			}
			jobSubscriptionSettingsBase.InitalizeFromBatch(batch);
			jobSubscriptionSettingsBase.LastModifiedTime = (ExDateTime)batch.SubscriptionSettingsModified;
			return jobSubscriptionSettingsBase;
		}

		internal static IJobSubscriptionSettings CreateFromMessage(IMigrationStoreObject message, MigrationType migrationType, PersistableDictionary extendedProperties, bool isPAW)
		{
			JobSubscriptionSettingsBase jobSubscriptionSettingsBase = JobSubscriptionSettingsBase.Create(migrationType, isPAW);
			if (jobSubscriptionSettingsBase != null && (jobSubscriptionSettingsBase.ReadFromMessageItem(message) | jobSubscriptionSettingsBase.ReadExtendedProperties(extendedProperties)))
			{
				return jobSubscriptionSettingsBase;
			}
			return null;
		}

		protected abstract void InitalizeFromBatch(MigrationBatch batch);
	}
}
