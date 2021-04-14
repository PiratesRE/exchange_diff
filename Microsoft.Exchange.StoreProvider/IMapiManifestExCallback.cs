using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMapiManifestExCallback
	{
		ManifestCallbackStatus Change(bool newMessage, PropValue[] headerProps, PropValue[] props);

		ManifestCallbackStatus Delete(byte[] idsetDeleted, bool softDeleted, bool expired);

		ManifestCallbackStatus ReadUnread(byte[] idsetReadUnread, bool read);
	}
}
