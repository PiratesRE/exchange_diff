using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	internal class MRSRequestWrapper : MRSRequest, IRequestIndexEntry, IConfigurable
	{
		public MRSRequestWrapper()
		{
		}

		internal MRSRequestWrapper(IConfigurationSession session, MRSRequestType type, string commonName)
		{
			base.RequestType = type;
			ADObjectId relativeContainerId = ADHandler.GetRelativeContainerId(type);
			base.SetId(session, relativeContainerId, commonName);
		}

		public Guid RequestGuid
		{
			get
			{
				return base.MailboxMoveRequestGuid;
			}
			set
			{
				base.MailboxMoveRequestGuid = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.DisplayName;
			}
			set
			{
				base.DisplayName = value;
			}
		}

		public RequestStatus Status
		{
			get
			{
				return base.MailboxMoveStatus;
			}
			set
			{
				base.MailboxMoveStatus = value;
			}
		}

		public RequestFlags Flags
		{
			get
			{
				return base.MailboxMoveFlags;
			}
			set
			{
				base.MailboxMoveFlags = value;
			}
		}

		public string RemoteHostName
		{
			get
			{
				return base.MailboxMoveRemoteHostName;
			}
			set
			{
				base.MailboxMoveRemoteHostName = value;
			}
		}

		public string BatchName
		{
			get
			{
				return base.MailboxMoveBatchName;
			}
			set
			{
				base.MailboxMoveBatchName = value;
			}
		}

		public ADObjectId SourceMDB
		{
			get
			{
				return base.MailboxMoveSourceMDB;
			}
			set
			{
				base.MailboxMoveSourceMDB = value;
			}
		}

		public ADObjectId TargetMDB
		{
			get
			{
				return base.MailboxMoveTargetMDB;
			}
			set
			{
				base.MailboxMoveTargetMDB = value;
			}
		}

		public ADObjectId StorageMDB
		{
			get
			{
				return base.MailboxMoveStorageMDB;
			}
			set
			{
				base.MailboxMoveStorageMDB = value;
			}
		}

		public string FilePath
		{
			get
			{
				return base.MailboxMoveFilePath;
			}
			set
			{
				base.MailboxMoveFilePath = value;
			}
		}

		public MRSRequestType Type
		{
			get
			{
				return base.RequestType;
			}
			set
			{
				base.RequestType = value;
			}
		}

		public ADObjectId TargetUserId
		{
			get
			{
				return base.MailboxMoveTargetUser;
			}
			set
			{
				base.MailboxMoveTargetUser = value;
			}
		}

		public ADObjectId SourceUserId
		{
			get
			{
				return base.MailboxMoveSourceUser;
			}
			set
			{
				base.MailboxMoveSourceUser = value;
			}
		}

		public RequestIndexId RequestIndexId
		{
			get
			{
				return MRSRequestWrapper.indexId;
			}
		}

		public RequestJobObjectId GetRequestJobId()
		{
			return new RequestJobObjectId(this.RequestGuid, (this.StorageMDB == null) ? Guid.Empty : this.StorageMDB.ObjectGuid, this);
		}

		public RequestIndexEntryObjectId GetRequestIndexEntryId(RequestBase owner)
		{
			return new RequestIndexEntryObjectId(this.RequestGuid, this.Type, base.OrganizationId, MRSRequestWrapper.indexId, owner);
		}

		DateTime? IRequestIndexEntry.get_WhenChanged()
		{
			return base.WhenChanged;
		}

		DateTime? IRequestIndexEntry.get_WhenCreated()
		{
			return base.WhenCreated;
		}

		DateTime? IRequestIndexEntry.get_WhenChangedUTC()
		{
			return base.WhenChangedUTC;
		}

		DateTime? IRequestIndexEntry.get_WhenCreatedUTC()
		{
			return base.WhenCreatedUTC;
		}

		private static RequestIndexId indexId = new RequestIndexId(RequestIndexLocation.AD);
	}
}
