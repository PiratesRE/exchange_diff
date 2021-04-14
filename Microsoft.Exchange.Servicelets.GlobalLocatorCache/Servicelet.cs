using System;
using System.ServiceModel;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GlobalLocatorCache;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Exchange.Servicelets.GlobalLocatorCache.Messages;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	public class Servicelet : Servicelet
	{
		public override void Work()
		{
			ExEventLog exEventLog = new ExEventLog(ExTraceGlobals.ServiceTracer.Category, "MSExchangeGlobalLocatorCache");
			bool flag = false;
			bool flag2 = false;
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				ServiceHost serviceHost = null;
				ServiceHost serviceHost2 = null;
				ExTraceGlobals.ServiceTracer.TraceDebug(0L, "The Microsoft Exchange Global Locator Cache Service is starting");
				exEventLog.LogEvent(MSExchangeGlobalLocatorCacheEventLogConstants.Tuple_GlobalLocatorCacheServiceStarting, "GlobalLocatorCacheServiceStarting", null);
				try
				{
					serviceHost = new ServiceHost(typeof(GlsCacheService), new Uri[0]);
					serviceHost2 = new ServiceHost(typeof(MserveCacheService), new Uri[0]);
					try
					{
						serviceHost.Open();
						flag = true;
						ExTraceGlobals.ServiceTracer.TraceDebug(0L, "The Microsoft Exchange GLS Cache Service has Started");
					}
					catch (AddressAlreadyInUseException ex)
					{
						ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "The Microsoft Exchange GLS Cache Service failed to register the Windows Communication Foundation endpoint due to another process already listening on that endpoint. Exception: {0}", ex.Message);
						exEventLog.LogEvent(MSExchangeGlobalLocatorCacheEventLogConstants.Tuple_GlobalLocatorCacheServiceFailedToRegisterEndpoint, "GlsCacheServiceFailedToStart", new object[]
						{
							ex.Message
						});
						throw;
					}
					try
					{
						serviceHost2.Open();
						flag2 = true;
						ExTraceGlobals.ServiceTracer.TraceDebug(0L, "The Microsoft Exchange MSERV Cache Service has Started");
					}
					catch (AddressAlreadyInUseException ex2)
					{
						ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "The Microsoft Exchange GLS Cache Service failed to register the Windows Communication Foundation endpoint due to another process already listening on that endpoint. Exception: {0}", ex2.Message);
						exEventLog.LogEvent(MSExchangeGlobalLocatorCacheEventLogConstants.Tuple_GlobalLocatorCacheServiceFailedToRegisterEndpoint, "MserveCacheServiceFailedToStart", new object[]
						{
							ex2.Message
						});
						throw;
					}
					exEventLog.LogEvent(MSExchangeGlobalLocatorCacheEventLogConstants.Tuple_GlobalLocatorCacheServiceStarted, "GlobalLocatorCacheServiceStarted", null);
					base.StopEvent.WaitOne();
				}
				finally
				{
					if (flag)
					{
						serviceHost.Abort();
						ExTraceGlobals.ServiceTracer.TraceDebug(0L, "The Microsoft Exchange GLS Cache Service has Stopped");
					}
					if (serviceHost != null)
					{
						serviceHost.Close();
						serviceHost = null;
					}
					if (flag2)
					{
						serviceHost2.Abort();
						ExTraceGlobals.ServiceTracer.TraceDebug(0L, "The Microsoft Exchange MSERV Cache Service has Stopped");
					}
					if (serviceHost2 != null)
					{
						serviceHost2.Close();
						serviceHost2 = null;
					}
					exEventLog.LogEvent(MSExchangeGlobalLocatorCacheEventLogConstants.Tuple_GlobalLocatorCacheServiceStopped, "GlobalLocatorCacheServiceStopped", null);
				}
			}
		}
	}
}
