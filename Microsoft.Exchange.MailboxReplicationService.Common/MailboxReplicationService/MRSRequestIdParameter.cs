using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public abstract class MRSRequestIdParameter : IIdentityParameter
	{
		protected MRSRequestIdParameter()
		{
			this.indexToUse = null;
			this.indexIds = null;
			this.requestGuid = Guid.Empty;
			this.mailboxName = null;
			this.mailboxId = null;
			this.requestName = null;
			this.organizationId = null;
			this.organizationName = null;
			this.rawIdentity = null;
		}

		protected MRSRequestIdParameter(RequestBase request) : this((RequestIndexEntryObjectId)request.Identity)
		{
			this.organizationId = request.OrganizationId;
		}

		protected MRSRequestIdParameter(RequestJobObjectId requestJobId)
		{
			if (requestJobId == null)
			{
				throw new ArgumentNullException("requestJobId");
			}
			if (requestJobId.RequestGuid == Guid.Empty)
			{
				throw new ArgumentException(MrsStrings.InvalidRequestJob);
			}
			this.indexToUse = null;
			this.indexIds = null;
			this.requestGuid = requestJobId.RequestGuid;
			this.mailboxName = null;
			this.mailboxId = null;
			this.requestName = null;
			this.organizationId = null;
			this.organizationName = null;
			this.rawIdentity = null;
		}

		protected MRSRequestIdParameter(RequestStatisticsBase requestStats)
		{
			if (requestStats == null)
			{
				throw new ArgumentNullException("requestStats");
			}
			if (requestStats.RequestType != this.RequestType)
			{
				throw new ArgumentException(MrsStrings.ImproperTypeForThisIdParameter, "requestStats");
			}
			this.indexToUse = null;
			this.indexIds = requestStats.IndexIds;
			this.requestGuid = requestStats.RequestGuid;
			this.mailboxName = null;
			this.mailboxId = null;
			this.requestName = null;
			this.organizationId = requestStats.OrganizationId;
			this.organizationName = null;
			this.rawIdentity = null;
		}

		protected MRSRequestIdParameter(RequestIndexEntryObjectId identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (identity.RequestType != this.RequestType)
			{
				throw new ArgumentException(MrsStrings.ImproperTypeForThisIdParameter, "identity");
			}
			this.indexToUse = identity.IndexId;
			this.indexIds = null;
			this.requestGuid = identity.RequestGuid;
			this.mailboxName = null;
			this.mailboxId = null;
			this.requestName = null;
			this.organizationId = identity.OrganizationId;
			this.organizationName = null;
			this.rawIdentity = null;
		}

		protected MRSRequestIdParameter(Guid guid)
		{
			this.requestGuid = guid;
			this.mailboxName = null;
			this.mailboxId = null;
			this.requestName = null;
			this.indexToUse = null;
			this.indexIds = null;
			this.organizationId = null;
			this.organizationName = null;
			this.rawIdentity = null;
		}

		protected MRSRequestIdParameter(string request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (request.Equals(string.Empty))
			{
				throw new ArgumentException(MrsStrings.MustProvideNonEmptyStringForIdentity);
			}
			if (request.Contains("\\"))
			{
				int num = request.LastIndexOf('\\');
				string g = request.Substring(num + 1);
				Guid guid;
				if (GuidHelper.TryParseGuid(g, out guid))
				{
					this.mailboxName = null;
					this.requestGuid = guid;
					this.mailboxId = null;
					this.requestName = null;
					this.organizationName = request.Substring(0, num);
				}
				else
				{
					this.mailboxName = request.Substring(0, num);
					this.requestGuid = Guid.Empty;
					this.mailboxId = null;
					this.requestName = g;
					this.organizationName = null;
				}
				this.indexToUse = null;
				this.indexIds = null;
			}
			else
			{
				Guid guid;
				if (!GuidHelper.TryParseGuid(request, out guid))
				{
					throw new ArgumentException(MrsStrings.IdentityWasNotInValidFormat(request));
				}
				this.requestGuid = guid;
				this.mailboxName = null;
				this.mailboxId = null;
				this.requestName = null;
				this.indexToUse = null;
				this.indexIds = null;
				this.organizationName = null;
			}
			this.organizationId = null;
			this.rawIdentity = request;
		}

		protected MRSRequestIdParameter(Guid requestGuid, OrganizationId orgId, string mailboxName)
		{
			this.indexToUse = null;
			this.indexIds = null;
			this.requestGuid = requestGuid;
			this.mailboxName = mailboxName;
			this.mailboxId = null;
			this.requestName = null;
			this.organizationId = orgId;
			this.organizationName = null;
			this.rawIdentity = mailboxName + "\\" + requestGuid;
		}

		public string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		internal RequestIndexId IndexToUse
		{
			get
			{
				return this.indexToUse;
			}
		}

		internal List<RequestIndexId> IndexIds
		{
			get
			{
				return this.indexIds;
			}
		}

		internal string MailboxName
		{
			get
			{
				return this.mailboxName;
			}
			set
			{
				this.mailboxName = value;
			}
		}

		internal string OrganizationName
		{
			get
			{
				return this.organizationName;
			}
			set
			{
				this.organizationName = value;
			}
		}

		internal OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		internal Guid RequestGuid
		{
			get
			{
				return this.requestGuid;
			}
		}

		internal ADObjectId MailboxId
		{
			get
			{
				return this.mailboxId;
			}
			set
			{
				this.mailboxId = value;
			}
		}

		internal MRSRequestType RequestType
		{
			get
			{
				return MRSRequestIdParameter.GetRequestType(base.GetType());
			}
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			if (!typeof(T).Equals(typeof(IRequestIndexEntry)) && !typeof(T).Equals(typeof(RequestBase)) && !typeof(IRequestIndexEntry).IsAssignableFrom(typeof(T)) && !typeof(RequestBase).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(MrsStrings.ImproperTypeForThisIdParameter);
			}
			return this.InternalGetObjects<T>(rootId, session, optionalData, out notFoundReason);
		}

		public void Initialize(ObjectId objectId)
		{
			RequestIndexEntryObjectId requestIndexEntryObjectId = objectId as RequestIndexEntryObjectId;
			if (requestIndexEntryObjectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (requestIndexEntryObjectId.RequestGuid == Guid.Empty || requestIndexEntryObjectId.IndexId == null)
			{
				throw new ArgumentException(MrsStrings.InitializedWithInvalidObjectId);
			}
			this.requestGuid = requestIndexEntryObjectId.RequestGuid;
			this.indexToUse = requestIndexEntryObjectId.IndexId;
		}

		public override string ToString()
		{
			if (this.RawIdentity != null)
			{
				return this.RawIdentity;
			}
			return string.Format("{0}", this.requestGuid);
		}

		internal static MRSRequestType GetRequestType<T>() where T : MRSRequestIdParameter
		{
			return MRSRequestIdParameter.GetRequestType(typeof(T));
		}

		internal IEnumerable<T> InternalGetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			RequestIndexEntryProvider requestIndexEntryProvider = session as RequestIndexEntryProvider;
			if (requestIndexEntryProvider == null)
			{
				throw new ArgumentException(MrsStrings.MustProvideValidSessionForFindingRequests);
			}
			if (this.requestGuid != Guid.Empty && this.indexToUse != null)
			{
				List<T> list = new List<T>(1);
				RequestIndexEntryObjectId identity = new RequestIndexEntryObjectId(this.requestGuid, this.RequestType, this.OrganizationId, this.indexToUse, null);
				T t = (T)((object)requestIndexEntryProvider.Read<T>(identity));
				if (t != null)
				{
					list.Add(t);
					notFoundReason = null;
				}
				else
				{
					notFoundReason = new LocalizedString?(MrsStrings.NoSuchRequestInSpecifiedIndex);
				}
				return list;
			}
			if (string.IsNullOrEmpty(this.requestName) || this.indexToUse == null)
			{
				notFoundReason = new LocalizedString?(MrsStrings.NotEnoughInformationSupplied);
				return new List<T>(0);
			}
			if (this.mailboxId != null)
			{
				QueryFilter filter = new RequestIndexEntryQueryFilter(this.requestName, this.mailboxId, this.RequestType, this.indexToUse, true);
				notFoundReason = new LocalizedString?(MrsStrings.NoSuchRequestInSpecifiedIndex);
				return requestIndexEntryProvider.FindPaged<T>(filter, rootId, true, null, 2);
			}
			QueryFilter filter2 = new RequestIndexEntryQueryFilter(this.requestName, null, this.RequestType, this.indexToUse, false);
			notFoundReason = new LocalizedString?(MrsStrings.NoSuchRequestInSpecifiedIndex);
			return requestIndexEntryProvider.FindPaged<T>(filter2, rootId, true, null, 2);
		}

		internal void SetDefaultIndex(RequestIndexId index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			if (this.indexToUse == null)
			{
				this.indexToUse = index;
			}
		}

		internal void SetSpecifiedIndex(RequestIndexId index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			this.indexToUse = index;
		}

		private static MRSRequestType GetRequestType(Type type)
		{
			return MRSRequestIdParameter.RequestTypes[type];
		}

		private static readonly Dictionary<Type, MRSRequestType> RequestTypes = new Dictionary<Type, MRSRequestType>
		{
			{
				typeof(MailboxExportRequestIdParameter),
				MRSRequestType.MailboxExport
			},
			{
				typeof(MailboxImportRequestIdParameter),
				MRSRequestType.MailboxImport
			},
			{
				typeof(MailboxRelocationRequestIdParameter),
				MRSRequestType.MailboxRelocation
			},
			{
				typeof(MailboxRestoreRequestIdParameter),
				MRSRequestType.MailboxRestore
			},
			{
				typeof(MergeRequestIdParameter),
				MRSRequestType.Merge
			},
			{
				typeof(PublicFolderMigrationRequestIdParameter),
				MRSRequestType.PublicFolderMigration
			},
			{
				typeof(PublicFolderMailboxMigrationRequestIdParameter),
				MRSRequestType.PublicFolderMailboxMigration
			},
			{
				typeof(PublicFolderMoveRequestIdParameter),
				MRSRequestType.PublicFolderMove
			},
			{
				typeof(FolderMoveRequestIdParameter),
				MRSRequestType.FolderMove
			},
			{
				typeof(SyncRequestIdParameter),
				MRSRequestType.Sync
			}
		};

		private RequestIndexId indexToUse;

		private List<RequestIndexId> indexIds;

		private Guid requestGuid;

		private string mailboxName;

		private ADObjectId mailboxId;

		private string organizationName;

		private readonly string requestName;

		private OrganizationId organizationId;

		private readonly string rawIdentity;
	}
}
