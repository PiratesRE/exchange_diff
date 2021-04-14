using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NoopWSManagerPerfCounters : IWSManagerPerfCounters
	{
		private NoopWSManagerPerfCounters()
		{
		}

		public void CertifySuccessful(long elapsedMilliseconds)
		{
		}

		public void CertifyFailed()
		{
		}

		public void GetClientLicensorCertSuccessful(long elapsedMilliseconds)
		{
		}

		public void GetClientLicensorCertFailed()
		{
		}

		public void AcquireLicenseSuccessful(long elapsedMilliseconds)
		{
		}

		public void AcquireLicenseFailed()
		{
		}

		public void AcquirePreLicenseSuccessful(long elapsedMilliseconds)
		{
		}

		public void AcquirePreLicenseFailed()
		{
		}

		public void AcquireTemplatesSuccessful(long elapsedMilliseconds)
		{
		}

		public void AcquireTemplatesFailed()
		{
		}

		public void FindServiceLocationsSuccessful(long elapsedMilliseconds)
		{
		}

		public void FindServiceLocationsFailed()
		{
		}

		public void WCFCertifySuccessful()
		{
		}

		public void WCFCertifyFailed()
		{
		}

		public void WCFAcquireServerLicenseSuccessful()
		{
		}

		public void WCFAcquireServerLicenseFailed()
		{
		}

		public static NoopWSManagerPerfCounters Instance = new NoopWSManagerPerfCounters();
	}
}
