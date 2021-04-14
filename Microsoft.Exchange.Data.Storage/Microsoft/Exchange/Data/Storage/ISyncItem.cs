using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncItem : IDisposeTrackable, IDisposable
	{
		ISyncItemId Id { get; }

		ISyncWatermark Watermark { get; }

		object NativeItem { get; }

		void Load();

		void Save();

		bool IsItemInFilter(QueryFilter filter);
	}
}
