using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication.Mailbox;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
	internal class AuthService : IAuthService
	{
		static AuthService()
		{
			AuthService.counters.Reset();
			AuthServiceStaticConfig authServiceStaticConfig = AuthServiceStaticConfig.Config;
			AuthService.wsseTokenBytesP1 = Encoding.UTF8.GetBytes("<wsse:UsernameToken wsu:Id=\"user\"><wsse:Username>");
			AuthService.wsseTokenBytesP2 = Encoding.UTF8.GetBytes("</wsse:Username><wsse:Password>");
			AuthService.wsseTokenBytesP3 = Encoding.UTF8.GetBytes("</wsse:Password></wsse:UsernameToken>");
			AuthService.wsseTokenByteCount = AuthService.wsseTokenBytesP1.Length + AuthService.wsseTokenBytesP2.Length + AuthService.wsseTokenBytesP3.Length;
			AuthService.hrdCache = new NamespaceCache(authServiceStaticConfig.hrdCacheBuckets, authServiceStaticConfig.hrdCacheBucketSize, TimeSpan.FromMinutes((double)authServiceStaticConfig.hrdCacheLifetime), authServiceStaticConfig.statisticCacheSize, TimeSpan.FromMinutes((double)authServiceStaticConfig.statisticLifetime));
			AuthService.hrdCache.PerfCounters = AuthService.counters;
			AuthService.userOperations = new UserOpsTracker();
			AuthService.logonCache = new LogonCache(authServiceStaticConfig.logonCacheSize, authServiceStaticConfig.logonCacheLifetime, authServiceStaticConfig.level1BadCredCacheSize, authServiceStaticConfig.level1BadCredLifetime, authServiceStaticConfig.level2BadCredCacheSize, authServiceStaticConfig.level2BadCredLifetime, authServiceStaticConfig.level2BadCredListSize);
			AuthService.adUserPropertyCache = new TimeoutCache<string, ADRawEntry>(authServiceStaticConfig.ADUserCacheBuckets, authServiceStaticConfig.ADUserCacheBucketSize, false);
			if (!authServiceStaticConfig.bypassTOUCheck)
			{
				AuthService.domainsByPassTOU = new TimeoutCache<string, bool>(authServiceStaticConfig.TOUCacheBuckets, authServiceStaticConfig.TOUCacheBucketSize, false);
			}
			LiveIdSTSBase.SiteName = authServiceStaticConfig.siteName;
			LiveIdSTSBase.RpsTicketLifetime = authServiceStaticConfig.RpsTicketLifetime;
			AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_AuthServiceConfigured, "AuthServiceConfigured", new object[]
			{
				authServiceStaticConfig.liveRst2Login,
				authServiceStaticConfig.liveHttpPostLogin,
				authServiceStaticConfig.liveRealmDiscoveryUri,
				LiveIdSTSBase.SiteName
			});
			AuthService.xmlSettings = new XmlWriterSettings();
			AuthService.xmlSettings.Encoding = new AuthService.UTF8EncodingMinusPreamble();
			AuthService.xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
			AuthService.xmlSettings.CheckCharacters = false;
			AuthService.propertiesToGet = new PropertyDefinition[]
			{
				ADRecipientSchema.PrimarySmtpAddress,
				ADUserSchema.UserPrincipalName,
				ADRecipientSchema.WindowsLiveID,
				ADObjectSchema.OrganizationId,
				ADUserSchema.NetIDSuffix,
				ADMailboxRecipientSchema.SamAccountName,
				ADUserSchema.NetID,
				ADUserSchema.ConsumerNetID,
				ADMailboxRecipientSchema.ExchangeGuid,
				ADMailboxRecipientSchema.Sid,
				ADMailboxRecipientSchema.Database,
				ADUserSchema.ArchiveDatabase,
				ADUserSchema.ArchiveGuid,
				IADMailStorageSchema.DatabaseName,
				IADSecurityPrincipalSchema.AltSecurityIdentities
			};
			AuthService.ResetStaticConfig(null);
			AuthService.percentageLogonCacheHitLastMinute = new SlidingPercentageCounter(TimeSpan.FromMinutes((double)authServiceStaticConfig.percentileCountersLastMinutes), TimeSpan.FromSeconds((double)authServiceStaticConfig.percentileCountersUnitOfExpiryInSeconds));
			AuthService.percentageAdUserHitLastMinute = new SlidingPercentageCounter(TimeSpan.FromMinutes((double)authServiceStaticConfig.percentileCountersLastMinutes), TimeSpan.FromSeconds((double)authServiceStaticConfig.percentileCountersUnitOfExpiryInSeconds));
			AuthService.percentileFailedRequestsLastMinutes = new SlidingPercentageCounter(TimeSpan.FromMinutes((double)authServiceStaticConfig.percentileCountersLastMinutes), TimeSpan.FromSeconds((double)authServiceStaticConfig.percentileCountersUnitOfExpiryInSeconds));
			AuthService.percentileFailedAuthenticationsLastMinutes = new SlidingPercentageCounter(TimeSpan.FromMinutes((double)authServiceStaticConfig.percentileCountersLastMinutes), TimeSpan.FromSeconds((double)authServiceStaticConfig.percentileCountersUnitOfExpiryInSeconds));
			AuthService.percentileTimedOutRequestsLastMinutes = new SlidingPercentageCounter(TimeSpan.FromMinutes((double)authServiceStaticConfig.percentileCountersLastMinutes), TimeSpan.FromSeconds((double)authServiceStaticConfig.percentileCountersUnitOfExpiryInSeconds));
			AuthService.percentileCountersTimer = new Timer(new TimerCallback(AuthService.UpdatePercentileCounters));
			AuthService.percentileCountersTimer.Change(authServiceStaticConfig.percentileCountersUpdateIntervalInSeconds * 1000, -1);
			AuthService.orgIdRequestCountLastMinutes = new SlidingTotalCounter(TimeSpan.FromMinutes((double)authServiceStaticConfig.percentileCountersLastMinutes), TimeSpan.FromSeconds((double)authServiceStaticConfig.percentileCountersUnitOfExpiryInSeconds));
			AuthService.offlineOrgIdRequestCountLastMinutes = new SlidingTotalCounter(TimeSpan.FromMinutes((double)authServiceStaticConfig.percentileCountersLastMinutes), TimeSpan.FromSeconds((double)authServiceStaticConfig.percentileCountersUnitOfExpiryInSeconds));
			AuthService.certErrorCache = new TimeoutCache<WebRequest, string>(authServiceStaticConfig.certErrorCacheBuckets, authServiceStaticConfig.certErrorCacheBucketSize, false);
			AuthService.IsBackendServer = AuthServiceHelper.IsMailboxRole;
		}

		internal static TimeoutCache<WebRequest, string> CertErrorCache
		{
			get
			{
				return AuthService.certErrorCache;
			}
		}

		internal static ExEventLog EventLogger
		{
			get
			{
				return AuthService.eventLogger;
			}
		}

		private static void ResetStaticConfig(object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction(0L, "Entering ResetStaticConfig()");
			AuthServiceStaticConfig.Config = null;
			AuthServiceStaticConfig authServiceStaticConfig = AuthServiceStaticConfig.Config;
			AuthService.hrdCache.UpdatePerfCounters(authServiceStaticConfig);
			LogonCacheConfig.Initialize(authServiceStaticConfig.badCredsLifetime, authServiceStaticConfig.badCredsRecoverableLifetime);
			LiveIdSTSBase.RpsTicketLifetime = authServiceStaticConfig.RpsTicketLifetime;
			if (AuthService.configTimer != null)
			{
				AuthService.configTimer.Dispose();
			}
			AuthService.configTimer = new Timer(new TimerCallback(AuthService.ResetStaticConfig));
			AuthService.configTimer.Change(AuthServiceStaticConfig.Config.configLifetimeSeconds * 1000, -1);
			ServicePointManager.MaxServicePointIdleTime = 1000 * AuthServiceStaticConfig.Config.MaxServicePointIdleTimeInSeconds;
			ExTraceGlobals.AuthenticationTracer.TraceFunction(0L, "Leaving ResetStaticConfig()");
		}

		private static void UpdatePercentileCounters(object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction(0L, "Entering UpdatePercentileCounters()");
			double slidingPercentage = AuthService.percentileFailedAuthenticationsLastMinutes.GetSlidingPercentage();
			AuthService.counters.NumberOfRequestsLastMinute.RawValue = AuthService.percentileFailedAuthenticationsLastMinutes.Denominator;
			AuthService.counters.NumberOfOrgIdRequestsLastMinute.RawValue = AuthService.orgIdRequestCountLastMinutes.Sum;
			AuthService.counters.NumberOfOfflineOrgIdRequestsLastMinute.RawValue = AuthService.offlineOrgIdRequestCountLastMinutes.Sum;
			if (AuthService.counters.NumberOfRequestsLastMinute.RawValue < (long)AuthServiceStaticConfig.Config.numberOfTotalRequestsToIgnoreInPercentileCounter)
			{
				AuthService.counters.PercentageFailedAuthenticationsLastMinute.RawValue = 0L;
				AuthService.counters.PercentageFailedRequestsLastMinute.RawValue = 0L;
				AuthService.counters.PercentageTimedOutRequestsLastMinute.RawValue = 0L;
				AuthService.counters.LogonCacheHit.RawValue = 0L;
				AuthService.counters.AdUserCacheHit.RawValue = 0L;
			}
			else
			{
				AuthService.counters.PercentageFailedAuthenticationsLastMinute.RawValue = (long)((int)slidingPercentage);
				AuthService.counters.PercentageFailedRequestsLastMinute.RawValue = (long)((int)AuthService.percentileFailedRequestsLastMinutes.GetSlidingPercentage());
				AuthService.counters.PercentageTimedOutRequestsLastMinute.RawValue = (long)((int)AuthService.percentileTimedOutRequestsLastMinutes.GetSlidingPercentage());
				AuthService.counters.LogonCacheHit.RawValue = (long)((int)AuthService.percentageLogonCacheHitLastMinute.GetSlidingPercentage());
				AuthService.counters.AdUserCacheHit.RawValue = (long)((int)AuthService.percentageAdUserHitLastMinute.GetSlidingPercentage());
			}
			AuthService.percentileCountersTimer.Dispose();
			AuthService.percentileCountersTimer = new Timer(new TimerCallback(AuthService.UpdatePercentileCounters));
			AuthService.percentileCountersTimer.Change(AuthServiceStaticConfig.Config.percentileCountersUpdateIntervalInSeconds * 1000, -1);
			ExTraceGlobals.AuthenticationTracer.TraceFunction(0L, "Leaving UpdatePercentileCounters()");
		}

		internal static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			WebRequest webRequest = sender as WebRequest;
			AuthServiceStaticConfig authServiceStaticConfig = AuthServiceStaticConfig.Config;
			if (webRequest != null)
			{
				X509Certificate2 x509Certificate = new X509Certificate2(certificate);
				string value = string.Format(CultureInfo.InvariantCulture, "The SSL connection could not be established because of the certification issue in the server. SslPolicyError:{0}. Certificate: {1}, {2}, {3}", new object[]
				{
					sslPolicyErrors,
					x509Certificate.IssuerName.Name,
					x509Certificate.Thumbprint,
					x509Certificate.GetExpirationDateString()
				});
				AuthService.CertErrorCache.AddAbsolute(webRequest, value, TimeSpan.FromSeconds((double)authServiceStaticConfig.certErrorExpireTimeInSeconds), null);
				return false;
			}
			string message = "Something unexpected happened. The sender is not instance of WebRequest. " + sender.GetType().FullName;
			ExTraceGlobals.AuthenticationTracer.TraceError((long)sender.GetHashCode(), message);
			return false;
		}

		internal AuthService()
		{
			this.config = AuthServiceStaticConfig.Config;
			this.Timeout = this.config.defaultTimeout;
			this.authState = AuthService.authStateEnum.authUnknown;
			this.nextState = AuthService.authStateEnum.authUnknown;
			this.iisLogMsg = new StringBuilder(256);
			this.allowOfflineOrgId = AuthServiceStaticConfig.IsOfflineOrgIdEnabled;
			this.enableXmlAuthForLiveId = this.config.EnableXmlAuthForLiveId;
		}

		~AuthService()
		{
			if (this.releaseClientOp)
			{
				if (this.userName != null && !this.userName.StartsWith("HealthMailbox", StringComparison.OrdinalIgnoreCase))
				{
					AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_ServiceIsCalledWhenShuttingDown, "~AuthService", new object[]
					{
						this.clientOp.User
					});
				}
				ExTraceGlobals.AuthenticationTracer.TraceError<string>((long)this.GetHashCode(), "Client operation for user {0} was not released, operation was still considered active", this.clientOp.User);
				AuthService.userOperations.ReleaseOperation(this.clientOp);
			}
			if (this.activityScope != null)
			{
				this.activityScope.Dispose();
			}
		}

		internal int Timeout { get; set; }

		public IntPtr LogonUserFederationCreds(uint clientProcessId, byte[] ansiUserName, byte[] ansiPassword, string organizationContext, bool syncAD, string userEndpoint, string userAgent, string userAddress, Guid requestId, out string iisLogMsg)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering LogonUserFederationCreds()");
			IAsyncResult asyncResult = this.BeginLogonUserFederationCredsAsync(clientProcessId, ansiUserName, ansiPassword, organizationContext, syncAD, userEndpoint, userAgent, userAddress, requestId, null, null);
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)asyncResult;
			lazyAsyncResult.InternalWaitForCompletion();
			IntPtr result = this.EndLogonUserFederationCredsAsync(out iisLogMsg, asyncResult);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving LogonUserFederationCreds()");
			return result;
		}

		public AuthStatus LogonCommonAccessTokenFederationCredsTest(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, AuthOptions options, string remoteOrganizationContext, string userEndpoint, string userAgent, string userAddress, Guid requestId, bool? preferOfflineOrgId, TestFailoverFlags testFailOver, out string commonAccessToken, out string iisLogMsg)
		{
			this.calledFromTestFunction = true;
			if (preferOfflineOrgId != null)
			{
				this.isOrgIdOffline = preferOfflineOrgId.Value;
			}
			this.allowOfflineOrgId = true;
			this.useHrdCache = false;
			this.useLogonCache = false;
			this.testFailOver = testFailOver;
			if ((options & AuthOptions.ReturnWindowsIdentity) == AuthOptions.None)
			{
				IAsyncResult asyncResult = this.BeginLogonCommonAccessTokenFederationCredsAsync(clientProcessId, remoteUserName, remotePassword, options, remoteOrganizationContext, userEndpoint, userAgent, userAddress, requestId, null, null);
				LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)asyncResult;
				lazyAsyncResult.InternalWaitForCompletion();
				return this.EndLogonCommonAccessTokenFederationCredsAsync(out commonAccessToken, out iisLogMsg, asyncResult);
			}
			if ((options & AuthOptions.BypassCache) != AuthOptions.None)
			{
				this.useLogonCache = false;
			}
			if ((options & AuthOptions.BypassPositiveCache) != AuthOptions.None)
			{
				this.usePositiveLogonCache = false;
			}
			if ((options & AuthOptions.PasswordAndHRDSync) != AuthOptions.None)
			{
				this.passwordAndHrdSync = true;
			}
			if ((options & AuthOptions.SyncADBackEndOnly) != AuthOptions.None)
			{
				this.syncADBackEndOnly = true;
			}
			IntPtr handle = this.LogonUserFederationCreds(clientProcessId, remoteUserName, remotePassword, remoteOrganizationContext, (options & AuthOptions.SyncAD) != AuthOptions.None, userEndpoint, userAgent, userAddress, requestId, out iisLogMsg);
			if (this.requestUpdateHrdInBackend)
			{
				this.TraceAndReturnInformation(this.GetHashCode(), "<HRD_IN_CAT>", new object[0]);
				this.TraceAndReturnInformation(this.GetHashCode(), "<PW_IN_CAT>", new object[0]);
			}
			else if (this.includePWinCat)
			{
				this.TraceAndReturnInformation(this.GetHashCode(), "<PW_IN_CAT>", new object[0]);
			}
			iisLogMsg = this.iisLogMsg.ToString();
			if (handle.ToInt32() > 0)
			{
				LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = LiveIdBasicTokenAccessor.Create(this.puid, this.userName);
				commonAccessToken = liveIdBasicTokenAccessor.GetToken().Serialize();
				if (this.clientProcessId != 4294967295U)
				{
					using (new SafeUserTokenHandle(handle))
					{
					}
				}
				return AuthStatus.LogonSuccess;
			}
			commonAccessToken = null;
			return AuthStatus.LogonFailed;
		}

		public IAsyncResult BeginLogonUserFederationCredsAsync(uint clientProcessId, byte[] ansiUserName, byte[] ansiPassword, string organizationContext, bool syncAD, string userEndpoint, string userAgent, string userAddress, Guid requestId, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering BeginLogonUserFederationCredsAsync()");
			this.iisLogMsg = new StringBuilder(256);
			AuthService.counters.LogonCacheSize.RawValue = (long)AuthService.logonCache.ValidCredsCount;
			AuthService.counters.AdUserCacheSize.RawValue = (long)AuthService.adUserPropertyCache.Count;
			AuthService.counters.InvalidCredCacheSize.RawValue = (long)AuthService.logonCache.InvalidCredsCount;
			AuthService.counters.HrdCacheSize.RawValue = (long)AuthService.hrdCache.Count;
			try
			{
				if (ExTraceGlobals.AuthenticationTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.AuthenticationTracer.Information((long)this.GetHashCode(), "{0} starting clientProcessId={1}, ansiUserName={2}, password={3}, syncAD={4}, userEndpoint={5}, userAgent={6}, userAddress={7}", new object[]
					{
						this.passwordAndHrdSync ? "Password sync" : "Logon",
						clientProcessId,
						(ansiUserName != null) ? Encoding.Default.GetString(ansiUserName) : "<null>",
						(ansiPassword != null) ? "********" : "<null>",
						syncAD,
						userEndpoint,
						userAgent,
						userAddress
					});
				}
				if (ansiUserName == null || ansiPassword == null)
				{
					if (ansiUserName == null)
					{
						this.TraceAndReturnError(this.GetHashCode(), "ansiUserName is null", new object[0]);
					}
					if (ansiPassword == null)
					{
						this.TraceAndReturnError(this.GetHashCode(), "ansiPassword is null", new object[0]);
					}
					throw new InvalidOperationException();
				}
				if (this.intCompleted != 0)
				{
					this.TraceAndReturnError(this.GetHashCode(), "improper use of BeginLogonUserFederation - already in completed state.", new object[0]);
					throw new InvalidOperationException();
				}
				if (ansiUserName.Length > 0 && ansiUserName[ansiUserName.Length - 1] == 0)
				{
					int num = ansiUserName.Length - 1;
					while (num >= 1 && ansiUserName[num - 1] == 0)
					{
						num--;
					}
					this.userName = Encoding.Default.GetString(ansiUserName, 0, num);
				}
				else
				{
					this.userName = Encoding.Default.GetString(ansiUserName);
				}
				if (ansiPassword.Length > 0 && ansiPassword[ansiPassword.Length - 1] == 0)
				{
					int num2 = ansiPassword.Length - 1;
					while (num2 >= 1 && ansiPassword[num2 - 1] == 0)
					{
						num2--;
					}
					byte[] array = new byte[num2];
					Array.Copy(ansiPassword, array, num2);
					Array.Clear(ansiPassword, 0, ansiPassword.Length);
					ansiPassword = array;
				}
				if (this.passwordAndHrdSync)
				{
					string[] array2 = this.userName.Split(AuthService.colonSeparator, 3);
					if (array2.Length < 2)
					{
						this.TraceAndReturnError(this.GetHashCode(), "ansiUserName \"{0}\" is not a valid PUID:UPN or PUID:UPN:true|false combination", new object[]
						{
							this.userName
						});
						throw new ArgumentException("ansiUserName not valid PUID:UPN");
					}
					this.puid = array2[0];
					this.userName = array2[1];
					if (array2.Length == 3)
					{
						this.requestUpdateHrdInBackend = bool.Parse(array2[array2.Length - 1]);
					}
					ansiUserName = Encoding.Default.GetBytes(this.userName);
				}
				SmtpAddress smtpAddress = new SmtpAddress(this.userName);
				if (!smtpAddress.IsValidAddress)
				{
					this.TraceAndReturnError(this.GetHashCode(), "ansiUserName \"{0}\" is not a valid SMTP address", new object[]
					{
						this.userName
					});
					throw new ArgumentException("ansiUserName not valid SmtpAddress");
				}
				this.userDomain = smtpAddress.Domain.ToLowerInvariant();
				this.namespaceInfo = null;
				this.clientOp = AuthService.userOperations.GetOperation(this.userName);
				this.releaseClientOp = true;
				if (requestId != Guid.Empty)
				{
					ActivityContextState activityContextState = new ActivityContextState(new Guid?(requestId), new ConcurrentDictionary<Enum, object>());
					ActivityContext.ClearThreadScope();
					this.activityScope = ActivityContext.Resume(activityContextState, null);
					this.iisLogMsg.AppendFormat("<RequestId={0}>", this.activityScope.ActivityId);
					this.activityScope.UserEmail = this.userName;
				}
				this.outerResult = new LazyAsyncResult(this, state, callback);
				Interlocked.Increment(ref AuthService.numberOfAuthRequests);
				AuthService.counters.NumberOfAuthRequests.RawValue = (long)AuthService.numberOfAuthRequests;
				Interlocked.Increment(ref AuthService.numberOfCurrentRequests);
				AuthService.counters.NumberOfCurrentRequests.RawValue = (long)AuthService.numberOfCurrentRequests;
				this.clientProcessId = clientProcessId;
				this.ansiUserName = ansiUserName;
				this.ansiPassword = ansiPassword;
				this.passwordHash = HashExtension.GetPasswordHash(ansiPassword);
				this.isAppPassword = LiveIdSTSBase.IsPossibleAppPassword(ansiPassword);
				this.syncAD = syncAD;
				this.organizationContext = organizationContext;
				this.userEndpoint = userEndpoint;
				this.userAgent = userAgent;
				this.userHostAddress = this.FilterInternalIP(userAddress);
				this.stopwatch = Stopwatch.StartNew();
				this.timer = new Timer(AuthService.timerCallback, this, TimeSpan.FromSeconds((double)this.Timeout), TimeSpan.Zero);
				this.namespaceStats = AuthService.hrdCache.GetStatistics(this.userDomain, this.GetHashCode());
				if (!this.isAppPassword && this.useHrdCache)
				{
					if (!AuthService.hrdCache.TryGetValue(this.userDomain, out this.namespaceInfo))
					{
						ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "namespace '{0}' not found in HRD cache", this.userDomain);
					}
				}
				else if (this.isAppPassword)
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "username \"{0}\" is possible app password - force HRD lookup", this.userName);
				}
				else
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.GetHashCode(), "useHrdCache is false - force HRD lookup");
				}
				this.escapedUserBytes = AuthService.CleanBytes(this.ansiUserName, false);
				this.escapedPassBytes = AuthService.CleanBytes(this.ansiPassword, false);
				if (this.passwordAndHrdSync)
				{
					object value = this.UpdateCreds(this.puid);
					if (this.requestUpdateHrdInBackend)
					{
						this.StartHrd();
					}
					else
					{
						this.InvokeCallback(value);
					}
					return this.outerResult;
				}
				if (this.returnWindowsToken && !this.syncAD)
				{
					this.useLogonCache = false;
				}
				ExTraceGlobals.AuthenticationTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "useLogonCache is {0} for user {1}", this.useLogonCache, this.userName);
				if (this.useLogonCache && this.CheckLogonCache())
				{
					AuthService.counters.NumberOfCachedRequests.Increment();
					return this.outerResult;
				}
				this.TraceAndReturnInformation(this.GetHashCode(), "<X-forwarded-for:{0}>", new object[]
				{
					this.userHostAddress ?? string.Empty
				});
				this.StartHrd();
			}
			catch (ArgumentException ex)
			{
				throw new FaultException<ArgumentException>(ex, new FaultReason(ex.ToString()), new FaultCode("Sender"));
			}
			catch (InvalidOperationException ex2)
			{
				throw new FaultException<InvalidOperationException>(ex2, new FaultReason(ex2.ToString()), new FaultCode("Sender"));
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving BeginLogonUserFederationCredsAsync()");
			return this.outerResult;
		}

		private string FilterInternalIP(string userHostAddresses)
		{
			if (string.IsNullOrEmpty(userHostAddresses))
			{
				return string.Empty;
			}
			if (userHostAddresses.IndexOf(',') < 0)
			{
				this.userClientIp = (this.config.InternalIPFilterRegex.IsMatch(userHostAddresses.Trim()) ? string.Empty : userHostAddresses.Trim());
				return this.userClientIp;
			}
			string[] array = userHostAddresses.Split(new char[]
			{
				','
			});
			StringBuilder stringBuilder = new StringBuilder(64);
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]) && !this.config.InternalIPFilterRegex.IsMatch(array[i].Trim()))
				{
					if (flag)
					{
						stringBuilder.Append(',');
					}
					else
					{
						flag = true;
					}
					stringBuilder.Append(array[i]);
					this.userClientIp = array[i];
				}
			}
			return stringBuilder.ToString();
		}

		private void StartHrd()
		{
			bool flag = this.allowOfflineOrgId && (this.namespaceInfo == null || !this.namespaceInfo.SyncedAD) && this.passwordAndHrdSync && this.requestUpdateHrdInBackend;
			if (this.namespaceInfo != null && (!(this.namespaceInfo.LastUpdateTime + TimeSpan.FromMinutes((double)AuthServiceStaticConfig.Config.hrdCacheLifetime) < DateTime.UtcNow) || this.isOrgIdOffline) && !flag)
			{
				AuthService.counters.HrdCacheHitBase.Increment();
				AuthService.counters.HrdCacheHit.Increment();
				this.FinishHrdResponse(this.namespaceInfo);
				return;
			}
			ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Starting home realm discovery for namespace '{0}'", this.userDomain);
			AuthService.counters.HrdCacheHitBase.Increment();
			if (AuthServiceHelper.IsOutlookComUser(this.userName))
			{
				if (this.namespaceInfo != null)
				{
					this.namespaceInfo.LastUpdateTime = DateTime.UtcNow;
				}
				else
				{
					this.namespaceInfo = new DomainConfig(this.userDomain, LiveIdInstanceType.Consumer, false, null, true, LivePreferredProtocol.Unknown);
					this.namespaceInfo.IsOutlookCom = true;
				}
				this.FinishHrdResponse(this.namespaceInfo);
				return;
			}
			bool flag2 = false;
			lock (this.clientOp)
			{
				if (this.clientOp.HrdEvent == null)
				{
					flag2 = true;
					this.hrdSetEventOnExit = true;
					this.clientOp.HrdEvent = new ManualResetEvent(false);
				}
			}
			if (flag2)
			{
				this.StartHrdRequestChain(this.config.defaultInstance);
				return;
			}
			this.WaitHrdRequestChain();
		}

		public IntPtr EndLogonUserFederationCredsAsync(out string iisLogMsg, IAsyncResult result)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering EndLogonUserFederationCredsAsync()");
			if (this.activityScope != null && this.activityScope.Status == ActivityContextStatus.ActivityStarted)
			{
				this.activityScope.End();
				this.activityScope = null;
			}
			AuthService.counters.LogonCacheSize.RawValue = (long)AuthService.logonCache.ValidCredsCount;
			AuthService.counters.AdUserCacheSize.RawValue = (long)AuthService.adUserPropertyCache.Count;
			AuthService.counters.InvalidCredCacheSize.RawValue = (long)AuthService.logonCache.InvalidCredsCount;
			AuthService.counters.HrdCacheSize.RawValue = (long)AuthService.hrdCache.Count;
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
			if (lazyAsyncResult.EndCalled)
			{
				this.TraceAndReturnError(this.GetHashCode(), "improper use of EndLogonUserFederation - EndLogon called already.", new object[0]);
				throw new InvalidOperationException();
			}
			lazyAsyncResult.EndCalled = true;
			if (this.escapedPassBytes != null)
			{
				Array.Clear(this.escapedPassBytes, 0, this.escapedPassBytes.Length);
			}
			if (this.returnWindowsToken)
			{
				Array.Clear(this.ansiPassword, 0, this.ansiPassword.Length);
			}
			iisLogMsg = this.iisLogMsg.ToString();
			Exception ex = lazyAsyncResult.Result as Exception;
			if (ex != null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving EndLogonUserFederationCredsAsync()");
				return (IntPtr)((long)this.TarpittedAuthError(AuthStatus.LogonFailed, this.namespaceStats.IsTarpitted));
			}
			TimerCallback timerCallback = lazyAsyncResult.Result as TimerCallback;
			if (timerCallback != null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving EndLogonUserFederationCredsAsync()");
				return (IntPtr)((long)this.TarpittedAuthError(AuthStatus.LiveIDFailed, this.namespaceStats.IsTarpitted));
			}
			IntPtr result2 = (IntPtr)lazyAsyncResult.Result;
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving EndLogonUserFederationCredsAsync()");
			return result2;
		}

		public IAsyncResult BeginLogonCommonAccessTokenFederationCredsAsync(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, AuthOptions options, string remoteOrganizationContext, string userEndpoint, string userAgent, string userAddress, Guid requestId, AsyncCallback callback, object state)
		{
			this.returnWindowsToken = false;
			if ((options & AuthOptions.BypassCache) != AuthOptions.None)
			{
				this.useLogonCache = false;
			}
			if ((options & AuthOptions.BypassPositiveCache) != AuthOptions.None)
			{
				this.usePositiveLogonCache = false;
			}
			if ((options & AuthOptions.PasswordAndHRDSync) != AuthOptions.None)
			{
				this.passwordAndHrdSync = true;
			}
			if ((options & AuthOptions.SyncADBackEndOnly) != AuthOptions.None)
			{
				this.syncADBackEndOnly = true;
			}
			if ((options & AuthOptions.SyncUPN) != AuthOptions.None)
			{
				this.syncUPN = true;
			}
			if ((options & AuthOptions.LiveIdXmlAuth) != AuthOptions.None)
			{
				this.enableXmlAuthForLiveId = true;
			}
			if ((options & AuthOptions.AllowOfflineOrgIdAsPrimeAuth) != AuthOptions.None && !this.isOrgIdOffline)
			{
				this.isOrgIdOffline = (AuthServiceStaticConfig.IsOrgIdOutage && this.allowOfflineOrgId);
			}
			return this.BeginLogonUserFederationCredsAsync(clientProcessId, remoteUserName, remotePassword, remoteOrganizationContext, (options & AuthOptions.SyncAD) != AuthOptions.None, userEndpoint, userAgent, userAddress, requestId, callback, state);
		}

		public AuthStatus EndLogonCommonAccessTokenFederationCredsAsync(out string commonAccessToken, out string iisLogMsg, IAsyncResult ar)
		{
			CommonAccessToken commonAccessToken2 = this.tokenToReturn;
			commonAccessToken = null;
			AuthStatus result = (AuthStatus)((int)this.EndLogonUserFederationCredsAsync(out iisLogMsg, ar));
			switch (result)
			{
			case AuthStatus.Redirect:
			case AuthStatus.LogonSuccess:
				if (commonAccessToken2 == null)
				{
					string memberName = (this.userType == UserType.OutlookCom && this.rpsUserProfile != null) ? this.rpsUserProfile.MemberName : this.userName;
					LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = LiveIdBasicTokenAccessor.Create(this.puid, memberName);
					commonAccessToken2 = liveIdBasicTokenAccessor.GetToken();
				}
				else
				{
					commonAccessToken2.ExtensionData["Puid"] = this.puid;
				}
				if (AuthService.IsBackendServer)
				{
					commonAccessToken2.ExtensionData["UserType"] = this.userType.ToString();
					commonAccessToken2.ExtensionData["CreateTime"] = this.lastAuthTime.ToString();
					if (this.userType == UserType.OutlookCom && this.rpsUserProfile != null && this.rpsUserProfile.Profile != null)
					{
						commonAccessToken2.ExtensionData["FirstName"] = this.rpsUserProfile.Profile.FirstName;
						commonAccessToken2.ExtensionData["LastName"] = this.rpsUserProfile.Profile.LastName;
						commonAccessToken2.ExtensionData["Language"] = this.rpsUserProfile.Profile.Language.ToString();
						commonAccessToken2.ExtensionData["Region"] = this.rpsUserProfile.Profile.Region.ToString();
						commonAccessToken2.ExtensionData["CurrentAlias"] = this.userName;
					}
				}
				if (!string.IsNullOrEmpty(this.compactToken))
				{
					commonAccessToken2.ExtensionData["CompactTicket"] = this.compactToken;
				}
				if (!string.IsNullOrEmpty(this.organizationContext))
				{
					commonAccessToken2.ExtensionData["OrganizationContext"] = this.organizationContext;
				}
				if (this.requestPasswordConfidenceCheckInBackend)
				{
					commonAccessToken2.ExtensionData["CheckPasswordConfidence"] = "true";
					commonAccessToken2.ExtensionData["PasswordConfidenceInDays"] = this.passwordConfidence.ToString();
					goto IL_225;
				}
				goto IL_225;
			}
			commonAccessToken2 = null;
			IL_225:
			if (commonAccessToken2 != null)
			{
				if (!this.passwordAndHrdSync && !this.isOrgIdOffline)
				{
					if (this.requestUpdateHrdInBackend)
					{
						this.TraceAndReturnInformation(this.GetHashCode(), "<HRD_IN_CAT>", new object[0]);
						commonAccessToken2.ExtensionData["SyncHRD"] = this.requestUpdateHrdInBackend.ToString();
						this.TraceAndReturnInformation(this.GetHashCode(), "<PW_IN_CAT>", new object[0]);
						commonAccessToken2.ExtensionData["PasswordToSync"] = Encoding.Default.GetString(this.ansiPassword);
					}
					else if (this.includePWinCat)
					{
						this.TraceAndReturnInformation(this.GetHashCode(), "<PW_IN_CAT>", new object[0]);
						commonAccessToken2.ExtensionData["PasswordToSync"] = Encoding.Default.GetString(this.ansiPassword);
					}
				}
				commonAccessToken = commonAccessToken2.Serialize();
			}
			Array.Clear(this.ansiPassword, 0, this.ansiPassword.Length);
			iisLogMsg = this.iisLogMsg.ToString();
			return result;
		}

		public bool IsNego2AuthEnabledForDomain(string domain)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering IsNego2AuthEnabledForDomain()");
			IAsyncResult asyncResult = this.BeginIsNego2AuthEnabledForDomainAsync(domain, null, null);
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)asyncResult;
			lazyAsyncResult.InternalWaitForCompletion();
			bool result = this.EndIsNego2AuthEnabledForDomainAsync(asyncResult);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving IsNego2AuthEnabledForDomain()");
			return result;
		}

		public IAsyncResult BeginIsNego2AuthEnabledForDomainAsync(string domain, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering BeginIsNego2AuthEnabledForDomainAsync()");
			ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Checking Nego2 setting for '{0}'", domain);
			this.outerResult = new LazyAsyncResult(this, state, callback);
			int num = -1;
			try
			{
				AuthService.counters.NumberOfTenantNegoRequests.Increment();
				AuthService.counters.TenantNegoCacheSize.RawValue = (long)AuthService.hrdCache.CountNegoSettings;
				if (string.IsNullOrEmpty(domain))
				{
					throw new InvalidOperationException();
				}
				if (!AuthService.hrdCache.TryGetNegoSetting(domain, out num))
				{
					num = -1;
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "domain namespace '{0}' not found in cache", domain);
					AuthService.counters.TenantNegoCacheHitBase.Increment();
					AuthService.counters.NumberOfMServRequests.Increment();
					AuthService.counters.PendingMServRequests.Increment();
					Stopwatch stopwatch = Stopwatch.StartNew();
					try
					{
						ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "Accessing MSERV NegoConfig for '{0}'", domain);
						num = EdgeSyncMservConnector.GetMserveEntryTenantNegoConfig(domain);
					}
					catch (Exception ex)
					{
						AuthService.counters.FailedMServRequests.Increment();
						AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, domain, new object[]
						{
							domain,
							ex.ToString()
						});
						throw;
					}
					finally
					{
						stopwatch.Stop();
						AuthService.counters.AverageMServResponseTime.IncrementBy(stopwatch.ElapsedMilliseconds);
						AuthService.counters.AverageMServResponseTimeBase.Increment();
						AuthService.counters.PendingMServRequests.Decrement();
						ExTraceGlobals.AuthenticationTracer.TraceDebug<long>((long)this.GetHashCode(), "MSERV responded in {0}ms", stopwatch.ElapsedMilliseconds);
					}
					AuthService.hrdCache.InsertNegoSetting(domain, num);
					AuthService.counters.TenantNegoCacheSize.RawValue = (long)AuthService.hrdCache.CountNegoSettings;
				}
				else
				{
					AuthService.counters.TenantNegoCacheHitBase.Increment();
					AuthService.counters.TenantNegoCacheHit.Increment();
				}
				bool flag = this.ParseNego2Config(num);
				ExTraceGlobals.AuthenticationTracer.Information<string, int, bool>((long)this.GetHashCode(), "MSERV setting for '{0}' is {1}, returning {2}", domain, num, flag);
				this.outerResult.InvokeCallback(flag);
			}
			catch (ArgumentException ex2)
			{
				throw new FaultException<ArgumentException>(ex2, new FaultReason(ex2.ToString()), new FaultCode("Sender"));
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving BeginIsNego2AuthEnabledForDomainAsync()");
			return this.outerResult;
		}

		public bool EndIsNego2AuthEnabledForDomainAsync(IAsyncResult result)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering EndIsNego2AuthEnabledForDomainAsync()");
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
			if (lazyAsyncResult.EndCalled)
			{
				this.TraceAndReturnError(this.GetHashCode(), "improper use of EndIsNego2AuthEnabledForDomainAsync - End called already.", new object[0]);
				throw new InvalidOperationException();
			}
			lazyAsyncResult.EndCalled = true;
			Exception ex = lazyAsyncResult.Result as Exception;
			if (ex != null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<string>((long)this.GetHashCode(), "Exception thrown {0}", ex.ToString());
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving EndIsNego2AuthEnabledForDomainAsync()");
				throw ex;
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving EndIsNego2AuthEnabledForDomainAsync()");
			return (bool)lazyAsyncResult.Result;
		}

		private bool ParseNego2Config(int negoConfig)
		{
			bool result = this.config.defaultTenantNegoConfig;
			if (negoConfig != -1)
			{
				result = (negoConfig != 0);
			}
			return result;
		}

		private void InvokeCallback(object value)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering InvokeCallback()");
			if (Interlocked.Exchange(ref this.hrdPending, 0) == 1)
			{
				AuthService.counters.PendingHrdRequests.Decrement();
			}
			if (Interlocked.Exchange(ref this.fedStsPending, 0) == 1)
			{
				AuthService.counters.PendingFedStsRequests.Decrement();
			}
			if (Interlocked.Exchange(ref this.samlStsPending, 0) == 1)
			{
				AuthService.counters.PendingSamlStsRequests.Decrement();
			}
			if (Interlocked.Exchange(ref this.liveStsPending, 0) == 1 && this.liveIdSTS != null)
			{
				AuthService.DecrementPendingStsCounter(this.liveIdSTS.Instance);
			}
			if (Interlocked.Increment(ref this.intCompleted) == 1)
			{
				try
				{
					AuthService.percentileFailedAuthenticationsLastMinutes.AddDenominator(1L);
					AuthService.percentileFailedRequestsLastMinutes.AddDenominator(1L);
					AuthService.percentileTimedOutRequestsLastMinutes.AddDenominator(1L);
					this.timer.Dispose();
					this.timer = null;
					if (this.hrdSetEventOnExit)
					{
						this.clientOp.HrdEvent.Set();
					}
					if (this.stsSetEventOnExit)
					{
						this.clientOp.StsEvent.Set();
					}
					AuthService.userOperations.ReleaseOperation(this.clientOp);
					this.releaseClientOp = false;
					Interlocked.Decrement(ref AuthService.numberOfCurrentRequests);
					AuthService.counters.NumberOfCurrentRequests.RawValue = (long)AuthService.numberOfCurrentRequests;
					if (ExTraceGlobals.AuthenticationTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.AuthenticationTracer.Information((long)this.GetHashCode(), "Logon finished for clientProcessId={0}, ansiUserName={1}, password={2}, syncAD={3}", new object[]
						{
							this.clientProcessId,
							(this.ansiUserName != null) ? this.userName : "<null>",
							(this.ansiPassword != null) ? "********" : "<null>",
							this.syncAD
						});
					}
					if (this.userType == UserType.Federated)
					{
						this.TraceAndReturnInformation(this.GetHashCode(), "<FEDERATED>", new object[0]);
					}
					this.TraceAndReturnInformation(this.GetHashCode(), "<UserType:{0}>", new object[]
					{
						this.userType
					});
					Exception ex = value as Exception;
					if (ex != null)
					{
						Interlocked.Increment(ref AuthService.numberOfFailedRequests);
						AuthService.counters.NumberOfFailedRequests.RawValue = (long)AuthService.numberOfFailedRequests;
						AuthService.percentileFailedRequestsLastMinutes.AddNumerator(1L);
						if (ex is WebException || ex is HttpException || ex is XmlException || ex is SocketException || ex is IOException)
						{
							WebException ex2 = ex as WebException;
							bool flag = false;
							if (ex2 != null)
							{
								WebExceptionStatus status = ex2.Status;
								WebExceptionStatus webExceptionStatus = status;
								switch (webExceptionStatus)
								{
								case WebExceptionStatus.NameResolutionFailure:
								case WebExceptionStatus.ConnectFailure:
									flag = true;
									break;
								default:
									if (webExceptionStatus != WebExceptionStatus.ProtocolError)
									{
										flag = false;
									}
									else
									{
										if (ex2.Response != null && ex2.Response.Headers != null)
										{
											string text = ex2.Response.Headers.Get("PPServer");
											if (!string.IsNullOrEmpty(text))
											{
												this.TraceAndReturnWarning(this.GetHashCode(), "<ppserver={0}>", new object[]
												{
													text
												});
											}
										}
										flag = false;
									}
									break;
								}
							}
							if (this.IsLiveSTSState())
							{
								string text2 = this.liveIdSTS.ErrorString;
								if (string.IsNullOrEmpty(text2))
								{
									text2 = string.Format("{0}-{1}-{2}&CallStack={3}", new object[]
									{
										ex.GetType().ToString(),
										ex.Message,
										(ex.InnerException != null) ? ex.InnerException.Message : string.Empty,
										ex.StackTrace
									});
								}
								this.TraceAndReturnWarning(this.GetHashCode(), "LiveID STS '{0}' is unreachable.  PPServer '{1}' User \"{2}\" &ExceptionDetails={3}&", new object[]
								{
									this.liveIdSTS.LogonUri,
									this.liveIdSTS.LiveServer,
									this.userName,
									text2
								});
								AuthService.eventLogger.LogEvent(flag ? SecurityEventLogConstants.Tuple_LiveIdServerUnreachableKHI : SecurityEventLogConstants.Tuple_LiveIdServerUnreachable, flag ? "LiveIdServerUnreachableKHI" : "LiveIdServerUnreachable", new object[]
								{
									this.liveIdSTS.LogonUri,
									this.userName,
									text2
								});
								value = (IntPtr)((long)this.TarpittedAuthError(AuthStatus.LiveIDFailed, this.namespaceStats.IsTarpitted));
							}
							else if (this.IsHomeRealmDiscoveryState())
							{
								if (!string.IsNullOrEmpty(this.realmDiscovery.ErrorString))
								{
									this.TraceAndReturnWarning(this.GetHashCode(), "Home Realm Discovery error.  PPServer '{0}' &ExceptionDetails={1}&", new object[]
									{
										this.realmDiscovery.LiveServer,
										this.realmDiscovery.ErrorString
									});
								}
								else
								{
									this.TraceAndReturnWarning(this.GetHashCode(), "Home Realm Discovery endpoint '{0}' is unreachable. PPServer '{1}'  User \"{2}\" &ExceptionDetails={3}&", new object[]
									{
										this.realmDiscovery.RealmDiscoveryUri,
										this.realmDiscovery.LiveServer,
										this.userName,
										ex
									});
									AuthService.eventLogger.LogEvent(flag ? SecurityEventLogConstants.Tuple_CannotConnectToHomeRealmDiscoveryKHI : SecurityEventLogConstants.Tuple_CannotConnectToHomeRealmDiscovery, flag ? "CannotConnectToHomeRealmDiscoveryKHI" : "CannotConnectToHomeRealmDiscovery", new object[]
									{
										this.realmDiscovery.RealmDiscoveryUri,
										this.userName,
										string.Format("{0}-{1}-{2}&CallStack={3}", new object[]
										{
											ex.GetType().ToString(),
											ex.Message,
											(ex.InnerException != null) ? ex.InnerException.Message : string.Empty,
											ex.StackTrace
										})
									});
								}
								value = (IntPtr)((long)this.TarpittedAuthError(AuthStatus.HRDFailed, this.namespaceStats.IsTarpitted));
							}
							else if (this.IsFederatedSTSState())
							{
								string text3 = this.federatedSTS.ErrorString;
								if (string.IsNullOrEmpty(text3))
								{
									text3 = string.Format("{0}-{1}-{2}&CallStack={3}", new object[]
									{
										ex.GetType().ToString(),
										ex.Message,
										(ex.InnerException != null) ? ex.InnerException.Message : string.Empty,
										ex.StackTrace
									});
								}
								this.TraceAndReturnWarning(this.GetHashCode(), "Federated STS '{0}' is unreachable.  User \"{1}\" &ExceptionDetails={2}&", new object[]
								{
									this.federatedSTS.FederatedLogonURI,
									this.userName,
									text3
								});
								AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_FederatedSTSUnreachable, this.federatedSTS.FederatedLogonURI, new object[]
								{
									this.federatedSTS.FederatedLogonURI,
									this.userName,
									text3
								});
								value = (IntPtr)((long)this.TarpittedAuthError(AuthStatus.FederatedStsFailed, this.namespaceStats.IsTarpitted));
							}
							else if (this.IsSamlSTSState())
							{
								string text4 = this.samlSTS.ErrorString;
								if (string.IsNullOrEmpty(text4))
								{
									text4 = string.Format("{0}-{1}-{2}&CallStack={3}", new object[]
									{
										ex.GetType().ToString(),
										ex.Message,
										(ex.InnerException != null) ? ex.InnerException.Message : string.Empty,
										ex.StackTrace
									});
								}
								this.TraceAndReturnWarning(this.GetHashCode(), "Shibboleth STS '{0}' is unreachable.  User \"{1}\" &ExceptionDetails={2}&", new object[]
								{
									this.samlSTS.ShibbolethLogonURI,
									this.userName,
									text4
								});
								AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_ShibbolethSTSUnreachable, this.samlSTS.ShibbolethLogonURI, new object[]
								{
									this.samlSTS.ShibbolethLogonURI,
									this.userName,
									text4
								});
								value = (IntPtr)((long)this.TarpittedAuthError(AuthStatus.FederatedStsFailed, this.namespaceStats.IsTarpitted));
							}
						}
						else if (value is ADHrdException)
						{
							string arg = value.ToString();
							if (this.ReasonOfUsingOfflineOrgId != null)
							{
								this.TraceAndReturnInformation(this.GetHashCode(), "<OfflineOrgIdAuthResult={0}>", new object[]
								{
									AuthStatus.OfflineHrdFailed
								});
								value = (IntPtr)((long)this.ReasonOfUsingOfflineOrgId.Value);
							}
							else
							{
								value = (IntPtr)(-21L);
							}
							this.TraceAndReturnInformation(this.GetHashCode(), string.Format("<OfflineHrd failed>{0}{1}", "AuthenticatedBy:OfflineOrgId.", arg), new object[0]);
						}
						else
						{
							this.TraceAndReturnError(this.GetHashCode(), "Exception thrown while processing logon for \"{0}\".  &ExceptionDetails={1}&", new object[]
							{
								this.userName,
								string.Format("{0}-{1}-{2}&CallStack={3}", new object[]
								{
									ex.GetType().ToString(),
									ex.Message,
									(ex.InnerException != null) ? ex.InnerException.Message : string.Empty,
									ex.StackTrace
								})
							});
							AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.userName, new object[]
							{
								this.userName,
								string.Format("{0}-{1}-{2}&CallStack={3}", new object[]
								{
									ex.GetType().ToString(),
									ex.Message,
									(ex.InnerException != null) ? ex.InnerException.Message : string.Empty,
									ex.StackTrace
								})
							});
							value = (IntPtr)((long)this.TarpittedAuthError(AuthStatus.InternalServerError, true));
						}
						this.outerResult.InvokeCallback(value);
						ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving InvokeCallback()");
						return;
					}
					TimerCallback timerCallback = value as TimerCallback;
					if (timerCallback != null)
					{
						string text5;
						if (this.IsFederatedSTSState())
						{
							if (this.nextState == AuthService.authStateEnum.authSendLiveSTSRequest)
							{
								text5 = this.liveIdSTS.LogonUri;
							}
							else
							{
								text5 = this.federatedSTS.FederatedLogonURI;
							}
						}
						else if (this.IsSamlSTSState())
						{
							text5 = this.samlSTS.ShibbolethLogonURI;
						}
						else if (this.IsHomeRealmDiscoveryState())
						{
							text5 = this.realmDiscovery.RealmDiscoveryUri;
						}
						else if (this.IsLiveSTSState())
						{
							text5 = this.liveIdSTS.LogonUri;
						}
						else if (this.IsWaitState())
						{
							text5 = "wait on other thread";
						}
						else
						{
							text5 = "end logon";
						}
						this.TraceAndReturnWarning(this.GetHashCode(), "Operation to '{0}' timed out after '{1}' s, State {2}.", new object[]
						{
							text5,
							this.Timeout,
							this.authState
						});
						Interlocked.Increment(ref AuthService.numberOfTimedOutRequests);
						AuthService.counters.NumberOfTimedOutRequests.RawValue = (long)AuthService.numberOfTimedOutRequests;
						AuthService.percentileTimedOutRequestsLastMinutes.AddNumerator(1L);
						Interlocked.Increment(ref this.namespaceStats.TimedOut);
						this.namespaceStats.User = this.userName;
						if (this.liveIdSTS != null)
						{
							this.liveIdSTS.Abort();
						}
						if (this.realmDiscovery != null)
						{
							this.realmDiscovery.Abort();
						}
						if (this.federatedSTS != null)
						{
							this.federatedSTS.Abort();
						}
						if (this.samlSTS != null)
						{
							this.samlSTS.Abort();
						}
						try
						{
							this.TraceAndReturnInformation(this.GetHashCode(), "<Timeout>", new object[0]);
							if (this.allowOfflineOrgId && !this.IsOfflineOrgIdState())
							{
								this.ReasonOfUsingOfflineOrgId = new AuthStatus?((this.IsFederatedSTSState() || this.IsSamlSTSState()) ? AuthStatus.FederatedStsFailed : (this.IsHomeRealmDiscoveryState() ? AuthStatus.HRDFailed : AuthStatus.LiveIDFailed));
								this.hasFailedOverForAuth = true;
								value = this.DoOfflineOrgIdAuth();
							}
							else if (this.IsOfflineOrgIdState())
							{
								this.offlineOrgIdDoneEvent.WaitOne(this.config.OfflineOrgIdWaitTimeInSeconds * 1000);
								value = this.offlineOrgIdResult;
							}
							if (this.ReasonOfUsingOfflineOrgId != null && ((IntPtr)value).ToInt32() < 1)
							{
								this.TraceAndReturnInformation(this.GetHashCode(), "<OfflineOrgIdAuthResult={0}>", new object[]
								{
									(AuthStatus)((IntPtr)value).ToInt32()
								});
								value = (IntPtr)((long)this.ReasonOfUsingOfflineOrgId.Value);
							}
						}
						catch (Exception ex3)
						{
							this.TraceAndReturnInformation(this.GetHashCode(), ex3.Message, new object[0]);
						}
						finally
						{
							AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_RequestTimeout, "Timeout" + this.userName, new object[]
							{
								this.iisLogMsg.ToString(),
								this.userName,
								this.stopwatch.ElapsedMilliseconds
							});
						}
						this.outerResult.InvokeCallback(value);
						ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving InvokeCallback()");
						return;
					}
					int num = ((IntPtr)value).ToInt32();
					switch (num)
					{
					case -28:
					case -21:
					case -19:
					case -18:
					case -17:
					case -16:
					case -14:
					case -13:
					case -12:
					case -6:
					case 0:
						if (this.isKnownBadCreds || (this.liveIdSTS != null && this.liveIdSTS.IsBadCredentials) || (this.federatedSTS != null && this.federatedSTS.IsBadCredentials) || (this.samlSTS != null && this.samlSTS.IsBadCredentials))
						{
							this.TraceAndReturnInformation(this.GetHashCode(), "Logon failed \"{0}\" - bad password.", new object[]
							{
								this.userName
							});
							AuthService.counters.NumberOfInvalidCredentials.Increment();
						}
						else
						{
							this.TraceAndReturnInformation(this.GetHashCode(), "Logon failed \"{0}\".", new object[]
							{
								this.userName
							});
							Interlocked.Increment(ref AuthService.numberOfFailedAuthentications);
							AuthService.counters.NumberOfFailedAuthentications.RawValue = (long)AuthService.numberOfFailedAuthentications;
							AuthService.percentileFailedAuthenticationsLastMinutes.AddNumerator(1L);
						}
						if (this.ReasonOfUsingOfflineOrgId != null)
						{
							this.TraceAndReturnInformation(this.GetHashCode(), "<OfflineOrgIdAuthResult={0}>", new object[]
							{
								(AuthStatus)num
							});
							value = (IntPtr)((long)this.ReasonOfUsingOfflineOrgId.Value);
							goto IL_10E0;
						}
						goto IL_10E0;
					case -26:
					case -25:
					case -24:
					case -22:
						this.TraceAndReturnInformation(this.GetHashCode(), "Logon failed \"{0}\".", new object[]
						{
							this.userName
						});
						Interlocked.Increment(ref AuthService.numberOfFailedAuthentications);
						AuthService.counters.NumberOfFailedAuthentications.RawValue = (long)AuthService.numberOfFailedAuthentications;
						AuthService.percentileFailedAuthenticationsLastMinutes.AddNumerator(1L);
						goto IL_10E0;
					case -15:
					case -8:
					case -7:
					case -3:
					case -2:
						this.TraceAndReturnInformation(this.GetHashCode(), "Logon failed \"{0}\".", new object[]
						{
							this.userName
						});
						Interlocked.Increment(ref AuthService.numberOfFailedRequests);
						AuthService.counters.NumberOfFailedRequests.RawValue = (long)AuthService.numberOfFailedRequests;
						AuthService.percentileFailedRequestsLastMinutes.AddNumerator(1L);
						goto IL_10E0;
					case -11:
						AuthService.counters.NumberOfLowConfidenceOfflineOrgIdRequests.Increment();
						Interlocked.Increment(ref AuthService.numberOfFailedAuthentications);
						AuthService.counters.NumberOfFailedAuthentications.RawValue = (long)AuthService.numberOfFailedAuthentications;
						AuthService.percentileFailedAuthenticationsLastMinutes.AddNumerator(1L);
						if (this.ReasonOfUsingOfflineOrgId != null)
						{
							this.TraceAndReturnInformation(this.GetHashCode(), "<OfflineOrgIdAuthResult={0}>", new object[]
							{
								AuthStatus.LowConfidence
							});
							value = (IntPtr)((long)this.ReasonOfUsingOfflineOrgId.Value);
							goto IL_10E0;
						}
						goto IL_10E0;
					case -10:
					case -5:
						this.TraceAndReturnInformation(this.GetHashCode(), "Logon failed \"{0}\" - expired.", new object[]
						{
							this.userName
						});
						Interlocked.Increment(ref AuthService.numberOfFailedRecoverableAuthentications);
						AuthService.counters.NumberOfFailedRecoverableAuthentications.RawValue = (long)AuthService.numberOfFailedRecoverableAuthentications;
						goto IL_10E0;
					case -9:
					case -4:
						this.TraceAndReturnInformation(this.GetHashCode(), "Logon failed \"{0}\" - recoverable.", new object[]
						{
							this.userName
						});
						Interlocked.Increment(ref AuthService.numberOfFailedRecoverableAuthentications);
						AuthService.counters.NumberOfFailedRecoverableAuthentications.RawValue = (long)AuthService.numberOfFailedRecoverableAuthentications;
						goto IL_10E0;
					case -1:
						this.TraceAndReturnInformation(this.GetHashCode(), "LiveID logon succeeded \"{0}\" - need redirect.", new object[]
						{
							this.userName
						});
						Interlocked.Increment(ref AuthService.numberOfSuccessfulAuthentications);
						AuthService.counters.NumberOfSuccessfulAuthentications.RawValue = (long)AuthService.numberOfSuccessfulAuthentications;
						goto IL_10E0;
					}
					ExTraceGlobals.AuthenticationTracer.Information((long)this.GetHashCode(), "Logon succeeded.");
					Interlocked.Increment(ref AuthService.numberOfSuccessfulAuthentications);
					AuthService.counters.NumberOfSuccessfulAuthentications.RawValue = (long)AuthService.numberOfSuccessfulAuthentications;
					IL_10E0:
					this.outerResult.InvokeCallback(value);
				}
				finally
				{
					AuthService.hrdCache.EvaluateStatistics(this.namespaceInfo, this.namespaceStats, this.GetHashCode());
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving InvokeCallback()");
		}

		private static void TimeOutCallback(object state)
		{
			AuthService authService = (AuthService)state;
			authService.InvokeCallback(AuthService.timerCallback);
		}

		private static void RequestCallback(IAsyncResult asyncResult)
		{
			AuthService authService = (AuthService)asyncResult.AsyncState;
			if (authService.extraWait != 0)
			{
				int num = authService.extraWait;
				authService.extraWait = 0;
				authService.waitTimer = new Timer(new TimerCallback(AuthService.ClientDelayCallback), asyncResult, TimeSpan.FromMilliseconds((double)num), TimeSpan.Zero);
				return;
			}
			bool flag = (!authService.hasFailedOverForAuth || !authService.hasFailedOverForHrd) && authService.allowOfflineOrgId;
			try
			{
				authService.authState = authService.nextState;
				authService.nextState = AuthService.authStateEnum.authUnknown;
				TestFailoverFlags testFailoverFlags = authService.testFailOver;
				if (flag && testFailoverFlags != TestFailoverFlags.None)
				{
					authService.StartTestFailover();
				}
				switch (authService.authState)
				{
				case AuthService.authStateEnum.authSendHrdRequest:
					authService.ProcessHrdRequest(asyncResult);
					break;
				case AuthService.authStateEnum.authProcessHrdResponse:
					authService.ProcessHrdResponse(asyncResult);
					break;
				case AuthService.authStateEnum.authSendLiveSTSRequest:
					authService.ProcessLiveRequest(asyncResult);
					break;
				case AuthService.authStateEnum.authProcessLiveSTSResponse:
					authService.ProcessLiveResponse(asyncResult);
					break;
				case AuthService.authStateEnum.authSendFedSTSRequest:
					authService.ProcessFederatedSTSRequest(asyncResult);
					break;
				case AuthService.authStateEnum.authProcessFedSTSResponse:
					authService.ProcessFederatedSTSResponse(asyncResult);
					break;
				case AuthService.authStateEnum.authSendLiveFedRequest:
					authService.ProcessLiveRequest(asyncResult);
					break;
				case AuthService.authStateEnum.authProcessLiveFedResponse:
					authService.ProcessLiveResponse(asyncResult);
					break;
				case AuthService.authStateEnum.authSendSamlSTSRequest:
					authService.ProcessSamlSTSRequest(asyncResult);
					break;
				case AuthService.authStateEnum.authProcessSamlSTSResponse:
					authService.ProcessSamlSTSResponse(asyncResult);
					break;
				case AuthService.authStateEnum.authSendLiveSamlRequest:
					authService.ProcessLiveRequest(asyncResult);
					break;
				case AuthService.authStateEnum.authProcessLiveSamlResponse:
					authService.ProcessLiveResponse(asyncResult);
					break;
				}
			}
			catch (Exception ex)
			{
				Interlocked.Increment(ref authService.namespaceStats.Failed);
				authService.namespaceStats.User = authService.userName;
				if (!flag || !authService.HandleFailOver(ex))
				{
					authService.InvokeCallback(ex);
				}
			}
		}

		private static void ClientDelayCallback(object state)
		{
			IAsyncResult asyncResult = (IAsyncResult)state;
			AuthService authService = (AuthService)asyncResult.AsyncState;
			authService.waitTimer.Dispose();
			authService.waitTimer = null;
			AuthService.RequestCallback((IAsyncResult)state);
		}

		private static void ClientOpWaitCallback(object state, bool timeout)
		{
			AuthService authService = (AuthService)state;
			try
			{
				if (authService.asyncWaitHandle != null)
				{
					authService.asyncWaitHandle.Unregister(null);
					authService.asyncWaitHandle = null;
				}
				if (!timeout)
				{
					authService.authState = authService.nextState;
					authService.nextState = AuthService.authStateEnum.authUnknown;
					switch (authService.authState)
					{
					case AuthService.authStateEnum.authWaitHrdResponse:
						authService.ResumeHrdRequestChain();
						break;
					case AuthService.authStateEnum.authWaitLogonResponse:
						authService.ResumeLogonRequestChain();
						break;
					}
				}
			}
			catch (Exception value)
			{
				authService.InvokeCallback(value);
			}
		}

		private static byte[] CreateWsseUsernameToken(byte[] memberName, byte[] password)
		{
			int num = memberName.Length + password.Length;
			byte[] array = new byte[num + AuthService.wsseTokenByteCount];
			using (Stream stream = new MemoryStream(array))
			{
				AuthService.WriteBytes(stream, AuthService.wsseTokenBytesP1);
				AuthService.WriteBytes(stream, memberName);
				AuthService.WriteBytes(stream, AuthService.wsseTokenBytesP2);
				AuthService.WriteBytes(stream, password);
				AuthService.WriteBytes(stream, AuthService.wsseTokenBytesP3);
				stream.Close();
			}
			return array;
		}

		private static byte[] CleanBytes(byte[] inputbytes, bool clearInput)
		{
			return AuthService.CleanBytes(inputbytes, clearInput, int.MaxValue);
		}

		private static byte[] CleanBytes(byte[] inputbytes, bool clearInput, int maxSize)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(inputbytes))
			{
				using (MemoryStream memoryStream2 = new MemoryStream((inputbytes.Length + 1) * 5))
				{
					using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.Default))
					{
						XmlWriter xmlWriter = XmlWriter.Create(memoryStream2, AuthService.xmlSettings);
						int num = 0;
						char[] array = new char[1];
						while (streamReader.Read(array, 0, 1) > 0 && num < maxSize)
						{
							if (array[0] != '\0')
							{
								num++;
								xmlWriter.WriteChars(array, 0, 1);
								xmlWriter.Flush();
							}
						}
						xmlWriter.Close();
						byte[] array2 = new byte[memoryStream2.Length];
						Array.Copy(memoryStream2.GetBuffer(), array2, (int)memoryStream2.Length);
						Array.Clear(memoryStream2.GetBuffer(), 0, (int)memoryStream2.Length);
						if (clearInput)
						{
							Array.Clear(inputbytes, 0, inputbytes.Length);
						}
						result = array2;
					}
				}
			}
			return result;
		}

		private static int GetPasswordConfidenceConfig(bool isFederated)
		{
			if (!isFederated)
			{
				return AuthServiceStaticConfig.Config.PasswordConfidenceInDays;
			}
			return AuthServiceStaticConfig.Config.PasswordConfidenceInDaysForADFSDown;
		}

		private bool CheckLogonCache()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering CheckLogonCache()");
			AuthService.percentageLogonCacheHitLastMinute.AddDenominator(1L);
			string userKey = this.UserKey;
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "User key='{0}' hash='{1}'", userKey, BitConverter.ToString(this.passwordHash).Replace("-", ""));
			HashedCredInfo hashedCredInfo;
			bool flag;
			if (AuthService.logonCache.CheckBadPassword(userKey, this.passwordHash, out hashedCredInfo, out flag))
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "User key {0} found in bad logon cache repeated={1}", userKey, flag);
				AuthService.percentageLogonCacheHitLastMinute.AddNumerator(1L);
				this.isKnownBadCreds = true;
				if (!flag)
				{
					AuthService.logonCache.Add(userKey, hashedCredInfo.Time, hashedCredInfo.Hash, hashedCredInfo.Mode, hashedCredInfo.Tag, hashedCredInfo.UserType, this.GetHashCode());
				}
				this.userType = hashedCredInfo.UserType;
				Interlocked.Increment(ref this.namespaceStats.Count);
				this.TraceAndReturnInformation(this.GetHashCode(), "<CACHE FAIL> creds {0}.", new object[]
				{
					hashedCredInfo.Mode
				});
				this.InvokeCallback((IntPtr)((long)this.FailureModeToAuthError(hashedCredInfo.Mode, true)));
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving CheckLogonCache()");
				return true;
			}
			if (this.usePositiveLogonCache)
			{
				PuidCredInfo puidCredInfo;
				bool flag2 = AuthService.logonCache.TryGetEntry(userKey, this.passwordHash, out puidCredInfo);
				if (flag2 || AuthService.logonCache.TryGetEntry(this.UserKeyShortVersion, this.passwordHash, out puidCredInfo))
				{
					ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Matched creds in logon cache for {0}", this.userName);
					AuthService.percentageLogonCacheHitLastMinute.AddNumerator(1L);
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "Performing Win32.LogonUser for {0}", this.userName);
					this.puid = puidCredInfo.PUID;
					this.userType = puidCredInfo.userType;
					this.requestPasswordConfidenceCheckInBackend = puidCredInfo.RequestPasswordConfidenceCheckInBackend;
					this.passwordConfidence = AuthService.GetPasswordConfidenceConfig(puidCredInfo.userType == UserType.Federated);
					this.lastAuthTime = puidCredInfo.Time;
					if (!string.IsNullOrEmpty(puidCredInfo.Tag))
					{
						this.TraceAndReturnInformation(this.GetHashCode(), puidCredInfo.Tag, new object[0]);
					}
					if (!this.returnWindowsToken && this.syncADBackEndOnly)
					{
						this.TraceAndReturnInformation(this.GetHashCode(), "AuthenticatedBy:Cache.", new object[0]);
						Interlocked.Increment(ref this.namespaceStats.Count);
						this.InvokeCallback(new IntPtr(1));
						return true;
					}
					if (puidCredInfo.userType == UserType.OutlookCom)
					{
						return false;
					}
					ADRawEntry adrawEntry = null;
					try
					{
						this.GetADUserEntry(puidCredInfo.PUID, out adrawEntry);
						if (adrawEntry != null && this.syncAD)
						{
							string text = (string)adrawEntry[ADUserSchema.UserPrincipalName];
							if (!string.Equals(this.userName, text, StringComparison.OrdinalIgnoreCase))
							{
								ExTraceGlobals.AuthenticationTracer.TraceWarning<string, string>((long)this.GetHashCode(), "LogonCache: UPN \"{0}\" does not match Live ID \"{1}\" forcing full logon", text, this.userName);
								return false;
							}
						}
					}
					catch (DataSourceTransientException ex)
					{
						this.TraceAndReturnError(this.GetHashCode(), "ITenantRecipientSession threw exception {0}", new object[]
						{
							ex
						});
						AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_CannotConnectToAD, "CannotConnectToAD" + this.userName, new object[]
						{
							ex.Message
						});
						throw;
					}
					catch (DataSourceOperationException ex2)
					{
						this.TraceAndReturnError(this.GetHashCode(), "ITenantRecipientSession threw exception {0}", new object[]
						{
							ex2
						});
						AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_CannotConnectToAD, "CannotConnectToAD" + this.userName, new object[]
						{
							ex2.Message
						});
						throw;
					}
					if (adrawEntry != null)
					{
						Interlocked.Increment(ref this.namespaceStats.Count);
						ExTraceGlobals.AuthenticationTracer.Information<string, ADObjectId>((long)this.GetHashCode(), "Found entry for user {0} - {1}", this.userName, adrawEntry.Id);
						IntPtr intPtr = (IntPtr)0L;
						this.TraceAndReturnInformation(this.GetHashCode(), "AuthenticatedBy:Cache.", new object[0]);
						if (this.returnWindowsToken)
						{
							ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "Creating S4U token for {0}", this.userName);
							string arg = (string)adrawEntry[ADMailboxRecipientSchema.SamAccountName];
							string forestFQDN = ((OrganizationId)adrawEntry[ADObjectSchema.OrganizationId]).PartitionId.ForestFQDN;
							string upn = string.Format("{0}@{1}", arg, forestFQDN);
							if (!this.S4ULogon(upn, this.clientProcessId, out intPtr, this.GetHashCode()))
							{
								this.InvokeCallback(new IntPtr(-14));
							}
							else
							{
								this.InvokeCallback(intPtr);
							}
						}
						else
						{
							this.InvokeCallback(new IntPtr(1));
						}
						ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving CheckLogonCache()");
						return true;
					}
					this.TraceAndReturnError(this.GetHashCode(), "Logon cache user lookup failed with PUID {0} couldn't be found. User \"{1}\" might be mail disabled or mailbox disabled.", new object[]
					{
						puidCredInfo.PUID,
						this.userName
					});
					if (flag2)
					{
						AuthService.logonCache.Remove(userKey);
						goto IL_56A;
					}
					AuthService.logonCache.Remove(this.UserKeyShortVersion);
				}
				else
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "User key {0} or {1} not found in valid or invalid logon cache", userKey, this.UserKeyShortVersion);
				}
			}
			else
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "Valid Logon Cache is skipped for the request of {0}.", userKey);
			}
			IL_56A:
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving CheckLogonCache()");
			return false;
		}

		private bool GetADUserEntry(string puid, out ADRawEntry entry)
		{
			AuthService.percentageAdUserHitLastMinute.AddDenominator(1L);
			entry = null;
			bool result;
			if (!AuthService.adUserPropertyCache.TryGetValue(puid, out entry))
			{
				ITenantRecipientSession tenantRecipientSession;
				entry = this.ADUserLookup(puid, string.IsNullOrEmpty(this.organizationContext) ? this.userDomain : this.organizationContext, out tenantRecipientSession);
				if (entry != null && tenantRecipientSession != null)
				{
					OrganizationId organizationId = (OrganizationId)entry[ADObjectSchema.OrganizationId];
					if (organizationId != null && !TenantRelocationStateCache.IsTenantVolatile(organizationId.OrganizationalUnit))
					{
						AuthService.adUserPropertyCache.InsertAbsolute(puid, entry, TimeSpan.FromMinutes((double)this.config.ADUserEntryExpireTimeInMinutes), null);
					}
				}
				result = false;
			}
			else
			{
				AuthService.percentageAdUserHitLastMinute.AddNumerator(1L);
				result = true;
			}
			if (entry != null && !this.returnWindowsToken && !this.syncADBackEndOnly)
			{
				this.tokenToReturn = LiveIdBasicTokenAccessor.Create(entry).GetToken();
			}
			return result;
		}

		private ADRawEntry ADUserLookup(string puid, string context, out ITenantRecipientSession recipientSession)
		{
			ADRawEntry entry = null;
			recipientSession = null;
			if (!string.IsNullOrEmpty(puid))
			{
				try
				{
					using (new ActivityScopeThreadGuard(this.activityScope))
					{
						ITenantRecipientSession tempRecipientSession = null;
						long latency = AuthServiceHelper.ExecuteAndRecordLatency(delegate
						{
							tempRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromTenantAcceptedDomain(context), 3008, "ADUserLookup", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\AuthService.cs");
							ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.GetHashCode(), "Searching GC {0} for PUID {1}", tempRecipientSession.DomainController ?? "<null>", puid);
						});
						recipientSession = tempRecipientSession;
						this.RecordLatency("CreateTenantRecipientSession", latency);
						latency = AuthServiceHelper.ExecuteAndRecordLatency(delegate
						{
							entry = AuthService.ADUserLookup(tempRecipientSession, puid, this.userName, context, this.userDomain, this.GetHashCode());
						});
						this.RecordLatency("ADUserLookup", latency);
					}
					if (entry != null)
					{
						string forestFQDN = recipientSession.SessionSettings.PartitionId.ForestFQDN;
					}
				}
				catch (CannotResolveTenantNameException)
				{
					ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Could not resolve recipient session for tenant '{0}'", context);
				}
				catch (CannotResolvePartitionException)
				{
					ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Could not resolve recipient session for domain '{0}'", context);
				}
			}
			return entry;
		}

		private bool SyncPasswordUpnRetrieveToken(ADRawEntry entry, string implicitUpn, bool returnWindowsToken, ref IntPtr clientProcessUserToken)
		{
			string upn = (string)entry[ADUserSchema.UserPrincipalName];
			ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.GetHashCode(), "Performing password sync in AD for user {0} with UPN {1}", this.userName, upn);
			ITenantRecipientSession writableSession = null;
			if (this.syncUPN && !string.Equals(this.userName, upn, StringComparison.OrdinalIgnoreCase))
			{
				if (!this.passwordAndHrdSync && this.syncADBackEndOnly)
				{
					this.includePWinCat = true;
					if (!this.returnWindowsToken)
					{
						return false;
					}
				}
				else if (!string.IsNullOrEmpty(this.userDomain) && !AuthServiceHelper.IsTenantInAccountForest(this.userDomain, null))
				{
					bool opSuccess = false;
					long latency = 0L;
					try
					{
						latency = AuthServiceHelper.ExecuteAndRecordLatency(delegate
						{
							using (new ActivityScopeThreadGuard(this.activityScope))
							{
								writableSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, false, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromAllTenantsObjectId(entry.Id), 3103, "SyncPasswordUpnRetrieveToken", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\AuthService.cs");
								ExTraceGlobals.AuthenticationTracer.TraceWarning<string, string>((long)this.GetHashCode(), "Password sync: UPN {0} does not match Live ID {1}", upn, this.userName);
								AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_UpnMismatch, "UpnMismatch" + this.userName, new object[]
								{
									this.userName,
									upn
								});
								ExTraceGlobals.AuthenticationTracer.Information<string, ADObjectId>((long)this.GetHashCode(), "Setting UPN and WindowsLiveID to {0} on {1}", this.userName, entry.Id);
								if (this.CheckUpnAgainstAcceptedDomains(entry.Id, (OrganizationId)entry[ADObjectSchema.OrganizationId]))
								{
									AuthService.SetUpn(writableSession, entry.Id, this.userName);
									opSuccess = true;
								}
							}
							if (AuthService.adUserPropertyCache.Contains(this.puid))
							{
								AuthService.adUserPropertyCache.Remove(this.puid);
							}
						});
					}
					finally
					{
						this.RecordLatency("UpnSync:" + opSuccess, latency);
					}
				}
			}
			bool flag = false;
			int num = 0;
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Performing Win32.LogonUser for {0} - upn {1}", this.userName, implicitUpn);
			SafeTokenHandle safeTokenHandle = new SafeTokenHandle();
			try
			{
				byte[] bytes = Encoding.ASCII.GetBytes(implicitUpn);
				Stopwatch stopwatch = Stopwatch.StartNew();
				flag = SspiNativeMethods.LogonUser(bytes, null, this.ansiPassword, LogonType.NetworkCleartext, LogonProvider.Default, ref safeTokenHandle);
				stopwatch.Stop();
				AuthService.counters.AverageAdResponseTime.IncrementBy(stopwatch.ElapsedMilliseconds);
				AuthService.counters.AverageAdResponseTimeBase.Increment();
				AuthService.counters.NumberOfAdRequests.Increment();
				this.RecordLatency("ADLogonUser", stopwatch.ElapsedMilliseconds);
				if (!flag)
				{
					AuthService.counters.FailedAdRequests.Increment();
					num = Marshal.GetLastWin32Error();
				}
				else
				{
					ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Password sync: Win32.LogonUser succeeded, no password reset required for {0}", this.userName);
					if (returnWindowsToken)
					{
						clientProcessUserToken = this.GetTokenForClientProcess(this.clientProcessId, safeTokenHandle.DangerousGetHandle());
					}
				}
			}
			finally
			{
				safeTokenHandle.Dispose();
			}
			if (!flag)
			{
				if (num == 1326)
				{
					if (!this.passwordAndHrdSync && this.syncADBackEndOnly)
					{
						this.includePWinCat = true;
						flag = this.S4ULogon(implicitUpn, this.clientProcessId, out clientProcessUserToken, this.GetHashCode());
						return flag;
					}
					ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Password sync: resetting password for {0}", this.userName);
					char[] chars = Encoding.Default.GetChars(this.ansiPassword);
					flag = this.SetPassword(writableSession, entry.Id, chars);
					if (returnWindowsToken)
					{
						ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.GetHashCode(), "Creating WindowsIdentity token for user {0} using UPN {1} after password synced.", this.userName, implicitUpn);
						if (this.S4ULogon(implicitUpn, this.clientProcessId, out clientProcessUserToken, this.GetHashCode()))
						{
							flag = true;
						}
					}
				}
				else
				{
					this.TraceAndReturnError(this.GetHashCode(), "Win32.LogonUser failed for \"{0}\" with error {1}", new object[]
					{
						this.userName,
						num
					});
					AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_NativeApiFailed, "NativeApiFailed" + this.userName, new object[]
					{
						"LogonUser - ProcessLiveResponse",
						implicitUpn,
						num
					});
				}
			}
			if (flag)
			{
				PuidCredInfo puidCredInfo;
				bool flag2 = AuthService.logonCache.TryGetEntry(this.UserKey, this.passwordHash, out puidCredInfo);
				if (flag2)
				{
					AuthService.logonCache.Add(this.UserKey, ExDateTime.UtcNow, this.puid, this.passwordHash, AuthServiceStaticConfig.Config.logonCacheLifetime, puidCredInfo.Tag + "PasswordSynced.", this.GetHashCode(), null, puidCredInfo.userType, puidCredInfo.appPassword, false);
				}
				else if (this.useLogonCache && this.usePositiveLogonCache && this.namespaceInfo != null)
				{
					UserType userType = AuthServiceHelper.GetUserType(this.namespaceInfo);
					AuthService.logonCache.Add(this.UserKey, ExDateTime.UtcNow, this.puid, this.passwordHash, AuthServiceStaticConfig.Config.logonCacheLifetime, "PasswordSynced.", this.GetHashCode(), null, userType, false, false);
				}
			}
			return flag;
		}

		private void InvalidatePasswordInAD()
		{
			if (!AuthService.IsBackendServer)
			{
				return;
			}
			if (this.namespaceInfo.IsFederated && !AuthServiceStaticConfig.Config.EnablePasswordInvalidationForFederatedUser)
			{
				return;
			}
			if (!this.namespaceInfo.IsFederated && !AuthServiceStaticConfig.Config.EnablePasswordInvalidationForManagedUser)
			{
				return;
			}
			ADRawEntry userEntry = null;
			long latency = AuthServiceHelper.ExecuteAndRecordLatency(delegate
			{
				ITenantRecipientSession tenantRecipientSession;
				userEntry = this.GetUniqueUserInAD(out tenantRecipientSession);
			});
			this.RecordLatency("GetADUser2", latency);
			if (userEntry == null)
			{
				return;
			}
			string implicitUpn = AuthServiceHelper.GetImplicitUpn(userEntry);
			SafeTokenHandle safeTokenHandle = new SafeTokenHandle();
			try
			{
				byte[] bytes = Encoding.ASCII.GetBytes(implicitUpn);
				Stopwatch stopwatch = Stopwatch.StartNew();
				bool flag = SspiNativeMethods.LogonUser(bytes, null, this.ansiPassword, LogonType.NetworkCleartext, LogonProvider.Default, ref safeTokenHandle);
				stopwatch.Stop();
				AuthService.counters.AverageAdResponseTime.IncrementBy(stopwatch.ElapsedMilliseconds);
				AuthService.counters.AverageAdResponseTimeBase.Increment();
				AuthService.counters.NumberOfAdRequests.Increment();
				this.RecordLatency("ADLogonUser2", stopwatch.ElapsedMilliseconds);
				if (!flag)
				{
					ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Password sync: Win32.LogonUser failed, no password invalidate required for {0}", this.userName);
				}
				else
				{
					ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Password sync: Win32.LogonUser succeeded, password invalidate required for {0}", this.userName);
					char[] unicodePassword = PasswordHelper.GetRandomPassword(implicitUpn, this.userName, 16).ToCharArray();
					this.SetPassword(null, userEntry.Id, unicodePassword);
				}
			}
			catch (Exception ex)
			{
				AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.userName, new object[]
				{
					this.userName,
					ex.ToString()
				});
			}
			finally
			{
				safeTokenHandle.Dispose();
			}
		}

		private object UpdateCreds(string puid)
		{
			ADRawEntry adrawEntry = null;
			object result;
			try
			{
				PuidCredInfo puidCredInfo;
				bool flag = AuthService.logonCache.TryGetEntry(this.UserKey, this.passwordHash, out puidCredInfo);
				if (flag && puidCredInfo.Tag.Contains("PasswordSynced."))
				{
					result = new IntPtr(1);
				}
				else
				{
					this.GetADUserEntry(puid, out adrawEntry);
					if (adrawEntry != null)
					{
						OrganizationId organizationId = (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
						string arg = (string)adrawEntry[ADMailboxRecipientSchema.SamAccountName];
						string implicitUpn = string.Format("{0}@{1}", arg, organizationId.PartitionId.ForestFQDN);
						IntPtr zero = IntPtr.Zero;
						this.SyncPasswordUpnRetrieveToken(adrawEntry, implicitUpn, false, ref zero);
						result = new IntPtr(1);
					}
					else
					{
						this.TraceAndReturnError(this.GetHashCode(), "Entry for PUID {0} cannot be found in tenant {1}", new object[]
						{
							puid,
							this.organizationContext
						});
						result = new IntPtr(-20);
					}
				}
			}
			catch (DataSourceTransientException ex)
			{
				this.TraceAndReturnError(this.GetHashCode(), "ITenantRecipientSession threw exception {0}", new object[]
				{
					ex
				});
				AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_CannotConnectToAD, "CannotConnectToAD" + this.userName, new object[]
				{
					ex.Message
				});
				result = ex;
			}
			catch (DataSourceOperationException ex2)
			{
				this.TraceAndReturnError(this.GetHashCode(), "ITenantRecipientSession threw exception {0}", new object[]
				{
					ex2
				});
				AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_CannotConnectToAD, "CannotConnectToAD" + this.userName, new object[]
				{
					ex2.Message
				});
				result = ex2;
			}
			return result;
		}

		internal ADRawEntry ADUserLookupByUPN(SmtpProxyAddress proxyAddress, out ITenantRecipientSession recipientSession, out bool ambiguous)
		{
			ADRawEntry result = null;
			recipientSession = null;
			ambiguous = false;
			if (proxyAddress != null)
			{
				try
				{
					recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(string.IsNullOrEmpty(this.organizationContext) ? this.userDomain : this.organizationContext), 3451, "ADUserLookupByUPN", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\AuthService.cs");
					ExTraceGlobals.AuthenticationTracer.Information<string, SmtpProxyAddress>((long)this.GetHashCode(), "Searching GC {0} for proxyAddress {1}", recipientSession.DomainController ?? "<null>", proxyAddress);
					result = recipientSession.FindByProxyAddress(proxyAddress, AuthService.propertiesToGet);
				}
				catch (CannotResolveTenantNameException)
				{
					return null;
				}
				return result;
			}
			return result;
		}

		private static ADRawEntry ADUserLookup(ITenantRecipientSession recipientSession, string puid, string userName, string context, string userDomain, int traceId)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)traceId, "Entering ADUserLookup()");
			ADRawEntry adrawEntry = null;
			try
			{
				adrawEntry = recipientSession.FindUniqueEntryByNetID(puid, context, AuthService.propertiesToGet);
			}
			catch (NonUniqueRecipientException)
			{
				AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_LiveIDAmbiguous, userName, new object[]
				{
					puid,
					userName
				});
				throw;
			}
			if (adrawEntry != null)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)traceId, "Found 1 entry matching PUID {0} for user {1}.", puid, userName);
			}
			else
			{
				ExTraceGlobals.AuthenticationTracer.TraceWarning<string, string>((long)traceId, "Found zero entries matching PUID {0} for user {1}", puid, userName);
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)traceId, "Leaving ADUserLookup()");
			return adrawEntry;
		}

		private bool S4ULogon(string upn, uint clientProcessId, out IntPtr clientProcessUserToken, int traceid)
		{
			IntPtr tempPtr = IntPtr.Zero;
			clientProcessUserToken = (IntPtr)(-14L);
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				if (!this.OOMExceptionRetry(delegate
				{
					using (WindowsIdentity windowsIdentity = new WindowsIdentity(upn))
					{
						tempPtr = this.GetTokenForClientProcess(clientProcessId, windowsIdentity.Token);
					}
				}))
				{
					this.TraceAndReturnError(traceid, "OOM error creating windows identity for UPN \"{0}\"", new object[]
					{
						upn
					});
					return false;
				}
				clientProcessUserToken = tempPtr;
			}
			catch (SecurityException ex)
			{
				AuthService.counters.FailedAdRequests.Increment();
				this.TraceAndReturnError(this.GetHashCode(), "Couldn't create token for \"{0}\" with error {1}", new object[]
				{
					this.userName,
					ex.Message
				});
				AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.userName, new object[]
				{
					this.userName,
					ex.ToString()
				});
				return false;
			}
			finally
			{
				stopwatch.Stop();
				AuthService.counters.AverageAdResponseTime.IncrementBy(stopwatch.ElapsedMilliseconds);
				AuthService.counters.AverageAdResponseTimeBase.Increment();
				AuthService.counters.NumberOfAdRequests.Increment();
			}
			return true;
		}

		private bool SetPassword(ITenantRecipientSession writableSession, ADObjectId adObjectId, char[] unicodePassword)
		{
			long latency = -1L;
			bool opSuccess = false;
			try
			{
				using (new ActivityScopeThreadGuard(this.activityScope))
				{
					latency = AuthServiceHelper.ExecuteAndRecordLatency(delegate
					{
						if (writableSession == null)
						{
							writableSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, false, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromAllTenantsObjectId(adObjectId), 3586, "SetPassword", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\AuthService.cs");
						}
						AuthService.InternalSetPassword(writableSession, adObjectId, unicodePassword);
						opSuccess = true;
						ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Password sync: password set for {0}", this.userName);
					});
				}
			}
			finally
			{
				this.RecordLatency("SetPassword:" + opSuccess, latency);
				Array.Clear(unicodePassword, 0, unicodePassword.Length);
			}
			return opSuccess;
		}

		private static void WriteBytes(Stream stream, byte[] bytes)
		{
			stream.Write(bytes, 0, bytes.Length);
		}

		private static void InternalSetPassword(ITenantRecipientSession ads, ADObjectId id, char[] password)
		{
			SecureString secureString = new SecureString();
			try
			{
				foreach (char c in password)
				{
					secureString.AppendChar(c);
				}
				ads.SetPassword(id, secureString);
				AuthService.counters.AdPasswordSyncs.Increment();
			}
			finally
			{
				secureString.Clear();
				secureString.Dispose();
			}
		}

		private static void SetUpn(ITenantRecipientSession ads, ADObjectId id, string upn)
		{
			ADUser aduser = (ADUser)ads.Read(id);
			if (aduser == null)
			{
				throw new ADNoSuchObjectException(new LocalizedString(id.ToString()));
			}
			aduser.UserPrincipalName = upn;
			aduser.WindowsLiveID = new SmtpAddress(upn);
			ads.Save(aduser);
			AuthService.counters.AdUpnSyncs.Increment();
		}

		private IntPtr GetTokenForClientProcess(uint clientProcessId, IntPtr token)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering GetTokenForClientProcess()");
			if (clientProcessId == 4294967295U)
			{
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Returning empty token for monitoring cmdlet.");
				return new IntPtr(1);
			}
			IntPtr result = (IntPtr)(-14L);
			SafeProcessHandle safeProcessHandle = NativeMethods.OpenProcess(NativeMethods.ProcessAccess.DupHandle, false, clientProcessId);
			using (safeProcessHandle)
			{
				if (safeProcessHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					this.TraceAndReturnError(this.GetHashCode(), "Win32.OpenProcess failed for PID {0} with error 0x{1:X8}", new object[]
					{
						clientProcessId,
						lastWin32Error
					});
					AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_NativeApiFailed, "NativeApiFailed" + clientProcessId.ToString(), new object[]
					{
						"OpenProcess",
						clientProcessId,
						lastWin32Error
					});
					ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving GetTokenForClientProcess()");
					return (IntPtr)(-14L);
				}
				if (!Win32.DuplicateHandle(Win32.GetCurrentProcess(), token, safeProcessHandle, ref result, 0U, false, Win32.DuplicateHandleOptions.DUPLICATE_SAME_ACCESS))
				{
					int lastWin32Error2 = Marshal.GetLastWin32Error();
					this.TraceAndReturnError(this.GetHashCode(), "Win32.DuplicateHandle failed for PID {0} with error 0x{1:X8}", new object[]
					{
						clientProcessId,
						lastWin32Error2
					});
					AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_NativeApiFailed, "NativeApiFailed" + clientProcessId.ToString(), new object[]
					{
						"DuplicateHandle",
						clientProcessId,
						lastWin32Error2
					});
					ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving GetTokenForClientProcess()");
					return (IntPtr)(-14L);
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving GetTokenForClientProcess()");
			return result;
		}

		private bool OOMExceptionRetry(Action action)
		{
			bool flag = false;
			try
			{
				IL_02:
				action();
				return true;
			}
			catch (OutOfMemoryException ex)
			{
				if (!flag)
				{
					flag = true;
					goto IL_02;
				}
				AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.userName, new object[]
				{
					this.userName,
					ex.ToString()
				});
			}
			return false;
		}

		private NameValueCollection CreateUserContextHeaders()
		{
			NameValueCollection nameValueCollection = new NameValueCollection(3);
			if (!string.IsNullOrEmpty(this.userAgent))
			{
				nameValueCollection.Add("X-MS-Client-User-Agent", this.userAgent);
			}
			if (!string.IsNullOrEmpty(this.userHostAddress))
			{
				nameValueCollection.Add("X-MS-Forwarded-Client-IP", this.userHostAddress);
			}
			if (!string.IsNullOrEmpty(this.userEndpoint))
			{
				nameValueCollection.Add("X-MS-Client-Application", this.userEndpoint);
			}
			if (nameValueCollection.Count > 0)
			{
				return nameValueCollection;
			}
			return null;
		}

		private AuthStatus TarpittedAuthError(AuthStatus error, bool tarpit)
		{
			if (tarpit)
			{
				this.TraceAndReturnInformation(this.GetHashCode(), "<tarpit suggested>", new object[0]);
				AuthStatus authStatus = error;
				if (authStatus != AuthStatus.ADFSRulesDenied)
				{
					if (authStatus != AuthStatus.BadPassword)
					{
						switch (authStatus)
						{
						case AuthStatus.ExpiredCredentials:
							error = AuthStatus.RepeatedExpiredCredentials;
							break;
						case AuthStatus.RecoverableLogonFailed:
							error = AuthStatus.RepeatedRecoverableFailure;
							break;
						case AuthStatus.FederatedStsFailed:
							error = AuthStatus.RepeatedFederatedStsFailure;
							break;
						case AuthStatus.LiveIDFailed:
							error = AuthStatus.RepeatedLiveIDFailure;
							break;
						case AuthStatus.LogonFailed:
							error = AuthStatus.RepeatedLogonFailure;
							break;
						}
					}
					else
					{
						error = AuthStatus.RepeatedBadPassword;
					}
				}
				else
				{
					error = AuthStatus.RepeatedADFSRulesDenied;
				}
			}
			return error;
		}

		private AuthStatus FailureModeToAuthError(CredFailure mode, bool repeated)
		{
			switch (mode)
			{
			case CredFailure.Invalid:
				return this.TarpittedAuthError(AuthStatus.BadPassword, repeated);
			case CredFailure.Expired:
				return this.TarpittedAuthError(AuthStatus.ExpiredCredentials, repeated);
			case CredFailure.LockedOut:
				return this.TarpittedAuthError(AuthStatus.RecoverableLogonFailed, repeated);
			case CredFailure.LiveIdFailure:
				return this.TarpittedAuthError(AuthStatus.LiveIDFailed, repeated);
			case CredFailure.STSFailure:
				return this.TarpittedAuthError(AuthStatus.FederatedStsFailed, repeated);
			case CredFailure.AppPasswordRequired:
				return this.TarpittedAuthError(AuthStatus.AppPasswordRequired, repeated);
			case CredFailure.ADFSRulesDeny:
				return this.TarpittedAuthError(AuthStatus.ADFSRulesDenied, repeated);
			case CredFailure.AccountNotProvisioned:
				return this.TarpittedAuthError(AuthStatus.AccountNotProvisioned, repeated);
			case CredFailure.Forbidden:
				return this.TarpittedAuthError(AuthStatus.Forbidden, repeated);
			case CredFailure.UnfamiliarLocation:
				return this.TarpittedAuthError(AuthStatus.UnfamiliarLocation, repeated);
			default:
				return AuthStatus.LogonFailed;
			}
		}

		private string UserKey
		{
			get
			{
				return string.Format("{0} {1} {2} {3}", new object[]
				{
					this.userName,
					this.userEndpoint,
					this.userHostAddress,
					this.organizationContext
				}).ToLowerInvariant();
			}
		}

		private string UserKeyShortVersion
		{
			get
			{
				return string.Format("{0} {1}", this.userName, this.organizationContext).ToLowerInvariant();
			}
		}

		private bool IsHomeRealmDiscoveryState()
		{
			return this.authState == AuthService.authStateEnum.authSendHrdRequest || this.authState == AuthService.authStateEnum.authProcessHrdResponse;
		}

		private bool IsFederatedSTSState()
		{
			return this.authState == AuthService.authStateEnum.authSendFedSTSRequest || this.authState == AuthService.authStateEnum.authProcessFedSTSResponse;
		}

		private bool IsSamlSTSState()
		{
			return this.authState == AuthService.authStateEnum.authSendSamlSTSRequest || this.authState == AuthService.authStateEnum.authProcessSamlSTSResponse;
		}

		private bool IsLiveSTSState()
		{
			return this.authState == AuthService.authStateEnum.authSendLiveSTSRequest || this.authState == AuthService.authStateEnum.authProcessLiveSTSResponse || this.authState == AuthService.authStateEnum.authSendLiveFedRequest || this.authState == AuthService.authStateEnum.authProcessLiveFedResponse || this.authState == AuthService.authStateEnum.authSendLiveSamlRequest || this.authState == AuthService.authStateEnum.authProcessLiveSamlResponse;
		}

		private bool IsOfflineOrgIdState()
		{
			return this.authState == AuthService.authStateEnum.authOfflineAuth;
		}

		private bool IsWaitState()
		{
			return this.authState == AuthService.authStateEnum.authWaitHrdResponse || this.authState == AuthService.authStateEnum.authWaitLogonResponse;
		}

		private bool IsEndState()
		{
			return this.authState == AuthService.authStateEnum.authFinishLogon;
		}

		private void RecordLatency(string tag, LiveIdInstanceType instance, string server, long sslLatency, long latency)
		{
			this.iisLogMsg.AppendFormat("<{0}-{1}-{3}ms-{4}ms-ppserver={2}>", new object[]
			{
				tag,
				instance.ToString(),
				server,
				sslLatency,
				latency
			});
		}

		private void RecordLatency(string tag, LiveIdInstanceType instance, string server, long sslLatency, long latency, long rpsLatency, string puid)
		{
			this.iisLogMsg.AppendFormat("<{0}-{1}-{3}ms-{4}ms-{6}ms-ppserver={2}-puid={5}>", new object[]
			{
				tag,
				instance.ToString(),
				server,
				sslLatency,
				latency,
				puid,
				rpsLatency
			});
		}

		private void RecordLatency(string tag, LiveIdInstanceType instance, long latency)
		{
			this.iisLogMsg.AppendFormat("<{0}-{1}-{2}ms>", tag, instance.ToString(), latency);
		}

		private void RecordLatency(string tag, long latency)
		{
			this.iisLogMsg.AppendFormat("<{0}-{1}ms>", tag, latency);
		}

		private void TraceAndReturnInformation(int hashcode, string trace, params object[] args)
		{
			this.iisLogMsg.AppendFormat(trace, args);
			ExTraceGlobals.AuthenticationTracer.Information((long)hashcode, trace, args);
		}

		private void TraceAndReturnWarning(int hashcode, string trace, params object[] args)
		{
			this.iisLogMsg.AppendFormat(trace, args);
			ExTraceGlobals.AuthenticationTracer.TraceWarning((long)hashcode, trace, args);
		}

		private void TraceAndReturnError(int hashcode, string trace, params object[] args)
		{
			this.iisLogMsg.AppendFormat(trace, args);
			ExTraceGlobals.AuthenticationTracer.TraceError((long)hashcode, trace, args);
		}

		private static void IncrementPendingStsCounter(LiveIdInstanceType instance)
		{
			if (instance == LiveIdInstanceType.Consumer)
			{
				AuthService.counters.PendingStsRequests.Increment();
				return;
			}
			AuthService.counters.PendingMsoIdStsRequests.Increment();
		}

		private static void DecrementPendingStsCounter(LiveIdInstanceType instance)
		{
			if (instance == LiveIdInstanceType.Consumer)
			{
				AuthService.counters.PendingStsRequests.Decrement();
				return;
			}
			AuthService.counters.PendingMsoIdStsRequests.Decrement();
		}

		private void StartHrdRequestChain(LiveIdInstanceType instance)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering StartHrdRequestChain()");
			object user;
			if (this.isOrgIdOffline && LiveIdInstanceType.Business == instance)
			{
				this.realmDiscovery = new ADHomeRealmDiscovery(instance);
				user = this.userDomain;
			}
			else
			{
				user = this.escapedUserBytes;
				if (instance == LiveIdInstanceType.Business)
				{
					this.realmDiscovery = new HomeRealmDiscovery(this.GetHashCode(), instance, AuthServiceStaticConfig.Config.MsoRealmDiscoveryUri);
				}
				else
				{
					this.realmDiscovery = new HomeRealmDiscovery(this.GetHashCode(), instance, AuthServiceStaticConfig.Config.liveRealmDiscoveryUri);
				}
			}
			AuthService.counters.PendingHrdRequests.Increment();
			Interlocked.Exchange(ref this.hrdPending, 1);
			this.nextState = AuthService.authStateEnum.authSendHrdRequest;
			this.realmDiscovery.StartRequestChain(user, AuthService.asyncRequestCallback, this);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving StartHrdRequestChain()");
		}

		private void ProcessHrdRequest(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ProcessHrdRequest()");
			this.nextState = AuthService.authStateEnum.authProcessHrdResponse;
			this.realmDiscovery.ProcessRequest(asyncResult, AuthService.asyncRequestCallback, this);
			if (this.realmDiscovery.Instance == LiveIdInstanceType.Business)
			{
				this.extraWait = this.config.msoHrdDelay;
			}
			else
			{
				this.extraWait = this.config.liveHrdDelay;
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessHrdRequest()");
		}

		private void ProcessHrdResponse(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ProcessHrdResponse()");
			DomainConfig domainConfig = this.realmDiscovery.ProcessResponse(asyncResult);
			this.RecordLatency(this.realmDiscovery.StsTag, this.realmDiscovery.Instance, this.realmDiscovery.LiveServer, this.realmDiscovery.SSLConnectionLatency, this.realmDiscovery.Latency);
			if (Interlocked.Exchange(ref this.hrdPending, 0) == 1)
			{
				AuthService.counters.PendingHrdRequests.Decrement();
			}
			if (domainConfig != null && this.realmDiscovery.Instance == LiveIdInstanceType.Business && domainConfig.Instance == LiveIdInstanceType.Consumer && domainConfig.IsFederated)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Domain '{0}' is federated and in the consumer instance. Calling HRD on consumer instance", domainConfig.DomainName);
				if (!this.namespaceStats.VerifiedNamespace)
				{
					AuthService.hrdCache.MarkNamespaceVerified(this.namespaceStats.Fqdn, this.GetHashCode());
				}
				this.StartHrdRequestChain(LiveIdInstanceType.Consumer);
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessHrdResponse()");
				return;
			}
			lock (this.clientOp)
			{
				this.clientOp.NamespaceInfo = domainConfig;
			}
			if (this.hrdSetEventOnExit)
			{
				this.clientOp.HrdEvent.Set();
				this.hrdSetEventOnExit = false;
			}
			if (domainConfig != null)
			{
				if (domainConfig.IsFederated)
				{
					if (!this.namespaceStats.VerifiedNamespace)
					{
						AuthService.hrdCache.MarkNamespaceVerified(this.namespaceStats.Fqdn, this.GetHashCode());
					}
					AcceptedDomain acceptedDomain = null;
					ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.GetHashCode(), "Looking up accepted domain for user \"{0}\" by HRD federated domain info {1}", this.userName, domainConfig.DomainName);
					try
					{
						using (new ActivityScopeThreadGuard(this.activityScope))
						{
							ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(domainConfig.DomainName), 4276, "ProcessHrdResponse", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\AuthService.cs");
							acceptedDomain = tenantConfigurationSession.GetAcceptedDomainByDomainName(domainConfig.DomainName);
						}
					}
					catch (CannotResolveTenantNameException)
					{
						ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Could not resolve recipient session for domain '{0}'", domainConfig.DomainName);
					}
					catch (CannotResolvePartitionException)
					{
						ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Could not resolve recipient session for domain '{0}'", domainConfig.DomainName);
					}
					if (acceptedDomain == null)
					{
						try
						{
							using (new ActivityScopeThreadGuard(this.activityScope))
							{
								ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(this.userDomain), 4303, "ProcessHrdResponse", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\AuthService.cs");
								ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.GetHashCode(), "Looking up accepted domain for user \"{0}\" by user SMTP domain info {1}", this.userName, this.userDomain);
								acceptedDomain = tenantConfigurationSession.GetAcceptedDomainByDomainName(this.userDomain);
							}
						}
						catch (CannotResolveTenantNameException)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Could not resolve recipient session for domain '{0}'", this.userDomain);
						}
						catch (CannotResolvePartitionException)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Could not resolve recipient session for domain '{0}'", this.userDomain);
						}
					}
					if (acceptedDomain != null)
					{
						ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.GetHashCode(), "Found accepted domain {0} for user \"{1}\"", acceptedDomain.Name, this.userName);
						domainConfig.OrgId = acceptedDomain.OrganizationId;
					}
					if (!domainConfig.AuthURL.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
					{
						this.TraceAndReturnError(this.GetHashCode(), "Federated STS endpoint '{0}' is not secured using HTTPS.  Aborting request for user \"{1}\"", new object[]
						{
							domainConfig.AuthURL,
							this.userName
						});
						AuthService.eventLogger.LogEvent(domainConfig.OrgId, domainConfig.OrgId.Equals(OrganizationId.ForestWideOrgId) ? SecurityEventLogConstants.Tuple_FederatedSTSUrlNotSecure_Forensic : SecurityEventLogConstants.Tuple_FederatedSTSUrlNotSecure_TenantAlert, domainConfig.DomainName, domainConfig.AuthURL, domainConfig.DomainName);
						this.InvokeCallback((IntPtr)((long)this.TarpittedAuthError(AuthStatus.FederatedStsUrlNotEncrypted, true)));
						ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessHrdResponse()");
						return;
					}
				}
				if (this.allowOfflineOrgId && !this.isOrgIdOffline)
				{
					if (!AuthService.IsBackendServer && !this.passwordAndHrdSync)
					{
						this.requestUpdateHrdInBackend = true;
					}
					else
					{
						DomainConfig domainConfig2;
						bool flag2 = AuthService.hrdCache.TryGetValue(this.userDomain, out domainConfig2);
						if (flag2 && domainConfig2.SyncedAD && string.Equals(domainConfig2.AuthURL, domainConfig.AuthURL, StringComparison.OrdinalIgnoreCase))
						{
							if (domainConfig2.PreferredProtocol == domainConfig.PreferredProtocol)
							{
								goto IL_58F;
							}
						}
						try
						{
							ITenantConfigurationSession session = DirectorySessionFactory.Default.CreateTenantConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(string.IsNullOrEmpty(this.organizationContext) ? this.userDomain : this.organizationContext), 4374, "ProcessHrdResponse", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\AuthService.cs");
							string text;
							DomainConfig hrdentryFromAD = OfflineOrgIdAuth.GetHRDEntryFromAD(session, domainConfig, this.userDomain, out text);
							if (text != null)
							{
								this.TraceAndReturnError(this.GetHashCode(), text, new object[0]);
							}
							if ((hrdentryFromAD != null && (hrdentryFromAD.PreferredProtocol != domainConfig.PreferredProtocol || !string.Equals(hrdentryFromAD.AuthURL, domainConfig.AuthURL, StringComparison.OrdinalIgnoreCase))) || (hrdentryFromAD == null && (!string.IsNullOrEmpty(domainConfig.AuthURL) || (domainConfig.PreferredProtocol != LivePreferredProtocol.Unknown && domainConfig.PreferredProtocol != LivePreferredProtocol.WS_Trust))))
							{
								string value;
								OfflineOrgIdAuth.UpdateHRDEntryInAD(session, domainConfig, this.userDomain, out value);
								if (!string.IsNullOrEmpty(value))
								{
									this.iisLogMsg.Append(value);
								}
								else
								{
									this.iisLogMsg.Append("<HRD_SYNCED>");
								}
								AuthService.counters.NumberOfADHrdUpdate.Increment();
							}
							domainConfig.SyncedAD = true;
							goto IL_5A0;
						}
						catch (Exception ex)
						{
							AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_AccessOfflineHRDFailed, "ProcessHrdResponse", new object[]
							{
								this.userDomain,
								ex.Message
							});
							goto IL_5A0;
						}
						IL_58F:
						if (domainConfig2 != null)
						{
							domainConfig.SyncedAD = domainConfig2.SyncedAD;
						}
					}
				}
				IL_5A0:
				if (domainConfig.Cache && !this.isAppPassword && this.useHrdCache)
				{
					AuthService.hrdCache.Insert(this.userDomain, domainConfig);
				}
				this.FinishHrdResponse(domainConfig);
			}
			else
			{
				this.TraceAndReturnInformation(this.GetHashCode(), "{0}", new object[]
				{
					this.realmDiscovery.ErrorString
				});
				this.InvokeCallback((IntPtr)(-15L));
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessHrdResponse()");
		}

		private void WaitHrdRequestChain()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering WaitHrdRequestChain()");
			this.nextState = AuthService.authStateEnum.authWaitHrdResponse;
			this.asyncWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.clientOp.HrdEvent, new WaitOrTimerCallback(AuthService.ClientOpWaitCallback), this, TimeSpan.FromSeconds((double)this.Timeout), true);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving WaitHrdRequestChain()");
		}

		private void ResumeHrdRequestChain()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ResumeHrdRequestChain()");
			if (this.clientOp.NamespaceInfo != null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.GetHashCode(), "HRD was successful, starting logon sequence.");
				this.FinishHrdResponse(this.clientOp.NamespaceInfo);
			}
			else
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.GetHashCode(), "HRD was unsuccessful, retrying HRD.");
				this.StartHrdRequestChain(this.config.defaultInstance);
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ResumeHrdRequestChain()");
		}

		private void FinishHrdResponse(DomainConfig namespaceInfo)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering FinishHrdResponse()");
			if (this.requestUpdateHrdInBackend && this.passwordAndHrdSync)
			{
				this.InvokeCallback(new IntPtr(1));
				return;
			}
			this.namespaceInfo = namespaceInfo;
			this.userType = AuthServiceHelper.GetUserType(this.namespaceInfo);
			bool flag = false;
			if (this.useLogonCache)
			{
				lock (this.clientOp)
				{
					if (this.clientOp.StsEvent == null)
					{
						flag = true;
						this.stsSetEventOnExit = true;
						this.clientOp.StsEvent = new ManualResetEvent(false);
					}
					else if (this.stsSetEventOnExit)
					{
						flag = true;
					}
					goto IL_AC;
				}
			}
			flag = true;
			IL_AC:
			if (flag)
			{
				this.StartChain();
			}
			else
			{
				this.WaitLogonRequestChain();
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving FinishHrdResponse()");
		}

		private void StartChain()
		{
			Interlocked.Increment(ref this.namespaceStats.Count);
			if (this.namespaceInfo.IsFederated && (!this.isOrgIdOffline || !this.isAppPassword))
			{
				if (this.namespaceStats.IsBlacklisted)
				{
					this.TraceAndReturnInformation(this.GetHashCode(), "Namespace {0} is blacklisted until {1}", new object[]
					{
						this.namespaceStats.Fqdn,
						this.namespaceStats.BlacklistExpires.ToString("u")
					});
					this.InvokeCallback((IntPtr)((long)this.TarpittedAuthError(AuthStatus.FederatedStsFailed, true)));
					return;
				}
				switch (this.namespaceInfo.PreferredProtocol)
				{
				case LivePreferredProtocol.SAML_20:
					this.StartSamlRequestChain();
					return;
				}
				this.StartFederatedRequestChain();
				return;
			}
			else
			{
				this.ThrottleOfflineOrgIdAsPrimaryAuth();
				if (!this.isOrgIdOffline || this.namespaceInfo.Instance == LiveIdInstanceType.Consumer)
				{
					this.StartManagedRequestChain();
					return;
				}
				if (this.allowOfflineOrgId)
				{
					this.DoOfflineOrgIdAuth();
					return;
				}
				this.StartManagedRequestChain();
				return;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrottleOfflineOrgIdAsPrimaryAuth()
		{
			if (AuthServiceStaticConfig.Config.ThrottleOfflineOrgIdAsPrimaryUsage && !this.hasFailedOverForHrd && !this.hasFailedOverForAuth && this.isOrgIdOffline && (double)AuthService.offlineOrgIdRequestCountLastMinutes.LastTotalValue / ((double)AuthService.orgIdRequestCountLastMinutes.LastTotalValue + 0.1 + (double)AuthService.offlineOrgIdRequestCountLastMinutes.LastTotalValue) > (double)AuthServiceStaticConfig.Config.OfflineOrgIdPrimaryUsageThreholdPercentage / 100.0)
			{
				this.isOrgIdOffline = false;
			}
		}

		private void StartManagedRequestChain()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering StartManagedRequestChain()");
			AuthService.IncrementPendingStsCounter(this.namespaceInfo.Instance);
			Interlocked.Exchange(ref this.liveStsPending, 1);
			bool flag = true;
			try
			{
				if (Encoding.Default.GetCharCount(this.ansiPassword) > 16)
				{
					if (this.retryCountForLongPassword == 0)
					{
						this.retryCountForLongPassword++;
					}
					else if (this.retryCountForLongPassword > 0)
					{
						this.retryCountForLongPassword = -1;
						ExTraceGlobals.AuthenticationTracer.Information<int>((long)this.GetHashCode(), "Password is longer than {0} characters, truncating", 16);
						this.iisLogMsg.Append("Truncating long password.");
						Array.Clear(this.escapedPassBytes, 0, this.escapedPassBytes.Length);
						this.escapedPassBytes = AuthService.CleanBytes(this.ansiPassword, false, 16);
					}
				}
				if (this.enableXmlAuthForLiveId && this.namespaceInfo.IsOutlookCom)
				{
					this.liveIdSTS = new LiveIdXmlAuth(this.GetHashCode(), this.config, this.namespaceStats, this.userClientIp, this.userEndpoint);
					this.nextState = AuthService.authStateEnum.authSendLiveSTSRequest;
					this.liveIdSTS.StartRequestChain(Encoding.Default.GetString(this.escapedUserBytes), this.escapedPassBytes, AuthService.asyncRequestCallback, this);
				}
				else
				{
					this.wsseToken = AuthService.CreateWsseUsernameToken(this.escapedUserBytes, this.escapedPassBytes);
					bool offlineAuthEnabled = !this.hasFailedOverForAuth && this.allowOfflineOrgId;
					this.liveIdSTS = new LiveIdSTS(this.GetHashCode(), this.namespaceInfo.Instance, this.config, this.namespaceStats, offlineAuthEnabled);
					this.liveIdSTS.ExtraHeaders = this.CreateUserContextHeaders();
					this.nextState = AuthService.authStateEnum.authSendLiveSTSRequest;
					this.liveIdSTS.StartRequestChain(this.userName, this.wsseToken, AuthService.asyncRequestCallback, this);
				}
				flag = false;
			}
			finally
			{
				if (flag && this.wsseToken != null)
				{
					Array.Clear(this.wsseToken, 0, this.wsseToken.Length);
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving StartManagedRequestChain()");
		}

		private IntPtr DoOfflineOrgIdAuth()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering StartOffineOrgIdAuthChain()");
			this.offlineOrgIdDoneEvent = new AutoResetEvent(false);
			this.authState = AuthService.authStateEnum.authOfflineAuth;
			SafeTokenHandle userToken = null;
			IntPtr intPtr = (IntPtr)(-16L);
			if (this.namespaceInfo != null && this.namespaceInfo.IsOutlookCom)
			{
				this.InvokeCallback(intPtr);
				return intPtr;
			}
			Exception ex = null;
			ADRawEntry userEntry = null;
			try
			{
				if (!this.hasFailedOverForAuth && this.calledFromTestFunction)
				{
					this.StartTestFailover();
				}
				AuthService.offlineOrgIdRequestCountLastMinutes.AddValue(1L);
				ITenantRecipientSession recipientSession = null;
				long latency = AuthServiceHelper.ExecuteAndRecordLatency(delegate
				{
					userEntry = this.GetUniqueUserInAD(out recipientSession);
				});
				this.RecordLatency("GetADUser", latency);
				if (userEntry == null)
				{
					return intPtr;
				}
				if (this.namespaceInfo == null && !AuthService.hrdCache.TryGetValue(this.userDomain, out this.namespaceInfo))
				{
					ITenantConfigurationSession session = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(this.userDomain), 4742, "DoOfflineOrgIdAuth", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\AuthService.cs");
					string text;
					this.namespaceInfo = OfflineOrgIdAuth.GetHRDEntryFromAD(session, null, this.userDomain, out text);
				}
				string implicitUpn = AuthServiceHelper.GetImplicitUpn(userEntry);
				if (this.hasPassedFirstAuth)
				{
					bool flag = !string.IsNullOrEmpty((string)userEntry[ADUserSchema.NetIDSuffix]);
					if (flag)
					{
						this.TraceAndReturnWarning(this.GetHashCode(), "External LiveId is found in user \"{0}\", cannot sync to AD.", new object[]
						{
							this.userName
						});
						intPtr = (IntPtr)((long)this.TarpittedAuthError(AuthStatus.LiveIDFailed, this.namespaceStats.IsTarpitted));
					}
					else if (this.returnWindowsToken)
					{
						this.S4ULogon(implicitUpn, this.clientProcessId, out intPtr, this.GetHashCode());
					}
					else
					{
						intPtr = (IntPtr)1L;
					}
				}
				else
				{
					bool loggedOn = false;
					int error;
					latency = AuthServiceHelper.ExecuteAndRecordLatency(delegate
					{
						loggedOn = AuthService.ADLogonUser(userEntry, this.ansiPassword, out userToken, out error);
					});
					this.RecordLatency("ADLogonUser", latency);
					AuthService.counters.NumberOfAdRequestForOfflineOrgId.Increment();
					AuthService.counters.OfflineOrgIdAuthenticationCount.Increment();
					if (!loggedOn)
					{
						AuthService.counters.NumberOfOfflineOrgIdRequestWithInvalidCredential.Increment();
						intPtr = (IntPtr)((long)this.FailureModeToAuthError(CredFailure.Invalid, true));
					}
					else
					{
						this.passwordConfidence = AuthService.GetPasswordConfidenceConfig(this.namespaceInfo.IsFederated);
						if (!AuthService.IsBackendServer)
						{
							this.requestPasswordConfidenceCheckInBackend = true;
							this.TraceAndReturnInformation(this.GetHashCode(), "<RequestCheckPwdConfidence:{0}", new object[]
							{
								this.passwordConfidence
							});
							if (this.returnWindowsToken)
							{
								intPtr = this.GetTokenForClientProcess(this.clientProcessId, userToken.DangerousGetHandle());
							}
							else
							{
								intPtr = (IntPtr)1L;
							}
						}
						else
						{
							ADObjectId adobjectId = (ADObjectId)userEntry[ADMailboxRecipientSchema.Database];
							if (userEntry[ADMailboxRecipientSchema.ExchangeGuid] == null || adobjectId == null || Guid.Empty.Equals((Guid)userEntry[ADMailboxRecipientSchema.ExchangeGuid]))
							{
								this.TraceAndReturnWarning(this.GetHashCode(), "User \"{0}\" is not a mailbox user.", new object[]
								{
									this.userName
								});
								intPtr = (IntPtr)(-11L);
							}
							else if (!this.CheckPasswordConfidence(userEntry, recipientSession))
							{
								intPtr = (IntPtr)(-11L);
							}
							else if (this.returnWindowsToken)
							{
								intPtr = this.GetTokenForClientProcess(this.clientProcessId, userToken.DangerousGetHandle());
							}
							else
							{
								intPtr = (IntPtr)1L;
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				ex = ex2;
				if ((this.testFailOver & TestFailoverFlags.OfflineAuthentication) == TestFailoverFlags.None)
				{
					this.TraceAndReturnWarning(this.GetHashCode(), "OfflineAuth for user \"{0}\" failed due to unexpected exception. {1}", new object[]
					{
						this.userName,
						ex2.Message
					});
					AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_OfflineAuthFailed, "Offline Authentication fails" + this.userName, new object[]
					{
						"LogonUser - DoOfflineAuth",
						this.userName,
						ex2.ToString()
					});
				}
			}
			finally
			{
				this.offlineOrgIdResult = intPtr;
				this.offlineOrgIdDoneEvent.Set();
				if (userToken != null)
				{
					userToken.Dispose();
				}
				if (intPtr.ToInt32() <= 0 && intPtr.ToInt32() >= -29)
				{
					if ((this.testFailOver & TestFailoverFlags.OfflineAuthentication) == TestFailoverFlags.None)
					{
						AuthService.counters.NumberOfFailedOfflineOrgIdRequests.Increment();
					}
					if (!this.HandleFailOver(ex))
					{
						this.TraceAndReturnInformation(this.GetHashCode(), "{0} Error:{1}", new object[]
						{
							"AuthenticatedBy:OfflineOrgId.",
							intPtr.ToInt32()
						});
						if (ex != null)
						{
							this.InvokeCallback(ex);
						}
						else
						{
							this.InvokeCallback((IntPtr)((long)this.TarpittedAuthError((AuthStatus)((int)intPtr), this.namespaceStats.IsTarpitted)));
						}
					}
				}
				else
				{
					this.puid = userEntry[ADUserSchema.NetID].ToString();
					string key = this.namespaceInfo.IsFederated ? this.UserKey : this.UserKeyShortVersion;
					if (!string.IsNullOrEmpty(this.puid) && this.useLogonCache && this.usePositiveLogonCache)
					{
						AuthService.logonCache.Add(key, ExDateTime.UtcNow, this.puid, this.passwordHash, AuthServiceStaticConfig.Config.ADLookupCacheLifetimeOffline, "AuthenticatedBy:OfflineOrgId.", this.GetHashCode(), null, AuthServiceHelper.GetUserType(this.namespaceInfo), false, this.requestPasswordConfidenceCheckInBackend);
					}
					if (!this.returnWindowsToken && !this.syncADBackEndOnly)
					{
						this.tokenToReturn = LiveIdBasicTokenAccessor.Create(userEntry).GetToken();
					}
					this.TraceAndReturnInformation(this.GetHashCode(), "AuthenticatedBy:OfflineOrgId.", new object[0]);
					this.InvokeCallback(intPtr);
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving OfflineOrgIdAuth()");
			return intPtr;
		}

		private ADRawEntry GetUniqueUserInAD(out ITenantRecipientSession recipientSession)
		{
			SmtpProxyAddress proxyAddress = new SmtpProxyAddress(this.userName, true);
			bool flag = false;
			ADRawEntry adrawEntry = this.ADUserLookupByUPN(proxyAddress, out recipientSession, out flag);
			AuthService.counters.NumberOfAdRequestForOfflineOrgId.Increment();
			if (adrawEntry == null)
			{
				if (!flag)
				{
					this.TraceAndReturnWarning(this.GetHashCode(), "No mailbox found for UPN \"{0}\"", new object[]
					{
						this.userName
					});
				}
				else
				{
					this.TraceAndReturnWarning(this.GetHashCode(), "Ambiguous user found: \"{0}\". Fail the request", new object[]
					{
						this.userName
					});
				}
			}
			return adrawEntry;
		}

		private static bool ADLogonUser(ADRawEntry userEntry, byte[] password, out SafeTokenHandle userToken, out int errorCode)
		{
			string implicitUpn = AuthServiceHelper.GetImplicitUpn(userEntry);
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)implicitUpn.GetHashCode(), "Performing Win32.LogonUser for \"{0}\"", implicitUpn);
			byte[] bytes = Encoding.ASCII.GetBytes(implicitUpn);
			userToken = new SafeTokenHandle();
			Stopwatch stopwatch = Stopwatch.StartNew();
			bool result = SspiNativeMethods.LogonUser(bytes, null, password, LogonType.NetworkCleartext, LogonProvider.Default, ref userToken);
			errorCode = Marshal.GetLastWin32Error();
			stopwatch.Stop();
			AuthService.counters.AverageAdResponseTime.IncrementBy(stopwatch.ElapsedMilliseconds);
			AuthService.counters.AverageAdResponseTimeBase.Increment();
			AuthService.counters.NumberOfAdRequests.Increment();
			return result;
		}

		private bool CheckPasswordConfidence(ADRawEntry userEntry, ITenantRecipientSession recipientSession)
		{
			if ((this.testFailOver & TestFailoverFlags.LowPasswordConfidence) != TestFailoverFlags.None)
			{
				this.iisLogMsg.Append("Test failover because of low confidence.");
				return false;
			}
			DateTime dateTime = MailboxUserProfile.GetLastLogonTime(userEntry[ADUserSchema.NetID].ToString());
			if (dateTime == DateTime.MinValue)
			{
				ADObjectId adobjectId = (ADObjectId)userEntry[ADMailboxRecipientSchema.Database];
				Stopwatch.StartNew();
				string text;
				dateTime = MailboxUserProfile.GetLastLogonTimeFromMailbox((Guid)userEntry[ADMailboxRecipientSchema.ExchangeGuid], adobjectId.ObjectGuid, (OrganizationId)userEntry[ADObjectSchema.OrganizationId], out text);
				if (dateTime != DateTime.MinValue)
				{
					MailboxUserProfile.SetLastLogonTime(userEntry[ADUserSchema.NetID].ToString(), dateTime);
				}
				this.stopwatch.Stop();
				this.TraceAndReturnInformation(this.GetHashCode(), "<{0}:{1}ms>{2}", new object[]
				{
					"GetLastLogonTimeFromMailbox",
					this.stopwatch.ElapsedMilliseconds,
					text ?? string.Empty
				});
			}
			this.TraceAndReturnInformation(this.GetHashCode(), "<{0}-{1}-{2}>", new object[]
			{
				"CheckPwdConfidence",
				this.passwordConfidence,
				dateTime.ToString()
			});
			return dateTime + TimeSpan.FromDays((double)this.passwordConfidence) >= DateTime.UtcNow;
		}

		private void ContinueRequestChain()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ContinueRequestChain()");
			this.hasPassedFirstAuth = true;
			AuthService.IncrementPendingStsCounter(this.liveIdSTS.Instance);
			Interlocked.Exchange(ref this.liveStsPending, 1);
			try
			{
				switch (this.namespaceInfo.PreferredProtocol)
				{
				case LivePreferredProtocol.SAML_20:
					this.nextState = AuthService.authStateEnum.authSendLiveSamlRequest;
					goto IL_69;
				}
				this.nextState = AuthService.authStateEnum.authSendLiveFedRequest;
				IL_69:
				this.ThrottleOfflineOrgIdAsPrimaryAuth();
				if (!this.isOrgIdOffline || this.namespaceInfo.Instance == LiveIdInstanceType.Consumer)
				{
					this.liveIdSTS.StartRequestChain(this.userName, this.wsseToken, AuthService.asyncRequestCallback, this);
				}
				else
				{
					this.DoOfflineOrgIdAuth();
				}
			}
			catch (Exception)
			{
				Array.Clear(this.wsseToken, 0, this.wsseToken.Length);
				throw;
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ContinueRequestChain()");
		}

		private void ContinueTwoHopRequestChain()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ContinueTwoHopRequestChain()");
			this.hasPassedFirstAuth = true;
			this.ThrottleOfflineOrgIdAsPrimaryAuth();
			if (!this.isOrgIdOffline)
			{
				AuthService.IncrementPendingStsCounter(LiveIdInstanceType.Business);
				Interlocked.Exchange(ref this.liveStsPending, 1);
				try
				{
					bool offlineAuthEnabled = !this.hasFailedOverForAuth && this.allowOfflineOrgId;
					this.liveIdSTS = new LiveIdSTS(this.GetHashCode(), LiveIdInstanceType.Business, this.config, this.namespaceStats, offlineAuthEnabled);
					this.liveIdSTS.ExtraHeaders = this.CreateUserContextHeaders();
					this.nextState = AuthService.authStateEnum.authSendLiveFedRequest;
					this.liveIdSTS.StartRequestChain(this.userName, this.wsseToken, AuthService.asyncRequestCallback, this);
					goto IL_C8;
				}
				catch (Exception)
				{
					Array.Clear(this.wsseToken, 0, this.wsseToken.Length);
					throw;
				}
			}
			this.DoOfflineOrgIdAuth();
			IL_C8:
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ContinueTwoHopRequestChain()");
		}

		private void ProcessLiveRequest(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ProcessLiveRequest()");
			if (this.authState == AuthService.authStateEnum.authSendLiveFedRequest)
			{
				this.nextState = AuthService.authStateEnum.authProcessLiveFedResponse;
			}
			else if (this.authState == AuthService.authStateEnum.authSendLiveSamlRequest)
			{
				this.nextState = AuthService.authStateEnum.authProcessLiveSamlResponse;
			}
			else
			{
				this.nextState = AuthService.authStateEnum.authProcessLiveSTSResponse;
			}
			if (this.liveIdSTS.Instance == LiveIdInstanceType.Business)
			{
				this.extraWait = this.config.msoRst2Delay;
			}
			else
			{
				this.extraWait = this.config.liveRst2Delay;
			}
			this.liveIdSTS.ProcessRequest(asyncResult, AuthService.asyncRequestCallback, this);
			if (this.wsseToken != null)
			{
				Array.Clear(this.wsseToken, 0, this.wsseToken.Length);
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveRequest()");
		}

		private void ProcessLiveResponse(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ProcessLiveResponse()");
			string text;
			RPSProfile rpsprofile;
			string text2;
			this.liveIdSTS.ProcessResponse(this.userName, asyncResult, out text, out rpsprofile, out text2);
			this.rpsUserProfile = rpsprofile;
			ADRawEntry adrawEntry = null;
			StringBuilder stringBuilder = new StringBuilder(256);
			IntPtr intPtr = (IntPtr)0L;
			string text3 = (rpsprofile != null) ? rpsprofile.HexPuid : string.Empty;
			this.RecordLatency(this.liveIdSTS.StsTag, this.liveIdSTS.Instance, this.liveIdSTS.LiveServer, this.liveIdSTS.SSLConnectionLatency, this.liveIdSTS.Latency, this.liveIdSTS.RPSParseLatency, text3);
			if (Interlocked.Exchange(ref this.liveStsPending, 0) == 1)
			{
				AuthService.DecrementPendingStsCounter(this.liveIdSTS.Instance);
			}
			if (string.IsNullOrEmpty(text3) || string.IsNullOrEmpty(text))
			{
				if (!string.IsNullOrEmpty(this.liveIdSTS.RecoveryUrl))
				{
					stringBuilder.AppendFormat("<RECOVERY {0}>", this.liveIdSTS.RecoveryUrl);
					this.TraceAndReturnInformation(this.GetHashCode(), stringBuilder.ToString(), new object[0]);
				}
				if (!string.IsNullOrEmpty(text2))
				{
					this.TraceAndReturnError(this.GetHashCode(), text2, new object[0]);
				}
				if (string.IsNullOrEmpty(text))
				{
					CredFailure mode;
					if (this.liveIdSTS.IsExpiredCreds)
					{
						mode = CredFailure.Expired;
						this.TraceAndReturnInformation(this.GetHashCode(), "{0} logon failure - expired credentials - '{1}'", new object[]
						{
							this.liveIdSTS.GetType().Name,
							this.liveIdSTS.ErrorString
						});
					}
					else if (this.liveIdSTS.IsAccountNotProvisioned)
					{
						mode = CredFailure.AccountNotProvisioned;
						this.TraceAndReturnInformation(this.GetHashCode(), "{0} logon failure - account has not provisioned - '{1}'", new object[]
						{
							this.liveIdSTS.GetType().Name,
							this.liveIdSTS.ErrorString
						});
					}
					else if (this.liveIdSTS.AppPasswordRequired)
					{
						mode = CredFailure.AppPasswordRequired;
						this.TraceAndReturnInformation(this.GetHashCode(), "{0} logon failure - recovery possible - '{1}'", new object[]
						{
							this.liveIdSTS.GetType().Name,
							this.liveIdSTS.ErrorString
						});
					}
					else if (this.liveIdSTS.UserRecoveryPossible())
					{
						mode = CredFailure.LockedOut;
						this.TraceAndReturnInformation(this.GetHashCode(), "{0} logon failure - recovery possible - '{1}'", new object[]
						{
							this.liveIdSTS.GetType().Name,
							this.liveIdSTS.ErrorString
						});
					}
					else if (this.liveIdSTS.IsUnfamiliarLocation)
					{
						mode = CredFailure.UnfamiliarLocation;
						this.TraceAndReturnInformation(this.GetHashCode(), "{0} logon failure - unfamiliar location", new object[]
						{
							this.liveIdSTS.GetType().Name
						});
					}
					else
					{
						if (this.retryCountForLongPassword > 0)
						{
							this.StartManagedRequestChain();
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveResponse()");
							return;
						}
						mode = (this.liveIdSTS.IsBadCredentials ? CredFailure.Invalid : CredFailure.LiveIdFailure);
						this.TraceAndReturnInformation(this.GetHashCode(), "{0} logon failure '{1}'", new object[]
						{
							this.liveIdSTS.GetType().Name,
							this.liveIdSTS.ErrorString
						});
					}
					if (this.liveIdSTS.IsBadCredentials || this.liveIdSTS.IsExpiredCreds)
					{
						this.InvalidatePasswordInAD();
					}
					bool repeated = this.useLogonCache && AuthService.logonCache.Add(this.UserKey, ExDateTime.UtcNow, this.passwordHash, mode, stringBuilder.ToString(), this.userType, this.GetHashCode());
					this.InvokeCallback((IntPtr)((long)this.FailureModeToAuthError(mode, repeated)));
					ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveResponse()");
					return;
				}
				this.InvokeCallback((IntPtr)((long)this.TarpittedAuthError(AuthStatus.UnableToOpenTicket, true)));
				return;
			}
			else
			{
				if (this.config.defaultInstance == LiveIdInstanceType.Business && this.liveIdSTS.Instance == LiveIdInstanceType.Consumer && !(this.liveIdSTS is LiveIdXmlAuth))
				{
					this.wsseToken = Encoding.UTF8.GetBytes(text);
					this.ContinueTwoHopRequestChain();
					ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveResponse()");
					return;
				}
				this.authState = AuthService.authStateEnum.authFinishLogon;
				if (rpsprofile.AppPassword)
				{
					stringBuilder.Append("<APPPASSWORD>");
					AuthService.counters.NumberOfAppPasswords.Increment();
				}
				if (this.config.defaultInstance == LiveIdInstanceType.Business && this.namespaceInfo.Instance == LiveIdInstanceType.Consumer && !(this.liveIdSTS is LiveIdXmlAuth))
				{
					text3 = rpsprofile.ConsumerPuid;
				}
				if (rpsprofile.CredsExpireIn <= (uint)this.config.passwordExpiryUpperBound)
				{
					stringBuilder.AppendFormat("<CREDEXPIRES {0}>", rpsprofile.CredsExpireIn);
					if (!string.IsNullOrEmpty(rpsprofile.RecoveryUrl))
					{
						stringBuilder.AppendFormat("<RECOVERY {0}>", rpsprofile.RecoveryUrl);
					}
				}
				this.TraceAndReturnInformation(this.GetHashCode(), stringBuilder.ToString(), new object[0]);
				if (!string.Equals(rpsprofile.MemberName, this.userName, StringComparison.OrdinalIgnoreCase))
				{
					this.TraceAndReturnInformation(this.GetHashCode(), "<TicketName:{0}>", new object[]
					{
						rpsprofile.MemberName ?? string.Empty
					});
				}
				this.stopwatch.Stop();
				AuthService.counters.AverageResponseTime.IncrementBy(this.stopwatch.ElapsedMilliseconds);
				AuthService.counters.AverageResponseTimeBase.Increment();
				intPtr = new IntPtr(1);
				this.puid = text3;
				this.lastAuthTime = ExDateTime.UtcNow;
				this.compactToken = text;
				if (this.config.bypassTOUCheck)
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>(0L, "Bypassed TOU check for user \"{0}\"", this.userName);
					rpsprofile.HasSignedTOU = true;
				}
				else if (!rpsprofile.HasSignedTOU)
				{
					bool flag = false;
					if (AuthService.domainsByPassTOU != null && !AuthService.domainsByPassTOU.TryGetValue(this.userDomain, out flag))
					{
						rpsprofile.HasSignedTOU = false;
					}
					else if (!flag)
					{
						AuthService.counters.NumberOfTOUFailures.Increment();
						bool tarpit = this.useLogonCache && AuthService.logonCache.Add(this.UserKey, ExDateTime.UtcNow, this.passwordHash, CredFailure.LockedOut, null, this.userType, this.GetHashCode());
						this.TraceAndReturnWarning(this.GetHashCode(), "User with PUID {0} has not accepted Terms Of Use.", new object[]
						{
							text3
						});
						this.InvokeCallback((IntPtr)((long)this.TarpittedAuthError(AuthStatus.RecoverableLogonFailed, tarpit)));
						ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessResponse()");
						return;
					}
				}
				if (this.useLogonCache)
				{
					string key = this.namespaceInfo.IsFederated ? this.UserKey : this.UserKeyShortVersion;
					if (this.usePositiveLogonCache)
					{
						AuthService.logonCache.Add(key, ExDateTime.UtcNow, text3, this.passwordHash, AuthServiceStaticConfig.Config.logonCacheLifetime, stringBuilder.ToString(), this.GetHashCode(), null, this.userType, rpsprofile.AppPassword, false);
					}
					AuthService.logonCache.RemoveBadPassword(this.UserKey, this.passwordHash);
				}
				if (!this.namespaceInfo.IsOutlookCom)
				{
					if (!this.returnWindowsToken && rpsprofile.HasSignedTOU && this.syncADBackEndOnly)
					{
						this.includePWinCat = true;
						this.InvokeCallback(new IntPtr(1));
						this.TryUpdateLastLogonTime(text3, DateTime.UtcNow);
						return;
					}
					try
					{
						this.GetADUserEntry(text3, out adrawEntry);
						if (adrawEntry == null)
						{
							this.TraceAndReturnWarning(this.GetHashCode(), "Redirect needed: no mailbox found for PUID {0}", new object[]
							{
								text3
							});
							this.InvokeCallback((IntPtr)(-1L));
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveResponse()");
							return;
						}
						MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)adrawEntry[IADSecurityPrincipalSchema.AltSecurityIdentities];
						bool flag2 = false;
						StringBuilder stringBuilder2 = new StringBuilder();
						if (multiValuedProperty != null)
						{
							foreach (string text4 in multiValuedProperty)
							{
								if (text4.IndexOf(text3, StringComparison.OrdinalIgnoreCase) >= 0)
								{
									flag2 = true;
									break;
								}
								stringBuilder2.Append(text4);
								stringBuilder2.Append(" ");
							}
						}
						if (!flag2)
						{
							this.TraceAndReturnWarning(this.GetHashCode(), "<Ldap Bug: Expected Puid {0}, actual Puid in AD {1}.>", new object[]
							{
								text3,
								stringBuilder2.ToString()
							});
							this.InvokeCallback((IntPtr)(-19L));
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveResponse()");
							return;
						}
						OrganizationId organizationId = (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
						string arg = (string)adrawEntry[ADMailboxRecipientSchema.SamAccountName];
						string text5 = string.Format("{0}@{1}", arg, organizationId.PartitionId.ForestFQDN);
						bool flag3 = !string.IsNullOrEmpty((string)adrawEntry[ADUserSchema.NetIDSuffix]);
						OrganizationProperties organizationProperties;
						if (!OrganizationPropertyCache.TryGetOrganizationProperties(organizationId, out organizationProperties))
						{
							this.TraceAndReturnError(this.GetHashCode(), "Redirect needed: could not locate org info for organization {0} even though user from this org was found.", new object[]
							{
								organizationId
							});
							this.InvokeCallback((IntPtr)(-1L));
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveResponse()");
							return;
						}
						if (!this.config.bypassTOUCheck)
						{
							if (AuthService.domainsByPassTOU == null)
							{
								AuthService.domainsByPassTOU = new TimeoutCache<string, bool>(this.config.TOUCacheBuckets, this.config.TOUCacheBucketSize, false);
							}
							AuthService.domainsByPassTOU.InsertAbsolute(this.userDomain, organizationProperties.SkipToUAndParentalControlCheck, TimeSpan.FromMinutes((double)AuthServiceStaticConfig.Config.TOUByPassCacheExpireTimeInMinutes), null);
						}
						if (!organizationProperties.SkipToUAndParentalControlCheck && !rpsprofile.HasSignedTOU)
						{
							AuthService.counters.NumberOfTOUFailures.Increment();
							bool tarpit2 = this.useLogonCache && AuthService.logonCache.Add(this.UserKey, ExDateTime.UtcNow, this.passwordHash, CredFailure.LockedOut, null, this.userType, this.GetHashCode());
							this.TraceAndReturnWarning(this.GetHashCode(), "User with PUID {0} has not accepted Terms Of Use.", new object[]
							{
								text3
							});
							this.InvokeCallback((IntPtr)((long)this.TarpittedAuthError(AuthStatus.RecoverableLogonFailed, tarpit2)));
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessResponse()");
							return;
						}
						if (!this.syncAD || flag3)
						{
							if (this.returnWindowsToken)
							{
								ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.GetHashCode(), "Creating WindowsIdentity token for user {0} using UPN {1}.  No password sync.", this.userName, text5);
								Stopwatch stopwatch = Stopwatch.StartNew();
								if (!this.S4ULogon(text5, this.clientProcessId, out intPtr, this.GetHashCode()))
								{
									ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Creating WindowsIdentity failed for user {0}.  No password sync.", this.userName);
								}
								stopwatch.Stop();
								this.RecordLatency("s4u", stopwatch.ElapsedMilliseconds);
							}
							else
							{
								ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Creating CommonAccessToken for user {0}.  No password sync.", this.userName);
							}
							this.InvokeCallback(intPtr);
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveResponse()");
							return;
						}
						bool flag4 = false;
						try
						{
							flag4 = this.SyncPasswordUpnRetrieveToken(adrawEntry, text5, this.returnWindowsToken, ref intPtr);
						}
						catch (ADOperationException ex)
						{
							AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.userName, new object[]
							{
								this.userName + ":SyncPasswordUpn",
								ex.ToString()
							});
						}
						catch (TenantIsArrivingException ex2)
						{
							AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.userName, new object[]
							{
								this.userName + ":SyncPasswordUpn",
								ex2.ToString()
							});
						}
						catch (TenantIsLockedDownForRelocationException ex3)
						{
							AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.userName, new object[]
							{
								this.userName + ":SyncPasswordUpn",
								ex3.ToString()
							});
						}
						if (this.returnWindowsToken && !flag4)
						{
							this.InvokeCallback((IntPtr)0L);
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveResponse()");
							return;
						}
					}
					catch (DataSourceTransientException ex4)
					{
						this.TraceAndReturnError(this.GetHashCode(), "ITenantRecipientSession threw exception {0}", new object[]
						{
							ex4
						});
						AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_CannotConnectToAD, "CannotConnectToAD" + this.userName, new object[]
						{
							ex4.Message
						});
						throw;
					}
					catch (DataSourceOperationException ex5)
					{
						this.TraceAndReturnError(this.GetHashCode(), "ITenantRecipientSession threw exception {0}", new object[]
						{
							ex5
						});
						AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_CannotConnectToAD, "CannotConnectToAD" + this.userName, new object[]
						{
							ex5.Message
						});
						throw;
					}
					this.InvokeCallback(intPtr);
					this.TryUpdateLastLogonTime(text3, DateTime.UtcNow);
					ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessLiveResponse()");
					return;
				}
				else
				{
					if (!this.returnWindowsToken && rpsprofile.HasSignedTOU && this.syncADBackEndOnly)
					{
						if (this.config.EnableConsumerRPSSync && AuthService.IsBackendServer)
						{
							ulong num = 0UL;
							if (ulong.TryParse(text3, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
							{
								string text6 = null;
								rpsprofile.CurrentAlias = this.userName;
								Stopwatch.StartNew();
								MailboxUserProfile.SetRPSProfileToMailbox(num, rpsprofile, out text6, this.config.UpdateExistingMbxEntryOnly);
								this.stopwatch.Stop();
								if (text6 != null)
								{
									this.TraceAndReturnWarning(this.GetHashCode(), text6, new object[0]);
								}
								else
								{
									this.TraceAndReturnInformation(this.GetHashCode(), "<SyncProfile-{0}ms>", new object[]
									{
										this.stopwatch.ElapsedMilliseconds
									});
								}
							}
							else
							{
								this.TraceAndReturnError(this.GetHashCode(), "<WrongPUIDFormat>", new object[0]);
							}
						}
						this.InvokeCallback(new IntPtr(1));
						return;
					}
					if (!rpsprofile.HasSignedTOU)
					{
						AuthService.counters.NumberOfTOUFailures.Increment();
						bool tarpit3 = this.useLogonCache && AuthService.logonCache.Add(this.UserKey, ExDateTime.UtcNow, this.passwordHash, CredFailure.LockedOut, null, this.userType, this.GetHashCode());
						this.TraceAndReturnWarning(this.GetHashCode(), "User with PUID {0} has not accepted Terms Of Use.", new object[]
						{
							this.puid
						});
						this.InvokeCallback((IntPtr)((long)this.TarpittedAuthError(AuthStatus.RecoverableLogonFailed, tarpit3)));
						ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessResponse()");
						return;
					}
					bool tarpit4 = this.useLogonCache && AuthService.logonCache.Add(this.UserKey, ExDateTime.UtcNow, this.passwordHash, CredFailure.NotSupportedProtocolForOutlookCom, null, this.userType, this.GetHashCode());
					this.TraceAndReturnInformation(this.GetHashCode(), "Outlook.com user cannot access CAT V1 protocol", new object[0]);
					this.InvokeCallback((IntPtr)((long)this.TarpittedAuthError(AuthStatus.Forbidden, tarpit4)));
					return;
				}
			}
		}

		private bool CheckUpnAgainstAcceptedDomains(ADObjectId entryId, OrganizationId orgId)
		{
			if (!this.config.bypassAcceptedDomainUPNCheck)
			{
				if (orgId == null)
				{
					this.TraceAndReturnWarning(this.GetHashCode(), "<SyncUPN user \"{0}\" is missing orgID>", new object[]
					{
						entryId.DistinguishedName.ToString()
					});
					return false;
				}
				if (!this.namespaceInfo.OrgId.Equals(OrganizationId.ForestWideOrgId) && !orgId.Equals(this.namespaceInfo.OrgId))
				{
					this.TraceAndReturnWarning(this.GetHashCode(), "<SyncUPN user \"{0}\" OrgID \"{1}\" does not match namespace \"{2}\" OrgId \"{3}\">", new object[]
					{
						entryId.DistinguishedName.ToString(),
						orgId.ToString(),
						this.namespaceInfo.DomainName,
						this.namespaceInfo.OrgId.ToString()
					});
					return false;
				}
				if (this.namespaceInfo.OrgId.Equals(OrganizationId.ForestWideOrgId))
				{
					ExTraceGlobals.AuthenticationTracer.Information<string, string, string>((long)this.GetHashCode(), "Looking up accepted domain for user \"{0}\" by domain name {1}, orgId {2}", this.userName, this.namespaceInfo.DomainName, orgId.ToString());
					using (new ActivityScopeThreadGuard(this.activityScope))
					{
						ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 5654, "CheckUpnAgainstAcceptedDomains", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\AuthService.cs");
						List<string> list = new List<string>(10);
						if (!string.IsNullOrEmpty(this.namespaceInfo.DomainName))
						{
							list.Add(this.namespaceInfo.DomainName);
							list.Add("*." + this.namespaceInfo.DomainName);
						}
						list.Add(this.userDomain);
						list.Add("*." + this.userDomain);
						int num = this.userDomain.IndexOf('.');
						int num2 = this.userDomain.LastIndexOf('.');
						while (num != -1 && num != num2)
						{
							list.Add(this.userDomain.Substring(num + 1));
							list.Add("*." + this.userDomain.Substring(num + 1));
							num = this.userDomain.IndexOf('.', num + 1);
						}
						QueryFilter[] array = new QueryFilter[list.Count];
						int num3 = 0;
						foreach (string propertyValue in list)
						{
							array[num3] = new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, propertyValue);
							num3++;
						}
						QueryFilter filter = new OrFilter(array);
						AcceptedDomain[] array2 = tenantConfigurationSession.Find<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 2);
						if (array2 == null || array2.Length == 0)
						{
							this.TraceAndReturnWarning(this.GetHashCode(), "<SyncUPN user \"{0}\" OrgID \"{1}\" has no accepted domain which matches any domain or subdomain of \"{2}\">", new object[]
							{
								entryId.DistinguishedName.ToString(),
								orgId.ToString(),
								this.userDomain
							});
							return false;
						}
					}
					return true;
				}
			}
			return true;
		}

		private void StartFederatedRequestChain()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering StartFederatedRequestChain()");
			AuthService.counters.PendingFedStsRequests.Increment();
			Interlocked.Exchange(ref this.fedStsPending, 1);
			bool offlineAuthEnabled = !this.hasFailedOverForAuth && this.allowOfflineOrgId;
			this.liveIdSTS = new LiveIdSTS(this.GetHashCode(), this.namespaceInfo.Instance, this.config, this.namespaceStats, offlineAuthEnabled);
			this.liveIdSTS.ExtraHeaders = this.CreateUserContextHeaders();
			this.federatedSTS = new FederatedSTS(this.GetHashCode(), this.namespaceInfo.Instance, this.namespaceStats);
			this.federatedSTS.FederatedLogonURI = this.namespaceInfo.AuthURL;
			this.federatedSTS.TokenIssuerURI = this.liveIdSTS.TokenIssuerUri;
			this.federatedSTS.MaxResponseSize = this.config.maxAdfsResponseSize;
			this.federatedSTS.ClockSkew = this.namespaceInfo.ClockSkew;
			this.federatedSTS.ClockSkewThreshold = TimeSpan.FromMinutes((double)this.config.adfsSkewThreshold);
			this.federatedSTS.ExtraHeaders = this.CreateUserContextHeaders();
			this.nextState = AuthService.authStateEnum.authSendFedSTSRequest;
			this.federatedSTS.StartRequestChain(this.escapedUserBytes, this.escapedPassBytes, AuthService.asyncRequestCallback, this);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving StartFederatedRequestChain()");
		}

		private void ProcessFederatedSTSRequest(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ProcessFederatedSTSRequest()");
			this.nextState = AuthService.authStateEnum.authProcessFedSTSResponse;
			this.federatedSTS.ProcessRequest(asyncResult, AuthService.asyncRequestCallback, this);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessFederatedSTSRequest()");
		}

		private void ProcessFederatedSTSResponse(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ProcessFederatedSTSResponse()");
			this.nextState = AuthService.authStateEnum.authSendLiveFedRequest;
			byte[] array = this.federatedSTS.ProcessResponse(asyncResult);
			this.RecordLatency(this.federatedSTS.StsTag, this.federatedSTS.Instance, this.federatedSTS.Latency);
			if (Interlocked.Exchange(ref this.fedStsPending, 0) == 1)
			{
				AuthService.counters.PendingFedStsRequests.Decrement();
			}
			if (array != null)
			{
				this.wsseToken = array;
				this.ContinueRequestChain();
			}
			else if (this.federatedSTS.PossibleClockSkew && this.attempt == 0)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, TimeSpan>((long)this.GetHashCode(), "Retrying federated logon for \"{0}\" due to clock skew of {0} minutes", this.userName, this.federatedSTS.ClockSkew);
				this.namespaceInfo.ClockSkew = this.federatedSTS.ClockSkew;
				this.attempt++;
				this.nextState = AuthService.authStateEnum.authSendFedSTSRequest;
				this.federatedSTS.StartRequestChain(this.escapedUserBytes, this.escapedPassBytes, AuthService.asyncRequestCallback, this);
			}
			else
			{
				CredFailure credFailure = CredFailure.STSFailure;
				StringBuilder stringBuilder = new StringBuilder();
				if (this.federatedSTS.IsExpiredCreds)
				{
					credFailure = CredFailure.Expired;
				}
				else if (this.federatedSTS.IsBadCredentials)
				{
					credFailure = CredFailure.Invalid;
				}
				else if (this.federatedSTS.IsFederatedStsADFSRulesDenied)
				{
					credFailure = CredFailure.ADFSRulesDeny;
				}
				else if (this.federatedSTS.IsForbidden)
				{
					credFailure = CredFailure.Forbidden;
				}
				if (!string.IsNullOrEmpty(this.federatedSTS.RecoveryUrl))
				{
					stringBuilder.AppendFormat("<RECOVERY {0}>", this.federatedSTS.RecoveryUrl);
					credFailure = CredFailure.LockedOut;
				}
				if (this.federatedSTS.IsBadCredentials || this.federatedSTS.IsExpiredCreds)
				{
					this.InvalidatePasswordInAD();
				}
				this.TraceAndReturnInformation(this.GetHashCode(), "{0}failed logon error - {1} - '{2}'", new object[]
				{
					stringBuilder.ToString(),
					credFailure,
					this.federatedSTS.ErrorString
				});
				bool flag = this.useLogonCache && AuthService.logonCache.Add(this.UserKey, ExDateTime.UtcNow, this.passwordHash, credFailure, stringBuilder.ToString(), this.userType, this.GetHashCode());
				this.InvokeCallback((IntPtr)((long)this.FailureModeToAuthError(credFailure, this.namespaceStats.IsTarpitted || flag)));
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessFederatedSTSResponse()");
		}

		private void StartSamlRequestChain()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering StartSamlRequestChain()");
			AuthService.counters.PendingSamlStsRequests.Increment();
			Interlocked.Exchange(ref this.samlStsPending, 1);
			this.liveIdSTS = new LiveIdSTSSamlForm(this.GetHashCode(), this.namespaceInfo.Instance, this.config, this.namespaceStats);
			this.liveIdSTS.ExtraHeaders = this.CreateUserContextHeaders();
			this.samlSTS = new SamlSTS(this.GetHashCode(), this.namespaceInfo.Instance, this.namespaceStats);
			this.samlSTS.ShibbolethLogonURI = this.namespaceInfo.AuthURL;
			this.samlSTS.TokenIssuerURI = this.liveIdSTS.TokenIssuerUri;
			this.samlSTS.AssertionConsumerService = this.config.liveHttpPostAssertionConsumerService;
			this.samlSTS.MaxResponseSize = this.config.maxShibbolethResponseSize;
			this.samlSTS.ClockSkew = this.namespaceInfo.ClockSkew;
			this.samlSTS.ClockSkewThreshold = TimeSpan.FromMinutes((double)this.config.shibbSkewThreshold);
			this.samlSTS.ExtraHeaders = this.CreateUserContextHeaders();
			this.nextState = AuthService.authStateEnum.authSendSamlSTSRequest;
			this.samlSTS.StartRequestChain(this.ansiUserName, this.ansiPassword, AuthService.asyncRequestCallback, this);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving StartSamlRequestChain()");
		}

		private void ProcessSamlSTSRequest(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ProcessSamlSTSRequest()");
			this.nextState = AuthService.authStateEnum.authProcessSamlSTSResponse;
			this.samlSTS.ProcessRequest(asyncResult, AuthService.asyncRequestCallback, this);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessSamlSTSRequest()");
		}

		private void ProcessSamlSTSResponse(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ProcessSamlSTSResponse()");
			this.nextState = AuthService.authStateEnum.authSendLiveSamlRequest;
			byte[] array = this.samlSTS.ProcessResponse(asyncResult);
			this.RecordLatency(this.samlSTS.StsTag, this.samlSTS.Instance, this.samlSTS.Latency);
			if (Interlocked.Exchange(ref this.samlStsPending, 0) == 1)
			{
				AuthService.counters.PendingSamlStsRequests.Decrement();
			}
			if (array != null)
			{
				this.wsseToken = array;
				this.ContinueRequestChain();
			}
			else if (this.samlSTS.PossibleClockSkew && this.attempt == 0)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, TimeSpan>((long)this.GetHashCode(), "Retrying shibboleth logon for \"{0}\" due to clock skew of {0} minutes", this.userName, this.samlSTS.ClockSkew);
				this.namespaceInfo.ClockSkew = this.samlSTS.ClockSkew;
				this.attempt++;
				this.nextState = AuthService.authStateEnum.authSendSamlSTSRequest;
				this.samlSTS.StartRequestChain(this.ansiUserName, this.ansiPassword, AuthService.asyncRequestCallback, this);
			}
			else
			{
				CredFailure mode = CredFailure.STSFailure;
				StringBuilder stringBuilder = new StringBuilder();
				if (this.samlSTS.IsExpiredCreds)
				{
					mode = CredFailure.Expired;
				}
				else if (this.samlSTS.IsBadCredentials)
				{
					mode = CredFailure.Invalid;
				}
				if (!string.IsNullOrEmpty(this.samlSTS.RecoveryUrl))
				{
					stringBuilder.AppendFormat("<RECOVERY {0}>", this.samlSTS.RecoveryUrl);
					mode = CredFailure.LockedOut;
				}
				if (this.samlSTS.IsBadCredentials || this.samlSTS.IsExpiredCreds)
				{
					this.InvalidatePasswordInAD();
				}
				this.TraceAndReturnInformation(this.GetHashCode(), "SamlSTS failed logon error '{0}'", new object[]
				{
					this.samlSTS.ErrorString
				});
				bool flag = this.useLogonCache && AuthService.logonCache.Add(this.UserKey, ExDateTime.UtcNow, this.passwordHash, mode, null, this.userType, this.GetHashCode());
				this.InvokeCallback((IntPtr)((long)this.FailureModeToAuthError(mode, this.namespaceStats.IsTarpitted || flag)));
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ProcessSamlSTSResponse()");
		}

		private void WaitLogonRequestChain()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering WaitLogonRequestChain()");
			this.nextState = AuthService.authStateEnum.authWaitLogonResponse;
			this.asyncWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.clientOp.StsEvent, new WaitOrTimerCallback(AuthService.ClientOpWaitCallback), this, TimeSpan.FromSeconds((double)this.Timeout), true);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving WaitLogonRequestChain()");
		}

		private void ResumeLogonRequestChain()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Entering ResumeLogonRequestChain()");
			if (this.useLogonCache && this.CheckLogonCache())
			{
				AuthService.counters.NumberOfCachedRequests.Increment();
				ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Credential check performed against cache for \"{0}\"", this.userName);
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ResumeLogonRequestChain()");
				return;
			}
			this.StartChain();
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.GetHashCode(), "Leaving ResumeLogonRequestChain()");
		}

		private bool TryUpdateLastLogonTime(string puid, DateTime lastLogonTime)
		{
			if (AuthService.IsBackendServer && lastLogonTime - MailboxUserProfile.GetLastLogonTime(puid) > TimeSpan.FromHours((double)this.config.LastLogonTimeUpdateFrequency))
			{
				if (!this.config.EnableMailboxBasedPasswordConfidence)
				{
					MailboxUserProfile.SetLastLogonTime(puid, lastLogonTime);
					return true;
				}
				try
				{
					ADRawEntry adrawEntry = null;
					this.GetADUserEntry(puid, out adrawEntry);
					if (adrawEntry == null)
					{
						return false;
					}
					ADObjectId adobjectId = (ADObjectId)adrawEntry[ADMailboxRecipientSchema.Database];
					if (adrawEntry[ADMailboxRecipientSchema.ExchangeGuid] == null || adobjectId == null || Guid.Empty.Equals((Guid)adrawEntry[ADMailboxRecipientSchema.ExchangeGuid]))
					{
						this.TraceAndReturnInformation(this.GetHashCode(), "<SetLastLogonTimeInMailbox:NoMailbox>", new object[0]);
						return false;
					}
					Stopwatch.StartNew();
					string text;
					bool flag = MailboxUserProfile.SetLastLogonTimeInMailbox(puid, (Guid)adrawEntry[ADMailboxRecipientSchema.ExchangeGuid], adobjectId.ObjectGuid, (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId], lastLogonTime, out text);
					this.stopwatch.Stop();
					this.TraceAndReturnInformation(this.GetHashCode(), "<SetLastLogonTimeInMailbox:{0}:{1}ms>", new object[]
					{
						flag,
						this.stopwatch.ElapsedMilliseconds
					});
					if (text != null)
					{
						this.TraceAndReturnInformation(this.GetHashCode(), text, new object[0]);
					}
					return flag;
				}
				catch (Exception ex)
				{
					this.TraceAndReturnError(this.GetHashCode(), "<SetLastLogonTimeInMailbox:false>{0}", new object[]
					{
						ex.ToString()
					});
					AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_UpdateLastLogonTimeFailed, this.userName, new object[]
					{
						this.userName,
						ex.ToString()
					});
					return false;
				}
				return false;
			}
			return false;
		}

		private void StartTestFailover()
		{
			if (!this.calledFromTestFunction || !this.allowOfflineOrgId)
			{
				return;
			}
			int num = this.Timeout + 5;
			switch (this.authState)
			{
			case AuthService.authStateEnum.authSendHrdRequest:
				if ((this.testFailOver & TestFailoverFlags.HRDRequest) != TestFailoverFlags.None && !this.hasFailedOverForHrd)
				{
					throw new WebException("Test Failover when sending HRD request.");
				}
				if ((this.testFailOver & TestFailoverFlags.HRDRequestTimeout) != TestFailoverFlags.None && !this.hasFailedOverForHrd)
				{
					Thread.Sleep(num * 1000);
					return;
				}
				break;
			case AuthService.authStateEnum.authProcessHrdResponse:
				if (((this.testFailOver & TestFailoverFlags.HRDResponse) != TestFailoverFlags.None || (this.testFailOver & TestFailoverFlags.OfflineHRD) != TestFailoverFlags.None) && !this.hasFailedOverForHrd)
				{
					throw new WebException("Test Failover when processing HRD response.");
				}
				break;
			case AuthService.authStateEnum.authSendLiveSTSRequest:
				if (!this.hasFailedOverForAuth)
				{
					if (((this.testFailOver & TestFailoverFlags.LiveIdRequest) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Consumer) || ((this.testFailOver & TestFailoverFlags.OrgIdRequest) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Business))
					{
						throw new WebException("Test Failover when sending LiveIdSTS request.");
					}
					if (((this.testFailOver & TestFailoverFlags.LiveIdRequestTimeout) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Consumer) || ((this.testFailOver & TestFailoverFlags.OrgIdRequestTimeout) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Business))
					{
						Thread.Sleep(num * 1000);
						return;
					}
				}
				break;
			case AuthService.authStateEnum.authProcessLiveSTSResponse:
				if (!this.hasFailedOverForAuth && (((this.testFailOver & TestFailoverFlags.LiveIdResponse) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Consumer) || ((this.testFailOver & TestFailoverFlags.OrgIdResponse) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Business)))
				{
					throw new WebException("Test Failover when processing LiveIdSTS response.");
				}
				break;
			case AuthService.authStateEnum.authSendFedSTSRequest:
				if ((this.testFailOver & TestFailoverFlags.FederatedRequestTimeout) != TestFailoverFlags.None && !this.hasFailedOverForAuth)
				{
					Thread.Sleep(num * 1000);
				}
				if ((this.testFailOver & TestFailoverFlags.FederatedRequest) != TestFailoverFlags.None && !this.hasFailedOverForAuth)
				{
					throw new WebException("Test Failover when doing federated authentication");
				}
				break;
			case AuthService.authStateEnum.authProcessFedSTSResponse:
			case AuthService.authStateEnum.authSendSamlSTSRequest:
			case AuthService.authStateEnum.authProcessSamlSTSResponse:
			case AuthService.authStateEnum.authFinishLogon:
			case AuthService.authStateEnum.authWaitHrdResponse:
			case AuthService.authStateEnum.authWaitLogonResponse:
				break;
			case AuthService.authStateEnum.authSendLiveFedRequest:
				if (!this.hasFailedOverForAuth)
				{
					if (((this.testFailOver & TestFailoverFlags.LiveIdRequest) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Consumer) || ((this.testFailOver & TestFailoverFlags.OrgIdRequest) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Business))
					{
						throw new WebException("Test Failover when sending LiveIdFedSTS request.");
					}
					if (((this.testFailOver & TestFailoverFlags.LiveIdRequestTimeout) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Consumer) || ((this.testFailOver & TestFailoverFlags.OrgIdRequestTimeout) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Business))
					{
						Thread.Sleep(num * 1000);
						return;
					}
				}
				break;
			case AuthService.authStateEnum.authProcessLiveFedResponse:
				if (!this.hasFailedOverForAuth && (((this.testFailOver & TestFailoverFlags.LiveIdResponse) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Consumer) || ((this.testFailOver & TestFailoverFlags.OrgIdResponse) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Business)))
				{
					throw new WebException("Test Failover when processing LiveIdFedSTS response.");
				}
				break;
			case AuthService.authStateEnum.authSendLiveSamlRequest:
				if (!this.hasFailedOverForAuth && (((this.testFailOver & TestFailoverFlags.LiveIdRequest) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Consumer) || ((this.testFailOver & TestFailoverFlags.OrgIdRequest) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Business)))
				{
					throw new WebException("Test Failover when sending LiveIdSamlSTS request.");
				}
				break;
			case AuthService.authStateEnum.authProcessLiveSamlResponse:
				if (!this.hasFailedOverForAuth && (((this.testFailOver & TestFailoverFlags.LiveIdResponse) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Consumer) || ((this.testFailOver & TestFailoverFlags.OrgIdResponse) != TestFailoverFlags.None && this.liveIdSTS.Instance == LiveIdInstanceType.Business)))
				{
					throw new WebException("Test Failover when processing LiveIdSamlSTS response.");
				}
				break;
			case AuthService.authStateEnum.authOfflineAuth:
				if ((this.testFailOver & TestFailoverFlags.OfflineAuthentication) != TestFailoverFlags.None)
				{
					throw new WebException("Test Failover when doing offline authentication");
				}
				break;
			default:
				return;
			}
		}

		private bool HandleFailOver(Exception ex)
		{
			if (!this.allowOfflineOrgId)
			{
				return false;
			}
			bool result = false;
			if (!this.isOrgIdOffline && !(ex is WebException) && !(ex is HttpException))
			{
				if (!(ex is SocketException))
				{
					return false;
				}
			}
			try
			{
				switch (this.authState)
				{
				case AuthService.authStateEnum.authSendHrdRequest:
				case AuthService.authStateEnum.authProcessHrdResponse:
					if (!this.hasFailedOverForHrd)
					{
						this.hasFailedOverForHrd = true;
						this.isOrgIdOffline = !this.isOrgIdOffline;
						if (this.isOrgIdOffline)
						{
							this.ReasonOfUsingOfflineOrgId = new AuthStatus?(AuthStatus.HRDFailed);
							this.TraceAndReturnInformation(this.GetHashCode(), "<HrdFailure><ppserver={0}>{1}", new object[]
							{
								this.realmDiscovery.LiveServer ?? string.Empty,
								this.realmDiscovery.ErrorString ?? ex.ToString()
							});
						}
						else
						{
							this.TraceAndReturnWarning(this.GetHashCode(), ("<OfflineHrd failed>" + this.realmDiscovery.ErrorString) ?? ex.ToString(), new object[0]);
						}
						result = true;
						this.StartHrdRequestChain(this.config.defaultInstance);
					}
					break;
				case AuthService.authStateEnum.authSendLiveSTSRequest:
				case AuthService.authStateEnum.authProcessLiveSTSResponse:
				case AuthService.authStateEnum.authSendLiveFedRequest:
				case AuthService.authStateEnum.authProcessLiveFedResponse:
				case AuthService.authStateEnum.authSendLiveSamlRequest:
				case AuthService.authStateEnum.authProcessLiveSamlResponse:
					if (!this.hasFailedOverForAuth)
					{
						this.hasFailedOverForAuth = true;
						this.isOrgIdOffline = true;
						this.TraceAndReturnInformation(this.GetHashCode(), "<OrgIdOrLiveIdFailure><ppserver={0}>{1}", new object[]
						{
							this.liveIdSTS.LiveServer ?? string.Empty,
							this.liveIdSTS.ErrorString ?? ex.ToString()
						});
						result = true;
						this.ReasonOfUsingOfflineOrgId = new AuthStatus?(AuthStatus.LiveIDFailed);
						this.DoOfflineOrgIdAuth();
					}
					break;
				case AuthService.authStateEnum.authSendFedSTSRequest:
				case AuthService.authStateEnum.authProcessFedSTSResponse:
				case AuthService.authStateEnum.authSendSamlSTSRequest:
				case AuthService.authStateEnum.authProcessSamlSTSResponse:
					if (!this.hasFailedOverForAuth && AuthServiceStaticConfig.Config.AllowOfflineOrgIdForADFS)
					{
						this.hasFailedOverForAuth = true;
						if (this.IsFederatedSTSState())
						{
							this.TraceAndReturnInformation(this.GetHashCode(), "<FederatedSTSFailure>" + ex.ToString(), new object[0]);
							AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_FederatedSTSUnreachable, this.federatedSTS.FederatedLogonURI, new object[]
							{
								this.federatedSTS.FederatedLogonURI,
								this.userName,
								ex.ToString()
							});
						}
						else if (this.IsSamlSTSState())
						{
							this.TraceAndReturnWarning(this.GetHashCode(), "Shibboleth STS '{0}' is unreachable.  User \"{1}\" &ExceptionDetails={2}&", new object[]
							{
								this.samlSTS.ShibbolethLogonURI,
								this.userName,
								ex.ToString()
							});
							AuthService.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_ShibbolethSTSUnreachable, this.samlSTS.ShibbolethLogonURI, new object[]
							{
								this.samlSTS.ShibbolethLogonURI,
								this.userName,
								ex.ToString()
							});
						}
						result = true;
						this.ReasonOfUsingOfflineOrgId = new AuthStatus?(AuthStatus.FederatedStsFailed);
						this.DoOfflineOrgIdAuth();
					}
					break;
				case AuthService.authStateEnum.authOfflineAuth:
					if (!this.hasFailedOverForAuth)
					{
						this.isOrgIdOffline = false;
						this.hasFailedOverForAuth = true;
						result = true;
						this.StartHrdRequestChain(this.config.defaultInstance);
					}
					break;
				}
			}
			catch (Exception value)
			{
				this.InvokeCallback(value);
			}
			return result;
		}

		private const uint monitoringProcessId = 4294967295U;

		private const int managedPasswordLimit = 16;

		private const string wsseTokenP1 = "<wsse:UsernameToken wsu:Id=\"user\"><wsse:Username>";

		private const string wsseTokenP2 = "</wsse:Username><wsse:Password>";

		private const string wsseTokenP3 = "</wsse:Password></wsse:UsernameToken>";

		private const string recoveryTag = "<RECOVERY {0}>";

		private const string expiresTag = "<CREDEXPIRES {0}>";

		private const string appPasswordTag = "<APPPASSWORD>";

		internal const string PasswordSynTag = "PasswordSynced.";

		private static readonly ExEventLog eventLogger = AuthServiceHelper.EventLogger;

		private static readonly LiveIdBasicAuthenticationCountersInstance counters = AuthServiceHelper.PerformanceCounters;

		private static readonly int wsseTokenByteCount;

		private static char[] colonSeparator = new char[]
		{
			':'
		};

		private static readonly byte[] wsseTokenBytesP1;

		private static readonly byte[] wsseTokenBytesP2;

		private static readonly byte[] wsseTokenBytesP3;

		private static readonly PropertyDefinition[] propertiesToGet;

		private static readonly TimerCallback timerCallback = new TimerCallback(AuthService.TimeOutCallback);

		private static readonly AsyncCallback asyncRequestCallback = new AsyncCallback(AuthService.RequestCallback);

		private static Timer configTimer = null;

		private static XmlWriterSettings xmlSettings;

		private static Timer percentileCountersTimer = null;

		private static int numberOfAuthRequests;

		private static int numberOfCurrentRequests;

		private static int numberOfFailedRequests;

		private static int numberOfTimedOutRequests;

		private static int numberOfFailedAuthentications;

		private static int numberOfSuccessfulAuthentications;

		private static int numberOfFailedRecoverableAuthentications;

		private static SlidingPercentageCounter percentageLogonCacheHitLastMinute = null;

		private static SlidingPercentageCounter percentageAdUserHitLastMinute = null;

		private static SlidingPercentageCounter percentileFailedRequestsLastMinutes = null;

		private static SlidingPercentageCounter percentileTimedOutRequestsLastMinutes = null;

		private static SlidingPercentageCounter percentileFailedAuthenticationsLastMinutes = null;

		internal static SlidingTotalCounter orgIdRequestCountLastMinutes;

		internal static SlidingTotalCounter offlineOrgIdRequestCountLastMinutes;

		private static NamespaceCache hrdCache;

		private static UserOpsTracker userOperations;

		private static LogonCache logonCache;

		private static TimeoutCache<WebRequest, string> certErrorCache;

		private static TimeoutCache<string, ADRawEntry> adUserPropertyCache;

		private static TimeoutCache<string, bool> domainsByPassTOU;

		private static bool IsBackendServer;

		private AuthService.authStateEnum authState;

		private AuthService.authStateEnum nextState;

		private int attempt;

		private AuthServiceStaticConfig config;

		private Timer timer;

		private UserOp clientOp;

		private bool releaseClientOp;

		private Stopwatch stopwatch;

		private LazyAsyncResult outerResult;

		private int intCompleted;

		private byte[] escapedUserBytes;

		private byte[] escapedPassBytes;

		private bool isAppPassword;

		private byte[] wsseToken;

		private string userName;

		private string userDomain;

		private DomainConfig namespaceInfo;

		private NamespaceStats namespaceStats;

		private IHomeRealmDiscovery realmDiscovery;

		private FederatedSTS federatedSTS;

		private SamlSTS samlSTS;

		private LiveIdSTSBase liveIdSTS;

		private bool isKnownBadCreds;

		private int hrdPending;

		private int fedStsPending;

		private int samlStsPending;

		private int liveStsPending;

		private bool hrdSetEventOnExit;

		private bool stsSetEventOnExit;

		private RegisteredWaitHandle asyncWaitHandle;

		private int extraWait;

		private Timer waitTimer;

		private StringBuilder iisLogMsg;

		private string puid;

		private CommonAccessToken tokenToReturn;

		private uint clientProcessId;

		private byte[] ansiUserName;

		private byte[] ansiPassword;

		private string compactToken;

		private byte[] passwordHash;

		private bool useLogonCache = true;

		private bool usePositiveLogonCache = true;

		private bool syncAD;

		private bool syncADBackEndOnly;

		private bool passwordAndHrdSync;

		private bool syncUPN;

		private bool returnWindowsToken = true;

		private bool includePWinCat;

		private string organizationContext;

		private int retryCountForLongPassword;

		private string userEndpoint;

		private string userAgent;

		private string userHostAddress;

		private string userClientIp;

		private ActivityScope activityScope;

		private bool isOrgIdOffline;

		private bool allowOfflineOrgId;

		private bool hasFailedOverForAuth;

		private bool hasFailedOverForHrd;

		private bool hasPassedFirstAuth;

		private IntPtr offlineOrgIdResult = (IntPtr)(-16L);

		private AuthStatus? ReasonOfUsingOfflineOrgId;

		private AutoResetEvent offlineOrgIdDoneEvent;

		private bool requestUpdateHrdInBackend;

		private bool requestPasswordConfidenceCheckInBackend;

		private int passwordConfidence;

		private UserType userType;

		private ExDateTime lastAuthTime;

		private RPSProfile rpsUserProfile;

		private bool enableXmlAuthForLiveId;

		private TestFailoverFlags testFailOver;

		private bool useHrdCache = true;

		private bool calledFromTestFunction;

		private enum authStateEnum
		{
			authUnknown,
			authSendHrdRequest,
			authProcessHrdResponse,
			authSendLiveSTSRequest,
			authProcessLiveSTSResponse,
			authSendFedSTSRequest,
			authProcessFedSTSResponse,
			authSendLiveFedRequest,
			authProcessLiveFedResponse,
			authSendSamlSTSRequest,
			authProcessSamlSTSResponse,
			authSendLiveSamlRequest,
			authProcessLiveSamlResponse,
			authFinishLogon,
			authWaitHrdResponse,
			authWaitLogonResponse,
			authOfflineAuth
		}

		private class UTF8EncodingMinusPreamble : UTF8Encoding
		{
			public override byte[] GetPreamble()
			{
				return AuthService.UTF8EncodingMinusPreamble.emptyByteArray;
			}

			private static byte[] emptyByteArray = new byte[0];
		}
	}
}
