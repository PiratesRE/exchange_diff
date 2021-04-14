using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class RequestIndexEntryQueryFilter : QueryFilter
	{
		public RequestIndexEntryQueryFilter()
		{
			this.requestGuid = Guid.Empty;
			this.requestQueueId = null;
			this.requestType = MRSRequestType.Move;
			this.requestName = null;
			this.mailboxId = null;
			this.dbId = null;
			this.looseMailboxSearch = false;
			this.wildcardedNameSearch = false;
			this.indexId = null;
			this.batchName = null;
			this.sourceMailbox = null;
			this.targetMailbox = null;
			this.sourceDatabase = null;
			this.targetDatabase = null;
			this.status = RequestStatus.None;
			this.flags = RequestFlags.None;
			this.notFlags = RequestFlags.None;
		}

		public RequestIndexEntryQueryFilter(RequestIndexEntryObjectId requestIndexEntryId)
		{
			this.requestGuid = requestIndexEntryId.RequestGuid;
			this.requestQueueId = null;
			this.requestType = requestIndexEntryId.RequestType;
			this.indexId = requestIndexEntryId.IndexId;
			this.requestName = null;
			this.mailboxId = null;
			this.dbId = null;
			this.looseMailboxSearch = false;
			this.wildcardedNameSearch = false;
			this.batchName = null;
			this.sourceMailbox = null;
			this.targetMailbox = null;
			this.sourceDatabase = null;
			this.targetDatabase = null;
			this.status = RequestStatus.None;
			this.flags = RequestFlags.None;
			this.notFlags = RequestFlags.None;
		}

		public RequestIndexEntryQueryFilter(string name, ADObjectId id, MRSRequestType type, RequestIndexId idx, bool mbxSearch)
		{
			this.requestName = name;
			this.mailboxId = (mbxSearch ? id : null);
			this.dbId = (mbxSearch ? null : id);
			this.looseMailboxSearch = false;
			this.wildcardedNameSearch = false;
			this.requestType = type;
			this.indexId = idx;
			this.requestGuid = Guid.Empty;
			this.requestQueueId = null;
			this.batchName = null;
			this.sourceMailbox = null;
			this.targetMailbox = null;
			this.sourceDatabase = null;
			this.status = RequestStatus.None;
			this.flags = RequestFlags.None;
			this.notFlags = RequestFlags.None;
		}

		public Guid RequestGuid
		{
			get
			{
				return this.requestGuid;
			}
			internal set
			{
				this.requestGuid = value;
			}
		}

		public ADObjectId RequestQueueId
		{
			get
			{
				return this.requestQueueId;
			}
			internal set
			{
				this.requestQueueId = value;
			}
		}

		public MRSRequestType RequestType
		{
			get
			{
				return this.requestType;
			}
			internal set
			{
				this.requestType = value;
			}
		}

		public string RequestName
		{
			get
			{
				return this.requestName;
			}
			internal set
			{
				this.requestName = value;
			}
		}

		public ADObjectId MailboxId
		{
			get
			{
				return this.mailboxId;
			}
			internal set
			{
				this.mailboxId = value;
			}
		}

		public bool LooseMailboxSearch
		{
			get
			{
				return this.looseMailboxSearch;
			}
			internal set
			{
				this.looseMailboxSearch = value;
			}
		}

		public bool WildcardedNameSearch
		{
			get
			{
				return this.wildcardedNameSearch;
			}
			internal set
			{
				this.wildcardedNameSearch = value;
			}
		}

		public string BatchName
		{
			get
			{
				return this.batchName;
			}
			internal set
			{
				this.batchName = value;
			}
		}

		public ADObjectId SourceMailbox
		{
			get
			{
				return this.sourceMailbox;
			}
			internal set
			{
				this.sourceMailbox = value;
			}
		}

		public ADObjectId TargetMailbox
		{
			get
			{
				return this.targetMailbox;
			}
			internal set
			{
				this.targetMailbox = value;
			}
		}

		public ADObjectId DBId
		{
			get
			{
				return this.dbId;
			}
			internal set
			{
				this.dbId = value;
			}
		}

		public ADObjectId SourceDatabase
		{
			get
			{
				return this.sourceDatabase;
			}
			internal set
			{
				this.sourceDatabase = value;
			}
		}

		public ADObjectId TargetDatabase
		{
			get
			{
				return this.targetDatabase;
			}
			internal set
			{
				this.targetDatabase = value;
			}
		}

		public RequestStatus Status
		{
			get
			{
				return this.status;
			}
			internal set
			{
				this.status = value;
			}
		}

		public RequestFlags Flags
		{
			get
			{
				return this.flags;
			}
			internal set
			{
				this.flags = value;
			}
		}

		public RequestFlags NotFlags
		{
			get
			{
				return this.notFlags;
			}
			internal set
			{
				this.notFlags = value;
			}
		}

		public RequestIndexId IndexId
		{
			get
			{
				return this.indexId;
			}
			internal set
			{
				this.indexId = value;
			}
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append("type=");
			sb.Append(this.requestType.ToString());
			if (this.requestGuid != Guid.Empty)
			{
				sb.Append(",requestGuid=");
				sb.Append(this.requestGuid.ToString());
			}
			if (this.requestQueueId != null)
			{
				sb.Append(",requestQueueId=");
				sb.Append(this.requestQueueId.ToString());
			}
			if (!string.IsNullOrEmpty(this.requestName))
			{
				sb.Append(",requestName=");
				sb.Append(this.requestName);
			}
			if (this.indexId != null)
			{
				sb.Append(",index=");
				sb.Append(this.indexId.ToString());
			}
			if (this.mailboxId != null)
			{
				sb.Append(",mailbox=");
				sb.Append(this.mailboxId.ToString());
				sb.Append(",search=");
				sb.Append(this.looseMailboxSearch ? "loose" : "strict");
			}
			if (this.batchName != null)
			{
				sb.Append(",batchName=");
				sb.Append(this.batchName);
			}
			if (this.sourceMailbox != null)
			{
				sb.Append(",sourceMailbox=");
				sb.Append(this.sourceMailbox.ToString());
			}
			if (this.targetMailbox != null)
			{
				sb.Append(",targetMailbox=");
				sb.Append(this.targetMailbox.ToString());
			}
			if (this.dbId != null)
			{
				sb.Append(",database=");
				sb.Append(this.dbId.ToString());
			}
			if (this.sourceDatabase != null)
			{
				sb.Append(",sourceDatabase=");
				sb.Append(this.sourceDatabase.ToString());
			}
			if (this.targetDatabase != null)
			{
				sb.Append(",targetDatabase=");
				sb.Append(this.targetDatabase.ToString());
			}
			if (this.status != RequestStatus.None)
			{
				sb.Append(",status=");
				sb.Append(this.status.ToString());
			}
			if (this.flags != RequestFlags.None)
			{
				sb.Append(",flags contains ");
				sb.Append(this.flags.ToString());
			}
			if (this.notFlags != RequestFlags.None)
			{
				sb.Append(",flags don't contain ");
				sb.Append(this.notFlags.ToString());
			}
			sb.Append(")");
		}

		private Guid requestGuid;

		private ADObjectId requestQueueId;

		private MRSRequestType requestType;

		private string requestName;

		private ADObjectId mailboxId;

		private ADObjectId dbId;

		private bool looseMailboxSearch;

		private bool wildcardedNameSearch;

		private string batchName;

		private ADObjectId sourceMailbox;

		private ADObjectId targetMailbox;

		private ADObjectId sourceDatabase;

		private ADObjectId targetDatabase;

		private RequestStatus status;

		private RequestFlags flags;

		private RequestFlags notFlags;

		private RequestIndexId indexId;
	}
}
