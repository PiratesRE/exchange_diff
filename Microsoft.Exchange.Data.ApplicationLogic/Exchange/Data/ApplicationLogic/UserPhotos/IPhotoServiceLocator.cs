using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IPhotoServiceLocator
	{
		Uri Locate(ADUser target);

		bool IsServiceOnThisServer(Uri service);
	}
}
