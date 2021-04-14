using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class UnifiedView : IDisposeTrackable, IDisposable
	{
		private UnifiedView(Trace tracer, CallContext callContext, Guid[] mailboxGuids, DistinguishedFolderIdName distinguishedFolderIdName)
		{
			this.tracer = tracer;
			this.disposeTracker = ((IDisposeTrackable)this).GetDisposeTracker();
			this.callContext = callContext;
			if (mailboxGuids == null)
			{
				throw new ArgumentNullException("mailboxGuids", "Parameters mailboxGuids must not be null.");
			}
			if (distinguishedFolderIdName == DistinguishedFolderIdName.none)
			{
				throw new ArgumentNullException("distinguishedFolderIdName", "Parameter distinguishedFolderIdName cannot be 'none' when parameter mailboxGuids is not null.");
			}
			this.mailboxGuids = mailboxGuids;
			this.distinguishedFolderIdName = distinguishedFolderIdName;
			this.defaultFolderType = IdConverter.GetDefaultFolderTypeFromDistinguishedFolderIdNameType(this.distinguishedFolderIdName.ToString());
			this.ValidateMailboxGuids();
			if (this.mailboxGuids.Length > 0)
			{
				this.accessingMailboxGuid = ((this.mailboxGuids.Length > 1) ? this.callContext.AccessingADUser.ExchangeGuid : this.mailboxGuids[0]);
			}
			this.unifiedSessionRequired = (this.MailboxGuids.Length > 1 || this.accessingMailboxGuid == this.callContext.AccessingADUser.ExchangeGuid);
			this.storeObjectIds = new LazyMember<StoreObjectId[]>(new InitializeLazyMember<StoreObjectId[]>(this.GetStoreObjectIds));
		}

		internal SearchFolder SearchFolder
		{
			get
			{
				this.CheckDisposed();
				return this.searchFolder;
			}
			private set
			{
				this.searchFolder = value;
			}
		}

		internal Guid[] MailboxGuids
		{
			get
			{
				this.CheckDisposed();
				return this.mailboxGuids;
			}
		}

		internal DefaultFolderType DefaultFolderType
		{
			get
			{
				this.CheckDisposed();
				return this.defaultFolderType;
			}
		}

		internal DistinguishedFolderIdName DistinguishedFolderIdName
		{
			get
			{
				this.CheckDisposed();
				return this.distinguishedFolderIdName;
			}
		}

		internal static string CreateSearchFolderParentFolderName(Guid[] mailboxGuids)
		{
			if (mailboxGuids == null)
			{
				throw new ArgumentNullException("mailboxGuids", "Parameters mailboxGuids must not be null.");
			}
			return string.Join<Guid>(" ", from guid in mailboxGuids
			orderby guid
			select guid).GetHashCode().ToString();
		}

		internal bool UnifiedViewScopeSpecified
		{
			get
			{
				this.CheckDisposed();
				return this.MailboxGuids.Length > 0;
			}
		}

		internal bool UnifiedSessionRequired
		{
			get
			{
				this.CheckDisposed();
				return this.unifiedSessionRequired;
			}
		}

		internal StoreObjectId[] StoreObjectIds
		{
			get
			{
				this.CheckDisposed();
				return this.storeObjectIds.Member;
			}
		}

		internal static UnifiedView Create(Trace tracer, CallContext callContext, Guid[] mailboxGuids, TargetFolderId parentFolder)
		{
			if (mailboxGuids == null)
			{
				return null;
			}
			DistinguishedFolderIdName distinguishedFolderIdName = DistinguishedFolderIdName.none;
			if (parentFolder != null)
			{
				DistinguishedFolderId distinguishedFolderId = parentFolder.BaseFolderId as DistinguishedFolderId;
				if (distinguishedFolderId != null)
				{
					distinguishedFolderIdName = distinguishedFolderId.Id;
				}
			}
			return new UnifiedView(tracer, callContext, mailboxGuids, distinguishedFolderIdName);
		}

		internal StoreObjectId GetSearchFolderParentFolderId(MailboxSession session)
		{
			this.CheckDisposed();
			StoreObjectId parentFolderId = null;
			using (Folder folder = Folder.Create(session, session.SafeGetDefaultFolderId(DefaultFolderType.Configuration), StoreObjectType.Folder, "UnifiedViews", CreateMode.OpenIfExists))
			{
				folder.Save();
				folder.Load();
				parentFolderId = folder.StoreObjectId;
			}
			StoreObjectId storeObjectId;
			using (Folder folder2 = Folder.Create(session, parentFolderId, StoreObjectType.Folder, UnifiedView.CreateSearchFolderParentFolderName(this.MailboxGuids), CreateMode.OpenIfExists))
			{
				folder2.Save();
				folder2.Load();
				storeObjectId = folder2.StoreObjectId;
			}
			return storeObjectId;
		}

		internal SearchFolder CreateOrOpenSearchFolder(MailboxSession session)
		{
			this.CheckDisposed();
			SearchFolder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				SearchFolder searchFolder = SearchFolder.Create(session, this.GetSearchFolderParentFolderId(session), this.DefaultFolderType.ToString(), CreateMode.OpenIfExists);
				disposeGuard.Add<SearchFolder>(searchFolder);
				searchFolder.Save();
				searchFolder.Load();
				SearchFolderCriteria searchFolderCriteria = null;
				try
				{
					searchFolderCriteria = searchFolder.GetSearchCriteria();
				}
				catch (ObjectNotInitializedException arg)
				{
					this.tracer.TraceDebug<ObjectNotInitializedException, DefaultFolderType>(-1L, "UnifiedView::CreateSearchFolder. Expected exception {0} which indicates the search folder criteria does not exist for search folder {1}. Will create new one.", arg, this.DefaultFolderType);
				}
				if (searchFolderCriteria == null)
				{
					QueryFilter searchQuery = UnifiedMailboxHelper.CreateQueryFilter(this.DefaultFolderType);
					searchFolderCriteria = new SearchFolderCriteria(searchQuery, this.StoreObjectIds);
					IAsyncResult asyncResult = searchFolder.BeginApplyContinuousSearch(searchFolderCriteria, null, null);
					if (asyncResult.AsyncWaitHandle.WaitOne(60000))
					{
						searchFolder.EndApplyContinuousSearch(asyncResult);
					}
					else
					{
						this.tracer.TraceDebug(-1L, "UnifiedView::CreateSearchFolder. Timeout expired waiting for the search folder to finish population.");
					}
				}
				disposeGuard.Success();
				result = searchFolder;
			}
			return result;
		}

		internal IdAndSession CreateIdAndSessionUsingSessionCache()
		{
			MailboxSession mailboxSessionByMailboxId = this.callContext.SessionCache.GetMailboxSessionByMailboxId(new MailboxId(this.accessingMailboxGuid), this.unifiedSessionRequired);
			return this.CreateIdAndSession(mailboxSessionByMailboxId);
		}

		internal IdAndSession CreateIdAndSession(MailboxSession mailboxSession)
		{
			this.CheckDisposed();
			StoreObjectId storeId;
			if (this.MailboxGuids.Length > 1)
			{
				this.SearchFolder = this.CreateOrOpenSearchFolder(mailboxSession);
				storeId = this.SearchFolder.StoreObjectId;
			}
			else
			{
				storeId = this.StoreObjectIds[0];
			}
			return new IdAndSession(storeId, mailboxSession);
		}

		internal static void UpdateConversationResponseShape(ConversationResponseShape shape)
		{
			List<PropertyPath> list = (shape.AdditionalProperties != null) ? shape.AdditionalProperties.ToList<PropertyPath>() : new List<PropertyPath>();
			foreach (PropertyUriEnum uri in UnifiedView.AdditionalProperties)
			{
				PropertyUri item = new PropertyUri(uri);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			shape.AdditionalProperties = list.ToArray();
		}

		private StoreObjectId[] GetStoreObjectIds()
		{
			MailboxSession[] mailboxSessions = (from mailboxGuid in this.MailboxGuids
			select this.callContext.SessionCache.GetMailboxSessionByMailboxId(new MailboxId(mailboxGuid), false)).ToArray<MailboxSession>();
			return UnifiedView.GetDefaultFolderIdsInUnifiedDefaultFolderView(mailboxSessions, this.DefaultFolderType);
		}

		private static StoreObjectId[] GetDefaultFolderIdsInUnifiedDefaultFolderView(MailboxSession[] mailboxSessions, DefaultFolderType defaultFolderType)
		{
			List<StoreObjectId> list = new List<StoreObjectId>();
			bool flag = UnifiedMailboxHelper.DefaultSearchFolderTypesSupportedForUnifiedViews.Contains(defaultFolderType);
			for (int i = 0; i < mailboxSessions.Length; i++)
			{
				MailboxSession mailboxSession = mailboxSessions[i];
				StoreObjectId storeObjectId = mailboxSession.SafeGetDefaultFolderId(defaultFolderType);
				if (storeObjectId.ObjectType == StoreObjectType.Folder)
				{
					list.Add(storeObjectId);
				}
				else
				{
					if (storeObjectId.ObjectType != StoreObjectType.SearchFolder || !flag)
					{
						throw new ArgumentException(string.Format("The default folder {0} is not supported for unified view.", defaultFolderType), "defaultFolderType");
					}
					if (mailboxSessions.Length == 1)
					{
						list.Add(storeObjectId);
					}
					else
					{
						list.AddRange((from defaultFolderTypeInSearchScope in UnifiedMailboxHelper.GetSearchScopeForDefaultSearchFolder(defaultFolderType)
						select mailboxSession.GetDefaultFolderId(defaultFolderTypeInSearchScope)).ToArray<StoreObjectId>());
					}
				}
			}
			return list.ToArray();
		}

		private void ValidateMailboxGuids()
		{
			if (this.mailboxGuids.Distinct<Guid>().Count<Guid>() != this.mailboxGuids.Length)
			{
				throw new ArgumentException("Array of mailbox GUIDs must not contain duplicates.");
			}
			ADUser accessingADUser = this.callContext.AccessingADUser;
			if (accessingADUser == null)
			{
				throw new CannotFindUserException(ResponseCodeType.ErrorCannotFindUser, CoreResources.ErrorUserADObjectNotFound);
			}
			Guid exchangeGuid = accessingADUser.ExchangeGuid;
			foreach (Guid guid in this.mailboxGuids)
			{
				if (guid != exchangeGuid)
				{
					accessingADUser.AggregatedMailboxGuids.Contains(guid);
				}
			}
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.searchFolder != null)
				{
					this.searchFolder.Dispose();
					this.searchFolder = null;
				}
				this.disposed = true;
			}
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UnifiedView>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		internal const int SearchTimeoutInMilliseconds = 60000;

		internal const string RootFolderNameForAllUnifiedViewSearchFolders = "UnifiedViews";

		internal const DefaultFolderType RootDeafultFolderTypeForAllUnifiedViewSearchFolders = DefaultFolderType.Configuration;

		internal static readonly PropertyUriEnum[] AdditionalProperties = new PropertyUriEnum[]
		{
			PropertyUriEnum.ConversationMailboxGuid
		};

		private readonly Guid[] mailboxGuids;

		private readonly Guid accessingMailboxGuid;

		private readonly bool unifiedSessionRequired;

		private readonly CallContext callContext;

		private bool disposed;

		private DisposeTracker disposeTracker;

		private Trace tracer;

		private SearchFolder searchFolder;

		private readonly DefaultFolderType defaultFolderType;

		private readonly DistinguishedFolderIdName distinguishedFolderIdName;

		private readonly LazyMember<StoreObjectId[]> storeObjectIds;
	}
}
