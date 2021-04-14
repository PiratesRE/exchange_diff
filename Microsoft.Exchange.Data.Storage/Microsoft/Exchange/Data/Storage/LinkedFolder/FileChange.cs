using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FileChange : ChangedItem
	{
		public string DocIcon { get; private set; }

		public string Author { get; private set; }

		public string Editor { get; private set; }

		public string CheckoutUser { get; private set; }

		public int Size { get; private set; }

		public FileChange(Uri authority, Guid id, string version, string docIcon, string author, string editor, string checkoutUser, string relativePath, string leafNode, ExDateTime whenCreated, ExDateTime lastModified, int size) : base(authority, id, version, relativePath, leafNode, whenCreated, lastModified)
		{
			if (string.IsNullOrEmpty(leafNode))
			{
				throw new ArgumentNullException("leafNode");
			}
			this.DocIcon = docIcon;
			this.Author = author;
			this.Editor = editor;
			this.CheckoutUser = checkoutUser;
			this.Size = size;
		}
	}
}
