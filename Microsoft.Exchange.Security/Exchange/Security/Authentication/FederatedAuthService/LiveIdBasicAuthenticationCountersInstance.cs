using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal sealed class LiveIdBasicAuthenticationCountersInstance : PerformanceCounterInstance
	{
		internal LiveIdBasicAuthenticationCountersInstance(string instanceName, LiveIdBasicAuthenticationCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange LiveIdBasicAuthentication")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfCurrentRequests = new ExPerformanceCounter(base.CategoryName, "Current Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfCurrentRequests);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfAuthRequests = new ExPerformanceCounter(base.CategoryName, "Total Auth Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfAuthRequests);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Successful Auth/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.NumberOfSuccessfulAuthentications = new ExPerformanceCounter(base.CategoryName, "Successful Auth Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.NumberOfSuccessfulAuthentications);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Invalid Credentials/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.NumberOfInvalidCredentials = new ExPerformanceCounter(base.CategoryName, "Invalid Credentials Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfInvalidCredentials);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Failed Auth/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.NumberOfFailedAuthentications = new ExPerformanceCounter(base.CategoryName, "Failed Auth Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.NumberOfFailedAuthentications);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Failed User Recoverable Auth/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.NumberOfFailedRecoverableAuthentications = new ExPerformanceCounter(base.CategoryName, "Failed User Recoverable Auth Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.NumberOfFailedRecoverableAuthentications);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Failed Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.NumberOfFailedRequests = new ExPerformanceCounter(base.CategoryName, "Failed Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.NumberOfFailedRequests);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Timed Out Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.NumberOfTimedOutRequests = new ExPerformanceCounter(base.CategoryName, "Timed Out Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter7
				});
				list.Add(this.NumberOfTimedOutRequests);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Application Password Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				this.NumberOfAppPasswords = new ExPerformanceCounter(base.CategoryName, "Application Password Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter8
				});
				list.Add(this.NumberOfAppPasswords);
				this.AverageResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Auth Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTime);
				this.AverageResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Auth Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTimeBase);
				this.NumberOfRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "Requests Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfRequestsLastMinute);
				this.NumberOfOrgIdRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "OrgId Requests Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOrgIdRequestsLastMinute);
				this.PercentageFailedAuthenticationsLastMinute = new ExPerformanceCounter(base.CategoryName, "Failed Authentications Percentage Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageFailedAuthenticationsLastMinute);
				this.PercentageFailedRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "Failed Requests Percentage Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageFailedRequestsLastMinute);
				this.PercentageTimedOutRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "Timed Out Requests Percentage Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageTimedOutRequestsLastMinute);
				this.NumberOfCachedRequests = new ExPerformanceCounter(base.CategoryName, "Cached Auth Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfCachedRequests);
				this.LogonCacheHit = new ExPerformanceCounter(base.CategoryName, "Logon Cache hit percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LogonCacheHit);
				this.LogonCacheSize = new ExPerformanceCounter(base.CategoryName, "Logon Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LogonCacheSize);
				this.InvalidCredCacheSize = new ExPerformanceCounter(base.CategoryName, "Invalid Cred Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.InvalidCredCacheSize);
				this.AdUserCacheHit = new ExPerformanceCounter(base.CategoryName, "User Entry Cache Hit Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdUserCacheHit);
				this.AdUserCacheSize = new ExPerformanceCounter(base.CategoryName, "User Entry Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdUserCacheSize);
				this.AverageAdResponseTime = new ExPerformanceCounter(base.CategoryName, "AD Average Auth Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAdResponseTime);
				this.AverageAdResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for AverageADResponseTime", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAdResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "AD Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.NumberOfAdRequests = new ExPerformanceCounter(base.CategoryName, "AD Auth Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter9
				});
				list.Add(this.NumberOfAdRequests);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "AD Access Requests/sec for OfflineOrgId", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				this.NumberOfAdRequestForOfflineOrgId = new ExPerformanceCounter(base.CategoryName, "AD Access Requests for OfflineOrgId", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter10
				});
				list.Add(this.NumberOfAdRequestForOfflineOrgId);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "AD Failed Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				this.FailedAdRequests = new ExPerformanceCounter(base.CategoryName, "AD Failed Auth Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter11
				});
				list.Add(this.FailedAdRequests);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "AD Password Synchronizations/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.AdPasswordSyncs = new ExPerformanceCounter(base.CategoryName, "AD Password Synchronizations", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter12
				});
				list.Add(this.AdPasswordSyncs);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "AD UPN Synchronizations/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.AdUpnSyncs = new ExPerformanceCounter(base.CategoryName, "AD UPN Synchronizations", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter13
				});
				list.Add(this.AdUpnSyncs);
				this.AverageHrdResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Windows Live Home Realm Discovery Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageHrdResponseTime);
				this.AverageHrdResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for AverageHrdResponseTimeAverage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageHrdResponseTimeBase);
				this.HrdCacheHit = new ExPerformanceCounter(base.CategoryName, "Cache hit percentage - Home Realm Discovery", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.HrdCacheHit);
				this.HrdCacheHitBase = new ExPerformanceCounter(base.CategoryName, "Base for HrdCacheHit", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.HrdCacheHitBase);
				this.HrdCacheSize = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Domain Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.HrdCacheSize);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.NumberOfOutgoingHrdRequests = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter14
				});
				list.Add(this.NumberOfOutgoingHrdRequests);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Requests/sec to AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.NumberOfADHrdRequests = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Requests Total to AD", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter15
				});
				list.Add(this.NumberOfADHrdRequests);
				ExPerformanceCounter exPerformanceCounter16 = new ExPerformanceCounter(base.CategoryName, "Failed Home Realm Discovery Requests/sec to AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter16);
				this.NumberOfFailedADHrdRequests = new ExPerformanceCounter(base.CategoryName, "Failed Home Realm Discovery Requests Total to AD", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter16
				});
				list.Add(this.NumberOfFailedADHrdRequests);
				ExPerformanceCounter exPerformanceCounter17 = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Record Update per second in AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter17);
				this.NumberOfADHrdUpdate = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Record Update Count in AD", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter17
				});
				list.Add(this.NumberOfADHrdUpdate);
				this.PendingHrdRequests = new ExPerformanceCounter(base.CategoryName, "Current Home Realm Discovery Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingHrdRequests);
				this.AverageLiveIdResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Windows Live Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLiveIdResponseTime);
				this.AverageLiveIdResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Windows Live Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLiveIdResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter18 = new ExPerformanceCounter(base.CategoryName, "LiveID STS Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter18);
				this.NumberOfLiveIdStsRequests = new ExPerformanceCounter(base.CategoryName, "LiveID STS Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter18
				});
				list.Add(this.NumberOfLiveIdStsRequests);
				this.PendingStsRequests = new ExPerformanceCounter(base.CategoryName, "Current LiveID STS Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingStsRequests);
				this.AverageMsoIdResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Microsoft Online Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMsoIdResponseTime);
				this.AverageMsoIdResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Microsoft Online Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMsoIdResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter19 = new ExPerformanceCounter(base.CategoryName, "Microsoft Online STS Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter19);
				this.NumberOfMsoIdStsRequests = new ExPerformanceCounter(base.CategoryName, "Microsoft Online STS Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter19
				});
				list.Add(this.NumberOfMsoIdStsRequests);
				this.PendingMsoIdStsRequests = new ExPerformanceCounter(base.CategoryName, "Current Microsoft Online STS Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingMsoIdStsRequests);
				this.AverageRPSCallLatency = new ExPerformanceCounter(base.CategoryName, "Average RPS Call Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageRPSCallLatency);
				this.AverageRPSCallLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average RPS Call Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageRPSCallLatencyBase);
				this.AverageFedStsResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Federated STS Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageFedStsResponseTime);
				this.AverageFedStsResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Federated STS Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageFedStsResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter20 = new ExPerformanceCounter(base.CategoryName, "Federated STS Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter20);
				this.PendingFedStsRequests = new ExPerformanceCounter(base.CategoryName, "Current Federated STS Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingFedStsRequests);
				this.AverageSamlStsResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Shibboleth STS Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSamlStsResponseTime);
				this.AverageSamlStsResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Shibboleth STS Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSamlStsResponseTimeBase);
				this.NumberOfOutgoingSamlStsRequests = new ExPerformanceCounter(base.CategoryName, "Shibboleth STS Auth Requests Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOutgoingSamlStsRequests);
				ExPerformanceCounter exPerformanceCounter21 = new ExPerformanceCounter(base.CategoryName, "Shibboleth STS Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter21);
				this.NumberOfOutgoingFedStsRequests = new ExPerformanceCounter(base.CategoryName, "Federated STS Auth Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter20,
					exPerformanceCounter21
				});
				list.Add(this.NumberOfOutgoingFedStsRequests);
				this.PendingSamlStsRequests = new ExPerformanceCounter(base.CategoryName, "Current Shibboleth STS Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingSamlStsRequests);
				ExPerformanceCounter exPerformanceCounter22 = new ExPerformanceCounter(base.CategoryName, "Failed TOU Checks/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter22);
				this.NumberOfTOUFailures = new ExPerformanceCounter(base.CategoryName, "Failed TOU Checks Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter22
				});
				list.Add(this.NumberOfTOUFailures);
				this.NamespaceBlacklistSize = new ExPerformanceCounter(base.CategoryName, "Black-listed Namespace Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NamespaceBlacklistSize);
				this.NamespaceWhitelistSize = new ExPerformanceCounter(base.CategoryName, "White-listed Namespace Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NamespaceWhitelistSize);
				ExPerformanceCounter exPerformanceCounter23 = new ExPerformanceCounter(base.CategoryName, "Offline OrgId Auth/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter23);
				this.OfflineOrgIdAuthenticationCount = new ExPerformanceCounter(base.CategoryName, "Offline OrgId Authentication Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter23
				});
				list.Add(this.OfflineOrgIdAuthenticationCount);
				this.NumberOfOfflineOrgIdRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "OfflineOrgId Requests Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOfflineOrgIdRequestsLastMinute);
				this.OfflineOrgIdRedirectCount = new ExPerformanceCounter(base.CategoryName, "Offline OrgId Authentication Redirect Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OfflineOrgIdRedirectCount);
				this.NumberOfLowConfidenceOfflineOrgIdRequests = new ExPerformanceCounter(base.CategoryName, "Low Password Confidence Auth Requests to AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfLowConfidenceOfflineOrgIdRequests);
				this.NumberOfFailedOfflineOrgIdRequests = new ExPerformanceCounter(base.CategoryName, "Failed Offline OrgId Auth Requests to AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfFailedOfflineOrgIdRequests);
				this.NumberOfOfflineOrgIdRequestWithInvalidCredential = new ExPerformanceCounter(base.CategoryName, "Offline OrgId Requests with invalid credential", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOfflineOrgIdRequestWithInvalidCredential);
				ExPerformanceCounter exPerformanceCounter24 = new ExPerformanceCounter(base.CategoryName, "Mailbox access count/sec for last logon timestamp", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter24);
				this.NumberOfMailboxAccess = new ExPerformanceCounter(base.CategoryName, "Mailbox access for last logon timestamp", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter24
				});
				list.Add(this.NumberOfMailboxAccess);
				ExPerformanceCounter exPerformanceCounter25 = new ExPerformanceCounter(base.CategoryName, "Tenant Nego config requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter25);
				this.NumberOfTenantNegoRequests = new ExPerformanceCounter(base.CategoryName, "Tenant Nego config requests total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter25
				});
				list.Add(this.NumberOfTenantNegoRequests);
				this.TenantNegoCacheHit = new ExPerformanceCounter(base.CategoryName, "Cache hit percentage - Tenant Nego config", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TenantNegoCacheHit);
				this.TenantNegoCacheHitBase = new ExPerformanceCounter(base.CategoryName, "Base for TenantNegoCacheHit", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TenantNegoCacheHitBase);
				this.TenantNegoCacheSize = new ExPerformanceCounter(base.CategoryName, "Tenant Nego config cache size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TenantNegoCacheSize);
				this.AverageMServResponseTime = new ExPerformanceCounter(base.CategoryName, "Average MServ response time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMServResponseTime);
				this.AverageMServResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for AverageMServResponseTimeAverage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMServResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter26 = new ExPerformanceCounter(base.CategoryName, "MServ requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter26);
				this.NumberOfMServRequests = new ExPerformanceCounter(base.CategoryName, "MServ requests total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter26
				});
				list.Add(this.NumberOfMServRequests);
				this.PendingMServRequests = new ExPerformanceCounter(base.CategoryName, "Current MServ requests pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingMServRequests);
				ExPerformanceCounter exPerformanceCounter27 = new ExPerformanceCounter(base.CategoryName, "Failed MServ requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter27);
				this.FailedMServRequests = new ExPerformanceCounter(base.CategoryName, "Failed MServ requests total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter27
				});
				list.Add(this.FailedMServRequests);
				ExPerformanceCounter exPerformanceCounter28 = new ExPerformanceCounter(base.CategoryName, "Cookie Based Auth requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter28);
				this.CookieBasedAuthRequests = new ExPerformanceCounter(base.CategoryName, "Cookie Based Auth requests total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter28
				});
				list.Add(this.CookieBasedAuthRequests);
				this.PercentageOfCookieBasedAuth = new ExPerformanceCounter(base.CategoryName, "Percentage of cookie based auth", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageOfCookieBasedAuth);
				ExPerformanceCounter exPerformanceCounter29 = new ExPerformanceCounter(base.CategoryName, "Expired Cookie Based Auth requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter29);
				this.ExpiredCookieAuthRequests = new ExPerformanceCounter(base.CategoryName, "Total auth requests with expired cookie", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter29
				});
				list.Add(this.ExpiredCookieAuthRequests);
				ExPerformanceCounter exPerformanceCounter30 = new ExPerformanceCounter(base.CategoryName, "Failed Cookie Based Auth requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter30);
				this.FailedCookieAuthRequests = new ExPerformanceCounter(base.CategoryName, "Total failed auth requests with cookie", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter30
				});
				list.Add(this.FailedCookieAuthRequests);
				ExPerformanceCounter exPerformanceCounter31 = new ExPerformanceCounter(base.CategoryName, "Remote Auth requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter31);
				this.RemoteAuthRequests = new ExPerformanceCounter(base.CategoryName, "Total remote auth requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter31
				});
				list.Add(this.RemoteAuthRequests);
				long num = this.NumberOfCurrentRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter32 in list)
					{
						exPerformanceCounter32.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal LiveIdBasicAuthenticationCountersInstance(string instanceName) : base(instanceName, "MSExchange LiveIdBasicAuthentication")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfCurrentRequests = new ExPerformanceCounter(base.CategoryName, "Current Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfCurrentRequests);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfAuthRequests = new ExPerformanceCounter(base.CategoryName, "Total Auth Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfAuthRequests);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Successful Auth/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.NumberOfSuccessfulAuthentications = new ExPerformanceCounter(base.CategoryName, "Successful Auth Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.NumberOfSuccessfulAuthentications);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Invalid Credentials/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.NumberOfInvalidCredentials = new ExPerformanceCounter(base.CategoryName, "Invalid Credentials Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfInvalidCredentials);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Failed Auth/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.NumberOfFailedAuthentications = new ExPerformanceCounter(base.CategoryName, "Failed Auth Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.NumberOfFailedAuthentications);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Failed User Recoverable Auth/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.NumberOfFailedRecoverableAuthentications = new ExPerformanceCounter(base.CategoryName, "Failed User Recoverable Auth Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.NumberOfFailedRecoverableAuthentications);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Failed Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.NumberOfFailedRequests = new ExPerformanceCounter(base.CategoryName, "Failed Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.NumberOfFailedRequests);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Timed Out Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.NumberOfTimedOutRequests = new ExPerformanceCounter(base.CategoryName, "Timed Out Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter7
				});
				list.Add(this.NumberOfTimedOutRequests);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Application Password Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				this.NumberOfAppPasswords = new ExPerformanceCounter(base.CategoryName, "Application Password Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter8
				});
				list.Add(this.NumberOfAppPasswords);
				this.AverageResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Auth Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTime);
				this.AverageResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Auth Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTimeBase);
				this.NumberOfRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "Requests Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfRequestsLastMinute);
				this.NumberOfOrgIdRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "OrgId Requests Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOrgIdRequestsLastMinute);
				this.PercentageFailedAuthenticationsLastMinute = new ExPerformanceCounter(base.CategoryName, "Failed Authentications Percentage Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageFailedAuthenticationsLastMinute);
				this.PercentageFailedRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "Failed Requests Percentage Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageFailedRequestsLastMinute);
				this.PercentageTimedOutRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "Timed Out Requests Percentage Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageTimedOutRequestsLastMinute);
				this.NumberOfCachedRequests = new ExPerformanceCounter(base.CategoryName, "Cached Auth Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfCachedRequests);
				this.LogonCacheHit = new ExPerformanceCounter(base.CategoryName, "Logon Cache hit percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LogonCacheHit);
				this.LogonCacheSize = new ExPerformanceCounter(base.CategoryName, "Logon Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LogonCacheSize);
				this.InvalidCredCacheSize = new ExPerformanceCounter(base.CategoryName, "Invalid Cred Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.InvalidCredCacheSize);
				this.AdUserCacheHit = new ExPerformanceCounter(base.CategoryName, "User Entry Cache Hit Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdUserCacheHit);
				this.AdUserCacheSize = new ExPerformanceCounter(base.CategoryName, "User Entry Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AdUserCacheSize);
				this.AverageAdResponseTime = new ExPerformanceCounter(base.CategoryName, "AD Average Auth Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAdResponseTime);
				this.AverageAdResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for AverageADResponseTime", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAdResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "AD Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.NumberOfAdRequests = new ExPerformanceCounter(base.CategoryName, "AD Auth Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter9
				});
				list.Add(this.NumberOfAdRequests);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "AD Access Requests/sec for OfflineOrgId", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				this.NumberOfAdRequestForOfflineOrgId = new ExPerformanceCounter(base.CategoryName, "AD Access Requests for OfflineOrgId", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter10
				});
				list.Add(this.NumberOfAdRequestForOfflineOrgId);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "AD Failed Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				this.FailedAdRequests = new ExPerformanceCounter(base.CategoryName, "AD Failed Auth Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter11
				});
				list.Add(this.FailedAdRequests);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "AD Password Synchronizations/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.AdPasswordSyncs = new ExPerformanceCounter(base.CategoryName, "AD Password Synchronizations", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter12
				});
				list.Add(this.AdPasswordSyncs);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "AD UPN Synchronizations/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.AdUpnSyncs = new ExPerformanceCounter(base.CategoryName, "AD UPN Synchronizations", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter13
				});
				list.Add(this.AdUpnSyncs);
				this.AverageHrdResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Windows Live Home Realm Discovery Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageHrdResponseTime);
				this.AverageHrdResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for AverageHrdResponseTimeAverage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageHrdResponseTimeBase);
				this.HrdCacheHit = new ExPerformanceCounter(base.CategoryName, "Cache hit percentage - Home Realm Discovery", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.HrdCacheHit);
				this.HrdCacheHitBase = new ExPerformanceCounter(base.CategoryName, "Base for HrdCacheHit", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.HrdCacheHitBase);
				this.HrdCacheSize = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Domain Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.HrdCacheSize);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.NumberOfOutgoingHrdRequests = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter14
				});
				list.Add(this.NumberOfOutgoingHrdRequests);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Requests/sec to AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.NumberOfADHrdRequests = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Requests Total to AD", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter15
				});
				list.Add(this.NumberOfADHrdRequests);
				ExPerformanceCounter exPerformanceCounter16 = new ExPerformanceCounter(base.CategoryName, "Failed Home Realm Discovery Requests/sec to AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter16);
				this.NumberOfFailedADHrdRequests = new ExPerformanceCounter(base.CategoryName, "Failed Home Realm Discovery Requests Total to AD", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter16
				});
				list.Add(this.NumberOfFailedADHrdRequests);
				ExPerformanceCounter exPerformanceCounter17 = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Record Update per second in AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter17);
				this.NumberOfADHrdUpdate = new ExPerformanceCounter(base.CategoryName, "Home Realm Discovery Record Update Count in AD", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter17
				});
				list.Add(this.NumberOfADHrdUpdate);
				this.PendingHrdRequests = new ExPerformanceCounter(base.CategoryName, "Current Home Realm Discovery Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingHrdRequests);
				this.AverageLiveIdResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Windows Live Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLiveIdResponseTime);
				this.AverageLiveIdResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Windows Live Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageLiveIdResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter18 = new ExPerformanceCounter(base.CategoryName, "LiveID STS Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter18);
				this.NumberOfLiveIdStsRequests = new ExPerformanceCounter(base.CategoryName, "LiveID STS Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter18
				});
				list.Add(this.NumberOfLiveIdStsRequests);
				this.PendingStsRequests = new ExPerformanceCounter(base.CategoryName, "Current LiveID STS Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingStsRequests);
				this.AverageMsoIdResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Microsoft Online Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMsoIdResponseTime);
				this.AverageMsoIdResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Microsoft Online Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMsoIdResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter19 = new ExPerformanceCounter(base.CategoryName, "Microsoft Online STS Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter19);
				this.NumberOfMsoIdStsRequests = new ExPerformanceCounter(base.CategoryName, "Microsoft Online STS Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter19
				});
				list.Add(this.NumberOfMsoIdStsRequests);
				this.PendingMsoIdStsRequests = new ExPerformanceCounter(base.CategoryName, "Current Microsoft Online STS Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingMsoIdStsRequests);
				this.AverageRPSCallLatency = new ExPerformanceCounter(base.CategoryName, "Average RPS Call Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageRPSCallLatency);
				this.AverageRPSCallLatencyBase = new ExPerformanceCounter(base.CategoryName, "Base for Average RPS Call Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageRPSCallLatencyBase);
				this.AverageFedStsResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Federated STS Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageFedStsResponseTime);
				this.AverageFedStsResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Federated STS Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageFedStsResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter20 = new ExPerformanceCounter(base.CategoryName, "Federated STS Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter20);
				this.PendingFedStsRequests = new ExPerformanceCounter(base.CategoryName, "Current Federated STS Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingFedStsRequests);
				this.AverageSamlStsResponseTime = new ExPerformanceCounter(base.CategoryName, "Average Shibboleth STS Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSamlStsResponseTime);
				this.AverageSamlStsResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Shibboleth STS Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSamlStsResponseTimeBase);
				this.NumberOfOutgoingSamlStsRequests = new ExPerformanceCounter(base.CategoryName, "Shibboleth STS Auth Requests Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOutgoingSamlStsRequests);
				ExPerformanceCounter exPerformanceCounter21 = new ExPerformanceCounter(base.CategoryName, "Shibboleth STS Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter21);
				this.NumberOfOutgoingFedStsRequests = new ExPerformanceCounter(base.CategoryName, "Federated STS Auth Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter20,
					exPerformanceCounter21
				});
				list.Add(this.NumberOfOutgoingFedStsRequests);
				this.PendingSamlStsRequests = new ExPerformanceCounter(base.CategoryName, "Current Shibboleth STS Requests Pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingSamlStsRequests);
				ExPerformanceCounter exPerformanceCounter22 = new ExPerformanceCounter(base.CategoryName, "Failed TOU Checks/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter22);
				this.NumberOfTOUFailures = new ExPerformanceCounter(base.CategoryName, "Failed TOU Checks Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter22
				});
				list.Add(this.NumberOfTOUFailures);
				this.NamespaceBlacklistSize = new ExPerformanceCounter(base.CategoryName, "Black-listed Namespace Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NamespaceBlacklistSize);
				this.NamespaceWhitelistSize = new ExPerformanceCounter(base.CategoryName, "White-listed Namespace Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NamespaceWhitelistSize);
				ExPerformanceCounter exPerformanceCounter23 = new ExPerformanceCounter(base.CategoryName, "Offline OrgId Auth/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter23);
				this.OfflineOrgIdAuthenticationCount = new ExPerformanceCounter(base.CategoryName, "Offline OrgId Authentication Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter23
				});
				list.Add(this.OfflineOrgIdAuthenticationCount);
				this.NumberOfOfflineOrgIdRequestsLastMinute = new ExPerformanceCounter(base.CategoryName, "OfflineOrgId Requests Last Minute Total", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOfflineOrgIdRequestsLastMinute);
				this.OfflineOrgIdRedirectCount = new ExPerformanceCounter(base.CategoryName, "Offline OrgId Authentication Redirect Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OfflineOrgIdRedirectCount);
				this.NumberOfLowConfidenceOfflineOrgIdRequests = new ExPerformanceCounter(base.CategoryName, "Low Password Confidence Auth Requests to AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfLowConfidenceOfflineOrgIdRequests);
				this.NumberOfFailedOfflineOrgIdRequests = new ExPerformanceCounter(base.CategoryName, "Failed Offline OrgId Auth Requests to AD", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfFailedOfflineOrgIdRequests);
				this.NumberOfOfflineOrgIdRequestWithInvalidCredential = new ExPerformanceCounter(base.CategoryName, "Offline OrgId Requests with invalid credential", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOfflineOrgIdRequestWithInvalidCredential);
				ExPerformanceCounter exPerformanceCounter24 = new ExPerformanceCounter(base.CategoryName, "Mailbox access count/sec for last logon timestamp", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter24);
				this.NumberOfMailboxAccess = new ExPerformanceCounter(base.CategoryName, "Mailbox access for last logon timestamp", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter24
				});
				list.Add(this.NumberOfMailboxAccess);
				ExPerformanceCounter exPerformanceCounter25 = new ExPerformanceCounter(base.CategoryName, "Tenant Nego config requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter25);
				this.NumberOfTenantNegoRequests = new ExPerformanceCounter(base.CategoryName, "Tenant Nego config requests total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter25
				});
				list.Add(this.NumberOfTenantNegoRequests);
				this.TenantNegoCacheHit = new ExPerformanceCounter(base.CategoryName, "Cache hit percentage - Tenant Nego config", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TenantNegoCacheHit);
				this.TenantNegoCacheHitBase = new ExPerformanceCounter(base.CategoryName, "Base for TenantNegoCacheHit", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TenantNegoCacheHitBase);
				this.TenantNegoCacheSize = new ExPerformanceCounter(base.CategoryName, "Tenant Nego config cache size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TenantNegoCacheSize);
				this.AverageMServResponseTime = new ExPerformanceCounter(base.CategoryName, "Average MServ response time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMServResponseTime);
				this.AverageMServResponseTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for AverageMServResponseTimeAverage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMServResponseTimeBase);
				ExPerformanceCounter exPerformanceCounter26 = new ExPerformanceCounter(base.CategoryName, "MServ requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter26);
				this.NumberOfMServRequests = new ExPerformanceCounter(base.CategoryName, "MServ requests total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter26
				});
				list.Add(this.NumberOfMServRequests);
				this.PendingMServRequests = new ExPerformanceCounter(base.CategoryName, "Current MServ requests pending", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PendingMServRequests);
				ExPerformanceCounter exPerformanceCounter27 = new ExPerformanceCounter(base.CategoryName, "Failed MServ requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter27);
				this.FailedMServRequests = new ExPerformanceCounter(base.CategoryName, "Failed MServ requests total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter27
				});
				list.Add(this.FailedMServRequests);
				ExPerformanceCounter exPerformanceCounter28 = new ExPerformanceCounter(base.CategoryName, "Cookie Based Auth requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter28);
				this.CookieBasedAuthRequests = new ExPerformanceCounter(base.CategoryName, "Cookie Based Auth requests total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter28
				});
				list.Add(this.CookieBasedAuthRequests);
				this.PercentageOfCookieBasedAuth = new ExPerformanceCounter(base.CategoryName, "Percentage of cookie based auth", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageOfCookieBasedAuth);
				ExPerformanceCounter exPerformanceCounter29 = new ExPerformanceCounter(base.CategoryName, "Expired Cookie Based Auth requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter29);
				this.ExpiredCookieAuthRequests = new ExPerformanceCounter(base.CategoryName, "Total auth requests with expired cookie", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter29
				});
				list.Add(this.ExpiredCookieAuthRequests);
				ExPerformanceCounter exPerformanceCounter30 = new ExPerformanceCounter(base.CategoryName, "Failed Cookie Based Auth requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter30);
				this.FailedCookieAuthRequests = new ExPerformanceCounter(base.CategoryName, "Total failed auth requests with cookie", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter30
				});
				list.Add(this.FailedCookieAuthRequests);
				ExPerformanceCounter exPerformanceCounter31 = new ExPerformanceCounter(base.CategoryName, "Remote Auth requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter31);
				this.RemoteAuthRequests = new ExPerformanceCounter(base.CategoryName, "Total remote auth requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter31
				});
				list.Add(this.RemoteAuthRequests);
				long num = this.NumberOfCurrentRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter32 in list)
					{
						exPerformanceCounter32.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter NumberOfCurrentRequests;

		public readonly ExPerformanceCounter NumberOfAuthRequests;

		public readonly ExPerformanceCounter NumberOfSuccessfulAuthentications;

		public readonly ExPerformanceCounter NumberOfInvalidCredentials;

		public readonly ExPerformanceCounter NumberOfFailedAuthentications;

		public readonly ExPerformanceCounter NumberOfFailedRecoverableAuthentications;

		public readonly ExPerformanceCounter NumberOfFailedRequests;

		public readonly ExPerformanceCounter NumberOfTimedOutRequests;

		public readonly ExPerformanceCounter NumberOfAppPasswords;

		public readonly ExPerformanceCounter AverageResponseTime;

		public readonly ExPerformanceCounter AverageResponseTimeBase;

		public readonly ExPerformanceCounter NumberOfRequestsLastMinute;

		public readonly ExPerformanceCounter NumberOfOrgIdRequestsLastMinute;

		public readonly ExPerformanceCounter PercentageFailedAuthenticationsLastMinute;

		public readonly ExPerformanceCounter PercentageFailedRequestsLastMinute;

		public readonly ExPerformanceCounter PercentageTimedOutRequestsLastMinute;

		public readonly ExPerformanceCounter NumberOfCachedRequests;

		public readonly ExPerformanceCounter LogonCacheHit;

		public readonly ExPerformanceCounter LogonCacheSize;

		public readonly ExPerformanceCounter InvalidCredCacheSize;

		public readonly ExPerformanceCounter AdUserCacheHit;

		public readonly ExPerformanceCounter AdUserCacheSize;

		public readonly ExPerformanceCounter AverageAdResponseTime;

		public readonly ExPerformanceCounter AverageAdResponseTimeBase;

		public readonly ExPerformanceCounter NumberOfAdRequests;

		public readonly ExPerformanceCounter NumberOfAdRequestForOfflineOrgId;

		public readonly ExPerformanceCounter FailedAdRequests;

		public readonly ExPerformanceCounter AdPasswordSyncs;

		public readonly ExPerformanceCounter AdUpnSyncs;

		public readonly ExPerformanceCounter AverageHrdResponseTime;

		public readonly ExPerformanceCounter AverageHrdResponseTimeBase;

		public readonly ExPerformanceCounter HrdCacheHit;

		public readonly ExPerformanceCounter HrdCacheHitBase;

		public readonly ExPerformanceCounter HrdCacheSize;

		public readonly ExPerformanceCounter NumberOfOutgoingHrdRequests;

		public readonly ExPerformanceCounter NumberOfADHrdRequests;

		public readonly ExPerformanceCounter NumberOfFailedADHrdRequests;

		public readonly ExPerformanceCounter NumberOfADHrdUpdate;

		public readonly ExPerformanceCounter PendingHrdRequests;

		public readonly ExPerformanceCounter AverageLiveIdResponseTime;

		public readonly ExPerformanceCounter AverageLiveIdResponseTimeBase;

		public readonly ExPerformanceCounter NumberOfLiveIdStsRequests;

		public readonly ExPerformanceCounter PendingStsRequests;

		public readonly ExPerformanceCounter AverageMsoIdResponseTime;

		public readonly ExPerformanceCounter AverageMsoIdResponseTimeBase;

		public readonly ExPerformanceCounter NumberOfMsoIdStsRequests;

		public readonly ExPerformanceCounter PendingMsoIdStsRequests;

		public readonly ExPerformanceCounter AverageRPSCallLatency;

		public readonly ExPerformanceCounter AverageRPSCallLatencyBase;

		public readonly ExPerformanceCounter AverageFedStsResponseTime;

		public readonly ExPerformanceCounter AverageFedStsResponseTimeBase;

		public readonly ExPerformanceCounter NumberOfOutgoingFedStsRequests;

		public readonly ExPerformanceCounter PendingFedStsRequests;

		public readonly ExPerformanceCounter AverageSamlStsResponseTime;

		public readonly ExPerformanceCounter AverageSamlStsResponseTimeBase;

		public readonly ExPerformanceCounter NumberOfOutgoingSamlStsRequests;

		public readonly ExPerformanceCounter PendingSamlStsRequests;

		public readonly ExPerformanceCounter NumberOfTOUFailures;

		public readonly ExPerformanceCounter NamespaceBlacklistSize;

		public readonly ExPerformanceCounter NamespaceWhitelistSize;

		public readonly ExPerformanceCounter OfflineOrgIdAuthenticationCount;

		public readonly ExPerformanceCounter NumberOfOfflineOrgIdRequestsLastMinute;

		public readonly ExPerformanceCounter OfflineOrgIdRedirectCount;

		public readonly ExPerformanceCounter NumberOfLowConfidenceOfflineOrgIdRequests;

		public readonly ExPerformanceCounter NumberOfFailedOfflineOrgIdRequests;

		public readonly ExPerformanceCounter NumberOfOfflineOrgIdRequestWithInvalidCredential;

		public readonly ExPerformanceCounter NumberOfMailboxAccess;

		public readonly ExPerformanceCounter NumberOfTenantNegoRequests;

		public readonly ExPerformanceCounter TenantNegoCacheHit;

		public readonly ExPerformanceCounter TenantNegoCacheHitBase;

		public readonly ExPerformanceCounter TenantNegoCacheSize;

		public readonly ExPerformanceCounter AverageMServResponseTime;

		public readonly ExPerformanceCounter AverageMServResponseTimeBase;

		public readonly ExPerformanceCounter NumberOfMServRequests;

		public readonly ExPerformanceCounter PendingMServRequests;

		public readonly ExPerformanceCounter FailedMServRequests;

		public readonly ExPerformanceCounter CookieBasedAuthRequests;

		public readonly ExPerformanceCounter PercentageOfCookieBasedAuth;

		public readonly ExPerformanceCounter ExpiredCookieAuthRequests;

		public readonly ExPerformanceCounter FailedCookieAuthRequests;

		public readonly ExPerformanceCounter RemoteAuthRequests;
	}
}
