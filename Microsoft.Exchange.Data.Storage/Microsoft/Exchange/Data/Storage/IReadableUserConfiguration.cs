using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IReadableUserConfiguration : IDisposable
	{
		string ConfigurationName { get; }

		UserConfigurationTypes DataTypes { get; }

		StoreObjectId FolderId { get; }

		StoreObjectId Id { get; }

		ExDateTime LastModifiedTime { get; }

		VersionedId VersionedId { get; }

		IDictionary GetDictionary();

		Stream GetXmlStream();
	}
}
