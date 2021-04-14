using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	internal abstract class ManageDelegationClient
	{
		protected abstract CustomSoapHttpClientProtocol Client { get; }

		public abstract void AddUri(string applicationId, string uri);

		public abstract void RemoveUri(string applicationId, string uri);

		public abstract void ReserveDomain(string applicationId, string domain, string programId);

		public abstract void ReleaseDomain(string applicationId, string domain);

		protected ManageDelegationClient(string serviceEndpoint, string certificateThumbprint, WriteVerboseDelegate writeVerbose)
		{
			this.serviceEndpoint = serviceEndpoint;
			this.certificateThumbprint = certificateThumbprint;
			this.writeVerbose = writeVerbose;
		}

		protected string ServiceEndpoint
		{
			get
			{
				return this.serviceEndpoint;
			}
		}

		protected X509Certificate2 Certificate
		{
			get
			{
				if (this.certificateThumbprint == null)
				{
					throw new ArgumentNullException("certificate");
				}
				if (this.certificate == null)
				{
					this.certificate = FederationCertificate.LoadCertificateWithPrivateKey(this.certificateThumbprint, this.WriteVerbose);
				}
				return this.certificate;
			}
		}

		protected WriteVerboseDelegate WriteVerbose
		{
			get
			{
				return this.writeVerbose;
			}
		}

		protected void ExecuteAndHandleError(string description, ManageDelegationClient.WebMethodDelegate webMethod)
		{
			LocalizedException ex = null;
			try
			{
				this.ExecuteAndRetry(description, webMethod);
			}
			catch (SoapException exception)
			{
				ex = this.GetLiveDomainServicesAccessExceptionToThrow(exception);
			}
			catch (WebException exception2)
			{
				ex = ManageDelegationClient.GetCommunicationException(exception2);
			}
			catch (IOException exception3)
			{
				ex = ManageDelegationClient.GetCommunicationException(exception3);
			}
			catch (SocketException exception4)
			{
				ex = ManageDelegationClient.GetCommunicationException(exception4);
			}
			catch (InvalidOperationException exception5)
			{
				ex = ManageDelegationClient.GetCommunicationException(exception5);
			}
			if (ex != null)
			{
				this.WriteVerbose(Strings.LiveDomainServicesRequestFailed(ManageDelegationClient.GetExceptionDetails(ex)));
				throw ex;
			}
		}

		protected static bool InvalidCertificateHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None || SslConfiguration.AllowExternalUntrustedCerts;
		}

		private void ExecuteAndRetry(string description, ManageDelegationClient.WebMethodDelegate webMethod)
		{
			DateTime t = DateTime.UtcNow + ManageDelegationClient.ErrorRetryLimit;
			string validDirectUrl = this.ServiceEndpoint;
			this.Client.AllowAutoRedirect = false;
			WebProxy webProxy = LiveConfiguration.GetWebProxy(this.WriteVerbose);
			if (webProxy != null)
			{
				this.Client.Proxy = webProxy;
			}
			int num = 0;
			for (;;)
			{
				this.Client.Url = validDirectUrl;
				try
				{
					this.WriteVerbose(Strings.CallingDomainServicesEndPoint(description, validDirectUrl));
					webMethod();
				}
				catch (WebException ex)
				{
					if (DateTime.UtcNow > t)
					{
						throw;
					}
					HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
					if (httpWebResponse == null)
					{
						throw;
					}
					HttpStatusCode statusCode = httpWebResponse.StatusCode;
					if (statusCode != HttpStatusCode.Found)
					{
						if (statusCode != HttpStatusCode.Forbidden)
						{
							throw;
						}
						Thread.Sleep(ManageDelegationClient.ErrorRetryInterval);
					}
					else
					{
						num++;
						if (num > 3)
						{
							throw;
						}
						validDirectUrl = this.GetValidDirectUrl(httpWebResponse);
						if (validDirectUrl == null)
						{
							throw;
						}
					}
					continue;
				}
				break;
			}
		}

		private string GetValidDirectUrl(HttpWebResponse webResponse)
		{
			string text = webResponse.Headers[HttpResponseHeader.Location];
			if (!Uri.IsWellFormedUriString(text, UriKind.Absolute))
			{
				return null;
			}
			Uri uri = new Uri(text, UriKind.Absolute);
			if (uri.Scheme != Uri.UriSchemeHttps)
			{
				return null;
			}
			return text;
		}

		private static LocalizedException GetCommunicationException(Exception exception)
		{
			return new LiveDomainServicesException(Strings.ErrorLiveDomainServicesAccess(exception.Message), exception);
		}

		private static string GetExceptionDetails(Exception exception)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder(10000);
			while (exception != null)
			{
				stringBuilder.AppendFormat("[{0}]: {1}", num.ToString(), exception.GetType().FullName);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(exception.Message);
				ManageDelegationClient.DetailException(stringBuilder, exception as SoapException);
				ManageDelegationClient.DetailException(stringBuilder, exception as WebException);
				ManageDelegationClient.DetailException(stringBuilder, exception as SocketException);
				if (exception.StackTrace != null)
				{
					stringBuilder.AppendLine(exception.StackTrace);
				}
				exception = exception.InnerException;
				num++;
				if (exception != null)
				{
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		private static void DetailException(StringBuilder detail, SoapException soapException)
		{
			if (soapException == null)
			{
				return;
			}
			if (soapException.Code != null)
			{
				detail.AppendFormat("Code: {0}", soapException.Code);
				detail.AppendLine();
			}
			if (soapException.SubCode != null)
			{
				int num = 0;
				detail.Append("SubCode: ");
				SoapFaultSubCode subCode = soapException.SubCode;
				while (subCode != null)
				{
					if (subCode.Code != null)
					{
						detail.Append(subCode.Code.ToString());
					}
					subCode = subCode.SubCode;
					num++;
					if (num > 10)
					{
						break;
					}
					if (subCode != null)
					{
						detail.Append(", ");
					}
				}
				detail.AppendLine();
			}
			if (soapException.Detail != null)
			{
				detail.AppendFormat("Detail: {0}", soapException.Detail.OuterXml);
				detail.AppendLine();
			}
		}

		private static void DetailException(StringBuilder detail, WebException webException)
		{
			if (webException == null)
			{
				return;
			}
			detail.AppendFormat("Status: {0} ({1})", webException.Status, (int)webException.Status);
			detail.AppendLine();
			if (webException.Response != null)
			{
				if (webException.Response.Headers != null && webException.Response.Headers.Count > 0)
				{
					detail.AppendLine("Response headers:");
					foreach (string text in webException.Response.Headers.AllKeys)
					{
						detail.AppendFormat("  {0}: {1}", text, webException.Response.Headers[text]);
						detail.AppendLine();
					}
				}
				Stream responseStream = webException.Response.GetResponseStream();
				if (responseStream != null)
				{
					using (responseStream)
					{
						if (responseStream.CanRead)
						{
							if (responseStream.CanSeek)
							{
								try
								{
									responseStream.Seek(0L, SeekOrigin.Begin);
								}
								catch (IOException)
								{
								}
								catch (NotSupportedException)
								{
								}
								catch (ObjectDisposedException)
								{
								}
							}
							using (StreamReader streamReader = new StreamReader(responseStream))
							{
								try
								{
									string value = streamReader.ReadToEnd();
									detail.AppendLine("Response body:");
									detail.AppendLine(value);
								}
								catch (IOException)
								{
								}
							}
						}
					}
				}
			}
			HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
			if (httpWebResponse != null)
			{
				detail.AppendFormat("StatusCode: {0}", httpWebResponse.StatusCode);
				detail.AppendLine();
				if (httpWebResponse.StatusDescription != null)
				{
					detail.AppendFormat("StatusDescription: {0}", httpWebResponse.StatusDescription);
					detail.AppendLine();
				}
				if (httpWebResponse.Server != null)
				{
					detail.AppendFormat("Server: {0}", httpWebResponse.Server);
					detail.AppendLine();
				}
			}
		}

		private static void DetailException(StringBuilder detail, SocketException socketException)
		{
			if (socketException == null)
			{
				return;
			}
			detail.AppendFormat("SocketErrorCode: {0}", socketException.SocketErrorCode);
			detail.AppendLine();
			detail.AppendFormat("ErrorCode: {0}", socketException.ErrorCode);
			detail.AppendLine();
		}

		private LocalizedException GetLiveDomainServicesAccessExceptionToThrow(SoapException exception)
		{
			int num;
			if (exception.Detail != null && exception.Detail.HasChildNodes && int.TryParse(exception.Detail.FirstChild.InnerText.Trim(), out num))
			{
				DomainError domainError = (DomainError)num;
				return new LiveDomainServicesException(domainError, this.GetMessageFromDomainError(domainError, exception), exception);
			}
			return new LiveDomainServicesException(Strings.ErrorLiveDomainServicesAccess(exception.Message), exception);
		}

		private LocalizedString GetMessageFromDomainError(DomainError domainError, SoapException exception)
		{
			if (domainError <= DomainError.ProofOfOwnershipNotValid)
			{
				switch (domainError)
				{
				case DomainError.InvalidPartner:
					return Strings.ErrorLiveDomainInaccessibleEpr(exception.Message, this.ServiceEndpoint);
				case DomainError.InvalidPartnerCert:
				case DomainError.InvalidManagementCertificate:
					return Strings.ErrorCertificateNotValid(this.Certificate.Subject, this.Certificate.Thumbprint, exception.Message);
				case DomainError.PartnerNotAuthorized:
				case DomainError.MemberNotAuthorized:
				case DomainError.MemberNotAuthenticated:
					return Strings.ErrorLiveIdAuthentication(exception.Message);
				default:
					switch (domainError)
					{
					case DomainError.InvalidDomainName:
					case DomainError.BlockedDomainName:
					case DomainError.InvalidDomainConfigId:
						return Strings.ErrorLiveIdDomainNameInvalid(exception.Message);
					case DomainError.DomainNotReserved:
						return Strings.ErrorsDomainNotReserved;
					case DomainError.DomainUnavailable:
						return Strings.ErrorLiveDomainReservationError(exception.Message);
					case DomainError.DomainPendingChanges:
					case DomainError.DomainSuspended:
					case DomainError.DomainPendingConfiguration:
						return Strings.ErrorLiveIdDomainTemporarilyUnavailable(exception.Message);
					case DomainError.NotPermittedForDomain:
						return Strings.ErrorLiveDomainUriNotUnique(exception.Message);
					case DomainError.ProofOfOwnershipNotValid:
						return Strings.ErrorProofOfOwnershipNotValid;
					}
					break;
				}
			}
			else
			{
				switch (domainError)
				{
				case DomainError.MemberNameInvalid:
				case DomainError.MemberNameBlocked:
				case DomainError.MemberNameUnavailable:
				case DomainError.MemberNameBlank:
				case DomainError.MemberNameIncludesInvalidChars:
				case DomainError.MemberNameIncludesDots:
				case DomainError.MemberNameInUse:
				case DomainError.ManagedMemberExists:
				case DomainError.ManagedMemberNotExists:
				case DomainError.UnmanagedMemberExists:
				case DomainError.UnmanagedMemberNotExists:
				case DomainError.MaxMembershipLimit:
				case DomainError.PasswordBlank:
				case DomainError.PasswordTooShort:
				case DomainError.PasswordTooLong:
				case DomainError.PasswordIncludesMemberName:
				case DomainError.PasswordIncludesInvalidChars:
				case DomainError.PasswordInvalid:
				case DomainError.InvalidNetId:
				case DomainError.InvalidOffer:
					break;
				default:
					switch (domainError)
					{
					case DomainError.InternalError:
					case DomainError.InvalidParameter:
					case DomainError.ExchangeError:
					case DomainError.SubscriptionServicesError:
					case DomainError.TestForcedError:
						break;
					case DomainError.PassportError:
						return this.GetPassportErrorMessage(exception);
					case DomainError.ServiceDown:
						return Strings.ErrorLiveIdServiceDown(exception.Message);
					default:
						if (domainError == DomainError.NYI)
						{
							return Strings.ErrorLiveDomainServicesUnexpectedResult(Strings.ErrorDomainServicesNotYetImplemented);
						}
						break;
					}
					break;
				}
			}
			return Strings.ErrorLiveDomainServicesUnexpectedResult(domainError.ToString() + " " + exception.Message);
		}

		private LocalizedString GetPassportErrorMessage(SoapException se)
		{
			int num = 0;
			try
			{
				string text = se.Detail.LastChild.InnerText.Trim().ToUpperInvariant();
				if (text.StartsWith("0X"))
				{
					text = text.Substring(2);
				}
				int.TryParse(text, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out num);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			int num2 = num;
			if (num2 == -2147198366)
			{
				return Strings.ErrorLiveDomainUriNotUnique(se.Message);
			}
			return Strings.ErrorLiveIdError(se.Message);
		}

		private const int MaximumRedirects = 3;

		private static readonly TimeSpan ErrorRetryLimit = TimeSpan.FromSeconds(30.0);

		private static readonly TimeSpan ErrorRetryInterval = TimeSpan.FromSeconds(5.0);

		private readonly string serviceEndpoint;

		private readonly string certificateThumbprint;

		private X509Certificate2 certificate;

		private WriteVerboseDelegate writeVerbose;

		protected delegate void WebMethodDelegate();
	}
}
