using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class RemotePowerShellProxyRequestHandler : BEServerCookieProxyRequestHandler<WebServicesService>
	{
		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.InternalNLBBypass;
			}
		}

		protected override string[] BackEndCookieNames
		{
			get
			{
				return RemotePowerShellProxyRequestHandler.ClientSupportedBackEndCookieNames;
			}
		}

		protected override int MaxBackEndCookieEntries
		{
			get
			{
				return 1;
			}
		}

		protected override void OnInitializingHandler()
		{
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(base.HttpContext.Request.Url.Query.Replace(';', '&'));
			string value = nameValueCollection["DelegatedOrg"];
			if (!string.IsNullOrEmpty(value))
			{
				this.isSyndicatedAdmin = true;
				return;
			}
			base.OnInitializingHandler();
		}

		protected override void ResetForRetryOnError()
		{
			if (base.ClientResponse != null && base.ClientResponse.Headers != null)
			{
				WinRMInfo.ClearFailureCategoryInfo(base.ClientResponse.Headers);
			}
			base.ResetForRetryOnError();
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			if (base.ClientRequest.IsAuthenticated && base.ProxyToDownLevel)
			{
				IIdentity callerIdentity = base.HttpContext.GetCallerIdentity();
				WindowsIdentity windowsIdentity = callerIdentity as WindowsIdentity;
				GenericSidIdentity genericSidIdentity = callerIdentity as GenericSidIdentity;
				IPrincipal user = base.HttpContext.User;
				if (windowsIdentity != null)
				{
					if (!string.IsNullOrEmpty((string)base.HttpContext.Items[Constants.WLIDMemberName]))
					{
						headers["X-RemotePS-GenericIdentity"] = windowsIdentity.User.ToString();
					}
					else
					{
						headers["X-RemotePS-WindowsIdentity"] = base.HttpContext.GetSerializedAccessTokenString();
					}
				}
				else if (genericSidIdentity != null)
				{
					headers["X-RemotePS-GenericIdentity"] = genericSidIdentity.Sid.ToString();
				}
				else
				{
					headers["X-RemotePS-GenericIdentity"] = base.HttpContext.User.Identity.GetSafeName(true);
				}
			}
			if (this.isSyndicatedAdminManageDownLevelTarget)
			{
				headers["msExchCafeForceRouteToLogonAccount"] = "1";
			}
			WinRMInfo winRMInfo = null;
			if (LoggerHelper.IsProbePingRequest(base.ClientRequest))
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.Logger, HttpProxyMetadata.ProtocolAction, "ProbePingBackend");
			}
			else if (WinRMHelper.WinRMParserEnabled.Value)
			{
				try
				{
					winRMInfo = base.ParseClientRequest<WinRMInfo>(new Func<Stream, WinRMInfo>(this.ParseWinRMInfo), 10000);
				}
				catch (InvalidOperationException ex)
				{
					ExTraceGlobals.ExceptionTracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::AddProtocolSpecificHeadersToServerRequest] ParseClientRequest throws exception {0}", ex);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(base.Logger, "ParseClientRequestException", ex.ToString());
				}
				if (winRMInfo != null)
				{
					WinRMInfo.StampToHttpHeaders(winRMInfo, headers);
				}
			}
			DatabaseBasedAnchorMailbox databaseBasedAnchorMailbox = base.AnchoredRoutingTarget.AnchorMailbox as DatabaseBasedAnchorMailbox;
			if (databaseBasedAnchorMailbox != null)
			{
				ADObjectId database = databaseBasedAnchorMailbox.GetDatabase();
				if (database != null)
				{
					headers[WellKnownHeader.MailboxDatabaseGuid] = database.ObjectGuid.ToString();
				}
			}
			if (!base.ShouldRetryOnError)
			{
				headers[WellKnownHeader.XCafeLastRetryHeaderKey] = "Y";
			}
			base.AddProtocolSpecificHeadersToServerRequest(headers);
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return !string.Equals(headerName, "X-RemotePS-GenericIdentity", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "X-RemotePS-WindowsIdentity", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "msExchCafeForceRouteToLogonAccount", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, WellKnownHeader.MailboxDatabaseGuid, StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, WellKnownHeader.XCafeLastRetryHeaderKey, StringComparison.OrdinalIgnoreCase) && !WinRMInfo.IsHeaderReserverd(headerName) && base.ShouldCopyHeaderToServerRequest(headerName);
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			Uri targetBackEndServerUrl = base.GetTargetBackEndServerUrl();
			if (targetBackEndServerUrl.Port == 444)
			{
				return targetBackEndServerUrl;
			}
			string absolutePath = targetBackEndServerUrl.AbsolutePath;
			if (string.IsNullOrEmpty(absolutePath))
			{
				throw new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.EndpointNotFound, string.Format("Unable to process URL: " + targetBackEndServerUrl.ToString(), new object[0]));
			}
			UriBuilder uriBuilder = new UriBuilder(targetBackEndServerUrl);
			int num = absolutePath.IndexOf('/', 1);
			if (num > 1)
			{
				uriBuilder.Path = absolutePath.Substring(0, num) + "-proxy" + absolutePath.Substring(num);
			}
			else
			{
				uriBuilder.Path = absolutePath + "-proxy";
			}
			return uriBuilder.Uri;
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(base.HttpContext.Request.Url.Query.Replace(';', '&'));
			string text = nameValueCollection["TargetServer"];
			if (!string.IsNullOrEmpty(text))
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "TargetServer");
				return new ServerInfoAnchorMailbox(text, this);
			}
			string text2 = nameValueCollection["ExchClientVer"];
			if (!string.IsNullOrWhiteSpace(text2))
			{
				text2 = Utilities.NormalizeExchClientVer(text2);
			}
			if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExchClientVer");
				return base.GetServerVersionAnchorMailbox(text2);
			}
			bool flag;
			string text3;
			string routingBasedOrganization = this.GetRoutingBasedOrganization(nameValueCollection, out flag, out text3);
			if (!this.isSyndicatedAdmin && !string.IsNullOrWhiteSpace(text2))
			{
				if (!string.IsNullOrWhiteSpace(routingBasedOrganization))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::ResolveAnchorMailbox]: Datacenter, Version parameter {0}, from {1}, organization {2}, context {3}.", new object[]
					{
						text2,
						text3,
						routingBasedOrganization,
						base.TraceContext
					});
					base.Logger.Set(HttpProxyMetadata.RoutingHint, text3 + "-" + text2);
					return VersionedDomainAnchorMailbox.GetAnchorMailbox(routingBasedOrganization, text2, this);
				}
				ExTraceGlobals.VerboseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::ResolveAnchorMailbox]: ExchClientVer {0} is specified, but User-Org/Org anization/DelegatedOrg is null. Go with normal routing. Context {2}.", text2, base.TraceContext);
			}
			string text4 = nameValueCollection["DelegatedUser"];
			if (!string.IsNullOrEmpty(text4))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::ResolveAnchorMailbox]: User hint {0}, context {1}.", text4, base.TraceContext);
				if (!string.IsNullOrEmpty(text4) && SmtpAddress.IsValidSmtpAddress(text4))
				{
					base.Logger.Set(HttpProxyMetadata.RoutingHint, "DelegatedUser-SMTP-UrlQuery");
					return new SmtpAnchorMailbox(text4, this);
				}
			}
			if (flag)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::ResolveAnchorMailbox]: Organization-based. Organization {0} from {1}, context {2}.", routingBasedOrganization, text3, base.TraceContext);
				DomainAnchorMailbox domainAnchorMailbox = new DomainAnchorMailbox(routingBasedOrganization, this);
				if (this.isSyndicatedAdmin && !this.IsSecurityTokenPresent())
				{
					ADRawEntry adrawEntry = domainAnchorMailbox.GetADRawEntry();
					ExchangeObjectVersion exchangeObjectVersion = adrawEntry[OrganizationConfigSchema.AdminDisplayVersion] as ExchangeObjectVersion;
					if (exchangeObjectVersion.ExchangeBuild.Major < ExchangeObjectVersion.Exchange2012.ExchangeBuild.Major)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<ExchangeObjectVersion>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::ResolveAnchorMailbox] Syndicated Admin. Target tenant is in E14 forest. Let backend generate security token and do the redirection. ExchangeVersion: {0}", exchangeObjectVersion);
						this.isSyndicatedAdminManageDownLevelTarget = true;
						base.Logger.AppendGenericInfo("SyndicatedAdminTargetTenantDownLevel", true);
						return base.ResolveAnchorMailbox();
					}
				}
				base.Logger.Set(HttpProxyMetadata.RoutingHint, text3);
				return domainAnchorMailbox;
			}
			return base.ResolveAnchorMailbox();
		}

		protected override Uri UpdateExternalRedirectUrl(Uri originalRedirectUrl)
		{
			return new UriBuilder(base.ClientRequest.Url)
			{
				Host = originalRedirectUrl.Host,
				Port = originalRedirectUrl.Port
			}.Uri;
		}

		protected override void ExposeExceptionToClientResponse(Exception ex)
		{
			if (!WinRMHelper.FriendlyErrorEnabled.Value)
			{
				base.ExposeExceptionToClientResponse(ex);
				return;
			}
			if (ex is WebException)
			{
				WebException ex2 = (WebException)ex;
				if (WinRMHelper.IsPingRequest(ex2))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::ExposeExceptionToClientResponse]: Context={0}, Ping found.", base.TraceContext);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.Logger, HttpProxyMetadata.ProtocolAction, "Ping");
					base.ClientResponse.Headers["X-RemotePS-Ping"] = "Ping";
					return;
				}
				if (WinRMHelper.CouldBePingRequest(ex2))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::ExposeExceptionToClientResponse]: Context={0}, Could be Ping request.", base.TraceContext);
					base.ClientResponse.Headers["X-RemotePS-Ping"] = "Possible-Ping";
					return;
				}
				if (ex2.Status != WebExceptionStatus.ProtocolError)
				{
					WinRMInfo.SetFailureCategoryInfo(base.ClientResponse.Headers, FailureCategory.Cafe, ex2.Status.ToString());
				}
			}
			if (ex is HttpProxyException && !string.IsNullOrWhiteSpace(ex.Message) && !WinRMHelper.DiagnosticsInfoHasBeenWritten(base.ClientResponse.Headers))
			{
				WinRMInfo.SetFailureCategoryInfo(base.ClientResponse.Headers, FailureCategory.Cafe, ex.GetType().Name);
				string diagnosticsInfo = WinRMHelper.GetDiagnosticsInfo(base.HttpContext);
				ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::ExposeExceptionToClientResponse]: Context={0}, Write Message {1} to client response.", base.TraceContext, ex.Message);
				WinRMHelper.SetDiagnosticsInfoWrittenFlag(base.ClientResponse.Headers);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.Logger, "FriendlyError", "ExposeException");
				base.ClientResponse.Write(diagnosticsInfo + ex.Message);
				return;
			}
			base.ExposeExceptionToClientResponse(ex);
		}

		protected override StreamProxy BuildResponseStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer)
		{
			if (!LoggerHelper.IsProbePingRequest(base.ClientRequest) && WinRMHelper.FriendlyErrorEnabled.Value)
			{
				return new RpsOutDataResponseStreamProxy(streamProxyType, source, target, buffer, this);
			}
			return base.BuildResponseStreamProxy(streamProxyType, source, target, buffer);
		}

		private string GetRoutingBasedOrganization(NameValueCollection urlParameters, out bool routeBasedOnOrgnaizationInUrl, out string organizationRoutingHint)
		{
			string text = urlParameters["organization"];
			if (!string.IsNullOrEmpty(text))
			{
				routeBasedOnOrgnaizationInUrl = true;
				organizationRoutingHint = "Url-Organization";
				return text;
			}
			text = urlParameters["DelegatedOrg"];
			if (!string.IsNullOrEmpty(text))
			{
				this.RedirectIfNeeded();
				routeBasedOnOrgnaizationInUrl = true;
				organizationRoutingHint = "Url-DelegatedOrg";
				return text;
			}
			routeBasedOnOrgnaizationInUrl = false;
			return this.GetExecutingUserOrganization(out organizationRoutingHint);
		}

		private string GetExecutingUserOrganization(out string organizatonRoutingHint)
		{
			organizatonRoutingHint = null;
			CommonAccessToken commonAccessToken = base.HttpContext.Items["Item-CommonAccessToken"] as CommonAccessToken;
			if (commonAccessToken == null)
			{
				if (base.AuthBehavior.AuthState != AuthState.FrontEndFullAuth)
				{
					string executingUserOrganization = base.AuthBehavior.GetExecutingUserOrganization();
					if (!string.IsNullOrEmpty(executingUserOrganization))
					{
						organizatonRoutingHint = "LiveIdBasic-UserOrg";
						return executingUserOrganization;
					}
				}
				return null;
			}
			switch ((AccessTokenType)Enum.Parse(typeof(AccessTokenType), commonAccessToken.TokenType, true))
			{
			case AccessTokenType.LiveIdBasic:
			{
				LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = LiveIdBasicTokenAccessor.Attach(commonAccessToken);
				SmtpAddress smtpAddress = new SmtpAddress(liveIdBasicTokenAccessor.LiveIdMemberName);
				organizatonRoutingHint = "LiveIdBasic-UserOrg";
				return smtpAddress.Domain;
			}
			case AccessTokenType.LiveIdNego2:
			{
				string result;
				commonAccessToken.ExtensionData.TryGetValue("OrganizationName", out result);
				organizatonRoutingHint = "LiveIdNego2-UserOrg";
				return result;
			}
			default:
				return null;
			}
		}

		private void RedirectIfNeeded()
		{
			if (!this.IsSecurityTokenPresent())
			{
				UserBasedAnchorMailbox userBasedAnchorMailbox = AnchorMailboxFactory.CreateFromCaller(this) as UserBasedAnchorMailbox;
				if (userBasedAnchorMailbox != null)
				{
					ADRawEntry adrawEntry = userBasedAnchorMailbox.GetADRawEntry();
					ExTraceGlobals.VerboseTracer.TraceDebug<UserBasedAnchorMailbox, ExchangeObjectVersion>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::RedirectIfNeeded] Redirect if the user is in E14 forest. User: {0}; ExchangeVersion: {1}", userBasedAnchorMailbox, adrawEntry.ExchangeVersion);
					if (adrawEntry.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012))
					{
						base.Logger.AppendGenericInfo("RedirectReason", "NoSecurityTokenAndOlderThanE15");
						base.DatacenterRedirectStrategy.RedirectMailbox(userBasedAnchorMailbox);
					}
				}
			}
		}

		private bool IsSecurityTokenPresent()
		{
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(base.ClientRequest.Url.Query.Replace(';', '&'));
			string value = nameValueCollection["SecurityToken"];
			bool flag = !string.IsNullOrEmpty(value);
			ExTraceGlobals.VerboseTracer.TraceDebug<bool>((long)this.GetHashCode(), "[RemotePowerShellProxyRequestHandler::IsSecurityTokenPresent] {0}", flag);
			return flag;
		}

		private WinRMInfo ParseWinRMInfo(Stream stream)
		{
			WinRMParser winRMParser = new WinRMParser(base.TraceContext);
			WinRMInfo winRMInfo;
			string value;
			if (winRMParser.TryParseStream(stream, out winRMInfo, out value))
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.Logger, HttpProxyMetadata.ProtocolAction, winRMInfo.Action);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.Logger, "CommandId", winRMInfo.CommandId);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.Logger, "SessionId", winRMInfo.SessionId);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.Logger, "ShellId", winRMInfo.ShellId);
				if (!"http://schemas.microsoft.com/wbem/wsman/1/windows/shell/signal/terminate".Equals(winRMInfo.SignalCode, StringComparison.OrdinalIgnoreCase))
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.Logger, "SignalCode", winRMInfo.SignalCode);
				}
			}
			else
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.Logger, HttpProxyMetadata.ProtocolAction, value);
			}
			return winRMInfo;
		}

		private const string CafeForceRouteToLogonAccountHeaderKey = "msExchCafeForceRouteToLogonAccount";

		private const string SecurityTokenKey = "SecurityToken";

		private static readonly string[] ClientSupportedBackEndCookieNames = new string[]
		{
			Constants.RPSBackEndServerCookieName
		};

		private static readonly Regex ExchClientVerRegex = new Regex("(?<major>\\d{2})\\.(?<minor>\\d{1,})\\.(?<build>\\d{1,})\\.(?<revision>\\d{1,})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private bool isSyndicatedAdmin;

		private bool isSyndicatedAdminManageDownLevelTarget;
	}
}
