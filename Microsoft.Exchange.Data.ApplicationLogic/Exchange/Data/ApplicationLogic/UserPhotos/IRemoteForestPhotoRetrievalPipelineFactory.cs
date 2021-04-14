using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRemoteForestPhotoRetrievalPipelineFactory
	{
		IPhotoHandler Create();
	}
}
