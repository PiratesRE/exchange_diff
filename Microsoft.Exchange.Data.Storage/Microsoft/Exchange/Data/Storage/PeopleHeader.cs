using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleHeader
	{
		public PeopleHeader(string displayName, string startChar)
		{
			Util.ThrowOnNullOrEmptyArgument(displayName, "displayName");
			this.DisplayName = displayName;
			this.StartChar = startChar;
		}

		public string DisplayName { get; private set; }

		public string StartChar { get; private set; }

		public override bool Equals(object obj)
		{
			PeopleHeader peopleHeader = obj as PeopleHeader;
			return peopleHeader != null && this.DisplayName == peopleHeader.DisplayName && this.StartChar == peopleHeader.StartChar;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, PeopleHeader.ToStringFormat, new object[]
			{
				this.DisplayName,
				this.StartChar
			});
		}

		private static readonly string ToStringFormat = "DiplayName={0}, StartChar:{1}";
	}
}
