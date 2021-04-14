using System;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.Compliance;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharepointAccessManager
	{
		private SharepointAccessManager()
		{
			this.cacheExpiryForAllowedIdentity = TimeSpan.FromSeconds((double)StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox\\SharepointAccessManager", "CacheExpiryForAllowedIdentity", 21600, (int x) => x > 0));
			this.cacheExpiryForDeniedIdentity = TimeSpan.FromSeconds((double)StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox\\SharepointAccessManager", "CacheExpiryForDeniedIdentity", 900, (int x) => x > 0));
			this.requestTimeout = TimeSpan.FromSeconds((double)StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox\\SharepointAccessManager", "RequestTimeout", 90, (int x) => x > 0));
			this.synchronousWaitTimeout = TimeSpan.FromSeconds((double)StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox\\SharepointAccessManager", "SynchronousWaitTimeout", 10, (int x) => x > 0));
			this.cacheSize = StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox\\SharepointAccessManager", "CacheSize", 100000, (int x) => x > 0);
			this.requestThroughLocalProxy = StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox\\SharepointAccessManager", "RequestThroughLocalProxy", 0, (int x) => x == 0 || x == 1);
			Random random = new Random((int)ExDateTime.UtcNow.UtcTicks & 65535);
			this.hmacKey = new byte[64];
			random.NextBytes(this.hmacKey);
			if (this.cacheSize > 200000)
			{
				this.cacheSize = 200000;
			}
			int numberOfBuckets = (int)Math.Ceiling((double)this.cacheSize / 10000.0);
			this.cache = new TimeoutCache<SharepointAccessManager.Key, AccessState>(numberOfBuckets, 10000, false);
		}

		private static void OnCheckComplete(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new InvalidOperationException("SharepointAccessManager.OnCheckComplete: asyncResult cannot be null here.");
			}
			SharepointAccessManager.PermissionCheckerClient permissionCheckerClient = asyncResult.AsyncState as SharepointAccessManager.PermissionCheckerClient;
			if (permissionCheckerClient == null)
			{
				throw new InvalidOperationException("SharepointAccessManager.OnCheckComplete: asyncResult.AsyncState is not PermissionCheckerClient");
			}
			object obj = permissionCheckerClient.EndExecute(asyncResult);
			if (obj is AccessState)
			{
				SharepointAccessManager.Instance.SetValue(permissionCheckerClient.SessionHashCode, permissionCheckerClient.Key, permissionCheckerClient.UserSid, permissionCheckerClient.MailboxGuid, (AccessState)obj);
			}
		}

		public bool UpdateAccessTokenIfNeeded(ExchangePrincipal siteMailbox, ICredentials accessingUserCredential, ClientSecurityContext accessingUserSecurityContext, out Exception exception, bool isTest = false)
		{
			int hashCode = Guid.NewGuid().GetHashCode();
			if (siteMailbox == null)
			{
				throw new ArgumentNullException("siteMailbox");
			}
			Uri sharepointUrl;
			if (siteMailbox.MailboxInfo.Configuration.SharePointUrl != null)
			{
				sharepointUrl = siteMailbox.MailboxInfo.Configuration.SharePointUrl;
			}
			else
			{
				sharepointUrl = SharepointAccessManager.GetSharepointUrl(siteMailbox, hashCode);
			}
			return this.UpdateAccessTokenIfNeeded(sharepointUrl, siteMailbox, accessingUserCredential, accessingUserSecurityContext, hashCode, out exception, isTest);
		}

		public bool UpdateAccessTokenIfNeeded(Uri sharepointUrl, ExchangePrincipal mailbox, ICredentials accessingUserCredential, ClientSecurityContext accessingUserSecurityContext, out Exception exception, bool isTest = false)
		{
			int hashCode = Guid.NewGuid().GetHashCode();
			return this.UpdateAccessTokenIfNeeded(sharepointUrl, mailbox, accessingUserCredential, accessingUserSecurityContext, hashCode, out exception, isTest);
		}

		private bool UpdateAccessTokenIfNeeded(Uri sharepointUrl, ExchangePrincipal siteMailbox, ICredentials accessingUserCredential, ClientSecurityContext accessingUserSecurityContext, int sessionHashCode, out Exception exception, bool isTest = false)
		{
			exception = null;
			if (siteMailbox == null)
			{
				throw new ArgumentNullException("siteMailbox");
			}
			if (accessingUserSecurityContext == null)
			{
				throw new ArgumentNullException("accessingUserSecurityContext");
			}
			if (sharepointUrl == null)
			{
				ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceDebug<string>((long)sessionHashCode, "SharepointAccessManager.UpdateAccessTokenIfNeeded: Skip site mailbox {0} because its Sharepoint URL is null", siteMailbox.MailboxInfo.PrimarySmtpAddress.ToString());
				return false;
			}
			ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceDebug<ClientSecurityContext, string, string>((long)sessionHashCode, "SharepointAccessManager.UpdateAccessTokenIfNeeded: Entered with user {0}, site mailbox {1}, URL {2}", accessingUserSecurityContext, siteMailbox.MailboxInfo.PrimarySmtpAddress.ToString(), sharepointUrl.AbsoluteUri);
			LoggingContext loggingContext = new LoggingContext(siteMailbox.MailboxInfo.MailboxGuid, sharepointUrl.AbsoluteUri, accessingUserSecurityContext.ToString(), null);
			AccessState accessState = AccessState.Denied;
			bool flag = false;
			SharepointAccessManager.Key key = this.GetKey(accessingUserSecurityContext.UserSid, siteMailbox.MailboxInfo.MailboxGuid);
			if (!SharepointAccessManager.Instance.TryGetValue(key, out accessState))
			{
				flag = true;
				ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceDebug((long)sessionHashCode, "SharepointAccessManager.UpdateAccessTokenIfNeeded: Cache miss, query Sharepoint");
				SharepointAccessManager.PermissionCheckerClient permissionCheckerClient = new SharepointAccessManager.PermissionCheckerClient(sessionHashCode, siteMailbox.MailboxInfo.MailboxGuid, sharepointUrl.AbsoluteUri, accessingUserSecurityContext.UserSid, key, accessingUserCredential, this.requestTimeout, this.requestThroughLocalProxy, isTest);
				LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)permissionCheckerClient.BeginExecute(new AsyncCallback(SharepointAccessManager.OnCheckComplete), permissionCheckerClient);
				int num = (int)this.synchronousWaitTimeout.TotalSeconds * 2;
				int num2 = 0;
				while (!lazyAsyncResult.IsCompleted || lazyAsyncResult.Result == null)
				{
					Thread.Sleep(500);
					num2++;
					if (num2 > num)
					{
						break;
					}
				}
				object result = lazyAsyncResult.Result;
				if (!(result is AccessState))
				{
					exception = ((lazyAsyncResult.Result is Exception) ? (lazyAsyncResult.Result as Exception) : new TimeoutException("Timed out"));
					WebException ex = exception as WebException;
					if (ex != null)
					{
						SharePointException exception2 = new SharePointException(sharepointUrl.AbsoluteUri, ex, false);
						ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceError<WebExceptionStatus, string, Exception>((long)permissionCheckerClient.SessionHashCode, "SharepointAccessManager.UpdateAccessTokenIfNeeded: Query failed with WebException. Status: {0}, Message: {1}, InnerException: {2}", ex.Status, ex.Message, ex.InnerException);
						ProtocolLog.LogError(ProtocolLog.Component.SharepointAccessManager, loggingContext, "Query failed with SharePointException", exception2);
					}
					else
					{
						ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceError<Type, string, Exception>((long)permissionCheckerClient.SessionHashCode, "SharepointAccessManager.UpdateAccessTokenIfNeeded: Query failed with {0}, Message: {1}, InnerException: {2}", exception.GetType(), exception.Message, exception.InnerException);
						ProtocolLog.LogError(ProtocolLog.Component.SharepointAccessManager, loggingContext, string.Format("Query failed with {0}", exception.GetType()), exception);
					}
					return false;
				}
				accessState = (AccessState)result;
			}
			else if (accessState == AccessState.Pending)
			{
				ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceDebug((long)sessionHashCode, "SharepointAccessManager.UpdateAccessTokenIfNeeded: Skip because there is an pending request");
				return true;
			}
			if (accessState == AccessState.Allowed)
			{
				if (!accessingUserSecurityContext.AddGroupSids(new SidBinaryAndAttributes[]
				{
					new SidBinaryAndAttributes(WellKnownSids.SiteMailboxGrantedAccessMembers, 4U)
				}))
				{
					ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceError((long)sessionHashCode, "SharepointAccessManager.UpdateAccessTokenIfNeeded: Failed to add SiteMailboxGrantedAccessMembers group sid to access token");
					return false;
				}
				ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceDebug<ClientSecurityContext, string, string>((long)sessionHashCode, "SharepointAccessManager.UpdateAccessTokenIfNeeded: Access is allowed for user {0}, site mailbox {1}, URL {2}", accessingUserSecurityContext, siteMailbox.MailboxInfo.PrimarySmtpAddress.ToString(), sharepointUrl.AbsoluteUri);
				if (flag)
				{
					ProtocolLog.LogInformation(ProtocolLog.Component.SharepointAccessManager, loggingContext, string.Format("Allow access for site mailbox {0}, URL {1}", siteMailbox.MailboxInfo.PrimarySmtpAddress.ToString(), sharepointUrl));
				}
			}
			else if (flag)
			{
				ProtocolLog.LogInformation(ProtocolLog.Component.SharepointAccessManager, loggingContext, string.Format("Deny access for site mailbox {0}, URL {1}", siteMailbox.MailboxInfo.PrimarySmtpAddress.ToString(), sharepointUrl));
			}
			return true;
		}

		public TimeSpan CacheExpiryForDeniedIdentity
		{
			get
			{
				return this.cacheExpiryForDeniedIdentity;
			}
			set
			{
				this.cacheExpiryForDeniedIdentity = value;
			}
		}

		private static Uri GetSharepointUrl(ExchangePrincipal siteMailbox, int sessionHashCode)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(siteMailbox.MailboxInfo.OrganizationId), 541, "GetSharepointUrl", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\LinkedFolder\\SharepointAccessManager.cs");
			ADRecipient adrecipient = null;
			try
			{
				adrecipient = tenantOrRootOrgRecipientSession.FindByExchangeGuid(siteMailbox.MailboxInfo.MailboxGuid);
			}
			catch (TransientException arg)
			{
				ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceError<TransientException>((long)sessionHashCode, "SharepointAccessManager.GetSharepointUrl: Failed to FindByExchangeGuid for site mailbox with TransientException {0}", arg);
				return null;
			}
			catch (DataValidationException arg2)
			{
				ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceError<DataValidationException>((long)sessionHashCode, "SharepointAccessManager.GetSharepointUrl: Failed to FindByExchangeGuid for site mailbox with DataValidationException {0}", arg2);
				return null;
			}
			catch (DataSourceOperationException arg3)
			{
				ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceError<DataSourceOperationException>((long)sessionHashCode, "SharepointAccessManager.GetSharepointUrl: Failed to FindByExchangeGuid for site mailbox with DataSourceOperationException {0}", arg3);
				return null;
			}
			ADUser aduser = adrecipient as ADUser;
			if (aduser == null)
			{
				ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceError((long)sessionHashCode, "SharepointAccessManager.GetSharepointUrl: siteMailboxRecipientObject is not ADUser");
				return null;
			}
			return aduser.SharePointUrl;
		}

		private bool TryGetValue(SharepointAccessManager.Key key, out AccessState value)
		{
			bool result;
			lock (this.lockObj)
			{
				if (!this.cache.TryGetValue(key, out value))
				{
					this.cache.AddAbsolute(key, AccessState.Pending, DateTime.UtcNow + this.requestTimeout + TimeSpan.FromSeconds(5.0), null);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private void SetValue(int sessionHashCode, SharepointAccessManager.Key key, SecurityIdentifier userSid, Guid mailboxGuid, AccessState value)
		{
			DateTime dateTime;
			lock (this.lockObj)
			{
				this.cache.Remove(key);
				dateTime = ((value == AccessState.Allowed) ? (DateTime.UtcNow + this.cacheExpiryForAllowedIdentity) : (DateTime.UtcNow + this.cacheExpiryForDeniedIdentity));
				this.cache.AddAbsolute(key, value, dateTime, null);
			}
			ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceDebug((long)sessionHashCode, "SharepointAccessManager.SetValue: Add to cache {0} for user {1}, site mailbox {2}. Expiry is {3}", new object[]
			{
				value,
				userSid,
				mailboxGuid,
				dateTime
			});
		}

		private SharepointAccessManager.Key GetKey(SecurityIdentifier userSid, Guid mailboxGuid)
		{
			SharepointAccessManager.Key result;
			using (HMACSHA256Cng hmacsha256Cng = new HMACSHA256Cng(this.hmacKey))
			{
				byte[] array = new byte[userSid.BinaryLength + 16];
				userSid.GetBinaryForm(array, 0);
				Buffer.BlockCopy(mailboxGuid.ToByteArray(), 0, array, userSid.BinaryLength, 16);
				byte[] src = hmacsha256Cng.ComputeHash(array);
				byte[] array2 = new byte[16];
				Buffer.BlockCopy(src, 0, array2, 0, 16);
				result = new SharepointAccessManager.Key(array2);
			}
			return result;
		}

		public static SharepointAccessManager Instance
		{
			get
			{
				return SharepointAccessManager.singleton;
			}
		}

		private const string RegKeySharepointAccessManager = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox\\SharepointAccessManager";

		private const string RegValueCacheExpiryForAllowedIdentity = "CacheExpiryForAllowedIdentity";

		private const string RegValueCacheExpiryForDeniedIdentity = "CacheExpiryForDeniedIdentity";

		private const string RegValueCacheSize = "CacheSize";

		private const string RegValueRequestTimeout = "RequestTimeout";

		private const string RegValueSynchronousWaitTimeout = "SynchronousWaitTimeout";

		private const string RegValueRequestThroughLocalProxy = "RequestThroughLocalProxy";

		private readonly object lockObj = new object();

		private readonly TimeoutCache<SharepointAccessManager.Key, AccessState> cache;

		private readonly byte[] hmacKey;

		private static SharepointAccessManager singleton = new SharepointAccessManager();

		private readonly int cacheSize;

		private readonly int requestThroughLocalProxy;

		private readonly TimeSpan cacheExpiryForAllowedIdentity;

		private readonly TimeSpan requestTimeout;

		private readonly TimeSpan synchronousWaitTimeout;

		private TimeSpan cacheExpiryForDeniedIdentity;

		private sealed class Key : IEquatable<SharepointAccessManager.Key>
		{
			public Key(byte[] keyBytes)
			{
				this.keyBytes = keyBytes;
			}

			public override int GetHashCode()
			{
				return 0;
			}

			public override bool Equals(object obj)
			{
				return this.Equals(obj as SharepointAccessManager.Key);
			}

			public bool Equals(SharepointAccessManager.Key other)
			{
				return other != null && (object.ReferenceEquals(this, other) || ArrayComparer<byte>.Comparer.Equals(this.keyBytes, other.keyBytes));
			}

			private readonly byte[] keyBytes;
		}

		private sealed class PermissionCheckerClient
		{
			public SecurityIdentifier UserSid { get; private set; }

			public SharepointAccessManager.Key Key { get; private set; }

			public Guid MailboxGuid { get; private set; }

			public int SessionHashCode { get; private set; }

			public PermissionCheckerClient(int sessionHashCode, Guid mailboxGuid, string siteUrl, SecurityIdentifier userSid, SharepointAccessManager.Key key, ICredentials credential, TimeSpan requestTimeout, int requestThroughLocalProxy, bool isTest)
			{
				this.SessionHashCode = sessionHashCode;
				this.MailboxGuid = mailboxGuid;
				this.siteUrl = siteUrl;
				this.UserSid = userSid;
				this.Key = key;
				this.credential = credential;
				this.requestTimeout = requestTimeout;
				this.requestThroughLocalProxy = requestThroughLocalProxy;
				this.isTest = isTest;
			}

			public IAsyncResult BeginExecute(AsyncCallback executeCallback, object state)
			{
				this.executionAsyncResult = new LazyAsyncResult(null, state, executeCallback);
				this.InternalBeginQuery();
				return this.executionAsyncResult;
			}

			public object EndExecute(IAsyncResult asyncResult)
			{
				LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
				if (lazyAsyncResult == null)
				{
					throw new InvalidOperationException("EndExecute: asyncResult cannot be null.");
				}
				if (!lazyAsyncResult.IsCompleted)
				{
					lazyAsyncResult.InternalWaitForCompletion();
				}
				return lazyAsyncResult.Result;
			}

			private void InternalBeginQuery()
			{
				bool flag = false;
				try
				{
					this.InitializeHttpClient();
					StringBuilder stringBuilder = new StringBuilder(this.siteUrl);
					stringBuilder.Append("/_api/web/EffectiveBasePermissions");
					this.httpClient.BeginDownload(new Uri(stringBuilder.ToString()), this.httpSessionConfig, new CancelableAsyncCallback(this.OnInternalQueryComplete), null);
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						this.Cleanup();
					}
				}
			}

			private void OnInternalQueryComplete(ICancelableAsyncResult asyncResult)
			{
				if (asyncResult == null)
				{
					throw new InvalidOperationException("OnInternalQueryComplete: asyncResult cannot be null.");
				}
				try
				{
					Exception ex = null;
					DownloadResult downloadResult = this.httpClient.EndDownload(asyncResult);
					if (!downloadResult.IsSucceeded)
					{
						ex = downloadResult.Exception;
					}
					if (downloadResult.ResponseStream == null && ex == null)
					{
						ex = new SharePointException((this.httpClient.LastKnownRequestedUri != null) ? this.httpClient.LastKnownRequestedUri.AbsoluteUri : string.Empty, new LocalizedString("Response is empty. The user probably doesn't have permission"));
					}
					if (ex != null)
					{
						this.executionAsyncResult.InvokeCallback(ex);
					}
					else
					{
						downloadResult.ResponseStream.Position = 0L;
						long num = 0L;
						try
						{
							using (XmlTextReader xmlTextReader = new XmlTextReader(downloadResult.ResponseStream))
							{
								xmlTextReader.ReadToFollowing("d:High");
								long num2 = xmlTextReader.ReadElementContentAsLong();
								long num3 = xmlTextReader.ReadElementContentAsLong();
								num = (num2 << 32) + num3;
							}
						}
						catch (XmlException value)
						{
							this.executionAsyncResult.InvokeCallback(value);
							return;
						}
						catch (DirectoryNotFoundException value2)
						{
							this.executionAsyncResult.InvokeCallback(value2);
							return;
						}
						catch (WebException value3)
						{
							this.executionAsyncResult.InvokeCallback(value3);
							return;
						}
						AccessState accessState = ((num & 47L) == 47L) ? AccessState.Allowed : AccessState.Denied;
						ExTraceGlobals.SiteMailboxPermissionCheckTracer.TraceDebug<long>((long)this.SessionHashCode, "PermissionCheckerClient.OnInternalQueryComplete: Permission bits retured by Sharepoint is {0}", num);
						this.executionAsyncResult.InvokeCallback(accessState);
					}
				}
				finally
				{
					this.Cleanup();
				}
			}

			private void InitializeHttpClient()
			{
				if (this.httpClient == null)
				{
					this.httpClient = new HttpClient();
					this.httpSessionConfig = new HttpSessionConfig();
					this.httpSessionConfig.Method = "GET";
					this.httpSessionConfig.Credentials = this.credential;
					this.httpSessionConfig.Timeout = (int)this.requestTimeout.TotalMilliseconds;
					this.httpSessionConfig.PreAuthenticate = true;
					this.httpSessionConfig.Headers = new WebHeaderCollection();
					this.httpSessionConfig.Headers["Authorization"] = "Bearer";
					this.httpSessionConfig.Headers["X-RequestForceAuthentication"] = "true";
					this.httpSessionConfig.Headers["client-request-id"] = Guid.NewGuid().ToString();
					this.httpSessionConfig.Headers["return-client-request-id"] = "true";
					this.httpSessionConfig.ContentType = "application/json;odata=verbose";
					this.httpSessionConfig.UserAgent = Utils.GetUserAgentStringForSiteMailboxRequests();
					if (this.requestThroughLocalProxy == 1)
					{
						this.httpSessionConfig.Proxy = new WebProxy("127.0.0.1", 8888);
					}
					if (this.isTest)
					{
						this.httpSessionConfig.Headers["UserSid"] = this.UserSid.ToString();
					}
				}
			}

			private void Cleanup()
			{
				if (this.httpClient != null)
				{
					this.httpClient.Dispose();
					this.httpClient = null;
				}
				if (this.httpSessionConfig != null && this.httpSessionConfig.RequestStream != null)
				{
					this.httpSessionConfig.RequestStream.Dispose();
					this.httpSessionConfig.RequestStream = null;
					this.httpSessionConfig = null;
				}
			}

			private readonly string siteUrl;

			private readonly int requestThroughLocalProxy;

			private readonly ICredentials credential;

			private readonly TimeSpan requestTimeout;

			private HttpClient httpClient;

			private HttpSessionConfig httpSessionConfig;

			private LazyAsyncResult executionAsyncResult;

			private readonly bool isTest;
		}
	}
}
