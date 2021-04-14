using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderMailboxSynchronizerManager
	{
		public static PublicFolderMailboxSynchronizerManager Instance
		{
			get
			{
				return PublicFolderMailboxSynchronizerManager.singleton;
			}
		}

		public PublicFolderMailboxSynchronizerReference GetPublicFolderMailboxSynchronizer(IExchangePrincipal publicFolderMailboxPrincipal, bool onlyRefCounting, bool forHierarchyAccess)
		{
			PublicFolderMailboxSynchronizerReference result;
			lock (this.lockObject)
			{
				PublicFolderMailboxSynchronizerManager.PublicFolderMailboxSynchronizerAndReferenceCount publicFolderMailboxSynchronizerAndReferenceCount;
				if (!this.publicFolderMailboxSynchronizers.TryGetValue(publicFolderMailboxPrincipal.MailboxInfo.MailboxGuid, out publicFolderMailboxSynchronizerAndReferenceCount))
				{
					PublicFolderMailboxSynchronizer publicFolderMailboxSynchronizer2 = new PublicFolderMailboxSynchronizer(publicFolderMailboxPrincipal.MailboxInfo.OrganizationId, publicFolderMailboxPrincipal.MailboxInfo.MailboxGuid, publicFolderMailboxPrincipal.MailboxInfo.Location.ServerFqdn, onlyRefCounting);
					publicFolderMailboxSynchronizerAndReferenceCount = new PublicFolderMailboxSynchronizerManager.PublicFolderMailboxSynchronizerAndReferenceCount(publicFolderMailboxSynchronizer2);
					this.publicFolderMailboxSynchronizers[publicFolderMailboxPrincipal.MailboxInfo.MailboxGuid] = publicFolderMailboxSynchronizerAndReferenceCount;
				}
				publicFolderMailboxSynchronizerAndReferenceCount.ReferenceCount++;
				if (forHierarchyAccess)
				{
					publicFolderMailboxSynchronizerAndReferenceCount.HierarchyAccessReferenceCount++;
				}
				result = new PublicFolderMailboxSynchronizerReference(publicFolderMailboxSynchronizerAndReferenceCount.PublicFolderMailboxSynchronizer, delegate(PublicFolderMailboxSynchronizer publicFolderMailboxSynchronizer)
				{
					this.OnReferenceDisposed(publicFolderMailboxSynchronizer, forHierarchyAccess);
				});
			}
			return result;
		}

		public int GetActiveReferenceCount(IExchangePrincipal publicFolderMailboxPrincipal)
		{
			return this.publicFolderMailboxSynchronizers[publicFolderMailboxPrincipal.MailboxInfo.MailboxGuid].ReferenceCount;
		}

		public int GetActiveHierarchyAccessReferenceCount(IExchangePrincipal publicFolderMailboxPrincipal)
		{
			return this.publicFolderMailboxSynchronizers[publicFolderMailboxPrincipal.MailboxInfo.MailboxGuid].HierarchyAccessReferenceCount;
		}

		public ExDateTime GetAlertIssuedTime(IExchangePrincipal publicFolderMailboxPrincipal)
		{
			return this.publicFolderMailboxSynchronizers[publicFolderMailboxPrincipal.MailboxInfo.MailboxGuid].AlertIssuedTime;
		}

		public void SetAlertIssuedTime(IExchangePrincipal publicFolderMailboxPrincipal, ExDateTime alertTime)
		{
			this.publicFolderMailboxSynchronizers[publicFolderMailboxPrincipal.MailboxInfo.MailboxGuid].AlertIssuedTime = alertTime;
		}

		private void OnReferenceDisposed(PublicFolderMailboxSynchronizer publicFolderMailboxSynchronizer, bool forHierarchyAccess)
		{
			bool flag = false;
			lock (this.lockObject)
			{
				PublicFolderMailboxSynchronizerManager.PublicFolderMailboxSynchronizerAndReferenceCount publicFolderMailboxSynchronizerAndReferenceCount;
				if (!this.publicFolderMailboxSynchronizers.TryGetValue(publicFolderMailboxSynchronizer.MailboxGuid, out publicFolderMailboxSynchronizerAndReferenceCount) || publicFolderMailboxSynchronizerAndReferenceCount.ReferenceCount == 0)
				{
					throw new InvalidOperationException("The public folder mailbox synchronizer has already been removed. This should not happen. ReferenceCount = " + ((publicFolderMailboxSynchronizerAndReferenceCount == null) ? -1 : publicFolderMailboxSynchronizerAndReferenceCount.ReferenceCount));
				}
				publicFolderMailboxSynchronizerAndReferenceCount.ReferenceCount--;
				if (forHierarchyAccess)
				{
					publicFolderMailboxSynchronizerAndReferenceCount.HierarchyAccessReferenceCount--;
				}
				if (publicFolderMailboxSynchronizerAndReferenceCount.ReferenceCount == 0)
				{
					this.publicFolderMailboxSynchronizers.Remove(publicFolderMailboxSynchronizer.MailboxGuid);
					flag = true;
				}
			}
			if (flag)
			{
				publicFolderMailboxSynchronizer.Dispose();
			}
		}

		private static PublicFolderMailboxSynchronizerManager singleton = new PublicFolderMailboxSynchronizerManager();

		private Dictionary<Guid, PublicFolderMailboxSynchronizerManager.PublicFolderMailboxSynchronizerAndReferenceCount> publicFolderMailboxSynchronizers = new Dictionary<Guid, PublicFolderMailboxSynchronizerManager.PublicFolderMailboxSynchronizerAndReferenceCount>();

		private object lockObject = new object();

		private class PublicFolderMailboxSynchronizerAndReferenceCount
		{
			public PublicFolderMailboxSynchronizerAndReferenceCount(PublicFolderMailboxSynchronizer publicFolderMailboxSynchronizer)
			{
				this.PublicFolderMailboxSynchronizer = publicFolderMailboxSynchronizer;
			}

			public readonly PublicFolderMailboxSynchronizer PublicFolderMailboxSynchronizer;

			public int ReferenceCount;

			public int HierarchyAccessReferenceCount;

			public ExDateTime AlertIssuedTime = ExDateTime.MinValue;
		}
	}
}
