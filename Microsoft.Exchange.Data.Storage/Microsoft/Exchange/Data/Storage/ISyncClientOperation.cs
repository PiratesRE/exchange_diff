using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncClientOperation
	{
		int?[] ChangeTrackingInformation { get; set; }

		ChangeType ChangeType { get; }

		string ClientAddId { get; }

		ISyncItemId Id { get; }

		ISyncItem Item { get; }

		bool SendEnabled { get; }
	}
}
