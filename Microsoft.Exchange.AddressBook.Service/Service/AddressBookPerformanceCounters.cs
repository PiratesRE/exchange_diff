using System;
using Microsoft.Exchange.AddressBook.Service.PerformanceCounters;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class AddressBookPerformanceCounters : IAddressBookPerformanceCounters
	{
		public IExPerformanceCounter PID
		{
			get
			{
				return AddressBookCounters.PID;
			}
		}

		public IExPerformanceCounter NspiConnectionsCurrent
		{
			get
			{
				return AddressBookCounters.NspiConnectionsCurrent;
			}
		}

		public IExPerformanceCounter NspiConnectionsTotal
		{
			get
			{
				return AddressBookCounters.NspiConnectionsTotal;
			}
		}

		public IExPerformanceCounter NspiConnectionsRate
		{
			get
			{
				return AddressBookCounters.NspiConnectionsRate;
			}
		}

		public IExPerformanceCounter NspiRequests
		{
			get
			{
				return AddressBookCounters.NspiRpcRequests;
			}
		}

		public IExPerformanceCounter NspiRequestsTotal
		{
			get
			{
				return AddressBookCounters.NspiRpcRequestsTotal;
			}
		}

		public IExPerformanceCounter NspiRequestsAverageLatency
		{
			get
			{
				return AddressBookCounters.NspiRpcRequestsAverageLatency;
			}
		}

		public IExPerformanceCounter NspiRequestsRate
		{
			get
			{
				return AddressBookCounters.NspiRpcRequestsRate;
			}
		}

		public IExPerformanceCounter NspiBrowseRequests
		{
			get
			{
				return AddressBookCounters.NspiRpcBrowseRequests;
			}
		}

		public IExPerformanceCounter NspiBrowseRequestsTotal
		{
			get
			{
				return AddressBookCounters.NspiRpcBrowseRequestsTotal;
			}
		}

		public IExPerformanceCounter NspiBrowseRequestsAverageLatency
		{
			get
			{
				return AddressBookCounters.NspiRpcBrowseRequestsAverageLatency;
			}
		}

		public IExPerformanceCounter NspiBrowseRequestsRate
		{
			get
			{
				return AddressBookCounters.NspiRpcBrowseRequestsRate;
			}
		}

		public IExPerformanceCounter RfrRequests
		{
			get
			{
				return AddressBookCounters.RfrRpcRequests;
			}
		}

		public IExPerformanceCounter RfrRequestsTotal
		{
			get
			{
				return AddressBookCounters.RfrRpcRequestsTotal;
			}
		}

		public IExPerformanceCounter RfrRequestsRate
		{
			get
			{
				return AddressBookCounters.RfrRpcRequestsRate;
			}
		}

		public IExPerformanceCounter RfrRequestsAverageLatency
		{
			get
			{
				return AddressBookCounters.RfrRpcRequestsAverageLatency;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoAverageTime
		{
			get
			{
				return AddressBookCounters.ThumbnailPhotoAverageTime;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoAverageTimeBase
		{
			get
			{
				return AddressBookCounters.ThumbnailPhotoAverageTimeBase;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoFromMailboxCount
		{
			get
			{
				return AddressBookCounters.ThumbnailPhotoFromMailboxCount;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoFromDirectoryCount
		{
			get
			{
				return AddressBookCounters.ThumbnailPhotoFromDirectoryCount;
			}
		}

		public IExPerformanceCounter ThumbnailPhotoNotPresentCount
		{
			get
			{
				return AddressBookCounters.ThumbnailPhotoNotPresentCount;
			}
		}
	}
}
