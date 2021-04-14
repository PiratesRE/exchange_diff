using System;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class HttpContextExtensionMethods
	{
		internal static async Task WriteResponseDataAsync(this HttpContextBase context, ArraySegment<byte> data)
		{
			if (data.Count > 0)
			{
				await context.Response.OutputStream.WriteAsync(data.Array, data.Offset, data.Count);
			}
		}

		internal static async Task WriteResponseBuffersAsync(this HttpContextBase context, ArraySegment<byte> metaData, WorkBuffer[] responseBuffers, bool setContentLength)
		{
			if (setContentLength)
			{
				int num = metaData.Count;
				if (responseBuffers != null && responseBuffers.Length > 0)
				{
					foreach (WorkBuffer workBuffer2 in responseBuffers)
					{
						if (workBuffer2 != null)
						{
							num += workBuffer2.Count;
						}
					}
				}
				context.Response.BufferOutput = true;
				context.Response.Headers["Content-Length"] = num.ToString();
			}
			await context.WriteResponseDataAsync(metaData);
			if (responseBuffers != null && responseBuffers.Length > 0)
			{
				foreach (WorkBuffer workBuffer in responseBuffers)
				{
					if (workBuffer != null)
					{
						await context.WriteResponseDataAsync(workBuffer.ArraySegment);
					}
				}
			}
		}

		internal static bool TryGetRequestSize(this HttpContextBase context, out int requestSize)
		{
			requestSize = 0;
			string value = context.Request.Headers["Transfer-Encoding"];
			if (!string.IsNullOrEmpty(value))
			{
				return false;
			}
			string text = context.Request.Headers["Content-Length"];
			return !string.IsNullOrEmpty(text) && int.TryParse(text, out requestSize) && requestSize >= 0;
		}

		internal static async Task<WorkBuffer> ReadRequestBufferAsync(this HttpContextBase context, int maxRequestSize)
		{
			WorkBuffer requestBuffer = null;
			Stream inputStream = null;
			bool isSuccessful = false;
			WorkBuffer result;
			try
			{
				bool checkForOverflow = false;
				int requestSize;
				if (!context.TryGetRequestSize(out requestSize))
				{
					requestSize = maxRequestSize;
					checkForOverflow = true;
					requestBuffer = new WorkBuffer(8192);
				}
				else
				{
					if (requestSize > maxRequestSize)
					{
						throw ProtocolException.FromResponseCode((LID)32828, string.Format("Request was too large; maximum is {0}", maxRequestSize), ResponseCode.TooLarge, null);
					}
					requestBuffer = new WorkBuffer(requestSize);
				}
				bool useAsyncRead = true;
				ReadEntityBodyMode bodyMode = context.Request.ReadEntityBodyMode;
				switch (bodyMode)
				{
				case ReadEntityBodyMode.Classic:
				case ReadEntityBodyMode.Buffered:
				{
					inputStream = context.Request.InputStream;
					long length = inputStream.Length;
					if (length > (long)maxRequestSize)
					{
						throw ProtocolException.FromResponseCode((LID)61452, string.Format("Request was too large; maximum is {0}", maxRequestSize), ResponseCode.TooLarge, null);
					}
					inputStream.Seek(0L, SeekOrigin.Begin);
					if ((int)length > requestSize)
					{
						Util.DisposeIfPresent(requestBuffer);
						requestBuffer = new WorkBuffer((int)length);
					}
					requestSize = (int)length;
					checkForOverflow = false;
					useAsyncRead = false;
					goto IL_211;
				}
				}
				try
				{
					if (MapiHttpHandler.UseBufferedReadStream)
					{
						inputStream = context.Request.GetBufferedInputStream();
					}
					else
					{
						inputStream = context.Request.GetBufferlessInputStream();
					}
				}
				catch (HttpException innerException)
				{
					throw ProtocolException.FromHttpStatusCode((LID)40972, string.Format("Unable to get request body stream; ReadEntityBodyMode={0}.", bodyMode), string.Empty, HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError.ToString(), null, null, innerException);
				}
				IL_211:
				int sizeInBuffer = 0;
				while (sizeInBuffer < requestSize)
				{
					if (requestSize > requestBuffer.MaxSize && sizeInBuffer == requestBuffer.MaxSize)
					{
						WorkBuffer workBuffer = null;
						try
						{
							workBuffer = new WorkBuffer(Math.Min(requestSize, requestBuffer.MaxSize * 2));
							Array.Copy(requestBuffer.Array, requestBuffer.Offset, workBuffer.Array, workBuffer.Offset, sizeInBuffer);
							Util.DisposeIfPresent(requestBuffer);
							requestBuffer = workBuffer;
							workBuffer = null;
						}
						finally
						{
							Util.DisposeIfPresent(workBuffer);
						}
					}
					int sizeRead;
					if (useAsyncRead)
					{
						sizeRead = await inputStream.ReadAsync(requestBuffer.Array, requestBuffer.Offset + sizeInBuffer, requestBuffer.MaxSize - sizeInBuffer);
					}
					else
					{
						sizeRead = inputStream.Read(requestBuffer.Array, requestBuffer.Offset + sizeInBuffer, requestBuffer.MaxSize - sizeInBuffer);
					}
					sizeInBuffer += sizeRead;
					if (sizeRead == 0)
					{
						checkForOverflow = false;
						break;
					}
				}
				requestBuffer.Count = sizeInBuffer;
				if (checkForOverflow)
				{
					int sizeRead2;
					if (useAsyncRead)
					{
						sizeRead2 = await inputStream.ReadAsync(HttpContextExtensionMethods.ScratchBuffer, 0, HttpContextExtensionMethods.ScratchBuffer.Length);
					}
					else
					{
						sizeRead2 = inputStream.Read(HttpContextExtensionMethods.ScratchBuffer, 0, HttpContextExtensionMethods.ScratchBuffer.Length);
					}
					if (sizeRead2 != 0)
					{
						throw ProtocolException.FromResponseCode((LID)46592, string.Format("Request was too large; maximum is {0}", requestSize), ResponseCode.TooLarge, null);
					}
				}
				isSuccessful = true;
				result = requestBuffer;
			}
			finally
			{
				Util.DisposeIfPresent(inputStream);
				if (!isSuccessful)
				{
					Util.DisposeIfPresent(requestBuffer);
				}
			}
			return result;
		}

		internal static void SetFailureResponse(this HttpContextBase context, ResponseCode? responseCode, LID? failureLID, HttpStatusCode? httpStatusCode, string httpStatusDescription, string failureDescription)
		{
			context.SetContentType("text/html");
			if (responseCode != null)
			{
				context.SetResponseCode(responseCode.Value);
			}
			if (failureLID != null)
			{
				context.SetFailureLID(failureLID.Value);
			}
			if (httpStatusCode != null)
			{
				context.SetHttpStatusCode(httpStatusCode.Value);
			}
			if (!string.IsNullOrWhiteSpace(httpStatusDescription))
			{
				context.SetHttpStatusDescription(httpStatusDescription);
			}
			if (!string.IsNullOrWhiteSpace(failureDescription))
			{
				context.SetFailureDescription(failureDescription);
			}
		}

		internal static bool TryGetClientInfo(this HttpContextBase context, out string clientInfo)
		{
			return context.TryGetStringHeaderSingleton("X-ClientInfo", out clientInfo);
		}

		internal static void SetClientInfo(this HttpContextBase context, string clientInfo)
		{
			context.SetStringHeader("X-ClientInfo", clientInfo);
		}

		internal static bool TryGetExpirationInfo(this HttpContextBase context, out TimeSpan expiration)
		{
			expiration = TimeSpan.MinValue;
			TimeSpan timeSpan;
			if (context.TryGetTimeSpanHeaderSingleton("X-ExpirationInfo", out timeSpan))
			{
				expiration = timeSpan.Bound(Constants.MinExpirationInfo, Constants.MaxExpirationInfo);
				return true;
			}
			return false;
		}

		internal static void SetExpirationInfo(this HttpContextBase context, TimeSpan expiration)
		{
			context.SetUInt32Header("X-ExpirationInfo", (uint)expiration.TotalMilliseconds);
		}

		internal static void SetTunnelExpirationTime(this HttpContextBase context, TimeSpan expiration)
		{
			context.SetUInt32Header("X-TunnelExpirationTime", (uint)expiration.TotalMilliseconds);
		}

		internal static bool TryGetClientVersion(this HttpContextBase context, out MapiHttpVersion version)
		{
			version = null;
			string text;
			if (context.TryGetStringHeaderSingleton("X-ClientApplication", out text) && !string.IsNullOrWhiteSpace(text))
			{
				int num = text.IndexOf('/');
				if (num >= 0)
				{
					return MapiHttpVersion.TryParse(text.Substring(num + 1), out version);
				}
			}
			return false;
		}

		internal static void SetServerVersion(this HttpContextBase context)
		{
			context.SetStringHeader("X-ServerApplication", HttpContextExtensionMethods.serverApplication);
		}

		internal static bool TryGetRequestType(this HttpContextBase context, out string requestType)
		{
			return context.TryGetStringHeaderSingleton("X-RequestType", out requestType);
		}

		internal static string GetRequestType(this HttpContextBase context)
		{
			string result;
			if (!context.TryGetRequestType(out result))
			{
				throw ProtocolException.FromResponseCode((LID)47872, string.Format("Unable to find required header {0}.", "X-RequestType"), ResponseCode.MissingHeader, null);
			}
			return result;
		}

		internal static void SetRequestType(this HttpContextBase context, string requestType)
		{
			context.SetStringHeader("X-RequestType", requestType);
		}

		internal static bool TryGetRequestId(this HttpContextBase context, out string requestId)
		{
			return context.TryGetStringHeaderSingleton("X-RequestId", out requestId);
		}

		internal static string GetRequestId(this HttpContextBase context)
		{
			string result;
			if (!context.TryGetRequestId(out result))
			{
				throw ProtocolException.FromResponseCode((LID)62496, string.Format("Unable to find required header {0}.", "X-RequestId"), ResponseCode.MissingHeader, null);
			}
			return result;
		}

		internal static void SetRequestId(this HttpContextBase context, string requestId)
		{
			context.SetStringHeader("X-RequestId", requestId);
		}

		internal static bool TryGetCafeActivityId(this HttpContextBase context, out string activityId)
		{
			activityId = null;
			ActivityContextState activityContextState = ActivityContextState.DeserializeFrom(context.Request);
			if (activityContextState == null || activityContextState.ActivityId == null)
			{
				return false;
			}
			activityId = activityContextState.ActivityId.Value.ToString();
			return true;
		}

		internal static bool TryGetClientIPEndpoints(this HttpContextBase context, out string clientAddress, out string serverAddress)
		{
			clientAddress = string.Empty;
			serverAddress = string.Empty;
			IPAddress ipaddress;
			IPAddress ipaddress2;
			if (!GccUtils.GetClientIPEndpointsFromHttpRequest(context, out ipaddress, out ipaddress2, false, MapiHttpHandler.CanTrustEntireForwardedForHeader))
			{
				return false;
			}
			if (ipaddress != IPAddress.IPv6None)
			{
				clientAddress = ipaddress.ToString();
			}
			else
			{
				clientAddress = GccUtils.GetClientAddress(context);
			}
			if (ipaddress2 != IPAddress.IPv6Loopback)
			{
				serverAddress = ipaddress2.ToString();
			}
			return true;
		}

		internal static bool TryGetMailboxIdParameter(this HttpContextBase context, out string mailboxId)
		{
			mailboxId = context.Request.QueryString["MailboxId"];
			if (string.IsNullOrEmpty(mailboxId))
			{
				mailboxId = null;
				return false;
			}
			return true;
		}

		internal static bool TryGetUserPrincipalName(this HttpContextBase context, out string userPrincipalName)
		{
			IIdentity identity = context.User.Identity;
			if (identity != null)
			{
				LiveIDIdentity liveIDIdentity = identity as LiveIDIdentity;
				if (liveIDIdentity != null)
				{
					userPrincipalName = liveIDIdentity.MemberName;
				}
				else
				{
					SidBasedIdentity sidBasedIdentity = identity as SidBasedIdentity;
					if (sidBasedIdentity != null)
					{
						userPrincipalName = sidBasedIdentity.PrincipalName;
					}
					else
					{
						userPrincipalName = identity.Name;
					}
				}
				if (string.IsNullOrEmpty(userPrincipalName))
				{
					userPrincipalName = "<null>";
				}
				return true;
			}
			userPrincipalName = null;
			return false;
		}

		internal static bool TryGetUserSecurityIdentifier(this HttpContextBase context, out string userSecurityIdentifier)
		{
			userSecurityIdentifier = null;
			IIdentity identity = (context.User != null) ? context.User.Identity : null;
			if (identity == null)
			{
				return false;
			}
			bool flag = false;
			string text = null;
			bool result;
			try
			{
				LiveIDIdentity liveIDIdentity = identity as LiveIDIdentity;
				if (liveIDIdentity != null)
				{
					text = liveIDIdentity.Sid.Value.ToUpper();
				}
				else
				{
					OAuthIdentity oauthIdentity = identity as OAuthIdentity;
					if (oauthIdentity != null)
					{
						if (oauthIdentity.ActAsUser == null)
						{
							return false;
						}
						text = oauthIdentity.ActAsUser.Sid.Value.ToUpper();
					}
					else
					{
						SidBasedIdentity sidBasedIdentity = identity as SidBasedIdentity;
						if (sidBasedIdentity != null)
						{
							text = sidBasedIdentity.Sid.Value.ToUpper();
						}
						else
						{
							WindowsTokenIdentity windowsTokenIdentity = identity as WindowsTokenIdentity;
							if (windowsTokenIdentity != null)
							{
								text = windowsTokenIdentity.Sid.Value.ToUpper();
							}
							else
							{
								WindowsIdentity windowsIdentity = identity as WindowsIdentity;
								if (windowsIdentity != null)
								{
									try
									{
										text = identity.GetSecurityIdentifier().ToString().ToUpper();
									}
									catch (NotSupportedException)
									{
										return false;
									}
								}
							}
						}
					}
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					result = false;
				}
				else
				{
					flag = true;
					result = true;
				}
			}
			finally
			{
				if (flag)
				{
					userSecurityIdentifier = text;
				}
			}
			return result;
		}

		internal static bool TryGetUserAuthIdentifier(this HttpContextBase context, out string userAuthIdentifier)
		{
			userAuthIdentifier = null;
			string value = context.Items["BackEndAuthenticator"] as string;
			string text2;
			if (!string.IsNullOrWhiteSpace(value))
			{
				string text = context.Items["BEAuthIdentifier"] as string;
				if (!string.IsNullOrWhiteSpace(text))
				{
					userAuthIdentifier = text;
					return true;
				}
			}
			else if (context.TryGetUserSecurityIdentifier(out text2))
			{
				userAuthIdentifier = text2;
				return true;
			}
			return false;
		}

		internal static bool TryGetShowDebugParameter(this HttpContextBase context, out string showDebug)
		{
			showDebug = context.Request.QueryString["ShowDebug"];
			if (string.IsNullOrEmpty(showDebug))
			{
				showDebug = null;
				return false;
			}
			return true;
		}

		internal static bool TryGetContextCookie(this HttpContextBase context, out string contextCookie)
		{
			contextCookie = null;
			HttpCookieCollection cookies = context.Request.Cookies;
			if (cookies != null)
			{
				HttpCookie httpCookie = cookies.Get("MapiContext");
				if (httpCookie != null && !string.IsNullOrWhiteSpace(httpCookie.Value))
				{
					contextCookie = httpCookie.Value;
					return true;
				}
			}
			return false;
		}

		internal static string GetContextCookie(this HttpContextBase context)
		{
			string result;
			if (!context.TryGetContextCookie(out result))
			{
				throw ProtocolException.FromResponseCode((LID)37920, string.Format("Unable to find cookie {0}.", "MapiContext"), ResponseCode.MissingCookie, null);
			}
			return result;
		}

		internal static void SetContextCookie(this HttpContextBase context, string contextCookie, string vdirPath)
		{
			context.SetCookie("MapiContext", contextCookie, vdirPath);
		}

		internal static bool TryGetSequenceCookie(this HttpContextBase context, out string sequenceCookie)
		{
			sequenceCookie = null;
			HttpCookieCollection cookies = context.Request.Cookies;
			if (cookies != null)
			{
				HttpCookie httpCookie = cookies.Get("MapiSequence");
				if (httpCookie != null && !string.IsNullOrWhiteSpace(httpCookie.Value))
				{
					sequenceCookie = httpCookie.Value;
					return true;
				}
			}
			return false;
		}

		internal static string GetSequenceCookie(this HttpContextBase context)
		{
			string result;
			if (!context.TryGetSequenceCookie(out result))
			{
				throw ProtocolException.FromResponseCode((LID)42016, string.Format("Unable to find sequence cookie {0}.", "MapiSequence"), ResponseCode.MissingCookie, null);
			}
			return result;
		}

		internal static void SetSequenceCookie(this HttpContextBase context, string sequenceCookie, string vdirPath)
		{
			context.SetCookie("MapiSequence", sequenceCookie, vdirPath);
		}

		internal static void SetResponseCode(this HttpContextBase context, ResponseCode responseCode)
		{
			context.SetUInt32Header("X-ResponseCode", (uint)responseCode);
		}

		internal static void SetServiceCode(this HttpContextBase context, ServiceCode serviceCode)
		{
			context.SetUInt32Header("X-ServiceCode", (uint)serviceCode);
		}

		internal static void SetHttpStatusCode(this HttpContextBase context, HttpStatusCode httpStatusCode)
		{
			context.Response.StatusCode = (int)httpStatusCode;
		}

		internal static void SetHttpStatusDescription(this HttpContextBase context, string httpStatusDescription)
		{
			context.Response.StatusDescription = httpStatusDescription;
		}

		internal static void SetFailureDescription(this HttpContextBase context, string failureDescription)
		{
			context.SetStringHeader("X-FailureDescription", failureDescription);
		}

		internal static void SetFailureLID(this HttpContextBase context, LID failureLID)
		{
			context.SetUInt32Header("X-FailureLID", (uint)failureLID);
		}

		internal static string GetContentType(this HttpContextBase context)
		{
			string contentType = context.Request.ContentType;
			if (!string.IsNullOrEmpty(contentType))
			{
				return contentType;
			}
			return string.Empty;
		}

		internal static void SetContentType(this HttpContextBase context, string contentType)
		{
			context.Response.ContentType = contentType;
		}

		internal static void GetTimingOverrides(this HttpContextBase context, out TimeSpan requestDelay, out TimeSpan responseDelay)
		{
			requestDelay = TimeSpan.Zero;
			responseDelay = TimeSpan.Zero;
			TimeSpan timeSpan;
			if (context.TryGetTimeSpanHeaderSingleton("X-DelayRequest", out timeSpan))
			{
				requestDelay = timeSpan.Bound(TimeSpan.Zero, Constants.MaxDelayRequest);
			}
			if (context.TryGetTimeSpanHeaderSingleton("X-DelayResponse", out timeSpan))
			{
				responseDelay = timeSpan.Bound(TimeSpan.Zero, Constants.MaxDelayResponse);
			}
		}

		internal static TimeSpan GetPendingPeriod(this HttpContextBase context)
		{
			TimeSpan timeSpan;
			if (context.TryGetTimeSpanHeaderSingleton("X-PendingPeriod", out timeSpan))
			{
				return timeSpan.Bound(Constants.MinPendingPeriod, TimeSpan.MaxValue);
			}
			return Constants.DefaultPendingPeriod;
		}

		internal static void SetPendingPeriod(this HttpContextBase context, TimeSpan timeSpan)
		{
			context.Response.Headers.Set("X-PendingPeriod", timeSpan.TotalMilliseconds.ToString());
		}

		internal static bool TryGetUserIdentityInfo(this HttpContextBase context, out string userName, out string userPrincipalName, out string securityIdentifier, out string authenticationType, out string organization)
		{
			userName = null;
			userPrincipalName = null;
			securityIdentifier = null;
			authenticationType = null;
			organization = null;
			IIdentity identity = (context.User != null) ? context.User.Identity : null;
			if (identity == null)
			{
				return false;
			}
			LiveIDIdentity liveIDIdentity = identity as LiveIDIdentity;
			if (liveIDIdentity != null)
			{
				userName = liveIDIdentity.MemberName;
				userPrincipalName = liveIDIdentity.PrincipalName;
				authenticationType = liveIDIdentity.AuthenticationType;
				securityIdentifier = liveIDIdentity.Sid.Value.ToUpper();
				if (liveIDIdentity.UserOrganizationId != null && liveIDIdentity.UserOrganizationId.OrganizationalUnit != null)
				{
					organization = liveIDIdentity.UserOrganizationId.OrganizationalUnit.ToCanonicalName();
				}
				return true;
			}
			OAuthIdentity oauthIdentity = identity as OAuthIdentity;
			if (oauthIdentity != null)
			{
				if (oauthIdentity.ActAsUser == null)
				{
					return false;
				}
				userName = oauthIdentity.ActAsUser.UserPrincipalName;
				userPrincipalName = oauthIdentity.ActAsUser.UserPrincipalName;
				authenticationType = oauthIdentity.AuthenticationType;
				securityIdentifier = oauthIdentity.ActAsUser.Sid.Value.ToUpper();
				if (oauthIdentity.OrganizationId != null && oauthIdentity.OrganizationId.OrganizationalUnit != null)
				{
					organization = oauthIdentity.OrganizationId.OrganizationalUnit.ToCanonicalName();
				}
				return true;
			}
			else
			{
				SidBasedIdentity sidBasedIdentity = identity as SidBasedIdentity;
				if (sidBasedIdentity != null)
				{
					userName = sidBasedIdentity.MemberName;
					userPrincipalName = sidBasedIdentity.PrincipalName;
					authenticationType = sidBasedIdentity.AuthenticationType;
					securityIdentifier = sidBasedIdentity.Sid.Value.ToUpper();
					if (sidBasedIdentity.UserOrganizationId != null && sidBasedIdentity.UserOrganizationId.OrganizationalUnit != null)
					{
						organization = sidBasedIdentity.UserOrganizationId.OrganizationalUnit.ToCanonicalName();
					}
					return true;
				}
				WindowsTokenIdentity windowsTokenIdentity = identity as WindowsTokenIdentity;
				if (windowsTokenIdentity != null)
				{
					userName = windowsTokenIdentity.Name;
					authenticationType = windowsTokenIdentity.AuthenticationType;
					securityIdentifier = windowsTokenIdentity.Sid.Value.ToUpper();
					return true;
				}
				WindowsIdentity windowsIdentity = identity as WindowsIdentity;
				if (windowsIdentity != null)
				{
					userName = windowsIdentity.Name;
					authenticationType = windowsIdentity.AuthenticationType;
					try
					{
						securityIdentifier = identity.GetSecurityIdentifier().Value.ToUpper();
					}
					catch (NotSupportedException)
					{
					}
					return true;
				}
				return false;
			}
		}

		internal static bool TryGetSourceCafeServer(this HttpContextBase context, out string sourceCafeServer)
		{
			return context.TryGetStringHeaderSingleton("X-SourceCafeServer", out sourceCafeServer);
		}

		internal static string HtmlEncode(this HttpContextBase context, string stringValue)
		{
			if (string.IsNullOrWhiteSpace(stringValue))
			{
				return stringValue;
			}
			return AntiXssEncoder.HtmlEncode(stringValue, false);
		}

		internal static string GetOriginalHost(this HttpContextBase context)
		{
			string text;
			if (context.TryGetStringHeaderSingleton(WellKnownHeader.MsExchProxyUri, out text) && !string.IsNullOrWhiteSpace(text))
			{
				try
				{
					UriBuilder uriBuilder = new UriBuilder(text);
					string host = uriBuilder.Host;
					if (!string.IsNullOrWhiteSpace(host))
					{
						return host;
					}
				}
				catch (UriFormatException)
				{
				}
			}
			return context.Request.Url.Host;
		}

		private static bool TryGetHeaderSingletonAndConvert<T>(this HttpContextBase context, string headerName, out T convertedValue, HttpContextExtensionMethods.TryConvertHeaderDelegate<T> tryConvertDelegate)
		{
			convertedValue = default(T);
			string[] values = context.Request.Headers.GetValues(headerName);
			if (values == null || values.Length == 0)
			{
				return false;
			}
			if (values.Length != 1)
			{
				return false;
			}
			string text = values[0].Trim();
			if (string.IsNullOrWhiteSpace(text))
			{
				return false;
			}
			if (text.Contains(","))
			{
				return false;
			}
			bool result;
			try
			{
				result = tryConvertDelegate(text, out convertedValue);
			}
			catch (FormatException)
			{
				result = false;
			}
			catch (OverflowException)
			{
				result = false;
			}
			return result;
		}

		private static bool TryGetBase64HeaderSingleton(this HttpContextBase context, string headerName, out byte[] byteArrayValue)
		{
			byteArrayValue = null;
			return context.TryGetHeaderSingletonAndConvert(headerName, out byteArrayValue, delegate(string stringHeaderValue, out byte[] localByteArrayValue)
			{
				localByteArrayValue = Convert.FromBase64String(stringHeaderValue);
				return true;
			});
		}

		private static void SetBase64Header(this HttpContextBase context, string headerName, byte[] byteArrayValue)
		{
			context.Response.Headers.Set(headerName, Convert.ToBase64String(byteArrayValue));
		}

		private static bool TryGetStringHeaderSingleton(this HttpContextBase context, string headerName, out string stringValue)
		{
			stringValue = string.Empty;
			return context.TryGetHeaderSingletonAndConvert(headerName, out stringValue, delegate(string stringHeaderValue, out string localStringValue)
			{
				localStringValue = stringHeaderValue;
				return true;
			});
		}

		private static void SetStringHeader(this HttpContextBase context, string headerName, string stringValue)
		{
			if (!string.IsNullOrWhiteSpace(stringValue))
			{
				context.Response.Headers.Set(headerName, stringValue.Trim());
			}
		}

		private static bool TryGetUInt64HeaderSingleton(this HttpContextBase context, string headerName, out ulong headerValue)
		{
			headerValue = 0UL;
			return context.TryGetHeaderSingletonAndConvert(headerName, out headerValue, delegate(string stringHeaderValue, out ulong localHeaderValue)
			{
				return ulong.TryParse(stringHeaderValue, out localHeaderValue);
			});
		}

		private static void SetUInt64Header(this HttpContextBase context, string headerName, ulong headerValue)
		{
			context.Response.Headers.Set(headerName, headerValue.ToString());
		}

		private static bool TryGetUInt32HeaderSingleton(this HttpContextBase context, string headerName, out uint headerValue)
		{
			headerValue = 0U;
			return context.TryGetHeaderSingletonAndConvert(headerName, out headerValue, delegate(string stringHeaderValue, out uint localHeaderValue)
			{
				return uint.TryParse(stringHeaderValue, out localHeaderValue);
			});
		}

		private static void SetUInt32Header(this HttpContextBase context, string headerName, uint headerValue)
		{
			context.Response.Headers.Set(headerName, headerValue.ToString());
		}

		private static bool TryGetTimeSpanHeaderSingleton(this HttpContextBase context, string headerName, out TimeSpan headerValue)
		{
			headerValue = TimeSpan.Zero;
			return context.TryGetHeaderSingletonAndConvert(headerName, out headerValue, delegate(string stringHeaderValue, out TimeSpan localHeaderValue)
			{
				localHeaderValue = TimeSpan.Zero;
				uint num;
				if (!uint.TryParse(stringHeaderValue, out num))
				{
					return false;
				}
				localHeaderValue = TimeSpan.FromMilliseconds(num);
				return true;
			});
		}

		private static void SetCookie(this HttpContextBase context, string cookieName, string cookieValue, string vdirPath)
		{
			HttpCookie httpCookie = new HttpCookie(cookieName, cookieValue);
			httpCookie.HttpOnly = true;
			httpCookie.Secure = true;
			if (!string.IsNullOrEmpty(vdirPath))
			{
				httpCookie.Path = vdirPath;
			}
			context.Response.Cookies.Add(httpCookie);
		}

		private const string NoUserPrincipalNameAvailable = "<null>";

		private const string BackendAuthenticationIdentifierNameKey = "BEAuthIdentifier";

		private const string BackendAuthenticatorNameKey = "BackEndAuthenticator";

		private static readonly byte[] ScratchBuffer = new byte[16];

		private static readonly string serverApplication = string.Format("{0}/{1}", "Exchange", "15.00.1497.012");

		private delegate bool TryConvertHeaderDelegate<T>(string header, out T convertedValue);
	}
}
