using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoGarbageCollectionScheduler
	{
		public PhotoGarbageCollectionScheduler(PhotosConfiguration configuration, IPerformanceDataLogger perfLogger, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.tracer = tracer;
			this.configuration = configuration;
			this.collector = new PhotoGarbageCollector(configuration, perfLogger, tracer);
		}

		public void Run(WaitHandle stopRequested)
		{
			this.PerformInitialCollection();
			while (!stopRequested.WaitOne(this.configuration.GarbageCollectionInterval))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Garbage collection scheduler: performing collection.");
				this.Collect();
				this.tracer.TraceDebug((long)this.GetHashCode(), "Garbage collection scheduler: collection complete.");
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "Garbage collection scheduler: stop requested.");
		}

		private void PerformInitialCollection()
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "Garbage collection scheduler: performing initial collection.");
			this.Collect();
			this.tracer.TraceDebug((long)this.GetHashCode(), "Garbage collection scheduler: initial collection complete.");
		}

		private void Collect()
		{
			this.collector.Collect(DateTime.UtcNow);
		}

		private readonly PhotosConfiguration configuration;

		private readonly ITracer tracer;

		private readonly PhotoGarbageCollector collector;
	}
}
