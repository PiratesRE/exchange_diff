using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncState
	{
		int? BackendVersion { get; }

		int Version { get; set; }

		ICustomSerializableBuilder this[string key]
		{
			get;
			set;
		}

		bool Contains(string key);

		void Remove(string key);
	}
}
