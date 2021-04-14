using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal interface IAddressBookPerformanceCounters
	{
		IExPerformanceCounter PID { get; }

		IExPerformanceCounter NspiConnectionsCurrent { get; }

		IExPerformanceCounter NspiConnectionsTotal { get; }

		IExPerformanceCounter NspiConnectionsRate { get; }

		IExPerformanceCounter NspiRequests { get; }

		IExPerformanceCounter NspiRequestsTotal { get; }

		IExPerformanceCounter NspiRequestsAverageLatency { get; }

		IExPerformanceCounter NspiRequestsRate { get; }

		IExPerformanceCounter NspiBrowseRequests { get; }

		IExPerformanceCounter NspiBrowseRequestsTotal { get; }

		IExPerformanceCounter NspiBrowseRequestsAverageLatency { get; }

		IExPerformanceCounter NspiBrowseRequestsRate { get; }

		IExPerformanceCounter RfrRequests { get; }

		IExPerformanceCounter RfrRequestsTotal { get; }

		IExPerformanceCounter RfrRequestsRate { get; }

		IExPerformanceCounter RfrRequestsAverageLatency { get; }

		IExPerformanceCounter ThumbnailPhotoAverageTime { get; }

		IExPerformanceCounter ThumbnailPhotoAverageTimeBase { get; }

		IExPerformanceCounter ThumbnailPhotoFromMailboxCount { get; }

		IExPerformanceCounter ThumbnailPhotoFromDirectoryCount { get; }

		IExPerformanceCounter ThumbnailPhotoNotPresentCount { get; }
	}
}
