using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal static class Errors
	{
		[SecurityTreatAsSafe]
		[SecurityCritical]
		internal static void ThrowOnErrorCode(int hr)
		{
			Errors.ThrowOnErrorCode(hr, LocalizedString.Empty);
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		internal static void ThrowOnErrorCode(int hr, LocalizedString contextMessage)
		{
			if (hr >= 0)
			{
				return;
			}
			if (!Errors.IsRightsManagementFailureCodeDefined((RightsManagementFailureCode)hr))
			{
				try
				{
					Marshal.ThrowExceptionForHR(hr);
				}
				catch (Exception innerException)
				{
					throw new RightsManagementException(RightsManagementFailureCode.UnknownDRMFailure, (contextMessage == LocalizedString.Empty) ? DrmStrings.RmExceptionGenericMessage : contextMessage, innerException);
				}
				return;
			}
			if (contextMessage == LocalizedString.Empty)
			{
				throw new RightsManagementException((RightsManagementFailureCode)hr, new LocalizedString(((RightsManagementFailureCode)hr).ToString()));
			}
			throw new RightsManagementException((RightsManagementFailureCode)hr, contextMessage);
		}

		private static bool IsRightsManagementFailureCodeDefined(RightsManagementFailureCode failureCode)
		{
			if (failureCode != RightsManagementFailureCode.ManifestPolicyViolation)
			{
				switch (failureCode)
				{
				case RightsManagementFailureCode.InvalidLicense:
				case RightsManagementFailureCode.InfoNotInLicense:
				case RightsManagementFailureCode.InvalidLicenseSignature:
				case RightsManagementFailureCode.EncryptionNotPermitted:
				case RightsManagementFailureCode.UserRightNotGranted:
				case RightsManagementFailureCode.InvalidVersion:
				case RightsManagementFailureCode.InvalidEncodingType:
				case RightsManagementFailureCode.InvalidNumericalValue:
				case RightsManagementFailureCode.InvalidAlgorithmType:
				case RightsManagementFailureCode.EnvironmentNotLoaded:
				case RightsManagementFailureCode.EnvironmentCannotLoad:
				case RightsManagementFailureCode.TooManyLoadedEnvironments:
				case RightsManagementFailureCode.IncompatibleObjects:
				case RightsManagementFailureCode.LibraryFail:
				case RightsManagementFailureCode.EnablingPrincipalFailure:
				case RightsManagementFailureCode.InfoNotPresent:
				case RightsManagementFailureCode.BadGetInfoQuery:
				case RightsManagementFailureCode.KeyTypeUnsupported:
				case RightsManagementFailureCode.CryptoOperationUnsupported:
				case RightsManagementFailureCode.ClockRollbackDetected:
				case RightsManagementFailureCode.QueryReportsNoResults:
				case RightsManagementFailureCode.UnexpectedException:
				case RightsManagementFailureCode.BindValidityTimeViolated:
				case RightsManagementFailureCode.BrokenCertChain:
				case RightsManagementFailureCode.BindPolicyViolation:
				case RightsManagementFailureCode.BindRevokedLicense:
				case RightsManagementFailureCode.BindRevokedIssuer:
				case RightsManagementFailureCode.BindRevokedPrincipal:
				case RightsManagementFailureCode.BindRevokedResource:
				case RightsManagementFailureCode.BindRevokedModule:
				case RightsManagementFailureCode.BindContentNotInEndUseLicense:
				case RightsManagementFailureCode.BindAccessPrincipalNotEnabling:
				case RightsManagementFailureCode.BindAccessUnsatisfied:
				case RightsManagementFailureCode.BindIndicatedPrincipalMissing:
				case RightsManagementFailureCode.BindMachineNotFoundInGroupIdentity:
				case RightsManagementFailureCode.LibraryUnsupportedPlugIn:
				case RightsManagementFailureCode.BindRevocationListStale:
				case RightsManagementFailureCode.BindNoApplicableRevocationList:
				case RightsManagementFailureCode.InvalidHandle:
				case RightsManagementFailureCode.BindIntervalTimeViolated:
				case RightsManagementFailureCode.BindNoSatisfiedRightsGroup:
				case RightsManagementFailureCode.BindSpecifiedWorkMissing:
				case RightsManagementFailureCode.NoMoreData:
				case RightsManagementFailureCode.LicenseAcquisitionFailed:
				case RightsManagementFailureCode.IdMismatch:
				case RightsManagementFailureCode.TooManyCertificates:
				case RightsManagementFailureCode.NoDistributionPointUrlFound:
				case RightsManagementFailureCode.AlreadyInProgress:
				case RightsManagementFailureCode.GroupIdentityNotSet:
				case RightsManagementFailureCode.RecordNotFound:
				case RightsManagementFailureCode.NoConnect:
				case RightsManagementFailureCode.NoLicense:
				case RightsManagementFailureCode.NeedsMachineActivation:
				case RightsManagementFailureCode.NeedsGroupIdentityActivation:
				case RightsManagementFailureCode.ActivationFailed:
				case RightsManagementFailureCode.Aborted:
				case RightsManagementFailureCode.OutOfQuota:
				case RightsManagementFailureCode.AuthenticationFailed:
				case RightsManagementFailureCode.ServerError:
				case RightsManagementFailureCode.InstallationFailed:
				case RightsManagementFailureCode.HidCorrupted:
				case RightsManagementFailureCode.InvalidServerResponse:
				case RightsManagementFailureCode.ServiceNotFound:
				case RightsManagementFailureCode.UseDefault:
				case RightsManagementFailureCode.ServerNotFound:
				case RightsManagementFailureCode.InvalidEmail:
				case RightsManagementFailureCode.ValidityTimeViolation:
				case RightsManagementFailureCode.OutdatedModule:
				case RightsManagementFailureCode.NotSet:
				case RightsManagementFailureCode.MetadataNotSet:
				case RightsManagementFailureCode.RevocationInfoNotSet:
				case RightsManagementFailureCode.InvalidTimeInfo:
				case RightsManagementFailureCode.RightNotSet:
				case RightsManagementFailureCode.LicenseBindingToWindowsIdentityFailed:
				case RightsManagementFailureCode.InvalidIssuanceLicenseTemplate:
				case RightsManagementFailureCode.InvalidKeyLength:
				case RightsManagementFailureCode.ExpiredOfficialIssuanceLicenseTemplate:
				case RightsManagementFailureCode.InvalidClientLicensorCertificate:
				case RightsManagementFailureCode.HidInvalid:
				case RightsManagementFailureCode.EmailNotVerified:
				case RightsManagementFailureCode.ServiceMoved:
				case RightsManagementFailureCode.ServiceGone:
				case RightsManagementFailureCode.AdEntryNotFound:
				case RightsManagementFailureCode.NotAChain:
				case RightsManagementFailureCode.RequestDenied:
				case RightsManagementFailureCode.DebuggerDetected:
				case RightsManagementFailureCode.InvalidLockboxType:
				case RightsManagementFailureCode.InvalidLockboxPath:
				case RightsManagementFailureCode.InvalidRegistryPath:
				case RightsManagementFailureCode.NoAesCryptoProvider:
				case RightsManagementFailureCode.GlobalOptionAlreadySet:
				case RightsManagementFailureCode.OwnerLicenseNotFound:
					return true;
				case (RightsManagementFailureCode)(-2147168509):
				case (RightsManagementFailureCode)(-2147168499):
				case (RightsManagementFailureCode)(-2147168486):
				case (RightsManagementFailureCode)(-2147168471):
				case (RightsManagementFailureCode)(-2147168470):
				case (RightsManagementFailureCode)(-2147168469):
				case (RightsManagementFailureCode)(-2147168467):
				case (RightsManagementFailureCode)(-2147168466):
				case (RightsManagementFailureCode)(-2147168462):
				case (RightsManagementFailureCode)(-2147168449):
				case (RightsManagementFailureCode)(-2147168426):
				case (RightsManagementFailureCode)(-2147168415):
				case (RightsManagementFailureCode)(-2147168414):
				case (RightsManagementFailureCode)(-2147168413):
				case (RightsManagementFailureCode)(-2147168412):
				case (RightsManagementFailureCode)(-2147168411):
				case (RightsManagementFailureCode)(-2147168410):
				case (RightsManagementFailureCode)(-2147168409):
				case (RightsManagementFailureCode)(-2147168408):
				case (RightsManagementFailureCode)(-2147168407):
				case (RightsManagementFailureCode)(-2147168406):
				case (RightsManagementFailureCode)(-2147168405):
				case (RightsManagementFailureCode)(-2147168404):
				case (RightsManagementFailureCode)(-2147168403):
				case (RightsManagementFailureCode)(-2147168402):
				case (RightsManagementFailureCode)(-2147168401):
					break;
				default:
					if (failureCode == RightsManagementFailureCode.InsufficientBuffer)
					{
						return true;
					}
					break;
				}
				return false;
			}
			return true;
		}
	}
}
