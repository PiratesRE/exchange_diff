using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxPhotoReader
	{
		PhotoMetadata Read(IMailboxSession session, UserPhotoSize size, bool preview, Stream output, IPerformanceDataLogger perfLogger);

		int ReadThumbprint(IMailboxSession session, bool preview, bool forceReloadThumbprint);

		int ReadThumbprint(IMailboxSession session, bool preview);

		int ReadAllPreviewSizes(IMailboxSession session, IDictionary<UserPhotoSize, byte[]> output);

		bool HasPhotoBeenDeleted(Exception e);
	}
}
