using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class BEServerCookieProxyRequestHandler<ServiceType> : ProxyRequestHandler where ServiceType : HttpService
	{
		internal string Domain { get; set; }

		protected bool IsWsSecurityRequest { get; set; }

		protected bool IsDomainBasedRequest { get; set; }

		protected abstract ClientAccessType ClientAccessType { get; }

		protected override bool WillAddProtocolSpecificCookiesToClientResponse
		{
			get
			{
				return true;
			}
		}

		protected virtual int MaxBackEndCookieEntries
		{
			get
			{
				return 5;
			}
		}

		protected virtual string[] BackEndCookieNames
		{
			get
			{
				return BEServerCookieProxyRequestHandler<ServiceType>.ClientSupportedBackEndCookieNames;
			}
		}

		protected override bool ShouldBackendRequestBeAnonymous()
		{
			return this.IsWsSecurityRequest;
		}

		protected override BackEndServer GetDownLevelClientAccessServer(AnchorMailbox anchorMailbox, BackEndServer mailboxServer)
		{
			if (mailboxServer.Version < Server.E14MinVersion)
			{
				return this.GetE12TargetServer(mailboxServer);
			}
			Uri uri = null;
			BackEndServer downLevelClientAccessServer = DownLevelServerManager.Instance.GetDownLevelClientAccessServer<ServiceType>(anchorMailbox, mailboxServer, this.ClientAccessType, base.Logger, HttpProxyGlobals.ProtocolType == ProtocolType.Owa || HttpProxyGlobals.ProtocolType == ProtocolType.OwaCalendar || HttpProxyGlobals.ProtocolType == ProtocolType.Ecp, out uri);
			if (uri != null)
			{
				Uri uri2 = this.UpdateExternalRedirectUrl(uri);
				if (Uri.Compare(uri2, base.ClientRequest.Url, UriComponents.Host, UriFormat.Unescaped, StringComparison.Ordinal) != 0)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::GetDownLevelClientAccessServer]: Stop processing and redirect to {0}.", uri2.ToString());
					throw new HttpException(302, uri2.ToString());
				}
			}
			return downLevelClientAccessServer;
		}

		protected override void ResetForRetryOnError()
		{
			this.haveSetBackEndCookie = false;
			this.removeBackEndCookieEntry = false;
			base.ResetForRetryOnError();
		}

		protected virtual BackEndServer GetE12TargetServer(BackEndServer mailboxServer)
		{
			return MailboxServerCache.Instance.GetRandomE15Server(this);
		}

		protected virtual Uri UpdateExternalRedirectUrl(Uri originalRedirectUrl)
		{
			return originalRedirectUrl;
		}

		protected virtual bool ShouldExcludeFromExplicitLogonParsing()
		{
			return true;
		}

		protected virtual bool IsValidExplicitLogonNode(string node, bool nodeIsLast)
		{
			return true;
		}

		protected override bool ShouldCopyCookieToServerRequest(HttpCookie cookie)
		{
			return !FbaModule.IsCadataCookie(cookie.Name) && (base.AuthBehavior.AuthState == AuthState.BackEndFullAuth || (!string.Equals(cookie.Name, Constants.LiveIdRPSAuth, StringComparison.OrdinalIgnoreCase) && !string.Equals(cookie.Name, Constants.LiveIdRPSSecAuth, StringComparison.OrdinalIgnoreCase) && !string.Equals(cookie.Name, Constants.LiveIdRPSTAuth, StringComparison.OrdinalIgnoreCase))) && !this.BackEndCookieNames.Any((string cookieName) => string.Equals(cookie.Name, cookieName, StringComparison.OrdinalIgnoreCase)) && !string.Equals(cookie.Name, Constants.RPSBackEndServerCookieName, StringComparison.OrdinalIgnoreCase) && base.ShouldCopyCookieToServerRequest(cookie);
		}

		protected override void CopySupplementalCookiesToClientResponse()
		{
			this.SetBackEndCookie();
			base.CopySupplementalCookiesToClientResponse();
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			AnchorMailbox anchorMailbox = null;
			if (!base.HasPreemptivelyCheckedForRoutingHint)
			{
				anchorMailbox = base.CreateAnchorMailboxFromRoutingHint();
			}
			if (anchorMailbox != null)
			{
				return anchorMailbox;
			}
			anchorMailbox = this.TryGetAnchorMailboxFromWsSecurityRequest();
			if (anchorMailbox != null)
			{
				return anchorMailbox;
			}
			anchorMailbox = this.TryGetAnchorMailboxFromDomainBasedRequest();
			if (anchorMailbox != null)
			{
				return anchorMailbox;
			}
			return AnchorMailboxFactory.CreateFromCaller(this);
		}

		protected override AnchoredRoutingTarget TryFastTargetCalculationByAnchorMailbox(AnchorMailbox anchorMailbox)
		{
			if (this.backEndCookie == null || !base.IsRetryOnErrorEnabled)
			{
				this.FetchBackEndServerCookie();
			}
			PerfCounters.HttpProxyCacheCountersInstance.CookieUseRateBase.Increment();
			PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.HttpProxyCacheCountersInstance.MovingPercentageCookieUseRate);
			if (this.backEndCookie != null)
			{
				BackEndServer backEndServer = anchorMailbox.AcceptBackEndCookie(this.backEndCookie);
				if (backEndServer != null)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailbox, BackEndServer>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::TryFastTargetCalculationByAnchorMailbox]: Back end server {1} resolved from anchor mailbox {0}", anchorMailbox, backEndServer);
					base.Logger.AppendString(HttpProxyMetadata.RoutingHint, "-ServerCookie");
					return new AnchoredRoutingTarget(anchorMailbox, backEndServer);
				}
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailbox>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::TryFastTargetCalculationByAnchorMailbox]: No cookie associated with anchor mailbox {0}", anchorMailbox);
			return base.TryFastTargetCalculationByAnchorMailbox(anchorMailbox);
		}

		protected virtual string TryGetExplicitLogonNode(ExplicitLogonNode node)
		{
			if (this.ShouldExcludeFromExplicitLogonParsing())
			{
				return null;
			}
			string text = null;
			bool nodeIsLast;
			string explicitLogonNode = ProtocolHelper.GetExplicitLogonNode(base.ClientRequest.ApplicationPath, base.ClientRequest.FilePath, node, out nodeIsLast);
			if (!string.IsNullOrEmpty(explicitLogonNode) && this.IsValidExplicitLogonNode(explicitLogonNode, nodeIsLast))
			{
				text = explicitLogonNode;
				ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[OwaEcpProxyRequestHandler::TryGetExplicitLogonNode]: Context {0}; candidate explicit logon node: {1}", base.TraceContext, text);
			}
			return text;
		}

		protected AnchorMailbox TryGetAnchorMailboxFromWsSecurityRequest()
		{
			if (!this.IsWsSecurityRequest)
			{
				return null;
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::TryGetAnchorMailboxFromWsSecurityRequest]: Context {0}; WSSecurity request.", base.TraceContext);
			WsSecurityParser @object = new WsSecurityParser(base.TraceContext);
			bool flag = false;
			string address;
			if (base.ClientRequest.IsPartnerAuthRequest())
			{
				address = base.ParseClientRequest<string>(new Func<Stream, string>(@object.FindAddressFromPartnerAuthRequest), 73628);
			}
			else if (base.ClientRequest.IsX509CertAuthRequest())
			{
				address = base.ParseClientRequest<string>(new Func<Stream, string>(@object.FindAddressFromX509CertAuthRequest), 73628);
			}
			else
			{
				KeyValuePair<string, bool> keyValuePair = base.ParseClientRequest<KeyValuePair<string, bool>>(new Func<Stream, KeyValuePair<string, bool>>(@object.FindAddressFromWsSecurityRequest), 73628);
				flag = keyValuePair.Value;
				address = keyValuePair.Key;
			}
			if (flag)
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "WSSecurityRequest-DelegationToken-Random");
				return new AnonymousAnchorMailbox(this);
			}
			return AnchorMailboxFactory.CreateFromSamlTokenAddress(address, this);
		}

		protected AnchorMailbox TryGetAnchorMailboxFromDomainBasedRequest()
		{
			if (!this.IsDomainBasedRequest)
			{
				return null;
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::ResolveAnchorMailbox]: Context {0}; Domain-based request with domain {1}.", base.TraceContext, this.Domain);
			if (!string.IsNullOrEmpty(this.Domain) && SmtpAddress.IsValidDomain(this.Domain))
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "DomainBaseRequest-Domain");
				return new DomainAnchorMailbox(this.Domain, this);
			}
			base.Logger.Set(HttpProxyMetadata.RoutingHint, "DomainBaseRequest-Random");
			return new AnonymousAnchorMailbox(this);
		}

		protected ServerVersionAnchorMailbox<ServiceType> GetServerVersionAnchorMailbox(string serverVersionString)
		{
			ServerVersion serverVersion = new ServerVersion(LocalServerCache.LocalServer.VersionNumber);
			if (!string.IsNullOrEmpty(serverVersionString))
			{
				Match match = Constants.ExchClientVerRegex.Match(serverVersionString);
				ServerVersion serverVersion2;
				if (match.Success && RegexUtilities.TryGetServerVersionFromRegexMatch(match, out serverVersion2) && serverVersion2.Major >= 14)
				{
					serverVersion = serverVersion2;
				}
			}
			int build = (serverVersion.Build > 0) ? (serverVersion.Build - 1) : serverVersion.Build;
			serverVersion = new ServerVersion(serverVersion.Major, serverVersion.Minor, build, serverVersion.Revision);
			return new ServerVersionAnchorMailbox<ServiceType>(serverVersion, this.ClientAccessType, false, this);
		}

		protected override void UpdateOrInvalidateAnchorMailboxCache(Guid mdbGuid, string resourceForest)
		{
			this.removeBackEndCookieEntry = true;
			this.SetBackEndCookie();
			base.UpdateOrInvalidateAnchorMailboxCache(mdbGuid, resourceForest);
		}

		protected override void OnDatabaseNotFound(AnchorMailbox anchorMailbox)
		{
			foreach (string name in this.BackEndCookieNames)
			{
				Utility.DeleteCookie(base.ClientRequest, base.ClientResponse, name, this.GetCookiePath(), false);
				Utility.DeleteCookie(base.ClientRequest, base.ClientResponse, name, null, true);
			}
			base.OnDatabaseNotFound(anchorMailbox);
		}

		private void FetchBackEndServerCookie()
		{
			foreach (string text in this.BackEndCookieNames)
			{
				if (this.ShouldProcessBackEndCookie(text))
				{
					HttpCookie httpCookie = base.ClientRequest.Cookies[text];
					if (httpCookie != null && httpCookie.Values != null)
					{
						this.backEndCookie = httpCookie;
						if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder();
							foreach (string text2 in httpCookie.Values.AllKeys)
							{
								stringBuilder.AppendFormat("{0}:{1};", text2, httpCookie.Values[text2]);
							}
							ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::FetchBackEndServerCookie]: Context {0}; Recieving cookie {1}", base.TraceContext, stringBuilder.ToString());
							return;
						}
						break;
					}
				}
			}
		}

		private void SanitizeCookie(HttpCookie backEndCookie)
		{
			if (backEndCookie == null)
			{
				return;
			}
			if (this.removeBackEndCookieEntry && base.AnchoredRoutingTarget != null)
			{
				string text = base.AnchoredRoutingTarget.AnchorMailbox.ToCookieKey();
				backEndCookie.Values.Remove(text);
				ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::SanitizeCookie]: Context {0}; Removed cookie entry with key {1}.", base.TraceContext, text);
			}
			ExDateTime t = ExDateTime.UtcNow.AddYears(-30);
			int num = 0;
			for (int i = backEndCookie.Values.Count - 1; i >= 0; i--)
			{
				bool flag = true;
				BackEndCookieEntryBase backEndCookieEntryBase = null;
				if (num < this.MaxBackEndCookieEntries)
				{
					string entryValue = backEndCookie.Values[i];
					if (BackEndCookieEntryParser.TryParse(entryValue, out backEndCookieEntryBase))
					{
						flag = backEndCookieEntryBase.Expired;
						if (!flag && this.removeBackEndCookieEntry && base.AnchoredRoutingTarget != null && backEndCookieEntryBase.ShouldInvalidate(base.AnchoredRoutingTarget.BackEndServer))
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					string key = backEndCookie.Values.GetKey(i);
					backEndCookie.Values.Remove(key);
					ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::SanitizeCookie]: Context {0}; Removed cookie entry with key {1}.", base.TraceContext, key);
				}
				else
				{
					num++;
					if (backEndCookieEntryBase.ExpiryTime > t)
					{
						t = backEndCookieEntryBase.ExpiryTime;
					}
				}
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::SanitizeCookie]: Context {0}; {1}", base.TraceContext, (num == 0) ? "Marking current cookie as expired." : "Extending cookie expiration.");
			backEndCookie.Expires = t.UniversalTime;
		}

		private void SetBackEndCookie()
		{
			if (this.haveSetBackEndCookie)
			{
				return;
			}
			foreach (string text in this.BackEndCookieNames)
			{
				if (this.ShouldProcessBackEndCookie(text))
				{
					HttpCookie httpCookie = base.ClientRequest.Cookies[text];
					if (httpCookie == null)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::SetBackEndCookie]: Context {0}; Client request does not include back end cookie.", base.TraceContext);
						httpCookie = new HttpCookie(text);
					}
					httpCookie.HttpOnly = true;
					httpCookie.Secure = base.ClientRequest.IsSecureConnection;
					httpCookie.Path = this.GetCookiePath();
					if (base.AnchoredRoutingTarget != null)
					{
						string text2 = base.AnchoredRoutingTarget.AnchorMailbox.ToCookieKey();
						BackEndCookieEntryBase backEndCookieEntryBase = base.AnchoredRoutingTarget.AnchorMailbox.BuildCookieEntryForTarget(base.AnchoredRoutingTarget.BackEndServer, base.ProxyToDownLevel, this.ShouldBackEndCookieHaveResourceForest(text));
						if (backEndCookieEntryBase != null)
						{
							httpCookie.Values[text2] = backEndCookieEntryBase.ToObscureString();
							ExTraceGlobals.VerboseTracer.TraceDebug<int, string, BackEndCookieEntryBase>((long)this.GetHashCode(), "[BEServerCookieProxyRequestHandler::SetBackEndCookie]: Context {0}; Setting cookie entry {1}={2}.", base.TraceContext, text2, backEndCookieEntryBase);
						}
					}
					this.SanitizeCookie(httpCookie);
					base.ClientResponse.Cookies.Add(httpCookie);
					this.haveSetBackEndCookie = true;
				}
			}
		}

		private string GetCookiePath()
		{
			if (base.ClientRequest.ApplicationPath.Length < base.ClientRequest.Url.AbsolutePath.Length)
			{
				return base.ClientRequest.Url.AbsolutePath.Remove(base.ClientRequest.ApplicationPath.Length);
			}
			return base.ClientRequest.Url.AbsolutePath;
		}

		private bool ShouldProcessBackEndCookie(string backEndCookieName)
		{
			return this.BackEndCookieNames.Length <= 1 || (((HttpProxySettings.SupportBackEndCookie.Value & ProxyRequestHandler.SupportBackEndCookie.V1) != (ProxyRequestHandler.SupportBackEndCookie)0 || !string.Equals(backEndCookieName, "X-BackEndCookie", StringComparison.OrdinalIgnoreCase)) && ((HttpProxySettings.SupportBackEndCookie.Value & ProxyRequestHandler.SupportBackEndCookie.V2) != (ProxyRequestHandler.SupportBackEndCookie)0 || !string.Equals(backEndCookieName, "X-BackEndCookie2", StringComparison.OrdinalIgnoreCase)));
		}

		private bool ShouldBackEndCookieHaveResourceForest(string backEndCookieName)
		{
			if ((HttpProxySettings.SupportBackEndCookie.Value & ProxyRequestHandler.SupportBackEndCookie.V2) != (ProxyRequestHandler.SupportBackEndCookie)0)
			{
				if (this.BackEndCookieNames.Length <= 1)
				{
					return true;
				}
				if (string.Equals(backEndCookieName, "X-BackEndCookie2", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private const string BackEndCookie2Name = "X-BackEndCookie2";

		private static readonly string[] ClientSupportedBackEndCookieNames = new string[]
		{
			"X-BackEndCookie2",
			"X-BackEndCookie"
		};

		private bool haveSetBackEndCookie;

		private bool removeBackEndCookieEntry;

		private HttpCookie backEndCookie;
	}
}
