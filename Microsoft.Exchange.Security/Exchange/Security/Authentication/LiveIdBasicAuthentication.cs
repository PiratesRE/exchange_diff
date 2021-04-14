using System;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class LiveIdBasicAuthentication : ILiveIdBasicAuthentication
	{
		private static AuthServiceClient GetClient()
		{
			AuthServiceClient authServiceClient;
			lock (LiveIdBasicAuthentication.clientLock)
			{
				if (LiveIdBasicAuthentication.sharedClient == null)
				{
					LiveIdBasicAuthentication.sharedClient = LiveIdBasicAuthentication.NewClientDelegate();
					LiveIdBasicAuthentication.sharedClient.AddRef();
				}
				authServiceClient = LiveIdBasicAuthentication.sharedClient;
				authServiceClient.AddRef();
			}
			AuthServiceClient authServiceClient2 = authServiceClient;
			lock (authServiceClient2)
			{
				if (authServiceClient2.State == CommunicationState.Closing || authServiceClient2.State == CommunicationState.Closed || authServiceClient2.State == CommunicationState.Faulted)
				{
					authServiceClient2.Release();
					authServiceClient = LiveIdBasicAuthentication.NewClientDelegate();
					authServiceClient.AddRef();
					LiveIdBasicAuthentication.SetClient(authServiceClient);
				}
			}
			return authServiceClient;
		}

		private static void SetClient(AuthServiceClient value)
		{
			lock (LiveIdBasicAuthentication.clientLock)
			{
				if (value != null)
				{
					value.AddRef();
				}
				if (LiveIdBasicAuthentication.sharedClient != null)
				{
					LiveIdBasicAuthentication.sharedClient.Release();
				}
				LiveIdBasicAuthentication.sharedClient = value;
			}
		}

		private static void InvalidateClient(AuthServiceClient value)
		{
			lock (LiveIdBasicAuthentication.clientLock)
			{
				if (value == LiveIdBasicAuthentication.sharedClient)
				{
					LiveIdBasicAuthentication.sharedClient.Release();
					LiveIdBasicAuthentication.sharedClient = null;
				}
			}
		}

		~LiveIdBasicAuthentication()
		{
			if (this.instanceClient != null)
			{
				this.instanceClient.Release();
			}
		}

		public string ApplicationName
		{
			get
			{
				return this.application;
			}
			set
			{
				this.application = value;
			}
		}

		public string UserIpAddress
		{
			get
			{
				return this.userAddress;
			}
			set
			{
				this.userAddress = value;
			}
		}

		public string UserAgent
		{
			get
			{
				return this.userAgent;
			}
			set
			{
				this.userAgent = value;
			}
		}

		public bool SyncAD
		{
			get
			{
				return this.syncAD;
			}
			set
			{
				this.syncAD = value;
			}
		}

		public bool SyncADBackEndOnly
		{
			get
			{
				return this.syncADBackEndOnly;
			}
			set
			{
				this.syncADBackEndOnly = value;
			}
		}

		public bool SyncUPN { get; set; }

		public bool BypassPositiveLogonCache { get; set; }

		public bool AllowLiveIDOnlyAuth
		{
			get
			{
				return this.allowLiveIDOnlyAuth;
			}
			set
			{
				this.allowLiveIDOnlyAuth = value;
			}
		}

		public bool AllowOfflineOrgIdAsPrimeAuth
		{
			get
			{
				return this.allowOfflineOrgIdAsPrimeAuth;
			}
			set
			{
				this.allowOfflineOrgIdAsPrimeAuth = value;
			}
		}

		public string LastRequestErrorMessage
		{
			get
			{
				return this.lastRequestErrorMessage;
			}
		}

		public LiveIdAuthResult LastAuthResult
		{
			get
			{
				return this.lastResult;
			}
		}

		public bool RecoverableLogonFailure { get; private set; }

		public bool Tarpit { get; private set; }

		public bool AuthenticatedByOfflineAuth { get; private set; }

		public LiveIdAuthResult? OfflineOrgIdFailureResult { get; set; }

		public SecurityStatus GetWindowsIdentity(byte[] userBytes, byte[] passBytes, out WindowsIdentity identity, out IAccountValidationContext accountValidationContext)
		{
			return this.GetWindowsIdentity(userBytes, passBytes, Guid.Empty, out identity, out accountValidationContext);
		}

		public SecurityStatus GetWindowsIdentity(byte[] userBytes, byte[] passBytes, Guid requestId, out WindowsIdentity identity, out IAccountValidationContext accountValidationContext)
		{
			accountValidationContext = null;
			bool flag;
			identity = this.GetWindowsIdentity(userBytes, passBytes, string.Empty, requestId, out flag);
			if (identity != null)
			{
				if (ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("AccountValidationEnabled"))
				{
					accountValidationContext = new AccountValidationContextBySID(identity.Owner, ExDateTime.UtcNow, this.ApplicationName);
				}
				return SecurityStatus.OK;
			}
			return SecurityStatus.LogonDenied;
		}

		public WindowsIdentity GetWindowsIdentity(byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, out bool userNotFoundInAD)
		{
			return this.GetWindowsIdentity(userBytes, passBytes, remoteOrganizationContext, Guid.Empty, out userNotFoundInAD);
		}

		public WindowsIdentity GetWindowsIdentity(byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, Guid requestId, out bool userNotFoundInAD)
		{
			IAsyncResult asyncResult = this.BeginGetWindowsIdentity(userBytes, passBytes, remoteOrganizationContext, requestId, null, null);
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult != null)
			{
				lazyAsyncResult.InternalWaitForCompletion();
			}
			else
			{
				asyncResult.AsyncWaitHandle.WaitOne();
				asyncResult.AsyncWaitHandle.Close();
			}
			WindowsIdentity result;
			LiveIdAuthResult liveIdAuthResult = this.EndGetWindowsIdentity(asyncResult, out result);
			userNotFoundInAD = (liveIdAuthResult == LiveIdAuthResult.UserNotFoundInAD);
			return result;
		}

		public IAsyncResult BeginGetWindowsIdentity(byte[] userBytes, byte[] passBytes, AsyncCallback callback, object state, Guid requestId = default(Guid))
		{
			return this.BeginGetWindowsIdentity(userBytes, passBytes, string.Empty, requestId, callback, state);
		}

		public IAsyncResult BeginGetWindowsIdentity(byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, AsyncCallback callback, object state)
		{
			return this.BeginGetWindowsIdentity(userBytes, passBytes, remoteOrganizationContext, Guid.Empty, callback, state);
		}

		public IAsyncResult BeginGetWindowsIdentity(byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, Guid requestId, AsyncCallback callback, object state)
		{
			return this.BeginGetAuthToken(userBytes, passBytes, callback, state, delegate
			{
				IAsyncResult result;
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					result = this.instanceClient.BeginLogonUserFederationCredsAsync((uint)currentProcess.Id, userBytes, passBytes, remoteOrganizationContext, this.syncAD, this.application, this.userAgent, this.userAddress, requestId, callback, state);
				}
				return result;
			});
		}

		public LiveIdAuthResult EndGetWindowsIdentity(IAsyncResult ar, out WindowsIdentity identity)
		{
			WindowsIdentity localIdentity = null;
			LiveIdAuthResult result = this.EndGetAuthToken(ar, delegate
			{
				AuthStatus result2 = AuthStatus.LogonFailed;
				SafeUserTokenHandle safeUserTokenHandle = new SafeUserTokenHandle(this.instanceClient.EndLogonUserFederationCredsAsync(out this.lastRequestErrorMessage, ar));
				using (safeUserTokenHandle)
				{
					int num = safeUserTokenHandle.DangerousGetHandle().ToInt32();
					if (num >= -29 && num <= 0)
					{
						result2 = (AuthStatus)num;
					}
					else if (!safeUserTokenHandle.IsInvalid)
					{
						localIdentity = new WindowsIdentity(safeUserTokenHandle.DangerousGetHandle(), "Kerberos", WindowsAccountType.Normal, true);
						result2 = AuthStatus.LogonSuccess;
					}
					if (num == -1 && this.allowLiveIDOnlyAuth)
					{
						localIdentity = WindowsIdentity.GetCurrent();
					}
				}
				return result2;
			});
			identity = localIdentity;
			return result;
		}

		public SecurityStatus GetCommonAccessToken(byte[] userBytes, byte[] passBytes, Guid requestId, out string commonAccessToken, out IAccountValidationContext accountValidationContext)
		{
			accountValidationContext = null;
			bool flag;
			commonAccessToken = this.GetCommonAccessToken(userBytes, passBytes, string.Empty, out flag, requestId);
			if (!string.IsNullOrEmpty(commonAccessToken))
			{
				if (ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("AccountValidationEnabled"))
				{
					CommonAccessToken commonAccessToken2 = CommonAccessToken.Deserialize(commonAccessToken);
					ExDateTime utcNow;
					if (!commonAccessToken2.ExtensionData.ContainsKey("CreateTime") || !ExDateTime.TryParse(commonAccessToken2.ExtensionData["CreateTime"], out utcNow))
					{
						utcNow = ExDateTime.UtcNow;
					}
					string puid = commonAccessToken2.ExtensionData["Puid"];
					accountValidationContext = new AccountValidationContextByPUID(puid, utcNow, this.ApplicationName);
				}
				return SecurityStatus.OK;
			}
			return SecurityStatus.LogonDenied;
		}

		public string GetCommonAccessToken(byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, out bool userNotFoundInAD, Guid requestId = default(Guid))
		{
			IAsyncResult asyncResult = this.BeginGetCommonAccessToken(userBytes, passBytes, remoteOrganizationContext, requestId, null, null);
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult != null)
			{
				lazyAsyncResult.InternalWaitForCompletion();
			}
			else
			{
				asyncResult.AsyncWaitHandle.WaitOne();
				asyncResult.AsyncWaitHandle.Close();
			}
			string result;
			LiveIdAuthResult liveIdAuthResult = this.EndGetCommonAccessToken(asyncResult, out result);
			userNotFoundInAD = (liveIdAuthResult == LiveIdAuthResult.UserNotFoundInAD);
			return result;
		}

		public IAsyncResult BeginGetCommonAccessToken(byte[] userBytes, byte[] passBytes, AsyncCallback callback, object state)
		{
			return this.BeginGetCommonAccessToken(userBytes, passBytes, Guid.Empty, callback, state);
		}

		public IAsyncResult BeginGetCommonAccessToken(byte[] userBytes, byte[] passBytes, Guid requestId, AsyncCallback callback, object state)
		{
			return this.BeginGetCommonAccessToken(userBytes, passBytes, string.Empty, requestId, callback, state);
		}

		public IAsyncResult BeginGetCommonAccessToken(byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, Guid requestId, AsyncCallback callback, object state)
		{
			AuthOptions flags = AuthOptions.None;
			if (this.SyncAD)
			{
				flags |= AuthOptions.SyncAD;
			}
			if (this.SyncADBackEndOnly)
			{
				flags |= AuthOptions.SyncADBackEndOnly;
			}
			if (this.BypassPositiveLogonCache)
			{
				flags |= AuthOptions.BypassPositiveCache;
			}
			if (this.SyncUPN)
			{
				flags |= AuthOptions.SyncUPN;
			}
			return this.BeginGetAuthToken(userBytes, passBytes, callback, state, delegate
			{
				IAsyncResult result;
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					result = this.instanceClient.BeginLogonCommonAccessTokenFederationCredsAsync((uint)currentProcess.Id, userBytes, passBytes, flags, remoteOrganizationContext, this.application, this.userAgent, this.userAddress, requestId, callback, state);
				}
				return result;
			});
		}

		public LiveIdAuthResult EndGetCommonAccessToken(IAsyncResult ar, out string commonAccessToken)
		{
			string localCat = null;
			LiveIdAuthResult result = this.EndGetAuthToken(ar, () => this.instanceClient.EndLogonCommonAccessTokenFederationCredsAsync(out localCat, out this.lastRequestErrorMessage, ar));
			commonAccessToken = localCat;
			return result;
		}

		private IAsyncResult BeginGetAuthToken(byte[] userBytes, byte[] passBytes, AsyncCallback callback, object state, Func<IAsyncResult> beginAuthToken)
		{
			this.lastRequestErrorMessage = null;
			this.RecoverableLogonFailure = false;
			this.Tarpit = false;
			string @string;
			if (userBytes != null && userBytes.Length > 0 && userBytes[userBytes.Length - 1] == 0)
			{
				@string = Encoding.Default.GetString(userBytes, 0, userBytes.Length - 1);
			}
			else
			{
				@string = Encoding.Default.GetString(userBytes);
			}
			if (!SmtpAddress.IsValidSmtpAddress(@string))
			{
				this.beginResult = LiveIdAuthResult.InvalidUsername;
				this.lastResult = this.beginResult;
				this.lastRequestErrorMessage = "member name is not a valid SMTP address";
				IAsyncResult asyncResult = new LazyAsyncResult(this, state, callback);
				((LazyAsyncResult)asyncResult).InvokeCallback();
				return asyncResult;
			}
			return this.BeginOperation(callback, state, beginAuthToken);
		}

		private LiveIdAuthResult EndGetAuthToken(IAsyncResult ar, Func<AuthStatus> endAuthToken)
		{
			LiveIdAuthResult result = LiveIdAuthResult.AuthFailure;
			try
			{
				LazyAsyncResult lazyAsyncResult = ar as LazyAsyncResult;
				if (lazyAsyncResult != null)
				{
					result = this.beginResult;
				}
				else
				{
					AuthStatus authStatus = endAuthToken();
					if (!string.IsNullOrEmpty(this.lastRequestErrorMessage) && this.lastRequestErrorMessage.IndexOf("AuthenticatedBy:OfflineOrgId.") >= 0)
					{
						this.AuthenticatedByOfflineAuth = true;
						Match match = LiveIdBasicAuthentication.offlineOrgIdResultRegex.Match(this.lastRequestErrorMessage);
						AuthStatus authStatus2;
						if (match.Success && Enum.TryParse<AuthStatus>(match.Groups[1].Value, out authStatus2))
						{
							AuthStatus authStatus3 = authStatus2;
							if (authStatus3 != AuthStatus.OfflineHrdFailed)
							{
								switch (authStatus3)
								{
								case AuthStatus.AmbigiousMailboxFound:
									this.OfflineOrgIdFailureResult = new LiveIdAuthResult?(LiveIdAuthResult.AmbigiousMailboxFoundFailure);
									break;
								case AuthStatus.OffineOrgIdAuthFailed:
									this.OfflineOrgIdFailureResult = new LiveIdAuthResult?(LiveIdAuthResult.OfflineOrgIdAuthFailure);
									break;
								default:
									if (authStatus3 == AuthStatus.LowConfidence)
									{
										this.OfflineOrgIdFailureResult = new LiveIdAuthResult?(LiveIdAuthResult.LowPasswordConfidence);
									}
									break;
								}
							}
							else
							{
								this.OfflineOrgIdFailureResult = new LiveIdAuthResult?(LiveIdAuthResult.OfflineHrdFailed);
							}
						}
					}
					switch (authStatus)
					{
					case AuthStatus.UnfamiliarLocation:
						result = LiveIdAuthResult.UnfamiliarLocation;
						break;
					case AuthStatus.Forbidden:
						result = LiveIdAuthResult.Forbidden;
						break;
					case AuthStatus.InternalServerError:
						result = LiveIdAuthResult.InternalServerError;
						break;
					case AuthStatus.AccountNotProvisioned:
						result = LiveIdAuthResult.AccountNotProvisioned;
						break;
					case AuthStatus.RepeatedADFSRulesDenied:
					case AuthStatus.ADFSRulesDenied:
						result = LiveIdAuthResult.FederatedStsADFSRulesDenied;
						break;
					case AuthStatus.FederatedStsUrlNotEncrypted:
						result = LiveIdAuthResult.FederatedStsUrlNotEncrypted;
						break;
					case AuthStatus.AppPasswordRequired:
						result = LiveIdAuthResult.AppPasswordRequired;
						this.RecoverableLogonFailure = true;
						break;
					case AuthStatus.OfflineHrdFailed:
					case AuthStatus.HRDFailed:
						result = LiveIdAuthResult.HRDFailure;
						break;
					case AuthStatus.PuidNotFound:
					case AuthStatus.Redirect:
						result = LiveIdAuthResult.UserNotFoundInAD;
						break;
					case AuthStatus.PuidMismatch:
						result = LiveIdAuthResult.PuidMismatchFailure;
						break;
					case AuthStatus.UnableToOpenTicket:
						result = LiveIdAuthResult.UnableToOpenTicketFailure;
						break;
					case AuthStatus.AmbigiousMailboxFound:
						result = LiveIdAuthResult.AmbigiousMailboxFoundFailure;
						break;
					case AuthStatus.OffineOrgIdAuthFailed:
						result = LiveIdAuthResult.OfflineOrgIdAuthFailure;
						break;
					case AuthStatus.S4ULogonFailed:
						result = LiveIdAuthResult.S4ULogonFailure;
						break;
					case AuthStatus.RepeatedBadPassword:
					case AuthStatus.BadPassword:
						result = LiveIdAuthResult.InvalidCreds;
						break;
					case AuthStatus.LowConfidence:
						result = LiveIdAuthResult.LowPasswordConfidence;
						break;
					case AuthStatus.RepeatedExpiredCredentials:
					case AuthStatus.ExpiredCredentials:
						result = LiveIdAuthResult.ExpiredCreds;
						this.RecoverableLogonFailure = true;
						break;
					case AuthStatus.RepeatedRecoverableFailure:
					case AuthStatus.RecoverableLogonFailed:
						result = LiveIdAuthResult.RecoverableAuthFailure;
						this.RecoverableLogonFailure = true;
						break;
					case AuthStatus.RepeatedFederatedStsFailure:
					case AuthStatus.FederatedStsFailed:
						result = LiveIdAuthResult.FederatedStsUnreachable;
						break;
					case AuthStatus.RepeatedLiveIDFailure:
					case AuthStatus.LiveIDFailed:
						result = LiveIdAuthResult.LiveServerUnreachable;
						break;
					case AuthStatus.RepeatedLogonFailure:
					case AuthStatus.LogonFailed:
						result = LiveIdAuthResult.AuthFailure;
						break;
					case AuthStatus.LogonSuccess:
						result = LiveIdAuthResult.Success;
						break;
					default:
						result = LiveIdAuthResult.AuthFailure;
						break;
					}
					switch (authStatus)
					{
					case AuthStatus.RepeatedBadPassword:
					case AuthStatus.RepeatedExpiredCredentials:
					case AuthStatus.RepeatedRecoverableFailure:
					case AuthStatus.RepeatedFederatedStsFailure:
					case AuthStatus.RepeatedLogonFailure:
						this.Tarpit = true;
						break;
					}
				}
			}
			catch (InvalidOperationException ex)
			{
				this.lastRequestErrorMessage = string.Format("{0}", ex.Message);
				ExTraceGlobals.AuthenticationTracer.TraceWarning<string>((long)this.GetHashCode(), "Invalid Operation Exception while retrieving result from LiveIdBasic service: {0}", this.lastRequestErrorMessage);
				result = LiveIdAuthResult.CommunicationFailure;
				LiveIdBasicAuthentication.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralClientException, "InvalidOperationException", new object[]
				{
					(uint)Process.GetCurrentProcess().Id,
					ex + "\n" + ((ex.InnerException != null) ? ex.InnerException.ToString() : string.Empty)
				});
			}
			catch (TimeoutException ex2)
			{
				this.lastRequestErrorMessage = string.Format("{0}", ex2.Message);
				ExTraceGlobals.AuthenticationTracer.TraceWarning<string>((long)this.GetHashCode(), "Timed out while trying to query the LiveIdBasic service: {0}", this.lastRequestErrorMessage);
				result = LiveIdAuthResult.OperationTimedOut;
			}
			catch (FaultException ex3)
			{
				this.lastRequestErrorMessage = string.Format("{0}", (ex3.InnerException != null) ? ex3.InnerException.ToString() : ex3.ToString());
				ExTraceGlobals.AuthenticationTracer.TraceWarning<string>((long)this.GetHashCode(), "Fault Exception while retrieving result from LiveIdBasic service: {0}", this.lastRequestErrorMessage);
				result = LiveIdAuthResult.FaultException;
			}
			catch (CommunicationException ex4)
			{
				this.lastRequestErrorMessage = string.Format("{0}", (ex4.InnerException != null) ? ex4.InnerException.ToString() : ex4.ToString());
				ExTraceGlobals.AuthenticationTracer.TraceWarning<string>((long)this.GetHashCode(), "Communication Exception while retrieving result from LiveIdBasic service: {0}", this.lastRequestErrorMessage);
				result = LiveIdAuthResult.CommunicationFailure;
				LiveIdBasicAuthentication.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralClientException, "CommunicationException", new object[]
				{
					(uint)Process.GetCurrentProcess().Id,
					ex4 + "\n" + ((ex4.InnerException != null) ? ex4.InnerException.ToString() : string.Empty)
				});
			}
			finally
			{
				this.lastResult = result;
				if (this.instanceClient != null)
				{
					this.instanceClient.Release();
					this.instanceClient = null;
				}
			}
			return result;
		}

		public LiveIdAuthResult SyncADPassword(string puid, byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, bool syncHrd)
		{
			IAsyncResult asyncResult = this.BeginSyncADPassword(puid, userBytes, passBytes, remoteOrganizationContext, null, null, Guid.Empty, syncHrd);
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult != null)
			{
				lazyAsyncResult.InternalWaitForCompletion();
			}
			else
			{
				asyncResult.AsyncWaitHandle.WaitOne();
				asyncResult.AsyncWaitHandle.Close();
			}
			return this.EndSyncADPassword(asyncResult);
		}

		public IAsyncResult BeginSyncADPassword(string puid, byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, AsyncCallback callback, object state, Guid requestId = default(Guid), bool syncHrd = false)
		{
			if (string.IsNullOrEmpty(puid))
			{
				throw new ArgumentException("puid is null or empty");
			}
			byte[] puidPlusUserBytes;
			if (syncHrd)
			{
				puidPlusUserBytes = new byte[userBytes.Length + puid.Length + 2 + "true".Length];
			}
			else
			{
				puidPlusUserBytes = new byte[userBytes.Length + puid.Length + 1];
			}
			Array.Copy(Encoding.ASCII.GetBytes(puid), puidPlusUserBytes, puid.Length);
			puidPlusUserBytes[puid.Length] = 58;
			Array.Copy(userBytes, 0, puidPlusUserBytes, puid.Length + 1, userBytes.Length);
			if (syncHrd)
			{
				puidPlusUserBytes[userBytes.Length + puid.Length + 1] = 58;
				Array.Copy(Encoding.ASCII.GetBytes("true"), 0, puidPlusUserBytes, userBytes.Length + puid.Length + 2, "true".Length);
			}
			AuthOptions options = AuthOptions.SyncAD | AuthOptions.PasswordAndHRDSync;
			if (this.SyncUPN)
			{
				options |= AuthOptions.SyncUPN;
			}
			return this.BeginGetAuthToken(userBytes, passBytes, callback, state, delegate
			{
				IAsyncResult result;
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					result = this.instanceClient.BeginLogonCommonAccessTokenFederationCredsAsync((uint)currentProcess.Id, puidPlusUserBytes, passBytes, options, remoteOrganizationContext, this.application, this.userAgent, this.userAddress, requestId, callback, state);
				}
				return result;
			});
		}

		public LiveIdAuthResult EndSyncADPassword(IAsyncResult ar)
		{
			return this.EndGetAuthToken(ar, delegate
			{
				string text;
				return this.instanceClient.EndLogonCommonAccessTokenFederationCredsAsync(out text, out this.lastRequestErrorMessage, ar);
			});
		}

		public bool IsNego2AuthEnabledForDomain(string domain, out bool nego2Enabled)
		{
			AuthServiceClient authServiceClient = null;
			bool flag = false;
			bool flag2 = true;
			nego2Enabled = false;
			do
			{
				try
				{
					authServiceClient = LiveIdBasicAuthentication.GetClient();
					nego2Enabled = authServiceClient.IsNego2AuthEnabledForDomain(domain);
					flag2 = false;
					flag = false;
				}
				catch (Exception ex)
				{
					if (ex is CommunicationException)
					{
						ExTraceGlobals.AuthenticationTracer.TraceWarning<Exception>((long)this.GetHashCode(), "CommunicationException {0}", ex);
					}
					else if (ex is InvalidOperationException)
					{
						ExTraceGlobals.AuthenticationTracer.TraceWarning<Exception>((long)this.GetHashCode(), "InvalidOperationException {0}", ex);
					}
					else
					{
						if (!(ex is TimeoutException))
						{
							throw;
						}
						ExTraceGlobals.AuthenticationTracer.TraceWarning<Exception>((long)this.GetHashCode(), "TimeoutException {0}", ex);
					}
					if (flag)
					{
						LiveIdBasicAuthentication.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_CannotConnectToAuthService, "CannotConnectToAuthService", new object[]
						{
							(uint)Process.GetCurrentProcess().Id,
							ex.Message,
							(ex.InnerException != null) ? ex.InnerException.ToString() : string.Empty
						});
						return false;
					}
					flag = true;
				}
				finally
				{
					if (authServiceClient != null)
					{
						if (flag2)
						{
							LiveIdBasicAuthentication.InvalidateClient(authServiceClient);
						}
						authServiceClient.Release();
						authServiceClient = null;
					}
				}
			}
			while (flag);
			return !flag2;
		}

		private IAsyncResult BeginOperation(AsyncCallback callback, object state, Func<IAsyncResult> beginOperation)
		{
			this.lastRequestErrorMessage = null;
			this.beginResult = LiveIdAuthResult.Success;
			IAsyncResult asyncResult = null;
			bool flag = false;
			if (this.instanceClient != null)
			{
				throw new InvalidOperationException("You cannot call Begin twice without first calling End on a single instance of LiveIdBasicAuthentication");
			}
			do
			{
				bool flag2 = false;
				try
				{
					this.instanceClient = LiveIdBasicAuthentication.GetClient();
					flag2 = true;
					asyncResult = beginOperation();
					flag2 = false;
					flag = false;
				}
				catch (Exception ex)
				{
					if (ex is CommunicationException)
					{
						ExTraceGlobals.AuthenticationTracer.TraceWarning<Exception>((long)this.GetHashCode(), "CommunicationException {0}", ex);
					}
					else if (ex is InvalidOperationException)
					{
						ExTraceGlobals.AuthenticationTracer.TraceWarning<Exception>((long)this.GetHashCode(), "InvalidOperationException {0}", ex);
					}
					else
					{
						if (!(ex is TimeoutException))
						{
							throw;
						}
						ExTraceGlobals.AuthenticationTracer.TraceWarning<Exception>((long)this.GetHashCode(), "TimeoutException {0}", ex);
					}
					if (flag)
					{
						LiveIdBasicAuthentication.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_CannotConnectToAuthService, "CannotConnectToAuthService", new object[]
						{
							(uint)Process.GetCurrentProcess().Id,
							ex.GetHashCode() + ex.Message,
							(ex.InnerException != null) ? ex.InnerException.ToString() : string.Empty
						});
						this.beginResult = LiveIdAuthResult.CommunicationFailure;
						this.lastRequestErrorMessage = string.Format("{0}", (ex.InnerException != null) ? ex.InnerException.ToString() : ex.ToString());
						asyncResult = new LazyAsyncResult(this, state, callback);
						((LazyAsyncResult)asyncResult).InvokeCallback(ex);
						return asyncResult;
					}
					flag = true;
				}
				finally
				{
					this.lastResult = this.beginResult;
					if (flag2)
					{
						flag2 = false;
						if (this.instanceClient != null)
						{
							LiveIdBasicAuthentication.InvalidateClient(this.instanceClient);
							this.instanceClient.Release();
							this.instanceClient = null;
						}
					}
				}
			}
			while (flag);
			return asyncResult;
		}

		private static AuthServiceClient CreateAuthServiceClient()
		{
			AuthServiceClient authServiceClient;
			try
			{
				authServiceClient = new AuthServiceClient();
			}
			catch (Exception ex)
			{
				LiveIdBasicAuthentication.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_CannotConnectToAuthService, "CannotFindEndPointConfig", new object[]
				{
					(uint)Process.GetCurrentProcess().Id,
					ex.GetHashCode() + ex.Message,
					(ex.InnerException != null) ? ex.InnerException.ToString() : string.Empty
				});
				int num = 1009;
				string uri = string.Format("net.tcp://{0}:{1}/Microsoft.Exchange.Security.Authentication.FederatedAuthService", "localhost", num);
				NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
				EndpointAddress remoteAddress = new EndpointAddress(uri);
				authServiceClient = new AuthServiceClient(binding, remoteAddress);
			}
			authServiceClient.Open();
			return authServiceClient;
		}

		public static readonly string LiveIdComponent = "MSExchange LiveIdBasicAuthentication";

		public static LiveIdBasicAuthentication.NewAuthServiceClient NewClientDelegate = () => LiveIdBasicAuthentication.CreateAuthServiceClient();

		private static readonly Regex offlineOrgIdResultRegex = new Regex("<OfflineOrgIdAuthResult=(.*?)>", RegexOptions.Compiled);

		private bool syncAD;

		private bool syncADBackEndOnly;

		private bool allowOfflineOrgIdAsPrimeAuth;

		private bool allowLiveIDOnlyAuth;

		private string userAgent;

		private string userAddress;

		private string application;

		private string lastRequestErrorMessage;

		private LiveIdAuthResult beginResult;

		private LiveIdAuthResult lastResult;

		private static AuthServiceClient sharedClient;

		private static readonly object clientLock = new object();

		private AuthServiceClient instanceClient;

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.AuthenticationTracer.Category, LiveIdBasicAuthentication.LiveIdComponent);

		public delegate AuthServiceClient NewAuthServiceClient();
	}
}
