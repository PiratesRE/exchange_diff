using System;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;

namespace Microsoft.Exchange.DxStore.Common
{
	public class WcfStoreAccessClient : IDxStoreAccessClient
	{
		public WcfStoreAccessClient(CachedChannelFactory<IDxStoreAccess> channelFactory, TimeSpan? operationTimeout = null)
		{
			this.Initialize(channelFactory, operationTimeout);
		}

		public static DxStoreAccessExceptionTranslator Runner { get; set; } = new DxStoreAccessExceptionTranslator();

		public TimeSpan? OperationTimeout { get; set; }

		public CachedChannelFactory<IDxStoreAccess> ChannelFactory { get; set; }

		public void Initialize(CachedChannelFactory<IDxStoreAccess> channelFactory, TimeSpan? operationTimeout)
		{
			this.ChannelFactory = channelFactory;
			this.OperationTimeout = operationTimeout;
			if (ExTraceGlobals.AccessClientTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AccessClientTracer.TraceDebug<string, string>(0L, "{0} Initialized (timeout: {1})", base.GetType().Name, (operationTimeout != null) ? operationTimeout.ToString() : "<null>");
			}
		}

		public DxStoreAccessReply.CheckKey CheckKey(DxStoreAccessRequest.CheckKey request, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreAccess> runner = WcfStoreAccessClient.Runner;
			CachedChannelFactory<IDxStoreAccess> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<DxStoreAccessReply.CheckKey>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreAccess service) => this.TraceRequests<DxStoreAccessRequest.CheckKey, DxStoreAccessReply.CheckKey>(request, new Func<DxStoreAccessRequest.CheckKey, DxStoreAccessReply.CheckKey>(service.CheckKey)));
		}

		public DxStoreAccessReply.DeleteKey DeleteKey(DxStoreAccessRequest.DeleteKey request, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreAccess> runner = WcfStoreAccessClient.Runner;
			CachedChannelFactory<IDxStoreAccess> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<DxStoreAccessReply.DeleteKey>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreAccess service) => this.TraceRequests<DxStoreAccessRequest.DeleteKey, DxStoreAccessReply.DeleteKey>(request, new Func<DxStoreAccessRequest.DeleteKey, DxStoreAccessReply.DeleteKey>(service.DeleteKey)));
		}

		public DxStoreAccessReply.SetProperty SetProperty(DxStoreAccessRequest.SetProperty request, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreAccess> runner = WcfStoreAccessClient.Runner;
			CachedChannelFactory<IDxStoreAccess> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<DxStoreAccessReply.SetProperty>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreAccess service) => this.TraceRequests<DxStoreAccessRequest.SetProperty, DxStoreAccessReply.SetProperty>(request, new Func<DxStoreAccessRequest.SetProperty, DxStoreAccessReply.SetProperty>(service.SetProperty)));
		}

		public DxStoreAccessReply.DeleteProperty DeleteProperty(DxStoreAccessRequest.DeleteProperty request, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreAccess> runner = WcfStoreAccessClient.Runner;
			CachedChannelFactory<IDxStoreAccess> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<DxStoreAccessReply.DeleteProperty>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreAccess service) => this.TraceRequests<DxStoreAccessRequest.DeleteProperty, DxStoreAccessReply.DeleteProperty>(request, new Func<DxStoreAccessRequest.DeleteProperty, DxStoreAccessReply.DeleteProperty>(service.DeleteProperty)));
		}

		public DxStoreAccessReply.ExecuteBatch ExecuteBatch(DxStoreAccessRequest.ExecuteBatch request, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreAccess> runner = WcfStoreAccessClient.Runner;
			CachedChannelFactory<IDxStoreAccess> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<DxStoreAccessReply.ExecuteBatch>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreAccess service) => this.TraceRequests<DxStoreAccessRequest.ExecuteBatch, DxStoreAccessReply.ExecuteBatch>(request, new Func<DxStoreAccessRequest.ExecuteBatch, DxStoreAccessReply.ExecuteBatch>(service.ExecuteBatch)));
		}

		public DxStoreAccessReply.GetProperty GetProperty(DxStoreAccessRequest.GetProperty request, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreAccess> runner = WcfStoreAccessClient.Runner;
			CachedChannelFactory<IDxStoreAccess> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<DxStoreAccessReply.GetProperty>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreAccess service) => this.TraceRequests<DxStoreAccessRequest.GetProperty, DxStoreAccessReply.GetProperty>(request, new Func<DxStoreAccessRequest.GetProperty, DxStoreAccessReply.GetProperty>(service.GetProperty)));
		}

		public DxStoreAccessReply.GetAllProperties GetAllProperties(DxStoreAccessRequest.GetAllProperties request, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreAccess> runner = WcfStoreAccessClient.Runner;
			CachedChannelFactory<IDxStoreAccess> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<DxStoreAccessReply.GetAllProperties>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreAccess service) => this.TraceRequests<DxStoreAccessRequest.GetAllProperties, DxStoreAccessReply.GetAllProperties>(request, new Func<DxStoreAccessRequest.GetAllProperties, DxStoreAccessReply.GetAllProperties>(service.GetAllProperties)));
		}

		public DxStoreAccessReply.GetPropertyNames GetPropertyNames(DxStoreAccessRequest.GetPropertyNames request, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreAccess> runner = WcfStoreAccessClient.Runner;
			CachedChannelFactory<IDxStoreAccess> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<DxStoreAccessReply.GetPropertyNames>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreAccess service) => this.TraceRequests<DxStoreAccessRequest.GetPropertyNames, DxStoreAccessReply.GetPropertyNames>(request, new Func<DxStoreAccessRequest.GetPropertyNames, DxStoreAccessReply.GetPropertyNames>(service.GetPropertyNames)));
		}

		public DxStoreAccessReply.GetSubkeyNames GetSubkeyNames(DxStoreAccessRequest.GetSubkeyNames request, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreAccess> runner = WcfStoreAccessClient.Runner;
			CachedChannelFactory<IDxStoreAccess> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<DxStoreAccessReply.GetSubkeyNames>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreAccess service) => this.TraceRequests<DxStoreAccessRequest.GetSubkeyNames, DxStoreAccessReply.GetSubkeyNames>(request, new Func<DxStoreAccessRequest.GetSubkeyNames, DxStoreAccessReply.GetSubkeyNames>(service.GetSubkeyNames)));
		}

		public TRep TraceRequests<TReq, TRep>(TReq request, Func<TReq, TRep> action) where TReq : DxStoreAccessRequest where TRep : DxStoreAccessReply
		{
			Trace accessClientTracer = ExTraceGlobals.AccessClientTracer;
			bool flag = accessClientTracer.IsTraceEnabled(TraceType.DebugTrace);
			bool flag2 = accessClientTracer.IsTraceEnabled(TraceType.ErrorTrace);
			if (!flag && !flag2)
			{
				return action(request);
			}
			TRep trep = default(TRep);
			try
			{
				if (flag)
				{
					string arg = Utils.SerializeObjectToJsonString<TReq>(request, false, true) ?? "<serialization error>";
					accessClientTracer.TraceDebug<string, string>(0L, "Sending Request: {0}{1}", typeof(TReq).Name, arg);
				}
				trep = action(request);
			}
			catch (Exception ex)
			{
				if (flag2)
				{
					string arg2 = "<none>";
					FaultException<DxStoreServerFault> faultException = ex as FaultException<DxStoreServerFault>;
					if (faultException != null)
					{
						DxStoreServerFault detail = faultException.Detail;
						arg2 = (Utils.SerializeObjectToJsonString<DxStoreServerFault>(detail, false, true) ?? "<serialization error>");
					}
					accessClientTracer.TraceDebug<string, string, Exception>(0L, "Send failed - Request: {0} - Fault: {1} - Exception: {2}", typeof(TReq).Name, arg2, ex);
				}
				throw;
			}
			if (flag)
			{
				string arg3 = (trep != null) ? (Utils.SerializeObjectToJsonString<TRep>(trep, false, true) ?? "<serialization error>") : "<null>";
				accessClientTracer.TraceDebug<string, string>(0L, "Received reply: {0}{1}", typeof(TRep).Name, arg3);
			}
			return trep;
		}
	}
}
