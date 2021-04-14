using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxPhotoWriter
	{
		void UploadPreview(int thumbprint, IDictionary<UserPhotoSize, byte[]> photos);

		void Clear();

		void ClearPreview();

		void Save();
	}
}
