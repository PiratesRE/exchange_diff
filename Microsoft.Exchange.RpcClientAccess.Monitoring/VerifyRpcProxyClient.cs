using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class VerifyRpcProxyClient : IVerifyRpcProxyClient
	{
		public VerifyRpcProxyClient(RpcBindingInfo bindingInfo)
		{
			this.bindingInfo = bindingInfo;
		}

		public IAsyncResult BeginVerifyRpcProxy(bool makeHangingRequest, AsyncCallback asyncCallback, object asyncState)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.bindingInfo.Uri);
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.Method = "RPC_IN_DATA";
			httpWebRequest.Accept = "application/rpc";
			httpWebRequest.UserAgent = "MSRPC";
			httpWebRequest.KeepAlive = false;
			httpWebRequest.ContentLength = (makeHangingRequest ? 2147483647L : 0L);
			httpWebRequest.Timeout = (int)this.bindingInfo.Timeout.TotalMilliseconds;
			httpWebRequest.Credentials = this.bindingInfo.Credential;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
			if (string.IsNullOrEmpty(this.bindingInfo.WebProxyServer) || this.bindingInfo.WebProxyServer == null)
			{
				httpWebRequest.Proxy = WebRequest.GetSystemWebProxy();
			}
			else if (this.bindingInfo.WebProxyServer == "<none>")
			{
				httpWebRequest.Proxy = VerifyRpcProxyClient.WebProxy.Bypass();
			}
			else
			{
				httpWebRequest.Proxy = VerifyRpcProxyClient.WebProxy.Fixed(this.bindingInfo.WebProxyServer);
			}
			httpWebRequest.CookieContainer = new CookieContainer();
			httpWebRequest.CookieContainer.Add(httpWebRequest.RequestUri, this.bindingInfo.RpcHttpCookies);
			httpWebRequest.Headers.Add(this.bindingInfo.RpcHttpHeaders);
			httpWebRequest.ConnectionGroupName = string.Format("VerifyRpcProxyClient_{0:G}_{1}", ExDateTime.UtcNow, Guid.NewGuid());
			return new VerifyRpcProxyClient.VerifyRpcProxyContext(httpWebRequest, this.bindingInfo.RpcProxyAuthentication, this.bindingInfo.IgnoreInvalidRpcProxyServerCertificateSubject, asyncCallback, asyncState).Begin();
		}

		public VerifyRpcProxyResult EndVerifyRpcProxy(IAsyncResult asyncResult)
		{
			VerifyRpcProxyClient.VerifyRpcProxyContext verifyRpcProxyContext = (VerifyRpcProxyClient.VerifyRpcProxyContext)asyncResult;
			return verifyRpcProxyContext.End(asyncResult);
		}

		private readonly RpcBindingInfo bindingInfo;

		private class VerifyRpcProxyContext : ClientCallContext<VerifyRpcProxyResult>
		{
			static VerifyRpcProxyContext()
			{
				CertificateValidationManager.RegisterCallback("VerifyRpcProxy.Strict", VerifyRpcProxyClient.VerifyRpcProxyContext.GetCertificateValidationCallback(SslPolicyErrors.None));
				CertificateValidationManager.RegisterCallback("VerifyRpcProxy.Relaxed", VerifyRpcProxyClient.VerifyRpcProxyContext.GetCertificateValidationCallback(SslPolicyErrors.RemoteCertificateNameMismatch));
			}

			public VerifyRpcProxyContext(HttpWebRequest request, HttpAuthenticationScheme rpcProxyAuthentication, bool ignoreSubjectMismatch, AsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
			{
				VerifyRpcProxyClient.VerifyRpcProxyContext <>4__this = this;
				Util.ThrowOnNullArgument(request, "request");
				this.HttpRequest = request;
				CertificateValidationManager.SetComponentId(request, ignoreSubjectMismatch ? "VerifyRpcProxy.Relaxed" : "VerifyRpcProxy.Strict");
				if (this.HttpRequest.Credentials != null)
				{
					this.HttpRequest.Credentials = new RestrictedCredentials(this.HttpRequest.Credentials, delegate(string requestedAuthType)
					{
						<>4__this.requestedRpcProxyAuthenticationTypes.Add(requestedAuthType);
						return StringComparer.OrdinalIgnoreCase.Equals(requestedAuthType, RpcHelper.HttpAuthenticationSchemeMapping.Get(rpcProxyAuthentication));
					});
				}
				else
				{
					this.HttpRequest.UseDefaultCredentials = true;
				}
				if (this.HttpRequest.ContentLength > 0L)
				{
					this.HttpRequest.SendChunked = true;
					this.requestStream = this.HttpRequest.GetRequestStream();
				}
				this.timer = new Timer(new TimerCallback(this.TimeoutCallback), this.HttpRequest, this.HttpRequest.Timeout, -1);
			}

			private HttpWebRequest HttpRequest { get; set; }

			public VerifyRpcProxyResult End(IAsyncResult asyncResult)
			{
				return base.GetResult();
			}

			protected override IAsyncResult OnBegin(AsyncCallback asyncCallback, object asyncState)
			{
				IAsyncResult result = this.HttpRequest.BeginGetResponse(asyncCallback, asyncState);
				this.completedTimer = 0;
				return result;
			}

			protected override VerifyRpcProxyResult OnEnd(IAsyncResult asyncResult)
			{
				HttpWebResponse httpWebResponse = null;
				VerifyRpcProxyResult result;
				try
				{
					httpWebResponse = (HttpWebResponse)this.HttpRequest.EndGetResponse(asyncResult);
					result = VerifyRpcProxyResult.CreateSuccessfulResult(httpWebResponse);
				}
				finally
				{
					this.TimerCleanup();
					Util.DisposeIfPresent(this.requestStream);
					Util.DisposeIfPresent(httpWebResponse);
				}
				return result;
			}

			protected override VerifyRpcProxyResult ConvertExceptionToResult(Exception exception)
			{
				WebException ex = exception as WebException;
				if (ex != null)
				{
					try
					{
						return VerifyRpcProxyResult.CreateFailureResult((HttpWebResponse)ex.Response, ex);
					}
					finally
					{
						Util.DisposeIfPresent(ex.Response);
					}
				}
				return null;
			}

			protected override VerifyRpcProxyResult PostProcessResult(VerifyRpcProxyResult result)
			{
				result.RpcProxyUrl = this.HttpRequest.RequestUri.ToString();
				result.RequestedRpcProxyAuthenticationTypes = this.requestedRpcProxyAuthenticationTypes.ToArray();
				this.HttpRequest.ServicePoint.CloseConnectionGroup(this.HttpRequest.ConnectionGroupName);
				lock (VerifyRpcProxyClient.VerifyRpcProxyContext.sslErrorsLock)
				{
					CertificateValidationError serverCertificateValidationError;
					if (VerifyRpcProxyClient.VerifyRpcProxyContext.sslErrors.TryGetValue(this.HttpRequest, out serverCertificateValidationError))
					{
						result.ServerCertificateValidationError = serverCertificateValidationError;
						VerifyRpcProxyClient.VerifyRpcProxyContext.sslErrors.Remove(this.HttpRequest);
					}
				}
				return base.PostProcessResult(result);
			}

			private static RemoteCertificateValidationCallback GetCertificateValidationCallback(SslPolicyErrors errorsToIgnore)
			{
				return delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
				{
					HttpWebRequest httpWebRequest = sender as HttpWebRequest;
					if (httpWebRequest != null)
					{
						lock (VerifyRpcProxyClient.VerifyRpcProxyContext.sslErrorsLock)
						{
							VerifyRpcProxyClient.VerifyRpcProxyContext.sslErrors[httpWebRequest] = new CertificateValidationError
							{
								Certificate = certificate,
								Chain = chain,
								SslPolicyErrors = sslPolicyErrors
							};
						}
					}
					return (sslPolicyErrors & ~errorsToIgnore) == SslPolicyErrors.None;
				};
			}

			private void TimeoutCallback(object state)
			{
				if (Interlocked.CompareExchange(ref this.completedTimer, 1, 0) == 0)
				{
					this.HttpRequest.Abort();
					this.TimerCleanup();
				}
			}

			private void TimerCleanup()
			{
				Timer timer = Interlocked.Exchange<Timer>(ref this.timer, null);
				if (timer != null)
				{
					timer.Change(-1, -1);
					timer.Dispose();
				}
			}

			private const string StrictVerificationKey = "VerifyRpcProxy.Strict";

			private const string RelaxedVerificationKey = "VerifyRpcProxy.Relaxed";

			private static readonly IDictionary<HttpWebRequest, CertificateValidationError> sslErrors = new Dictionary<HttpWebRequest, CertificateValidationError>();

			private static readonly object sslErrorsLock = new object();

			private readonly List<string> requestedRpcProxyAuthenticationTypes = new List<string>();

			private Stream requestStream;

			private Timer timer;

			private int completedTimer;
		}

		private class WebProxy : IWebProxy
		{
			private WebProxy(Uri proxy)
			{
				this.proxy = proxy;
			}

			ICredentials IWebProxy.Credentials { get; set; }

			public static IWebProxy Bypass()
			{
				return new VerifyRpcProxyClient.WebProxy(null);
			}

			public static IWebProxy Fixed(string proxyAndOptionalPort)
			{
				UriBuilder uriBuilder = new UriBuilder();
				string[] array = proxyAndOptionalPort.Split(new char[]
				{
					':'
				});
				int num;
				if (array.Length == 2 && int.TryParse(array[1], out num))
				{
					uriBuilder.Scheme = (RpcHelper.DetectShouldUseSsl((RpcProxyPort)num) ? Uri.UriSchemeHttps : Uri.UriSchemeHttp);
					uriBuilder.Host = array[0];
					uriBuilder.Port = num;
				}
				else
				{
					uriBuilder.Scheme = Uri.UriSchemeHttp;
					uriBuilder.Host = proxyAndOptionalPort;
				}
				return new VerifyRpcProxyClient.WebProxy(uriBuilder.Uri);
			}

			Uri IWebProxy.GetProxy(Uri destination)
			{
				return this.proxy;
			}

			bool IWebProxy.IsBypassed(Uri host)
			{
				return this.proxy == null;
			}

			private readonly Uri proxy;
		}
	}
}
