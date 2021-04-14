using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class VerifyRpcProxyResult : CallResult
	{
		public VerifyRpcProxyResult()
		{
			this.RequestedRpcProxyAuthenticationTypes = Array<string>.Empty;
		}

		public override bool IsSuccessful
		{
			get
			{
				return this.ResponseStatusCode == HttpStatusCode.OK;
			}
		}

		public WebHeaderCollection ResponseWebHeaderCollection { get; internal set; }

		public HttpStatusCode? ResponseStatusCode { get; internal set; }

		public string RpcProxyUrl { get; internal set; }

		public CertificateValidationError ServerCertificateValidationError { get; internal set; }

		public string[] RequestedRpcProxyAuthenticationTypes { get; internal set; }

		public string ResponseStatusDescription { get; internal set; }

		public string ResponseBody { get; internal set; }

		public WebException Exception { get; internal set; }

		public override void Validate()
		{
			ExAssert.RetailAssert(this.RequestedRpcProxyAuthenticationTypes != null, "RequestedRpcProxyAuthenticationTypes shouldn't be null");
			ExAssert.RetailAssert(this.IsSuccessful == (this.Exception == null) && (!this.IsSuccessful || this.ResponseWebHeaderCollection != null), string.Format("Inconclusive data: status code={0}, exception is null={1}, headers is null={2}", this.ResponseStatusCode, this.Exception == null, this.ResponseWebHeaderCollection == null));
			base.Validate();
		}

		internal static VerifyRpcProxyResult CreateSuccessfulResult(HttpWebResponse response)
		{
			VerifyRpcProxyResult verifyRpcProxyResult = new VerifyRpcProxyResult();
			verifyRpcProxyResult.ApplyHttpWebResponse(response);
			return verifyRpcProxyResult;
		}

		internal static VerifyRpcProxyResult CreateFailureResult(HttpWebResponse response, WebException webException)
		{
			VerifyRpcProxyResult verifyRpcProxyResult = new VerifyRpcProxyResult();
			if (response != null)
			{
				verifyRpcProxyResult.ApplyHttpWebResponse(response);
			}
			verifyRpcProxyResult.ApplyWebException(webException);
			return verifyRpcProxyResult;
		}

		private void ApplyWebException(WebException webException)
		{
			this.Exception = webException;
		}

		private void ApplyHttpWebResponse(HttpWebResponse response)
		{
			Util.ThrowOnNullArgument(response, "response");
			using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
			{
				this.ResponseBody = streamReader.ReadToEnd();
			}
			this.ResponseStatusCode = new HttpStatusCode?(response.StatusCode);
			this.ResponseStatusDescription = response.StatusDescription;
			this.ResponseWebHeaderCollection = response.Headers;
		}
	}
}
