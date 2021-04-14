using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public abstract class RequestBase : ConfigurableObject
	{
		public RequestBase() : base(new SimpleProviderPropertyBag())
		{
		}

		internal RequestBase(IRequestIndexEntry index) : this()
		{
			this.Initialize(index);
		}

		public string Name
		{
			get
			{
				return (string)this[RequestSchema.Name];
			}
			private set
			{
				this[RequestSchema.Name] = value;
			}
		}

		public Guid RequestGuid
		{
			get
			{
				return (Guid)this[RequestSchema.RequestGuid];
			}
			private set
			{
				this[RequestSchema.RequestGuid] = value;
			}
		}

		public ADObjectId RequestQueue
		{
			get
			{
				return (ADObjectId)this[RequestSchema.RequestQueue];
			}
			private set
			{
				this[RequestSchema.RequestQueue] = value;
			}
		}

		public RequestFlags Flags
		{
			get
			{
				return (RequestFlags)this[RequestSchema.Flags];
			}
			private set
			{
				this[RequestSchema.Flags] = value;
			}
		}

		public string BatchName
		{
			get
			{
				return (string)this[RequestSchema.BatchName];
			}
			private set
			{
				this[RequestSchema.BatchName] = value;
			}
		}

		public RequestStatus Status
		{
			get
			{
				return (RequestStatus)this[RequestSchema.Status];
			}
			private set
			{
				this[RequestSchema.Status] = value;
			}
		}

		public bool Protect
		{
			get
			{
				return (this.Flags & RequestFlags.Protected) != RequestFlags.None;
			}
		}

		public bool Suspend
		{
			get
			{
				return (this.Flags & RequestFlags.Suspend) != RequestFlags.None;
			}
		}

		public RequestDirection Direction
		{
			get
			{
				if ((this.Flags & RequestFlags.Push) == RequestFlags.None)
				{
					return RequestDirection.Pull;
				}
				return RequestDirection.Push;
			}
		}

		public RequestStyle RequestStyle
		{
			get
			{
				if ((this.Flags & RequestFlags.CrossOrg) == RequestFlags.None)
				{
					return RequestStyle.IntraOrg;
				}
				return RequestStyle.CrossOrg;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)this[RequestSchema.OrganizationId];
			}
			private set
			{
				this[RequestSchema.OrganizationId] = value;
			}
		}

		public DateTime? WhenChanged
		{
			get
			{
				return (DateTime?)this[RequestSchema.WhenChanged];
			}
			private set
			{
				this[RequestSchema.WhenChanged] = value;
			}
		}

		public DateTime? WhenCreated
		{
			get
			{
				return (DateTime?)this[RequestSchema.WhenCreated];
			}
			private set
			{
				this[RequestSchema.WhenCreated] = value;
			}
		}

		public DateTime? WhenChangedUTC
		{
			get
			{
				return (DateTime?)this[RequestSchema.WhenChangedUTC];
			}
			private set
			{
				this[RequestSchema.WhenChangedUTC] = value;
			}
		}

		public DateTime? WhenCreatedUTC
		{
			get
			{
				return (DateTime?)this[RequestSchema.WhenCreatedUTC];
			}
			private set
			{
				this[RequestSchema.WhenCreatedUTC] = value;
			}
		}

		internal string RemoteHostName
		{
			get
			{
				return (string)this[RequestSchema.RemoteHostName];
			}
			private set
			{
				this[RequestSchema.RemoteHostName] = value;
			}
		}

		internal bool SuspendWhenReadyToComplete
		{
			get
			{
				return (this.Flags & RequestFlags.SuspendWhenReadyToComplete) != RequestFlags.None;
			}
		}

		internal ADObjectId SourceDatabase
		{
			get
			{
				return (ADObjectId)this[RequestSchema.SourceDatabase];
			}
			private set
			{
				this[RequestSchema.SourceDatabase] = value;
			}
		}

		internal ADObjectId TargetDatabase
		{
			get
			{
				return (ADObjectId)this[RequestSchema.TargetDatabase];
			}
			private set
			{
				this[RequestSchema.TargetDatabase] = value;
			}
		}

		internal string FilePath
		{
			get
			{
				return (string)this[RequestSchema.FilePath];
			}
			private set
			{
				this[RequestSchema.FilePath] = value;
			}
		}

		internal ADObjectId SourceMailbox
		{
			get
			{
				return (ADObjectId)this[RequestSchema.SourceMailbox];
			}
			private set
			{
				this[RequestSchema.SourceMailbox] = value;
			}
		}

		internal ADObjectId TargetMailbox
		{
			get
			{
				return (ADObjectId)this[RequestSchema.TargetMailbox];
			}
			private set
			{
				this[RequestSchema.TargetMailbox] = value;
			}
		}

		internal MRSRequestType Type { get; private set; }

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return RequestBase.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public override string ToString()
		{
			if (this.RequestGuid != Guid.Empty)
			{
				return this.RequestGuid.ToString();
			}
			return base.ToString();
		}

		internal virtual void Initialize(IRequestIndexEntry index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			this[SimpleProviderObjectSchema.Identity] = index.GetRequestIndexEntryId(this);
			this.Name = index.Name;
			this.Status = index.Status;
			this.Flags = index.Flags;
			this.RemoteHostName = index.RemoteHostName;
			this.SourceDatabase = index.SourceMDB;
			this.TargetDatabase = index.TargetMDB;
			this.FilePath = index.FilePath;
			this.SourceMailbox = index.SourceUserId;
			this.TargetMailbox = index.TargetUserId;
			this.RequestGuid = index.RequestGuid;
			this.RequestQueue = index.StorageMDB;
			this.OrganizationId = index.OrganizationId;
			this.BatchName = index.BatchName;
			this.Type = index.Type;
			this.WhenChanged = index.WhenChanged;
			this.WhenCreated = index.WhenCreated;
			this.WhenChangedUTC = index.WhenChangedUTC;
			this.WhenCreatedUTC = index.WhenCreatedUTC;
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<RequestSchema>();
	}
}
