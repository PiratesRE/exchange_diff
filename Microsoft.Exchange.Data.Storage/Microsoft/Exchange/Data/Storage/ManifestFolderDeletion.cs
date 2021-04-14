using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ManifestFolderDeletion : ManifestChangeBase
	{
		internal ManifestFolderDeletion(byte[] idSetDeleted)
		{
			this.idSetDeleted = idSetDeleted;
		}

		public byte[] IdsetDeleted
		{
			get
			{
				return this.idSetDeleted;
			}
		}

		private readonly byte[] idSetDeleted;
	}
}
