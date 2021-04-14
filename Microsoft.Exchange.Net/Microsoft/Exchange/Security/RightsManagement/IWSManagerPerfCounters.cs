using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IWSManagerPerfCounters
	{
		void CertifySuccessful(long elapsedMilliseconds);

		void CertifyFailed();

		void GetClientLicensorCertSuccessful(long elapsedMilliseconds);

		void GetClientLicensorCertFailed();

		void AcquireLicenseSuccessful(long elapsedMilliseconds);

		void AcquireLicenseFailed();

		void AcquirePreLicenseSuccessful(long elapsedMilliseconds);

		void AcquirePreLicenseFailed();

		void AcquireTemplatesSuccessful(long elapsedMilliseconds);

		void AcquireTemplatesFailed();

		void FindServiceLocationsSuccessful(long elapsedMilliseconds);

		void FindServiceLocationsFailed();

		void WCFCertifySuccessful();

		void WCFCertifyFailed();

		void WCFAcquireServerLicenseSuccessful();

		void WCFAcquireServerLicenseFailed();
	}
}
