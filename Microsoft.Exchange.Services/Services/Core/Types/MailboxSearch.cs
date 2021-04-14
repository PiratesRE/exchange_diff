using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MailboxSearch : IDisposable
	{
		internal MailboxSearch(string mailboxId, bool isArchiveMailbox, OrganizationId orgId)
		{
			this.Initialize(mailboxId, isArchiveMailbox, orgId);
		}

		internal SearchMailboxCriteria SearchCriteria
		{
			get
			{
				return this.searchMailboxCriteria;
			}
		}

		internal SearchMailboxResult SearchResult
		{
			get
			{
				if (this.searchMailboxWorker == null)
				{
					return null;
				}
				return this.searchMailboxWorker.SearchResult;
			}
		}

		internal OrganizationId OrganizationId
		{
			get
			{
				return this.exchangePrincipal.MailboxInfo.OrganizationId;
			}
		}

		internal void CreateSearchMailboxCriteria(CultureInfo language, MultiValuedProperty<string> senders, MultiValuedProperty<string> recipients, DateTime startDate, DateTime endDate, MultiValuedProperty<KindKeyword> messageTypes, string userQuery, bool searchDumpster, bool includeUnsearchableItems, bool includePersonalArchive)
		{
			if (this.searchMailboxCriteria == null)
			{
				SearchObject searchObject = new SearchObject();
				searchObject.Language = language;
				searchObject.Senders = senders;
				searchObject.Recipients = recipients;
				if (!startDate.Equals(DateTime.MinValue))
				{
					searchObject.StartDate = new ExDateTime?((ExDateTime)startDate.ToLocalTime());
				}
				if (!endDate.Equals(DateTime.MinValue))
				{
					searchObject.EndDate = new ExDateTime?((ExDateTime)endDate.ToLocalTime());
				}
				searchObject.MessageTypes = messageTypes;
				searchObject.SearchQuery = userQuery;
				SearchUser searchUser = new SearchUser(this.exchangePrincipal.ObjectId, this.exchangePrincipal.MailboxInfo.DisplayName, this.exchangePrincipal.MailboxInfo.Location.ServerFqdn);
				this.searchMailboxCriteria = new SearchMailboxCriteria(searchObject.Language, searchObject.AqsQuery, searchObject.SearchQuery, new SearchUser[]
				{
					searchUser
				});
				this.searchMailboxCriteria.SearchDumpster = searchDumpster;
				this.searchMailboxCriteria.IncludeUnsearchableItems = includeUnsearchableItems;
				this.searchMailboxCriteria.IncludePersonalArchive = includePersonalArchive;
				this.searchMailboxCriteria.ResolveQueryFilter(null, null);
				this.searchMailboxCriteria.GenerateSubQueryFilters(null, null);
				this.folderScope = this.searchMailboxCriteria.GetFolderScope(this.mailboxSession);
			}
		}

		internal void CreateSearchFolderAndPerformInitialEstimation()
		{
			this.CreateSearchMailboxWorkerInstance();
			this.searchFolder = this.searchMailboxWorker.CreateSearchFolder(this.mailboxSession, ref this.dumpsterFolders, ref this.resultFolders);
			this.searchHit = new KeywordHit
			{
				Phrase = this.searchMailboxCriteria.UserQuery
			};
			this.searchMailboxWorker.PerformInitialEstimation(this.mailboxSession, this.resultFolders, ref this.searchHit);
		}

		internal void PerformSingleKeywordSearch(string keyword)
		{
			this.searchMailboxWorker.PerformSingleKeywordEstimationSearch(this.mailboxSession, this.searchFolder, this.folderScope, this.resultFolders, keyword, ref this.searchHit);
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				if (this.searchMailboxCriteria != null)
				{
					this.searchMailboxCriteria = null;
				}
				if (this.searchMailboxWorker != null)
				{
					this.searchMailboxWorker = null;
				}
				if (this.dumpsterFolders != null)
				{
					this.dumpsterFolders.ForEach(delegate(Folder folder)
					{
						folder.Dispose();
					});
					this.dumpsterFolders.Clear();
					this.dumpsterFolders = null;
				}
				if (this.searchFolder != null)
				{
					StoreId id = this.searchFolder.Id;
					this.searchFolder.Dispose();
					this.mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
					{
						id
					});
					this.searchFolder = null;
				}
				if (this.mailboxSession != null)
				{
					this.mailboxSession.Dispose();
					this.mailboxSession = null;
				}
				if (this.exchangePrincipal != null)
				{
					this.exchangePrincipal = null;
				}
				this.isDisposed = true;
			}
		}

		private void Initialize(string id, bool isArchiveMailbox, OrganizationId orgId)
		{
			bool flag = false;
			do
			{
				RemotingOptions remotingOptions = flag ? RemotingOptions.AllowCrossSite : RemotingOptions.LocalConnectionsOnly;
				if (isArchiveMailbox)
				{
					this.exchangePrincipal = ExchangePrincipal.FromMailboxGuid(orgId.ToADSessionSettings(), new Guid(id), remotingOptions, null);
				}
				else
				{
					this.exchangePrincipal = ExchangePrincipal.FromProxyAddress(orgId.ToADSessionSettings(), id, remotingOptions);
				}
				try
				{
					this.mailboxSession = MailboxSession.OpenAsSystemService(this.exchangePrincipal, CultureInfo.InvariantCulture, "Client=EDiscoverySearch;Action=Search;Interactive=False");
					break;
				}
				catch (WrongServerException)
				{
					ExTraceGlobals.SearchTracer.TraceDebug<bool>(0L, "FindMailboxStatisticsByKeywords EWS call from [Hybrid E14->E15(arbitration)->E15(mailbox)] Failed with WrongServerException. retryByAllowingCrossSite={0}", flag);
					if (flag)
					{
						throw;
					}
					flag = true;
				}
			}
			while (flag);
		}

		private void CreateSearchMailboxWorkerInstance()
		{
			if (this.searchMailboxWorker == null)
			{
				this.searchMailboxWorker = new SearchMailboxWorker(this.searchMailboxCriteria, 0);
			}
		}

		private bool isDisposed;

		private SearchMailboxWorker searchMailboxWorker;

		private SearchMailboxCriteria searchMailboxCriteria;

		private ExchangePrincipal exchangePrincipal;

		private MailboxSession mailboxSession;

		private StoreId[] folderScope;

		private SearchFolder searchFolder;

		private Folder[] resultFolders;

		private List<Folder> dumpsterFolders;

		private KeywordHit searchHit;
	}
}
