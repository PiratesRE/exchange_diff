using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Security;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaContext
	{
		private OwaContext(HttpContext httpContext)
		{
			this.httpContext = httpContext;
			this.FormsRegistryContext = new FormsRegistryContext(ApplicationElement.NotSet, null, null, null);
			string pathAndQuery = this.GetModifiedUri().PathAndQuery;
			this.latencyDetectionContext = OwaContext.OwaLatencyDetectionContextFactory.CreateContext(Globals.ApplicationVersion, pathAndQuery, new IPerformanceDataProvider[]
			{
				PerformanceContext.Current,
				RpcDataProvider.Instance,
				MemoryDataProvider.Instance
			});
			if (Globals.CollectPerRequestPerformanceStats)
			{
				this.owaPerformanceData = new OwaPerformanceData(httpContext.Request);
			}
			if (!StringSanitizer<OwaHtml>.TrustedStringsInitialized)
			{
				string location = Assembly.GetExecutingAssembly().Location;
				StringSanitizer<OwaHtml>.InitializeTrustedStrings(new string[]
				{
					location
				});
			}
		}

		public static OwaContext Create(HttpContext httpContext)
		{
			return new OwaContext(httpContext);
		}

		internal SecureNameValueCollection FormNameValueCollection
		{
			get
			{
				return this.formNameValueCollection;
			}
			set
			{
				this.formNameValueCollection = value;
			}
		}

		public bool HandledCriticalError
		{
			get
			{
				return this.handledCriticalError;
			}
			private set
			{
				this.handledCriticalError = value;
			}
		}

		public bool IsAsyncRequest
		{
			get
			{
				return this.isAsyncRequest;
			}
			set
			{
				this.isAsyncRequest = value;
			}
		}

		public bool ShouldDeferInlineImages
		{
			get
			{
				return this.shouldDeferInlineImages;
			}
			set
			{
				this.shouldDeferInlineImages = value;
			}
		}

		internal bool IgnoreUnlockForcefully { get; set; }

		public static OwaContext Current
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					return (OwaContext)httpContext.Items["OwaContext"];
				}
				return null;
			}
		}

		internal static OwaContext Get(HttpContext httpContext)
		{
			return (OwaContext)httpContext.Items["OwaContext"];
		}

		internal static void Set(HttpContext httpContext, OwaContext owaContext)
		{
			httpContext.Items["OwaContext"] = owaContext;
		}

		internal static IBudget TryGetCurrentBudget()
		{
			OwaContext owaContext = OwaContext.Current;
			if (owaContext != null)
			{
				return owaContext.Budget;
			}
			return null;
		}

		internal void DoNotTriggerLatencyDetectionReport()
		{
			this.latencyDetectionContext.TriggerOptions = TriggerOptions.DoNotTrigger;
		}

		internal void ExitLatencyDetectionContext()
		{
			if (this.LogonIdentity != null)
			{
				this.latencyDetectionContext.UserIdentity = this.LogonIdentity.SafeGetRenderableName();
			}
			if (string.IsNullOrEmpty(this.latencyDetectionContext.UserIdentity) && this.mailboxIdentity != null)
			{
				this.latencyDetectionContext.UserIdentity = this.mailboxIdentity.SafeGetRenderableName();
			}
			TaskPerformanceData[] array = this.latencyDetectionContext.StopAndFinalizeCollection();
			int num = 0;
			if (array.Length > num)
			{
				this.ldapData = array[num];
			}
			int num2 = 1;
			if (array.Length > num2)
			{
				this.rpcData = array[num2];
			}
			int num3 = 2;
			if (array.Length > num3)
			{
				this.MemoryData = array[num3];
			}
		}

		internal void UnlockMinResourcesOnCriticalError()
		{
			try
			{
				if (this.userContext != null && this.userContext.LockedByCurrentThread())
				{
					this.userContext.DisconnectAllSessions();
					this.userContext.UnlockForcefully();
				}
			}
			catch (InvalidOperationException)
			{
			}
			this.HandledCriticalError = true;
		}

		internal void AddObjectToDisposeOnEndRequest(IDisposable objectToDispose)
		{
			if (objectToDispose == null)
			{
				throw new ArgumentNullException("objectToDispose");
			}
			if (this.objectsToDispose == null)
			{
				this.objectsToDispose = new List<IDisposable>();
			}
			this.objectsToDispose.Add(objectToDispose);
		}

		internal void DisposeObjectsOnEndRequest()
		{
			if (this.formNameValueCollection != null)
			{
				this.formNameValueCollection.Dispose();
				this.formNameValueCollection = null;
			}
			if (this.objectsToDispose != null)
			{
				foreach (IDisposable disposable in this.objectsToDispose)
				{
					try
					{
						disposable.Dispose();
					}
					catch (ObjectDisposedException)
					{
					}
				}
				this.objectsToDispose.Clear();
			}
			if (this.mailboxIdentity != null)
			{
				this.mailboxIdentity.Dispose();
			}
			if (this.logonIdentity != null)
			{
				this.logonIdentity.Dispose();
			}
		}

		internal void AcquireBudgetAndStartTiming()
		{
			string callerInfo = "OwaContext.AcquireBudgetAndStartTiming";
			this.budget = StandardBudget.Acquire(this.LogonIdentity.UserSid, BudgetType.Owa, Utilities.CreateScopedADSessionSettings(this.LogonIdentity.DomainName));
			this.budget.CheckOverBudget();
			this.budget.StartConnection(callerInfo);
			this.budget.StartLocal(callerInfo, default(TimeSpan));
			this.httpContext.Response.AppendToLog("&Initial Budget>>");
			this.httpContext.Response.AppendToLog(this.budget.ToString());
		}

		internal void TryReleaseBudgetAndStopTiming()
		{
			if (this.budget != null)
			{
				this.httpContext.Response.AppendToLog("&End Budget>>");
				this.httpContext.Response.AppendToLog(this.budget.ToString());
				this.budget.Dispose();
				this.budget = null;
			}
		}

		internal IBudget Budget
		{
			get
			{
				return this.budget;
			}
		}

		public object this[OwaContextProperty property]
		{
			get
			{
				if (this.propertyBag.ContainsKey(property))
				{
					return this.propertyBag[property];
				}
				return null;
			}
			set
			{
				this.propertyBag[property] = value;
			}
		}

		public HttpContext HttpContext
		{
			get
			{
				return this.httpContext;
			}
		}

		public UserContext UserContext
		{
			get
			{
				if (Globals.OwaVDirType == OWAVDirType.Calendar)
				{
					throw new InvalidOperationException("No User Context is available in the anonymous calendar vdir");
				}
				return this.userContext;
			}
			internal set
			{
				this.userContext = value;
				if (this.userContext != null)
				{
					if (Globals.CollectPerRequestPerformanceStats && this.userContext.PerformanceConsoleNotifier != null && this.owaPerformanceData != null)
					{
						this.userContext.PerformanceConsoleNotifier.AddPerformanceData(this.owaPerformanceData);
					}
					if (this.logonIdentity == null)
					{
						if (this.UserContext.LogonIdentity == null)
						{
							throw new ArgumentException("logon identity in the user context is null");
						}
					}
					else
					{
						this.userContext.LogonIdentity.Refresh(this.logonIdentity);
					}
				}
			}
		}

		public UserContext TryGetUserContext()
		{
			return this.userContext;
		}

		internal AnonymousSessionContext AnonymousSessionContext
		{
			get
			{
				if (Globals.OwaVDirType == OWAVDirType.OWA)
				{
					throw new InvalidOperationException("No anonymous session context is available in the OWA vdir");
				}
				return this.anonymousSessionContext;
			}
			set
			{
				if (Globals.OwaVDirType == OWAVDirType.OWA)
				{
					throw new InvalidOperationException("No anonymous session context is available in the OWA vdir");
				}
				if (this.anonymousSessionContext == null)
				{
					this.anonymousSessionContext = value;
					return;
				}
				throw new InvalidOperationException("Cannot set anonymousSessionContext twice!");
			}
		}

		internal ISessionContext SessionContext
		{
			get
			{
				if (Globals.OwaVDirType == OWAVDirType.OWA)
				{
					return this.UserContext;
				}
				return this.anonymousSessionContext;
			}
		}

		public OwaRequestType RequestType
		{
			get
			{
				return this.requestType;
			}
			set
			{
				this.requestType = value;
				if (Globals.CollectPerRequestPerformanceStats)
				{
					this.owaPerformanceData.RequestType = value.ToString();
				}
			}
		}

		public bool IsMowa
		{
			get
			{
				return this.httpContext.Request.Headers["X-OWA-Protocol"] == "MOWA";
			}
		}

		public OwaIdentity LogonIdentity
		{
			get
			{
				if (this.userContext != null && this.userContext.LogonIdentity != null)
				{
					return this.userContext.LogonIdentity;
				}
				return this.logonIdentity;
			}
			internal set
			{
				this.logonIdentity = value;
			}
		}

		public OwaIdentity MailboxIdentity
		{
			get
			{
				if (this.userContext != null && this.userContext.MailboxIdentity != null)
				{
					return this.userContext.MailboxIdentity;
				}
				return this.mailboxIdentity;
			}
			internal set
			{
				this.mailboxIdentity = value;
			}
		}

		internal TaskPerformanceData RpcData
		{
			get
			{
				return this.rpcData;
			}
		}

		internal PerformanceData EwsRpcData
		{
			get
			{
				return this.ewsRpcData;
			}
			set
			{
				this.ewsRpcData = value;
			}
		}

		internal TaskPerformanceData LdapData
		{
			get
			{
				return this.ldapData;
			}
		}

		internal PerformanceData EwsLdapData
		{
			get
			{
				return this.ewsLdapData;
			}
			set
			{
				this.ewsLdapData = value;
			}
		}

		internal string EwsPerformanceHeader
		{
			get
			{
				return this.ewsPerformanceHeader;
			}
			set
			{
				this.ewsPerformanceHeader = value;
			}
		}

		internal TaskPerformanceData MemoryData { get; private set; }

		internal long RequestLatencyMilliseconds
		{
			get
			{
				return (long)this.latencyDetectionContext.Elapsed.TotalMilliseconds;
			}
		}

		internal long RequestCpuLatencyMilliseconds
		{
			get
			{
				return (long)this.latencyDetectionContext.ElapsedCpu.TotalMilliseconds;
			}
		}

		internal bool HasTrustworthyRequestCpuLatency
		{
			get
			{
				return this.latencyDetectionContext.HasTrustworthyCpuTime;
			}
		}

		internal ExchangePrincipal LogonExchangePrincipal
		{
			get
			{
				return this.logonExchangePrincipal;
			}
			set
			{
				this.logonExchangePrincipal = value;
			}
		}

		internal ExchangePrincipal ExchangePrincipal
		{
			get
			{
				return this.mailboxExchangePrincipal;
			}
			set
			{
				this.mailboxExchangePrincipal = value;
			}
		}

		internal OwaPerformanceData OwaPerformanceData
		{
			get
			{
				return this.owaPerformanceData;
			}
		}

		internal SearchPerformanceData SearchPerformanceData
		{
			get
			{
				return this.searchPerformanceData;
			}
			set
			{
				this.searchPerformanceData = value;
			}
		}

		internal RequestExecution RequestExecution
		{
			get
			{
				return this.requestExecution;
			}
			set
			{
				this.requestExecution = value;
			}
		}

		internal string UrlToHost
		{
			get
			{
				if (this.urlToHost == null)
				{
					HttpRequest request = this.HttpContext.Request;
					this.urlToHost = (request.IsSecureConnection ? "https://" : "http://");
					this.urlToHost += request.Url.Host;
					if (request.Url.Port != 80 && request.Url.Port != 443)
					{
						this.urlToHost = this.urlToHost + ":" + request.Url.Port.ToString(CultureInfo.InvariantCulture);
					}
				}
				return this.urlToHost;
			}
		}

		internal string LocalHostName
		{
			get
			{
				if (this.localHostName == null)
				{
					this.localHostName = this.UrlToHost + this.HttpContext.Request.ApplicationPath;
				}
				return this.localHostName;
			}
		}

		internal ProxyUri SecondCasUri
		{
			get
			{
				return this.secondCasUri;
			}
			set
			{
				this.secondCasUri = value;
			}
		}

		internal bool IsTemporaryRedirection
		{
			get
			{
				return this.isTemporaryRedirection;
			}
			set
			{
				this.isTemporaryRedirection = value;
			}
		}

		internal bool CanAccessUsualAddressInAnHour
		{
			get
			{
				return this.canAccessUsualAddressInAnHour;
			}
			set
			{
				this.canAccessUsualAddressInAnHour = value;
			}
		}

		internal FormsRegistryContext FormsRegistryContext
		{
			get
			{
				return this.formsRegistryContext;
			}
			set
			{
				this.formsRegistryContext = value;
			}
		}

		internal bool IsProxyRequest
		{
			get
			{
				return this.isProxyRequest;
			}
			set
			{
				this.isProxyRequest = value;
			}
		}

		internal bool IsFromCafe
		{
			get
			{
				return this.isFromCafe;
			}
			set
			{
				this.isFromCafe = value;
			}
		}

		internal string DestinationUrl
		{
			get
			{
				return this.destinationUrl;
			}
			set
			{
				this.destinationUrl = value;
			}
		}

		internal string DestinationUrlQueryString
		{
			get
			{
				return this.destinationUrlQueryString;
			}
			set
			{
				this.destinationUrlQueryString = value;
			}
		}

		internal bool IsManualRedirect
		{
			get
			{
				return this.isManualRedirect;
			}
			set
			{
				this.isManualRedirect = value;
			}
		}

		internal HttpStatusCode HttpStatusCode
		{
			get
			{
				return this.httpStatusCode;
			}
			set
			{
				this.httpStatusCode = value;
			}
		}

		public string ErrorString
		{
			get
			{
				return this.errorString;
			}
			set
			{
				this.errorString = value;
			}
		}

		internal CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
			set
			{
				this.culture = value;
			}
		}

		internal Guid TraceRequestId
		{
			get
			{
				return this.traceRequestID;
			}
			set
			{
				this.traceRequestID = value;
			}
		}

		internal string TimeZoneId
		{
			get
			{
				if (this.timeZoneId == null)
				{
					HttpCookie httpCookie = this.httpContext.Request.Cookies["tzid"];
					this.timeZoneId = ((httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value)) ? httpCookie.Value : null);
					if (this.timeZoneId == null && this.UserContext != null)
					{
						this.TimeZoneId = this.UserContext.TimeZone.Id;
					}
				}
				return this.timeZoneId;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("TimezoneId can't be set to null");
				}
				this.timeZoneId = value;
				this.httpContext.Response.Cookies.Add(new HttpCookie("tzid", value));
			}
		}

		public bool LoadedByFormsRegistry
		{
			get
			{
				return this[OwaContextProperty.LoadedByFormsRegistry] != null && (bool)this[OwaContextProperty.LoadedByFormsRegistry];
			}
			set
			{
				this[OwaContextProperty.LoadedByFormsRegistry] = value;
			}
		}

		public bool ErrorSent
		{
			get
			{
				return this[OwaContextProperty.ErrorSent] != null && (bool)this[OwaContextProperty.ErrorSent];
			}
			set
			{
				this[OwaContextProperty.ErrorSent] = value;
			}
		}

		public ErrorInformation ErrorInformation
		{
			get
			{
				return (ErrorInformation)this[OwaContextProperty.ErrorInformation];
			}
			set
			{
				this[OwaContextProperty.ErrorInformation] = value;
			}
		}

		internal OwaStoreObjectId PreFormActionId
		{
			get
			{
				return (OwaStoreObjectId)this[OwaContextProperty.PreFormActionId];
			}
			set
			{
				this[OwaContextProperty.PreFormActionId] = value;
			}
		}

		public object PreFormActionData
		{
			get
			{
				return this[OwaContextProperty.PreFormActionData];
			}
			set
			{
				this[OwaContextProperty.PreFormActionData] = value;
			}
		}

		internal Dictionary<string, object> InternalHandlerParameters
		{
			get
			{
				if (this.internalHandlerParameters == null)
				{
					this.internalHandlerParameters = new Dictionary<string, object>();
				}
				return this.internalHandlerParameters;
			}
		}

		public void SetInternalHandlerParameter(string name, object value)
		{
			this.InternalHandlerParameters.Add(name, value);
		}

		internal ProxyUriQueue ProxyUriQueue
		{
			get
			{
				return this.proxyUriQueue;
			}
			set
			{
				this.proxyUriQueue = value;
			}
		}

		public ServerVersion ProxyCasVersion
		{
			get
			{
				return this.proxyCasVersion;
			}
			internal set
			{
				this.proxyCasVersion = value;
			}
		}

		public Uri ProxyCasUri
		{
			get
			{
				return this.proxyCasUri;
			}
			internal set
			{
				this.proxyCasUri = value;
			}
		}

		internal SecurityIdentifier ProxyUserSid
		{
			get
			{
				return this.proxyUserSid;
			}
			set
			{
				this.proxyUserSid = value;
			}
		}

		internal bool IsExplicitLogon
		{
			get
			{
				return this.isExplicitLogon;
			}
			set
			{
				this.isExplicitLogon = value;
			}
		}

		internal bool IsAlternateMailbox
		{
			get
			{
				return this.isAlternateMailbox;
			}
			set
			{
				this.isAlternateMailbox = value;
			}
		}

		internal bool IsDifferentMailbox
		{
			get
			{
				return this.IsExplicitLogon || this.IsAlternateMailbox;
			}
		}

		internal bool IsProxyWebPart
		{
			get
			{
				return this.isProxyWebPart;
			}
			set
			{
				this.isProxyWebPart = value;
			}
		}

		internal CultureInfo LanguagePostUserCulture
		{
			get
			{
				return this.languagePostUserCulture;
			}
			set
			{
				this.languagePostUserCulture = value;
			}
		}

		internal bool FailedToSaveUserCulture
		{
			get
			{
				return this.failedToSaveUserCulture;
			}
			set
			{
				this.failedToSaveUserCulture = value;
			}
		}

		internal SerializedClientSecurityContext ReceivedSerializedClientSecurityContext
		{
			get
			{
				return this.receivedSerializedClientSecurityContext;
			}
			set
			{
				this.receivedSerializedClientSecurityContext = value;
			}
		}

		internal IDictionary<string, string> CustomErrorInfo
		{
			get
			{
				return this.customErrorInfo;
			}
			set
			{
				this.customErrorInfo = value;
			}
		}

		internal SanitizingTextWriter<OwaHtml> SanitizingResponseWriter
		{
			get
			{
				if (this.cachedResponseOutput != this.httpContext.Response.Output)
				{
					this.cachedResponseOutput = this.httpContext.Response.Output;
					this.sanitizingResponseWriter = new SanitizingTextWriter<OwaHtml>(this.cachedResponseOutput);
				}
				return this.sanitizingResponseWriter;
			}
		}

		internal LiveAssetReader LiveAssetReader
		{
			get
			{
				if (this.liveAssetReader == null)
				{
					this.liveAssetReader = new LiveAssetReader(this.httpContext);
				}
				return this.liveAssetReader;
			}
		}

		private Uri GetModifiedUri()
		{
			Uri uri = this.httpContext.Request.Url;
			if (!string.IsNullOrEmpty(uri.Query))
			{
				MatchCollection matchCollection = OwaContext.AllowedParametersExpression.Matches(uri.Query);
				UriBuilder uriBuilder = new UriBuilder(uri);
				StringBuilder stringBuilder = new StringBuilder(uriBuilder.Query.Length);
				if (matchCollection.Count > 0)
				{
					int num = matchCollection.Count - 1;
					for (int i = 0; i < num; i++)
					{
						stringBuilder.Append(matchCollection[i].Value).Append('&');
					}
					stringBuilder.Append(matchCollection[num].Value);
				}
				uriBuilder.Query = stringBuilder.ToString();
				uri = uriBuilder.Uri;
			}
			return uri;
		}

		private const string TimeZoneIdCookieName = "tzid";

		private const string OwaContextKey = "OwaContext";

		private static readonly TimeSpan DefaultOwaThreshold = TimeSpan.FromSeconds(30.0);

		private static readonly TimeSpan MinimumOwaThreshold = TimeSpan.FromMilliseconds(100.0);

		private static readonly LatencyDetectionContextFactory OwaLatencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory("OwaContext", OwaContext.MinimumOwaThreshold, OwaContext.DefaultOwaThreshold);

		private static readonly Regex AllowedParametersExpression = new Regex("(?:a|ae|t|ns)=\\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly int PropertyBagSize = Enum.GetValues(typeof(OwaContextProperty)).Length;

		private readonly Dictionary<OwaContextProperty, object> propertyBag = new Dictionary<OwaContextProperty, object>(OwaContext.PropertyBagSize);

		private HttpContext httpContext;

		private UserContext userContext;

		private OwaRequestType requestType;

		private OwaIdentity logonIdentity;

		private OwaIdentity mailboxIdentity;

		private ExchangePrincipal logonExchangePrincipal;

		private ExchangePrincipal mailboxExchangePrincipal;

		private RequestExecution requestExecution;

		private string urlToHost;

		private string localHostName;

		private FormsRegistryContext formsRegistryContext;

		private bool isExplicitLogon;

		private bool isAlternateMailbox;

		private CultureInfo languagePostUserCulture;

		private bool failedToSaveUserCulture;

		private SerializedClientSecurityContext receivedSerializedClientSecurityContext;

		private ServerVersion proxyCasVersion;

		private Uri proxyCasUri;

		private SecurityIdentifier proxyUserSid;

		private bool isProxyRequest;

		private bool isFromCafe;

		private ProxyUri secondCasUri;

		private ProxyUriQueue proxyUriQueue;

		private bool isProxyWebPart;

		private bool isTemporaryRedirection;

		private bool canAccessUsualAddressInAnHour;

		private bool handledCriticalError;

		private Dictionary<string, object> internalHandlerParameters;

		private string destinationUrl;

		private string destinationUrlQueryString;

		private bool isManualRedirect = true;

		private HttpStatusCode httpStatusCode = HttpStatusCode.OK;

		private string errorString;

		private CultureInfo culture;

		private IDictionary<string, string> customErrorInfo;

		private List<IDisposable> objectsToDispose;

		internal uint AvailabilityQueryCount;

		internal long AvailabilityQueryLatency;

		private LatencyDetectionContext latencyDetectionContext;

		private TaskPerformanceData rpcData;

		private TaskPerformanceData ldapData;

		private PerformanceData ewsRpcData;

		private PerformanceData ewsLdapData;

		private string ewsPerformanceHeader;

		private Guid traceRequestID;

		private OwaPerformanceData owaPerformanceData;

		private SearchPerformanceData searchPerformanceData;

		private IStandardBudget budget;

		private bool isAsyncRequest;

		private string timeZoneId;

		private bool shouldDeferInlineImages;

		private SanitizingTextWriter<OwaHtml> sanitizingResponseWriter;

		private TextWriter cachedResponseOutput;

		private SecureNameValueCollection formNameValueCollection;

		private LiveAssetReader liveAssetReader;

		private AnonymousSessionContext anonymousSessionContext;
	}
}
