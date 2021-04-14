using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class RmsTemplateReaderCache
	{
		public static IEnumerable<RmsTemplate> GetRmsTemplates(OrganizationId organizationId)
		{
			bool flag = false;
			IEnumerable<RmsTemplate> result;
			try
			{
				if (!RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(organizationId))
				{
					RmsTemplateReaderCache.negativeCacheTimeoutForFirstOrg = ExDateTime.MinValue;
					throw new AirSyncPermanentException(StatusCode.IRM_FeatureDisabled, false)
					{
						ErrorStringForProtocolLogger = "rtrcGrtInternalLicensingDisabled"
					};
				}
				if (organizationId == OrganizationId.ForestWideOrgId && RmsTemplateReaderCache.negativeCacheTimeoutForFirstOrg > ExDateTime.UtcNow)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Found RMSTemplateReader for first org in negative cache");
					throw new AirSyncPermanentException(StatusCode.IRM_TransientError, false)
					{
						ErrorStringForProtocolLogger = "rtrcGrtNegativeCacheHit"
					};
				}
				result = RmsClientManager.AcquireRmsTemplates(organizationId, false);
			}
			catch (ExchangeConfigurationException ex)
			{
				AirSyncDiagnostics.TraceError<ExchangeConfigurationException>(ExTraceGlobals.RequestsTracer, null, "ExchangeConfigurationException encountered while acquiring RMS templates: {0}", ex);
				flag = true;
				throw new AirSyncPermanentException(StatusCode.IRM_TransientError, ex, false)
				{
					ErrorStringForProtocolLogger = "rtrcGrtExchangeConfigurationException"
				};
			}
			catch (RightsManagementException ex2)
			{
				AirSyncDiagnostics.TraceError<RightsManagementException>(ExTraceGlobals.RequestsTracer, null, "RightsManagementException encountered while acquiring RMS templates: {0}", ex2);
				if (ex2.FailureCode != RightsManagementFailureCode.InternalLicensingDisabled)
				{
					flag = true;
				}
				throw new AirSyncPermanentException(ex2.IsPermanent ? StatusCode.IRM_PermanentError : StatusCode.IRM_TransientError, ex2, false)
				{
					ErrorStringForProtocolLogger = "rtrcGrtRightsManagementException" + ex2.FailureCode.ToString()
				};
			}
			finally
			{
				if (organizationId == OrganizationId.ForestWideOrgId && flag)
				{
					RmsTemplateReaderCache.negativeCacheTimeoutForFirstOrg = ExDateTime.UtcNow.Add(GlobalSettings.NegativeRmsTemplateCacheExpirationInterval);
				}
			}
			return result;
		}

		public static RmsTemplate LookupRmsTemplate(OrganizationId organizationId, Guid templateId)
		{
			foreach (RmsTemplate rmsTemplate in RmsTemplateReaderCache.GetRmsTemplates(organizationId))
			{
				if (rmsTemplate.Id == templateId)
				{
					return rmsTemplate;
				}
			}
			return null;
		}

		private static ExDateTime negativeCacheTimeoutForFirstOrg = ExDateTime.MinValue;
	}
}
