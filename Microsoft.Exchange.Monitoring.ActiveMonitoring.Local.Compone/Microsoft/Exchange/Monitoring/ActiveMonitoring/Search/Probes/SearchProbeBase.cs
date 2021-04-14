using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public abstract class SearchProbeBase : ProbeWorkItem
	{
		private protected AttributeHelper AttributeHelper { protected get; private set; }

		protected virtual bool SkipOnNonHealthyCatalog
		{
			get
			{
				return false;
			}
		}

		protected virtual bool SkipOnNonActiveDatabase
		{
			get
			{
				return false;
			}
		}

		protected virtual bool SkipOnDisabledCatalog
		{
			get
			{
				return false;
			}
		}

		protected virtual bool SkipOnAutoDagExcludeFromMonitoring
		{
			get
			{
				return false;
			}
		}

		private protected bool IsFromInvokeMonitoringItem { protected get; private set; }

		protected abstract void InternalDoWork(CancellationToken cancellationToken);

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				this.AttributeHelper = new AttributeHelper(base.Definition);
				this.IsFromInvokeMonitoringItem = this.AttributeHelper.GetBool("FromInvokeMonitoringItem", false, false);
				if (!this.IsFromInvokeMonitoringItem)
				{
					string targetResource = base.Definition.TargetResource;
					if ((this.SkipOnDisabledCatalog || this.SkipOnNonHealthyCatalog) && SearchMonitoringHelper.IsCatalogDisabled(targetResource))
					{
						base.Result.StateAttribute23 = "Skipped";
						base.Result.StateAttribute24 = "CatalogDisabled";
						return;
					}
					if (this.SkipOnNonHealthyCatalog)
					{
						IndexStatus indexStatus = null;
						try
						{
							indexStatus = SearchMonitoringHelper.GetCachedLocalDatabaseIndexStatus(targetResource, true);
						}
						catch (IndexStatusException ex)
						{
							base.Result.StateAttribute23 = "Skipped";
							base.Result.StateAttribute24 = "CatalogNotHealthy";
							base.Result.StateAttribute25 = ex.GetType().Name;
							return;
						}
						if (indexStatus == null)
						{
							base.Result.StateAttribute23 = "Skipped";
							base.Result.StateAttribute24 = "CatalogNotHealthy";
							base.Result.StateAttribute25 = "IndexStatusNull";
							return;
						}
						if (indexStatus.IndexingState != ContentIndexStatusType.Healthy && indexStatus.IndexingState != ContentIndexStatusType.HealthyAndUpgrading)
						{
							base.Result.StateAttribute23 = "Skipped";
							base.Result.StateAttribute24 = "CatalogNotHealthy";
							base.Result.StateAttribute25 = indexStatus.IndexingState.ToString();
							return;
						}
					}
					if (this.SkipOnNonActiveDatabase)
					{
						if (!SearchMonitoringHelper.IsDatabaseActive(targetResource))
						{
							base.Result.StateAttribute23 = "Skipped";
							base.Result.StateAttribute24 = "CatalogNotActive";
							return;
						}
						CopyStatusClientCachedEntry cachedLocalDatabaseCopyStatus = SearchMonitoringHelper.GetCachedLocalDatabaseCopyStatus(targetResource);
						if (cachedLocalDatabaseCopyStatus == null || cachedLocalDatabaseCopyStatus.CopyStatus == null)
						{
							base.Result.StateAttribute23 = "Skipped";
							base.Result.StateAttribute24 = "CopyStatusNull";
							return;
						}
						if (cachedLocalDatabaseCopyStatus.CopyStatus.CopyStatus != CopyStatusEnum.Mounted)
						{
							base.Result.StateAttribute23 = "Skipped";
							base.Result.StateAttribute24 = "DatabaseActiveButNotMounted";
							return;
						}
					}
					if (this.SkipOnAutoDagExcludeFromMonitoring)
					{
						Guid mailboxDatabaseGuid = SearchMonitoringHelper.GetDatabaseInfo(targetResource).MailboxDatabaseGuid;
						if (CachedAdReader.Instance.GetDatabaseOnLocalServer(mailboxDatabaseGuid).AutoDagExcludeFromMonitoring)
						{
							base.Result.StateAttribute23 = "Skipped";
							base.Result.StateAttribute24 = "AutoDagExcludeFromMonitoring";
							return;
						}
					}
				}
				this.InternalDoWork(cancellationToken);
			}
			catch (Exception ex2)
			{
				string text = ex2.ToString();
				if (ex2 is SearchProbeFailureException || this.IsFromInvokeMonitoringItem)
				{
					string @string = this.AttributeHelper.GetString("OverrideString", false, null);
					if (string.IsNullOrEmpty(@string) || (!Regex.IsMatch(text, @string, RegexOptions.IgnoreCase) && text.IndexOf(@string, StringComparison.OrdinalIgnoreCase) < 0))
					{
						throw;
					}
					base.Result.StateAttribute23 = "Override";
					base.Result.StateAttribute24 = text;
					base.Result.StateAttribute25 = @string;
					SearchMonitoringHelper.LogInfo(this, "Failed probe is overriden by: '{0}', {1}", new object[]
					{
						@string,
						text
					});
				}
				else
				{
					base.Result.StateAttribute23 = "SkippedDueToFailure";
					base.Result.StateAttribute24 = text;
					SearchMonitoringHelper.LogInfo(this, "Unexpected probe failure: '{0}'", new object[]
					{
						ex2
					});
				}
			}
		}

		public override void PopulateDefinition<TDefinition>(TDefinition definition, Dictionary<string, string> propertyBag)
		{
		}
	}
}
