using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMapiManifestCallback
	{
		ManifestCallbackStatus Change(byte[] entryId, byte[] sourceKey, byte[] changeKey, byte[] changeList, DateTime lastModificationTime, ManifestChangeType changeType, bool associated, PropValue[] props);

		ManifestCallbackStatus Delete(byte[] entryId, bool softDelete, bool expiry);

		ManifestCallbackStatus ReadUnread(byte[] entryId, bool read);
	}
}
