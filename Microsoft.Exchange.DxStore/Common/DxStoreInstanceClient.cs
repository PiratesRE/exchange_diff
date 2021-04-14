using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public class DxStoreInstanceClient : IDxStoreClient<IDxStoreInstance>
	{
		public DxStoreInstanceClient(CachedChannelFactory<IDxStoreInstance> channelFactory, TimeSpan? operationTimeout = null)
		{
			this.Initialize(channelFactory, operationTimeout);
		}

		public static DxStoreInstanceExceptionTranslator Runner { get; set; } = new DxStoreInstanceExceptionTranslator();

		public TimeSpan? OperationTimeout { get; set; }

		public CachedChannelFactory<IDxStoreInstance> ChannelFactory { get; set; }

		public void Initialize(CachedChannelFactory<IDxStoreInstance> channelFactory, TimeSpan? operationTimeout = null)
		{
			this.ChannelFactory = channelFactory;
			this.OperationTimeout = operationTimeout;
		}

		public void Stop(bool isFlush, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreInstance> runner = DxStoreInstanceClient.Runner;
			CachedChannelFactory<IDxStoreInstance> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreInstance service)
			{
				service.Stop(isFlush);
			});
		}

		public void Flush(TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreInstance> runner = DxStoreInstanceClient.Runner;
			CachedChannelFactory<IDxStoreInstance> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreInstance service)
			{
				service.Flush();
			});
		}

		public void Reconfigure(InstanceGroupMemberConfig[] members, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreInstance> runner = DxStoreInstanceClient.Runner;
			CachedChannelFactory<IDxStoreInstance> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreInstance service)
			{
				service.Reconfigure(members);
			});
		}

		public InstanceStatusInfo GetStatus(TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreInstance> runner = DxStoreInstanceClient.Runner;
			CachedChannelFactory<IDxStoreInstance> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<InstanceStatusInfo>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreInstance service) => service.GetStatus());
		}

		public InstanceSnapshotInfo AcquireSnapshot(string fullKeyName = null, bool isCompress = true, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreInstance> runner = DxStoreInstanceClient.Runner;
			CachedChannelFactory<IDxStoreInstance> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			return runner.Execute<InstanceSnapshotInfo>(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, (IDxStoreInstance service) => service.AcquireSnapshot(fullKeyName, isCompress));
		}

		public void ApplySnapshot(InstanceSnapshotInfo snapshot, bool isForce = false, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreInstance> runner = DxStoreInstanceClient.Runner;
			CachedChannelFactory<IDxStoreInstance> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreInstance service)
			{
				service.ApplySnapshot(snapshot, isForce);
			});
		}

		public void TryBecomeLeader(TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreInstance> runner = DxStoreInstanceClient.Runner;
			CachedChannelFactory<IDxStoreInstance> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreInstance service)
			{
				service.TryBecomeLeader();
			});
		}

		public void NotifyInitiator(Guid commandId, string sender, int instanceNumber, bool isSucceeded, string errorMessage, TimeSpan? timeout = null)
		{
			WcfExceptionTranslator<IDxStoreInstance> runner = DxStoreInstanceClient.Runner;
			CachedChannelFactory<IDxStoreInstance> channelFactory = this.ChannelFactory;
			TimeSpan? timeSpan = timeout;
			runner.Execute(channelFactory, (timeSpan != null) ? new TimeSpan?(timeSpan.GetValueOrDefault()) : this.OperationTimeout, delegate(IDxStoreInstance service)
			{
				service.NotifyInitiator(commandId, sender, instanceNumber, isSucceeded, errorMessage);
			});
		}
	}
}
