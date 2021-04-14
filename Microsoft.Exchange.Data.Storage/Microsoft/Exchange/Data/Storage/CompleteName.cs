using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CompleteName
	{
		public abstract string Title { get; set; }

		public abstract string FirstName { get; set; }

		public abstract string MiddleName { get; set; }

		public abstract string LastName { get; set; }

		public abstract string Suffix { get; set; }

		public abstract string Initials { get; set; }

		public abstract string FullName { get; set; }

		public abstract string Nickname { get; set; }

		public abstract string YomiCompany { get; set; }

		public abstract string YomiFirstName { get; set; }

		public abstract string YomiLastName { get; set; }
	}
}
