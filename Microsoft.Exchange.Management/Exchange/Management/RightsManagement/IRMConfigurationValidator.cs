using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Security.RightsManagement.SOAP.Server;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class IRMConfigurationValidator
	{
		public IRMConfigurationValidator(RmsClientManagerContext context, SmtpAddress sender, SmtpAddress[] recipients)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.organizationId = context.OrgId;
			this.senderAddress = sender.ToString();
			List<string> list = new List<string>();
			if (recipients != null)
			{
				foreach (SmtpAddress smtpAddress in recipients)
				{
					list.Add(smtpAddress.ToString());
				}
			}
			if (list.Count == 0)
			{
				this.recipientsAddress = new string[]
				{
					this.senderAddress
				};
			}
			else
			{
				this.recipientsAddress = list.ToArray();
			}
			this.context = context;
		}

		public IRMConfigurationValidationResult Validate()
		{
			IRMConfigurationValidationResult irmconfigurationValidationResult;
			try
			{
				this.result = new IRMConfigurationValidationResult();
				RmsClientManager.IgnoreLicensingEnabled = true;
				if (!this.TryIsDatacenter())
				{
					irmconfigurationValidationResult = this.result;
				}
				else if (!this.TryLoadIRMConfig())
				{
					irmconfigurationValidationResult = this.result;
				}
				else
				{
					this.GetTestRequirement();
					if (!this.TryGetRacAndClc())
					{
						irmconfigurationValidationResult = this.result;
					}
					else if (!this.TryGetTemplate())
					{
						irmconfigurationValidationResult = this.result;
					}
					else if (!this.TryGetULAndPreL())
					{
						irmconfigurationValidationResult = this.result;
					}
					else
					{
						if (!this.rmsConfig.IsInternalLicensingEnabledForTenant(this.organizationId))
						{
							this.result.SetFailureResult(Strings.WarningInternalLicensingDisabled, null, false);
						}
						irmconfigurationValidationResult = this.result;
					}
				}
			}
			finally
			{
				RmsClientManager.IgnoreLicensingEnabled = false;
				this.result.SetOverallResult();
				this.result.PrepareFinalOutput(this.organizationId);
				if (this.tenantLicenses != null)
				{
					this.tenantLicenses.Dispose();
					this.tenantLicenses = null;
				}
			}
			return irmconfigurationValidationResult;
		}

		private bool TryIsDatacenter()
		{
			this.result.SetTask(Strings.InfoCheckMode);
			try
			{
				this.datacenter = (Datacenter.IsMicrosoftHostedOnly(true) || Datacenter.IsPartnerHostedOnly(true));
				this.result.SetSuccessResult(this.datacenter ? Strings.InfoDatacenterMode : Strings.InfoEnterpriseMode);
			}
			catch (CannotDetermineExchangeModeException ex)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToCheckMode, ex, true);
				return false;
			}
			return true;
		}

		private bool TryLoadIRMConfig()
		{
			this.result.SetTask(Strings.InfoLoadIRMConfig);
			try
			{
				this.rmsConfig = RmsClientManager.IRMConfig;
				this.result.SetSuccessResult(Strings.InfoIRMConfigLoaded);
			}
			catch (ExchangeConfigurationException ex)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToLoadIRMConfig, ex, true);
				return false;
			}
			catch (RightsManagementException ex2)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToLoadIRMConfig, ex2, true);
				return false;
			}
			return true;
		}

		private void GetTestRequirement()
		{
			bool flag = this.rmsConfig.IsInternalLicensingEnabledForTenant(this.organizationId);
			this.encryptionEnabled = flag;
			this.prelicensingEnabled = flag;
			this.licensingEnabled = (flag && (this.rmsConfig.IsSearchEnabledForTenant(this.organizationId) || this.rmsConfig.IsClientAccessServerEnabledForTenant(this.organizationId) || this.rmsConfig.IsJournalReportDecryptionEnabledForTenant(this.organizationId) || this.rmsConfig.GetTenantTransportDecryptionSetting(this.organizationId) != TransportDecryptionSetting.Disabled));
		}

		private bool TryGetRacAndClc()
		{
			this.result.SetTask(Strings.InfoGetCertificationUri);
			try
			{
				this.certificationUri = RmsClientManager.GetRMSServiceLocation(this.organizationId, ServiceType.Certification);
				this.certificationUri = RmsoProxyUtil.GetCertificationServerRedirectUrl(this.certificationUri);
			}
			catch (RightsManagementException ex)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToGetCertificationUri, ex, this.encryptionEnabled);
				return false;
			}
			catch (ExchangeConfigurationException ex2)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToGetCertificationUri, ex2, this.encryptionEnabled);
				return false;
			}
			if (this.certificationUri == null)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToGetCertificationUri, null, this.encryptionEnabled);
				return false;
			}
			this.result.SetSuccessResult(Strings.InfoCertificationUri(this.certificationUri));
			this.result.SetTask(Strings.InfoCheckRmsVersion(this.certificationUri));
			try
			{
				if (this.ValidateRmsVersion(this.certificationUri, ServiceType.CertificationService))
				{
					this.result.SetSuccessResult(Strings.InfoRmsVersionChecked);
				}
				else
				{
					this.result.SetFailureResult(Strings.ErrorFailedRmsVersionCheck, null, this.encryptionEnabled);
				}
			}
			catch (RightsManagementException ex3)
			{
				this.result.SetFailureResult(Strings.ErrorFailedRmsVersionCheck, ex3, this.encryptionEnabled);
				return false;
			}
			catch (ExchangeConfigurationException ex4)
			{
				this.result.SetFailureResult(Strings.ErrorFailedRmsVersionCheckInitialization, ex4, this.encryptionEnabled);
				return false;
			}
			this.result.SetTask(Strings.InfoGetPublishingUri);
			try
			{
				this.publishingUri = RmsClientManager.GetRMSServiceLocation(this.organizationId, ServiceType.Publishing);
				this.publishingUri = RmsoProxyUtil.GetLicenseServerRedirectUrl(this.publishingUri);
			}
			catch (RightsManagementException ex5)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToGetPublishingUri, ex5, this.encryptionEnabled);
				return false;
			}
			catch (ExchangeConfigurationException ex6)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToGetPublishingUri, ex6, this.encryptionEnabled);
				return false;
			}
			if (this.publishingUri == null)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToGetPublishingUri, null, this.encryptionEnabled);
				return false;
			}
			this.result.SetSuccessResult(Strings.InfoPublishingUri(this.publishingUri));
			this.result.SetTask(Strings.InfoGetRacAndClc);
			try
			{
				this.tenantLicenses = RmsClientManager.AcquireTenantLicenses(this.context, this.publishingUri);
				this.result.SetSuccessResult(Strings.InfoRacAndClc);
			}
			catch (ExchangeConfigurationException ex7)
			{
				this.result.SetFailureResult(this.datacenter ? Strings.ErrorFailedToGetRacAndClcTenant : Strings.ErrorFailedToGetRacAndClcEnterprise, ex7, this.encryptionEnabled);
				return false;
			}
			catch (RightsManagementException ex8)
			{
				this.result.SetFailureResult(this.datacenter ? Strings.ErrorFailedToGetRacAndClcTenant : Strings.ErrorFailedToGetRacAndClcEnterprise, ex8, this.encryptionEnabled);
				return false;
			}
			return true;
		}

		private bool TryGetTemplate()
		{
			this.result.SetTask(Strings.InfoGetTemplate);
			try
			{
				RmsClientManager.AcquireRMSTemplate(this.context, RmsTemplate.DoNotForward.Id);
				this.result.SetSuccessResult(Strings.InfoTemplate);
			}
			catch (ExchangeConfigurationException ex)
			{
				this.result.SetFailureResult(this.datacenter ? Strings.ErrorFailedToGetTemplateTenant : Strings.ErrorFailedToGetTemplateEnterprise, ex, this.encryptionEnabled);
				return false;
			}
			catch (RightsManagementException ex2)
			{
				this.result.SetFailureResult(this.datacenter ? Strings.ErrorFailedToGetTemplateTenant : Strings.ErrorFailedToGetTemplateEnterprise, ex2, this.encryptionEnabled);
				return false;
			}
			return true;
		}

		private bool TryGetULAndPreL()
		{
			this.result.SetTask(Strings.InfoGetLicensingUri);
			try
			{
				this.licensingUris = RmsClientManager.IRMConfig.GetTenantLicensingLocations(this.organizationId);
				if (this.licensingUris != null)
				{
					for (int i = 0; i < this.licensingUris.Count; i++)
					{
						this.licensingUris[i] = RmsoProxyUtil.GetLicenseServerRedirectUrl(this.licensingUris[i]);
					}
				}
			}
			catch (RightsManagementException ex)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToGetLicensingUri, ex, this.encryptionEnabled);
				return false;
			}
			catch (ExchangeConfigurationException ex2)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToGetLicensingUri, ex2, this.encryptionEnabled);
				return false;
			}
			if (this.licensingUris == null || this.licensingUris.Count == 0)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToGetLicensingUri, null, this.encryptionEnabled);
				return false;
			}
			foreach (Uri uri in this.licensingUris)
			{
				this.result.SetSuccessResult(Strings.InfoLicensingUri(uri));
			}
			bool flag = true;
			foreach (Uri uri2 in this.licensingUris)
			{
				this.result.SetTask(Strings.InfoCheckRmsVersion(uri2));
				try
				{
					if (this.ValidateRmsVersion(uri2, ServiceType.LicensingService))
					{
						this.result.SetSuccessResult(Strings.InfoRmsVersionChecked);
					}
					else
					{
						this.result.SetFailureResult(Strings.ErrorFailedRmsVersionCheck, null, this.encryptionEnabled);
						flag = false;
					}
				}
				catch (RightsManagementException ex3)
				{
					this.result.SetFailureResult(Strings.ErrorFailedRmsVersionCheck, ex3, this.encryptionEnabled);
					flag = false;
				}
				catch (ExchangeConfigurationException ex4)
				{
					this.result.SetFailureResult(Strings.ErrorFailedRmsVersionCheckInitialization, ex4, this.encryptionEnabled);
					flag = false;
				}
			}
			if (!flag)
			{
				return false;
			}
			XmlNode[] issuanceLicense;
			flag = this.TryCreatePL(out issuanceLicense);
			if (!flag)
			{
				return false;
			}
			if (!this.datacenter)
			{
				foreach (string text in this.recipientsAddress)
				{
					this.result.SetTask(Strings.InfoGetPreL(text, this.publishingUri));
					try
					{
						LicenseResponse[] array2 = RmsClientManager.AcquirePreLicense(this.context, this.publishingUri, issuanceLicense, new string[]
						{
							text
						});
						int num = array2.Length;
						for (int k = 0; k < num; k++)
						{
							if (array2[k].Exception != null)
							{
								this.result.SetFailureResult(Strings.InfoPreLIndividual(array2[k].Exception.FailureCode), null, this.prelicensingEnabled);
								flag = false;
							}
							else
							{
								this.result.SetSuccessResult(Strings.InfoPreL);
							}
						}
					}
					catch (ExchangeConfigurationException ex5)
					{
						this.result.SetFailureResult(Strings.ErrorFailedToGetPreL, ex5, this.prelicensingEnabled);
						flag = false;
					}
					catch (RightsManagementException ex6)
					{
						this.result.SetFailureResult(Strings.ErrorFailedToGetPreL, ex6, this.prelicensingEnabled);
						flag = false;
					}
				}
			}
			this.result.SetTask(Strings.InfoGetUL(this.publishingUri));
			try
			{
				RmsClientManager.AcquireUseLicense(this.context, this.publishingUri, issuanceLicense, null);
				this.result.SetSuccessResult(Strings.InfoUL);
			}
			catch (ExchangeConfigurationException ex7)
			{
				this.result.SetFailureResult(this.datacenter ? Strings.ErrorFailedToGetULTenant : Strings.ErrorFailedToGetULEnterprise, ex7, this.licensingEnabled);
				flag = false;
			}
			catch (RightsManagementException ex8)
			{
				if (ex8.FailureCode == RightsManagementFailureCode.ServerRightNotGranted)
				{
					this.result.SetFailureResult(Strings.ErrorFailedToGetULDueToInvalidSuperUserConfiguration, null, this.licensingEnabled);
				}
				else
				{
					this.result.SetFailureResult(this.datacenter ? Strings.ErrorFailedToGetULTenant : Strings.ErrorFailedToGetULEnterprise, ex8, this.licensingEnabled);
				}
				flag = false;
			}
			return flag;
		}

		private bool TryCreatePL(out XmlNode[] plXml)
		{
			plXml = null;
			bool flag;
			try
			{
				this.result.SetTask(Strings.InfoCreatePL);
				string text;
				string text2;
				string text3;
				string certChain = RmsTemplate.DoNotForward.CreatePublishLicense(this.senderAddress, null, this.recipientsAddress, null, this.tenantLicenses, RmsClientManager.EnvironmentHandle, out text, out text2, out text3);
				if (!RMUtil.TryConvertCertChainToXmlNodeArray(certChain, out plXml))
				{
					this.result.SetFailureResult(Strings.ErrorFailedToCreatePL, null, this.encryptionEnabled);
					flag = false;
				}
				else
				{
					this.result.SetSuccessResult(Strings.InfoPLCreated);
					flag = true;
				}
			}
			catch (RightsManagementException ex)
			{
				this.result.SetFailureResult(Strings.ErrorFailedToCreatePL, ex, this.encryptionEnabled);
				flag = false;
			}
			return flag;
		}

		private bool ValidateRmsVersion(Uri uri, ServiceType serviceType)
		{
			if (this.datacenter)
			{
				return true;
			}
			using (ServerWSManager serverWSManager = new ServerWSManager(uri, serviceType, null, null, RmsClientManagerUtils.GetLocalServerProxy(this.datacenter), RmsClientManager.AppSettings.RmsSoapQueriesTimeout))
			{
				if (serviceType == ServiceType.CertificationService && serverWSManager.ValidateCertificationServiceVersion())
				{
					return true;
				}
				if (serviceType == ServiceType.LicensingService && serverWSManager.ValidateLicensingServiceVersion())
				{
					return true;
				}
			}
			return false;
		}

		private readonly OrganizationId organizationId;

		private readonly string senderAddress;

		private readonly string[] recipientsAddress;

		private bool datacenter;

		private RmsConfiguration rmsConfig;

		private Uri certificationUri;

		private Uri publishingUri;

		private List<Uri> licensingUris;

		private DisposableTenantLicensePair tenantLicenses;

		private bool encryptionEnabled;

		private bool licensingEnabled;

		private bool prelicensingEnabled;

		private IRMConfigurationValidationResult result;

		private RmsClientManagerContext context;
	}
}
