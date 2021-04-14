using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Forefront.RecoveryActionArbiter.Contract;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ServiceContextProvider
{
	public sealed class ServiceContextProvider
	{
		private ServiceContextProvider()
		{
			string forefrontArbitrationServiceUrl = DatacenterRegistry.GetForefrontArbitrationServiceUrl();
			if (DatacenterRegistry.IsForefrontForOffice() && forefrontArbitrationServiceUrl != string.Empty)
			{
				this.SetRaaServiceStrategy(new RaaServiceStrategy());
				return;
			}
			this.SetRaaServiceStrategy(new RaaServiceNoOpStrategy());
		}

		public static ServiceContextProvider Instance
		{
			get
			{
				if (ServiceContextProvider.instance == null)
				{
					lock (ServiceContextProvider.syncRoot)
					{
						if (ServiceContextProvider.instance == null)
						{
							ServiceContextProvider.instance = new ServiceContextProvider();
						}
					}
				}
				return ServiceContextProvider.instance;
			}
		}

		internal static RegistryKey ServiceContextProviderRegistryKey
		{
			get
			{
				RegistryKey registryKey;
				if (ServiceContextProvider.UseExchangeLabsRegistryKey)
				{
					registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ExchangeLabs", true);
				}
				else
				{
					registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft", true);
				}
				using (registryKey)
				{
					if (registryKey != null)
					{
						RegistryKey registryKey3 = registryKey.OpenSubKey("ServiceContextProvider", true);
						if (registryKey3 == null)
						{
							registryKey3 = registryKey.CreateSubKey("ServiceContextProvider");
						}
						return registryKey3;
					}
				}
				return null;
			}
		}

		internal static bool UseExchangeLabsRegistryKey { get; set; }

		public static bool RecoveryRequestExists(string recoveryID)
		{
			bool result;
			using (RegistryKey registryKey = ServiceContextProvider.ServiceContextProviderRegistryKey.OpenSubKey(recoveryID.ToString()))
			{
				result = (registryKey != null);
			}
			return result;
		}

		public bool RequestApprovalForRecovery(string recoveryID, RecoveryFlags recoveryFlags, string failureReason)
		{
			return this.RequestApprovalForRecovery(recoveryID, 2, 1, 0, recoveryFlags, failureReason, "");
		}

		public bool RequestApprovalForRecovery(string recoveryID, ArbitrationScope scope, ArbitrationSource source, RequestedAction requestedAction, RecoveryFlags recoveryFlags, string failureReason, string machineName = "")
		{
			ApprovalRequest approvalRequest = new ApprovalRequest();
			approvalRequest.MachineName = (string.IsNullOrEmpty(machineName) ? Environment.MachineName : machineName);
			approvalRequest.RecoveryFlags = recoveryFlags;
			approvalRequest.FailureReason = failureReason;
			approvalRequest.ArbitrationScope = scope;
			approvalRequest.ArbitrationSource = source;
			approvalRequest.RequestedAction = requestedAction;
			approvalRequest.FailureCategory = 4;
			ApprovalResponse approvalResponse = this.raaServiceStrategy.RequestApprovalForRecovery(approvalRequest);
			if (approvalResponse.ArbitrationResult == 1)
			{
				ServiceContextProvider.SaveRecoveryRequest(recoveryID);
				return true;
			}
			return false;
		}

		public void NotifyRecoveryCompletion(string recoveryID, bool recoverySucceeded, string machineName = "")
		{
			if (ServiceContextProvider.RecoveryRequestExists(recoveryID))
			{
				if (ServiceContextProvider.RecoveryRequestCount() == 1)
				{
					this.raaServiceStrategy.NotifyRecoveryCompletion(string.IsNullOrEmpty(machineName) ? Environment.MachineName : machineName, recoverySucceeded);
				}
				ServiceContextProvider.RemoveRecoveryRequest(recoveryID);
			}
		}

		internal void SetRaaServiceStrategy(IRaaService strategy)
		{
			this.raaServiceStrategy = strategy;
		}

		private static void SaveRecoveryRequest(string recoveryID)
		{
			using (RegistryKey serviceContextProviderRegistryKey = ServiceContextProvider.ServiceContextProviderRegistryKey)
			{
				RegistryKey registryKey = serviceContextProviderRegistryKey.CreateSubKey(recoveryID.ToString());
				registryKey.Dispose();
			}
		}

		private static void RemoveRecoveryRequest(string recoveryID)
		{
			ServiceContextProvider.ServiceContextProviderRegistryKey.DeleteSubKey(recoveryID.ToString());
		}

		private static int RecoveryRequestCount()
		{
			int subKeyCount;
			using (RegistryKey serviceContextProviderRegistryKey = ServiceContextProvider.ServiceContextProviderRegistryKey)
			{
				subKeyCount = serviceContextProviderRegistryKey.SubKeyCount;
			}
			return subKeyCount;
		}

		private static volatile ServiceContextProvider instance;

		private static object syncRoot = new object();

		private IRaaService raaServiceStrategy;
	}
}
