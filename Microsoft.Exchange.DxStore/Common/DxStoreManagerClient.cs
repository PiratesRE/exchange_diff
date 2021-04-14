using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public class DxStoreManagerClient : IDxStoreClient<IDxStoreManager>
	{
		public DxStoreManagerClient(CachedChannelFactory<IDxStoreManager> channelFactory, TimeSpan? operationTimeout = null)
		{
			this.Initialize(channelFactory, operationTimeout);
		}

		public static DxStoreManagerExceptionTranslator Runner { get; set; } = new DxStoreManagerExceptionTranslator();

		public TimeSpan? OperationTimeout { get; set; }

		public CachedChannelFactory<IDxStoreManager> ChannelFactory { get; set; }

		public void Initialize(CachedChannelFactory<IDxStoreManager> channelFactory, TimeSpan? operationTimeout = null)
		{
			this.ChannelFactory = channelFactory;
			this.OperationTimeout = operationTimeout;
		}

		public void StartInstance(string groupName, bool isForce = false, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreManager> runner = DxStoreManagerClient.Runner;
			CachedChannelFactory<IDxStoreManager> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreManager service)
			{
				service.StartInstance(groupName, isForce);
			});
		}

		public void RestartInstance(string groupName, bool isForce = false, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreManager> runner = DxStoreManagerClient.Runner;
			CachedChannelFactory<IDxStoreManager> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreManager service)
			{
				service.RestartInstance(groupName, isForce);
			});
		}

		public void RemoveInstance(string groupName, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreManager> runner = DxStoreManagerClient.Runner;
			CachedChannelFactory<IDxStoreManager> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreManager service)
			{
				service.RemoveInstance(groupName);
			});
		}

		public void StopInstance(string groupName, bool isDisable = true, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreManager> runner = DxStoreManagerClient.Runner;
			CachedChannelFactory<IDxStoreManager> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreManager service)
			{
				service.StopInstance(groupName, true);
			});
		}

		public InstanceGroupConfig GetInstanceConfig(string groupName, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreManager> runner = DxStoreManagerClient.Runner;
			CachedChannelFactory<IDxStoreManager> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<InstanceGroupConfig>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreManager service) => service.GetInstanceConfig(groupName, false));
		}

		public void TriggerRefresh(string reason, bool isForceRefreshCache, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreManager> runner = DxStoreManagerClient.Runner;
			CachedChannelFactory<IDxStoreManager> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreManager service)
			{
				service.TriggerRefresh(reason, isForceRefreshCache);
			});
		}
	}
}
