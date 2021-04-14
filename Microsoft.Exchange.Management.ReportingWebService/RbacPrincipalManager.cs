using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ReportingWebService;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class RbacPrincipalManager
	{
		private RbacPrincipalManager()
		{
		}

		public static RbacPrincipalManager Instance
		{
			get
			{
				return RbacPrincipalManager.LazyInstance.Value;
			}
		}

		public RbacPrincipal AcquireRbacPrincipalWrapper(HttpContext httpContext)
		{
			RbacPrincipal rbacPrincipal = null;
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.RbacPrincipalAcquireLatency, delegate
			{
				rbacPrincipal = this.AcquireRbacPrincipal(httpContext);
			});
			return rbacPrincipal;
		}

		public RbacPrincipal AcquireRbacPrincipal(HttpContext httpContext)
		{
			IIdentity identity = httpContext.User.Identity;
			if (identity == null)
			{
				ExTraceGlobals.ReportingWebServiceTracer.TraceError((long)this.GetHashCode(), "[RbacPrincipalManager::AcquirePrincipal] Unable to create RbacPrincipal, since identity is null.");
				return null;
			}
			string tenantDomain = this.GetTenant(identity, httpContext.Request);
			string cacheKey = this.GetCacheKey(identity, tenantDomain);
			Lazy<RbacPrincipal> lazy = (Lazy<RbacPrincipal>)HttpRuntime.Cache[cacheKey];
			if (lazy != null)
			{
				ExTraceGlobals.ReportingWebServiceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RbacPrincipalManager::AcquirePrincipal] Get cached RbacPrincipal. Name: {0}, Key: {1}", identity.Name, cacheKey);
				return lazy.Value;
			}
			lock (this.syncRoot)
			{
				lazy = (Lazy<RbacPrincipal>)HttpRuntime.Cache[cacheKey];
				if (lazy == null)
				{
					ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "[RbacPrincipalManager::AcquirePrincipal] Create lazy initialized RbacPrincipal.");
					lazy = new Lazy<RbacPrincipal>(() => this.CreateRbacPrincipal(identity, tenantDomain, cacheKey, httpContext));
					ExTraceGlobals.ReportingWebServiceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RbacPrincipalManager::AcquirePrincipal] Add RbacPrincipal to cache. Name: {0}, Key: {1}", identity.Name, cacheKey);
					HttpRuntime.Cache.Insert(cacheKey, lazy, null, (DateTime)ExDateTime.UtcNow.Add(RbacSection.Instance.RbacPrincipalMaximumAge), Cache.NoSlidingExpiration, CacheItemPriority.High, new CacheItemRemovedCallback(this.RbacPrincipalRemovedCallback));
					this.rbacPrincipalCounters.Increment();
				}
				else
				{
					ExTraceGlobals.ReportingWebServiceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[RbacPrincipalManager::AcquirePrincipal] Get cached RbacPrincipal. Name: {0}, Key: {1}", identity.Name, cacheKey);
				}
			}
			return lazy.Value;
		}

		private SnapinSet GetSnapinSet()
		{
			if (!RbacPrincipalManager.snapinSetParsed)
			{
				string value = ConfigurationManager.AppSettings["ExchangeRunspaceConfigurationSnapinSet"];
				if (!string.IsNullOrEmpty(value))
				{
					Enum.TryParse<SnapinSet>(value, true, out RbacPrincipalManager.snapinSet);
				}
				RbacPrincipalManager.snapinSetParsed = true;
			}
			return RbacPrincipalManager.snapinSet;
		}

		private string GetCacheKey(IIdentity identity, string tenantDomain)
		{
			string result;
			if (DelegatedPrincipal.DelegatedAuthenticationType.Equals(identity.AuthenticationType, StringComparison.OrdinalIgnoreCase))
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0}-Delegated-{1}-{2}", new object[]
				{
					"ExchangeRws-RbacPrincipal",
					identity.GetSafeName(true),
					tenantDomain
				});
			}
			else if (!string.IsNullOrEmpty(tenantDomain))
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2}", new object[]
				{
					"ExchangeRws-RbacPrincipal",
					identity.GetSecurityIdentifier(),
					tenantDomain
				});
			}
			else
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[]
				{
					"ExchangeRws-RbacPrincipal",
					identity.GetSecurityIdentifier()
				});
			}
			return result;
		}

		private void RbacPrincipalRemovedCallback(string key, object value, CacheItemRemovedReason reason)
		{
			ExTraceGlobals.ReportingWebServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "[RbacPrincipalManager::RbacPrincipalRemovedCallback] RbacPrincipal removed from cache. Key: {0}", key);
			this.rbacPrincipalCounters.Decrement();
		}

		private RbacPrincipal CreateRbacPrincipal(IIdentity identity, string tenantDomain, string cacheKey, HttpContext httpContext)
		{
			ExTraceGlobals.ReportingWebServiceTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "[RbacPrincipalManager::CreateRbacPrincipal] Create RbacPrincipal. Identity: {0}; tenantDomain: {1}; cacheKey: '{2}'", identity.GetSafeName(true), tenantDomain ?? string.Empty, cacheKey);
			ExchangeRunspaceConfigurationSettings rbacSettings = null;
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.NewExchangeRunspaceConfigurationSettingsLatency, delegate
			{
				rbacSettings = new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.ReportingWebService, tenantDomain, ExchangeRunspaceConfigurationSettings.SerializationLevel.None);
			});
			ReportingSchema schema = ReportingSchema.GetCurrentReportingSchema(httpContext);
			try
			{
				RequestStatistics requestStatistics = HttpContext.Current.Items[RequestStatistics.RequestStatsKey] as RequestStatistics;
				if (requestStatistics != null)
				{
					requestStatistics.AddExtendedStatisticsDataPoint("AuthType", identity.AuthenticationType);
				}
				using (new AverageTimePerfCounter(RwsPerfCounters.AverageRbacPrincipalCreation, RwsPerfCounters.AverageRbacPrincipalCreationBase, true))
				{
					RwsExchangeRunspaceConfiguration rbacConfiguration = null;
					ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.NewRwsExchangeRunspaceConfigurationLatency, delegate
					{
						rbacConfiguration = RwsExchangeRunspaceConfiguration.NewInstance(identity, rbacSettings, schema.CmdletFilter, this.GetSnapinSet());
					});
					RbacPrincipal rbacPrincipal = null;
					ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.NewRbacPrincipalLatency, delegate
					{
						rbacPrincipal = new RbacPrincipal(rbacConfiguration, cacheKey);
					});
					return rbacPrincipal;
				}
			}
			catch (CmdletAccessDeniedException ex)
			{
				HttpRuntime.Cache.Remove(cacheKey);
				ExTraceGlobals.ReportingWebServiceTracer.TraceError<CmdletAccessDeniedException>((long)this.GetHashCode(), "[RbacPrincipalManager::CreateRbacPrincipal] CmdletAccessDeniedException: {0}", ex);
				ServiceDiagnostics.ThrowError(ReportingErrorCode.ErrorTenantNotInOrgScope, Strings.ErrorTenantNotInOrgScope(tenantDomain), ex);
			}
			catch (CannotResolveTenantNameException ex2)
			{
				HttpRuntime.Cache.Remove(cacheKey);
				ExTraceGlobals.ReportingWebServiceTracer.TraceError<CannotResolveTenantNameException>((long)this.GetHashCode(), "[RbacPrincipalManager::CreateRbacPrincipal] CannotResolveTenantNameException: {0}", ex2);
				ServiceDiagnostics.ThrowError(ReportingErrorCode.ErrorTenantNotResolved, Strings.ErrorTenantNotResolved(tenantDomain), ex2);
			}
			catch (ADTransientException ex3)
			{
				HttpRuntime.Cache.Remove(cacheKey);
				ExTraceGlobals.ReportingWebServiceTracer.TraceError<ADTransientException>((long)this.GetHashCode(), "[RbacPrincipalManager::CreateRbacPrincipal] ADTransientException: {0}", ex3);
				ServiceDiagnostics.ThrowError(ReportingErrorCode.ADTransientError, Strings.ADTransientError, ex3);
			}
			catch (DataSourceOperationException ex4)
			{
				HttpRuntime.Cache.Remove(cacheKey);
				ExTraceGlobals.ReportingWebServiceTracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "[RbacPrincipalManager::CreateRbacPrincipal] DataSourceOperationException: {0}", ex4);
				ServiceDiagnostics.ThrowError(ReportingErrorCode.ADOperationError, Strings.ADOperationError, ex4);
			}
			catch (TimeoutException ex5)
			{
				HttpRuntime.Cache.Remove(cacheKey);
				ExTraceGlobals.ReportingWebServiceTracer.TraceError<TimeoutException>((long)this.GetHashCode(), "[RbacPrincipalManager::CreateRbacPrincipal] TimeoutException: {0}", ex5);
				ServiceDiagnostics.ThrowError(ReportingErrorCode.CreateRunspaceConfigTimeoutError, Strings.CreateRunspaceConfigTimeoutError, ex5);
			}
			catch (Exception ex6)
			{
				HttpRuntime.Cache.Remove(cacheKey);
				ExTraceGlobals.ReportingWebServiceTracer.TraceError<Exception>((long)this.GetHashCode(), "[RbacPrincipalManager::CreateRbacPrincipal] Exception: {0}", ex6);
				ServiceDiagnostics.ThrowError(ReportingErrorCode.UnknownError, Strings.UnknownError, ex6);
			}
			return null;
		}

		private string GetTenant(IIdentity identity, HttpRequest httpRequest)
		{
			if (identity == null)
			{
				return null;
			}
			SidOAuthIdentity sidOAuthIdentity = identity as SidOAuthIdentity;
			string result;
			if (sidOAuthIdentity != null)
			{
				ExTraceGlobals.ReportingWebServiceTracer.TraceDebug(0, 0L, "[RbacPrincipalManager::GetTenant] Return SidOAuthIdentity.ManagedTenantName.");
				result = sidOAuthIdentity.ManagedTenantName;
			}
			else
			{
				ExTraceGlobals.ReportingWebServiceTracer.TraceDebug(0, 0L, "[RbacPrincipalManager::GetTenant] Not SidOAuthIdentity.");
				result = this.GetTenantDomain(httpRequest);
			}
			return result;
		}

		private string GetTenantDomain(HttpRequest httpRequest)
		{
			string text = null;
			if (httpRequest.Url.Segments.Any((string segment) => segment.Equals("partner/", StringComparison.OrdinalIgnoreCase)))
			{
				text = httpRequest.QueryString["tenantDomain"];
				if (string.IsNullOrEmpty(text))
				{
					ExTraceGlobals.ReportingWebServiceTracer.TraceError(0L, "[RbacPrincipalManager::GetTenantDomain] Get tenant domain from query string failed.");
					ServiceDiagnostics.ThrowError(ReportingErrorCode.ErrorMissingTenantDomainInRequest, Strings.ErrorMissingTenantDomainInRequest);
				}
			}
			return text;
		}

		private const string RbacPrincipalCachePrefix = "ExchangeRws-RbacPrincipal";

		private const string PartnerSegmentName = "partner/";

		private const string TenantParameterName = "tenantDomain";

		private static readonly Lazy<RbacPrincipalManager> LazyInstance = new Lazy<RbacPrincipalManager>(() => new RbacPrincipalManager());

		private static SnapinSet snapinSet = SnapinSet.Admin;

		private static bool snapinSetParsed = false;

		private readonly PerfCounterGroup rbacPrincipalCounters = new PerfCounterGroup(RwsPerfCounters.RbacPrincipals, RwsPerfCounters.RbacPrincipalsPeak, RwsPerfCounters.RbacPrincipalsTotal);

		private readonly object syncRoot = new object();
	}
}
