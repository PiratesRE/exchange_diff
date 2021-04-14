using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal class Factory
	{
		protected Factory()
		{
		}

		internal static Hookable<Factory> Instance
		{
			get
			{
				return Factory.instance;
			}
		}

		internal static Factory Current
		{
			get
			{
				return Factory.instance.Value;
			}
		}

		internal virtual IFlowManager CreateFlowManager()
		{
			return FlowManager.Instance;
		}

		internal virtual IIndexManager CreateIndexManager()
		{
			return IndexManager.Instance;
		}

		internal virtual INodeManager CreateNodeManagementClient()
		{
			return NodeManagementClient.Instance;
		}

		internal virtual IWatermarkStorage CreateWatermarkStorage(ISubmitDocument fastWatermarkFeeder, ISearchServiceConfig config, string indexSystemName)
		{
			return new WatermarkStorage(fastWatermarkFeeder, config, indexSystemName);
		}

		internal virtual IFailedItemStorage CreateFailedItemStorage(ISearchServiceConfig config, string indexSystemName)
		{
			return this.CreateFailedItemStorage(config, indexSystemName, config.HostName);
		}

		internal virtual IFailedItemStorage CreateFailedItemStorage(ISearchServiceConfig config, string indexSystemName, string hostName)
		{
			return new FailedItemStorage(config, indexSystemName, hostName);
		}

		internal virtual ISubmitDocument CreateFastFeeder(ISearchServiceConfig config, string indexSystemFlow, string indexSystemName, string instanceName)
		{
			return this.InternalCreateFastFeeder(config, indexSystemFlow, indexSystemName, instanceName, config.MinFeederSessions);
		}

		internal virtual ISubmitDocument CreateFastFeeder(ISearchServiceConfig config, string indexSystemFlow, string indexSystemName, string instanceName, int numberOfSessions)
		{
			return this.InternalCreateFastFeeder(config, indexSystemFlow, indexSystemName, instanceName, numberOfSessions);
		}

		internal virtual ISubmitDocument CreateFastFeeder(string hostName, int contentSubmissionPort, int numSessions, TimeSpan connectionTimeout, TimeSpan submissionTimeout, TimeSpan processingTimeout, string flowName)
		{
			ISubmitDocument result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				FastFeeder fastFeeder = new FastFeeder(hostName, contentSubmissionPort, submissionTimeout, processingTimeout, TimeSpan.Zero, true, numSessions, flowName);
				disposeGuard.Add<FastFeeder>(fastFeeder);
				fastFeeder.Initialize();
				fastFeeder.ConnectionTimeout = connectionTimeout;
				disposeGuard.Success();
				result = fastFeeder;
			}
			return result;
		}

		internal virtual ISubmitDocument InternalCreateFastFeeder(ISearchServiceConfig config, string indexSystemFlow, string indexSystemName, string instanceName, int numberOfSessions)
		{
			Util.ThrowOnNullArgument(indexSystemFlow, "indexSystemFlow");
			ISubmitDocument result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				FastFeeder fastFeeder = new FastFeeder(config.HostName, config.ContentSubmissionPort, config.DocumentFeederSubmissionTimeout, config.DocumentFeederProcessingTimeout, config.DocumentFeederLostCallbackTimeout, false, numberOfSessions, indexSystemFlow);
				disposeGuard.Add<FastFeeder>(fastFeeder);
				fastFeeder.IndexSystemName = indexSystemName;
				fastFeeder.InstanceName = instanceName;
				fastFeeder.PoisonErrorMessages = config.PoisonErrorMessages;
				fastFeeder.DocumentRetries = config.MaxAttemptCount;
				fastFeeder.DocumentFeederBatchSize = config.DocumentFeederBatchSize;
				fastFeeder.ConnectionTimeout = config.DocumentFeederConnectionTimeout;
				fastFeeder.DocumentFeederMaxConnectRetries = config.DocumentFeederMaxConnectRetries;
				fastFeeder.Initialize();
				disposeGuard.Success();
				result = fastFeeder;
			}
			return result;
		}

		internal virtual PagingImsFlowExecutor CreatePagingImsFlowExecutor(string hostName, int queryServicePort, int channelPoolSize, TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout, TimeSpan proxyIdleTimeout, long maxReceivedMessageSize, int maxStringContentLength, int retryCount)
		{
			return new PagingImsFlowExecutor(hostName, queryServicePort, channelPoolSize, openTimeout, sendTimeout, receiveTimeout, proxyIdleTimeout, maxReceivedMessageSize, maxStringContentLength, retryCount);
		}

		private static readonly Hookable<Factory> instance = Hookable<Factory>.Create(true, new Factory());
	}
}
