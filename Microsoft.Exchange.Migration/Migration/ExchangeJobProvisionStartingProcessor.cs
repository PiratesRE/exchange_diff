using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ExchangeJobProvisionStartingProcessor : MigrationJobProvisionStartingProcessor
	{
		protected override IProvisioningData GetProvisioningData(MigrationJobItem jobItem)
		{
			ProvisioningData provisioningData = null;
			if (jobItem.Status == MigrationUserStatus.Provisioning)
			{
				provisioningData = ExchangeProvisioningDataFactory.GetProvisioningData(base.DataProvider, base.Job, jobItem);
			}
			else if (jobItem.Status == MigrationUserStatus.ProvisionUpdating)
			{
				provisioningData = ExchangeProvisioningDataFactory.GetProvisioningUpdateData(base.DataProvider, base.Job, jobItem);
			}
			if (provisioningData != null && base.Job.SubmittedByUserAdminType == SubmittedByUserAdminType.DataCenterAdmin)
			{
				provisioningData.Organization = base.DataProvider.ADProvider.TenantOrganizationName;
			}
			return provisioningData;
		}

		protected override void HandleProvisioningCompletedEvent(ProvisionedObject provisionedObj, MigrationJobItem jobItem)
		{
			bool flag;
			switch (jobItem.RecipientType)
			{
			case MigrationUserRecipientType.Mailbox:
				MigrationLogger.Log(MigrationEventType.Verbose, "provisioning complete: job item {0} handling mailbox user", new object[]
				{
					jobItem
				});
				flag = this.HandleProvisioningCompletedEventForMailboxUser(provisionedObj, jobItem);
				goto IL_D8;
			case MigrationUserRecipientType.Contact:
				MigrationLogger.Log(MigrationEventType.Verbose, "provisioning complete: job item {0} handling mail contact", new object[]
				{
					jobItem
				});
				flag = this.HandleProvisioningCompletedEventForMailContact(provisionedObj, jobItem);
				goto IL_D8;
			case MigrationUserRecipientType.Group:
				MigrationLogger.Log(MigrationEventType.Verbose, "provisioning complete: job item {0} handling group properties", new object[]
				{
					jobItem
				});
				flag = this.HandleProvisioningCompletedEventForGroup(provisionedObj, jobItem);
				goto IL_D8;
			case MigrationUserRecipientType.Mailuser:
				MigrationLogger.Log(MigrationEventType.Verbose, "provisioning complete: job item {0} handling MEU", new object[]
				{
					jobItem
				});
				flag = this.HandleProvisioningCompletedEventForMailEnabledUser(provisionedObj, jobItem);
				goto IL_D8;
			}
			throw new MigrationDataCorruptionException(string.Format("recipient type '{0}' was not expected.", jobItem.RecipientType));
			IL_D8:
			if (!flag)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "provisioning complete: job item {0} event not handled", new object[]
				{
					jobItem
				});
				base.HandleProvisioningCompletedEvent(provisionedObj, jobItem);
			}
		}

		protected override void HandleNullIProvisioningDataEvent(MigrationJobItem jobItem)
		{
			if (jobItem.Status == MigrationUserStatus.Provisioning)
			{
				base.HandleNullIProvisioningDataEvent(jobItem);
				return;
			}
			switch (jobItem.RecipientType)
			{
			case MigrationUserRecipientType.Mailbox:
				MigrationLogger.Log(MigrationEventType.Verbose, "null iprovisioningDataEvent: job item {0} checking for CSV", new object[]
				{
					jobItem
				});
				if (base.Job.IsStaged && !base.Job.SkipSettingTargetAddress)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "job {0} has input CSV, setting up source forwarding", new object[]
					{
						base.Job
					});
					LocalizedString? localizedString = this.UpdateTargetAddressAtSource(jobItem);
					if (localizedString != null)
					{
						MigrationPermanentException ex = new MigrationPermanentException(localizedString.Value);
						base.Job.ReportData.Append(Strings.MigrationReportJobItemFailed(jobItem.Identifier, localizedString.Value), ex, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
						jobItem.SetFailedStatus(base.DataProvider, MigrationUserStatus.Failed, ex, "error updating target address at source");
						return;
					}
				}
				jobItem.SetStatus(base.DataProvider, MigrationUserStatus.Queued);
				return;
			case MigrationUserRecipientType.Contact:
				MigrationLogger.Log(MigrationEventType.Verbose, "null iprovisioningDataEvent: job item {0} setting contact properties", new object[]
				{
					jobItem
				});
				jobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.Synced), null, null, null);
				return;
			case MigrationUserRecipientType.Group:
				MigrationLogger.Log(MigrationEventType.Verbose, "null iprovisioningDataEvent: job item {0} setting group properties", new object[]
				{
					jobItem
				});
				jobItem.SetGroupProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.Synced), new ExDateTime?(ExDateTime.UtcNow), null, 0, 0);
				return;
			case MigrationUserRecipientType.Mailuser:
				MigrationLogger.Log(MigrationEventType.Verbose, "null iprovisioningDataEvent: job item {0} setting MEU properties", new object[]
				{
					jobItem
				});
				jobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.Synced), null, null, null);
				return;
			}
			throw new MigrationDataCorruptionException(string.Format("recipient type '{0}' was not expected.", jobItem.RecipientType));
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeJobProvisionStartingProcessor>(this);
		}

		private bool HandleProvisioningCompletedEventForMailboxUser(ProvisionedObject provisionedObj, MigrationJobItem exchangeJobItem)
		{
			if (provisionedObj.Succeeded)
			{
				if (exchangeJobItem.Status == MigrationUserStatus.Provisioning)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "provisioning pass 1, for job item {0}", new object[]
					{
						exchangeJobItem
					});
					exchangeJobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.ProvisionUpdating), (MailboxData)provisionedObj.MailboxData, null, new ExDateTime?(ExDateTime.UtcNow));
				}
				else
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "provisioning pass 2, for job item {0}", new object[]
					{
						exchangeJobItem
					});
					MigrationUserStatus value = MigrationUserStatus.Queued;
					exchangeJobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(value), (MailboxData)provisionedObj.MailboxData, null, null);
				}
				return true;
			}
			return false;
		}

		private bool HandleProvisioningCompletedEventForMailEnabledUser(ProvisionedObject provisionedObj, MigrationJobItem exchangeJobItem)
		{
			if (provisionedObj.Succeeded)
			{
				if (exchangeJobItem.Status == MigrationUserStatus.Provisioning)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "provisioning pass 1, for job item {0}", new object[]
					{
						exchangeJobItem
					});
					exchangeJobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.ProvisionUpdating), null, null, null);
				}
				else
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "provisioning pass 2, for job item {0}", new object[]
					{
						exchangeJobItem
					});
					exchangeJobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.Synced), null, null, new ExDateTime?(ExDateTime.UtcNow));
				}
				return true;
			}
			return false;
		}

		private bool HandleProvisioningCompletedEventForMailContact(ProvisionedObject provisionedObj, MigrationJobItem exchangeJobItem)
		{
			if (provisionedObj.Succeeded)
			{
				if (exchangeJobItem.Status == MigrationUserStatus.Provisioning)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "provisioning pass 1, for job item {0}", new object[]
					{
						exchangeJobItem
					});
					exchangeJobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.ProvisionUpdating), null, null, new ExDateTime?(ExDateTime.UtcNow));
				}
				else
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "provisioning pass 2, for job item {0}", new object[]
					{
						exchangeJobItem
					});
					exchangeJobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.Synced), null, null, null);
				}
				return true;
			}
			return false;
		}

		private bool HandleProvisioningCompletedEventForGroup(ProvisionedObject provisionedObj, MigrationJobItem exchangeJobItem)
		{
			if (provisionedObj.Succeeded)
			{
				ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = (ExchangeProvisioningDataStorage)exchangeJobItem.ProvisioningData;
				LegacyExchangeMigrationGroupRecipient legacyExchangeMigrationGroupRecipient = (LegacyExchangeMigrationGroupRecipient)exchangeProvisioningDataStorage.ExchangeRecipient;
				if (exchangeJobItem.Status == MigrationUserStatus.Provisioning)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "({0}) Handling Pass-1 Provisioning-Completed Event for Group.", new object[]
					{
						exchangeJobItem
					});
					exchangeJobItem.SetGroupProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.ProvisionUpdating), null, null, 0, 0);
				}
				else
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "({0}) Handling Pass-2 Provisioning-Completed Event for Group.", new object[]
					{
						exchangeJobItem
					});
					if (legacyExchangeMigrationGroupRecipient.IsMembersProvisioned())
					{
						MigrationLogger.Log(MigrationEventType.Verbose, "({0}) Group properties provisioned; marking it as Synced.", new object[]
						{
							exchangeJobItem
						});
						exchangeJobItem.SetGroupProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.Synced), new ExDateTime?(ExDateTime.UtcNow), null, provisionedObj.GroupMemberProvisioned, provisionedObj.GroupMemberSkipped);
					}
					else
					{
						MigrationLogger.Log(MigrationEventType.Verbose, "({0}) Group properties are not provisioned completely yet; keeping it as ProvisionUpdating.", new object[]
						{
							exchangeJobItem
						});
						exchangeJobItem.SetGroupProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.ProvisionUpdating), null, null, provisionedObj.GroupMemberProvisioned, provisionedObj.GroupMemberSkipped);
					}
				}
				return true;
			}
			return false;
		}

		private LocalizedString? UpdateTargetAddressAtSource(MigrationJobItem jobItem)
		{
			ExchangeOutlookAnywhereEndpoint exchangeOutlookAnywhereEndpoint = (ExchangeOutlookAnywhereEndpoint)base.Job.SourceEndpoint;
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = (ExchangeProvisioningDataStorage)jobItem.ProvisioningData;
			string propertyValue = exchangeProvisioningDataStorage.ExchangeRecipient.GetPropertyValue<string>(PropTag.SmtpAddress);
			SmtpAddress smtpAddress = new SmtpAddress(propertyValue);
			if (!smtpAddress.IsValidAddress)
			{
				throw new InvalidSmtpAddressException(propertyValue);
			}
			ADRecipient adrecipientByProxyAddress = base.DataProvider.ADProvider.GetADRecipientByProxyAddress(propertyValue);
			if (adrecipientByProxyAddress == null)
			{
				return new LocalizedString?(ServerStrings.MigrationInvalidTargetAddress(propertyValue));
			}
			SmtpAddress? smtpAddress2 = null;
			foreach (ProxyAddress proxyAddress in adrecipientByProxyAddress.EmailAddresses)
			{
				SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
				if (smtpProxyAddress != null)
				{
					SmtpAddress value = new SmtpAddress(smtpProxyAddress.SmtpAddress);
					if (value.IsValidAddress && string.Equals(value.Domain, base.Job.TargetDomainName, StringComparison.OrdinalIgnoreCase))
					{
						smtpAddress2 = new SmtpAddress?(value);
						break;
					}
				}
			}
			if (smtpAddress2 == null)
			{
				return new LocalizedString?(ServerStrings.MigrationInvalidTargetProxyAddress(base.Job.TargetDomainName));
			}
			if (!smtpAddress2.Value.IsValidAddress)
			{
				throw new InvalidSmtpAddressException(smtpAddress2.Value.ToString());
			}
			string text = "SMTP:" + smtpAddress2.Value;
			NspiMigrationDataReader nspiDataReader = exchangeOutlookAnywhereEndpoint.GetNspiDataReader(null);
			nspiDataReader.SetRecipient(propertyValue, MRSMergeRequestAccessor.GetSettings(jobItem).MailboxDN, text);
			MigrationLogger.Log(MigrationEventType.Information, "job item ({0}) successfully updated source forwarding from {1} to {2}", new object[]
			{
				jobItem,
				propertyValue,
				text
			});
			return null;
		}
	}
}
