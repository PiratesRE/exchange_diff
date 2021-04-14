using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPhotoUploadHandler
	{
		PhotoResponse Upload(PhotoRequest request, PhotoResponse response);

		IPhotoUploadHandler Then(IPhotoUploadHandler next);
	}
}
