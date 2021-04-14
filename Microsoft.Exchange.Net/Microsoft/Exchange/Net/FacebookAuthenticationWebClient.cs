using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FacebookAuthenticationWebClient : IFacebookAuthenticationWebClient
	{
		public AuthenticateApplicationResponse AuthenticateApplication(Uri accessTokenEndpoint, TimeSpan requestTimeout)
		{
			WebRequest webRequest = WebRequest.Create(accessTokenEndpoint);
			webRequest.UseDefaultCredentials = false;
			webRequest.Timeout = (int)requestTimeout.TotalMilliseconds;
			AuthenticateApplicationResponse result;
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse())
			{
				if (httpWebResponse.ContentLength > 4096L)
				{
					throw new FacebookAuthenticationException(NetServerException.AppAuthenticationResponseTooLarge(httpWebResponse.ContentLength));
				}
				if (!"UTF-8".Equals(httpWebResponse.CharacterSet, StringComparison.OrdinalIgnoreCase))
				{
					throw new FacebookAuthenticationException(NetServerException.UnexpectedCharSetInAppAuthenticationResponse(httpWebResponse.CharacterSet));
				}
				using (Stream responseStream = httpWebResponse.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
					{
						result = new AuthenticateApplicationResponse
						{
							Code = httpWebResponse.StatusCode,
							Body = streamReader.ReadToEnd()
						};
					}
				}
			}
			return result;
		}

		private const int MaxResponseLength = 4096;

		private const string AuthenticateResponseCharSet = "UTF-8";
	}
}
