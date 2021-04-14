using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class DirectoryObject
	{
		public DirectoryObject(IDirectoryProvider directory, DirectoryIdentity identity)
		{
			AnchorUtil.ThrowOnNullArgument(directory, "directory");
			AnchorUtil.ThrowOnNullArgument(identity, "identity");
			this.directory = directory;
			this.Identity = identity;
		}

		public IDirectoryProvider Directory
		{
			get
			{
				return this.directory ?? NullDirectory.Instance;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.Identity.Guid;
			}
		}

		[DataMember(Name = "DirectoryObjectIdentity")]
		public DirectoryIdentity Identity { get; private set; }

		public string Name
		{
			get
			{
				return this.Identity.Name;
			}
		}

		[DataMember]
		public DirectoryObject Parent { get; set; }

		public virtual bool SupportsMoving
		{
			get
			{
				return false;
			}
		}

		public virtual IRequest CreateRequestToMove(DirectoryIdentity target, string batchName, ILogger logger)
		{
			throw new NotSupportedException("Directory objects of type " + base.GetType() + " can't be moved.");
		}

		[IgnoreDataMember]
		private readonly IDirectoryProvider directory;
	}
}
