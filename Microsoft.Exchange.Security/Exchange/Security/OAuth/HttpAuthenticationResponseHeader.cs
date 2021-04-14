using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class HttpAuthenticationResponseHeader
	{
		private HttpAuthenticationResponseHeader(IEnumerable<HttpAuthenticationChallenge> challenges)
		{
			this._challenges = new List<HttpAuthenticationChallenge>(challenges);
		}

		internal static HttpAuthenticationResponseHeader Parse(string wwwAuthenticateHeader)
		{
			IEnumerable<HttpAuthenticationChallenge> challenges = HttpAuthenticationResponseHeader.EnumerateChallengesRelaxed(wwwAuthenticateHeader);
			return new HttpAuthenticationResponseHeader(challenges);
		}

		private static IEnumerable<HttpAuthenticationChallenge> EnumerateChallengesRelaxed(string header)
		{
			HttpTokenReader reader = new HttpTokenReader(header);
			HttpAuthenticationChallenge challenge = null;
			reader.SkipLinearWhiteSpace(true);
			while (!reader.EndOfContent)
			{
				if (reader.PeekChar() != ',')
				{
					break;
				}
				reader.SkipChar(',');
				reader.SkipLinearWhiteSpace(true);
			}
			while (!reader.EndOfContent)
			{
				string token = reader.ReadToken();
				reader.SkipLinearWhiteSpace(true);
				if (!reader.EndOfContent && reader.PeekChar() == '=')
				{
					reader.SkipChar('=');
					reader.SkipLinearWhiteSpace(true);
					string value = reader.ReadTokenOrQuotedString(true);
					reader.SkipLinearWhiteSpace(true);
					if (challenge != null)
					{
						challenge.AddParameter(token, value);
					}
				}
				else
				{
					if (challenge != null)
					{
						yield return challenge;
					}
					challenge = new HttpAuthenticationChallenge(token);
					if (!reader.EndOfContent && reader.PeekChar() == ',')
					{
						yield return challenge;
						challenge = null;
					}
				}
				while (!reader.EndOfContent && reader.PeekChar() == ',')
				{
					reader.SkipChar(',');
					reader.SkipLinearWhiteSpace(true);
				}
			}
			if (challenge != null)
			{
				yield return challenge;
			}
			yield break;
		}

		internal IEnumerable<HttpAuthenticationChallenge> Challenges
		{
			get
			{
				return this._challenges.AsReadOnly();
			}
		}

		internal HttpAuthenticationChallenge FindFirstChallenge(string scheme)
		{
			foreach (HttpAuthenticationChallenge httpAuthenticationChallenge in this.Challenges)
			{
				if (string.Compare(scheme, httpAuthenticationChallenge.Scheme, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return httpAuthenticationChallenge;
				}
			}
			return null;
		}

		private List<HttpAuthenticationChallenge> _challenges;
	}
}
