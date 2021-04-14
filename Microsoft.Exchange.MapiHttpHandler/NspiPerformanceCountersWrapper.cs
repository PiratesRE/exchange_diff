using System;
using Microsoft.Exchange.AddressBook.Service;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MapiHttp.PerformanceCounters;

namespace Microsoft.Exchange.MapiHttp
{
	internal class NspiPerformanceCountersWrapper : IAddressBookPerformanceCounters
	{
		public IExPerformanceCounter PID
		{
			get
			{
				return NspiPerformanceCounters.PID;
			}
		}

		public IExPerformanceCounter NspiConnectionsCurrent
		{
			get
			{
				return NspiPerformanceCounters.NspiConnectionsCurrent;
			}
		}

		public IExPerformanceCounter NspiConnectionsTotal
		{
			get
			{
				return NspiPerformanceCounters.NspiConnectionsTotal;
			}
		}

		public IExPerformanceCounter NspiConnectionsRate
		{
			get
			{
				return NspiPerformanceCounters.NspiConnectionsRate;
			}
		}

		public IExPerformanceCounter NspiRequests
		{
			get
			{
				return NspiPerformanceCounters.NspiRequests;
			}
		}

		public IExPerformanceCounter NspiRequestsTotal
		{
			get
			{
				return NspiPerformanceCounters.NspiRequestsTotal;
			}
		}

		public IExPerformanceCounter NspiRequestsAverageLatency
		{
			get
			{
				return NspiPerformanceCounters.NspiRequestsAverageLatency;
			}
		}

		public IExPerformanceCounter NspiRequestsRate
		{
			get
			{
				return NspiPerformanceCounters.NspiRequestsRate;
			}
		}

		public IExPerformanceCounter NspiBrowseRequests
		{
			get
			{
				return NspiPerformanceCounters.NspiBrowseRequests;
			}
		}

		public IExPerformanceCounter NspiBrowseRequestsTotal
		{
			get
			{
				return NspiPerformanceCounters.NspiBrowseRequestsTotal;
			}
		}

		public IExPerformanceCounter NspiBrowseRequestsAverageLatency
		{
			get
			{
				return NspiPerformanceCounters.NspiBrowseRequestsAverageLatency;
			}
		}

		public IExPerformanceCounter NspiBrowseRequestsRate
		{
			get
			{
				return NspiPerformanceCounters.NspiBrowseRequestsRate;
			}
		}

		public IExPerformanceCounter RfrRequests
		{
			get
			{
				return NspiPerformanceCounters.RfrRequests;
			}
		}

		public IExPerformanceCounter RfrRequestsTotal
		{
			get
			{
				return NspiPerformanceCounters.RfrRequestsTotal;
			}
		}

		public IExPerformanceCounter RfrRequestsRate
		{
			get
			{
				return NspiPerformanceCounters.RfrRequestsRate;
			}
		}

		public IExPerformanceCounter RfrRequestsAverageLatency
		{
			get
			{
				return NspiPerformanceCounters.RfrRequestsAverageLatency;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoAverageTime
		{
			get
			{
				return NspiPerformanceCounters.ThumbnailPhotoAverageTime;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoAverageTimeBase
		{
			get
			{
				return NspiPerformanceCounters.ThumbnailPhotoAverageTimeBase;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoFromMailboxCount
		{
			get
			{
				return NspiPerformanceCounters.ThumbnailPhotoFromMailboxCount;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoFromDirectoryCount
		{
			get
			{
				return NspiPerformanceCounters.ThumbnailPhotoFromDirectoryCount;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoNotPresentCount
		{
			get
			{
				return NspiPerformanceCounters.ThumbnailPhotoNotPresentCount;
			}
		}
	}
}
