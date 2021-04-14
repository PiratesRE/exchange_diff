using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TeamMailboxSyncInfo
	{
		public Guid MailboxGuid { get; private set; }

		public MailboxSession MailboxSession { get; set; }

		public string DisplayName { get; private set; }

		public Uri WebCollectionUrl { get; private set; }

		public Guid WebId { get; private set; }

		public string SiteUrl { get; private set; }

		public string PendingClientString
		{
			get
			{
				return this.pendingClientString;
			}
			set
			{
				this.pendingClientString = value;
			}
		}

		public ExDateTime PendingClientRequestTime { get; set; }

		public bool IsPending
		{
			get
			{
				return this.isPending;
			}
			set
			{
				this.isPending = value;
			}
		}

		public ExDateTime NextAllowedSyncUtcTime { get; set; }

		public ExDateTime LastSyncUtcTime { get; set; }

		public ExDateTime WhenCreatedUtcTime { get; private set; }

		public SortedList<ExDateTime, Exception> SyncErrors { get; private set; }

		public IResourceMonitor ResourceMonitor { get; private set; }

		public ExchangePrincipal MailboxPrincipal { get; private set; }

		public UserConfiguration Logger { get; private set; }

		public TeamMailboxLifecycleState LifeCycleState { get; private set; }

		public TeamMailboxSyncInfo(Guid mailboxGuid, TeamMailboxLifecycleState lifeCycleState, MailboxSession mailboxSession, ExchangePrincipal mailboxPrincipal, string displayName, Uri webCollectionUrl, Guid webId, string siteUrl, IResourceMonitor resourceMonitor, UserConfiguration logger)
		{
			this.MailboxGuid = mailboxGuid;
			this.LifeCycleState = lifeCycleState;
			this.MailboxSession = mailboxSession;
			this.DisplayName = displayName;
			this.WebCollectionUrl = webCollectionUrl;
			this.WebId = webId;
			this.SiteUrl = siteUrl;
			this.NextAllowedSyncUtcTime = ExDateTime.UtcNow;
			this.LastSyncUtcTime = ExDateTime.MinValue;
			this.SyncErrors = new SortedList<ExDateTime, Exception>();
			this.ResourceMonitor = resourceMonitor;
			this.MailboxPrincipal = mailboxPrincipal;
			this.Logger = logger;
			this.WhenCreatedUtcTime = ExDateTime.UtcNow;
		}

		private volatile bool isPending;

		private volatile string pendingClientString;
	}
}
