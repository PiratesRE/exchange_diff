using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SoftDeletedMailboxRemovalCheck
	{
		protected SoftDeletedMailboxRemovalCheck(SoftDeletedRemovalData data, IDirectoryProvider directory)
		{
			this.Data = data;
			this.targetDatabase = new Lazy<DirectoryDatabase>(new Func<DirectoryDatabase>(this.FetchTargetDatabase));
			this.directory = directory;
		}

		private protected SoftDeletedRemovalData Data { protected get; private set; }

		protected DirectoryDatabase TargetDatabase
		{
			get
			{
				return this.targetDatabase.Value;
			}
		}

		private SoftDeletedMailboxRemovalCheck Next { get; set; }

		public SoftDeleteMailboxRemovalCheckRemoval GetRemovalResult()
		{
			return this.CheckRemoval() ?? this.CheckNext();
		}

		public void SetNext(SoftDeletedMailboxRemovalCheck next)
		{
			this.Next = next;
		}

		protected abstract SoftDeleteMailboxRemovalCheckRemoval CheckRemoval();

		private SoftDeleteMailboxRemovalCheckRemoval CheckNext()
		{
			if (this.Next == null)
			{
				return SoftDeleteMailboxRemovalCheckRemoval.AllowRemoval();
			}
			return this.Next.GetRemovalResult();
		}

		private DirectoryDatabase FetchTargetDatabase()
		{
			return (DirectoryDatabase)this.directory.GetDirectoryObject(this.Data.TargetDatabase);
		}

		private readonly IDirectoryProvider directory;

		private readonly Lazy<DirectoryDatabase> targetDatabase;
	}
}
