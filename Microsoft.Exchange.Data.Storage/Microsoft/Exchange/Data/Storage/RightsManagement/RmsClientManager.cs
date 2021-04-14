using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.com.IPC.WSService;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Data.Storage.OfflineRms;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Security.RightsManagement.SOAP.Server;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.RightsManagementServices.Core;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RmsClientManager
	{
		internal static int DataCenterTestHookMachineCertIndexOverride
		{
			get
			{
				return RmsClientManager.dataCenterTestHookMachineCertIndexOverride;
			}
			set
			{
				RmsClientManager.dataCenterTestHookMachineCertIndexOverride = value;
			}
		}

		private RmsClientManager()
		{
		}

		~RmsClientManager()
		{
			RmsClientManager.Shutdown();
		}

		public static bool IgnoreLicensingEnabled
		{
			get
			{
				return RmsClientManager.ignoreLicensingEnabled;
			}
			set
			{
				RmsClientManager.ignoreLicensingEnabled = value;
			}
		}

		public static bool Initialized
		{
			get
			{
				return RmsClientManager.initialized;
			}
		}

		public static RmsConfiguration IRMConfig
		{
			get
			{
				RmsClientManager.InitializeIfNeeded();
				return RmsClientManager.irmConfig;
			}
		}

		public static RmsAppSettings AppSettings
		{
			get
			{
				return RmsAppSettings.Instance;
			}
		}

		public static IConfigurationSession ADSession
		{
			get
			{
				return RmsClientManager.adSession;
			}
			set
			{
				RmsClientManager.adSession = value;
			}
		}

		public static SafeRightsManagementHandle LibraryHandle
		{
			get
			{
				RmsClientManager.InitializeIfNeeded();
				if (RmsClientManager.drmEnvironment != null)
				{
					return RmsClientManager.drmEnvironment.LibraryHandle;
				}
				return null;
			}
		}

		public static SafeRightsManagementEnvironmentHandle EnvironmentHandle
		{
			get
			{
				RmsClientManager.InitializeIfNeeded();
				if (RmsClientManager.drmEnvironment != null)
				{
					return RmsClientManager.drmEnvironment.EnvironmentHandle;
				}
				return null;
			}
		}

		public static XmlNode[] GetMachineCertificateChain(RmsClientManagerContext clientContext)
		{
			RmsClientManager.InitializeIfNeeded();
			if (RmsClientManager.drmEnvironment == null)
			{
				return null;
			}
			if (!RmsClientManager.UseOfflineRms)
			{
				return RmsClientManager.drmEnvironment.MachineCertificatesChain[RmsClientManager.OnPremiseActiveMachineCertIndex];
			}
			if (-1 != RmsClientManager.DataCenterTestHookMachineCertIndexOverride)
			{
				return RmsClientManager.drmEnvironment.MachineCertificatesChain[RmsClientManager.DataCenterTestHookMachineCertIndexOverride];
			}
			int tenantActiveCryptoMode = ServerManager.GetTenantActiveCryptoMode(clientContext);
			if (tenantActiveCryptoMode <= 0)
			{
				throw new InvalidOperationException("Tenant's active crypto mode cannot be less than or equal to zero");
			}
			int num = tenantActiveCryptoMode - 1;
			if (num > RmsClientManager.drmEnvironment.MachineCertificatesChain.Count)
			{
				throw new InvalidOperationException("Unexpected tenant's machine certificate index: " + num.ToString() + "; machine certs available: " + RmsClientManager.drmEnvironment.MachineCertificatesChain.Count.ToString());
			}
			return RmsClientManager.drmEnvironment.MachineCertificatesChain[num];
		}

		public static bool UseOfflineRms
		{
			get
			{
				return RmsClientManager.useOfflineRms;
			}
		}

		public static void TracePass(object obj, Guid systemProbeId, string formatString, params object[] args)
		{
			RmsClientManager.SystemProbeTracer.TracePass(systemProbeId, (long)((obj == null) ? 0 : obj.GetHashCode()), formatString, args);
		}

		public static void TraceFail(object obj, Guid systemProbeId, string formatString, params object[] args)
		{
			RmsClientManager.SystemProbeTracer.TraceFail(systemProbeId, (long)((obj == null) ? 0 : obj.GetHashCode()), formatString, args);
		}

		public static bool IsDRMInTopology(OrganizationId orgId)
		{
			return RmsClientManager.IRMConfig.GetTenantServiceLocation(orgId) != null;
		}

		public static Uri GetRMSServiceLocation(OrganizationId orgId, ServiceType serviceType)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			if (!RmsClientManager.IsDRMInTopology(orgId))
			{
				RmsClientManager.Tracer.TraceError<OrganizationId>(0L, "Failed to find RMS servers for organization {0}", orgId);
				return null;
			}
			switch (serviceType)
			{
			case ServiceType.Certification:
			case ServiceType.Activation | ServiceType.Certification:
				goto IL_51;
			case ServiceType.Publishing:
				break;
			default:
				if (serviceType != ServiceType.ClientLicensor)
				{
					goto IL_51;
				}
				break;
			}
			Uri uri = RmsClientManager.IRMConfig.GetTenantPublishingLocation(orgId);
			goto IL_5D;
			IL_51:
			uri = RmsClientManager.IRMConfig.GetTenantServiceLocation(orgId);
			IL_5D:
			RmsClientManager.Tracer.TraceDebug<Uri, OrganizationId, ServiceType>(0L, "Returning Uri {0} for Organization {1}, ServiceType: {2}", uri, orgId, serviceType);
			return uri;
		}

		public static Uri GetFirstLicensingLocation(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			if (!RmsClientManager.IsDRMInTopology(orgId))
			{
				RmsClientManager.Tracer.TraceError<OrganizationId>(0L, "Failed to find RMS servers for organization {0}", orgId);
				return null;
			}
			List<Uri> tenantLicensingLocations = RmsClientManager.IRMConfig.GetTenantLicensingLocations(orgId);
			if (tenantLicensingLocations.Count > 0)
			{
				return tenantLicensingLocations[0];
			}
			return null;
		}

		public static bool IsTenantInfoInCache(OrganizationId organizationId)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			TenantLicensePair tenantLicensePair;
			return RmsClientManager.licenseStoreManager.ReadFromStore(RmsClientManagerUtils.GetTenantGuidFromOrgId(organizationId), RmsClientManager.IRMConfig.GetTenantServiceLocation(organizationId), RmsClientManager.IRMConfig.GetTenantPublishingLocation(organizationId), RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(organizationId), out tenantLicensePair);
		}

		public static void BindUseLicenseForDecryption(RmsClientManagerContext context, Uri licenseUri, string useLicense, string publishLicense, out SafeRightsManagementHandle decryptorHandle)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			using (DisposableTenantLicensePair disposableTenantLicensePair = RmsClientManager.AcquireTenantLicenses(context, licenseUri))
			{
				RmsClientManager.BindUseLicenseForDecryption(disposableTenantLicensePair.EnablingPrincipalRac, useLicense, publishLicense, out decryptorHandle);
			}
		}

		public static void BindUseLicenseForDecryption(SafeRightsManagementHandle enablingPrincipalRac, string useLicense, string publishLicense, out SafeRightsManagementHandle decryptorHandle)
		{
			ArgumentValidator.ThrowIfNull("enablingPrincipalRac", enablingPrincipalRac);
			ArgumentValidator.ThrowIfNullOrEmpty("useLicense", useLicense);
			ArgumentValidator.ThrowIfNullOrEmpty("publishLicense", publishLicense);
			RmsClientManager.InitializeIfNeeded();
			SafeRightsManagementHandle safeRightsManagementHandle;
			DrmClientUtils.BindUseLicense(useLicense, publishLicense, "VIEW", false, enablingPrincipalRac, RmsClientManager.EnvironmentHandle, out safeRightsManagementHandle, out decryptorHandle);
		}

		public static void BindUseLicenseForEncryption(SafeRightsManagementHandle enablingPrincipalRac, string useLicense, string publishLicense, out SafeRightsManagementHandle encryptorHandle, out SafeRightsManagementHandle decryptorHandle)
		{
			ArgumentValidator.ThrowIfNull("enablingPrincipalRac", enablingPrincipalRac);
			ArgumentValidator.ThrowIfNullOrEmpty("useLicense", useLicense);
			ArgumentValidator.ThrowIfNullOrEmpty("publishLicense", publishLicense);
			RmsClientManager.InitializeIfNeeded();
			DrmClientUtils.BindUseLicense(useLicense, publishLicense, "EDIT", true, enablingPrincipalRac, RmsClientManager.EnvironmentHandle, out encryptorHandle, out decryptorHandle);
		}

		public static void BindUseLicenseForEncryption(SafeRightsManagementHandle enablingPrincipalRac, string useLicense, string contentId, string contentIdType, out SafeRightsManagementHandle encryptorHandle, out SafeRightsManagementHandle decryptorHandle)
		{
			ArgumentValidator.ThrowIfNull("enablingPrincipalRac", enablingPrincipalRac);
			ArgumentValidator.ThrowIfNullOrEmpty("useLicense", useLicense);
			RmsClientManager.InitializeIfNeeded();
			RmsClientManager.Tracer.TraceDebug<string, string>(0L, "Binding to the use license, Content Id: {0}, Content Type: {1}", contentId, contentIdType);
			DrmClientUtils.BindUseLicense(useLicense, contentId, contentIdType, "EDIT", true, enablingPrincipalRac, RmsClientManager.EnvironmentHandle, out encryptorHandle, out decryptorHandle);
		}

		public static IEnumerable<RmsTemplate> AcquireRmsTemplates(OrganizationId orgId, bool forceRefresh)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			if (!RmsClientManager.IgnoreLicensingEnabled && !RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(orgId))
			{
				RmsClientManager.Tracer.TraceDebug<OrganizationId>(0L, "Internal Licensing is disabled for Tenant ({0}).", orgId);
				return DrmEmailConstants.EmptyTemplateArray;
			}
			Uri uri = RmsClientManager.GetRMSServiceLocation(orgId, ServiceType.ClientLicensor);
			uri = RmsoProxyUtil.GetCertificationServerRedirectUrl(uri);
			LinkedList<RmsTemplate> linkedList = null;
			if (uri == null)
			{
				RmsClientManager.Tracer.TraceDebug(0L, "Service Location not set - returning empty array");
				return DrmEmailConstants.EmptyTemplateArray;
			}
			bool flag = false;
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				Cache<Guid, RmsTemplate> templateCacheForFirstOrg = RmsClientManager.TemplateCacheForFirstOrg;
				ICollection<RmsTemplate> collection;
				if (!forceRefresh && templateCacheForFirstOrg != null && !templateCacheForFirstOrg.GetAllValues(out collection) && collection.Count > 0)
				{
					RmsClientManager.Tracer.TraceDebug(0L, "Found templates in cache");
					linkedList = new LinkedList<RmsTemplate>(collection);
					flag = true;
					goto IL_117;
				}
				using (TemplateWSManager templateWSManager = new TemplateWSManager(uri, RmsClientManager.perfCounters, null, RmsClientManagerUtils.GetLocalServerProxy(RmsClientManager.UseOfflineRms), RmsClientManager.AppSettings.RmsSoapQueriesTimeout))
				{
					RmsClientManager.Tracer.TraceDebug<Uri>(0L, "Acquiring templates from {0}", uri);
					linkedList = new LinkedList<RmsTemplate>(templateWSManager.AcquireAllTemplates());
					goto IL_117;
				}
			}
			linkedList = new LinkedList<RmsTemplate>(RmsClientManager.IRMConfig.GetRmsTemplates(orgId));
			IL_117:
			if (!flag)
			{
				linkedList.AddLast(RmsTemplate.DoNotForward);
				if (RmsClientManager.IRMConfig.IsInternetConfidentialEnabledForTenant(orgId))
				{
					RmsClientManager.Tracer.TraceDebug(0L, "Adding InternetConfidential template");
					linkedList.AddLast(RmsTemplate.InternetConfidential);
				}
				if (orgId == OrganizationId.ForestWideOrgId)
				{
					Cache<Guid, RmsTemplate> cache = new Cache<Guid, RmsTemplate>(RmsClientManager.AppSettings.TemplateCacheSizeInBytes, RmsClientManager.AppSettings.TemplateCacheExpirationInterval, TimeSpan.Zero);
					foreach (RmsTemplate rmsTemplate in linkedList)
					{
						RmsClientManager.Tracer.TraceDebug<Guid>(0L, "Adding templateId {0} for first org to the template cache", rmsTemplate.Id);
						cache.TryAdd(rmsTemplate.Id, rmsTemplate);
					}
					RmsClientManager.TemplateCacheForFirstOrg = cache;
					RmsClientManager.FirstOrgTemplateCacheVersion = RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(orgId);
				}
			}
			RmsClientManager.Tracer.TraceDebug<int>(0L, "Returning {0} templates", linkedList.Count);
			RmsClientManagerLog.LogAcquireRmsTemplateResult(new RmsClientManagerContext(orgId, null), uri, linkedList);
			return linkedList;
		}

		public static RmsTemplate AcquireRMSTemplate(RmsClientManagerContext context, Guid templateId)
		{
			IAsyncResult asyncResult = RmsClientManager.BeginAcquireRMSTemplate(context, templateId, null, null);
			return RmsClientManager.EndAcquireRMSTemplate(asyncResult);
		}

		public static IAsyncResult BeginAcquireRMSTemplate(RmsClientManagerContext context, Guid templateId, AsyncCallback callback, object state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			RmsClientManager.TracePass(null, context.SystemProbeId, "BeginAcquireRMSTemplate: Looking for template {0} for organization {1}", new object[]
			{
				templateId,
				context.OrgId
			});
			TemplateAsyncResult templateAsyncResult = new TemplateAsyncResult(context, templateId, state, callback);
			templateAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireRMSTemplate);
			IAsyncResult result;
			try
			{
				if (!RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(context.OrgId))
				{
					RmsClientManager.TracePass(null, context.SystemProbeId, "Internal Licensing is disabled for Tenant ({0}).", new object[]
					{
						context.OrgId
					});
					templateAsyncResult.InvokeCallback();
					result = templateAsyncResult;
				}
				else if (RmsClientManager.UseOfflineRms)
				{
					templateAsyncResult.InvokeCallback();
					result = templateAsyncResult;
				}
				else
				{
					Cache<Guid, RmsTemplate> templateCacheForFirstOrg = RmsClientManager.TemplateCacheForFirstOrg;
					RmsTemplate rmsTemplate;
					if (templateCacheForFirstOrg != null && templateCacheForFirstOrg.TryGetValue(templateId, out rmsTemplate))
					{
						RmsClientManager.TracePass(null, context.SystemProbeId, "Found the template in the cache", new object[0]);
						RmsClientManagerLog.LogAcquireRmsTemplateCacheResult(context, templateId);
						templateAsyncResult.InvokeCallback();
						result = templateAsyncResult;
					}
					else
					{
						Uri uri = RmsClientManager.GetRMSServiceLocation(context.OrgId, ServiceType.ClientLicensor);
						uri = RmsoProxyUtil.GetCertificationServerRedirectUrl(uri);
						if (uri == null)
						{
							RmsClientManager.TraceFail(null, context.SystemProbeId, "Failed to find service location for Tenant ({0})", new object[]
							{
								context.OrgId
							});
							templateAsyncResult.InvokeCallback();
							result = templateAsyncResult;
						}
						else
						{
							WebProxy localServerProxy = RmsClientManagerUtils.GetLocalServerProxy(RmsClientManager.UseOfflineRms);
							bool flag = RmsClientManager.outstandingPerTenantFindTemplatesCalls.EnqueueResult(context.TenantId, templateAsyncResult);
							if (flag)
							{
								templateAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireRMSTemplateFirstRequest);
								templateAsyncResult.TemplateManager = new TemplateWSManager(uri, RmsClientManager.perfCounters, context.LatencyTracker, localServerProxy, RmsClientManager.AppSettings.RmsSoapQueriesTimeout);
								RmsClientManager.TracePass(null, context.SystemProbeId, "No outstanding template calls for the tenant. Issuing template request to {0}", new object[]
								{
									uri
								});
								RmsClientManagerLog.LogUriEvent(RmsClientManagerLog.RmsClientManagerFeature.Template, RmsClientManagerLog.RmsClientManagerEvent.Acquire, context, uri);
								templateAsyncResult.TemplateManager.BeginAcquireAllTemplates(RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireRmsTemplatesCallback)), templateAsyncResult);
							}
							else
							{
								RmsClientManager.TracePass(null, context.SystemProbeId, "AcquireTemplate call for the tenant is already pending - enqueued to the template queue", new object[0]);
								RmsClientManagerLog.LogUriEvent(RmsClientManagerLog.RmsClientManagerFeature.Template, RmsClientManagerLog.RmsClientManagerEvent.Queued, context, uri);
								templateAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireRMSTemplatePendingRequest);
							}
							result = templateAsyncResult;
						}
					}
				}
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "Failed to initialize RmsClientManager. Error {0}", new object[]
				{
					ex
				});
				templateAsyncResult.InvokeCallback(ex);
				result = templateAsyncResult;
			}
			catch (ExchangeConfigurationException ex2)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "Failed to read configuration when initializing RmsClientManager. Error {0}", new object[]
				{
					ex2
				});
				templateAsyncResult.InvokeCallback(ex2);
				result = templateAsyncResult;
			}
			return result;
		}

		public static RmsTemplate EndAcquireRMSTemplate(IAsyncResult asyncResult)
		{
			TemplateAsyncResult templateAsyncResult = asyncResult as TemplateAsyncResult;
			if (templateAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult cannot be null and has to be type of TemplateAsyncResult");
			}
			if (!templateAsyncResult.IsCompleted)
			{
				templateAsyncResult.InternalWaitForCompletion();
			}
			templateAsyncResult.AddBreadCrumb(Constants.State.EndAcquireRMSTemplate);
			if (!RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(templateAsyncResult.Context.OrgId))
			{
				RmsClientManager.TracePass(null, templateAsyncResult.Context.SystemProbeId, "Internal Licensing is disabled for Tenant ({0}).", new object[]
				{
					templateAsyncResult.Context.OrgId
				});
				RmsClientManagerLog.LogAcquireRmsTemplateResult(templateAsyncResult.Context, RmsTemplate.Empty);
				return null;
			}
			Exception ex = templateAsyncResult.Result as Exception;
			if (ex != null)
			{
				RmsClientManager.TraceFail(null, templateAsyncResult.Context.SystemProbeId, "EndAcquireRMSTemplates hit an exception {0}", new object[]
				{
					ex
				});
				RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.Template, templateAsyncResult.Context, ex);
				throw ex;
			}
			RmsTemplate rmsTemplate = null;
			if (RmsClientManager.UseOfflineRms)
			{
				templateAsyncResult.Context.LatencyTracker.BeginTrackRmsLatency(RmsOperationType.AcquireTemplates);
				rmsTemplate = RmsClientManager.IRMConfig.GetRmsTemplate(templateAsyncResult.Context.OrgId, templateAsyncResult.TemplateId);
				templateAsyncResult.Context.LatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireTemplates);
				return rmsTemplate;
			}
			Cache<Guid, RmsTemplate> templateCacheForFirstOrg = RmsClientManager.TemplateCacheForFirstOrg;
			if (templateCacheForFirstOrg != null)
			{
				templateCacheForFirstOrg.TryGetValue(templateAsyncResult.TemplateId, out rmsTemplate);
			}
			if (rmsTemplate == null)
			{
				rmsTemplate = RmsTemplate.Empty;
			}
			RmsClientManagerLog.LogAcquireRmsTemplateResult(templateAsyncResult.Context, rmsTemplate);
			if (rmsTemplate == RmsTemplate.Empty && templateCacheForFirstOrg != null)
			{
				RmsClientManager.TraceFail(null, templateAsyncResult.Context.SystemProbeId, "EndAcquireRMSTemplates: Failed to find template {0} for first org. Adding negative entry to the cache", new object[]
				{
					templateAsyncResult.TemplateId
				});
				templateCacheForFirstOrg.TryAdd(templateAsyncResult.TemplateId, rmsTemplate);
			}
			if (rmsTemplate == RmsTemplate.Empty)
			{
				return null;
			}
			return rmsTemplate;
		}

		public static DisposableTenantLicensePair AcquireTenantLicenses(RmsClientManagerContext context, Uri licenseUri)
		{
			IAsyncResult asyncResult = RmsClientManager.BeginAcquireTenantLicenses(context, licenseUri, RmsClientManager.IsInternalRMSLicensingUri(context.OrgId, licenseUri), null, null);
			return RmsClientManager.EndAcquireTenantLicenses(asyncResult);
		}

		public static IAsyncResult BeginAcquireTenantLicenses(RmsClientManagerContext context, Uri licenseUri, bool isInternal, AsyncCallback callback, object state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			if (isInternal)
			{
				return RmsClientManager.BeginAcquireInternalOrganizationRACAndCLC(context, callback, state);
			}
			return RmsClientManager.BeginAcquireFederationRAC(context, licenseUri, callback, state);
		}

		public static DisposableTenantLicensePair EndAcquireTenantLicenses(IAsyncResult asyncResult)
		{
			TenantLicenseAsyncResult tenantLicenseAsyncResult = asyncResult as TenantLicenseAsyncResult;
			if (tenantLicenseAsyncResult != null)
			{
				return RmsClientManager.EndAcquireInternalOrganizationRACAndCLC(tenantLicenseAsyncResult);
			}
			FederationRacAsyncResult federationRacAsyncResult = asyncResult as FederationRacAsyncResult;
			if (federationRacAsyncResult != null)
			{
				return RmsClientManager.EndAcquireFederationRAC(federationRacAsyncResult);
			}
			throw new InvalidOperationException("EndAcquireTenantLicenses has invalid IAsyncResult");
		}

		public static UseLicenseAndUsageRights AcquireUseLicenseAndUsageRights(RmsClientManagerContext context, string publishLicense, string userIdentity, SecurityIdentifier userSid, RecipientTypeDetails userType)
		{
			IAsyncResult asyncResult = RmsClientManager.BeginAcquireUseLicenseAndUsageRights(context, publishLicense, userIdentity, userSid, userType, null, null);
			return RmsClientManager.EndAcquireUseLicenseAndUsageRights(asyncResult);
		}

		public static IAsyncResult BeginAcquireUseLicenseAndUsageRights(RmsClientManagerContext context, string publishLicense, string userIdentity, SecurityIdentifier userSid, RecipientTypeDetails userType, AsyncCallback callback, object state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNullOrEmpty("publishLicense", publishLicense);
			ArgumentValidator.ThrowIfNullOrEmpty("userIdentity", userIdentity);
			ArgumentValidator.ThrowIfNull("userSid", userSid);
			Uri uri;
			XmlNode[] array;
			bool flag;
			RmsClientManager.GetLicensingUri(context.OrgId, publishLicense, out uri, out array, out flag);
			UseLicenseAndUsageRightsAsyncResult useLicenseAndUsageRightsAsyncResult = new UseLicenseAndUsageRightsAsyncResult(context, uri, publishLicense, array, userIdentity, userSid, RmsClientManagerUtils.TreatRecipientAsRMSSuperuser(context.OrgId, userType), state, callback);
			useLicenseAndUsageRightsAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireUseLicenseAndUsageRights);
			if (flag && RmsClientManager.UseOfflineRms)
			{
				RmsClientManager.BeginAcquireServerPreLicense(context, uri, array, new string[]
				{
					userIdentity
				}, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireUseLicenseAndUsageRightsCallbackForUseLicense)), useLicenseAndUsageRightsAsyncResult);
			}
			else
			{
				RmsClientManager.BeginAcquireUseLicense(context, uri, flag, array, userIdentity, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireUseLicenseAndUsageRightsCallbackForUseLicense)), useLicenseAndUsageRightsAsyncResult);
			}
			return useLicenseAndUsageRightsAsyncResult;
		}

		public static UseLicenseAndUsageRights EndAcquireUseLicenseAndUsageRights(IAsyncResult asyncResult)
		{
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			UseLicenseAndUsageRightsAsyncResult useLicenseAndUsageRightsAsyncResult = asyncResult as UseLicenseAndUsageRightsAsyncResult;
			if (useLicenseAndUsageRightsAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult MUST be type of UseLicenseAndUsageRightsAsyncResult");
			}
			useLicenseAndUsageRightsAsyncResult.AddBreadCrumb(Constants.State.EndAcquireUseLicenseAndUsageRights);
			if (!useLicenseAndUsageRightsAsyncResult.IsCompleted)
			{
				useLicenseAndUsageRightsAsyncResult.InternalWaitForCompletion();
			}
			Exception ex = useLicenseAndUsageRightsAsyncResult.Result as Exception;
			if (ex != null)
			{
				RmsClientManager.TraceFail(null, useLicenseAndUsageRightsAsyncResult.Context.SystemProbeId, "Throwing exception at EndAcquireUseLicenseAndUsageRights.  Exception: {0}", new object[]
				{
					ex
				});
				throw ex;
			}
			return new UseLicenseAndUsageRights(useLicenseAndUsageRightsAsyncResult.UseLicense, useLicenseAndUsageRightsAsyncResult.UsageRights, useLicenseAndUsageRightsAsyncResult.ExpiryTime, useLicenseAndUsageRightsAsyncResult.DRMPropsSignature, useLicenseAndUsageRightsAsyncResult.Context.OrgId, useLicenseAndUsageRightsAsyncResult.PublishLicense, useLicenseAndUsageRightsAsyncResult.LicensingUri);
		}

		public static string AcquireUseLicense(RmsClientManagerContext context, Uri licenseUri, XmlNode[] issuanceLicense, string recipientAddress)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			ArgumentValidator.ThrowIfNull("issuanceLicense", issuanceLicense);
			IAsyncResult asyncResult = RmsClientManager.BeginAcquireUseLicense(context, licenseUri, RmsClientManager.IsInternalRMSLicensingUri(context.OrgId, licenseUri), issuanceLicense, recipientAddress, null, null);
			return RmsClientManager.EndAcquireUseLicense(asyncResult).License;
		}

		public static IAsyncResult BeginAcquireUseLicense(RmsClientManagerContext context, Uri licenseUri, bool isInternalRmsUri, XmlNode[] issuanceLicense, string recipientAddress, AsyncCallback callback, object state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			ArgumentValidator.ThrowIfNull("issuanceLicense", issuanceLicense);
			bool flag = false;
			UseLicenseAsyncResult useLicenseAsyncResult = new UseLicenseAsyncResult(context, licenseUri, state, callback);
			RmsClientManagerLog.LogAcquireUseLicense(context, licenseUri, recipientAddress);
			try
			{
				flag = (RmsClientManager.IgnoreLicensingEnabled || (isInternalRmsUri ? RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(context.OrgId) : RmsClientManager.IRMConfig.IsExternalLicensingEnabledForTenant(context.OrgId)));
			}
			catch (ExchangeConfigurationException ex)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "IsInternalRMSLicenseUri threw an exception for {0}, Org {1}. Error {2}", new object[]
				{
					licenseUri,
					context.OrgId,
					ex
				});
				useLicenseAsyncResult.InvokeCallback(ex);
				return useLicenseAsyncResult;
			}
			catch (RightsManagementException ex2)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "IsInternalRMSLicenseUri threw an exception for {0}, Org {1}. Error {2}", new object[]
				{
					licenseUri,
					context.OrgId,
					ex2
				});
				useLicenseAsyncResult.InvokeCallback(ex2);
				return useLicenseAsyncResult;
			}
			RmsClientManager.SaveContextCallback = useLicenseAsyncResult.SaveContextCallback;
			if (isInternalRmsUri)
			{
				RmsClientManager.TracePass(null, context.SystemProbeId, "Querying the internal RMS server {0} for organization  {1} to fetch use license", new object[]
				{
					licenseUri,
					context.OrgId
				});
				if (!flag)
				{
					if (RmsClientManager.UseOfflineRms)
					{
						ExTraceGlobals.RightsManagementTracer.TraceError<OrganizationId>(0L, "InternalLicensing is disabled for tenant {0}.", context.OrgId);
						useLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.InternalLicensingDisabled, ServerStrings.InternalLicensingDisabledForTenant(context.OrgId)));
					}
					else
					{
						ExTraceGlobals.RightsManagementTracer.TraceError(0L, "InternalLicensing is disabled for Enterprise.");
						useLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.InternalLicensingDisabled, ServerStrings.InternalLicensingDisabledForEnterprise));
					}
					return useLicenseAsyncResult;
				}
				return RmsClientManager.BeginAcquireSuperUserUseLicense(context, licenseUri, issuanceLicense, callback, state);
			}
			else
			{
				if (!flag)
				{
					if (RmsClientManager.UseOfflineRms)
					{
						ExTraceGlobals.RightsManagementTracer.TraceError<OrganizationId, Uri>(0L, "ExternalLicensing is disabled for tenant {0}. License Uri '{1}'.", context.OrgId, licenseUri);
						useLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.ExternalLicensingDisabled, ServerStrings.ExternalLicensingDisabledForTenant(licenseUri, context.OrgId)));
					}
					else
					{
						ExTraceGlobals.RightsManagementTracer.TraceError<Uri>(0L, "ExternalLicensing is disabled for Enterprise. License Uri '{0}'.", licenseUri);
						useLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.ExternalLicensingDisabled, ServerStrings.ExternalLicensingDisabledForEnterprise(licenseUri)));
					}
					return useLicenseAsyncResult;
				}
				if (string.IsNullOrEmpty(recipientAddress))
				{
					ExTraceGlobals.RightsManagementTracer.TraceError<OrganizationId, Uri>(0L, "Recipient address not specified for external RMS servers. Tenant {0}. License Uri {1}", context.OrgId, licenseUri);
					useLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.InvalidRecipient, ServerStrings.RecipientAddressNotSpecifiedForExternalLicensing(licenseUri, context.OrgId.ToString())));
					return useLicenseAsyncResult;
				}
				if (!SmtpAddress.IsValidSmtpAddress(recipientAddress) || string.Equals(recipientAddress, (string)SmtpAddress.NullReversePath, StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.RightsManagementTracer.TraceError<OrganizationId, Uri, string>(0L, "Recipient address is invalidfor external RMS servers. Tenant {0}. License Uri {1}. Recipient address {2}", context.OrgId, licenseUri, recipientAddress);
					useLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.InvalidRecipient, ServerStrings.RecipientAddressInvalidForExternalLicensing(recipientAddress, licenseUri, context.OrgId.ToString())));
					return useLicenseAsyncResult;
				}
				LicenseIdentity licenseIdentity;
				try
				{
					licenseIdentity = RmsClientManagerUtils.GetLicenseIdentity(context, recipientAddress);
				}
				catch (ExchangeConfigurationException ex3)
				{
					ExTraceGlobals.RightsManagementTracer.TraceError<string, OrganizationId, ExchangeConfigurationException>(0L, "GetLicenseIdentity threw an exception for {0}, Org {1}. Error {2}", recipientAddress, context.OrgId, ex3);
					useLicenseAsyncResult.InvokeCallback(ex3);
					return useLicenseAsyncResult;
				}
				catch (RightsManagementException ex4)
				{
					ExTraceGlobals.RightsManagementTracer.TraceError<string, OrganizationId, RightsManagementException>(0L, "GetLicenseIdentity threw an exception for {0}, Org {1}. Error {2}", recipientAddress, context.OrgId, ex4);
					useLicenseAsyncResult.InvokeCallback(ex4);
					return useLicenseAsyncResult;
				}
				ExTraceGlobals.RightsManagementTracer.TraceDebug<Uri, OrganizationId>(0L, "Querying the external RMS server {0} for organization  {1} to fetch use license. This will use SAML based authentication", licenseUri, context.OrgId);
				return RmsClientManager.BeginAcquireFederationServerLicense(context, new LicenseIdentity[]
				{
					licenseIdentity
				}, null, issuanceLicense, licenseUri, callback, state);
			}
			IAsyncResult result;
			return result;
		}

		public static LicenseResponse EndAcquireUseLicense(IAsyncResult asyncResult)
		{
			UseLicenseAsyncResult useLicenseAsyncResult = asyncResult as UseLicenseAsyncResult;
			if (useLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult cannot be null and has to be type of UseLicenseAsyncResult");
			}
			useLicenseAsyncResult.AddBreadCrumb(Constants.State.EndAcquireUseLicense);
			if (!useLicenseAsyncResult.IsCompleted)
			{
				useLicenseAsyncResult.InternalWaitForCompletion();
			}
			Exception ex = useLicenseAsyncResult.Result as Exception;
			if (ex != null)
			{
				RmsClientManager.TraceFail(null, useLicenseAsyncResult.Context.SystemProbeId, "EndAcquireUseLicense hit an exception {0}", new object[]
				{
					ex
				});
				RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.UseLicense, useLicenseAsyncResult.Context, ex);
				throw ex;
			}
			SuperUserUseLicenseAsyncResult superUserUseLicenseAsyncResult = asyncResult as SuperUserUseLicenseAsyncResult;
			if (superUserUseLicenseAsyncResult != null)
			{
				RmsClientManager.TracePass(null, useLicenseAsyncResult.Context.SystemProbeId, "Successfully found the useLicense. Empty : {0}", new object[]
				{
					string.IsNullOrEmpty(superUserUseLicenseAsyncResult.UseLicense)
				});
				RmsClientManagerLog.LogAcquireUseLicenseResult(superUserUseLicenseAsyncResult.Context, superUserUseLicenseAsyncResult.UseLicense);
				return new LicenseResponse(superUserUseLicenseAsyncResult.UseLicense, new ContentRight?(ContentRight.Owner));
			}
			FederationServerLicenseAsyncResult federationServerLicenseAsyncResult = asyncResult as FederationServerLicenseAsyncResult;
			if (federationServerLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("EndAcquireUseLicense has invalid IAsyncResult");
			}
			if (federationServerLicenseAsyncResult.Responses.Length != 1)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "FederationServerLicenseAsyncResult contains {0} responses, while only one request was made", new object[]
				{
					federationServerLicenseAsyncResult.Responses.Length
				}));
			}
			LicenseResponse licenseResponse = federationServerLicenseAsyncResult.Responses[0];
			if (licenseResponse.Exception != null)
			{
				RmsClientManager.TraceFail(null, useLicenseAsyncResult.Context.SystemProbeId, "Hit an exception while acquiring federation server license {0}", new object[]
				{
					licenseResponse.Exception
				});
				RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.UseLicense, useLicenseAsyncResult.Context, licenseResponse.Exception);
				throw licenseResponse.Exception;
			}
			RmsClientManagerLog.LogAcquireUseLicenseResult(useLicenseAsyncResult.Context, licenseResponse.License);
			return licenseResponse;
		}

		public static LicenseResponse[] AcquirePreLicense(RmsClientManagerContext context, Uri prelicenseUri, XmlNode[] issuanceLicense, string[] identities)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("prelicenseUri", prelicenseUri);
			ArgumentValidator.ThrowIfNull("issuanceLicense", issuanceLicense);
			ArgumentValidator.ThrowIfNull("identities", identities);
			IAsyncResult asyncResult = RmsClientManager.BeginAcquirePreLicense(context, prelicenseUri, true, issuanceLicense, identities, null, null);
			return RmsClientManager.EndAcquirePreLicense(asyncResult);
		}

		public static IAsyncResult BeginAcquirePreLicense(RmsClientManagerContext context, Uri prelicenseUri, bool isInternalRmsUri, XmlNode[] issuanceLicense, string[] identities, AsyncCallback callback, object state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("prelicenseUri", prelicenseUri);
			ArgumentValidator.ThrowIfNull("issuanceLicense", issuanceLicense);
			ArgumentValidator.ThrowIfNull("identities", identities);
			if (isInternalRmsUri && RmsClientManager.UseOfflineRms)
			{
				RmsClientManager.TracePass(null, context.SystemProbeId, "BeginAcquirePreLicense: using offline RMS APIs to get user licenses tied to server box Rac", new object[0]);
				return RmsClientManager.BeginAcquireServerPreLicense(context, prelicenseUri, issuanceLicense, identities, callback, state);
			}
			PreLicenseAsyncResult preLicenseAsyncResult = new PreLicenseAsyncResult(context, null, state, callback);
			RmsClientManagerLog.LogAcquirePrelicense(context, prelicenseUri, identities);
			if (isInternalRmsUri)
			{
				try
				{
					preLicenseAsyncResult.PreLicenseManager = new PreLicenseWSManager(prelicenseUri, RmsClientManager.perfCounters, context.LatencyTracker, RmsClientManagerUtils.GetLocalServerProxy(RmsClientManager.UseOfflineRms), RmsClientManager.AppSettings.RmsSoapQueriesTimeout);
				}
				catch (ExchangeConfigurationException ex)
				{
					RmsClientManager.TraceFail(null, context.SystemProbeId, "BeginAcquirePreLicense: Failed to read configuration when processing a prelicense request for organization {0} against {1}. Details: {2}", new object[]
					{
						context.OrgId,
						prelicenseUri,
						ex
					});
					preLicenseAsyncResult.InvokeCallback(ex);
					return preLicenseAsyncResult;
				}
				RmsClientManager.TracePass(null, context.SystemProbeId, "BeginAcquirePreLicense: Querying for prelicense against {0} for organization {1}. Number of users: {2}", new object[]
				{
					prelicenseUri,
					context.OrgId,
					identities.Length
				});
				preLicenseAsyncResult.AddBreadCrumb(Constants.State.BeginAcquirePreLicense);
				LicenseIdentity[] array = new LicenseIdentity[identities.Length];
				for (int i = 0; i < identities.Length; i++)
				{
					array[i] = new LicenseIdentity(identities[i], null);
				}
				preLicenseAsyncResult.PreLicenseManager.BeginAcquireLicense(issuanceLicense, array, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquirePreLicenseCallback)), preLicenseAsyncResult);
				return preLicenseAsyncResult;
			}
			LicenseResponse[] array2 = new LicenseResponse[identities.Length];
			List<LicenseIdentity> list = new List<LicenseIdentity>(identities.Length);
			for (int j = 0; j < identities.Length; j++)
			{
				try
				{
					list.Add(RmsClientManagerUtils.GetLicenseIdentity(context, identities[j]));
				}
				catch (ExchangeConfigurationException ex2)
				{
					ExTraceGlobals.RightsManagementTracer.TraceError<string, OrganizationId, ExchangeConfigurationException>(0L, "BeginAcquirePreLicense: GetLicenseIdentity threw an exception for {0}, Org {1}. Error {2}", identities[j], context.OrgId, ex2);
					preLicenseAsyncResult.InvokeCallback(ex2);
					return preLicenseAsyncResult;
				}
				catch (RightsManagementException ex3)
				{
					ExTraceGlobals.RightsManagementTracer.TraceError<string, OrganizationId, RightsManagementException>(0L, "BeginAcquirePreLicense: GetLicenseIdentity threw an exception for {0}, Org {1}. Error {2}", identities[j], context.OrgId, ex3);
					array2[j] = new LicenseResponse(ex3);
				}
			}
			if (list.Count == 0)
			{
				ExTraceGlobals.RightsManagementTracer.TraceError(0L, "BeginAcquirePreLicense: None of the recipients to request licenses for.");
				preLicenseAsyncResult.Responses = array2;
				preLicenseAsyncResult.InvokeCallback();
				return preLicenseAsyncResult;
			}
			return RmsClientManager.BeginAcquireFederationServerLicense(context, list.ToArray(), array2, issuanceLicense, prelicenseUri, callback, state);
		}

		public static LicenseResponse[] EndAcquirePreLicense(IAsyncResult asyncResult)
		{
			RightsManagementAsyncResult rightsManagementAsyncResult = asyncResult as RightsManagementAsyncResult;
			if (rightsManagementAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult must be type of RightsManagementAsyncResult");
			}
			rightsManagementAsyncResult.AddBreadCrumb(Constants.State.EndAcquirePreLicense);
			if (!rightsManagementAsyncResult.IsCompleted)
			{
				rightsManagementAsyncResult.InternalWaitForCompletion();
			}
			Exception ex = rightsManagementAsyncResult.Result as Exception;
			if (ex != null)
			{
				RmsClientManager.TraceFail(null, rightsManagementAsyncResult.Context.SystemProbeId, "EndAcquirePreLicense hit an exception {0}", new object[]
				{
					ex
				});
				RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.Prelicense, rightsManagementAsyncResult.Context, ex);
				throw ex;
			}
			LicenseResponse[] array = null;
			if (!(asyncResult is PreLicenseAsyncResult) && !(asyncResult is FederationServerLicenseAsyncResult))
			{
				throw new InvalidOperationException("asyncResult must be of type PreLicenseAsyncResult or FederationServerLicenseAsyncResult");
			}
			PreLicenseAsyncResult preLicenseAsyncResult = asyncResult as PreLicenseAsyncResult;
			if (preLicenseAsyncResult != null)
			{
				array = preLicenseAsyncResult.Responses;
			}
			FederationServerLicenseAsyncResult federationServerLicenseAsyncResult = asyncResult as FederationServerLicenseAsyncResult;
			if (federationServerLicenseAsyncResult != null)
			{
				array = federationServerLicenseAsyncResult.Responses;
			}
			RmsClientManager.TracePass(null, rightsManagementAsyncResult.Context.SystemProbeId, "EndAcquirePreLicense : Returning {0} responses", new object[]
			{
				(array != null) ? array.Length : 0
			});
			RmsClientManagerLog.LogAcquirePrelicenseResult(rightsManagementAsyncResult.Context, array);
			return array;
		}

		public static bool IsInternalRMSLicensingUri(OrganizationId orgId, Uri licenseUri)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			if (RmsClientManager.AppSettings.IsSamlAuthenticationEnabledForInternalRMS)
			{
				return false;
			}
			List<Uri> tenantLicensingLocations = RmsClientManager.IRMConfig.GetTenantLicensingLocations(orgId);
			if (tenantLicensingLocations.Count == 0)
			{
				RmsClientManager.Tracer.TraceDebug<OrganizationId>(0L, "No RMS licensing Urls configured for org {0}", orgId);
				return false;
			}
			string a = RMUtil.ConvertUriToLicenseUrl(licenseUri);
			string a2 = RMUtil.ConvertUriToLicenseUrl(RmsoProxyUtil.OriginalLicenseServerUrl);
			foreach (Uri offeredUri in tenantLicensingLocations)
			{
				string b = RMUtil.ConvertUriToLicenseUrl(offeredUri);
				if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase) || string.Equals(a2, b, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			RmsClientManager.Tracer.TraceDebug<Uri, OrganizationId>(0L, "Failed to find a match for Uri {0} for org {1}", licenseUri, orgId);
			return false;
		}

		public static void GetLicensingUri(OrganizationId orgId, string publishLicense, out Uri licensingUri, out XmlNode[] publishLicenseAsXmlNodes, out bool isInternalToOrg)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			ArgumentValidator.ThrowIfNull("publishLicense", publishLicense);
			licensingUri = null;
			isInternalToOrg = false;
			Uri licenseServerRedirectUrl;
			Uri uri;
			if (!RMUtil.TryGetIssuanceLicenseAndUrls(publishLicense, out licenseServerRedirectUrl, out uri, out publishLicenseAsXmlNodes))
			{
				RmsClientManager.Tracer.TraceError(0L, "GetLicensingUri: failed to extract licensing URIs from publish license");
				throw new RightsManagementException(RightsManagementFailureCode.InvalidIssuanceLicense, ServerStrings.FailedToFindLicenseUri(orgId.ToString()));
			}
			if (licenseServerRedirectUrl == null && uri == null)
			{
				RmsClientManager.Tracer.TraceError(0L, "GetLicensingUri: Invalid PublishLicense - Extranet/IntranetUris are not set");
				throw new RightsManagementException(RightsManagementFailureCode.InvalidIssuanceLicense, ServerStrings.FailedToFindLicenseUri(orgId.ToString()));
			}
			if (uri == null)
			{
				uri = licenseServerRedirectUrl;
			}
			licenseServerRedirectUrl = RmsoProxyUtil.GetLicenseServerRedirectUrl(licenseServerRedirectUrl);
			uri = RmsoProxyUtil.GetLicenseServerRedirectUrl(uri);
			if (orgId == OrganizationId.ForestWideOrgId && licenseServerRedirectUrl != null && RMUtil.IsWellFormedRmServiceUrl(licenseServerRedirectUrl) && RmsClientManager.IsInternalRMSLicensingUri(orgId, licenseServerRedirectUrl))
			{
				isInternalToOrg = true;
				licensingUri = licenseServerRedirectUrl;
				RmsClientManager.Tracer.TraceError<string, string>(0L, "licensingUri proxy set as {0}, orignianl uri is {1}", licensingUri.AbsoluteUri.ToString(), licenseServerRedirectUrl.AbsoluteUri.ToString());
				return;
			}
			if (RMUtil.IsWellFormedRmServiceUrl(uri))
			{
				isInternalToOrg = RmsClientManager.IsInternalRMSLicensingUri(orgId, uri);
				licensingUri = uri;
				RmsClientManager.Tracer.TraceError<string, string>(0L, "licensingUri proxy set as {0}, orignianl uri is {1}", licensingUri.AbsoluteUri.ToString(), uri.AbsoluteUri.ToString());
				return;
			}
			throw new RightsManagementException(RightsManagementFailureCode.InvalidIssuanceLicense, ServerStrings.FailedToFindLicenseUri(orgId.ToString()));
		}

		public static SafeRightsManagementHandle VerifyDRMPropsSignatureAndGetDecryptor(RmsClientManagerContext context, SecurityIdentifier userSid, RecipientTypeDetails userType, string userIdentity, ContentRight usageRights, ExDateTime expiryTime, byte[] drmPropsSignature, string useLicense, string publishLicense, UsageRightsSignatureVerificationOptions verificationOptions, IEnumerable<SecurityIdentifier> userSidHistory)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNullOrEmpty("useLicense", useLicense);
			ArgumentValidator.ThrowIfNullOrEmpty("publishLicense", publishLicense);
			ArgumentValidator.ThrowIfNull("drmPropsSignature", drmPropsSignature);
			bool flag = RmsClientManagerUtils.TreatRecipientAsRMSSuperuser(context.OrgId, userType);
			if (!flag && expiryTime < ExDateTime.UtcNow)
			{
				BadDRMPropsSignatureException ex = new BadDRMPropsSignatureException(ServerStrings.LicenseExpired(expiryTime.ToString()));
				ex.FailureCode = RightsManagementFailureCode.ExpiredLicense;
				RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex);
				throw ex;
			}
			Uri licensingUri;
			XmlNode[] array;
			bool flag2;
			RmsClientManager.GetLicensingUri(context.OrgId, publishLicense, out licensingUri, out array, out flag2);
			if (!flag2 || RmsClientManager.UseOfflineRms)
			{
				List<string> delegatedEmailAddressesFromB2BUseLicense = RmsClientManagerUtils.GetDelegatedEmailAddressesFromB2BUseLicense(useLicense);
				if (delegatedEmailAddressesFromB2BUseLicense != null && delegatedEmailAddressesFromB2BUseLicense.Count != 0)
				{
					return RmsClientManager.VerifyDelegatedEmailDRMPropsSignatureAndGetDecryptor(context, userIdentity, flag, useLicense, publishLicense, licensingUri, delegatedEmailAddressesFromB2BUseLicense);
				}
			}
			if (flag2)
			{
				ArgumentValidator.ThrowIfNull("userSid", userSid);
				ArgumentValidator.ThrowIfNull("userSidHistory", userSidHistory);
				return RmsClientManager.VerifySuperUserDRMPropsSignatureAndGetDecryptor(context, userSid, flag, usageRights, expiryTime, drmPropsSignature, useLicense, publishLicense, verificationOptions, userSidHistory, licensingUri);
			}
			BadDRMPropsSignatureException ex2 = new BadDRMPropsSignatureException();
			RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex2);
			throw ex2;
		}

		public static IEnumerable<XElement> GetTemplateDiagnosticInfo(OrganizationId organizationId)
		{
			IEnumerable<RmsTemplate> enumerable = null;
			List<XElement> list = new List<XElement>();
			if (organizationId == OrganizationId.ForestWideOrgId)
			{
				Cache<Guid, RmsTemplate> templateCacheForFirstOrg = RmsClientManager.TemplateCacheForFirstOrg;
				if (templateCacheForFirstOrg != null)
				{
					ICollection<RmsTemplate> collection;
					templateCacheForFirstOrg.GetAllValues(out collection);
					enumerable = collection;
					list.Add(new XElement("templateVersion", RmsClientManager.FirstOrgTemplateCacheVersion));
				}
			}
			else
			{
				enumerable = RmsClientManager.IRMConfig.GetRmsTemplates(organizationId);
			}
			if (enumerable != null)
			{
				using (IEnumerator<RmsTemplate> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RmsTemplate rmsTemplate = enumerator.Current;
						list.Add(new XElement("Template", new object[]
						{
							new XElement("id", rmsTemplate.Id),
							new XElement("name", rmsTemplate.Name),
							new XElement("description", rmsTemplate.Description)
						}));
					}
					return list;
				}
			}
			list.Add(new XElement("error", "Templates not available."));
			return list;
		}

		public static XElement GetLicenseDiagnosticInfo(OrganizationId organizationId)
		{
			Uri tenantServiceLocation = RmsClientManager.IRMConfig.GetTenantServiceLocation(organizationId);
			Uri tenantPublishingLocation = RmsClientManager.IRMConfig.GetTenantPublishingLocation(organizationId);
			byte tenantServerCertificatesVersion = RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(organizationId);
			Guid tenantGuidFromOrgId = RmsClientManagerUtils.GetTenantGuidFromOrgId(organizationId);
			TenantLicensePair tenantLicensePair;
			if (RmsClientManager.licenseStoreManager != null && RmsClientManager.licenseStoreManager.ReadFromStore(tenantGuidFromOrgId, tenantServiceLocation, tenantPublishingLocation, tenantServerCertificatesVersion, out tenantLicensePair))
			{
				return new XElement("Licenses", new object[]
				{
					new XElement("clc", (tenantLicensePair.BoundLicenseClc == null) ? "null" : "non-null"),
					new XElement("clcExpire", tenantLicensePair.ClcExpire),
					new XElement("rac", DrmClientUtils.ConvertXmlNodeArrayToCertificateChain(tenantLicensePair.Rac)),
					new XElement("racExpire", tenantLicensePair.RacExpire),
					new XElement("version", tenantLicensePair.Version)
				});
			}
			return new XElement("error", "LicenseStoreManager not available.");
		}

		public static XElement GetConfigDiagnosticInfo(OrganizationId organizationId)
		{
			XElement xelement = new XElement("Config");
			List<XElement> list = new List<XElement>();
			foreach (Uri content in RmsClientManager.IRMConfig.GetTenantLicensingLocations(organizationId))
			{
				list.Add(new XElement("licensingLocation", content));
			}
			xelement.Add(new object[]
			{
				new XElement("clientAccessServerEnabled", RmsClientManager.IRMConfig.IsClientAccessServerEnabledForTenant(organizationId)),
				new XElement("externalLicensingEnabled", RmsClientManager.IRMConfig.IsExternalLicensingEnabledForTenant(organizationId)),
				new XElement("externalServerPreLicensingEnabled", RmsClientManager.IRMConfig.IsExternalServerPreLicensingEnabledForTenant(organizationId)),
				new XElement("internalLicensingEnabled", RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(organizationId)),
				new XElement("internalServerPreLicensingEnabled", RmsClientManager.IRMConfig.IsInternalServerPreLicensingEnabledForTenant(organizationId)),
				new XElement("journalReportDecryptionEnabled", RmsClientManager.IRMConfig.IsJournalReportDecryptionEnabledForTenant(organizationId)),
				new XElement("searchEnabled", RmsClientManager.IRMConfig.IsSearchEnabledForTenant(organizationId)),
				new XElement("eDiscoverySuperUserEnabled", RmsClientManager.IRMConfig.IsEDiscoverySuperUserEnabledForTenant(organizationId)),
				new XElement("federatedMailbox", RmsClientManager.IRMConfig.GetTenantFederatedMailbox(organizationId)),
				new XElement("publishingLocation", RmsClientManager.IRMConfig.GetTenantPublishingLocation(organizationId)),
				new XElement("serviceLocation", RmsClientManager.IRMConfig.GetTenantServiceLocation(organizationId)),
				new XElement("rmsOnlineKeySharingLocation", RmsClientManager.IRMConfig.GetTenantRMSOnlineKeySharingLocation(organizationId)),
				new XElement("serverCertificatesVersion", RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(organizationId)),
				list
			});
			return xelement;
		}

		internal static bool IsPublishedByOrganizationRMS(OrganizationId organizationId, string publishLicense)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfNullOrEmpty("publishLicense", publishLicense);
			Uri uri;
			Uri uri2;
			XmlNode[] array;
			if (!RMUtil.TryGetIssuanceLicenseAndUrls(publishLicense, out uri, out uri2, out array))
			{
				RmsClientManager.Tracer.TraceError(0L, "IsPublishedByOrganizationRMS: failed to extract licensing URIs from publish license");
				throw new RightsManagementException(RightsManagementFailureCode.InvalidIssuanceLicense, ServerStrings.FailedToCheckPublishLicenseOwnership(organizationId.ToString()));
			}
			Uri uri3 = uri ?? uri2;
			if (organizationId != OrganizationId.ForestWideOrgId && uri2 != null)
			{
				uri3 = uri2;
			}
			if (uri3 == null)
			{
				RmsClientManager.Tracer.TraceError(0L, "IsPublishedByOrganizationRMS: couldn't find a licensing URI in the publish license");
				throw new RightsManagementException(RightsManagementFailureCode.InvalidIssuanceLicense, ServerStrings.FailedToCheckPublishLicenseOwnership(organizationId.ToString()));
			}
			return RmsClientManager.IsInternalRMSLicensingUri(organizationId, uri3);
		}

		private static void Shutdown()
		{
			if (RmsClientManager.initialized)
			{
				if (RmsClientManager.licenseStoreManager != null)
				{
					RmsClientManager.licenseStoreManager.Clear();
					RmsClientManager.licenseStoreManager = null;
				}
				if (RmsClientManager.drmEnvironment != null)
				{
					RmsClientManager.drmEnvironment.CloseHandles();
				}
				RmsClientManagerLog.Stop();
				RmsClientManager.initialized = false;
			}
		}

		private static SafeRightsManagementHandle VerifyDelegatedEmailDRMPropsSignatureAndGetDecryptor(RmsClientManagerContext context, string userIdentity, bool isSuperUser, string useLicense, string publishLicense, Uri licensingUri, List<string> delegatedAddresses)
		{
			ArgumentValidator.ThrowIfNullOrEmpty(userIdentity, "userIdentity");
			if (!SmtpAddress.IsValidSmtpAddress(userIdentity))
			{
				ArgumentException ex = new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is not a valid SMTP address", new object[]
				{
					userIdentity
				}), "userIdentity");
				RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex);
				throw ex;
			}
			SafeRightsManagementHandle safeRightsManagementHandle = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				try
				{
					RmsClientManager.BindUseLicenseForDecryption(context, licensingUri, useLicense, publishLicense, out safeRightsManagementHandle);
					disposeGuard.Add<SafeRightsManagementHandle>(safeRightsManagementHandle);
				}
				catch (RightsManagementException ex2)
				{
					RmsClientManager.TraceFail(null, context.SystemProbeId, "VerifyDelegatedEmailDRMPropsSignature: BindUseLicenseForDecryption failed.  Exception: {0}", new object[]
					{
						ex2
					});
					BadDRMPropsSignatureException ex3 = new BadDRMPropsSignatureException(ServerStrings.FailedToBindToUseLicense, ex2);
					RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex3);
					throw ex3;
				}
				if (isSuperUser || delegatedAddresses.Contains(userIdentity, StringComparer.OrdinalIgnoreCase))
				{
					RmsClientManagerLog.LogVerifySignatureResult(context, userIdentity);
					disposeGuard.Success();
					return safeRightsManagementHandle;
				}
				ProxyAddressCollection proxyAddressCollection = null;
				try
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(context.OrgId), 2436, "VerifyDelegatedEmailDRMPropsSignatureAndGetDecryptor", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\rightsmanagement\\RmsClientManager.cs");
					MiniRecipient miniRecipient = tenantOrRootOrgRecipientSession.FindMiniRecipientByProxyAddress<MiniRecipient>(new SmtpProxyAddress(userIdentity, false), RmsClientManager.FindByEmailAddressesProperties);
					if (miniRecipient == null)
					{
						BadDRMPropsSignatureException ex4 = new BadDRMPropsSignatureException(ServerStrings.CannotVerifyDRMPropsSignatureUserNotFound(userIdentity));
						RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex4);
						throw ex4;
					}
					proxyAddressCollection = (ProxyAddressCollection)miniRecipient[ADRecipientSchema.EmailAddresses];
				}
				catch (TransientException ex5)
				{
					RmsClientManager.TraceFail(null, context.SystemProbeId, "VerifyDelegatedEmailDRMPropsSignature: FindADUser failed.  Exception: {0}", new object[]
					{
						ex5
					});
					BadDRMPropsSignatureException ex6 = new BadDRMPropsSignatureException(ServerStrings.FailedToVerifyDRMPropsSignatureADError(userIdentity), ex5);
					RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex6);
					throw ex6;
				}
				catch (DataValidationException ex7)
				{
					RmsClientManager.TraceFail(null, context.SystemProbeId, "VerifyDelegatedEmailDRMPropsSignature: FindADUser failed.  Exception: {0}", new object[]
					{
						ex7
					});
					BadDRMPropsSignatureException ex8 = new BadDRMPropsSignatureException(ServerStrings.FailedToVerifyDRMPropsSignatureADError(userIdentity), ex7);
					RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex8);
					throw ex8;
				}
				catch (DataSourceOperationException ex9)
				{
					RmsClientManager.TraceFail(null, context.SystemProbeId, "VerifyDelegatedEmailDRMPropsSignature: FindADUser failed.  Exception: {0}", new object[]
					{
						ex9
					});
					BadDRMPropsSignatureException ex10 = new BadDRMPropsSignatureException(ServerStrings.FailedToVerifyDRMPropsSignatureADError(userIdentity), ex9);
					RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex10);
					throw ex10;
				}
				if (proxyAddressCollection == null)
				{
					BadDRMPropsSignatureException ex11 = new BadDRMPropsSignatureException();
					RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex11);
					throw ex11;
				}
				foreach (ProxyAddress proxyAddress in proxyAddressCollection)
				{
					if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp && delegatedAddresses.Contains(proxyAddress.AddressString, StringComparer.OrdinalIgnoreCase))
					{
						RmsClientManager.TraceFail(null, context.SystemProbeId, "VerifyDelegatedEmailDRMPropsSignature: Matched user {0}", new object[]
						{
							proxyAddress.AddressString
						});
						RmsClientManagerLog.LogVerifySignatureResult(context, proxyAddress.AddressString);
						disposeGuard.Success();
						return safeRightsManagementHandle;
					}
				}
			}
			BadDRMPropsSignatureException ex12 = new BadDRMPropsSignatureException();
			RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex12);
			throw ex12;
		}

		private static SafeRightsManagementHandle VerifySuperUserDRMPropsSignatureAndGetDecryptor(RmsClientManagerContext context, SecurityIdentifier userSid, bool isSuperUser, ContentRight usageRights, ExDateTime expiryTime, byte[] drmPropsSignature, string useLicense, string publishLicense, UsageRightsSignatureVerificationOptions verificationOptions, IEnumerable<SecurityIdentifier> userSidHistory, Uri licensingUri)
		{
			using (DisposableTenantLicensePair disposableTenantLicensePair = RmsClientManager.AcquireTenantLicenses(context, licensingUri))
			{
				if (isSuperUser)
				{
					RmsClientManagerLog.LogVerifySignatureResult(context, userSid.Value);
					SafeRightsManagementHandle result;
					RmsClientManager.BindUseLicenseForDecryption(disposableTenantLicensePair.EnablingPrincipalRac, useLicense, publishLicense, out result);
					return result;
				}
				using (RightsSignatureBuilder rightsSignatureBuilder = new RightsSignatureBuilder(useLicense, publishLicense, RmsClientManager.EnvironmentHandle, disposableTenantLicensePair))
				{
					try
					{
						byte[] y = rightsSignatureBuilder.Sign(usageRights, expiryTime, userSid);
						if (ArrayComparer<byte>.Comparer.Equals(drmPropsSignature, y))
						{
							RmsClientManagerLog.LogVerifySignatureResult(context, userSid.Value);
							return rightsSignatureBuilder.GetDuplicateDecryptorHandle();
						}
					}
					catch (RightsManagementException ex)
					{
						RmsClientManager.TraceFail(null, context.SystemProbeId, "VerifySuperUserDRMPropsSignature: Sign failed.  Exception: {0}", new object[]
						{
							ex.Message
						});
						BadDRMPropsSignatureException ex2 = new BadDRMPropsSignatureException(ServerStrings.FailedToVerifyDRMPropsSignature(userSid.Value, (int)ex.FailureCode));
						RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex2);
						throw ex2;
					}
					IEnumerable<SecurityIdentifier> enumerable;
					if (verificationOptions == UsageRightsSignatureVerificationOptions.LookupSidHistory)
					{
						IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(context.OrgId), 2656, "VerifySuperUserDRMPropsSignatureAndGetDecryptor", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\rightsmanagement\\RmsClientManager.cs");
						try
						{
							MiniRecipient miniRecipient = tenantOrRootOrgRecipientSession.FindMiniRecipientBySid<MiniRecipient>(userSid, RmsClientManager.FindBySidProperties);
							if (miniRecipient == null)
							{
								BadDRMPropsSignatureException ex3 = new BadDRMPropsSignatureException(ServerStrings.CannotVerifyDRMPropsSignatureUserNotFound(userSid.Value));
								RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex3);
								throw ex3;
							}
							enumerable = (MultiValuedProperty<SecurityIdentifier>)miniRecipient[ADMailboxRecipientSchema.SidHistory];
							goto IL_1C5;
						}
						catch (DataValidationException ex4)
						{
							RmsClientManager.TraceFail(null, context.SystemProbeId, "VerifySuperUserDRMPropsSignature: FindADUserBySid failed.  Exception: {0}", new object[]
							{
								ex4
							});
							BadDRMPropsSignatureException ex5 = new BadDRMPropsSignatureException(ServerStrings.FailedToVerifyDRMPropsSignatureADError(userSid.Value), ex4);
							RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex5);
							throw ex5;
						}
						catch (DataSourceOperationException ex6)
						{
							RmsClientManager.TraceFail(null, context.SystemProbeId, "VerifySuperUserDRMPropsSignature: FindADUserBySid failed.  Exception: {0}", new object[]
							{
								ex6
							});
							BadDRMPropsSignatureException ex7 = new BadDRMPropsSignatureException(ServerStrings.FailedToVerifyDRMPropsSignatureADError(userSid.Value), ex6);
							RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex7);
							throw ex7;
						}
					}
					enumerable = userSidHistory;
					IL_1C5:
					if (enumerable == null)
					{
						BadDRMPropsSignatureException ex8 = new BadDRMPropsSignatureException();
						RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex8);
						throw ex8;
					}
					foreach (SecurityIdentifier securityIdentifier in enumerable)
					{
						try
						{
							byte[] y2 = rightsSignatureBuilder.Sign(usageRights, expiryTime, securityIdentifier);
							if (ArrayComparer<byte>.Comparer.Equals(drmPropsSignature, y2))
							{
								RmsClientManagerLog.LogVerifySignatureResult(context, securityIdentifier.Value);
								return rightsSignatureBuilder.GetDuplicateDecryptorHandle();
							}
						}
						catch (RightsManagementException ex9)
						{
							RmsClientManager.TraceFail(null, context.SystemProbeId, "VerifySuperUserDRMPropsSignature: Sign failed.  Exception: {0}", new object[]
							{
								ex9
							});
							BadDRMPropsSignatureException ex10 = new BadDRMPropsSignatureException(ServerStrings.FailedToVerifyDRMPropsSignature(userSid.Value, (int)ex9.FailureCode), ex9);
							RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex10);
							throw ex10;
						}
					}
				}
			}
			BadDRMPropsSignatureException ex11 = new BadDRMPropsSignatureException();
			RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, context, ex11);
			throw ex11;
		}

		private static IAsyncResult BeginAcquireInternalOrganizationRACAndCLC(RmsClientManagerContext context, AsyncCallback callback, object state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			RmsClientManager.TracePass(null, context.SystemProbeId, "BeginAcquireInternalOrganizationRACAndCLC: Acquiring tenant license pair (RAC/CLC) for tenant {0}", new object[]
			{
				context.OrgId
			});
			TenantLicenseAsyncResult tenantLicenseAsyncResult = new TenantLicenseAsyncResult(context, state, callback);
			bool flag = false;
			tenantLicenseAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireInternalOrganizationRACAndCLC);
			try
			{
				if (!RmsClientManager.IgnoreLicensingEnabled && !RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(context.OrgId))
				{
					RmsClientManagerLog.LogAcquireRacClc(context);
					flag = true;
					if (RmsClientManager.UseOfflineRms)
					{
						RmsClientManager.TraceFail(null, context.SystemProbeId, "InternalLicensing is disabled for tenant {0}.", new object[]
						{
							context.OrgId
						});
						tenantLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.InternalLicensingDisabled, ServerStrings.InternalLicensingDisabledForTenant(context.OrgId)));
					}
					else
					{
						RmsClientManager.TraceFail(null, context.SystemProbeId, "InternalLicensing is disabled for Enterprise.", new object[0]);
						tenantLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.InternalLicensingDisabled, ServerStrings.InternalLicensingDisabledForEnterprise));
					}
					return tenantLicenseAsyncResult;
				}
				Uri tenantServiceLocation = RmsClientManager.IRMConfig.GetTenantServiceLocation(context.OrgId);
				Uri tenantPublishingLocation = RmsClientManager.IRMConfig.GetTenantPublishingLocation(context.OrgId);
				byte tenantServerCertificatesVersion = RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(context.OrgId);
				TenantLicensePair tenantLicensePair;
				if (RmsClientManager.licenseStoreManager.ReadFromStore(context.TenantId, tenantServiceLocation, tenantPublishingLocation, tenantServerCertificatesVersion, out tenantLicensePair))
				{
					RmsClientManager.TracePass(null, context.SystemProbeId, "Found the tenant licenses in the license store", new object[0]);
					RmsClientManagerLog.LogAcquireRacClcCacheResult(context, tenantServiceLocation, tenantPublishingLocation, tenantServerCertificatesVersion);
					tenantLicenseAsyncResult.InvokeCallback();
					return tenantLicenseAsyncResult;
				}
				RmsClientManagerLog.LogAcquireRacClc(context);
				flag = true;
				if (RmsClientManager.UseOfflineRms)
				{
					try
					{
						XmlNode[] rac = null;
						XmlNode[] clc = null;
						context.LatencyTracker.BeginTrackRmsLatency(RmsOperationType.AcquireServerBoxRac);
						ServerManager.AcquireTenantLicenses(context, RmsClientManager.GetMachineCertificateChain(context), RmsClientManager.IRMConfig.GetTenantFederatedMailbox(context.OrgId), out rac, out clc);
						context.LatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireServerBoxRac);
						RmsClientManager.TracePass(null, context.SystemProbeId, "Writing RAC/CLC acquired from OfflineRMS component for tenant {0} to the license store", new object[]
						{
							context.TenantId
						});
						RmsClientManager.licenseStoreManager.WriteToStore(context.TenantId, rac, clc, tenantServerCertificatesVersion);
						tenantLicenseAsyncResult.InvokeCallback();
						return tenantLicenseAsyncResult;
					}
					catch (RightsManagementServerException ex)
					{
						context.LatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireServerBoxRac);
						RmsClientManager.TraceFail(null, context.SystemProbeId, "Failed to acquire tenant license for {0} from offline RMS. Error {1}", new object[]
						{
							context.TenantId,
							ex
						});
						tenantLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.RacAcquisitionFailed, ServerStrings.FailedToAcquireTenantLicenses(context.OrgId.ToString(), tenantServiceLocation.ToString()), ex));
						return tenantLicenseAsyncResult;
					}
					catch (ExchangeConfigurationException ex2)
					{
						context.LatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireServerBoxRac);
						RmsClientManager.TraceFail(null, context.SystemProbeId, "Failed to read configuration when reading tenant's federated mailbox. Error {0}", new object[]
						{
							ex2
						});
						tenantLicenseAsyncResult.InvokeCallback(ex2);
						return tenantLicenseAsyncResult;
					}
				}
				Uri uri = RmsClientManager.GetRMSServiceLocation(context.OrgId, ServiceType.Certification);
				if (uri == null)
				{
					RmsClientManager.TracePass(null, context.SystemProbeId, "Failed to find service location for certification", new object[0]);
					tenantLicenseAsyncResult.InvokeCallback();
					return tenantLicenseAsyncResult;
				}
				uri = RmsoProxyUtil.GetCertificationServerRedirectUrl(uri);
				WebProxy localServerProxy = RmsClientManagerUtils.GetLocalServerProxy(RmsClientManager.UseOfflineRms);
				bool flag2 = RmsClientManager.outstandingPerTenantFindLicensesCalls.EnqueueResult(context.TenantId, tenantLicenseAsyncResult);
				if (flag2)
				{
					tenantLicenseAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireInternalOrganizationRACAndCLCFirstRequest);
					tenantLicenseAsyncResult.ServerCertificationManager = new ServerCertificationWSManager(uri, RmsClientManager.perfCounters, context.LatencyTracker, localServerProxy, RmsClientManager.AppSettings.RmsSoapQueriesTimeout);
					RmsClientManager.TracePass(null, context.SystemProbeId, "No outstanding calls for the tenant to acquire tenant licenses. Acquiring server box RAC from {0}. Using cert based auth: {1}", new object[]
					{
						uri
					});
					RmsClientManagerLog.LogAcquireRac(context, uri);
					tenantLicenseAsyncResult.ServerCertificationManager.BeginAcquireRac(RmsClientManager.GetMachineCertificateChain(context), RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireServerRacCallback)), tenantLicenseAsyncResult);
				}
				else
				{
					RmsClientManager.TracePass(null, context.SystemProbeId, "BeginAcquireInternalOrganizationRACAndCLC call for the tenant is already pending - enqueued to the find tenant queue", new object[0]);
					RmsClientManagerLog.LogUriEvent(RmsClientManagerLog.RmsClientManagerFeature.RacClc, RmsClientManagerLog.RmsClientManagerEvent.Queued, context, uri);
					tenantLicenseAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireInternalOrganizationRACAndCLCPendingRequest);
				}
			}
			catch (RightsManagementException ex3)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "Failed to initialize RmsClientManager. Error {0}", new object[]
				{
					ex3
				});
				if (!flag)
				{
					RmsClientManagerLog.LogAcquireRacClc(context);
				}
				tenantLicenseAsyncResult.InvokeCallback(ex3);
				return tenantLicenseAsyncResult;
			}
			catch (ExchangeConfigurationException ex4)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "Failed to read configuration when initializing RmsClientManager. Error {0}", new object[]
				{
					ex4
				});
				if (!flag)
				{
					RmsClientManagerLog.LogAcquireRacClc(context);
				}
				tenantLicenseAsyncResult.InvokeCallback(ex4);
				return tenantLicenseAsyncResult;
			}
			return tenantLicenseAsyncResult;
		}

		private static DisposableTenantLicensePair EndAcquireInternalOrganizationRACAndCLC(IAsyncResult asyncResult)
		{
			TenantLicenseAsyncResult tenantLicenseAsyncResult = asyncResult as TenantLicenseAsyncResult;
			if (tenantLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult cannot be null and has to be type of TenantLicenseAsyncResult");
			}
			tenantLicenseAsyncResult.AddBreadCrumb(Constants.State.EndAcquireInternalOrganizationRACAndCLC);
			if (!tenantLicenseAsyncResult.IsCompleted)
			{
				tenantLicenseAsyncResult.InternalWaitForCompletion();
			}
			Exception ex = tenantLicenseAsyncResult.Result as Exception;
			if (ex != null)
			{
				RmsClientManager.TraceFail(null, tenantLicenseAsyncResult.Context.SystemProbeId, "EndAcquireInternalOrganizationRACAndCLC hit an exception {0}", new object[]
				{
					ex
				});
				RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.RacClc, tenantLicenseAsyncResult.Context, ex);
				throw ex;
			}
			TenantLicensePair tenantLicensePair = null;
			Uri tenantServiceLocation = RmsClientManager.IRMConfig.GetTenantServiceLocation(tenantLicenseAsyncResult.Context.OrgId);
			Uri tenantPublishingLocation = RmsClientManager.IRMConfig.GetTenantPublishingLocation(tenantLicenseAsyncResult.Context.OrgId);
			byte tenantServerCertificatesVersion = RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(tenantLicenseAsyncResult.Context.OrgId);
			DisposableTenantLicensePair disposableTenantLicensePair;
			if (!RmsClientManager.licenseStoreManager.ReadFromStore(tenantLicenseAsyncResult.Context.TenantId, tenantServiceLocation, tenantPublishingLocation, tenantServerCertificatesVersion, out tenantLicensePair))
			{
				RmsClientManager.TraceFail(null, tenantLicenseAsyncResult.Context.SystemProbeId, "Failed to acquire tenant licenses for tenant {0}", new object[]
				{
					tenantLicenseAsyncResult.Context.OrgId
				});
				disposableTenantLicensePair = null;
			}
			else
			{
				RmsClientManager.TracePass(null, tenantLicenseAsyncResult.Context.SystemProbeId, "Successfully acquired tenant licenses for tenant {0}", new object[]
				{
					tenantLicenseAsyncResult.Context.OrgId
				});
				RmsClientManagerLog.LogAcquireRacClcResult(tenantLicenseAsyncResult.Context, tenantLicensePair);
				disposableTenantLicensePair = DisposableTenantLicensePair.CreateDisposableTenantLicenses(tenantLicensePair);
			}
			if (disposableTenantLicensePair == null)
			{
				RmsClientManager.TraceFail(null, tenantLicenseAsyncResult.Context.SystemProbeId, "Could not find the tenant licenses or the licenses are invalid - tenant {0}", new object[]
				{
					tenantLicenseAsyncResult.Context.OrgId
				});
				throw new RightsManagementException(RightsManagementFailureCode.InvalidTenantLicense, ServerStrings.TenantLicensesDistributionPointMismatch(tenantLicenseAsyncResult.Context.OrgId.ToString(), tenantServiceLocation, tenantPublishingLocation))
				{
					IsPermanent = false
				};
			}
			return disposableTenantLicensePair;
		}

		private static IAsyncResult BeginAcquireSuperUserUseLicense(RmsClientManagerContext context, Uri licenseUri, XmlNode[] issuanceLicense, AsyncCallback callback, object state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			ArgumentValidator.ThrowIfNull("issuanceLicense", issuanceLicense);
			SuperUserUseLicenseAsyncResult superUserUseLicenseAsyncResult = new SuperUserUseLicenseAsyncResult(context, licenseUri, issuanceLicense, state, callback);
			superUserUseLicenseAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireSuperUserUseLicense);
			RmsClientManager.TracePass(null, context.SystemProbeId, "BeginAcquireSuperUserUseLicense: Acquiring use license for tenant {0}, from {1}", new object[]
			{
				context.OrgId,
				licenseUri
			});
			RmsClientManager.TracePass(null, context.SystemProbeId, "Trying to acquire tenant licenses before querying for the use license", new object[0]);
			RmsClientManager.BeginAcquireInternalOrganizationRACAndCLC(context, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireTenantLicenseCallback)), superUserUseLicenseAsyncResult);
			return superUserUseLicenseAsyncResult;
		}

		private static IAsyncResult BeginAcquireFederationRAC(RmsClientManagerContext context, Uri licenseUri, AsyncCallback callback, object state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			FederationRacAsyncResult federationRacAsyncResult = new FederationRacAsyncResult(context, licenseUri, state, callback);
			federationRacAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireFederationRAC);
			if (!ExternalAuthentication.GetCurrent().Enabled)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "BeginAcquireFederationRAC: Federation with Live is not established or the federation trust is not enabled. Cannot acquire a federation RAC", new object[0]);
				federationRacAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.FederationNotEnabled, ServerStrings.FederationNotEnabled));
				return federationRacAsyncResult;
			}
			try
			{
				if (!RmsClientManager.IRMConfig.IsExternalLicensingEnabledForTenant(context.OrgId))
				{
					if (RmsClientManager.UseOfflineRms)
					{
						RmsClientManager.TraceFail(null, context.SystemProbeId, "ExternalLicensing is disabled for tenant {0}. License Uri '{1}'.", new object[]
						{
							context.OrgId,
							licenseUri
						});
						federationRacAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.ExternalLicensingDisabled, ServerStrings.ExternalLicensingDisabledForTenant(licenseUri, context.OrgId)));
					}
					else
					{
						RmsClientManager.TraceFail(null, context.SystemProbeId, "ExternalLicensing is disabled for Enterprise. License Uri '{0}'.", new object[]
						{
							licenseUri
						});
						federationRacAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.ExternalLicensingDisabled, ServerStrings.ExternalLicensingDisabledForEnterprise(licenseUri)));
					}
					return federationRacAsyncResult;
				}
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "Failed to initialize RmsClientManager or to read IsExternalLicensingEnabled flag. Error {0}", new object[]
				{
					ex
				});
				federationRacAsyncResult.InvokeCallback(ex);
				return federationRacAsyncResult;
			}
			catch (ExchangeConfigurationException ex2)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "Failed to initialize RmsClientManager or to read IsExternalLicensingEnabled flag due to a configuration error. Error {0}", new object[]
				{
					ex2
				});
				federationRacAsyncResult.InvokeCallback(ex2);
				return federationRacAsyncResult;
			}
			TenantLicensePair tenantLicensePair;
			if (RmsClientManager.licenseStoreManager.ReadFromStore(context.TenantId, licenseUri, RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(context.OrgId), out tenantLicensePair))
			{
				RmsClientManager.TracePass(null, context.SystemProbeId, "Found the tenant licenses in the license store", new object[0]);
				federationRacAsyncResult.InvokeCallback();
				return federationRacAsyncResult;
			}
			RmsClientManager.TracePass(null, context.SystemProbeId, "BeginAcquireFederationRAC: Acquiring federation RAC for tenant {0}, from {1}", new object[]
			{
				context.OrgId,
				licenseUri
			});
			RmsServerInfoManager.BeginAcquireServerInfo(context, licenseUri, federationRacAsyncResult, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireExternalRMSInfoCertificationCallback)));
			return federationRacAsyncResult;
		}

		private static DisposableTenantLicensePair EndAcquireFederationRAC(IAsyncResult asyncResult)
		{
			FederationRacAsyncResult federationRacAsyncResult = asyncResult as FederationRacAsyncResult;
			if (federationRacAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult cannot be null and has to be type of FederationRacAsyncResult");
			}
			federationRacAsyncResult.AddBreadCrumb(Constants.State.EndAcquireFederationRAC);
			if (!federationRacAsyncResult.IsCompleted)
			{
				federationRacAsyncResult.InternalWaitForCompletion();
			}
			Exception ex = federationRacAsyncResult.Result as Exception;
			if (ex != null)
			{
				RmsClientManager.TraceFail(null, federationRacAsyncResult.Context.SystemProbeId, "EndAcquireFederationRAC hit an exception {0}", new object[]
				{
					ex
				});
				throw ex;
			}
			TenantLicensePair licensePair = null;
			DisposableTenantLicensePair disposableTenantLicensePair;
			if (!RmsClientManager.licenseStoreManager.ReadFromStore(federationRacAsyncResult.Context.TenantId, federationRacAsyncResult.LicenseUri, RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(federationRacAsyncResult.Context.OrgId), out licensePair))
			{
				RmsClientManager.TraceFail(null, federationRacAsyncResult.Context.SystemProbeId, "Failed to acquire tenant licenses for tenant {0}", new object[]
				{
					federationRacAsyncResult.Context.OrgId
				});
				disposableTenantLicensePair = null;
			}
			else
			{
				RmsClientManager.TracePass(null, federationRacAsyncResult.Context.SystemProbeId, "Successfully found the federation rac for tenant {0}", new object[]
				{
					federationRacAsyncResult.Context.OrgId
				});
				disposableTenantLicensePair = DisposableTenantLicensePair.CreateDisposableTenantLicenses(licensePair);
			}
			if (disposableTenantLicensePair == null)
			{
				RmsClientManager.TraceFail(null, federationRacAsyncResult.Context.SystemProbeId, "Could not find the tenant licenses or the licenses are invalid", new object[0]);
				throw new RightsManagementException(RightsManagementFailureCode.RacAcquisitionFailed, ServerStrings.FailedToAcquireTenantLicenses(federationRacAsyncResult.Context.OrgId.ToString(), federationRacAsyncResult.LicenseUri.AbsoluteUri))
				{
					IsPermanent = false
				};
			}
			return disposableTenantLicensePair;
		}

		private static IAsyncResult BeginAcquireFederationServerLicense(RmsClientManagerContext context, LicenseIdentity[] identities, LicenseResponse[] responses, XmlNode[] issuanceLicense, Uri licenseUri, AsyncCallback callback, object state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			ArgumentValidator.ThrowIfNull("issuanceLicense", issuanceLicense);
			RmsClientManagerUtils.ThrowOnNullOrEmptyArrayArgument("identities", identities);
			FederationServerLicenseAsyncResult federationServerLicenseAsyncResult = new FederationServerLicenseAsyncResult(context, licenseUri, issuanceLicense, identities, responses, state, callback);
			federationServerLicenseAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireFederationServerLicense);
			if (!ExternalAuthentication.GetCurrent().Enabled)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "BeginAcquireFederationServerLicense: Federation with Live is not established or the federation trust is not enabled. Cannot acquire a federation server use license", new object[0]);
				federationServerLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.FederationNotEnabled, ServerStrings.FederationNotEnabled));
				return federationServerLicenseAsyncResult;
			}
			RmsClientManager.TracePass(null, context.SystemProbeId, "BeginAcquireFederationServerLicense: Acquiring server use license for tenant {0}, from {1}. Recipient Addresses {2}", new object[]
			{
				context.OrgId,
				licenseUri,
				LicenseIdentity.ToString(identities)
			});
			RmsClientManager.BeginAcquireFederationRAC(context, licenseUri, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireFederationRacCallback)), federationServerLicenseAsyncResult);
			return federationServerLicenseAsyncResult;
		}

		private static IAsyncResult BeginAcquireServerPreLicense(RmsClientManagerContext context, Uri prelicenseUri, XmlNode[] issuanceLicense, string[] identities, AsyncCallback callback, object state)
		{
			PreLicenseAsyncResult preLicenseAsyncResult = new PreLicenseAsyncResult(context, null, state, callback);
			RmsClientManagerLog.LogAcquirePrelicense(context, prelicenseUri, identities);
			IAsyncResult result;
			try
			{
				using (DisposableTenantLicensePair disposableTenantLicensePair = RmsClientManager.AcquireTenantLicenses(context, prelicenseUri))
				{
					try
					{
						LicenseeIdentity[] licenseeIdentities = RmsClientManagerUtils.ConvertToLicenseeIdentities(context, identities);
						context.LatencyTracker.BeginTrackRmsLatency(RmsOperationType.AcquirePrelicense);
						UseLicenseResult[] endUseLicenses = ServerManager.AcquireUseLicenses(context, disposableTenantLicensePair.Rac, issuanceLicense, licenseeIdentities);
						context.LatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquirePrelicense);
						preLicenseAsyncResult.Responses = RmsClientManagerUtils.GetLicenseResponses(endUseLicenses, prelicenseUri);
						preLicenseAsyncResult.InvokeCallback();
					}
					catch (RightsManagementServerException ex)
					{
						RmsClientManager.TraceFail(null, context.SystemProbeId, "BeginAcquireServerPreLicense: Hit an exception when processing a prelicense request for organization {0} against {1} with offline RMS. Details: {2}", new object[]
						{
							context.OrgId,
							prelicenseUri,
							ex
						});
						context.LatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquirePrelicense);
						preLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.OfflineRmsServerFailure, ServerStrings.FailedToAcquireUseLicense(prelicenseUri), ex));
					}
					result = preLicenseAsyncResult;
				}
			}
			catch (RightsManagementException ex2)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "BeginAcquireServerPreLicense: Hit an exception when processing a prelicense request for organization {0} against {1}. Details: {2}", new object[]
				{
					context.OrgId,
					prelicenseUri,
					ex2
				});
				preLicenseAsyncResult.InvokeCallback(ex2);
				result = preLicenseAsyncResult;
			}
			catch (ExchangeConfigurationException ex3)
			{
				RmsClientManager.TraceFail(null, context.SystemProbeId, "BeginAcquireServerPreLicense: Failed to read configuration when processing a prelicense request for organization {0} against {1}. Details: {2}", new object[]
				{
					context.OrgId,
					prelicenseUri,
					ex3
				});
				preLicenseAsyncResult.InvokeCallback(ex3);
				result = preLicenseAsyncResult;
			}
			return result;
		}

		private static void InitializeIfNeeded()
		{
			if (RmsClientManager.initialized)
			{
				return;
			}
			lock (RmsClientManager.initializationLock)
			{
				if (!RmsClientManager.initialized)
				{
					RmsClientManager.Initialize();
					RmsClientManager.initialized = true;
				}
			}
		}

		private static void Initialize()
		{
			RmsClientManagerLog.Start();
			RmsClientManager.useOfflineRms = VariantConfiguration.InvariantNoFlightingSnapshot.DataStorage.UseOfflineRms.Enabled;
			RmsClientManager.perfCounters = new RmsPerformanceCounters();
			RmsClientManager.perfCounters.Initialize();
			RmsClientManager.drmEnvironment = RmsClientManager.DrmEnvironment.InitializeEnvironment();
			RmsClientManager.InitializeIRMConfig();
			string text;
			using (WindowsIdentity current = WindowsIdentity.GetCurrent(TokenAccessLevels.Query))
			{
				text = Path.Combine(DrmEmailConstants.LicenseStorePath, current.User.ToString());
			}
			RmsClientManager.licenseStoreManager = new RmsLicenseStoreManager(text, RmsClientManager.AppSettings.MaxRacClcEntryCount, RmsClientManager.perfCounters);
			RmsServerInfoManager.InitializeServerInfoMap(text, RmsClientManager.AppSettings.MaxServerInfoEntryCount, RmsClientManager.perfCounters);
			RmsClientManager.Tracer.TraceDebug<bool, string>(0L, "Initialize: DatacenterMode {0}, LicenseStorePath :{1}", RmsClientManager.UseOfflineRms, text);
		}

		private static void InitializeIRMConfig()
		{
			RmsClientManager.irmConfig = RmsConfiguration.Instance;
			try
			{
				RmsClientManager.irmConfig.Load(RmsClientManager.UseOfflineRms);
			}
			catch (ExchangeConfigurationException innerException)
			{
				throw new ExchangeConfigurationException(DrmStrings.FailedToInitializeRMSEnvironment, innerException);
			}
		}

		private static void ThrowIfNotInitialized()
		{
			if (!RmsClientManager.initialized)
			{
				throw new InvalidOperationException("Rights management environment is not initialized");
			}
		}

		private static void AcquirePreLicenseCallback(IAsyncResult asyncResult)
		{
			RmsClientManager.Tracer.TraceDebug(0L, "AcquirePreLicenseCallback called");
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			PreLicenseAsyncResult preLicenseAsyncResult = asyncResult.AsyncState as PreLicenseAsyncResult;
			if (preLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of PreLicenseAsyncResult.");
			}
			preLicenseAsyncResult.AddBreadCrumb(Constants.State.AcquirePreLicenseCallback);
			PreLicenseWSManager preLicenseManager = preLicenseAsyncResult.PreLicenseManager;
			if (preLicenseManager == null)
			{
				throw new InvalidOperationException("result.AsyncObject cannot be null and has to be type of PreLicenseWSManager");
			}
			Exception value = null;
			try
			{
				preLicenseAsyncResult.Responses = preLicenseManager.EndAcquireLicense(asyncResult);
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, preLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquirePreLicenseCallback - {0}", new object[]
				{
					ex
				});
				value = ex;
			}
			catch (ExchangeConfigurationException ex2)
			{
				RmsClientManager.TraceFail(null, preLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquirePreLicenseCallback - {0}", new object[]
				{
					ex2
				});
				value = ex2;
			}
			finally
			{
				preLicenseAsyncResult.ReleaseManagers();
			}
			RmsClientManager.TracePass(null, preLicenseAsyncResult.Context.SystemProbeId, "Invoking prelicense callback", new object[0]);
			preLicenseAsyncResult.InvokeCallback(value);
		}

		private static void AcquireRmsTemplatesCallback(IAsyncResult asyncResult)
		{
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			TemplateAsyncResult templateAsyncResult = asyncResult.AsyncState as TemplateAsyncResult;
			if (templateAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of TemplateAsyncResult.");
			}
			if (templateAsyncResult.Context.OrgId != OrganizationId.ForestWideOrgId)
			{
				throw new InvalidOperationException("result.Context.OrgId must correspond to first organization.");
			}
			templateAsyncResult.AddBreadCrumb(Constants.State.AcquireRmsTemplatesCallback);
			Exception result = null;
			try
			{
				RmsTemplate[] array = templateAsyncResult.TemplateManager.EndAcquireAllTemplates(asyncResult);
				Cache<Guid, RmsTemplate> cache = new Cache<Guid, RmsTemplate>(RmsClientManager.AppSettings.TemplateCacheSizeInBytes, RmsClientManager.AppSettings.TemplateCacheExpirationInterval, TimeSpan.Zero);
				foreach (RmsTemplate rmsTemplate in array)
				{
					RmsClientManager.TracePass(null, templateAsyncResult.Context.SystemProbeId, "Adding templateId {0} for first org to the template cache", new object[]
					{
						rmsTemplate.Id
					});
					cache.TryAdd(rmsTemplate.Id, rmsTemplate);
				}
				cache.TryAdd(RmsTemplate.DoNotForward.Id, RmsTemplate.DoNotForward);
				if (RmsClientManager.IRMConfig.IsInternetConfidentialEnabledForTenant(templateAsyncResult.Context.OrgId))
				{
					RmsClientManager.TracePass(null, templateAsyncResult.Context.SystemProbeId, "Adding InternetConfidential template first org to the template cache", new object[0]);
					cache.TryAdd(RmsTemplate.InternetConfidential.Id, RmsTemplate.InternetConfidential);
				}
				RmsClientManager.TemplateCacheForFirstOrg = cache;
				RmsClientManager.FirstOrgTemplateCacheVersion = RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(templateAsyncResult.Context.OrgId);
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, templateAsyncResult.Context.SystemProbeId, "Hit an exception during AcquireRmsTemplatesCallback {0}", new object[]
				{
					ex
				});
				result = ex;
			}
			finally
			{
				templateAsyncResult.ReleaseWsManagers();
			}
			RmsClientManager.TracePass(null, templateAsyncResult.Context.SystemProbeId, "Invoking find template callbacks", new object[0]);
			RmsClientManager.outstandingPerTenantFindTemplatesCalls.InvokeCallbacks(templateAsyncResult.Context.TenantId, result);
		}

		private static void AcquireServerRacCallback(IAsyncResult asyncResult)
		{
			RmsClientManager.Tracer.TraceDebug(0L, "AcquireServerRacCallback invoked");
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			TenantLicenseAsyncResult tenantLicenseAsyncResult = asyncResult.AsyncState as TenantLicenseAsyncResult;
			if (tenantLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of TenantLicenseAsyncResult.");
			}
			tenantLicenseAsyncResult.AddBreadCrumb(Constants.State.AcquireServerRacCallback);
			bool flag = true;
			Exception result = null;
			try
			{
				tenantLicenseAsyncResult.Rac = tenantLicenseAsyncResult.ServerCertificationManager.EndAcquireRac(asyncResult);
				Uri uri = RmsClientManager.GetRMSServiceLocation(tenantLicenseAsyncResult.Context.OrgId, ServiceType.ClientLicensor);
				uri = RmsoProxyUtil.GetCertificationServerRedirectUrl(uri);
				if (uri != null)
				{
					tenantLicenseAsyncResult.PublishManager = new PublishWSManager(uri, RmsClientManager.perfCounters, tenantLicenseAsyncResult.LatencyTracker, RmsClientManagerUtils.GetLocalServerProxy(RmsClientManager.UseOfflineRms), RmsClientManager.AppSettings.RmsSoapQueriesTimeout);
					RmsClientManager.TraceFail(null, tenantLicenseAsyncResult.Context.SystemProbeId, "Acquired server box RAC for org {0}, acquiring CLC from {1}", new object[]
					{
						tenantLicenseAsyncResult.Context.OrgId,
						uri
					});
					tenantLicenseAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireClc);
					RmsClientManagerLog.LogAcquireClc(tenantLicenseAsyncResult.Context, uri);
					tenantLicenseAsyncResult.PublishManager.BeginAcquireClc(tenantLicenseAsyncResult.Rac, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireClcCallback)), tenantLicenseAsyncResult);
					flag = false;
				}
				else
				{
					RmsClientManager.TraceFail(null, tenantLicenseAsyncResult.Context.SystemProbeId, "Failed to find serviceLocation for acquiring CLC for org {0}", new object[]
					{
						tenantLicenseAsyncResult.Context.OrgId
					});
				}
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, tenantLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquireServerRacCallback {0}", new object[]
				{
					ex
				});
				result = ex;
			}
			catch (ExchangeConfigurationException ex2)
			{
				RmsClientManager.TraceFail(null, tenantLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquireServerRacCallback {0}", new object[]
				{
					ex2
				});
				result = ex2;
			}
			finally
			{
				if (flag)
				{
					tenantLicenseAsyncResult.ReleaseWsManagers();
				}
			}
			if (flag)
			{
				RmsClientManager.TracePass(null, tenantLicenseAsyncResult.Context.SystemProbeId, "AcquireServerRacCallback: Invoking find tenant license callbacks", new object[0]);
				RmsClientManager.outstandingPerTenantFindLicensesCalls.InvokeCallbacks(tenantLicenseAsyncResult.Context.TenantId, result);
			}
		}

		private static void AcquireClcCallback(IAsyncResult asyncResult)
		{
			RmsClientManager.Tracer.TraceDebug(0L, "AcquireClcCallback invoked");
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			TenantLicenseAsyncResult tenantLicenseAsyncResult = asyncResult.AsyncState as TenantLicenseAsyncResult;
			if (tenantLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of TenantLicenseAsyncResult.");
			}
			tenantLicenseAsyncResult.AddBreadCrumb(Constants.State.AcquireClcCallback);
			Exception result = null;
			try
			{
				XmlNode[] clc = tenantLicenseAsyncResult.PublishManager.EndAcquireClc(asyncResult);
				RmsClientManager.TracePass(null, tenantLicenseAsyncResult.Context.SystemProbeId, "Writing RAC/CLC for tenant {0} to the license store", new object[]
				{
					tenantLicenseAsyncResult.Context.TenantId
				});
				RmsClientManager.licenseStoreManager.WriteToStore(tenantLicenseAsyncResult.Context.TenantId, tenantLicenseAsyncResult.Rac, clc, RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(tenantLicenseAsyncResult.Context.OrgId));
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, tenantLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquireClcCallback {0}", new object[]
				{
					ex
				});
				result = ex;
			}
			finally
			{
				tenantLicenseAsyncResult.ReleaseWsManagers();
			}
			RmsClientManager.outstandingPerTenantFindLicensesCalls.InvokeCallbacks(tenantLicenseAsyncResult.Context.TenantId, result);
		}

		private static void AcquireTenantLicenseCallback(IAsyncResult asyncResult)
		{
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			SuperUserUseLicenseAsyncResult superUserUseLicenseAsyncResult = asyncResult.AsyncState as SuperUserUseLicenseAsyncResult;
			if (superUserUseLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of SuperUserUseLicenseAsyncResult.");
			}
			superUserUseLicenseAsyncResult.AddBreadCrumb(Constants.State.AcquireTenantLicenseCallback);
			try
			{
				using (DisposableTenantLicensePair disposableTenantLicensePair = RmsClientManager.EndAcquireInternalOrganizationRACAndCLC(asyncResult))
				{
					if (RmsClientManager.UseOfflineRms)
					{
						Exception value = null;
						try
						{
							LicenseeIdentity licenseeIdentity = new LicenseeIdentity(RmsClientManager.IRMConfig.GetTenantFederatedMailbox(superUserUseLicenseAsyncResult.Context.OrgId), true);
							superUserUseLicenseAsyncResult.Context.LatencyTracker.BeginTrackRmsLatency(RmsOperationType.AcquireLicense);
							UseLicenseResult[] array = ServerManager.AcquireUseLicenses(superUserUseLicenseAsyncResult.Context, disposableTenantLicensePair.Rac, superUserUseLicenseAsyncResult.IssuanceLicense, new LicenseeIdentity[]
							{
								licenseeIdentity
							});
							superUserUseLicenseAsyncResult.Context.LatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireLicense);
							if (array != null && array[0] != null)
							{
								UseLicenseResult useLicenseResult = array[0];
								if (useLicenseResult.Error != null)
								{
									value = new RightsManagementException(RightsManagementFailureCode.UseLicenseAcquisitionFailed, ServerStrings.FailedToAcquireUseLicenses(superUserUseLicenseAsyncResult.Context.OrgId.ToString()), useLicenseResult.Error);
								}
								else
								{
									superUserUseLicenseAsyncResult.UseLicense = useLicenseResult.EndUseLicense;
								}
							}
							else
							{
								value = new RightsManagementException(RightsManagementFailureCode.UseLicenseAcquisitionFailed, ServerStrings.FailedToAcquireUseLicenses(superUserUseLicenseAsyncResult.Context.OrgId.ToString()));
							}
							return;
						}
						catch (RightsManagementServerException ex)
						{
							superUserUseLicenseAsyncResult.Context.LatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireLicense);
							RmsClientManager.TraceFail(null, superUserUseLicenseAsyncResult.Context.SystemProbeId, "Failed to acquire use license for {0} from offline RMS. Error {1}", new object[]
							{
								superUserUseLicenseAsyncResult.Context.OrgId,
								ex
							});
							value = new RightsManagementException(RightsManagementFailureCode.UseLicenseAcquisitionFailed, ServerStrings.FailedToAcquireUseLicenses(superUserUseLicenseAsyncResult.Context.OrgId.ToString()), ex);
							return;
						}
						catch (ExchangeConfigurationException ex2)
						{
							superUserUseLicenseAsyncResult.Context.LatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireLicense);
							RmsClientManager.TraceFail(null, superUserUseLicenseAsyncResult.Context.SystemProbeId, "Failed to read configuration when reading tenant's federated mailbox. Error {0}", new object[]
							{
								ex2
							});
							value = ex2;
							return;
						}
						finally
						{
							superUserUseLicenseAsyncResult.InvokeCallback(value);
							superUserUseLicenseAsyncResult.ReleaseWebManager();
						}
					}
					superUserUseLicenseAsyncResult.Manager = new LicenseWSManager(superUserUseLicenseAsyncResult.LicenseUri, RmsClientManager.perfCounters, superUserUseLicenseAsyncResult.LatencyTracker, RmsClientManagerUtils.GetLocalServerProxy(RmsClientManager.UseOfflineRms), RmsClientManager.AppSettings.RmsSoapQueriesTimeout);
					RmsClientManager.TracePass(null, superUserUseLicenseAsyncResult.Context.SystemProbeId, "Issuing a request for server use license", new object[0]);
					superUserUseLicenseAsyncResult.AddBreadCrumb(Constants.State.BeginAcquireUseLicense);
					superUserUseLicenseAsyncResult.Manager.BeginAcquireUseLicense(disposableTenantLicensePair.Rac, superUserUseLicenseAsyncResult.IssuanceLicense, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireUseLicenseCallback)), superUserUseLicenseAsyncResult);
				}
			}
			catch (RightsManagementException ex3)
			{
				RmsClientManager.TraceFail(null, superUserUseLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquireTenantLicenseCallback {0}", new object[]
				{
					ex3
				});
				superUserUseLicenseAsyncResult.InvokeCallback(ex3);
				superUserUseLicenseAsyncResult.ReleaseWebManager();
			}
			catch (ExchangeConfigurationException ex4)
			{
				RmsClientManager.TraceFail(null, superUserUseLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquireTenantLicenseCallback {0}", new object[]
				{
					ex4
				});
				superUserUseLicenseAsyncResult.InvokeCallback(ex4);
				superUserUseLicenseAsyncResult.ReleaseWebManager();
			}
		}

		private static void AcquireUseLicenseCallback(IAsyncResult asyncResult)
		{
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			SuperUserUseLicenseAsyncResult superUserUseLicenseAsyncResult = asyncResult.AsyncState as SuperUserUseLicenseAsyncResult;
			if (superUserUseLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of SuperUserUseLicenseAsyncResult.");
			}
			superUserUseLicenseAsyncResult.AddBreadCrumb(Constants.State.AcquireUseLicenseCallback);
			Exception value = null;
			try
			{
				superUserUseLicenseAsyncResult.UseLicense = superUserUseLicenseAsyncResult.Manager.EndAcquireUseLicense(asyncResult);
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, superUserUseLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquireUseLicenseCallback {0}", new object[]
				{
					ex
				});
				if (ex.FailureCode == RightsManagementFailureCode.UserRightNotGranted)
				{
					ex.FailureCode = RightsManagementFailureCode.ServerRightNotGranted;
					ex.IsPermanent = false;
				}
				value = ex;
			}
			finally
			{
				superUserUseLicenseAsyncResult.ReleaseWebManager();
			}
			superUserUseLicenseAsyncResult.InvokeCallback(value);
		}

		private static void AcquireUseLicenseAndUsageRightsCallbackForUseLicense(IAsyncResult asyncResultForUseLicense)
		{
			RmsClientManager.Tracer.TraceDebug(0L, "AcquireUseLicenseAndUsageRightsCallbackForUseLicense invoked");
			if (asyncResultForUseLicense == null)
			{
				throw new InvalidOperationException("asyncResultForUseLicense must NOT be null.");
			}
			if (asyncResultForUseLicense.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResultForUseLicense.AsyncState must NOT be null.");
			}
			UseLicenseAndUsageRightsAsyncResult useLicenseAndUsageRightsAsyncResult = asyncResultForUseLicense.AsyncState as UseLicenseAndUsageRightsAsyncResult;
			if (useLicenseAndUsageRightsAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResultForUseLicense.AsyncState MUST be type of UseLicenseAndUsageRightsAsyncResult.");
			}
			useLicenseAndUsageRightsAsyncResult.AddBreadCrumb(Constants.State.AcquireUseLicenseAndUsageRightsCallbackForUseLicense);
			OrganizationId orgId = useLicenseAndUsageRightsAsyncResult.Context.OrgId;
			Uri licensingUri = useLicenseAndUsageRightsAsyncResult.LicensingUri;
			LicenseResponse licenseResponse;
			bool flag;
			try
			{
				if (RmsClientManager.UseOfflineRms)
				{
					licenseResponse = RmsClientManager.EndAcquirePreLicense(asyncResultForUseLicense)[0];
				}
				else
				{
					licenseResponse = RmsClientManager.EndAcquireUseLicense(asyncResultForUseLicense);
				}
				flag = RmsClientManager.IsInternalRMSLicensingUri(orgId, licensingUri);
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, useLicenseAndUsageRightsAsyncResult.Context.SystemProbeId, "Caught exception in AcquireUseLicenseAndUsageRightsCallbackForUseLicense.  Exception: {0}", new object[]
				{
					ex
				});
				useLicenseAndUsageRightsAsyncResult.InvokeCallback(ex);
				return;
			}
			catch (ExchangeConfigurationException ex2)
			{
				RmsClientManager.TraceFail(null, useLicenseAndUsageRightsAsyncResult.Context.SystemProbeId, "Caught exception in AcquireUseLicenseAndUsageRightsCallbackForUseLicense.  Exception: {0}", new object[]
				{
					ex2
				});
				useLicenseAndUsageRightsAsyncResult.InvokeCallback(ex2);
				return;
			}
			useLicenseAndUsageRightsAsyncResult.UseLicense = licenseResponse.License;
			if (flag && !RmsClientManager.UseOfflineRms)
			{
				if (useLicenseAndUsageRightsAsyncResult.IsSuperUser)
				{
					useLicenseAndUsageRightsAsyncResult.UsageRights = ContentRight.Owner;
					useLicenseAndUsageRightsAsyncResult.ExpiryTime = RmsClientManagerUtils.GetUseLicenseExpiryTime(licenseResponse);
					useLicenseAndUsageRightsAsyncResult.DRMPropsSignature = Constants.EmptyByteArray;
					useLicenseAndUsageRightsAsyncResult.InvokeCallback();
					return;
				}
				XmlNode[] publishLicenseAsXmlNodes = useLicenseAndUsageRightsAsyncResult.PublishLicenseAsXmlNodes;
				string userIdentity = useLicenseAndUsageRightsAsyncResult.UserIdentity;
				RmsClientManager.BeginAcquirePreLicense(useLicenseAndUsageRightsAsyncResult.Context, licensingUri, true, publishLicenseAsXmlNodes, new string[]
				{
					userIdentity
				}, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireUseLicenseAndUsageRightsCallbackForPreLicense)), useLicenseAndUsageRightsAsyncResult);
			}
			else
			{
				if (licenseResponse.UsageRights == null)
				{
					useLicenseAndUsageRightsAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.UseLicenseAcquisitionFailed, ServerStrings.QueryUsageRightsPrelicenseResponseFailedToExtractRights(useLicenseAndUsageRightsAsyncResult.LicensingUri.AbsoluteUri)));
					return;
				}
				useLicenseAndUsageRightsAsyncResult.UsageRights = licenseResponse.UsageRights.Value;
				useLicenseAndUsageRightsAsyncResult.ExpiryTime = RmsClientManagerUtils.GetUseLicenseExpiryTime(licenseResponse);
				useLicenseAndUsageRightsAsyncResult.DRMPropsSignature = Constants.EmptyByteArray;
				useLicenseAndUsageRightsAsyncResult.InvokeCallback();
				return;
			}
		}

		private static void AcquireUseLicenseAndUsageRightsCallbackForPreLicense(IAsyncResult asyncResultForPreLicense)
		{
			RmsClientManager.Tracer.TraceDebug(0L, "AcquireUseLicenseAndUsageRightsCallbackForPreLicense invoked");
			if (asyncResultForPreLicense == null)
			{
				throw new InvalidOperationException("asyncResultForPreLicense must NOT be null.");
			}
			if (asyncResultForPreLicense.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResultForPreLicense.AsyncState must NOT be null.");
			}
			UseLicenseAndUsageRightsAsyncResult useLicenseAndUsageRightsAsyncResult = asyncResultForPreLicense.AsyncState as UseLicenseAndUsageRightsAsyncResult;
			if (useLicenseAndUsageRightsAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResultForBeginAcquireUseLicense.AsyncState MUST be type of UseLicenseAndUsageRightsAsyncResult.");
			}
			useLicenseAndUsageRightsAsyncResult.AddBreadCrumb(Constants.State.AcquireUseLicenseAndUsageRightsCallbackForPreLicense);
			LicenseResponse[] array;
			try
			{
				array = RmsClientManager.EndAcquirePreLicense(asyncResultForPreLicense);
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, useLicenseAndUsageRightsAsyncResult.Context.SystemProbeId, "Caught exception in AcquireUseLicenseAndUsageRightsCallbackForPreLicense at EndAcquirePreLicense.  Exception: {0}", new object[]
				{
					ex
				});
				useLicenseAndUsageRightsAsyncResult.InvokeCallback(ex);
				return;
			}
			catch (ExchangeConfigurationException ex2)
			{
				RmsClientManager.TraceFail(null, useLicenseAndUsageRightsAsyncResult.Context.SystemProbeId, "Caught exception in AcquireUseLicenseAndUsageRightsCallbackForPreLicense at EndAcquirePreLicense.  Exception: {0}", new object[]
				{
					ex2
				});
				useLicenseAndUsageRightsAsyncResult.InvokeCallback(ex2);
				return;
			}
			if (array == null || array.Length != 1)
			{
				useLicenseAndUsageRightsAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.PreLicenseAcquisitionFailed, ServerStrings.QueryUsageRightsNoPrelicenseResponse(useLicenseAndUsageRightsAsyncResult.LicensingUri.AbsoluteUri)));
			}
			else
			{
				LicenseResponse licenseResponse = array[0];
				if (licenseResponse.Exception != null)
				{
					useLicenseAndUsageRightsAsyncResult.InvokeCallback(new RightsManagementException(licenseResponse.Exception.FailureCode, ServerStrings.QueryUsageRightsPrelicenseResponseHasFailure(useLicenseAndUsageRightsAsyncResult.LicensingUri.AbsoluteUri, licenseResponse.Exception.FailureCode)));
					return;
				}
				if (licenseResponse.UsageRights == null)
				{
					useLicenseAndUsageRightsAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.PreLicenseAcquisitionFailed, ServerStrings.QueryUsageRightsPrelicenseResponseFailedToExtractRights(useLicenseAndUsageRightsAsyncResult.LicensingUri.AbsoluteUri)));
					return;
				}
				useLicenseAndUsageRightsAsyncResult.UsageRights = licenseResponse.UsageRights.Value;
				try
				{
					useLicenseAndUsageRightsAsyncResult.ExpiryTime = RmsClientManagerUtils.GetUseLicenseExpiryTime(licenseResponse);
					using (DisposableTenantLicensePair disposableTenantLicensePair = RmsClientManager.AcquireTenantLicenses(useLicenseAndUsageRightsAsyncResult.Context, useLicenseAndUsageRightsAsyncResult.LicensingUri))
					{
						using (RightsSignatureBuilder rightsSignatureBuilder = new RightsSignatureBuilder(useLicenseAndUsageRightsAsyncResult.UseLicense, useLicenseAndUsageRightsAsyncResult.PublishLicense, RmsClientManager.EnvironmentHandle, disposableTenantLicensePair))
						{
							useLicenseAndUsageRightsAsyncResult.DRMPropsSignature = rightsSignatureBuilder.Sign(useLicenseAndUsageRightsAsyncResult.UsageRights, useLicenseAndUsageRightsAsyncResult.ExpiryTime, useLicenseAndUsageRightsAsyncResult.UserSid);
						}
						useLicenseAndUsageRightsAsyncResult.InvokeCallback();
					}
				}
				catch (RightsManagementException ex3)
				{
					RmsClientManager.TraceFail(null, useLicenseAndUsageRightsAsyncResult.Context.SystemProbeId, "Caught exception in AcquireUseLicenseAndUsageRightsCallbackForPreLicense at computing rights signature.  Exception: {0}", new object[]
					{
						ex3
					});
					useLicenseAndUsageRightsAsyncResult.InvokeCallback(ex3);
					return;
				}
				catch (ExchangeConfigurationException ex4)
				{
					RmsClientManager.TraceFail(null, useLicenseAndUsageRightsAsyncResult.Context.SystemProbeId, "Caught exception in AcquireUseLicenseAndUsageRightsCallbackForPreLicense at computing rights signature.  Exception: {0}", new object[]
					{
						ex4
					});
					useLicenseAndUsageRightsAsyncResult.InvokeCallback(ex4);
					return;
				}
				return;
			}
		}

		private static void AcquireExternalRMSInfoCertificationCallback(IAsyncResult asyncResult)
		{
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			FederationRacAsyncResult federationRacAsyncResult = asyncResult.AsyncState as FederationRacAsyncResult;
			if (federationRacAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of FederationRacAsyncResult.");
			}
			federationRacAsyncResult.AddBreadCrumb(Constants.State.AcquireExternalRMSInfoCertificationCallback);
			Exception ex = null;
			LicenseIdentity federatedLicenseIdentity;
			try
			{
				federationRacAsyncResult.ServerInfo = RmsServerInfoManager.EndAcquireServerInfo(asyncResult);
				federatedLicenseIdentity = RmsClientManagerUtils.GetFederatedLicenseIdentity(federationRacAsyncResult.Context.OrgId);
				if (federationRacAsyncResult.ServerInfo == null)
				{
					ex = new RightsManagementException(RightsManagementFailureCode.GetServerInfoFailed, ServerStrings.FailedToFindServerInfo(federationRacAsyncResult.LicenseUri), federationRacAsyncResult.LicenseUri.ToString());
					return;
				}
			}
			catch (ExchangeConfigurationException ex2)
			{
				ex = ex2;
				return;
			}
			catch (RightsManagementException ex3)
			{
				ex = ex3;
				return;
			}
			finally
			{
				if (ex != null)
				{
					RmsClientManager.TraceFail(null, federationRacAsyncResult.Context.SystemProbeId, "Hit an exception when talking to the external RMS server pipeline {0} for certify request. Error {1}", new object[]
					{
						(federationRacAsyncResult.ServerInfo != null) ? federationRacAsyncResult.ServerInfo.CertificationWSPipeline : null,
						ex
					});
					federationRacAsyncResult.ReleaseWebManager();
					federationRacAsyncResult.InvokeCallback(ex);
				}
			}
			EndpointAddress epa = new EndpointAddress(federationRacAsyncResult.ServerInfo.CertificationWSPipeline.ToString());
			federationRacAsyncResult.Manager = new ServerCertificationWCFManager(RmsClientManagerUtils.CreateCertificationChannelFactory(federationRacAsyncResult.ServerInfo.CertificationWSTargetUri, epa, federatedLicenseIdentity, federationRacAsyncResult.Context.OrgId, federationRacAsyncResult.LatencyTracker), federationRacAsyncResult.ServerInfo.CertificationWSTargetUri, RmsClientManager.perfCounters, federationRacAsyncResult.LatencyTracker);
			federationRacAsyncResult.AddBreadCrumb(Constants.State.WCFBeginCertify);
			federationRacAsyncResult.Manager.BeginAcquireRac(RmsClientManager.GetMachineCertificateChain(federationRacAsyncResult.Context), RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireTenantExternalRacCallback)), federationRacAsyncResult);
		}

		private static void AcquireTenantExternalRacCallback(IAsyncResult asyncResult)
		{
			RmsClientManager.Tracer.TraceDebug(0L, "AcquireTenantExternalRacCallback called");
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			FederationRacAsyncResult federationRacAsyncResult = asyncResult.AsyncState as FederationRacAsyncResult;
			if (federationRacAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of FederationRacAsyncResult.");
			}
			federationRacAsyncResult.AddBreadCrumb(Constants.State.AcquireTenantExternalRacCallback);
			ServerCertificationWCFManager manager = federationRacAsyncResult.Manager;
			if (manager == null)
			{
				throw new InvalidOperationException("result.Manager cannot be null");
			}
			Exception value = null;
			try
			{
				RmsClientManager.licenseStoreManager.WriteToStore(federationRacAsyncResult.Context.TenantId, federationRacAsyncResult.LicenseUri, manager.EndAcquireRac(asyncResult), RmsClientManager.IRMConfig.GetTenantServerCertificatesVersion(federationRacAsyncResult.Context.OrgId));
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, federationRacAsyncResult.Context.SystemProbeId, "Hit an exception when talking to the external RMS server pipeline {0} for Certification request. Error {1}", new object[]
				{
					federationRacAsyncResult.LicenseUri,
					ex
				});
				if (RmsClientManagerUtils.ShouldMarkWCFErrorAsNegative(ex.FailureCode))
				{
					RmsServerInfoManager.AddNegativeServerInfo(federationRacAsyncResult.LicenseUri);
				}
				value = ex;
			}
			finally
			{
				federationRacAsyncResult.ReleaseWebManager();
			}
			federationRacAsyncResult.InvokeCallback(value);
		}

		private static void AcquireFederationRacCallback(IAsyncResult asyncResult)
		{
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			FederationServerLicenseAsyncResult federationServerLicenseAsyncResult = asyncResult.AsyncState as FederationServerLicenseAsyncResult;
			if (federationServerLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of FederationServerLicenseAsyncResult.");
			}
			federationServerLicenseAsyncResult.AddBreadCrumb(Constants.State.AcquireFederationRacCallback);
			DisposableTenantLicensePair disposableTenantLicensePair = null;
			try
			{
				try
				{
					disposableTenantLicensePair = RmsClientManager.EndAcquireFederationRAC(asyncResult);
				}
				catch (RightsManagementException ex)
				{
					RmsClientManager.TraceFail(null, federationServerLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquireFederationRacCallback {0}", new object[]
					{
						ex
					});
					federationServerLicenseAsyncResult.InvokeCallback(ex);
					return;
				}
				catch (ExchangeConfigurationException ex2)
				{
					RmsClientManager.TraceFail(null, federationServerLicenseAsyncResult.Context.SystemProbeId, "Hit an exception during AcquireFederationRacCallback {0}", new object[]
					{
						ex2
					});
					federationServerLicenseAsyncResult.InvokeCallback(ex2);
					return;
				}
				if (disposableTenantLicensePair == null || disposableTenantLicensePair.Rac == null || disposableTenantLicensePair.Rac[0] == null || string.IsNullOrEmpty(disposableTenantLicensePair.Rac[0].OuterXml))
				{
					federationServerLicenseAsyncResult.InvokeCallback(new RightsManagementException(RightsManagementFailureCode.RacAcquisitionFailed, ServerStrings.FailedToAcquireFederationRac(federationServerLicenseAsyncResult.Context.OrgId.ToString(), federationServerLicenseAsyncResult.LicenseUri))
					{
						IsPermanent = false
					});
					RmsClientManager.TraceFail(null, federationServerLicenseAsyncResult.Context.SystemProbeId, "Failed to find tenant {0} RAC from Uri {1}", new object[]
					{
						federationServerLicenseAsyncResult.Context.OrgId,
						federationServerLicenseAsyncResult.LicenseUri
					});
				}
				else
				{
					federationServerLicenseAsyncResult.Rac = new XrmlCertificateChain();
					federationServerLicenseAsyncResult.Rac.CertificateChain = new string[]
					{
						disposableTenantLicensePair.Rac[0].OuterXml
					};
					RmsServerInfoManager.BeginAcquireServerInfo(federationServerLicenseAsyncResult.Context, federationServerLicenseAsyncResult.LicenseUri, federationServerLicenseAsyncResult, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireExternalRMSInfoLicensingCallback)));
				}
			}
			finally
			{
				if (disposableTenantLicensePair != null)
				{
					disposableTenantLicensePair.Dispose();
				}
			}
		}

		private static void AcquireExternalRMSInfoLicensingCallback(IAsyncResult asyncResult)
		{
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			FederationServerLicenseAsyncResult federationServerLicenseAsyncResult = asyncResult.AsyncState as FederationServerLicenseAsyncResult;
			if (federationServerLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of FederationServerLicenseAsyncResult.");
			}
			federationServerLicenseAsyncResult.AddBreadCrumb(Constants.State.AcquireExternalRMSInfoLicensingCallback);
			Exception ex = null;
			LicenseIdentity federatedLicenseIdentity;
			try
			{
				federationServerLicenseAsyncResult.ServerInfo = RmsServerInfoManager.EndAcquireServerInfo(asyncResult);
				federatedLicenseIdentity = RmsClientManagerUtils.GetFederatedLicenseIdentity(federationServerLicenseAsyncResult.Context.OrgId);
				if (federationServerLicenseAsyncResult.ServerInfo == null)
				{
					ex = new RightsManagementException(RightsManagementFailureCode.GetServerInfoFailed, ServerStrings.FailedToFindServerInfo(federationServerLicenseAsyncResult.LicenseUri), federationServerLicenseAsyncResult.LicenseUri.ToString());
					return;
				}
			}
			catch (ExchangeConfigurationException ex2)
			{
				ex = ex2;
				return;
			}
			catch (RightsManagementException ex3)
			{
				ex = ex3;
				return;
			}
			finally
			{
				if (ex != null)
				{
					RmsClientManager.TraceFail(null, federationServerLicenseAsyncResult.Context.SystemProbeId, "Hit an exception when talking to the external RMS server pipeline {0} for AcquireLicense request. Error {1}", new object[]
					{
						(federationServerLicenseAsyncResult.ServerInfo != null) ? federationServerLicenseAsyncResult.ServerInfo.ServerLicensingWSPipeline : null,
						ex
					});
					federationServerLicenseAsyncResult.ReleaseWebManager();
					federationServerLicenseAsyncResult.InvokeCallback(ex);
				}
			}
			EndpointAddress epa = new EndpointAddress(federationServerLicenseAsyncResult.ServerInfo.ServerLicensingWSPipeline.ToString());
			federationServerLicenseAsyncResult.Manager = new ServerLicenseWCFManager(RmsClientManagerUtils.CreateServerLicensingChannelFactory(federationServerLicenseAsyncResult.ServerInfo.ServerLicensingWSTargetUri, epa, federatedLicenseIdentity, federationServerLicenseAsyncResult.Context.OrgId, federationServerLicenseAsyncResult.Identities.Length, federationServerLicenseAsyncResult.LatencyTracker), federationServerLicenseAsyncResult.ServerInfo.ServerLicensingWSTargetUri, federationServerLicenseAsyncResult.Rac, RmsClientManager.perfCounters, federationServerLicenseAsyncResult.LatencyTracker);
			federationServerLicenseAsyncResult.AddBreadCrumb(Constants.State.WCFBeginAcquireLicense);
			federationServerLicenseAsyncResult.Manager.BeginAcquireLicense(federationServerLicenseAsyncResult.IssuanceLicense, federationServerLicenseAsyncResult.Identities, RmsClientManagerUtils.WrapCallbackWithUnhandledExceptionHandlerAndUpdatePoisonContext(new AsyncCallback(RmsClientManager.AcquireFederationLicenseCallback)), federationServerLicenseAsyncResult);
		}

		private static void AcquireFederationLicenseCallback(IAsyncResult asyncResult)
		{
			RmsClientManager.Tracer.TraceDebug(0L, "AcquireFederationLicenseCallback called");
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or the AsynState cannot be null here.");
			}
			FederationServerLicenseAsyncResult federationServerLicenseAsyncResult = asyncResult.AsyncState as FederationServerLicenseAsyncResult;
			if (federationServerLicenseAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of FederationServerLicenseAsyncResult.");
			}
			federationServerLicenseAsyncResult.AddBreadCrumb(Constants.State.AcquireFederationLicenseCallback);
			ServerLicenseWCFManager manager = federationServerLicenseAsyncResult.Manager;
			if (manager == null)
			{
				throw new InvalidOperationException("result.Manager cannot be null");
			}
			Exception value = null;
			try
			{
				LicenseResponse[] array = manager.EndAcquireLicense(asyncResult);
				if (federationServerLicenseAsyncResult.Responses == null)
				{
					federationServerLicenseAsyncResult.Responses = array;
				}
				else
				{
					int num = 0;
					for (int i = 0; i < federationServerLicenseAsyncResult.Responses.Length; i++)
					{
						if (federationServerLicenseAsyncResult.Responses[i] == null)
						{
							federationServerLicenseAsyncResult.Responses[i] = array[num++];
						}
					}
					if (num != array.Length)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "ServerLicenseWCFManager returned {0} responses, while {1} requests were made", new object[]
						{
							array.Length,
							federationServerLicenseAsyncResult.Responses.Length
						}));
					}
				}
			}
			catch (RightsManagementException ex)
			{
				RmsClientManager.TraceFail(null, federationServerLicenseAsyncResult.Context.SystemProbeId, "Hit an exception when talking to the external RMS server pipeline {0} for AcquireServerLicense request. Error {1}", new object[]
				{
					federationServerLicenseAsyncResult.LicenseUri,
					ex
				});
				if (RmsClientManagerUtils.ShouldMarkWCFErrorAsNegative(ex.FailureCode))
				{
					RmsServerInfoManager.AddNegativeServerInfo(federationServerLicenseAsyncResult.LicenseUri);
				}
				value = ex;
			}
			finally
			{
				federationServerLicenseAsyncResult.ReleaseWebManager();
			}
			federationServerLicenseAsyncResult.InvokeCallback(value);
		}

		private static int GetMachineCertIndexFromMsDrmBitsAndRmsServerCryptoMode()
		{
			if (!DrmClientUtils.DoesMsDrmSupportMode2Crypto())
			{
				return 0;
			}
			int rmsServerActiveCryptoMode = RmsClientManager.GetRmsServerActiveCryptoMode();
			if (rmsServerActiveCryptoMode > 0)
			{
				return rmsServerActiveCryptoMode - 1;
			}
			return 0;
		}

		private static int GetRmsServerActiveCryptoMode()
		{
			if (RmsClientManager.UseOfflineRms)
			{
				throw new InvalidOperationException("There is no RMS server in the data center.");
			}
			int result = 1;
			if (RmsClientManager.ReadRmsServerCryptoModeFromRegistry(out result))
			{
				return result;
			}
			Uri uri = RmsClientManager.GetOnPremiseRmsServerUri();
			uri = RmsoProxyUtil.GetCertificationServerRedirectUrl(uri);
			if (null == uri)
			{
				return 1;
			}
			RmsClientManagerLog.LogDrmInitialization(uri);
			int serviceCryptoMode;
			using (ServerWSManager serverWSManager = new ServerWSManager(uri, ServiceType.CertificationService, null, null, RmsClientManagerUtils.GetLocalServerProxy(RmsClientManager.UseOfflineRms), RmsClientManager.AppSettings.RmsSoapQueriesTimeout))
			{
				serviceCryptoMode = serverWSManager.GetServiceCryptoMode();
			}
			return serviceCryptoMode;
		}

		private static Uri GetOnPremiseRmsServerUri()
		{
			Uri result;
			try
			{
				result = DrmClientUtils.GetServiceLocation(SafeRightsManagementSessionHandle.InvalidHandle, ServiceType.ClientLicensor, ServiceLocation.Enterprise, null);
			}
			catch (RightsManagementException arg)
			{
				RmsClientManager.Tracer.TraceError<RightsManagementException>(0L, "Caught a RightsManagementException in GetOnPremiseRmsServerUri: {0}", arg);
				result = null;
			}
			return result;
		}

		private static bool ReadRmsServerCryptoModeFromRegistry(out int cryptoModeFromRegistry)
		{
			cryptoModeFromRegistry = 1;
			try
			{
				object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\ExchangeServer", "RmsServerCryptoModeOverride", -1);
				if (value != null && value is int && (int)value != -1)
				{
					cryptoModeFromRegistry = (int)value;
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		[ThreadStatic]
		internal static RmsClientManager.SaveContextOnAsyncQueryCallback SaveContextCallback;

		public static Cache<Guid, RmsTemplate> TemplateCacheForFirstOrg;

		internal static byte FirstOrgTemplateCacheVersion;

		internal static int OnPremiseActiveMachineCertIndex = 0;

		internal static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private static readonly SystemProbeTrace SystemProbeTracer = new SystemProbeTrace(ExTraceGlobals.RightsManagementTracer, ExchangeComponent.Rms.Name);

		private static readonly PropertyDefinition[] FindBySidProperties = new PropertyDefinition[]
		{
			ADMailboxRecipientSchema.SidHistory
		};

		private static readonly PropertyDefinition[] FindByEmailAddressesProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses
		};

		private static readonly object initializationLock = new object();

		private static readonly PerTenantQueryController<Guid> outstandingPerTenantFindLicensesCalls = new PerTenantQueryController<Guid>(EqualityComparer<Guid>.Default);

		private static readonly PerTenantQueryController<Guid> outstandingPerTenantFindTemplatesCalls = new PerTenantQueryController<Guid>(EqualityComparer<Guid>.Default);

		private static bool useOfflineRms = false;

		private static bool initialized = false;

		private static RmsClientManager instance = new RmsClientManager();

		private static RmsPerformanceCounters perfCounters;

		private static RmsLicenseStoreManager licenseStoreManager;

		private static RmsClientManager.DrmEnvironment drmEnvironment;

		private static RmsConfiguration irmConfig;

		[ThreadStatic]
		private static IConfigurationSession adSession;

		[ThreadStatic]
		private static bool ignoreLicensingEnabled;

		private static int dataCenterTestHookMachineCertIndexOverride = -1;

		public delegate void SaveContextOnAsyncQueryCallback(object state);

		private sealed class DrmEnvironment
		{
			private DrmEnvironment(List<XmlNode[]> machineCertificatesChain, SafeRightsManagementHandle libraryHandle, SafeRightsManagementEnvironmentHandle environmentHandle)
			{
				if (machineCertificatesChain == null || machineCertificatesChain.Count == 0)
				{
					throw new ArgumentNullException("machineCertificatesChain");
				}
				this.MachineCertificatesChain = machineCertificatesChain;
				this.EnvironmentHandle = environmentHandle;
				this.LibraryHandle = libraryHandle;
			}

			public static RmsClientManager.DrmEnvironment InitializeEnvironment()
			{
				string text = null;
				Exception ex = null;
				try
				{
					text = DrmClientUtils.GetDefaultManifestFilePath();
				}
				catch (SecurityException ex2)
				{
					RmsClientManager.Tracer.TraceError<SecurityException>(0L, "SecurityException caught in GetDefaultManifestFilePath. Returning null. {0}", ex2);
					ex = ex2;
				}
				catch (IOException ex3)
				{
					RmsClientManager.Tracer.TraceError<IOException>(0L, "IOException caught in GetDefaultManifestFilePath. Returning null. {0}", ex3);
					ex = ex3;
				}
				if (ex != null || string.IsNullOrEmpty(text))
				{
					throw new RightsManagementException(RightsManagementFailureCode.EnvironmentCannotLoad, DrmStrings.FailedToReadManifestFileLocation, ex);
				}
				RmsClientManager.DrmEnvironment result;
				try
				{
					using (FileStream fileStream = File.OpenRead(text))
					{
						using (StreamReader streamReader = new StreamReader(fileStream))
						{
							DRMClientVersionInfo drmclientVersionInfo = new DRMClientVersionInfo();
							DrmClientUtils.VerifyDRMClientVersion(drmclientVersionInfo);
							RmsClientManagerLog.LogDrmInitialization(drmclientVersionInfo);
							DrmClientUtils.SetServerLockboxOptions();
							DrmClientUtils.ActivateMachine();
							List<string> list = DrmClientUtils.LoadAllMachineCertificates();
							List<XmlNode[]> list2 = DrmClientUtils.ConvertMachineCertsToXmlNodeArrays(list);
							if (list2.Count == 0)
							{
								throw new RightsManagementException(RightsManagementFailureCode.ActivationFailed, DrmStrings.FailedToInitializeRMSEnvironment);
							}
							if (!RmsClientManager.UseOfflineRms)
							{
								RmsClientManager.OnPremiseActiveMachineCertIndex = RmsClientManager.GetMachineCertIndexFromMsDrmBitsAndRmsServerCryptoMode();
								RmsClientManagerLog.LogDrmInitialization(RmsClientManager.OnPremiseActiveMachineCertIndex);
							}
							string securityProviderPath = DrmClientUtils.GetSecurityProviderPath();
							SafeRightsManagementEnvironmentHandle environmentHandle;
							SafeRightsManagementHandle libraryHandle;
							int hr = SafeNativeMethods.EnsureDRMEnvironmentInitialized(0U, 1U, securityProviderPath, streamReader.ReadToEnd(), list[0], out environmentHandle, out libraryHandle);
							Errors.ThrowOnErrorCode(hr);
							result = new RmsClientManager.DrmEnvironment(list2, libraryHandle, environmentHandle);
						}
					}
				}
				catch (IOException ex4)
				{
					RmsClientManager.Tracer.TraceError<string, IOException>(0L, "Failed to open the manifest file {0}, Error:{1}", text, ex4);
					throw new RightsManagementException(RightsManagementFailureCode.EnvironmentCannotLoad, DrmStrings.FailedToOpenManifestFile(text), ex4);
				}
				catch (UnauthorizedAccessException ex5)
				{
					RmsClientManager.Tracer.TraceError<string, UnauthorizedAccessException>(0L, "Failed to open the manifest file {0}, Error:{1}", text, ex5);
					throw new RightsManagementException(RightsManagementFailureCode.EnvironmentCannotLoad, DrmStrings.FailedToOpenManifestFile(text), ex5);
				}
				catch (ThreadAbortException ex6)
				{
					RmsClientManager.Tracer.TraceError<ThreadAbortException>(0L, "Failed to initialize DRM environment. Error:{0}", ex6);
					throw new RightsManagementException(RightsManagementFailureCode.EnvironmentCannotLoad, DrmStrings.FailedToInitializeRMSEnvironment, ex6);
				}
				return result;
			}

			public void CloseHandles()
			{
				if (this.LibraryHandle != null)
				{
					this.LibraryHandle.Close();
				}
				if (this.EnvironmentHandle != null)
				{
					this.EnvironmentHandle.Close();
				}
			}

			public readonly List<XmlNode[]> MachineCertificatesChain = new List<XmlNode[]>(2);

			public readonly SafeRightsManagementHandle LibraryHandle;

			public readonly SafeRightsManagementEnvironmentHandle EnvironmentHandle;
		}
	}
}
