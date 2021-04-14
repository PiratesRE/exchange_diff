using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IDocEntry : IEquatable<IDocEntry>
	{
		Guid MailboxGuid { get; }

		string RawItemId { get; }

		int DocumentId { get; }

		string EntryId { get; }

		long IndexId { get; }
	}
}
