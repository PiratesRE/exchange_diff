using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PersonNotes
	{
		public PersonNotes(string notes, bool isTruncated)
		{
			this.notesBody = notes;
			this.isTruncated = isTruncated;
		}

		public string NotesBody
		{
			get
			{
				return this.notesBody;
			}
		}

		public bool IsTruncated
		{
			get
			{
				return this.isTruncated;
			}
		}

		private readonly string notesBody;

		private readonly bool isTruncated;
	}
}
