using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[DebuggerStepThrough]
	public class FederatedServiceOnboardingClient : ClientBase<IFederatedServiceOnboarding>, IFederatedServiceOnboarding
	{
		public FederatedServiceOnboardingClient()
		{
		}

		public FederatedServiceOnboardingClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public FederatedServiceOnboardingClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public FederatedServiceOnboardingClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public FederatedServiceOnboardingClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GetServiceInstanceMapResponse IFederatedServiceOnboarding.GetServiceInstanceMap(GetServiceInstanceMapRequest request)
		{
			return base.Channel.GetServiceInstanceMap(request);
		}

		public ServiceInstanceMapValue GetServiceInstanceMap(string serviceType)
		{
			GetServiceInstanceMapResponse serviceInstanceMap = ((IFederatedServiceOnboarding)this).GetServiceInstanceMap(new GetServiceInstanceMapRequest
			{
				serviceType = serviceType
			});
			return serviceInstanceMap.GetServiceInstanceMapResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<GetServiceInstanceMapResponse> IFederatedServiceOnboarding.GetServiceInstanceMapAsync(GetServiceInstanceMapRequest request)
		{
			return base.Channel.GetServiceInstanceMapAsync(request);
		}

		public Task<GetServiceInstanceMapResponse> GetServiceInstanceMapAsync(string serviceType)
		{
			return ((IFederatedServiceOnboarding)this).GetServiceInstanceMapAsync(new GetServiceInstanceMapRequest
			{
				serviceType = serviceType
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		SetServiceInstanceMapResponse IFederatedServiceOnboarding.SetServiceInstanceMap(SetServiceInstanceMapRequest request)
		{
			return base.Channel.SetServiceInstanceMap(request);
		}

		public ResultCode SetServiceInstanceMap(string serviceType, ServiceInstanceMapValue serviceInstanceMap)
		{
			SetServiceInstanceMapResponse setServiceInstanceMapResponse = ((IFederatedServiceOnboarding)this).SetServiceInstanceMap(new SetServiceInstanceMapRequest
			{
				serviceType = serviceType,
				serviceInstanceMap = serviceInstanceMap
			});
			return setServiceInstanceMapResponse.SetServiceInstanceMapResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<SetServiceInstanceMapResponse> IFederatedServiceOnboarding.SetServiceInstanceMapAsync(SetServiceInstanceMapRequest request)
		{
			return base.Channel.SetServiceInstanceMapAsync(request);
		}

		public Task<SetServiceInstanceMapResponse> SetServiceInstanceMapAsync(string serviceType, ServiceInstanceMapValue serviceInstanceMap)
		{
			return ((IFederatedServiceOnboarding)this).SetServiceInstanceMapAsync(new SetServiceInstanceMapRequest
			{
				serviceType = serviceType,
				serviceInstanceMap = serviceInstanceMap
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GetServiceInstanceInfoResponse IFederatedServiceOnboarding.GetServiceInstanceInfo(GetServiceInstanceInfoRequest request)
		{
			return base.Channel.GetServiceInstanceInfo(request);
		}

		public ServiceInstanceInfoValue GetServiceInstanceInfo(string serviceInstance)
		{
			GetServiceInstanceInfoResponse serviceInstanceInfo = ((IFederatedServiceOnboarding)this).GetServiceInstanceInfo(new GetServiceInstanceInfoRequest
			{
				serviceInstance = serviceInstance
			});
			return serviceInstanceInfo.GetServiceInstanceInfoResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<GetServiceInstanceInfoResponse> IFederatedServiceOnboarding.GetServiceInstanceInfoAsync(GetServiceInstanceInfoRequest request)
		{
			return base.Channel.GetServiceInstanceInfoAsync(request);
		}

		public Task<GetServiceInstanceInfoResponse> GetServiceInstanceInfoAsync(string serviceInstance)
		{
			return ((IFederatedServiceOnboarding)this).GetServiceInstanceInfoAsync(new GetServiceInstanceInfoRequest
			{
				serviceInstance = serviceInstance
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		SetServiceInstanceInfoResponse IFederatedServiceOnboarding.SetServiceInstanceInfo(SetServiceInstanceInfoRequest request)
		{
			return base.Channel.SetServiceInstanceInfo(request);
		}

		public ResultCode SetServiceInstanceInfo(string serviceInstance, ServiceInstanceInfoValue serviceInstanceInfo)
		{
			SetServiceInstanceInfoResponse setServiceInstanceInfoResponse = ((IFederatedServiceOnboarding)this).SetServiceInstanceInfo(new SetServiceInstanceInfoRequest
			{
				serviceInstance = serviceInstance,
				serviceInstanceInfo = serviceInstanceInfo
			});
			return setServiceInstanceInfoResponse.SetServiceInstanceInfoResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<SetServiceInstanceInfoResponse> IFederatedServiceOnboarding.SetServiceInstanceInfoAsync(SetServiceInstanceInfoRequest request)
		{
			return base.Channel.SetServiceInstanceInfoAsync(request);
		}

		public Task<SetServiceInstanceInfoResponse> SetServiceInstanceInfoAsync(string serviceInstance, ServiceInstanceInfoValue serviceInstanceInfo)
		{
			return ((IFederatedServiceOnboarding)this).SetServiceInstanceInfoAsync(new SetServiceInstanceInfoRequest
			{
				serviceInstance = serviceInstance,
				serviceInstanceInfo = serviceInstanceInfo
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		QueueEduFaultinResponse IFederatedServiceOnboarding.QueueEduFaultin(QueueEduFaultinRequest request)
		{
			return base.Channel.QueueEduFaultin(request);
		}

		public ExchangeFaultinStatus QueueEduFaultin(string serviceInstance, string contextId)
		{
			QueueEduFaultinResponse queueEduFaultinResponse = ((IFederatedServiceOnboarding)this).QueueEduFaultin(new QueueEduFaultinRequest
			{
				serviceInstance = serviceInstance,
				contextId = contextId
			});
			return queueEduFaultinResponse.QueueEduFaultinResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<QueueEduFaultinResponse> IFederatedServiceOnboarding.QueueEduFaultinAsync(QueueEduFaultinRequest request)
		{
			return base.Channel.QueueEduFaultinAsync(request);
		}

		public Task<QueueEduFaultinResponse> QueueEduFaultinAsync(string serviceInstance, string contextId)
		{
			return ((IFederatedServiceOnboarding)this).QueueEduFaultinAsync(new QueueEduFaultinRequest
			{
				serviceInstance = serviceInstance,
				contextId = contextId
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GetEduFaultinStatusResponse IFederatedServiceOnboarding.GetEduFaultinStatus(GetEduFaultinStatusRequest request)
		{
			return base.Channel.GetEduFaultinStatus(request);
		}

		public ExchangeFaultinStatus[] GetEduFaultinStatus(string[] contextIds)
		{
			GetEduFaultinStatusResponse eduFaultinStatus = ((IFederatedServiceOnboarding)this).GetEduFaultinStatus(new GetEduFaultinStatusRequest
			{
				contextIds = contextIds
			});
			return eduFaultinStatus.GetEduFaultinStatusResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<GetEduFaultinStatusResponse> IFederatedServiceOnboarding.GetEduFaultinStatusAsync(GetEduFaultinStatusRequest request)
		{
			return base.Channel.GetEduFaultinStatusAsync(request);
		}

		public Task<GetEduFaultinStatusResponse> GetEduFaultinStatusAsync(string[] contextIds)
		{
			return ((IFederatedServiceOnboarding)this).GetEduFaultinStatusAsync(new GetEduFaultinStatusRequest
			{
				contextIds = contextIds
			});
		}
	}
}
