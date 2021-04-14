using System;
using System.Collections.Generic;
using System.Web.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ExchangeJobSyncInitializingProcessor : MigrationJobSyncInitializingProcessor
	{
		protected override bool IgnorePostCompleteSubmits
		{
			get
			{
				return true;
			}
		}

		protected override int UpdatesEncountered
		{
			get
			{
				if (base.Job.IsStaged)
				{
					return base.UpdatesEncountered;
				}
				return 0;
			}
			set
			{
				if (base.Job.IsStaged)
				{
					base.UpdatesEncountered = value;
				}
			}
		}

		protected override LegacyMigrationJobProcessorResponse UpdateJobItems(bool scheduleNewWork)
		{
			LegacyMigrationJobProcessorResponse response = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MaxRowsToProcessInOnePass");
			IEnumerable<MigrationJobItem> jobItemsByTypeAndGroupMemberProvisionedState = MigrationJobItem.GetJobItemsByTypeAndGroupMemberProvisionedState(base.DataProvider, base.Job, MigrationUserRecipientType.Group, MigrationUserStatus.Provisioning, GroupMembershipProvisioningState.MemberNotRetrieved, config);
			using (IEnumerator<MigrationJobItem> enumerator = jobItemsByTypeAndGroupMemberProvisionedState.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MigrationJobItem jobItem = enumerator.Current;
					response.Result = MigrationProcessorResult.Working;
					ItemStateTransitionHelper.RunJobItemOperation(base.Job, jobItem, base.DataProvider, MigrationUserStatus.Failed, delegate
					{
						ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = (ExchangeProvisioningDataStorage)jobItem.ProvisioningData;
						LegacyExchangeMigrationGroupRecipient legacyExchangeMigrationGroupRecipient = exchangeProvisioningDataStorage.ExchangeRecipient as LegacyExchangeMigrationGroupRecipient;
						MigrationUtil.AssertOrThrow(legacyExchangeMigrationGroupRecipient != null, "jobItem as expected to have an ExchangeMigrationGroupRecipient", new object[0]);
						if (!legacyExchangeMigrationGroupRecipient.IsMembersRetrieved())
						{
							NspiMigrationDataRowProvider nspiMigrationDataRowProvider = this.GetMigrationDataRowProvider() as NspiMigrationDataRowProvider;
							string[] members = nspiMigrationDataRowProvider.GetMembers(legacyExchangeMigrationGroupRecipient.Identifier);
							jobItem.SetGroupProperties(this.DataProvider, null, null, members, 0, 0);
							response.NumItemsProcessed++;
						}
					});
				}
			}
			return response;
		}

		protected override IMigrationDataRowProvider GetMigrationDataRowProvider()
		{
			ExchangeOutlookAnywhereEndpoint endpoint = (ExchangeOutlookAnywhereEndpoint)base.Job.SourceEndpoint;
			if (base.Job.IsStaged)
			{
				return new NspiCsvMigrationDataRowProvider(base.Job, base.DataProvider, true);
			}
			return new NspiMigrationDataRowProvider(endpoint, base.Job, true);
		}

		protected override void CreateNewJobItem(IMigrationDataRow dataRow)
		{
			NspiMigrationDataRow nspiMigrationDataRow = dataRow as NspiMigrationDataRow;
			MigrationUserStatus status = this.DetermineInitialStatus();
			LocalizedString localizedErrorMessage;
			if (!nspiMigrationDataRow.Recipient.TryValidateRequiredProperties(out localizedErrorMessage))
			{
				base.CreateFailedJobItem(dataRow, new MigrationPermanentException(localizedErrorMessage));
				return;
			}
			if (base.Job.IsStaged)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "handling CSV attachment provisioning", new object[0]);
				this.CreateOrUpdateJobItemForStagedSEM(null, nspiMigrationDataRow);
				return;
			}
			if (nspiMigrationDataRow.Recipient.RecipientType == MigrationUserRecipientType.PublicFolder)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "found a public folder {0}", new object[]
				{
					dataRow
				});
				base.CreateNewJobItem(dataRow, null, MigrationUserStatus.Queued);
				return;
			}
			if (nspiMigrationDataRow.Recipient.RecipientType == MigrationUserRecipientType.Mailbox || nspiMigrationDataRow.Recipient.RecipientType == MigrationUserRecipientType.Mailuser)
			{
				nspiMigrationDataRow.EncryptedPassword = ExchangeJobSyncInitializingProcessor.GenerateEncryptedPassword();
			}
			string propertyValue = nspiMigrationDataRow.Recipient.GetPropertyValue<string>(PropTag.SmtpAddress);
			ADRecipient adrecipientByProxyAddress = base.DataProvider.ADProvider.GetADRecipientByProxyAddress(propertyValue);
			if (adrecipientByProxyAddress == null)
			{
				base.CreateNewJobItem(dataRow, null, status);
				return;
			}
			MigrationUserRecipientType recipientType = ExchangeJobSyncInitializingProcessor.GetRecipientType(adrecipientByProxyAddress.RecipientTypeDetails);
			if (nspiMigrationDataRow.Recipient.RecipientType != recipientType)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "found a recipient type that DOESNT match {0} found {1} expected {2}", new object[]
				{
					propertyValue,
					nspiMigrationDataRow.Recipient.RecipientType,
					recipientType
				});
				base.CreateFailedJobItem(dataRow, new MigrationUnexpectedExchangePrincipalFoundException(propertyValue));
				return;
			}
			nspiMigrationDataRow.Recipient.DoesADObjectExist = true;
			base.CreateNewJobItem(dataRow, null, status);
		}

		protected override MigrationBatchError ProcessExistingJobItem(MigrationJobItem jobItem, IMigrationDataRow dataRow)
		{
			NspiMigrationDataRow nspiMigrationDataRow = dataRow as NspiMigrationDataRow;
			LocalizedString localizedErrorMessage;
			if (!nspiMigrationDataRow.Recipient.TryValidateRequiredProperties(out localizedErrorMessage))
			{
				bool flag = jobItem.UpdateDataRow(base.DataProvider, base.Job, dataRow);
				if (flag && base.Job.IsStaged)
				{
					this.UpdatesEncountered++;
				}
				MigrationPermanentException ex = new MigrationPermanentException(localizedErrorMessage);
				base.Job.ReportData.Append(Strings.MigrationReportJobItemFailed(jobItem.Identifier, ex.LocalizedString), ex, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
				jobItem.SetFailedStatus(base.DataProvider, MigrationUserStatus.Failed, ex, "all required properties are not available for type", true);
				return null;
			}
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = (ExchangeProvisioningDataStorage)jobItem.ProvisioningData;
			if ((nspiMigrationDataRow.Recipient.RecipientType == MigrationUserRecipientType.Mailbox || nspiMigrationDataRow.Recipient.RecipientType == MigrationUserRecipientType.Mailuser) && string.IsNullOrEmpty(nspiMigrationDataRow.EncryptedPassword))
			{
				if (!string.IsNullOrEmpty(exchangeProvisioningDataStorage.EncryptedPassword))
				{
					nspiMigrationDataRow.EncryptedPassword = exchangeProvisioningDataStorage.EncryptedPassword;
				}
				else if (!base.Job.IsStaged)
				{
					nspiMigrationDataRow.EncryptedPassword = ExchangeJobSyncInitializingProcessor.GenerateEncryptedPassword();
				}
			}
			if (base.Job.IsStaged)
			{
				if (this.CreateOrUpdateJobItemForStagedSEM(jobItem, nspiMigrationDataRow))
				{
					this.UpdatesEncountered++;
				}
				return null;
			}
			if (jobItem.RecipientType == MigrationUserRecipientType.Group && jobItem.Status == MigrationUserStatus.Synced)
			{
				LegacyExchangeMigrationGroupRecipient legacyExchangeMigrationGroupRecipient = exchangeProvisioningDataStorage.ExchangeRecipient as LegacyExchangeMigrationGroupRecipient;
				if (legacyExchangeMigrationGroupRecipient != null && legacyExchangeMigrationGroupRecipient.CountOfSkippedMembers > 0)
				{
					jobItem.UpdateDataRow(base.DataProvider, base.Job, dataRow);
					jobItem.UpdateAndEnableJobItem(base.DataProvider, base.Job, this.DetermineInitialStatus());
					return null;
				}
			}
			return base.ProcessExistingJobItem(jobItem, dataRow);
		}

		protected override MigrationBatchError HandleDuplicateMigrationDataRow(MigrationJobItem jobItem, IMigrationDataRow dataRow)
		{
			if (base.Job.IsStaged)
			{
				return base.HandleDuplicateMigrationDataRow(jobItem, dataRow);
			}
			return null;
		}

		protected override MigrationBatchError GetValidationError(IMigrationDataRow dataRow, LocalizedString locErrorString)
		{
			NspiMigrationDataRow nspiMigrationDataRow = (NspiMigrationDataRow)dataRow;
			return new MigrationBatchError
			{
				RowIndex = nspiMigrationDataRow.CursorPosition,
				EmailAddress = dataRow.Identifier,
				LocalizedErrorMessage = locErrorString
			};
		}

		protected override bool VerifyRequiredProperties(MigrationJobItem jobItem)
		{
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = (ExchangeProvisioningDataStorage)jobItem.ProvisioningData;
			LocalizedException ex = null;
			LocalizedString localizedErrorMessage;
			if (!exchangeProvisioningDataStorage.ExchangeRecipient.TryValidateRequiredProperties(out localizedErrorMessage))
			{
				ex = new MigrationPermanentException(localizedErrorMessage);
			}
			else if (jobItem.MigrationJob.IsStaged)
			{
				ADRecipient adrecipientByProxyAddress = base.DataProvider.ADProvider.GetADRecipientByProxyAddress(jobItem.Identifier);
				RecipientTypeDetails recipientTypeDetails = (adrecipientByProxyAddress != null) ? adrecipientByProxyAddress.RecipientTypeDetails : RecipientTypeDetails.None;
				RecipientTypeDetails recipientTypeDetails2 = recipientTypeDetails;
				if (recipientTypeDetails2 <= (RecipientTypeDetails)((ulong)-2147483648))
				{
					if (recipientTypeDetails2 <= RecipientTypeDetails.RoomMailbox)
					{
						if (recipientTypeDetails2 <= RecipientTypeDetails.LegacyMailbox)
						{
							if (recipientTypeDetails2 < RecipientTypeDetails.None)
							{
								goto IL_13E;
							}
							switch ((int)recipientTypeDetails2)
							{
							case 0:
								ex = new MigrationInvalidTargetAddressException(jobItem.Identifier);
								goto IL_150;
							case 1:
							case 2:
							case 4:
							case 8:
								goto IL_150;
							case 3:
							case 5:
							case 6:
							case 7:
								goto IL_13E;
							}
						}
						if (recipientTypeDetails2 == RecipientTypeDetails.RoomMailbox)
						{
							goto IL_150;
						}
					}
					else if (recipientTypeDetails2 == RecipientTypeDetails.EquipmentMailbox || recipientTypeDetails2 == RecipientTypeDetails.MailUser || recipientTypeDetails2 == (RecipientTypeDetails)((ulong)-2147483648))
					{
						goto IL_150;
					}
				}
				else if (recipientTypeDetails2 <= RecipientTypeDetails.RemoteEquipmentMailbox)
				{
					if (recipientTypeDetails2 == RecipientTypeDetails.RemoteRoomMailbox || recipientTypeDetails2 == RecipientTypeDetails.RemoteEquipmentMailbox)
					{
						goto IL_150;
					}
				}
				else if (recipientTypeDetails2 == RecipientTypeDetails.RemoteSharedMailbox || recipientTypeDetails2 == RecipientTypeDetails.TeamMailbox || recipientTypeDetails2 == RecipientTypeDetails.RemoteTeamMailbox)
				{
					goto IL_150;
				}
				IL_13E:
				ex = new UnsupportedTargetRecipientTypeException(recipientTypeDetails.ToString());
			}
			IL_150:
			if (ex != null)
			{
				base.Job.ReportData.Append(Strings.MigrationReportJobItemFailed(jobItem.Identifier, ex.LocalizedString), ex, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
				jobItem.SetFailedStatus(base.DataProvider, MigrationUserStatus.Failed, ex, "all required properties are not available for job-item", true);
				return false;
			}
			return base.VerifyRequiredProperties(jobItem);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeJobSyncInitializingProcessor>(this);
		}

		private static string GenerateEncryptedPassword()
		{
			string clearString = Membership.GeneratePassword(16, 3);
			return MigrationServiceFactory.Instance.GetCryptoAdapter().ClearStringToEncryptedString(clearString);
		}

		private static MigrationUserRecipientType GetRecipientType(RecipientTypeDetails type)
		{
			type &= RecipientTypeDetails.AllUniqueRecipientTypes;
			RecipientTypeDetails recipientTypeDetails = type;
			if (recipientTypeDetails <= RecipientTypeDetails.MailContact)
			{
				if (recipientTypeDetails <= RecipientTypeDetails.SharedMailbox)
				{
					if (recipientTypeDetails != RecipientTypeDetails.UserMailbox && recipientTypeDetails != RecipientTypeDetails.SharedMailbox)
					{
						return MigrationUserRecipientType.Unsupported;
					}
				}
				else if (recipientTypeDetails != RecipientTypeDetails.RoomMailbox && recipientTypeDetails != RecipientTypeDetails.EquipmentMailbox)
				{
					if (recipientTypeDetails != RecipientTypeDetails.MailContact)
					{
						return MigrationUserRecipientType.Unsupported;
					}
					return MigrationUserRecipientType.Contact;
				}
				return MigrationUserRecipientType.Mailbox;
			}
			if (recipientTypeDetails <= RecipientTypeDetails.MailUniversalDistributionGroup)
			{
				if (recipientTypeDetails == RecipientTypeDetails.MailUser)
				{
					return MigrationUserRecipientType.Mailuser;
				}
				if (recipientTypeDetails != RecipientTypeDetails.MailUniversalDistributionGroup)
				{
					return MigrationUserRecipientType.Unsupported;
				}
			}
			else if (recipientTypeDetails != RecipientTypeDetails.MailNonUniversalGroup && recipientTypeDetails != RecipientTypeDetails.MailUniversalSecurityGroup)
			{
				if (recipientTypeDetails != RecipientTypeDetails.PublicFolder)
				{
					return MigrationUserRecipientType.Unsupported;
				}
				return MigrationUserRecipientType.PublicFolder;
			}
			return MigrationUserRecipientType.Group;
		}

		private bool CreateOrUpdateJobItemForStagedSEM(MigrationJobItem jobItem, NspiMigrationDataRow dataRow)
		{
			ADRecipient adrecipientByProxyAddress = base.DataProvider.ADProvider.GetADRecipientByProxyAddress(dataRow.Identifier);
			RecipientTypeDetails recipientTypeDetails = (adrecipientByProxyAddress != null) ? adrecipientByProxyAddress.RecipientTypeDetails : RecipientTypeDetails.None;
			RecipientTypeDetails recipientTypeDetails2 = recipientTypeDetails;
			if (recipientTypeDetails2 <= (RecipientTypeDetails)((ulong)-2147483648))
			{
				if (recipientTypeDetails2 <= RecipientTypeDetails.RoomMailbox)
				{
					if (recipientTypeDetails2 <= RecipientTypeDetails.LegacyMailbox)
					{
						if (recipientTypeDetails2 < RecipientTypeDetails.None)
						{
							goto IL_126;
						}
						switch ((int)recipientTypeDetails2)
						{
						case 0:
							return this.CreateOrUpdateFailedJobItem(jobItem, dataRow, new MigrationInvalidTargetAddressException(dataRow.Identifier));
						case 1:
						case 2:
						case 4:
						case 8:
							goto IL_145;
						case 3:
						case 5:
						case 6:
						case 7:
							goto IL_126;
						}
					}
					if (recipientTypeDetails2 == RecipientTypeDetails.RoomMailbox)
					{
						goto IL_145;
					}
				}
				else if (recipientTypeDetails2 == RecipientTypeDetails.EquipmentMailbox || recipientTypeDetails2 == RecipientTypeDetails.MailUser || recipientTypeDetails2 == (RecipientTypeDetails)((ulong)-2147483648))
				{
					goto IL_145;
				}
			}
			else if (recipientTypeDetails2 <= RecipientTypeDetails.RemoteEquipmentMailbox)
			{
				if (recipientTypeDetails2 == RecipientTypeDetails.RemoteRoomMailbox || recipientTypeDetails2 == RecipientTypeDetails.RemoteEquipmentMailbox)
				{
					goto IL_145;
				}
			}
			else if (recipientTypeDetails2 == RecipientTypeDetails.RemoteSharedMailbox || recipientTypeDetails2 == RecipientTypeDetails.TeamMailbox || recipientTypeDetails2 == RecipientTypeDetails.RemoteTeamMailbox)
			{
				goto IL_145;
			}
			IL_126:
			return this.CreateOrUpdateFailedJobItem(jobItem, dataRow, new UnsupportedTargetRecipientTypeException(recipientTypeDetails.ToString()));
			IL_145:
			if (jobItem == null)
			{
				base.CreateNewJobItem(dataRow, null, this.DetermineInitialStatus());
				return false;
			}
			bool flag = jobItem.UpdateDataRow(base.DataProvider, base.Job, dataRow);
			if (flag)
			{
				if (Array.Exists<MigrationUserStatus>(MigrationJobSyncInitializingProcessor.ForcedUpdateStatusJobItemStatusArray, (MigrationUserStatus status) => jobItem.Status == status))
				{
					jobItem.UpdateAndEnableJobItem(base.DataProvider, base.Job, this.DetermineInitialStatus());
				}
			}
			return flag;
		}

		private bool CreateOrUpdateFailedJobItem(MigrationJobItem jobItem, IMigrationDataRow dataRow, LocalizedException localizedError)
		{
			if (jobItem == null)
			{
				base.CreateFailedJobItem(dataRow, localizedError);
				return false;
			}
			base.Job.ReportData.Append(Strings.MigrationReportJobItemFailed(jobItem.Identifier, localizedError.LocalizedString), localizedError, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
			bool result = jobItem.UpdateDataRow(base.DataProvider, base.Job, dataRow);
			jobItem.SetFailedStatus(base.DataProvider, MigrationUserStatus.Failed, localizedError, "CreateOrUpdateFailedJobItem had an error", true);
			return result;
		}

		private const int MaxPasswordLength = 16;

		private const int NumberAlphaNumericChars = 3;
	}
}
