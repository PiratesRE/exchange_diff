using System;

namespace Microsoft.Exchange.Security
{
	internal class LsaPolicy
	{
		internal static int GetDomainMembershipStatus(out bool memberOfDomain)
		{
			memberOfDomain = false;
			LsaNativeMethods.LsaObjectAttributes objectAttributes = new LsaNativeMethods.LsaObjectAttributes();
			SafeLsaPolicyHandle safeLsaPolicyHandle;
			int num = LsaNativeMethods.LsaOpenPolicy(null, objectAttributes, LsaNativeMethods.PolicyAccess.ViewLocalInformation, out safeLsaPolicyHandle);
			if (num != 0)
			{
				return LsaNativeMethods.LsaNtStatusToWinError(num);
			}
			SafeLsaMemoryHandle safeLsaMemoryHandle;
			num = LsaNativeMethods.LsaQueryInformationPolicy(safeLsaPolicyHandle, LsaNativeMethods.PolicyInformationClass.DnsDomainInformation, out safeLsaMemoryHandle);
			if (num != 0)
			{
				safeLsaPolicyHandle.Dispose();
				return LsaNativeMethods.LsaNtStatusToWinError(num);
			}
			LsaNativeMethods.PolicyDnsDomainInfo policyDnsDomainInfo = new LsaNativeMethods.PolicyDnsDomainInfo(safeLsaMemoryHandle);
			safeLsaPolicyHandle.Dispose();
			safeLsaMemoryHandle.Dispose();
			memberOfDomain = policyDnsDomainInfo.IsDomainMember;
			return 0;
		}
	}
}
