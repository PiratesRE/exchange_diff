using System;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.PhotoGarbageCollection;
using Microsoft.Exchange.ServiceHost;

namespace Microsoft.Exchange.Servicelets.PhotoGarbageCollection
{
	public sealed class PhotoGarbageCollectionServicelet : Servicelet
	{
		public override void Work()
		{
			using (PhotoGarbageCollectionLogger photoGarbageCollectionLogger = new PhotoGarbageCollectionLogger(PhotoGarbageCollectionServicelet.PhotosConfiguration, "PhotoGarbageCollectionServicelet"))
			{
				new PhotoGarbageCollectionScheduler(PhotoGarbageCollectionServicelet.PhotosConfiguration, photoGarbageCollectionLogger, photoGarbageCollectionLogger.Compose(PhotoGarbageCollectionServicelet.Tracer)).Run(base.StopEvent);
			}
		}

		private const string LogFileName = "PhotoGarbageCollectionServicelet";

		private static readonly Trace Tracer = ExTraceGlobals.GarbageCollectionTracer;

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);
	}
}
