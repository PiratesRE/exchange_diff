using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GlobalConfigurationCacheProvider : IGlobalConfigurationAndCacheProvider
	{
		public TraceLevel DiagnosticTraceLevel
		{
			get
			{
				return TraceLevel.Off;
			}
		}

		public void CheckInIssuanceLicense(string signatureKey, ISizeTraceableItem issuanceLicense)
		{
		}

		public ISizeTraceableItem CheckoutIssuanceLicense(string signatureKey)
		{
			return null;
		}

		public void AddRejectedUser(string signatureKey, string rightsAccountCertificatePrincipal)
		{
		}

		public bool IsRejectedUser(string signatureKey, string rightsAccountCertificatePrincipal)
		{
			return false;
		}

		public int MaxNumberOfThreads
		{
			get
			{
				return 20;
			}
		}

		private const int MaximumThreadNumberForCSPInstanceCaching = 20;
	}
}
