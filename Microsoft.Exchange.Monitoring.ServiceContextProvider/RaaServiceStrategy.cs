using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Forefront.RecoveryActionArbiter.Contract;

namespace Microsoft.Exchange.Monitoring.ServiceContextProvider
{
	public class RaaServiceStrategy : IRaaService
	{
		internal RaaServiceStrategy()
		{
			string forefrontArbitrationServiceUrl = DatacenterRegistry.GetForefrontArbitrationServiceUrl();
			if (forefrontArbitrationServiceUrl != string.Empty)
			{
				this.ServiceBinding = new NetTcpBinding();
				EndpointAddress serviceAddress = new EndpointAddress(forefrontArbitrationServiceUrl);
				this.ServiceAddress = serviceAddress;
				return;
			}
			throw new ArgumentException("The ForefrontArbitrationServiceUrl was not set.");
		}

		internal RaaServiceStrategy(Binding binding, EndpointAddress endpointAddress)
		{
			this.ServiceBinding = binding;
			this.ServiceAddress = endpointAddress;
		}

		private RaaServiceProxy ServiceProxy
		{
			get
			{
				if (this.raaServiceProxy == null)
				{
					this.CreateProxy();
				}
				return this.raaServiceProxy;
			}
		}

		private EndpointAddress ServiceAddress { get; set; }

		private Binding ServiceBinding { get; set; }

		public ApprovalResponse RequestApprovalForRecovery(ApprovalRequest request)
		{
			return this.RequestApprovalForRecovery(request, true);
		}

		public void NotifyRecoveryCompletion(string machineName, bool successfulRecovery)
		{
			this.NotifyRecoveryCompletion(machineName, successfulRecovery, true);
		}

		public ICollection<AvailabilityData> GetRoleAvailabilityData(string serviceInstance, string role)
		{
			return this.GetRoleAvailabilityData(serviceInstance, role, true);
		}

		private void CreateProxy()
		{
			if (this.raaServiceProxy != null)
			{
				try
				{
					this.raaServiceProxy.Close();
				}
				catch (CommunicationException)
				{
					this.raaServiceProxy.Abort();
				}
				catch (TimeoutException)
				{
					this.raaServiceProxy.Abort();
				}
				catch (Exception)
				{
					this.raaServiceProxy.Abort();
					throw;
				}
			}
			if (this.ServiceAddress == null)
			{
				throw new ArgumentException("The RAA service address was not set.");
			}
			if (this.ServiceBinding == null)
			{
				throw new ArgumentException("The RAA service binding was not set.");
			}
			this.raaServiceProxy = new RaaServiceProxy(this.ServiceBinding, this.ServiceAddress);
		}

		private ICollection<AvailabilityData> GetRoleAvailabilityData(string serviceInstance, string role, bool firstTry)
		{
			ICollection<AvailabilityData> roleAvailabilityData;
			try
			{
				roleAvailabilityData = this.ServiceProxy.GetRoleAvailabilityData(serviceInstance, role);
			}
			catch (Exception ex)
			{
				if (!(ex is CommunicationException) && !(ex is TimeoutException))
				{
					throw;
				}
				this.CreateProxy();
				if (!firstTry)
				{
					throw;
				}
				roleAvailabilityData = this.GetRoleAvailabilityData(serviceInstance, role, false);
			}
			return roleAvailabilityData;
		}

		private ApprovalResponse RequestApprovalForRecovery(ApprovalRequest request, bool firstTry)
		{
			ApprovalResponse result;
			try
			{
				result = this.ServiceProxy.RequestApprovalForRecovery(request);
			}
			catch (Exception ex)
			{
				if (!(ex is CommunicationException) && !(ex is TimeoutException))
				{
					throw;
				}
				this.CreateProxy();
				if (!firstTry)
				{
					throw;
				}
				result = this.RequestApprovalForRecovery(request, false);
			}
			return result;
		}

		private void NotifyRecoveryCompletion(string machineName, bool successfulRecovery, bool firstTry)
		{
			try
			{
				this.ServiceProxy.NotifyRecoveryCompletion(machineName, true);
			}
			catch (Exception ex)
			{
				if (!(ex is CommunicationException) && !(ex is TimeoutException))
				{
					throw;
				}
				this.CreateProxy();
				if (!firstTry)
				{
					throw;
				}
				this.NotifyRecoveryCompletion(machineName, successfulRecovery, false);
			}
		}

		private RaaServiceProxy raaServiceProxy;
	}
}
