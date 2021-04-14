using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class NullAddressBookPerformanceCounters : IAddressBookPerformanceCounters
	{
		public IExPerformanceCounter PID
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiConnectionsCurrent
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiConnectionsTotal
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiConnectionsRate
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiRequests
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiRequestsTotal
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiRequestsAverageLatency
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiRequestsRate
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiBrowseRequests
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiBrowseRequestsTotal
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiBrowseRequestsAverageLatency
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter NspiBrowseRequestsRate
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter RfrRequests
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter RfrRequestsTotal
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter RfrRequestsRate
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter RfrRequestsAverageLatency
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoAverageTime
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoAverageTimeBase
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoFromMailboxCount
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoFromDirectoryCount
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoNotPresentCount
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}
	}
}
