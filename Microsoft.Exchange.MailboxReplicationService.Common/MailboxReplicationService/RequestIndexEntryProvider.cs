using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RequestIndexEntryProvider : IConfigDataProvider
	{
		public RequestIndexEntryProvider(IRecipientSession recipSession, IConfigurationSession configSession)
		{
			this.ownsSessions = false;
			this.recipSession = recipSession;
			this.configSession = configSession;
			this.domainController = recipSession.Source;
			this.orgId = OrganizationId.ForestWideOrgId;
			this.configSession.SessionSettings.IncludeSoftDeletedObjectLinks = true;
		}

		public RequestIndexEntryProvider()
		{
			this.ownsSessions = true;
			this.recipSession = null;
			this.configSession = null;
			this.domainController = null;
			this.orgId = OrganizationId.ForestWideOrgId;
		}

		public string Source
		{
			get
			{
				return this.RecipientSession.Source;
			}
		}

		internal IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipSession == null)
				{
					this.CreateSessions();
				}
				return this.recipSession;
			}
			set
			{
				this.recipSession = value;
			}
		}

		internal IConfigurationSession ConfigSession
		{
			get
			{
				if (this.configSession == null)
				{
					this.CreateSessions();
				}
				return this.configSession;
			}
			set
			{
				this.configSession = value;
				this.configSession.SessionSettings.IncludeSoftDeletedObjectLinks = true;
			}
		}

		internal string DomainController
		{
			get
			{
				return this.domainController;
			}
			set
			{
				this.domainController = value;
				this.configSession = null;
				this.recipSession = null;
			}
		}

		internal OrganizationId OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		internal bool OwnsSessions
		{
			get
			{
				return this.ownsSessions;
			}
		}

		public static void GetMoveGuids(ADUser adUser, out Guid mailboxGuid, out Guid mdbGuid)
		{
			mdbGuid = Guid.Empty;
			if (adUser != null)
			{
				mailboxGuid = adUser.ExchangeGuid;
				if (adUser.MailboxMoveStatus != RequestStatus.None && adUser.MailboxMoveFlags != RequestFlags.None)
				{
					if ((adUser.MailboxMoveFlags & RequestFlags.Pull) != RequestFlags.None)
					{
						if (adUser.MailboxMoveTargetMDB != null)
						{
							mdbGuid = adUser.MailboxMoveTargetMDB.ObjectGuid;
							return;
						}
						if (adUser.MailboxMoveTargetArchiveMDB != null)
						{
							mdbGuid = adUser.MailboxMoveTargetArchiveMDB.ObjectGuid;
						}
						return;
					}
					else if ((adUser.MailboxMoveFlags & RequestFlags.Push) != RequestFlags.None)
					{
						if (adUser.MailboxMoveSourceMDB != null)
						{
							mdbGuid = adUser.MailboxMoveSourceMDB.ObjectGuid;
							return;
						}
						if (adUser.MailboxMoveSourceArchiveMDB != null)
						{
							mdbGuid = adUser.MailboxMoveSourceArchiveMDB.ObjectGuid;
						}
						return;
					}
				}
			}
			else
			{
				mailboxGuid = Guid.Empty;
			}
		}

		public static void CreateAndPopulateRequestIndexEntries(RequestJobBase requestJobBase, RequestIndexId requestIndexId)
		{
			if (requestJobBase == null)
			{
				throw new ArgumentNullException("requestJobBase");
			}
			if (requestJobBase.IndexEntries == null)
			{
				requestJobBase.IndexEntries = new List<IRequestIndexEntry>();
			}
			if (requestJobBase.IndexIds == null)
			{
				requestJobBase.IndexIds = new List<RequestIndexId>();
			}
			RequestIndexEntryProvider.Handle(requestIndexId.RequestIndexEntryType, delegate(IRequestIndexEntryHandler handler)
			{
				IRequestIndexEntry item = handler.CreateRequestIndexEntryFromRequestJob(requestJobBase, requestIndexId);
				requestJobBase.IndexIds.Add(requestIndexId);
				requestJobBase.IndexEntries.Add(item);
			});
		}

		public static void CreateAndPopulateRequestIndexEntries(RequestJobBase requestJobBase, IConfigurationSession session)
		{
			if (requestJobBase == null)
			{
				throw new ArgumentNullException("requestJobBase");
			}
			if (requestJobBase.IndexEntries == null)
			{
				requestJobBase.IndexEntries = new List<IRequestIndexEntry>();
			}
			if (requestJobBase.IndexIds == null)
			{
				requestJobBase.IndexIds = new List<RequestIndexId>();
			}
			RequestIndexId indexId = new RequestIndexId(RequestIndexLocation.AD);
			RequestIndexEntryProvider.Handle(indexId.RequestIndexEntryType, delegate(IRequestIndexEntryHandler handler)
			{
				IRequestIndexEntry item = handler.CreateRequestIndexEntryFromRequestJob(requestJobBase, session);
				requestJobBase.IndexIds.Add(indexId);
				requestJobBase.IndexEntries.Add(item);
			});
		}

		public void Delete(IConfigurable instance)
		{
			IRequestIndexEntry requestIndexEntry = RequestIndexEntryProvider.ValidateInstance(instance);
			RequestIndexEntryProvider.Handle(requestIndexEntry.GetType(), delegate(IRequestIndexEntryHandler handler)
			{
				handler.Delete(this, requestIndexEntry);
			});
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			Type requestIndexEntryType = RequestIndexEntryProvider.ValidateQueryFilter<T>(filter);
			IRequestIndexEntry[] array = RequestIndexEntryProvider.Handle<IRequestIndexEntry[]>(requestIndexEntryType, (IRequestIndexEntryHandler handler) => handler.Find(this, filter, rootId, deepSearch, sortBy));
			if (array == null)
			{
				return Array<IConfigurable>.Empty;
			}
			Type typeFromHandle = typeof(T);
			if (array.GetType().GetElementType() == typeFromHandle)
			{
				return array.Cast<IConfigurable>().ToArray<IConfigurable>();
			}
			if (typeFromHandle.IsSubclassOf(typeof(RequestBase)))
			{
				return array.Select(new Func<IRequestIndexEntry, T>(RequestIndexEntryProvider.CreateRequest<T>)).Cast<IConfigurable>().ToArray<IConfigurable>();
			}
			MrsTracer.Common.Warning("IndexId not supported by this provider.", new object[0]);
			return Array<IConfigurable>.Empty;
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			Type requestIndexEntryType = RequestIndexEntryProvider.ValidateQueryFilter<T>(filter);
			IEnumerable<IRequestIndexEntry> enumerable = RequestIndexEntryProvider.Handle<IEnumerable<IRequestIndexEntry>>(requestIndexEntryType, (IRequestIndexEntryHandler handler) => handler.FindPaged(this, filter, rootId, deepSearch, sortBy, pageSize));
			if (enumerable == null)
			{
				return Array<T>.Empty;
			}
			if (typeof(T).IsSubclassOf(typeof(RequestBase)))
			{
				return enumerable.Select(new Func<IRequestIndexEntry, T>(RequestIndexEntryProvider.CreateRequest<T>));
			}
			if (enumerable is IEnumerable<T>)
			{
				return (IEnumerable<T>)enumerable;
			}
			MrsTracer.Common.Warning("IndexId not supported by this provider.", new object[0]);
			return Array<T>.Empty;
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			RequestIndexEntryObjectId requestIndexEntryObjectId = identity as RequestIndexEntryObjectId;
			if (requestIndexEntryObjectId == null)
			{
				throw new ArgumentException("This provider only supports RequestIndexEntryObjectIds", "identity");
			}
			Type typeFromHandle = typeof(T);
			if (!typeFromHandle.IsSubclassOf(typeof(RequestBase)) && !typeof(IRequestIndexEntry).IsAssignableFrom(typeFromHandle))
			{
				throw new ArgumentException("This provider only supports reading types of RequestBase or IRequestIndexEntry");
			}
			IRequestIndexEntry requestIndexEntry = this.Read(requestIndexEntryObjectId);
			if (requestIndexEntry == null || requestIndexEntry is T)
			{
				return requestIndexEntry;
			}
			if (!typeFromHandle.IsSubclassOf(typeof(RequestBase)))
			{
				return null;
			}
			return RequestIndexEntryProvider.CreateRequest<T>(requestIndexEntry);
		}

		public void Save(IConfigurable instance)
		{
			IRequestIndexEntry requestIndexEntry = RequestIndexEntryProvider.ValidateInstance(instance);
			RequestIndexEntryProvider.Handle(requestIndexEntry.GetType(), delegate(IRequestIndexEntryHandler handler)
			{
				handler.Save(this, requestIndexEntry);
			});
		}

		public IRequestIndexEntry Read(RequestIndexEntryObjectId objectId)
		{
			if (objectId.IndexId.RequestIndexEntryType == null)
			{
				return null;
			}
			return RequestIndexEntryProvider.Handle<IRequestIndexEntry>(objectId.IndexId.RequestIndexEntryType, (IRequestIndexEntryHandler handler) => handler.Read(this, objectId));
		}

		internal static T CreateRequest<T>(IRequestIndexEntry requestIndexEntry) where T : IConfigurable, new()
		{
			if (requestIndexEntry == null)
			{
				throw new ArgumentNullException("requestIndexEntry");
			}
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			RequestBase requestBase = t as RequestBase;
			if (requestBase == null)
			{
				throw new ArgumentException(string.Format("Type [{0}] not supported.  Must specify a derivative of RequestBase.", typeof(T).Name));
			}
			requestBase.Initialize(requestIndexEntry);
			return t;
		}

		internal T Read<T>(Func<IRecipientSession, T> function) where T : class
		{
			if (function == null)
			{
				throw new ArgumentNullException("function");
			}
			int num = 0;
			T result;
			try
			{
				IL_10:
				result = function(this.RecipientSession);
			}
			catch (LocalizedException ex)
			{
				if (ex is ADTransientException || ex is DomainControllerFromWrongDomainException)
				{
					if (num >= 2)
					{
						MrsTracer.Common.Error("Another error connecting to Domain Controller: '{0}' of type '{1}'. Failing.", new object[]
						{
							CommonUtils.FullExceptionMessage(ex),
							ex.GetType().ToString()
						});
						throw new UnableToReadADTransientException(ex);
					}
					string text = string.Empty;
					this.domainController = null;
					if (this.recipSession != null)
					{
						this.recipSession.DomainController = null;
						text = this.recipSession.Source;
					}
					MrsTracer.Common.Warning("Error connecting to Domain Controller ('{0}'): {1}, trying another.", new object[]
					{
						text ?? string.Empty,
						CommonUtils.FullExceptionMessage(ex)
					});
					num++;
					goto IL_10;
				}
				else
				{
					if (ex is ADOperationException)
					{
						MrsTracer.Common.Error("ADOperationException while talking to Domain Controller '{0}': '{1}'", new object[]
						{
							this.RecipientSession.Source,
							CommonUtils.FullExceptionMessage(ex)
						});
						throw new UnableToReadADPermanentException(ex);
					}
					throw;
				}
			}
			return result;
		}

		internal ADUser ReadADUser(ADObjectId userId, Guid exchangeGuid)
		{
			if (userId == null)
			{
				return null;
			}
			ADRecipient adrecipient = this.Read<ADRecipient>(delegate(IRecipientSession session)
			{
				if (CommonUtils.IsMultiTenantEnabled() && exchangeGuid != Guid.Empty && !userId.GetPartitionId().Equals(this.RecipientSession.SessionSettings.PartitionId))
				{
					return session.FindByExchangeGuidIncludingArchive(exchangeGuid);
				}
				return session.Read(userId);
			});
			if (adrecipient == null)
			{
				MrsTracer.Common.Warning("No ADRecipient found with Identity '{0}' in organizaton '{1}'.", new object[]
				{
					userId.ToString(),
					this.orgId.ToString()
				});
				return null;
			}
			ADUser aduser = adrecipient as ADUser;
			if (aduser == null)
			{
				MrsTracer.Common.Warning("'{0}' is not a user.", new object[]
				{
					userId.ToString()
				});
				return null;
			}
			return aduser;
		}

		internal void UpdateADData(ADUser user, TransactionalRequestJob requestJob, bool save)
		{
			if (save)
			{
				user.MailboxMoveStatus = RequestJobBase.GetVersionAppropriateStatus(requestJob.Status, user.ExchangeVersion);
				user.MailboxMoveFlags = RequestJobBase.GetVersionAppropriateFlags(requestJob.Flags, user.ExchangeVersion);
				user.MailboxMoveSourceMDB = requestJob.SourceDatabase;
				user.MailboxMoveTargetMDB = requestJob.TargetDatabase;
				user.MailboxMoveSourceArchiveMDB = requestJob.SourceArchiveDatabase;
				user.MailboxMoveTargetArchiveMDB = requestJob.TargetArchiveDatabase;
				user.MailboxMoveRemoteHostName = requestJob.RemoteHostName;
				user.MailboxMoveBatchName = requestJob.BatchName;
				return;
			}
			user.MailboxMoveStatus = RequestStatus.None;
			user.MailboxMoveFlags = RequestFlags.None;
			user.MailboxMoveSourceMDB = null;
			user.MailboxMoveTargetMDB = null;
			user.MailboxMoveSourceArchiveMDB = null;
			user.MailboxMoveTargetArchiveMDB = null;
			user.MailboxMoveRemoteHostName = null;
			user.MailboxMoveBatchName = null;
		}

		internal IDisposable RescopeTo(string domainController, OrganizationId orgId)
		{
			IDisposable result = new RequestIndexEntryProvider.ScopeRestorer(this);
			if (this.OwnsSessions)
			{
				if (!StringComparer.OrdinalIgnoreCase.Equals(this.domainController, domainController) || !this.orgId.Equals(orgId))
				{
					this.recipSession = null;
					this.configSession = null;
				}
			}
			else
			{
				if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.recipSession, orgId))
				{
					this.recipSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.recipSession, orgId, true);
				}
				if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.configSession, orgId))
				{
					this.configSession = (IConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(this.configSession, orgId, true);
				}
			}
			this.domainController = domainController;
			this.orgId = (orgId ?? OrganizationId.ForestWideOrgId);
			return result;
		}

		private static void Handle(Type requestIndexEntryType, Action<IRequestIndexEntryHandler> action)
		{
			if (requestIndexEntryType == null)
			{
				throw new ArgumentNullException("requestIndexEntryType");
			}
			IRequestIndexEntryHandler obj;
			if (RequestIndexEntryProvider.requestIndexEntryHandlers.TryGetValue(requestIndexEntryType, out obj))
			{
				action(obj);
			}
		}

		private static T Handle<T>(Type requestIndexEntryType, Func<IRequestIndexEntryHandler, T> function)
		{
			T returnValue = default(T);
			RequestIndexEntryProvider.Handle(requestIndexEntryType, delegate(IRequestIndexEntryHandler handler)
			{
				returnValue = function(handler);
			});
			return returnValue;
		}

		private static IRequestIndexEntry ValidateInstance(IConfigurable instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			IRequestIndexEntry requestIndexEntry = instance as IRequestIndexEntry;
			if (requestIndexEntry == null)
			{
				throw new ArgumentException(string.Format("This provider only supports IRequestIndexEntrys but was provided {0}.", instance.GetType().Name), "instance");
			}
			return requestIndexEntry;
		}

		private static Type ValidateQueryFilter<T>(QueryFilter filter) where T : IConfigurable
		{
			RequestIndexEntryQueryFilter requestIndexEntryQueryFilter = filter as RequestIndexEntryQueryFilter;
			if (filter != null && requestIndexEntryQueryFilter == null)
			{
				throw new ArgumentException("This provider only supports RequestIndexEntryQueryFilters", "filter");
			}
			Type result = typeof(T);
			if (requestIndexEntryQueryFilter != null)
			{
				result = requestIndexEntryQueryFilter.IndexId.RequestIndexEntryType;
			}
			return result;
		}

		private void CreateSessions()
		{
			MrsTracer.Common.Debug("Creating AD sessions for '{0}'", new object[]
			{
				this.OrgId
			});
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.orgId);
			adsessionSettings.IncludeInactiveMailbox = true;
			adsessionSettings.IncludeSoftDeletedObjects = true;
			this.recipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.domainController, false, ConsistencyMode.PartiallyConsistent, adsessionSettings, 854, "CreateSessions", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\MRSRequestProviders\\IndexProvider\\RequestIndexEntryProvider.cs");
			this.recipSession.EnforceDefaultScope = false;
			this.configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.domainController, false, ConsistencyMode.PartiallyConsistent, adsessionSettings, 862, "CreateSessions", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\MRSRequestProviders\\IndexProvider\\RequestIndexEntryProvider.cs");
			this.configSession.SessionSettings.IncludeSoftDeletedObjectLinks = true;
		}

		private static readonly Dictionary<Type, IRequestIndexEntryHandler> requestIndexEntryHandlers = new Dictionary<Type, IRequestIndexEntryHandler>
		{
			{
				typeof(MRSRequestWrapper),
				new ADHandler()
			},
			{
				typeof(AggregatedAccountConfigurationWrapper),
				new UserMailboxHandler<AggregatedAccountConfigurationWrapper>()
			},
			{
				typeof(MRSRequestMailboxEntry),
				new MailboxRequestIndexEntryHandler()
			},
			{
				typeof(AggregatedAccountListConfigurationWrapper),
				new UserMailboxHandler<AggregatedAccountListConfigurationWrapper>()
			}
		};

		private readonly bool ownsSessions;

		private IRecipientSession recipSession;

		private IConfigurationSession configSession;

		private string domainController;

		private OrganizationId orgId;

		private class ScopeRestorer : DisposeTrackableBase
		{
			public ScopeRestorer(RequestIndexEntryProvider owner)
			{
				this.ownerProvider = owner;
				this.savedRecipSession = owner.recipSession;
				this.savedConfigSession = owner.configSession;
				this.savedDomainController = owner.domainController;
				this.savedOrgId = owner.orgId;
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					this.ownerProvider.recipSession = this.savedRecipSession;
					this.ownerProvider.configSession = this.savedConfigSession;
					this.ownerProvider.orgId = this.savedOrgId;
					this.ownerProvider.domainController = this.savedDomainController;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<RequestIndexEntryProvider.ScopeRestorer>(this);
			}

			private readonly IRecipientSession savedRecipSession;

			private readonly IConfigurationSession savedConfigSession;

			private readonly string savedDomainController;

			private readonly OrganizationId savedOrgId;

			private readonly RequestIndexEntryProvider ownerProvider;
		}
	}
}
