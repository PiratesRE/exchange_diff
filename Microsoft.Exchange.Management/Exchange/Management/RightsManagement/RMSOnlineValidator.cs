using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RMSOnlineValidator
	{
		public RMSOnlineValidator(IConfigurationSession configurationSession, IConfigurationSession dataSession, OrganizationId organizationId, Guid rmsOnlineGuidOverride, string authenticationCertificateSubjectNameOverride = null)
		{
			RmsUtil.ThrowIfParameterNull(dataSession, "configurationSession");
			RmsUtil.ThrowIfParameterNull(dataSession, "dataSession");
			RmsUtil.ThrowIfParameterNull(organizationId, "organizationId");
			this.configurationSession = configurationSession;
			this.dataSession = dataSession;
			this.organizationId = organizationId;
			this.rmsOnlineGuidOverride = rmsOnlineGuidOverride;
			if (authenticationCertificateSubjectNameOverride != null)
			{
				this.authenticationCertificateSubjectName = authenticationCertificateSubjectNameOverride;
			}
		}

		public IRMConfigurationValidationResult Validate()
		{
			try
			{
				if (!this.ValidateIsTenantContext())
				{
					return this.result;
				}
				IRMConfiguration irmconfiguration;
				if (!this.ValidateLoadTenantsIRMConfiguration(out irmconfiguration))
				{
					return this.result;
				}
				if (!this.ValidateRmsOnlinePrerequisites(irmconfiguration))
				{
					return this.result;
				}
				RmsOnlineTpdImporter tpdImporter = new RmsOnlineTpdImporter(irmconfiguration.RMSOnlineKeySharingLocation, this.authenticationCertificateSubjectName);
				if (!this.ValidateRmsOnlineAuthenticationCertificate(tpdImporter))
				{
					return this.result;
				}
				TrustedDocDomain tpd;
				if (!this.ValidateTPDCanBeObtainedFromRMSOnline(tpdImporter, out tpd))
				{
					return this.result;
				}
				if (!this.ValidateTpdSuitableForImport(irmconfiguration, tpd, tpdImporter))
				{
					return this.result;
				}
			}
			finally
			{
				this.result.SetOverallResult();
				this.result.PrepareFinalOutput(this.organizationId);
			}
			return this.result;
		}

		private bool ValidateIsTenantContext()
		{
			this.result.SetTask(Strings.InfoCheckingOrganizationContext);
			if (OrganizationId.ForestWideOrgId == this.organizationId)
			{
				return this.result.SetFailureResult(Strings.ErrorNotRunningAsTenantAdmin, null, true);
			}
			return this.result.SetSuccessResult(Strings.InfoOrganizationContextChecked);
		}

		private bool ValidateLoadTenantsIRMConfiguration(out IRMConfiguration irmConfiguration)
		{
			irmConfiguration = null;
			this.result.SetTask(Strings.InfoLoadIRMConfig);
			irmConfiguration = IRMConfiguration.Read(this.dataSession);
			return this.result.SetSuccessResult(Strings.InfoIRMConfigLoaded);
		}

		private bool ValidateRmsOnlinePrerequisites(IRMConfiguration irmConfiguration)
		{
			this.result.SetTask(Strings.InfoCheckingRmsOnlinePrerequisites);
			if (!RmsUtil.AreRmsOnlinePreRequisitesMet(irmConfiguration))
			{
				return this.result.SetFailureResult(Strings.ErrorRmsOnlinePrerequisites, null, true);
			}
			return this.result.SetSuccessResult(Strings.InfoRmsOnlinePrerequisitesChecked);
		}

		private bool ValidateRmsOnlineAuthenticationCertificate(RmsOnlineTpdImporter tpdImporter)
		{
			this.result.SetTask(Strings.InfoCheckingRmsOnlineAuthenticationCertificate);
			try
			{
				X509Certificate2 x509Certificate = tpdImporter.LoadAuthenticationCertificate();
				DateTime t = (DateTime)ExDateTime.UtcNow;
				if (x509Certificate.NotBefore > t)
				{
					return this.result.SetFailureResult(Strings.ErrorRmsOnlineAuthenticationCertificateNotYetValid, null, true);
				}
				if (x509Certificate.NotAfter < t)
				{
					return this.result.SetFailureResult(Strings.ErrorRmsOnlineAuthenticationCertificateExpired, null, true);
				}
				if (x509Certificate.NotAfter - this.certificateWarningPeriod < t)
				{
					return this.result.SetFailureResult(Strings.WarningRmsOnlineAuthenticationCertificateExpiryApproaching(x509Certificate.NotAfter), null, false);
				}
			}
			catch (ImportTpdException ex)
			{
				return this.result.SetFailureResult(Strings.ErrorRmsOnlineAuthenticationCertificateNotFound, ex, true);
			}
			return this.result.SetSuccessResult(Strings.InfoRmsOnlineAuthenticationCertificateChecked);
		}

		private bool ValidateTPDCanBeObtainedFromRMSOnline(RmsOnlineTpdImporter tpdImporter, out TrustedDocDomain tpd)
		{
			tpd = null;
			this.result.SetTask(Strings.InfoImportingTpdFromRmsOnline);
			bool flag;
			try
			{
				Guid externalDirectoryOrgIdThrowOnFailure = this.rmsOnlineGuidOverride;
				if (Guid.Empty == externalDirectoryOrgIdThrowOnFailure)
				{
					externalDirectoryOrgIdThrowOnFailure = RmsUtil.GetExternalDirectoryOrgIdThrowOnFailure(this.configurationSession, this.organizationId);
				}
				tpd = tpdImporter.Import(externalDirectoryOrgIdThrowOnFailure);
				if (tpd.m_astrRightsTemplates.Length == 0)
				{
					flag = this.result.SetSuccessResult(Strings.InfoImportingTpdFromRmsOnlineCheckedNoTemplates);
				}
				else
				{
					flag = this.result.SetSuccessResult(Strings.InfoImportingTpdFromRmsOnlineCheckedWithTemplates(RmsUtil.TemplateNamesFromTemplateArray(tpd.m_astrRightsTemplates)));
				}
			}
			catch (ImportTpdException ex)
			{
				flag = this.result.SetFailureResult(Strings.ErrorImportingTpdFromRmsOnline, ex, true);
			}
			return flag;
		}

		private bool ValidateTpdSuitableForImport(IRMConfiguration irmConfiguration, TrustedDocDomain tpd, RmsOnlineTpdImporter tpdImporter)
		{
			this.result.SetTask(Strings.InfoCheckingTpdFromRmsOnline);
			TpdValidator tpdValidator = new TpdValidator(irmConfiguration.InternalLicensingEnabled, tpdImporter.IntranetLicensingUrl, tpdImporter.ExtranetLicensingUrl, tpdImporter.IntranetCertificationUrl, tpdImporter.ExtranetCertificationUrl, true, true, false);
			try
			{
				object obj;
				tpdValidator.ValidateTpdSuitableForImport(tpd, Strings.RmsOnline, out obj, null, null, null, null, null, null);
			}
			catch (LocalizedException ex)
			{
				return this.result.SetFailureResult(Strings.ErrorTpdCheckingFailed, ex, true);
			}
			return this.result.SetSuccessResult(Strings.InfoTpdFromRmsOnlineChecked);
		}

		private readonly TimeSpan certificateWarningPeriod = TimeSpan.FromDays(30.0);

		private readonly IConfigurationSession configurationSession;

		private readonly IConfigurationSession dataSession;

		private readonly OrganizationId organizationId;

		private readonly Guid rmsOnlineGuidOverride;

		private readonly string authenticationCertificateSubjectName = RmsOnlineConstants.AuthenticationCertificateSubjectDistinguishedName;

		private readonly IRMConfigurationValidationResult result = new IRMConfigurationValidationResult();
	}
}
