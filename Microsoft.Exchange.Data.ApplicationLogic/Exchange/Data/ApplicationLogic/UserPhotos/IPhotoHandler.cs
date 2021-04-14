using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPhotoHandler
	{
		PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response);

		IPhotoHandler Then(IPhotoHandler next);
	}
}
