using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy
{
	internal class E4eProxyRequestHandler : ProxyRequestHandler
	{
		internal E4eProxyRequestHandler()
		{
		}

		protected override bool WillAddProtocolSpecificCookiesToClientResponse
		{
			get
			{
				return true;
			}
		}

		internal static bool IsE4ePayloadRequest(HttpRequest request)
		{
			return request.FilePath.EndsWith("store.ashx", StringComparison.OrdinalIgnoreCase);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			if (E4eProxyRequestHandler.IsErrorPageRequest(base.ClientRequest))
			{
				return new AnonymousAnchorMailbox(this);
			}
			if (E4eProxyRequestHandler.IsE4eInvalidStoreRequest(base.ClientRequest))
			{
				this.ThrowRedirectException(E4eProxyRequestHandler.GetErrorUrl(E4eProxyRequestHandler.E4eErrorType.InvalidStoreRequest));
			}
			bool flag = E4eProxyRequestHandler.IsE4ePostPayloadRequest(base.ClientRequest);
			this.GetSenderInfo(flag);
			string text = this.senderEmailAddress;
			if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text))
			{
				string recipientEmailAddress = base.ClientRequest.QueryString["RecipientEmailAddress"];
				if (flag)
				{
					if (E4eBackoffListCache.Instance.ShouldBackOff(text, recipientEmailAddress))
					{
						PerfCounters.HttpProxyCountersInstance.RejectedConnectionCount.Increment();
						this.ThrowRedirectException(E4eProxyRequestHandler.GetErrorUrl(E4eProxyRequestHandler.E4eErrorType.ThrottlingRestriction));
					}
					else
					{
						PerfCounters.HttpProxyCountersInstance.AcceptedConnectionCount.Increment();
					}
				}
				return new SmtpWithDomainFallbackAnchorMailbox(text, this)
				{
					UseServerCookie = true
				};
			}
			if (BEResourceRequestHandler.IsResourceRequest(base.ClientRequest.Url.LocalPath))
			{
				return new AnonymousAnchorMailbox(this);
			}
			string text2 = string.Format("The sender's email address is not valid. Email={0}, SMTP={1}", this.senderEmailAddress, text);
			base.Logger.AppendGenericError("Invalid sender email address", text2);
			throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.EndpointNotFound, text2);
		}

		protected override bool HandleBackEndCalculationException(Exception exception, AnchorMailbox anchorMailbox, string label)
		{
			HttpProxyException ex = exception as HttpProxyException;
			if (ex != null && ex.ErrorCode == HttpProxySubErrorCode.DomainNotFound)
			{
				HttpException exception2 = new HttpException(302, E4eProxyRequestHandler.GetErrorUrl(E4eProxyRequestHandler.E4eErrorType.OrgNotExisting));
				return base.HandleBackEndCalculationException(exception2, anchorMailbox, label);
			}
			return base.HandleBackEndCalculationException(exception, anchorMailbox, label);
		}

		protected override bool ShouldCopyCookieToClientResponse(Cookie cookie)
		{
			return !cookie.Name.Equals("X-E4eBudgetType", StringComparison.OrdinalIgnoreCase) && !cookie.Name.Equals("X-E4eEmailAddress", StringComparison.OrdinalIgnoreCase) && !cookie.Name.Equals("X-E4eBackOffUntilUtc", StringComparison.OrdinalIgnoreCase);
		}

		protected override void CopySupplementalCookiesToClientResponse()
		{
			this.UpdateBackoffCache();
			if (!string.IsNullOrEmpty(this.senderEmailAddress))
			{
				HttpCookie httpCookie = new HttpCookie("X-SenderEmailAddress", this.senderEmailAddress);
				httpCookie.HttpOnly = true;
				httpCookie.Secure = base.ClientRequest.IsSecureConnection;
				base.ClientResponse.Cookies.Add(httpCookie);
			}
			if (!string.IsNullOrEmpty(this.senderOrganization))
			{
				HttpCookie httpCookie2 = new HttpCookie("X-SenderOrganization", this.senderOrganization);
				httpCookie2.HttpOnly = true;
				httpCookie2.Secure = base.ClientRequest.IsSecureConnection;
				base.ClientResponse.Cookies.Add(httpCookie2);
			}
			base.CopySupplementalCookiesToClientResponse();
		}

		private static bool IsE4ePostPayloadRequest(HttpRequest request)
		{
			return request.HttpMethod.Equals(HttpMethod.Post.ToString(), StringComparison.OrdinalIgnoreCase) && E4eProxyRequestHandler.IsE4ePayloadRequest(request);
		}

		private static bool IsE4eInvalidStoreRequest(HttpRequest request)
		{
			return request.HttpMethod.Equals(HttpMethod.Get.ToString(), StringComparison.OrdinalIgnoreCase) && E4eProxyRequestHandler.IsE4ePayloadRequest(request);
		}

		private static bool IsErrorPageRequest(HttpRequest request)
		{
			if (request.HttpMethod.Equals(HttpMethod.Get.ToString(), StringComparison.OrdinalIgnoreCase) && request.FilePath.EndsWith("ErrorPage.aspx", StringComparison.OrdinalIgnoreCase))
			{
				string value = request.QueryString["code"];
				E4eProxyRequestHandler.E4eErrorType e4eErrorType;
				bool flag = Enum.TryParse<E4eProxyRequestHandler.E4eErrorType>(value, true, out e4eErrorType);
				return flag && (e4eErrorType == E4eProxyRequestHandler.E4eErrorType.OrgNotExisting || e4eErrorType == E4eProxyRequestHandler.E4eErrorType.InvalidStoreRequest);
			}
			return false;
		}

		private static string GetErrorUrl(E4eProxyRequestHandler.E4eErrorType type)
		{
			string text = string.Format("/Encryption/ErrorPage.aspx?src={0}&code={1}", 0, (int)type);
			try
			{
				string member = HttpProxyGlobals.LocalMachineFqdn.Member;
				if (!string.IsNullOrEmpty(member))
				{
					text = text + "&fe=" + HttpUtility.UrlEncode(member);
				}
			}
			catch (Exception)
			{
			}
			return text;
		}

		private void GetSenderInfo(bool isE4ePostPayloadRequest)
		{
			if (isE4ePostPayloadRequest)
			{
				this.senderEmailAddress = base.ClientRequest.QueryString["SenderEmailAddress"];
				this.senderOrganization = base.ClientRequest.QueryString["SenderOrganization"];
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "SMTP-EmailAddressFromUrlQuery");
				return;
			}
			HttpCookie httpCookie = base.ClientRequest.Cookies["X-SenderEmailAddress"];
			this.senderEmailAddress = ((httpCookie == null) ? null : httpCookie.Value);
			HttpCookie httpCookie2 = base.ClientRequest.Cookies["X-SenderOrganization"];
			this.senderOrganization = ((httpCookie2 == null) ? null : httpCookie2.Value);
			base.Logger.Set(HttpProxyMetadata.RoutingHint, "SMTP-EmailAddressFromCookie");
		}

		private void UpdateBackoffCache()
		{
			bool flag = E4eProxyRequestHandler.IsE4ePostPayloadRequest(base.ClientRequest);
			if (flag)
			{
				string serverResponseCookieValue = this.GetServerResponseCookieValue("X-E4eBudgetType");
				string serverResponseCookieValue2 = this.GetServerResponseCookieValue("X-E4eEmailAddress");
				string serverResponseCookieValue3 = this.GetServerResponseCookieValue("X-E4eBackOffUntilUtc");
				E4eBackoffListCache.Instance.UpdateCache(serverResponseCookieValue, serverResponseCookieValue2, serverResponseCookieValue3);
			}
		}

		private string GetServerResponseCookieValue(string cookieName)
		{
			Cookie cookie = base.ServerResponse.Cookies[cookieName];
			if (cookie != null)
			{
				return cookie.Value;
			}
			return string.Empty;
		}

		private void ThrowRedirectException(string redirectUrl)
		{
			if (!string.IsNullOrEmpty(this.senderEmailAddress))
			{
				HttpCookie httpCookie = new HttpCookie("X-SenderEmailAddress", this.senderEmailAddress);
				httpCookie.HttpOnly = true;
				httpCookie.Secure = base.ClientRequest.IsSecureConnection;
				base.ClientResponse.Cookies.Add(httpCookie);
			}
			if (!string.IsNullOrEmpty(this.senderOrganization))
			{
				HttpCookie httpCookie2 = new HttpCookie("X-SenderOrganization", this.senderOrganization);
				httpCookie2.HttpOnly = true;
				httpCookie2.Secure = base.ClientRequest.IsSecureConnection;
				base.ClientResponse.Cookies.Add(httpCookie2);
			}
			throw new HttpException(302, redirectUrl);
		}

		private string senderEmailAddress;

		private string senderOrganization;

		private enum E4eErrorType
		{
			GenericError,
			ConfigError,
			ThrottlingRestriction,
			OrgNotExisting,
			AuthenticationFailure,
			UploadFailure,
			ClientFailure,
			InvalidCredentials,
			InvalidEmailAddress,
			InvalidMetadata,
			InvalidMessage,
			MessageNotFound,
			MessageNotAuthorized,
			TransientFailure,
			SessionTimeout,
			ProbeRequest,
			ClientException,
			InvalidStoreRequest,
			OTPSendPerSession,
			OTPSendAcrossSession,
			OTPAttemptPerSession,
			OTPAttemptAcrossSession,
			OTPDisabled,
			OTPPasscodeExpired
		}

		private enum E4eErrorSource
		{
			Store,
			Auth,
			Backend,
			Client,
			Generic,
			OTP
		}

		private class E4eConstants
		{
			public const string ErrorPage = "ErrorPage.aspx";

			public const string ErrorCode = "code";

			public const string PostPayloadFilePath = "store.ashx";

			public const string RecipientEmailAddress = "RecipientEmailAddress";

			public const string SenderEmailAddress = "SenderEmailAddress";

			public const string SenderOrganization = "SenderOrganization";

			public const string XSenderEmailAddress = "X-SenderEmailAddress";

			public const string XSenderOrganization = "X-SenderOrganization";

			public const string XBudgetTypeCookieName = "X-E4eBudgetType";

			public const string XEmailAddressCookieName = "X-E4eEmailAddress";

			public const string XBackoffUntilUtcCookieName = "X-E4eBackOffUntilUtc";
		}
	}
}
