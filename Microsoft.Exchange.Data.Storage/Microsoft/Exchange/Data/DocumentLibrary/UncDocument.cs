using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UncDocument : UncDocumentLibraryItem, IDocument, IDocumentLibraryItem, IReadOnlyPropertyBag
	{
		internal UncDocument(UncSession session, UncObjectId objectId) : base(session, objectId, new FileInfo(objectId.Path.LocalPath), UncDocumentSchema.Instance)
		{
			this.fileInfo = (this.fileSystemInfo as FileInfo);
		}

		public new static UncDocument Read(UncSession session, ObjectId documentId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (documentId == null)
			{
				throw new ArgumentNullException("documentId");
			}
			UncObjectId uncObjectId = documentId as UncObjectId;
			if (uncObjectId == null)
			{
				throw new ArgumentException("documentId");
			}
			return Utils.DoUncTask<UncDocument>(session.Identity, uncObjectId, false, Utils.MethodType.Read, delegate
			{
				FileSystemInfo fileSystemInfo = new FileInfo(uncObjectId.Path.LocalPath);
				if (fileSystemInfo.Attributes != (FileAttributes)(-1) && fileSystemInfo.Exists)
				{
					return new UncDocument(session, uncObjectId);
				}
				throw new ObjectNotFoundException(uncObjectId, Strings.ExObjectNotFound(uncObjectId.Path.LocalPath));
			});
		}

		public override string DisplayName
		{
			get
			{
				return Path.GetFileNameWithoutExtension(this.fileSystemInfo.Name);
			}
		}

		public long Size
		{
			get
			{
				return this.fileInfo.Length;
			}
		}

		public Stream GetDocument()
		{
			return Utils.DoUncTask<Stream>(this.session.Identity, base.UncId, true, Utils.MethodType.GetStream, delegate
			{
				Stream stream = File.Open(this.fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				bool flag = false;
				try
				{
					DateTime lastWriteTimeUtc = this.fileInfo.LastWriteTimeUtc;
					this.fileInfo.Refresh();
					if (lastWriteTimeUtc < this.fileInfo.LastWriteTimeUtc)
					{
						throw new DocumentModifiedException(base.Id, this.fileInfo.FullName);
					}
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						stream.Dispose();
						stream = null;
					}
				}
				return stream;
			});
		}

		protected override string GetParentDirectoryNameInternal()
		{
			return this.fileInfo.DirectoryName;
		}

		private readonly FileInfo fileInfo;
	}
}
