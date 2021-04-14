using System;
using System.Security;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class WacRequest
	{
		private WacRequest(WacRequestType requestType, WacFileRep fileRep, SmtpAddress mailboxSmtpAddress, string exchangeSessionId, string ewsAttachmentId, string culture, string clientVersion, string machineName, bool perfTraceRequested, string correlationID)
		{
			this.RequestType = requestType;
			this.WacFileRep = fileRep;
			this.MailboxSmtpAddress = mailboxSmtpAddress;
			this.ExchangeSessionId = exchangeSessionId;
			this.EwsAttachmentId = ewsAttachmentId;
			this.CultureName = culture;
			this.ClientVersion = clientVersion;
			this.MachineName = machineName;
			this.PerfTraceRequested = perfTraceRequested;
			this.CorrelationID = correlationID;
		}

		public string CultureName { get; private set; }

		public WacFileRep WacFileRep { get; private set; }

		public WacRequestType RequestType { get; private set; }

		public SmtpAddress MailboxSmtpAddress { get; private set; }

		public string ExchangeSessionId { get; private set; }

		public string EwsAttachmentId { get; private set; }

		public string ClientVersion { get; private set; }

		public string MachineName { get; private set; }

		public bool PerfTraceRequested { get; private set; }

		public string CorrelationID { get; private set; }

		public string CacheKey
		{
			get
			{
				string primarySmtpAddress = (string)this.MailboxSmtpAddress;
				string ewsAttachmentId = this.EwsAttachmentId;
				return CachedAttachmentInfo.GetCacheKey(primarySmtpAddress, ewsAttachmentId);
			}
		}

		public static WacRequest ParseWacRequest(string mailboxSmtpAddress, HttpRequest request)
		{
			if (request == null)
			{
				throw new OwaInvalidRequestException("Request object is null");
			}
			if (!UrlUtilities.IsWacRequest(request))
			{
				throw new OwaInvalidRequestException("Expected a WAC request, but got this instead: " + request.Url.AbsoluteUri);
			}
			WacRequestType requestType = WacRequest.GetRequestType(request);
			string text = request.QueryString["access_token"] ?? string.Empty;
			string exchangeSessionId = WacUtilities.GetExchangeSessionId(text);
			string ewsAttachmentId;
			WacRequest.ParseAccessToken(text, out ewsAttachmentId);
			string fileRepAsString = request.QueryString["owaatt"] ?? string.Empty;
			WacFileRep fileRep = WacFileRep.Parse(fileRepAsString);
			string value = request.Headers["X-WOPI-PerfTraceRequested"] ?? string.Empty;
			bool perfTraceRequested;
			if (!bool.TryParse(value, out perfTraceRequested))
			{
				perfTraceRequested = false;
			}
			return new WacRequest(requestType, fileRep, (SmtpAddress)mailboxSmtpAddress, exchangeSessionId, ewsAttachmentId, request.QueryString["ui"] ?? "en-us", request.Headers["X-WOPI-InterfaceVersion"] ?? string.Empty, request.Headers["X-WOPI-MachineName"] ?? string.Empty, perfTraceRequested, request.Headers["X-WOPI-CorrelationID"] ?? string.Empty);
		}

		public override string ToString()
		{
			return string.Format("{0} request for {1}, session started at {2}", this.RequestType, this.MailboxSmtpAddress, this.WacFileRep.CreationTime);
		}

		internal static WacRequestType GetRequestType(HttpRequest request)
		{
			if (string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
			{
				string a = HttpUtility.UrlDecode(request.Url.AbsolutePath);
				WacRequestType result;
				if (string.Equals(a, "/owa/wopi/files/@/owaatt/contents", StringComparison.OrdinalIgnoreCase) || (Globals.IsPreCheckinApp && request.Url.AbsolutePath.Contains("/contents")))
				{
					result = WacRequestType.GetFile;
				}
				else
				{
					result = WacRequestType.CheckFile;
				}
				return result;
			}
			if (string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
			{
				string text = request.Headers["X-WOPI-Override"];
				string a2;
				if ((a2 = text) != null)
				{
					WacRequestType result2;
					if (!(a2 == "PUT"))
					{
						if (!(a2 == "COBALT"))
						{
							if (!(a2 == "REFRESH_LOCK"))
							{
								if (!(a2 == "LOCK"))
								{
									if (!(a2 == "UNLOCK"))
									{
										goto IL_E1;
									}
									result2 = WacRequestType.UnLock;
								}
								else
								{
									result2 = WacRequestType.Lock;
								}
							}
							else
							{
								result2 = WacRequestType.RefreshLock;
							}
						}
						else
						{
							result2 = WacRequestType.Cobalt;
						}
					}
					else
					{
						result2 = WacRequestType.PutFile;
					}
					return result2;
				}
				IL_E1:
				throw new OwaInvalidRequestException("Unknown WOPI request: " + text);
			}
			return WacRequestType.Unknown;
		}

		internal bool IsExpired()
		{
			DateTime t = this.WacFileRep.CreationTime + WacConfiguration.Instance.AccessTokenExpirationDuration;
			return DateTime.UtcNow > t;
		}

		internal TimeSpan GetElapsedTime()
		{
			ExDateTime value = new ExDateTime(ExTimeZone.UtcTimeZone, this.WacFileRep.CreationTime);
			return ExDateTime.Now.Subtract(value);
		}

		private static void ParseAccessToken(string rawToken, out string ewsAttachmentId)
		{
			ewsAttachmentId = null;
			try
			{
				ewsAttachmentId = OAuthTokenHandler.ValidateWacCallbackToken(rawToken);
			}
			catch (SecurityException innerException)
			{
				throw new OwaInvalidRequestException("Unable to parse WAC access token.", innerException);
			}
			catch (ArgumentException innerException2)
			{
				throw new OwaInvalidRequestException("Unable to parse WAC access token.", innerException2);
			}
			catch (InvalidOperationException innerException3)
			{
				throw new OwaInvalidRequestException("Unable to parse WAC access token.", innerException3);
			}
		}
	}
}
