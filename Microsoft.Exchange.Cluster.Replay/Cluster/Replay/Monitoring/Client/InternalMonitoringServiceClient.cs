using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring.Client
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	internal class InternalMonitoringServiceClient : ClientBase<InternalMonitoringService>, InternalMonitoringService
	{
		public InternalMonitoringServiceClient()
		{
		}

		public InternalMonitoringServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public InternalMonitoringServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public InternalMonitoringServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public InternalMonitoringServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public ServiceVersion GetVersion()
		{
			return base.Channel.GetVersion();
		}

		public Task<ServiceVersion> GetVersionAsync()
		{
			return base.Channel.GetVersionAsync();
		}

		public void PublishDagHealthInfo(HealthInfoPersisted healthInfo)
		{
			base.Channel.PublishDagHealthInfo(healthInfo);
		}

		public Task PublishDagHealthInfoAsync(HealthInfoPersisted healthInfo)
		{
			return base.Channel.PublishDagHealthInfoAsync(healthInfo);
		}

		public DateTime GetDagHealthInfoUpdateTimeUtc()
		{
			return base.Channel.GetDagHealthInfoUpdateTimeUtc();
		}

		public Task<DateTime> GetDagHealthInfoUpdateTimeUtcAsync()
		{
			return base.Channel.GetDagHealthInfoUpdateTimeUtcAsync();
		}

		public HealthInfoPersisted GetDagHealthInfo()
		{
			return base.Channel.GetDagHealthInfo();
		}

		public Task<HealthInfoPersisted> GetDagHealthInfoAsync()
		{
			return base.Channel.GetDagHealthInfoAsync();
		}
	}
}
