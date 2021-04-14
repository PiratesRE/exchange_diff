using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FolderSaveTransientException : StorageTransientException
	{
		internal FolderSaveTransientException(LocalizedString message, FolderSaveResult folderSaveResult) : base(message, folderSaveResult.Exception)
		{
			this.folderSaveResult = folderSaveResult;
		}

		public FolderSaveResult FolderSaveResult
		{
			get
			{
				return this.folderSaveResult;
			}
		}

		private readonly FolderSaveResult folderSaveResult;
	}
}
