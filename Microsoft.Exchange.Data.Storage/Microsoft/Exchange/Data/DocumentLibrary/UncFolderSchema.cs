using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UncFolderSchema : UncItemSchema
	{
		public new static UncFolderSchema Instance
		{
			get
			{
				if (UncFolderSchema.instance == null)
				{
					UncFolderSchema.instance = new UncFolderSchema();
				}
				return UncFolderSchema.instance;
			}
		}

		private static UncFolderSchema instance;
	}
}
