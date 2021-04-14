using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class FBASingleSignOnFilterChain : RequestFilterChain
	{
		internal override bool FilterRequest(object source, EventArgs e, RequestEventType eventType)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			HttpRequest request = context.Request;
			Configuration configuration = OwaConfigurationManager.Configuration;
			if (!Utilities.IsOwaUrl(request.Url, OwaUrl.AuthPost, true) || configuration == null)
			{
				return false;
			}
			if (configuration.FormsAuthenticationEnabled)
			{
				if (eventType == RequestEventType.BeginRequest)
				{
					this.SimulateBasicAuthenticationHeader(context);
				}
				else
				{
					if (eventType != RequestEventType.PostAuthorizeRequest)
					{
						throw new OwaInvalidOperationException("FBASingleSignOn was unexpectedly called");
					}
					HttpResponse response = context.Response;
					WindowsIdentity windowsIdentity = context.User.Identity as WindowsIdentity;
					if (windowsIdentity != null && !windowsIdentity.IsAnonymous)
					{
						this.TransferToAuthDll(httpApplication);
						return true;
					}
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_WebConfigAuthenticationIncorrect, string.Empty, new object[]
					{
						HttpRuntime.AppDomainAppPath
					});
					Utilities.RewritePathToError(OwaContext.Current, LocalizedStrings.GetNonEncoded(-1556449487));
				}
			}
			else
			{
				Utilities.EndResponse(context, HttpStatusCode.BadRequest);
			}
			return false;
		}

		private static void SetOwaContextDestinationUrl(OwaContext owaContext, string redirectUrl)
		{
			owaContext.SecondCasUri = ProxyUri.Create(redirectUrl);
			owaContext.SecondCasUri.NeedVdirValidation = false;
			owaContext.SecondCasUri.Parse();
		}

		private void TransferToAuthDll(HttpApplication httpApplication)
		{
			httpApplication.Server.TransferRequest(OwaUrl.AuthDll.ImplicitUrl, true);
		}

		private void SimulateBasicAuthenticationHeader(HttpContext httpContext)
		{
			HttpRequest request = httpContext.Request;
			HttpResponse response = httpContext.Response;
			SecureHtmlFormReader secureHtmlFormReader = new SecureHtmlFormReader(request);
			secureHtmlFormReader.AddSensitiveInputName("password");
			SecureNameValueCollection secureNameValueCollection;
			if (!secureHtmlFormReader.TryReadSecureFormData(out secureNameValueCollection))
			{
				if (secureNameValueCollection != null)
				{
					secureNameValueCollection.Dispose();
				}
				response.Redirect(OwaUrl.ApplicationRoot.ImplicitUrl);
			}
			OwaContext.Current.FormNameValueCollection = secureNameValueCollection;
			bool flag = true;
			string text;
			if (!secureNameValueCollection.TryGetUnsecureValue("username", out text))
			{
				flag = false;
			}
			SecureString secureString;
			if (!secureNameValueCollection.TryGetSecureValue("password", out secureString))
			{
				flag = false;
			}
			if (!flag)
			{
				Utilities.EndResponse(httpContext, HttpStatusCode.BadRequest);
			}
			string value;
			if (secureNameValueCollection.TryGetUnsecureValue("destination", out value))
			{
				response.AppendHeader("X-OWA-Destination", value);
			}
			HttpCookie httpCookie = request.Cookies["PBack"];
			if (httpCookie == null || httpCookie.Value != "0")
			{
				Utilities.EndResponse(httpContext, HttpStatusCode.Unauthorized);
			}
			text += ":";
			Encoding @default = Encoding.Default;
			int maxByteCount = @default.GetMaxByteCount(text.Length + secureString.Length);
			using (SecureArray<byte> secureArray = new SecureArray<byte>(maxByteCount))
			{
				int num = @default.GetBytes(text, 0, text.Length, secureArray.ArrayValue, 0);
				using (SecureArray<char> secureArray2 = secureString.ConvertToSecureCharArray())
				{
					num += @default.GetBytes(secureArray2.ArrayValue, 0, secureArray2.Length(), secureArray.ArrayValue, num);
					request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(secureArray.ArrayValue, 0, num);
				}
			}
		}

		private ExchangePrincipal GetExchangePrincipalFromWindowsIdentity(WindowsIdentity windowsIdentity, string smtpAddress)
		{
			OwaIdentity owaIdentity = OwaWindowsIdentity.CreateFromWindowsIdentity(windowsIdentity);
			OwaIdentity owaIdentity2 = null;
			bool flag = false;
			bool flag2 = false;
			ExchangePrincipal result;
			try
			{
				ExchangePrincipal exchangePrincipal = null;
				if (smtpAddress != null)
				{
					owaIdentity2 = OwaIdentity.CreateOwaIdentityFromSmtpAddress(owaIdentity, smtpAddress, out exchangePrincipal, out flag, out flag2);
				}
				else
				{
					owaIdentity2 = owaIdentity;
				}
				if (flag && owaIdentity2.IsPartial)
				{
					OwaMiniRecipientIdentity owaMiniRecipientIdentity = owaIdentity2 as OwaMiniRecipientIdentity;
					try
					{
						owaMiniRecipientIdentity.UpgradePartialIdentity();
					}
					catch (DataValidationException ex)
					{
						PropertyValidationError propertyValidationError = ex.Error as PropertyValidationError;
						if (propertyValidationError == null || propertyValidationError.PropertyDefinition != MiniRecipientSchema.Languages)
						{
							return null;
						}
						OWAMiniRecipient owaminiRecipient = owaIdentity2.FixCorruptOWAMiniRecipientCultureEntry();
						if (owaminiRecipient != null)
						{
							owaIdentity2.Dispose();
							owaIdentity2 = OwaMiniRecipientIdentity.CreateFromOWAMiniRecipient(owaminiRecipient);
						}
					}
				}
				if (owaIdentity == owaIdentity2 && exchangePrincipal != null)
				{
					result = exchangePrincipal;
				}
				else
				{
					try
					{
						result = owaIdentity2.InternalCreateExchangePrincipal();
					}
					catch (UserHasNoMailboxException)
					{
						result = null;
					}
				}
			}
			finally
			{
				if (owaIdentity != owaIdentity2 && owaIdentity2 != null)
				{
					owaIdentity2.Dispose();
					owaIdentity2 = null;
				}
				if (owaIdentity != null)
				{
					owaIdentity.Dispose();
					owaIdentity = null;
				}
			}
			return result;
		}

		private void RedirectUsingSSOFBA(SecureNameValueCollection collection, Uri redirectUrl, HttpResponse response, int majorCasVersion)
		{
			response.StatusCode = 200;
			response.Status = "200 - OK";
			response.BufferOutput = false;
			response.CacheControl = "no-cache";
			response.Cache.SetNoStore();
			HttpCookie httpCookie = new HttpCookie("PBack");
			httpCookie.Value = "1";
			response.Cookies.Add(httpCookie);
			SecureHttpBuffer secureHttpBuffer = new SecureHttpBuffer(1000, response);
			this.CreateHtmlForSsoFba(secureHttpBuffer, collection, redirectUrl, majorCasVersion);
			secureHttpBuffer.FlushBuffer();
			response.End();
		}

		private void CreateHtmlForSsoFba(SecureHttpBuffer buffer, SecureNameValueCollection collection, Uri redirectUrl, int majorCasVersion)
		{
			SanitizedHtmlString noScriptHtml = Utilities.GetNoScriptHtml();
			string value = "<html><noscript>";
			string value2 = "</noscript><head><title>Continue</title><script type='text/javascript'>function OnBack(){}function DoSubmit(){var subt=false;if(!subt){subt=true;document.logonForm.submit();}}</script></head><body onload='javascript:DoSubmit();'>";
			string value3 = "</body></html>";
			buffer.CopyAtCurrentPosition(value);
			buffer.CopyAtCurrentPosition(noScriptHtml.ToString());
			buffer.CopyAtCurrentPosition(value2);
			this.CreateFormHtmlForSsoFba(buffer, collection, redirectUrl, majorCasVersion);
			buffer.CopyAtCurrentPosition(value3);
		}

		private void CreateFormHtmlForSsoFba(SecureHttpBuffer buffer, SecureNameValueCollection collection, Uri redirectUrl, int majorCasVersion)
		{
			string value = "<form name='logonForm' id='logonForm' action='";
			string value2 = "' method='post' target='_top'>";
			string value3 = "</form>";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(redirectUrl.Scheme);
			stringBuilder.Append(Uri.SchemeDelimiter);
			stringBuilder.Append(redirectUrl.Authority);
			if (majorCasVersion < (int)ExchangeObjectVersion.Exchange2007.ExchangeBuild.Major)
			{
				stringBuilder.Append("/exchweb/bin/auth/owaauth.dll");
			}
			else
			{
				stringBuilder.Append(OwaUrl.AuthDll.ImplicitUrl);
			}
			buffer.CopyAtCurrentPosition(value);
			buffer.CopyAtCurrentPosition(stringBuilder.ToString());
			buffer.CopyAtCurrentPosition(value2);
			this.CreateInputHtmlCollection(collection, buffer, redirectUrl, majorCasVersion);
			buffer.CopyAtCurrentPosition(value3);
		}

		private void CreateInputHtmlCollection(SecureNameValueCollection collection, SecureHttpBuffer buffer, Uri redirectUrl, int majorCasVersion)
		{
			string value = "<input type='hidden' name='";
			string value2 = "' value='";
			string value3 = "'>";
			foreach (string text in collection)
			{
				buffer.CopyAtCurrentPosition(value);
				buffer.CopyAtCurrentPosition(text);
				buffer.CopyAtCurrentPosition(value2);
				if (text == "password")
				{
					SecureString secureValue;
					collection.TryGetSecureValue(text, out secureValue);
					buffer.CopyAtCurrentPosition(secureValue);
				}
				else if (text == "destination")
				{
					string text2;
					collection.TryGetUnsecureValue(text, out text2);
					Uri uri;
					if (!Uri.TryCreate(text2, UriKind.Absolute, out uri))
					{
						throw new OwaInvalidRequestException("destination value is not valid");
					}
					StringBuilder stringBuilder = new StringBuilder();
					if (majorCasVersion < (int)ExchangeObjectVersion.Exchange2007.ExchangeBuild.Major)
					{
						stringBuilder.Append(redirectUrl);
					}
					else
					{
						stringBuilder.Append(redirectUrl.Scheme);
						stringBuilder.Append(Uri.SchemeDelimiter);
						stringBuilder.Append(redirectUrl.Authority);
						if (Utilities.IsOwaUrl(uri, OwaUrl.AuthPost, true))
						{
							stringBuilder.Append(OwaUrl.ApplicationRoot.ImplicitUrl);
						}
						else
						{
							stringBuilder.Append(uri.PathAndQuery);
						}
					}
					buffer.CopyAtCurrentPosition(stringBuilder.ToString());
				}
				else
				{
					string text2;
					collection.TryGetUnsecureValue(text, out text2);
					buffer.CopyAtCurrentPosition(text2);
				}
				buffer.CopyAtCurrentPosition(value3);
			}
		}

		private const string PasswordFormName = "password";

		private const string DestinationFormName = "destination";

		private const string UsenameFormName = "username";

		private const string BasicAuthHeader = "Authorization";

		private const string DestinationHeader = "X-OWA-Destination";

		private const string BasicHeaderValue = "Basic ";

		private const string LegacyAuthDllPath = "/exchweb/bin/auth/owaauth.dll";

		private const string LegacyVdir = "/exchange";

		private const string PostBackFFCookieName = "PBack";
	}
}
