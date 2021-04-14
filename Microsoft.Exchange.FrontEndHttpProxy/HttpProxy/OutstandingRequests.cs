using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class OutstandingRequests
	{
		internal static void AddRequest(ProxyRequestHandler request)
		{
			if (OutstandingRequests.TrackOutstandingRequests.Value)
			{
				try
				{
					OutstandingRequests.hashsetLock.EnterWriteLock();
					OutstandingRequests.requests.Add(request);
				}
				finally
				{
					try
					{
						OutstandingRequests.hashsetLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			PerfCounters.HttpProxyCountersInstance.OutstandingProxyRequests.Increment();
		}

		internal static void RemoveRequest(ProxyRequestHandler request)
		{
			if (OutstandingRequests.TrackOutstandingRequests.Value)
			{
				try
				{
					OutstandingRequests.hashsetLock.EnterWriteLock();
					OutstandingRequests.requests.Remove(request);
				}
				finally
				{
					try
					{
						OutstandingRequests.hashsetLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			PerfCounters.HttpProxyCountersInstance.OutstandingProxyRequests.Decrement();
		}

		private static readonly BoolAppSettingsEntry TrackOutstandingRequests = new BoolAppSettingsEntry(HttpProxySettings.Prefix("TrackOutstandingRequests"), false, ExTraceGlobals.VerboseTracer);

		private static ReaderWriterLockSlim hashsetLock = new ReaderWriterLockSlim();

		private static HashSet<ProxyRequestHandler> requests = new HashSet<ProxyRequestHandler>();
	}
}
