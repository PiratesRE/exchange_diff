using System;
using System.Collections.Generic;
using Microsoft.Forefront.RecoveryActionArbiter.Contract;

namespace Microsoft.Exchange.Monitoring.ServiceContextProvider
{
	public class RaaServiceNoOpStrategy : IRaaService
	{
		public ApprovalResponse RequestApprovalForRecovery(ApprovalRequest request)
		{
			return new ApprovalResponse
			{
				ArbitrationResult = 1
			};
		}

		public void NotifyRecoveryCompletion(string machineName, bool successfulRecovery)
		{
		}

		public ICollection<AvailabilityData> GetRoleAvailabilityData(string serviceInstance, string role)
		{
			return new List<AvailabilityData>();
		}
	}
}
