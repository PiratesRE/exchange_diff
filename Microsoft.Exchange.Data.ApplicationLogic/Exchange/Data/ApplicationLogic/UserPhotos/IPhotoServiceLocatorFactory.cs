using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IPhotoServiceLocatorFactory
	{
		IPhotoServiceLocator CreateForLocalForest(IPerformanceDataLogger perfLogger);
	}
}
