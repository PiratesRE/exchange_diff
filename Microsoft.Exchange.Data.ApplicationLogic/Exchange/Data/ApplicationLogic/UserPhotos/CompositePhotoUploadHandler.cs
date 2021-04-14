using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CompositePhotoUploadHandler : IPhotoUploadHandler
	{
		public CompositePhotoUploadHandler(IPhotoUploadHandler first, IPhotoUploadHandler second)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			this.first = first;
			this.second = second;
		}

		public PhotoResponse Upload(PhotoRequest request, PhotoResponse response)
		{
			return this.second.Upload(request, this.first.Upload(request, response));
		}

		public IPhotoUploadHandler Then(IPhotoUploadHandler next)
		{
			return new CompositePhotoUploadHandler(this, next);
		}

		private readonly IPhotoUploadHandler first;

		private readonly IPhotoUploadHandler second;
	}
}
