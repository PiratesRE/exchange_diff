using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SiteMailboxSynchronizerManager
	{
		private SiteMailboxSynchronizerManager()
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox", "SyncManagerInterval", 600, (int x) => x > 0));
			this.disposeUnusedSiteMailboxSynchronizerTimer = new Timer(new TimerCallback(this.DisposeUnusedSiteMailboxSynchronizer), null, timeSpan, timeSpan);
		}

		private void DisposeUnusedSiteMailboxSynchronizer(object state)
		{
			lock (this.lockObject)
			{
				List<Guid> list = new List<Guid>();
				foreach (KeyValuePair<Guid, SiteMailboxSynchronizerManager.SiteMailboxSynchronizerAndReferenceCount> keyValuePair in this.siteFolderMailboxSynchronizers)
				{
					if (keyValuePair.Value.ReferenceCount <= 0 && keyValuePair.Value.SiteMailboxSynchronizer.TryToDispose())
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (Guid key in list)
				{
					this.siteFolderMailboxSynchronizers.Remove(key);
				}
			}
		}

		public static SiteMailboxSynchronizerManager Instance
		{
			get
			{
				return SiteMailboxSynchronizerManager.singleton;
			}
		}

		public SiteMailboxSynchronizerReference GetSiteMailboxSynchronizer(IExchangePrincipal siteMailboxPrincipal, string client)
		{
			SiteMailboxSynchronizerReference result;
			lock (this.lockObject)
			{
				SiteMailboxSynchronizerManager.SiteMailboxSynchronizerAndReferenceCount siteMailboxSynchronizerAndReferenceCount;
				if (!this.siteFolderMailboxSynchronizers.TryGetValue(siteMailboxPrincipal.MailboxInfo.MailboxGuid, out siteMailboxSynchronizerAndReferenceCount))
				{
					SiteMailboxSynchronizer siteMailboxSynchronizer = new SiteMailboxSynchronizer(siteMailboxPrincipal, client);
					siteMailboxSynchronizerAndReferenceCount = new SiteMailboxSynchronizerManager.SiteMailboxSynchronizerAndReferenceCount(siteMailboxSynchronizer);
					this.siteFolderMailboxSynchronizers[siteMailboxPrincipal.MailboxInfo.MailboxGuid] = siteMailboxSynchronizerAndReferenceCount;
				}
				siteMailboxSynchronizerAndReferenceCount.ReferenceCount++;
				result = new SiteMailboxSynchronizerReference(siteMailboxSynchronizerAndReferenceCount.SiteMailboxSynchronizer, new Action<SiteMailboxSynchronizer>(this.OnReferenceDisposed));
			}
			return result;
		}

		private void OnReferenceDisposed(SiteMailboxSynchronizer siteMailboxSynchronizer)
		{
			lock (this.lockObject)
			{
				SiteMailboxSynchronizerManager.SiteMailboxSynchronizerAndReferenceCount siteMailboxSynchronizerAndReferenceCount;
				if (!this.siteFolderMailboxSynchronizers.TryGetValue(siteMailboxSynchronizer.MailboxGuid, out siteMailboxSynchronizerAndReferenceCount) || siteMailboxSynchronizerAndReferenceCount.ReferenceCount == 0)
				{
					throw new InvalidOperationException(("The site mailbox synchronizer is already been removed. This should not happen. ReferenceCount = " + siteMailboxSynchronizerAndReferenceCount != null) ? siteMailboxSynchronizerAndReferenceCount.ReferenceCount.ToString() : "Null");
				}
				siteMailboxSynchronizerAndReferenceCount.ReferenceCount--;
			}
		}

		private const string RegKeySiteMailbox = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox";

		private const string RegValueSyncManagerInterval = "SyncManagerInterval";

		private static readonly SiteMailboxSynchronizerManager singleton = new SiteMailboxSynchronizerManager();

		private readonly Timer disposeUnusedSiteMailboxSynchronizerTimer;

		private readonly Dictionary<Guid, SiteMailboxSynchronizerManager.SiteMailboxSynchronizerAndReferenceCount> siteFolderMailboxSynchronizers = new Dictionary<Guid, SiteMailboxSynchronizerManager.SiteMailboxSynchronizerAndReferenceCount>();

		private readonly object lockObject = new object();

		private class SiteMailboxSynchronizerAndReferenceCount
		{
			public SiteMailboxSynchronizerAndReferenceCount(SiteMailboxSynchronizer siteMailboxSynchronizer)
			{
				this.SiteMailboxSynchronizer = siteMailboxSynchronizer;
			}

			public readonly SiteMailboxSynchronizer SiteMailboxSynchronizer;

			public int ReferenceCount;
		}
	}
}
