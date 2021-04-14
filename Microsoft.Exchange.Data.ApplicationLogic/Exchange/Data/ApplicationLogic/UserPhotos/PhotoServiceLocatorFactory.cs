using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PhotoServiceLocatorFactory : IPhotoServiceLocatorFactory
	{
		public PhotoServiceLocatorFactory(ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.upstreamTracer = upstreamTracer;
		}

		public IPhotoServiceLocator CreateForLocalForest(IPerformanceDataLogger perfLogger)
		{
			ArgumentValidator.ThrowIfNull("perfLogger", perfLogger);
			return new LocalForestPhotoServiceLocatorUsingMailboxServerLocator(perfLogger, this.upstreamTracer);
		}

		private readonly ITracer upstreamTracer;
	}
}
