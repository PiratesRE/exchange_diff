using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationAttachment : IDisposable
	{
		ExDateTime LastModifiedTime { get; }

		long Size { get; }

		string Id { get; }

		Stream Stream { get; }

		void Save(string contentId);
	}
}
