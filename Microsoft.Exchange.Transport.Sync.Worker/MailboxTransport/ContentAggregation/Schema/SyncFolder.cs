using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SyncFolder : DisposeTrackableBase, ISyncObject, IDisposeTrackable, IDisposable
	{
		internal SyncFolder(string displayName) : this(displayName, DefaultFolderType.None)
		{
		}

		internal SyncFolder(string displayName, DefaultFolderType defaultFolderType) : this(displayName, defaultFolderType, null)
		{
		}

		internal SyncFolder(string displayName, DefaultFolderType defaultFolderType, ExDateTime? lastModifiedTime)
		{
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			this.displayName = displayName;
			this.defaultFolderType = defaultFolderType;
			this.lastModifiedTime = lastModifiedTime;
		}

		public SchemaType Type
		{
			get
			{
				base.CheckDisposed();
				return SchemaType.Folder;
			}
		}

		public string DisplayName
		{
			get
			{
				base.CheckDisposed();
				return this.displayName;
			}
		}

		public DefaultFolderType DefaultFolderType
		{
			get
			{
				base.CheckDisposed();
				return this.defaultFolderType;
			}
		}

		public ExDateTime? LastModifiedTime
		{
			get
			{
				base.CheckDisposed();
				return this.lastModifiedTime;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncFolder>(this);
		}

		private readonly ExDateTime? lastModifiedTime;

		private readonly string displayName;

		private readonly DefaultFolderType defaultFolderType;
	}
}
