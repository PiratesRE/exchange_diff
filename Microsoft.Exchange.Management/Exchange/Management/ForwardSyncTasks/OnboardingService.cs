using System;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public abstract class OnboardingService : DisposeTrackableBase
	{
		protected OnboardingService()
		{
			this.InitializeSyncServiceProxy();
		}

		public ServiceInstanceMapValue GetServiceInstanceMap()
		{
			GetServiceInstanceMapRequest request = new GetServiceInstanceMapRequest("EXCHANGE");
			GetServiceInstanceMapResponse serviceInstanceMap = this.onboardingProxy.GetServiceInstanceMap(request);
			return serviceInstanceMap.GetServiceInstanceMapResult;
		}

		public ResultCode SetServiceInstanceMap(ServiceInstanceMapValue serviceInstanceMap)
		{
			SetServiceInstanceMapRequest serviceInstanceMap2 = new SetServiceInstanceMapRequest("EXCHANGE", serviceInstanceMap);
			SetServiceInstanceMapResponse setServiceInstanceMapResponse = this.onboardingProxy.SetServiceInstanceMap(serviceInstanceMap2);
			return setServiceInstanceMapResponse.SetServiceInstanceMapResult;
		}

		public ServiceInstanceInfoValue GetServiceInstanceInfo(string serviceInstance)
		{
			GetServiceInstanceInfoRequest request = new GetServiceInstanceInfoRequest(serviceInstance);
			GetServiceInstanceInfoResponse serviceInstanceInfo = this.onboardingProxy.GetServiceInstanceInfo(request);
			return serviceInstanceInfo.GetServiceInstanceInfoResult;
		}

		public ResultCode SetServiceInstanceInfo(string serviceInstance, ServiceInstanceInfoValue serviceInstanceInfo)
		{
			SetServiceInstanceInfoRequest serviceInstanceInfo2 = new SetServiceInstanceInfoRequest(serviceInstance, serviceInstanceInfo);
			SetServiceInstanceInfoResponse setServiceInstanceInfoResponse = this.onboardingProxy.SetServiceInstanceInfo(serviceInstanceInfo2);
			return setServiceInstanceInfoResponse.SetServiceInstanceInfoResult;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OnboardingService>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeSyncServiceProxy();
			}
		}

		protected abstract IFederatedServiceOnboarding CreateService();

		private void InitializeSyncServiceProxy()
		{
			if (this.onboardingProxy == null)
			{
				this.onboardingProxy = this.CreateService();
			}
		}

		private void DisposeSyncServiceProxy()
		{
			if (this.onboardingProxy != null)
			{
				IDisposable disposable = this.onboardingProxy as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				this.onboardingProxy = null;
			}
		}

		private const string ServiceType = "EXCHANGE";

		private IFederatedServiceOnboarding onboardingProxy;
	}
}
