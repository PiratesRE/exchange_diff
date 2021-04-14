using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ManifestItemDeletion : ManifestChangeBase
	{
		internal ManifestItemDeletion(byte[] idsetDeleted, bool isSoftDeleted, bool isExpired)
		{
			this.idsetDeleted = idsetDeleted;
			this.isSoftDeleted = isSoftDeleted;
			this.isExpired = isExpired;
		}

		public byte[] IdsetDeleted
		{
			get
			{
				return this.idsetDeleted;
			}
		}

		public bool IsExpired
		{
			get
			{
				return this.isExpired;
			}
		}

		public bool IsSoftDeleted
		{
			get
			{
				return this.isSoftDeleted;
			}
		}

		private readonly byte[] idsetDeleted;

		private readonly bool isSoftDeleted;

		private readonly bool isExpired;
	}
}
