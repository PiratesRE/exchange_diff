using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.HttpProxy
{
	internal class RpcHttpProxyRequestHandler : ProxyRequestHandler
	{
		internal RpcHttpProxyRequestHandler() : this(RpcHttpProxyRules.DefaultRpcHttpProxyRules)
		{
		}

		internal RpcHttpProxyRequestHandler(RpcHttpProxyRules rule)
		{
			this.proxyRules = rule;
		}

		protected override bool ShouldForceUnbufferedClientResponseOutput
		{
			get
			{
				return true;
			}
		}

		protected override bool IsBackendServerCacheValidationEnabled
		{
			get
			{
				return RpcHttpProxyRequestHandler.RpcHttpHeadRequestEnabled.Value && base.ClientRequest != null && base.ClientRequest.HttpMethod == "RPC_IN_DATA";
			}
		}

		protected override bool UseSmartBufferSizing
		{
			get
			{
				return true;
			}
		}

		protected override void BeginValidateBackendServerCache()
		{
			Exception ex = null;
			try
			{
				Uri targetBackEndServerUrl = this.GetTargetBackEndServerUrl();
				this.headRequest = base.CreateServerRequest(targetBackEndServerUrl);
				this.headRequest.Method = "HEAD";
				this.headRequest.Timeout = RpcHttpProxyRequestHandler.RpcHttpHeadRequestTimeout.Value;
				this.headRequest.KeepAlive = false;
				this.headRequest.ContentLength = 0L;
				this.headRequest.SendChunked = false;
				this.headRequest.BeginGetResponse(new AsyncCallback(RpcHttpProxyRequestHandler.ValidateBackendServerCacheCallback), base.ServerAsyncState);
				base.Logger.LogCurrentTime("H-BeginGetResponse");
			}
			catch (WebException ex2)
			{
				ex = ex2;
			}
			catch (HttpException ex3)
			{
				ex = ex3;
			}
			catch (IOException ex4)
			{
				ex = ex4;
			}
			catch (SocketException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				ExTraceGlobals.VerboseTracer.TraceError<Exception, int, ProxyRequestHandler.ProxyState>((long)this.GetHashCode(), "[ProxyRequestHandler::BeginValidateBackendServerCache]: An error occurred while trying to send head request: {0}; Context {1}; State {2}", ex, base.TraceContext, base.State);
				this.headRequest = null;
				base.BeginProxyRequestOrRecalculate();
			}
		}

		protected override StreamProxy BuildResponseStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer)
		{
			return this.BuildResponseStream(() => new RpcHttpOutDataResponseStreamProxy(streamProxyType, source, target, buffer, this), () => this.<>n__FabricatedMethod4(streamProxyType, source, target, buffer));
		}

		protected override StreamProxy BuildResponseStreamProxySmartSizing(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target)
		{
			return this.BuildResponseStream(() => new RpcHttpOutDataResponseStreamProxy(streamProxyType, source, target, HttpProxySettings.ResponseBufferSize.Value, HttpProxySettings.MinimumResponseBufferSize.Value, this), () => this.<>n__FabricatedMethod9(streamProxyType, source, target));
		}

		protected override void DoProtocolSpecificBeginProcess()
		{
			if (base.ClientRequest.HttpMethod.Equals("RPC_IN_DATA"))
			{
				base.ParseClientRequest<bool>(new Func<Stream, bool>(this.ParseOutAssociationGuid), 104);
			}
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			UriBuilder uriBuilder = new UriBuilder(base.ClientRequest.Url);
			if (string.IsNullOrEmpty(base.ClientRequest.Url.Query))
			{
				throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.EndpointNotFound, "No proxy destination is specified!");
			}
			RpcHttpQueryString rpcHttpQueryString = new RpcHttpQueryString(uriBuilder.Query);
			this.rpcServerTarget = rpcHttpQueryString.RcaServer;
			if (SmtpAddress.IsValidSmtpAddress(this.rpcServerTarget))
			{
				Guid mailboxGuid = Guid.Empty;
				string text = string.Empty;
				try
				{
					SmtpAddress smtpAddress = new SmtpAddress(this.rpcServerTarget);
					mailboxGuid = new Guid(smtpAddress.Local);
					text = smtpAddress.Domain;
				}
				catch (FormatException)
				{
					return this.ResolveToDefaultAnchorMailbox(this.rpcServerTarget, "InvalidFormat");
				}
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "MailboxGuidWithDomain");
				if (!text.Contains("."))
				{
					string text2 = base.HttpContext.Items[Constants.WLIDMemberName] as string;
					if (!string.IsNullOrEmpty(text2))
					{
						SmtpAddress smtpAddress2 = new SmtpAddress(text2);
						string domain = smtpAddress2.Domain;
						if (domain != null && !string.Equals(domain, text, StringComparison.OrdinalIgnoreCase))
						{
							ExTraceGlobals.BriefTracer.TraceError((long)this.GetHashCode(), "[RpcHttpProxyRequestHandler::ResolveAnchorMailbox]: Fixing up invalid domain name from client: {0} to {1}; Context {2}; State {3}", new object[]
							{
								text,
								domain,
								base.TraceContext,
								base.State
							});
							text = domain;
							this.rpcServerTarget = ExchangeRpcClientAccess.CreatePersonalizedServer(mailboxGuid, text);
							base.Logger.AppendString(HttpProxyMetadata.RoutingHint, "-ChangedToUserDomain");
						}
					}
				}
				this.updateRpcServer = true;
				return new MailboxGuidAnchorMailbox(mailboxGuid, text, this);
			}
			ProxyDestination proxyDestination;
			if (this.proxyRules.TryGetProxyDestination(this.rpcServerTarget, out proxyDestination))
			{
				string text3 = proxyDestination.GetHostName(this.GetKeyForCasAffinity());
				if (proxyDestination.IsDynamicTarget)
				{
					try
					{
						text3 = DownLevelServerManager.Instance.GetDownLevelClientAccessServerWithPreferredServer<RpcHttpService>(new ServerInfoAnchorMailbox(text3, this), text3, ClientAccessType.External, base.Logger, proxyDestination.Version).Fqdn;
					}
					catch (NoAvailableDownLevelBackEndException)
					{
						throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.EndpointNotFound, string.Format("Cannot find a healthy E12 or E14 CAS to proxy to: {0}", this.rpcServerTarget));
					}
				}
				uriBuilder.Host = text3;
				uriBuilder.Port = proxyDestination.Port;
				uriBuilder.Scheme = Uri.UriSchemeHttps;
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "RpcHttpProxyRules");
				this.updateRpcServer = false;
				return new UrlAnchorMailbox(uriBuilder.Uri, this);
			}
			return this.ResolveToDefaultAnchorMailbox(this.rpcServerTarget, "UnknownServerName");
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			UriBuilder uriBuilder = new UriBuilder(base.GetTargetBackEndServerUrl());
			if (this.updateRpcServer)
			{
				RpcHttpQueryString rpcHttpQueryString = new RpcHttpQueryString(uriBuilder.Query);
				if (string.IsNullOrEmpty(rpcHttpQueryString.RcaServerPort))
				{
					uriBuilder.Query = uriBuilder.Host + rpcHttpQueryString.AdditionalParameters;
				}
				else
				{
					uriBuilder.Query = uriBuilder.Host + ":" + rpcHttpQueryString.RcaServerPort + rpcHttpQueryString.AdditionalParameters;
				}
			}
			return uriBuilder.Uri;
		}

		protected override void SetProtocolSpecificServerRequestParameters(HttpWebRequest serverRequest)
		{
			serverRequest.AllowWriteStreamBuffering = false;
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return !RpcHttpProxyRequestHandler.ProtectedHeaderNames.Contains(headerName, StringComparer.OrdinalIgnoreCase) && base.ShouldCopyHeaderToServerRequest(headerName);
		}

		protected override bool ShouldLogClientDisconnectError(Exception ex)
		{
			return false;
		}

		protected override void DoProtocolSpecificBeginRequestLogging()
		{
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			headers[WellKnownHeader.RpcHttpProxyLogonUserName] = EncodingUtilities.EncodeToBase64(base.HttpContext.User.Identity.GetSafeName(true));
			headers[WellKnownHeader.RpcHttpProxyServerTarget] = this.rpcServerTarget;
			if (this.associationGuid != Guid.Empty)
			{
				headers[WellKnownHeader.RpcHttpProxyAssociationGuid] = this.associationGuid.ToString();
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
			base.AddProtocolSpecificHeadersToServerRequest(headers);
		}

		private static void ValidateBackendServerCacheCallback(IAsyncResult result)
		{
			RpcHttpProxyRequestHandler rpcHttpProxyRequestHandler = AsyncStateHolder.Unwrap<RpcHttpProxyRequestHandler>(result);
			if (result.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(rpcHttpProxyRequestHandler.OnValidateBackendServerCacheCompleted), result);
				return;
			}
			rpcHttpProxyRequestHandler.OnValidateBackendServerCacheCompleted(result);
		}

		private void OnValidateBackendServerCacheCompleted(object extraData)
		{
			base.CallThreadEntranceMethod(delegate
			{
				IAsyncResult asyncResult = extraData as IAsyncResult;
				HttpWebResponse httpWebResponse = null;
				Exception ex = null;
				try
				{
					this.Logger.LogCurrentTime("H-OnResponseReady");
					httpWebResponse = (HttpWebResponse)this.headRequest.EndGetResponse(asyncResult);
					this.ThrowWebExceptionForRetryOnErrorTest(httpWebResponse, new int[]
					{
						0,
						1,
						2
					});
				}
				catch (WebException ex2)
				{
					ex = ex2;
				}
				catch (HttpException ex3)
				{
					ex = ex3;
				}
				catch (IOException ex4)
				{
					ex = ex4;
				}
				catch (SocketException ex5)
				{
					ex = ex5;
				}
				finally
				{
					this.Logger.LogCurrentTime("H-EndGetResponse");
					if (httpWebResponse != null)
					{
						httpWebResponse.Close();
					}
					this.headRequest = null;
				}
				if (ex != null)
				{
					ExTraceGlobals.VerboseTracer.TraceError<Exception, int, ProxyRequestHandler.ProxyState>((long)this.GetHashCode(), "[ProxyRequestHandler::OnValidateBackendServerCacheCompleted]: Head response error: {0}; Context {1}; State {2}", ex, this.TraceContext, this.State);
				}
				WebException ex6 = ex as WebException;
				bool flag = true;
				if (ex6 != null)
				{
					this.LogWebException(ex6);
					if (this.HandleWebExceptionConnectivityError(ex6))
					{
						flag = false;
					}
				}
				if (flag && this.ShouldRecalculateBackendOnHead(ex6) && this.RecalculateTargetBackend())
				{
					flag = false;
				}
				if (flag)
				{
					this.BeginProxyRequestOrRecalculate();
				}
			});
		}

		private StreamProxy BuildResponseStream(Func<StreamProxy> outDataResponseStreamFactory, Func<StreamProxy> defaultResponseStreamFactory)
		{
			if (base.ClientRequest.HttpMethod.Equals("RPC_OUT_DATA") && base.ClientRequest.ContentLength == 76)
			{
				return outDataResponseStreamFactory();
			}
			return defaultResponseStreamFactory();
		}

		private bool ShouldRecalculateBackendOnHead(WebException webException)
		{
			bool flag;
			return webException != null && webException.Response != null && ((base.AuthBehavior.AuthState != AuthState.BackEndFullAuth && base.IsAuthenticationChallengeFromBackend(webException) && base.TryFindKerberosChallenge(webException.Response.Headers[Constants.AuthenticationHeader], out flag)) || base.HandleRoutingError((HttpWebResponse)webException.Response));
		}

		private bool ParseOutAssociationGuid(Stream stream)
		{
			byte[] array = new byte[104];
			stream.Read(array, 0, 20);
			if (array[2] == 20 && array[8] == 104 && array[18] == 6)
			{
				stream.Read(array, 20, 84);
				byte[] array2 = new byte[16];
				Array.Copy(array, 88, array2, 0, 16);
				this.associationGuid = new Guid(array2);
			}
			return true;
		}

		private int GetKeyForCasAffinity()
		{
			IIdentity identity = base.HttpContext.User.Identity;
			if (identity is WindowsIdentity || identity is ClientSecurityContextIdentity)
			{
				return identity.GetSecurityIdentifier().GetHashCode();
			}
			return identity.Name.GetHashCode();
		}

		private AnchorMailbox ResolveToDefaultAnchorMailbox(string originalRpcServerName, string reason)
		{
			string text = base.HttpContext.Items[Constants.WLIDMemberName] as string;
			if (!string.IsNullOrEmpty(text))
			{
				AnchorMailbox anchorMailbox = base.ResolveAnchorMailbox();
				if (anchorMailbox != null)
				{
					base.Logger.AppendString(HttpProxyMetadata.RoutingHint, "-" + reason);
					ExTraceGlobals.BriefTracer.TraceError((long)this.GetHashCode(), "[RpcHttpProxyRequestHandler::ResolveToDefaultAnchorMailbox]: Invalid explicit RPC server name from client: {0}; Defaulting to authenticated user {1} for routing; Context {2}; State {3}", new object[]
					{
						originalRpcServerName,
						text,
						base.TraceContext,
						base.State
					});
					this.rpcServerTarget = text;
					return anchorMailbox;
				}
			}
			throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.MailboxGuidWithDomainNotFound, string.Format("RPC server name passed in by client could not be resolved: {0}", originalRpcServerName));
		}

		internal const string PreAuthRequestHeaderValue = "true";

		private const int LookAheadBufferSize = 104;

		private static readonly string[] ProtectedHeaderNames = new string[]
		{
			WellKnownHeader.RpcHttpProxyServerTarget,
			WellKnownHeader.RpcHttpProxyAssociationGuid,
			WellKnownHeader.MailboxDatabaseGuid
		};

		private static readonly IntAppSettingsEntry RpcHttpHeadRequestTimeout = new IntAppSettingsEntry(HttpProxySettings.Prefix("RpcHttpHeadRequestTimeout"), 5000, ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry RpcHttpHeadRequestEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("RpcHttpHeadRequestEnabled"), false, ExTraceGlobals.VerboseTracer);

		private readonly RpcHttpProxyRules proxyRules;

		private HttpWebRequest headRequest;

		private string rpcServerTarget;

		private Guid associationGuid;

		private bool updateRpcServer;
	}
}
