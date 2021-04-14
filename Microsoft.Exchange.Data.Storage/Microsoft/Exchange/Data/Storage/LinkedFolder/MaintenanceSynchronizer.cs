using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.JobQueues;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MaintenanceSynchronizer : Synchronizer
	{
		public MaintenanceSynchronizer(TeamMailboxSyncJob job, MailboxSession mailboxSession, OrganizationId orgId, Uri webCollectionUrl, Guid webId, string siteUrl, string displayName, IResourceMonitor resourceMonitor, ICredentials credential, bool isOAuthCredential, bool enableHttpDebugProxy, Stream syncCycleLogStream) : base(job, mailboxSession, resourceMonitor, siteUrl, credential, isOAuthCredential, enableHttpDebugProxy, syncCycleLogStream)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			this.orgId = orgId;
			this.webCollectionUrl = webCollectionUrl;
			this.webId = webId;
			this.recordedSiteUrl = (string.IsNullOrEmpty(siteUrl) ? null : new Uri(siteUrl));
			this.displayName = displayName;
			this.loggingComponent = ProtocolLog.Component.Maintenance;
		}

		protected virtual Guid TeamMailboxGuid
		{
			get
			{
				return this.Job.SyncInfoEntry.MailboxGuid;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MaintenanceSynchronizer>(this);
		}

		public override IAsyncResult BeginExecute(AsyncCallback executeCallback, object state)
		{
			this.executionAsyncResult = new LazyAsyncResult(null, state, executeCallback);
			this.performanceCounter.Start(OperationType.EndToEnd);
			try
			{
				this.InitializeSyncMetadata();
				base.UpdateSyncMetadataOnBeginSync();
			}
			catch (StorageTransientException ex)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "MaintenanceSynchronizer.BeginExecute: failed with StorageTransientException", ex);
				this.executionAsyncResult.InvokeCallback(ex);
				return this.executionAsyncResult;
			}
			catch (StoragePermanentException ex2)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "MaintenanceSynchronizer.BeginExecute: failed with StoragePermanentException", ex2);
				this.executionAsyncResult.InvokeCallback(ex2);
				return this.executionAsyncResult;
			}
			ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, "MaintenanceSynchronizer.BeginExecute");
			Exception value = null;
			Uri uri;
			string b;
			if (this.recordedSiteUrl != null && this.webCollectionUrl != null && this.TryGetSharePointSiteUrlAndTitle(this.webCollectionUrl, this.webId, out uri, out b, out value))
			{
				if (UriComparer.IsEqual(this.recordedSiteUrl, uri))
				{
					if (string.Equals(this.displayName, b))
					{
						goto IL_286;
					}
				}
				try
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, false, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.orgId), 189, "BeginExecute", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\LinkedFolder\\MaintenanceSynchronizer.cs");
					ADUser aduser = tenantOrRootOrgRecipientSession.FindByExchangeGuid(this.TeamMailboxGuid) as ADUser;
					if (aduser == null)
					{
						value = new ObjectNotFoundException(new LocalizedString("Cannot find the team mailbox by mailbox guid " + this.TeamMailboxGuid));
					}
					else
					{
						aduser.SharePointUrl = uri;
						aduser.DisplayName = b;
						tenantOrRootOrgRecipientSession.Save(aduser);
						ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, "MaintenanceSynchronizer.BeginExecute: updated new SharePoint site URL: " + uri);
						if (!UriComparer.IsEqual(this.recordedSiteUrl, uri))
						{
							RpcClientWrapper.EnqueueTeamMailboxSyncRequest(this.mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn, this.mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, QueueType.TeamMailboxDocumentSync, this.mailboxSession.MailboxOwner.MailboxInfo.OrganizationId, "Maintenance Synchronizer", tenantOrRootOrgRecipientSession.LastUsedDc, SyncOption.Default);
							RpcClientWrapper.EnqueueTeamMailboxSyncRequest(this.mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn, this.mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, QueueType.TeamMailboxMembershipSync, this.mailboxSession.MailboxOwner.MailboxInfo.OrganizationId, "Maintenance Synchronizer", tenantOrRootOrgRecipientSession.LastUsedDc, SyncOption.Default);
						}
					}
				}
				catch (DataSourceOperationException ex3)
				{
					value = ex3;
				}
				catch (DataValidationException ex4)
				{
					value = ex4;
				}
				catch (TransientException ex5)
				{
					value = ex5;
				}
			}
			IL_286:
			this.executionAsyncResult.InvokeCallback(value);
			return this.executionAsyncResult;
		}

		public override void EndExecute(IAsyncResult asyncResult)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new InvalidOperationException("EndExecute: asyncResult or the AsyncState cannot be null here.");
			}
			if (!lazyAsyncResult.IsCompleted)
			{
				lazyAsyncResult.InternalWaitForCompletion();
			}
			this.performanceCounter.Stop(OperationType.EndToEnd, 1);
			this.resourceMonitor.ResetBudget();
			base.LastError = (lazyAsyncResult.Result as Exception);
			base.SaveSyncMetadata();
			if (base.LastError == null)
			{
				ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, "MaintenanceSynchronizer.EndExecute: Succeeded");
			}
			else
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "MaintenanceSynchronizer.EndExecute: Failed", base.LastError);
			}
			string[] logLine = this.performanceCounter.GetLogLine();
			foreach (string str in logLine)
			{
				ProtocolLog.LogStatistics(this.loggingComponent, this.loggingContext, "MaintenanceSynchronizer.Statistics: " + str);
			}
			base.Dispose();
		}

		protected override void InitializeSyncMetadata()
		{
			if (this.syncMetadata == null)
			{
				this.syncMetadata = UserConfigurationHelper.GetMailboxConfiguration(this.mailboxSession, "MaintenanceSynchronizerConfigurations", UserConfigurationTypes.Dictionary, true);
			}
		}

		protected override LocalizedString GetSyncIssueEmailErrorString(string error, out LocalizedString body)
		{
			body = ClientStrings.FailedMaintenanceSynchronizationsText(this.siteUri.AbsoluteUri, error);
			return ClientStrings.FailedMaintenanceSynchronizations(this.siteUri.AbsoluteUri);
		}

		protected virtual bool TryGetSharePointSiteUrlAndTitle(Uri collectionUrl, Guid webId, out Uri siteUrl, out string siteTitle, out Exception ex)
		{
			ex = null;
			siteUrl = null;
			siteTitle = string.Empty;
			try
			{
				using (ClientContext clientContext = new ClientContext(collectionUrl))
				{
					TeamMailboxClientOperations.ConfigureClientContext(clientContext, this.credential, this.isOAuthCredential, this.enableHttpDebugProxy, 60000);
					Site site = clientContext.Site;
					Web web = site.OpenWebById(webId);
					clientContext.Load<Web>(web, new Expression<Func<Web, object>>[]
					{
						(Web x) => x.Url,
						(Web x) => x.Title
					});
					clientContext.ExecuteQuery();
					string uriString = null;
					if (MaintenanceSynchronizer.TryEscapeAndGetWellFormedUrl(web.Url, out uriString))
					{
						siteUrl = new Uri(uriString);
						siteTitle = web.Title;
					}
					else
					{
						ex = new SharePointException(web.Url, ClientStrings.ErrorSharePointSiteHasNoValidUrl(web.Url));
					}
				}
			}
			catch (ClientRequestException ex2)
			{
				ex = ex2;
			}
			catch (ServerException ex3)
			{
				ex = ex3;
			}
			catch (TimeoutException ex4)
			{
				ex = ex4;
			}
			catch (IOException ex5)
			{
				ex = ex5;
			}
			catch (WebException ex6)
			{
				ex = ex6;
			}
			return ex == null;
		}

		public static bool TryEscapeAndGetWellFormedUrl(string url, out string escapedUrl)
		{
			escapedUrl = null;
			bool flag = false;
			if (!string.IsNullOrEmpty(url))
			{
				try
				{
					if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
					{
						flag = true;
						escapedUrl = url;
					}
					else
					{
						escapedUrl = Uri.EscapeUriString(url);
						flag = Uri.IsWellFormedUriString(escapedUrl, UriKind.Absolute);
					}
				}
				catch (UriFormatException)
				{
				}
			}
			if (!flag)
			{
				escapedUrl = null;
			}
			return flag;
		}

		private readonly PerformanceCounter performanceCounter = new PerformanceCounter();

		private readonly OrganizationId orgId;

		private readonly Uri webCollectionUrl;

		private readonly Guid webId;

		private readonly Uri recordedSiteUrl;

		private readonly string displayName;
	}
}
