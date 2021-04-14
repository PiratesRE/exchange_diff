using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CompositePhotoHandler : IPhotoHandler
	{
		public CompositePhotoHandler(IPhotoHandler first, IPhotoHandler second)
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

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			return this.second.Retrieve(request, this.first.Retrieve(request, response));
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			return new CompositePhotoHandler(this, next);
		}

		private readonly IPhotoHandler first;

		private readonly IPhotoHandler second;
	}
}
