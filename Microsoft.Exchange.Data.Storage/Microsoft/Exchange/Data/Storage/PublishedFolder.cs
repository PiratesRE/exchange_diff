using System;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PublishedFolder : IDisposeTrackable, IDisposable
	{
		public StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public string DisplayName
		{
			get
			{
				this.CheckDisposed("DisplayName::get");
				if (string.IsNullOrEmpty(this.displayName))
				{
					using (Folder folder = Folder.Bind(this.MailboxSession, this.FolderId))
					{
						this.displayName = folder.DisplayName;
					}
				}
				return this.displayName;
			}
		}

		public string OwnerDisplayName
		{
			get
			{
				this.CheckDisposed("OwnerDisplayName::get");
				return this.MailboxSession.MailboxOwner.MailboxInfo.DisplayName;
			}
		}

		public abstract string BrowseUrl { get; }

		protected MailboxSession MailboxSession
		{
			get
			{
				if (this.mailboxSession == null)
				{
					this.mailboxSession = this.CreateMailboxSession();
					this.disposeMailboxSession = true;
				}
				return this.mailboxSession;
			}
		}

		protected IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipientSession == null)
				{
					this.recipientSession = this.CreateRecipientSession();
				}
				return this.recipientSession;
			}
		}

		private PublishedFolder(StoreObjectId folderId)
		{
			Util.ThrowOnNullArgument(folderId, "folderId");
			this.folderId = folderId;
			this.disposeTracker = this.GetDisposeTracker();
		}

		protected PublishedFolder(string domain, SecurityIdentifier sid, StoreObjectId folderId) : this(folderId)
		{
			Util.ThrowOnNullArgument(sid, "user");
			this.sid = sid;
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.AcquireBudgetAndStartTiming();
				disposeGuard.Success();
			}
			try
			{
				this.sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(domain);
			}
			catch (CannotResolveTenantNameException arg)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, CannotResolveTenantNameException>((long)this.GetHashCode(), "ObscureUrlKey.Lookup(): Cannot resolve tenant name {0}.Error: {1}", domain, arg);
			}
		}

		protected PublishedFolder(MailboxSession mailboxSession, StoreObjectId folderId) : this(folderId)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			this.mailboxSession = mailboxSession;
			this.sid = mailboxSession.MailboxOwner.Sid;
			this.disposeMailboxSession = false;
			this.sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(mailboxSession.MailboxOwner.MailboxInfo.OrganizationId ?? OrganizationId.ForestWideOrgId);
		}

		public static PublishedFolder Create(PublishingUrl publishingUrl)
		{
			Util.ThrowOnNullArgument(publishingUrl, "publishingUrl");
			PublishedFolder.SleepIfNecessary();
			SharingAnonymousIdentityCache instance = SharingAnonymousIdentityCache.Instance;
			SharingAnonymousIdentityCacheKey key = publishingUrl.CreateKey();
			SharingAnonymousIdentityCacheValue sharingAnonymousIdentityCacheValue = instance.Get(key);
			if (!sharingAnonymousIdentityCacheValue.IsAccessAllowed)
			{
				ExTraceGlobals.SharingTracer.TraceError<PublishingUrl, Type>(0L, "PublishedFolder.Create(PublishingUrl): Cannot find access allowed folder from the request url: path = {0}, type = {1}.", publishingUrl, publishingUrl.GetType());
				throw new PublishedFolderAccessDeniedException();
			}
			ExTraceGlobals.SharingTracer.TraceDebug<SecurityIdentifier, string, string>(0L, "PublishedFolder.Create(PublishingUrl): User {0} has Sharing Anonymous identity {1}. The corresponding folder identity is {2}.", sharingAnonymousIdentityCacheValue.Sid, publishingUrl.Identity, sharingAnonymousIdentityCacheValue.FolderId);
			StoreObjectId storeObjectId = null;
			try
			{
				storeObjectId = StoreObjectId.Deserialize(sharingAnonymousIdentityCacheValue.FolderId);
			}
			catch (CorruptDataException innerException)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>(0L, "PublishedFolder.Create(PublishingUrl): The folder identity '{0}' is invalid.", sharingAnonymousIdentityCacheValue.FolderId);
				throw new PublishedFolderAccessDeniedException(innerException);
			}
			if (publishingUrl.DataType == SharingDataType.ReachCalendar || publishingUrl.DataType == SharingDataType.Calendar)
			{
				ObscureUrl obscureUrl = publishingUrl as ObscureUrl;
				return new PublishedCalendar(publishingUrl.Domain, sharingAnonymousIdentityCacheValue.Sid, storeObjectId, (obscureUrl == null) ? null : new ObscureKind?(obscureUrl.ObscureKind), (obscureUrl == null) ? null : obscureUrl.ReachUserSid);
			}
			throw new NotSupportedException();
		}

		internal static PublishedFolder Create(Folder folder)
		{
			Util.ThrowOnNullArgument(folder, "folder");
			MailboxSession mailboxSession = folder.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new ArgumentException("folder must be in a mailbox");
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(folder.ClassName, "IPF.Appointment"))
			{
				return new PublishedCalendar(mailboxSession, folder.StoreObjectId);
			}
			throw new NotSupportedException();
		}

		private static bool SleepIfNecessary()
		{
			uint value = (uint)PublishedFolder.PublishedFolderSlowdownCpuThreshold.Value;
			int arg;
			float arg2;
			if (CPUBasedSleeper.SleepIfNecessary(value, out arg, out arg2))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<int, float>(0L, "PublishedFolder.SleepIfNecessary(): SleepTime = {0}, CpuPercent = {1}", arg, arg2);
				return true;
			}
			return false;
		}

		public ExchangePrincipal CreateExchangePrincipal()
		{
			this.CheckDisposed("CreateExchangePrincipal");
			return ExchangePrincipal.FromUserSid(this.RecipientSession, this.sid, RemotingOptions.AllowCrossSite);
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(this.ToString());
			}
		}

		private MailboxSession CreateMailboxSession()
		{
			ExTraceGlobals.SharingTracer.TraceDebug<SecurityIdentifier>((long)this.GetHashCode(), "Create mailbox session as SystemService for sid {0}.", this.sid);
			MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(ExchangePrincipal.FromUserSid(this.RecipientSession, this.sid), Thread.CurrentThread.CurrentCulture, "Client=AS;Action=PublishedFolder");
			if (this.budget != null)
			{
				mailboxSession.AccountingObject = this.budget;
			}
			mailboxSession.ExTimeZone = (TimeZoneHelper.GetUserTimeZone(mailboxSession) ?? ExTimeZone.CurrentTimeZone);
			return mailboxSession;
		}

		private IRecipientSession CreateRecipientSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, this.sessionSettings, 392, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\PublishedFolder.cs");
			if (this.budget != null)
			{
				tenantOrRootOrgRecipientSession.SessionSettings.AccountingObject = this.budget;
			}
			return tenantOrRootOrgRecipientSession;
		}

		private void AcquireBudgetAndStartTiming()
		{
			ExTraceGlobals.SharingTracer.TraceDebug<SecurityIdentifier>((long)this.GetHashCode(), "Acquiring and check the budget for sid {0}.", this.sid);
			this.budget = StandardBudget.Acquire(this.sid, BudgetType.Anonymous, ADSessionSettings.FromRootOrgScopeSet());
			this.budget.CheckOverBudget();
			ExTraceGlobals.SharingTracer.TraceDebug<SecurityIdentifier>((long)this.GetHashCode(), "Start timing for sid {0}.", this.sid);
			PublishedFolder.SleepIfNecessary();
			string callerInfo = "PublishedFolder.AcquireBudgetAndStartTiming";
			this.budget.StartConnection(callerInfo);
			this.budget.StartLocal(callerInfo, default(TimeSpan));
		}

		private void ReleaseBudgetAndStopTiming()
		{
			if (this.budget != null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<SecurityIdentifier>((long)this.GetHashCode(), "Release the budget and stop timing for sid {0}.", this.sid);
				this.budget.Dispose();
				this.budget = null;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.InternalDispose(disposing);
				this.isDisposed = true;
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
				if (this.mailboxSession != null && this.disposeMailboxSession)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<SecurityIdentifier>((long)this.GetHashCode(), "Release the mailbox session sid {0}.", this.sid);
					this.mailboxSession.Dispose();
				}
				this.ReleaseBudgetAndStopTiming();
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PublishedFolder>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private readonly SecurityIdentifier sid;

		private readonly StoreObjectId folderId;

		private static readonly IntAppSettingsEntry PublishedFolderSlowdownCpuThreshold = new IntAppSettingsEntry("PublishedFolderSlowdownCpuThreshold", 25, ExTraceGlobals.SharingTracer);

		private MailboxSession mailboxSession;

		private IRecipientSession recipientSession;

		private DisposeTracker disposeTracker;

		private IStandardBudget budget;

		private string displayName;

		private bool disposeMailboxSession;

		private bool isDisposed;

		private ADSessionSettings sessionSettings;
	}
}
