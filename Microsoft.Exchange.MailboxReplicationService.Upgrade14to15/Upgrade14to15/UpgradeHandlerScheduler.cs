using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpgradeHandlerScheduler : CacheScheduler
	{
		internal UpgradeHandlerScheduler(AnchorContext context, IOrganizationOperation orgOperationProxyInstance, ISymphonyProxy symphonyProxyInstance, WaitHandle stopEvent) : base(context, stopEvent)
		{
			string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
			AnchorUtil.AssertOrThrow(!string.IsNullOrEmpty(domainName), "Expect to have valid domain name set", new object[0]);
			int num = domainName.IndexOf('.');
			AnchorUtil.AssertOrThrow(num >= 0, "Expect a valid dns name {0}", new object[]
			{
				domainName
			});
			this.ForestName = domainName.Substring(0, num);
			this.orgOperationProxy = orgOperationProxyInstance;
			if (orgOperationProxyInstance is OrgOperationProxy)
			{
				((OrgOperationProxy)this.orgOperationProxy).Context = context;
			}
			this.symphonyProxy = symphonyProxyInstance;
			if (symphonyProxyInstance is SymphonyProxy)
			{
				((SymphonyProxy)this.symphonyProxy).WorkloadUri = new Uri(context.Config.GetConfig<Uri>("WebServiceUri"), "WorkloadService.svc");
				((SymphonyProxy)this.symphonyProxy).Cert = CertificateHelper.GetExchangeCertificate(context.Config.GetConfig<string>("CertificateSubject"));
			}
		}

		private string ForestName { get; set; }

		public override XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(parameters);
			diagnosticInfo.Add(new XElement("queriedCount", this.queriedCount));
			diagnosticInfo.Add(new XElement("tenantUpdatedCount", this.tenantUpdatedCount));
			diagnosticInfo.Add(new XElement("workItemUpdatedCount", this.workItemUpdatedCount));
			return diagnosticInfo;
		}

		internal AnchorJobProcessorResult InternalProcessEntry(ICacheEntry cacheEntryProxy)
		{
			this.queriedCount = 0;
			this.tenantUpdatedCount = 0;
			this.workItemUpdatedCount = 0;
			using (UpgradeHandlerSyncLog upgradeHandlerSyncLog = new UpgradeHandlerSyncLog("UpgradeHandlerSync"))
			{
				foreach (UpgradeRequestTypes upgradeRequestTypes in UpgradeHandlerScheduler.RequestTypes)
				{
					foreach (WorkItemStatus workItemStatus in UpgradeHandlerScheduler.WorkItemStatuses)
					{
						base.Context.Logger.Log(MigrationEventType.Information, "Querying UpgradeRequestType '{0}' and WorkItemStatus '{1}'", new object[]
						{
							upgradeRequestTypes,
							workItemStatus
						});
						WorkItemInfo[] array;
						try
						{
							array = this.QueryWorkItems(workItemStatus, upgradeRequestTypes.ToString());
						}
						catch (ErrorQueryingWorkItemException ex)
						{
							base.Context.Logger.Log(MigrationEventType.Error, "Error Querying WorkItems of Status: {0} Type: {1} Error: {2}", new object[]
							{
								workItemStatus,
								upgradeRequestTypes,
								ex.Message
							});
							return AnchorJobProcessorResult.Deleted;
						}
						foreach (WorkItemInfo workItemInfo in array)
						{
							TenantOrganizationPresentationObjectWrapper tenantOrganizationPresentationObjectWrapper = null;
							RecipientWrapper recipientWrapper = null;
							try
							{
								if (cacheEntryProxy is CacheEntryProxy)
								{
									AnchorJobProcessorResult anchorJobProcessorResult = base.ShouldProcessEntry(((CacheEntryProxy)cacheEntryProxy).CacheEntryBase);
									if (anchorJobProcessorResult != AnchorJobProcessorResult.Working)
									{
										base.Context.Logger.Log(MigrationEventType.Information, "Returning to anchor service because cacheEntry is '{0}'.", new object[]
										{
											anchorJobProcessorResult
										});
										return anchorJobProcessorResult;
									}
								}
								UpgradeRequestTypes upgradeRequestTypes2 = (UpgradeRequestTypes)Enum.Parse(typeof(UpgradeRequestTypes), workItemInfo.WorkItemType, true);
								string text = workItemInfo.Tenant.TenantId.ToString();
								string text2 = (upgradeRequestTypes2 == UpgradeRequestTypes.PilotUpgrade) ? workItemInfo.PilotUser.PilotUserId.ToString() : null;
								base.Context.Logger.Log(MigrationEventType.Information, "Processing workitemId '{0}' Type '{1}' Status '{2}' for TenantId '{3}'{4}", new object[]
								{
									workItemInfo.WorkItemId,
									workItemInfo.WorkItemType,
									workItemInfo.WorkItemStatus.Status,
									text,
									(text2 != null) ? (" PilotUserId '" + text2 + "'") : string.Empty
								});
								tenantOrganizationPresentationObjectWrapper = this.orgOperationProxy.GetOrganization(text);
								base.Context.Logger.Log(MigrationEventType.Information, "Found organization '{0}' ('{1}') with UpgradeRequest '{2}' UpgradeStatus '{3}'", new object[]
								{
									text,
									tenantOrganizationPresentationObjectWrapper.Name,
									tenantOrganizationPresentationObjectWrapper.UpgradeRequest,
									tenantOrganizationPresentationObjectWrapper.UpgradeStatus
								});
								if (tenantOrganizationPresentationObjectWrapper.UpgradeStatus == UpgradeStatusTypes.Error)
								{
									this.UpdateWIStatus(workItemInfo, WorkItemStatus.Error, tenantOrganizationPresentationObjectWrapper.UpgradeMessage);
								}
								else
								{
									if (tenantOrganizationPresentationObjectWrapper.AdminDisplayVersion.ExchangeBuild.Major != ExchangeObjectVersion.Exchange2012.ExchangeBuild.Major && (tenantOrganizationPresentationObjectWrapper.AdminDisplayVersion.ExchangeBuild.Major != ExchangeObjectVersion.Exchange2010.ExchangeBuild.Major || tenantOrganizationPresentationObjectWrapper.AdminDisplayVersion.ExchangeBuild.Minor < 16))
									{
										throw new InvalidOrganizationVersionException(text, tenantOrganizationPresentationObjectWrapper.AdminDisplayVersion);
									}
									if (tenantOrganizationPresentationObjectWrapper.UpgradeRequest == UpgradeRequestTypes.TenantUpgradeDryRun)
									{
										throw new OrganizationInDryRunModeException(text, workItemInfo.WorkItemType);
									}
									if (upgradeRequestTypes2 != UpgradeRequestTypes.CancelPrestageUpgrade)
									{
										TenantData tenantData = new TenantData(text);
										tenantData.UpdateFromTenant(tenantOrganizationPresentationObjectWrapper);
										if (tenantData.Constraints != null && tenantData.Constraints.Length > 0)
										{
											throw new OrganizationHasConstraintsException(upgradeRequestTypes2, text, tenantOrganizationPresentationObjectWrapper.Name, string.Join(",", tenantData.Constraints));
										}
									}
									RecipientWrapper recipientWrapper2 = null;
									if (!this.orgOperationProxy.TryGetAnchorMailbox(text, out recipientWrapper2))
									{
										base.Context.Logger.Log(MigrationEventType.Information, "AnchorMailbox for '{0}' not found. Need to create it.", new object[]
										{
											tenantOrganizationPresentationObjectWrapper.Name
										});
										this.orgOperationProxy.CreateAnchorMailbox(tenantOrganizationPresentationObjectWrapper.Name);
									}
									else if (!recipientWrapper2.PersistedCapabilities.Contains(Capability.OrganizationCapabilityTenantUpgrade))
									{
										base.Context.Logger.Log(MigrationEventType.Information, "Anchor mbx for organization '{0}' exists, but need to set TenantUpgrade capability", new object[]
										{
											text
										});
										this.orgOperationProxy.SetTenantUpgradeCapability(recipientWrapper2.Identity, true);
									}
									WorkItemStatus newStatus = WorkItemStatus.InProgress;
									string comment = string.Empty;
									if (upgradeRequestTypes2 == UpgradeRequestTypes.PilotUpgrade && tenantOrganizationPresentationObjectWrapper.UpgradeRequest != UpgradeRequestTypes.TenantUpgrade)
									{
										recipientWrapper = this.orgOperationProxy.GetUser(text, text2);
										if (recipientWrapper.RecipientType != RecipientType.UserMailbox)
										{
											string text3 = string.Format("Pilot user '{0}' cannot be piloted because it is of type '{1}' instead of 'UserMailbox', completing WI", text2, recipientWrapper.RecipientType.ToString());
											base.Context.Logger.Log(MigrationEventType.Information, text3, new object[0]);
											this.orgOperationProxy.SetUser(recipientWrapper, UpgradeStatusTypes.Complete, UpgradeRequestTypes.PilotUpgrade, text3, string.Empty, new UpgradeStage?(UpgradeStage.SyncedWorkItem));
										}
										if (recipientWrapper.UpgradeRequest != UpgradeRequestTypes.PilotUpgrade)
										{
											base.Context.Logger.Log(MigrationEventType.Information, "PilotUpgrade. initializing user '{0}' to PilotUpgrade InProgress", new object[]
											{
												text2
											});
											this.orgOperationProxy.SetUser(recipientWrapper, UpgradeStatusTypes.InProgress, UpgradeRequestTypes.PilotUpgrade, string.Empty, string.Empty, new UpgradeStage?(UpgradeStage.SyncedWorkItem));
											upgradeHandlerSyncLog.Write(tenantOrganizationPresentationObjectWrapper, recipientWrapper, string.Empty, string.Empty);
										}
										else
										{
											if (!Enum.TryParse<WorkItemStatus>(recipientWrapper.UpgradeStatus.ToString(), out newStatus))
											{
												throw new InvalidUpgradeStatusException(recipientWrapper.Id.ToString(), recipientWrapper.UpgradeStatus);
											}
											comment = recipientWrapper.UpgradeMessage;
										}
										this.UpdateWIStatus(workItemInfo, newStatus, comment);
									}
									else
									{
										if (upgradeRequestTypes2 != UpgradeRequestTypes.TenantUpgrade && ((upgradeRequestTypes2 != UpgradeRequestTypes.PrestageUpgrade && upgradeRequestTypes2 != UpgradeRequestTypes.CancelPrestageUpgrade) || tenantOrganizationPresentationObjectWrapper.UpgradeRequest == UpgradeRequestTypes.TenantUpgrade))
										{
											throw new InvalidRequestedTypeException(text, tenantOrganizationPresentationObjectWrapper.UpgradeRequest, workItemInfo.WorkItemType);
										}
										if (upgradeRequestTypes2 != tenantOrganizationPresentationObjectWrapper.UpgradeRequest)
										{
											base.Context.Logger.Log(MigrationEventType.Information, "Changing organization '{0}' from '{1}','{2}' to '{3}','InProgress'", new object[]
											{
												text,
												tenantOrganizationPresentationObjectWrapper.UpgradeRequest,
												tenantOrganizationPresentationObjectWrapper.UpgradeStatus,
												upgradeRequestTypes2
											});
											this.SetOrganization(tenantOrganizationPresentationObjectWrapper, UpgradeStatusTypes.InProgress, upgradeRequestTypes2, UpgradeStage.SyncedWorkItem);
											upgradeHandlerSyncLog.Write(tenantOrganizationPresentationObjectWrapper, null, string.Empty, string.Empty);
										}
										else
										{
											if (tenantOrganizationPresentationObjectWrapper.UpgradeStatus == UpgradeStatusTypes.Complete || tenantOrganizationPresentationObjectWrapper.UpgradeStatus == UpgradeStatusTypes.ForceComplete)
											{
												base.Context.Logger.Log(MigrationEventType.Information, "organization '{0}' has UpgradeStatus '{1}'. Removing Upgrade capability", new object[]
												{
													text,
													tenantOrganizationPresentationObjectWrapper.UpgradeStatus
												});
												this.orgOperationProxy.SetTenantUpgradeCapability(recipientWrapper2.Identity, false);
											}
											if (!Enum.TryParse<WorkItemStatus>(tenantOrganizationPresentationObjectWrapper.UpgradeStatus.ToString(), out newStatus))
											{
												throw new InvalidUpgradeStatusException(text, tenantOrganizationPresentationObjectWrapper.UpgradeStatus);
											}
											comment = tenantOrganizationPresentationObjectWrapper.UpgradeMessage;
										}
										this.UpdateWIStatus(workItemInfo, newStatus, comment);
									}
								}
							}
							catch (Exception ex2)
							{
								base.Context.Logger.Log(MigrationEventType.Error, "Processing workitem id '{0}' failed due to: {1}", new object[]
								{
									workItemInfo.WorkItemId,
									ex2.ToString()
								});
								upgradeHandlerSyncLog.Write(tenantOrganizationPresentationObjectWrapper, recipientWrapper, ex2.GetType().ToString(), ex2.Message);
								try
								{
									this.UpdateWIStatus(workItemInfo, (ex2 is MigrationTransientException) ? WorkItemStatus.Warning : WorkItemStatus.Error, ex2.Message);
								}
								catch (Exception ex3)
								{
									base.Context.Logger.Log(MigrationEventType.Error, "Could not report exception for workitemId '{0}' due to: {1}", new object[]
									{
										workItemInfo.WorkItemId,
										ex3
									});
									upgradeHandlerSyncLog.Write(tenantOrganizationPresentationObjectWrapper, recipientWrapper, ex3.GetType().ToString(), ex3.Message);
								}
								try
								{
									if (recipientWrapper != null && recipientWrapper.UpgradeStatus != UpgradeStatusTypes.Warning && recipientWrapper.UpgradeStatus != UpgradeStatusTypes.Error)
									{
										this.orgOperationProxy.SetUser(recipientWrapper, UpgradeStatusTypes.Warning, recipientWrapper.UpgradeRequest, ex2.Message, ex2.ToString(), recipientWrapper.UpgradeStage);
									}
									else if (tenantOrganizationPresentationObjectWrapper != null && tenantOrganizationPresentationObjectWrapper.UpgradeStatus != UpgradeStatusTypes.Warning && tenantOrganizationPresentationObjectWrapper.UpgradeStatus != UpgradeStatusTypes.Error)
									{
										this.orgOperationProxy.SetOrganization(tenantOrganizationPresentationObjectWrapper, UpgradeStatusTypes.Warning, tenantOrganizationPresentationObjectWrapper.UpgradeRequest, ex2.Message, ex2.ToString(), tenantOrganizationPresentationObjectWrapper.UpgradeStage, -1, -1);
									}
								}
								catch (Exception ex4)
								{
									base.Context.Logger.Log(MigrationEventType.Error, "Could not report exception for workitemId '{0}' for '{1}' due to: {2}", new object[]
									{
										workItemInfo.WorkItemId,
										(recipientWrapper != null) ? recipientWrapper.ToString() : tenantOrganizationPresentationObjectWrapper.ToString(),
										ex4
									});
									upgradeHandlerSyncLog.Write(tenantOrganizationPresentationObjectWrapper, recipientWrapper, ex4.GetType().ToString(), ex4.Message);
								}
								if (!(ex2 is LocalizedException))
								{
									throw;
								}
							}
						}
					}
				}
			}
			base.Context.Logger.Log(MigrationEventType.Information, "Total WorkItems Queried: {0} Total Tenants Updated: {1} Total WorkItems Updated: {2}", new object[]
			{
				this.queriedCount,
				this.tenantUpdatedCount,
				this.workItemUpdatedCount
			});
			return AnchorJobProcessorResult.Deleted;
		}

		protected override AnchorJobProcessorResult ProcessEntry(CacheEntryBase cacheEntry)
		{
			return this.InternalProcessEntry(new CacheEntryProxy(cacheEntry));
		}

		private void SetOrganization(TenantOrganizationPresentationObjectWrapper tenant, UpgradeStatusTypes workitemStatus, UpgradeRequestTypes requestType, UpgradeStage upgradeStage = UpgradeStage.None)
		{
			base.Context.Logger.Log(MigrationEventType.Information, "updating {0} organization with status {1} and type {2}", new object[]
			{
				tenant.ExternalDirectoryOrganizationId,
				workitemStatus,
				requestType
			});
			this.orgOperationProxy.SetOrganization(tenant, workitemStatus, requestType, string.Empty, string.Empty, new UpgradeStage?(upgradeStage), -1, -1);
			this.tenantUpdatedCount++;
		}

		private void UpdateWIStatus(WorkItemInfo workItem, WorkItemStatus newStatus, string comment)
		{
			if (workItem.WorkItemStatus.Status == newStatus && string.Equals(workItem.WorkItemStatus.Comment, comment))
			{
				base.Context.Logger.Log(MigrationEventType.Information, "workItem '{0}' status is already set to {1} so skipping update-WI", new object[]
				{
					workItem.WorkItemId,
					newStatus
				});
				return;
			}
			if (workItem.WorkItemStatus.Status != WorkItemStatus.InProgress && newStatus != WorkItemStatus.ForceComplete)
			{
				base.Context.Logger.Log(MigrationEventType.Information, "First forcing WI status from {0} to InProgress", new object[]
				{
					workItem.WorkItemStatus.Status
				});
				workItem.WorkItemStatus.Status = WorkItemStatus.InProgress;
				this.symphonyProxy.UpdateWorkItem(workItem.WorkItemId, workItem.WorkItemStatus);
			}
			base.Context.Logger.Log(MigrationEventType.Information, "updating workItem '{0}' status from {1} to {2} Comment: '{3}'", new object[]
			{
				workItem.WorkItemId,
				workItem.WorkItemStatus.Status,
				newStatus,
				string.IsNullOrEmpty(comment) ? "<none>" : comment
			});
			workItem.WorkItemStatus.Status = newStatus;
			if (comment != null && comment.Length > 4000)
			{
				base.Context.Logger.Log(MigrationEventType.Information, "Truncating Comment reported to Symphony", new object[0]);
				comment = comment.Substring(0, 3997) + "...";
			}
			workItem.WorkItemStatus.Comment = comment;
			this.symphonyProxy.UpdateWorkItem(workItem.WorkItemId, workItem.WorkItemStatus);
			this.workItemUpdatedCount++;
		}

		private WorkItemInfo[] QueryWorkItems(WorkItemStatus status, string type)
		{
			WorkItemQueryResult workItemQueryResult = new WorkItemQueryResult();
			List<WorkItemInfo> list = new List<WorkItemInfo>();
			base.Context.Logger.Log(MigrationEventType.Information, "Querying for workitems of Type: {0} Status: {1}", new object[]
			{
				type,
				status
			});
			try
			{
				do
				{
					workItemQueryResult = this.symphonyProxy.QueryWorkItems(this.ForestName, null, type, status, 1000, workItemQueryResult.Bookmark);
					list.AddRange(workItemQueryResult.WorkItems);
				}
				while (workItemQueryResult.HasMoreResults);
			}
			catch (Exception ex)
			{
				if (ex is MigrationTransientException || ex is MigrationPermanentException || ex is InvalidOperationException)
				{
					base.Context.Logger.Log(MigrationEventType.Error, "Error querying workitems. Forest: {0} Type: {1} Status: {2} Exception: {3}", new object[]
					{
						this.ForestName,
						type,
						status,
						ex
					});
					throw new ErrorQueryingWorkItemException(ex);
				}
				throw;
			}
			base.Context.Logger.Log(MigrationEventType.Information, "Found {0} workitems for query of Forest: {1} Type: {2} Status: {3}", new object[]
			{
				list.Count,
				this.ForestName,
				type,
				status
			});
			this.queriedCount += list.Count;
			return list.ToArray();
		}

		private const int MaxSymphonyCommentLength = 4000;

		private static readonly UpgradeRequestTypes[] RequestTypes = new UpgradeRequestTypes[]
		{
			UpgradeRequestTypes.TenantUpgrade,
			UpgradeRequestTypes.PrestageUpgrade,
			UpgradeRequestTypes.CancelPrestageUpgrade,
			UpgradeRequestTypes.PilotUpgrade
		};

		private static readonly WorkItemStatus[] WorkItemStatuses = new WorkItemStatus[]
		{
			WorkItemStatus.NotStarted,
			WorkItemStatus.InProgress,
			WorkItemStatus.Warning,
			WorkItemStatus.Error
		};

		private int queriedCount;

		private int tenantUpdatedCount;

		private int workItemUpdatedCount;

		private IOrganizationOperation orgOperationProxy;

		private ISymphonyProxy symphonyProxy;
	}
}
