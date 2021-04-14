using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OrgOperationProxy : IOrganizationOperation
	{
		public AnchorContext Context { get; set; }

		TenantOrganizationPresentationObjectWrapper IOrganizationOperation.GetOrganization(string tenantId)
		{
			TenantOrganizationPresentationObjectWrapper result;
			using (AnchorRunspaceProxy anchorRunspaceProxy = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.Context, "upgradehandlers"))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Get-Organization");
				pscommand.AddParameter("Identity", tenantId);
				TenantOrganizationPresentationObject tenant = null;
				try
				{
					tenant = anchorRunspaceProxy.RunPSCommandSingleOrDefault<TenantOrganizationPresentationObject>(pscommand);
				}
				catch (Exception ex)
				{
					this.Context.Logger.Log(MigrationEventType.Error, "MigrationPermanentException from GetOrganization '{0}'.{1}", new object[]
					{
						tenantId,
						ex
					});
					if (ex.InnerException is ManagementObjectNotFoundException)
					{
						throw new OrganizationNotFoundException(tenantId, ex);
					}
					throw;
				}
				result = new TenantOrganizationPresentationObjectWrapper(tenant);
			}
			return result;
		}

		List<TenantOrganizationPresentationObjectWrapper> IOrganizationOperation.GetAllOrganizations(bool checkAllPartitions)
		{
			List<TenantOrganizationPresentationObjectWrapper> result;
			using (AnchorRunspaceProxy anchorRunspaceProxy = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.Context, "upgradehandlers"))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Get-AccountPartition");
				Collection<AccountPartition> collection = null;
				try
				{
					collection = anchorRunspaceProxy.RunPSCommand<AccountPartition>(pscommand);
				}
				catch (MigrationPermanentException ex)
				{
					this.Context.Logger.Log(MigrationEventType.Error, "Get-AccountPartition failed: {0}", new object[]
					{
						ex
					});
					throw;
				}
				pscommand.Clear();
				List<TenantOrganizationPresentationObject> list = new List<TenantOrganizationPresentationObject>();
				if (collection == null || collection.Count == 0)
				{
					result = null;
				}
				else
				{
					Collection<AccountPartition> collection2 = new Collection<AccountPartition>();
					if (!checkAllPartitions)
					{
						AccountPartition accountPartition = collection.FirstOrDefault((AccountPartition p) => p.IsLocalForest);
						if (accountPartition == null)
						{
							this.Context.Logger.Log(MigrationEventType.Information, "No LocalForest partitions found", new object[0]);
							return null;
						}
						collection2.Add(accountPartition);
						this.Context.Logger.Log(MigrationEventType.Information, "Get-Organization will run on partition: {0} IsLocalPartition: {1}", new object[]
						{
							collection2.ElementAt(0).Name,
							collection2.ElementAt(0).IsLocalForest
						});
					}
					else
					{
						collection2 = collection;
					}
					foreach (AccountPartition accountPartition2 in collection2)
					{
						pscommand.AddCommand("Get-Organization");
						pscommand.AddParameter("AccountPartition", accountPartition2);
						pscommand.AddParameter("Filter", "OrganizationStatus -eq 1");
						Stopwatch stopwatch = Stopwatch.StartNew();
						Collection<TenantOrganizationPresentationObject> collection3 = new Collection<TenantOrganizationPresentationObject>();
						try
						{
							collection3 = anchorRunspaceProxy.RunPSCommand<TenantOrganizationPresentationObject>(pscommand);
							this.Context.Logger.Log(MigrationEventType.Information, "Get-Organization ran for partition {0} in: {1}, found {2} orgs.", new object[]
							{
								accountPartition2.Name,
								stopwatch.Elapsed.TotalSeconds,
								collection3.Count
							});
						}
						catch (MigrationPermanentException ex2)
						{
							this.Context.Logger.Log(MigrationEventType.Error, "Get-Organization failed for Partition {0} : {1}", new object[]
							{
								accountPartition2.Name,
								ex2
							});
							throw;
						}
						List<TenantOrganizationPresentationObject> collection4 = CommonUtils.RandomizeSequence<TenantOrganizationPresentationObject>(collection3.ToList<TenantOrganizationPresentationObject>());
						list.AddRange(collection4);
					}
					List<TenantOrganizationPresentationObjectWrapper> list2 = new List<TenantOrganizationPresentationObjectWrapper>();
					foreach (TenantOrganizationPresentationObject tenant in list)
					{
						list2.Add(new TenantOrganizationPresentationObjectWrapper(tenant));
					}
					result = list2;
				}
			}
			return result;
		}

		void IOrganizationOperation.SetOrganization(TenantOrganizationPresentationObjectWrapper tenant, UpgradeStatusTypes status, UpgradeRequestTypes request, string message, string details, UpgradeStage? upgradeStage, int e14MbxCountForCurrentStage, int nonUpgradeMoveRequestCount)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Set-Organization");
			pscommand.AddParameter("Identity", tenant.ExternalDirectoryOrganizationId);
			if (tenant.UpgradeStatus != status)
			{
				tenant.UpgradeStatus = status;
				pscommand.AddParameter("UpgradeStatus", status);
			}
			if (tenant.UpgradeRequest != request)
			{
				tenant.UpgradeRequest = request;
				pscommand.AddParameter("UpgradeRequest", request);
			}
			if (tenant.UpgradeMessage != message)
			{
				tenant.UpgradeMessage = message;
				pscommand.AddParameter("UpgradeMessage", message);
			}
			if (tenant.UpgradeDetails != details)
			{
				tenant.UpgradeDetails = details;
				pscommand.AddParameter("UpgradeDetails", details);
			}
			if (upgradeStage != UpgradeStage.None)
			{
				DateTime? dateTime = (upgradeStage != null) ? new DateTime?(DateTime.UtcNow) : null;
				if (tenant.UpgradeStage != upgradeStage)
				{
					tenant.UpgradeStage = upgradeStage;
					pscommand.AddParameter("UpgradeStage", upgradeStage);
					tenant.UpgradeStageTimeStamp = dateTime;
					pscommand.AddParameter("UpgradeStageTimeStamp", dateTime);
				}
				if (upgradeStage == null || tenant.UpgradeE14MbxCountForCurrentStage != e14MbxCountForCurrentStage)
				{
					tenant.UpgradeE14MbxCountForCurrentStage = ((upgradeStage != null) ? new int?(e14MbxCountForCurrentStage) : null);
					pscommand.AddParameter("UpgradeE14MbxCountForCurrentStage", tenant.UpgradeE14MbxCountForCurrentStage);
				}
				if (upgradeStage == null || tenant.UpgradeE14RequestCountForCurrentStage != nonUpgradeMoveRequestCount)
				{
					tenant.UpgradeE14RequestCountForCurrentStage = ((upgradeStage != null) ? new int?(nonUpgradeMoveRequestCount) : null);
					pscommand.AddParameter("UpgradeE14RequestCountForCurrentStage", tenant.UpgradeE14RequestCountForCurrentStage);
				}
				tenant.UpgradeLastE14CountsUpdateTime = ((upgradeStage != null) ? dateTime : null);
				pscommand.AddParameter("UpgradeLastE14CountsUpdateTime", tenant.UpgradeLastE14CountsUpdateTime);
			}
			this.RunPSCommandForOrgOrUser(pscommand);
		}

		RecipientWrapper IOrganizationOperation.GetUser(string organizationId, string userId)
		{
			RecipientWrapper result;
			using (AnchorRunspaceProxy anchorRunspaceProxy = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.Context, "upgradehandlers"))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Get-User");
				pscommand.AddParameter("Organization", organizationId);
				pscommand.AddParameter("Identity", userId);
				User user;
				try
				{
					user = anchorRunspaceProxy.RunPSCommandSingleOrDefault<User>(pscommand);
				}
				catch (Exception ex)
				{
					this.Context.Logger.Log(MigrationEventType.Error, "MigrationPermanentException from GetUser '{0}'.{1}", new object[]
					{
						userId,
						ex
					});
					if (ex.InnerException is ManagementObjectNotFoundException)
					{
						throw new UserNotFoundException(userId, ex.InnerException);
					}
					throw;
				}
				result = new RecipientWrapper(user);
			}
			return result;
		}

		void IOrganizationOperation.SetUser(RecipientWrapper user, UpgradeStatusTypes status, UpgradeRequestTypes request, string message, string details, UpgradeStage? stage)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Set-User");
			pscommand.AddParameter("Identity", user.Id.ToDNString());
			if (user.UpgradeStatus != status)
			{
				pscommand.AddParameter("UpgradeStatus", status);
				user.UpgradeStatus = status;
			}
			if (user.UpgradeRequest != request)
			{
				pscommand.AddParameter("UpgradeRequest", request);
				user.UpgradeRequest = request;
			}
			if (user.UpgradeMessage != message)
			{
				pscommand.AddParameter("UpgradeMessage", message);
				user.UpgradeMessage = message;
			}
			if (user.UpgradeDetails != details)
			{
				pscommand.AddParameter("UpgradeDetails", details);
				user.UpgradeDetails = details;
			}
			if (user.UpgradeStage != stage)
			{
				pscommand.AddParameter("UpgradeStage", stage);
				user.UpgradeStage = stage;
				user.UpgradeStageTimeStamp = ((stage != null) ? new DateTime?(DateTime.UtcNow) : null);
				pscommand.AddParameter("UpgradeStageTimeStamp", user.UpgradeStageTimeStamp);
			}
			if (pscommand.Commands[0].Parameters.Count > 1)
			{
				this.RunPSCommandForOrgOrUser(pscommand);
				return;
			}
			this.Context.Logger.Log(MigrationEventType.Information, "Set-User will not be invoked because no property values have been changed", new object[0]);
		}

		void IOrganizationOperation.InvokeOrganizationCmdlet(string organizationId, string cmdlet, bool configOnly)
		{
			using (AnchorRunspaceProxy anchorRunspaceProxy = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.Context, "upgradehandlers"))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand(cmdlet);
				pscommand.AddParameter("Identity", organizationId);
				if (configOnly && cmdlet.Contains("Start-OrganizationUpgrade"))
				{
					pscommand.AddParameter("ConfigOnly");
				}
				try
				{
					anchorRunspaceProxy.RunPSCommand<PSObject>(pscommand);
				}
				catch (MigrationPermanentException ex)
				{
					this.Context.Logger.Log(MigrationEventType.Error, "{0} '{1}' failed due to {2}", new object[]
					{
						cmdlet,
						organizationId,
						ex
					});
					throw;
				}
			}
		}

		bool IOrganizationOperation.TryGetAnchorMailbox(string tenantId, out RecipientWrapper anchorMailbox)
		{
			anchorMailbox = null;
			bool result;
			using (AnchorRunspaceProxy anchorRunspaceProxy = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.Context, "upgradehandlers"))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Get-Mailbox");
				pscommand.AddParameter("Arbitration");
				pscommand.AddParameter("Organization", tenantId);
				pscommand.AddParameter("Identity", "Migration.8f3e7716-2011-43e4-96b1-aba62d229136");
				Mailbox mailbox;
				try
				{
					mailbox = anchorRunspaceProxy.RunPSCommandSingleOrDefault<Mailbox>(pscommand);
					if (mailbox == null)
					{
						this.Context.Logger.Log(MigrationEventType.Error, "Get-AnchorMailbox for '{0}' returned null", new object[0]);
						return false;
					}
				}
				catch (Exception ex)
				{
					if (ex.InnerException != null && ex.InnerException is ManagementObjectNotFoundException)
					{
						this.Context.Logger.Log(MigrationEventType.Information, "Get-AnchorMailbox for '{0}' failed due to: {1}", new object[]
						{
							tenantId,
							ex.InnerException
						});
						return false;
					}
					this.Context.Logger.Log(MigrationEventType.Error, "Get-AnchorMailbox for '{0}' failed due to: {1}", new object[]
					{
						tenantId,
						ex
					});
					throw;
				}
				anchorMailbox = new RecipientWrapper(mailbox);
				result = true;
			}
			return result;
		}

		void IOrganizationOperation.CreateAnchorMailbox(string tenantId)
		{
			using (AnchorRunspaceProxy anchorRunspaceProxy = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.Context, "upgradehandlers"))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("New-Mailbox");
				pscommand.AddParameter("Arbitration");
				pscommand.AddParameter("Organization", tenantId);
				pscommand.AddParameter("Name", "Migration.8f3e7716-2011-43e4-96b1-aba62d229136");
				pscommand.AddParameter("DisplayName", "Microsoft Exchange Migration");
				pscommand.AddParameter("UserPrincipalName", "Migration.8f3e7716-2011-43e4-96b1-aba62d229136@" + tenantId);
				pscommand.AddParameter("OverrideRecipientQuotas");
				Mailbox mailbox;
				try
				{
					mailbox = anchorRunspaceProxy.RunPSCommandSingleOrDefault<Mailbox>(pscommand);
				}
				catch (MigrationPermanentException ex)
				{
					this.Context.Logger.Log(MigrationEventType.Error, "Unable to create Migration Mailbox for organization '{0}': {1}", new object[]
					{
						tenantId,
						ex
					});
					throw;
				}
				pscommand = new PSCommand();
				pscommand.AddCommand("Set-Mailbox");
				pscommand.AddParameter("Identity", mailbox.Identity);
				pscommand.AddParameter("Arbitration");
				pscommand.AddParameter("ProhibitSendReceiveQuota", "10GB");
				pscommand.AddParameter("ProhibitSendQuota", "10GB");
				pscommand.AddParameter("IssueWarningQuota", "9GB");
				pscommand.AddParameter("RecoverableItemsQuota", "30GB");
				pscommand.AddParameter("RecoverableItemsWarningQuota", "20GB");
				pscommand.AddParameter("UseDatabaseQuotaDefaults", false);
				pscommand.AddParameter("SCLDeleteEnabled", false);
				pscommand.AddParameter("SCLJunkEnabled", false);
				pscommand.AddParameter("SCLQuarantineEnabled", false);
				pscommand.AddParameter("SCLRejectEnabled", false);
				pscommand.AddParameter("HiddenFromAddressListsEnabled", true);
				pscommand.AddParameter("Management", true);
				pscommand.AddParameter("Force");
				pscommand.AddParameter("TenantUpgrade", true);
				int config = this.Context.Config.GetConfig<int>("NumberOfSetMailboxAttempts");
				int config2 = this.Context.Config.GetConfig<int>("SetMailboxAttemptIntervalSeconds");
				for (int i = 1; i <= config; i++)
				{
					try
					{
						anchorRunspaceProxy.RunPSCommand<Mailbox>(pscommand);
						break;
					}
					catch (MigrationPermanentException ex2)
					{
						this.Context.Logger.Log(MigrationEventType.Warning, "Unable to set defaults for anchor Mailbox for organization '{0}' after {1} attempt(s): {2}", new object[]
						{
							config,
							tenantId,
							ex2
						});
						if (i >= config)
						{
							this.Context.Logger.Log(MigrationEventType.Error, "Gving up attempts to set defaults for anchor Mailbox for organization '{0}': {1}", new object[]
							{
								tenantId,
								ex2
							});
							throw;
						}
						Thread.Sleep(config2 * 1000);
					}
				}
			}
		}

		void IOrganizationOperation.SetTenantUpgradeCapability(string identity, bool tenantUpgradeCapabilityEnabled)
		{
			using (AnchorRunspaceProxy anchorRunspaceProxy = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.Context, "upgradehandlers"))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Set-Mailbox");
				pscommand.AddParameter("Identity", identity);
				pscommand.AddParameter("Arbitration");
				pscommand.AddParameter("Force");
				pscommand.AddParameter("TenantUpgrade", tenantUpgradeCapabilityEnabled);
				try
				{
					anchorRunspaceProxy.RunPSCommand<Mailbox>(pscommand);
				}
				catch (MigrationPermanentException ex)
				{
					this.Context.Logger.Log(MigrationEventType.Error, "Unable to set TenantUpgrade capability for '{0}': {1}", new object[]
					{
						identity,
						ex
					});
					throw;
				}
			}
		}

		bool IOrganizationOperation.TryRemoveMoveRequest(string identity)
		{
			using (AnchorRunspaceProxy anchorRunspaceProxy = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.Context, "upgradehandlers"))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Remove-MoveRequest");
				pscommand.AddParameter("Identity", identity);
				pscommand.AddParameter("Confirm", false);
				try
				{
					anchorRunspaceProxy.RunPSCommandSingleOrDefault<object>(pscommand);
				}
				catch (MigrationPermanentException ex)
				{
					this.Context.Logger.Log(MigrationEventType.Error, "Remove-MoveRequest for '{0}' failed due to: {1}", new object[]
					{
						identity,
						ex
					});
					return false;
				}
			}
			return true;
		}

		private void RunPSCommandForOrgOrUser(PSCommand cmd)
		{
			using (AnchorRunspaceProxy anchorRunspaceProxy = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.Context, "upgradehandlers"))
			{
				try
				{
					anchorRunspaceProxy.RunPSCommand<PSObject>(cmd);
				}
				catch (MigrationPermanentException ex)
				{
					this.Context.Logger.Log(MigrationEventType.Error, "Run cmd {0} failed due to {1}", new object[]
					{
						cmd.ToString(),
						ex
					});
					throw;
				}
			}
		}
	}
}
