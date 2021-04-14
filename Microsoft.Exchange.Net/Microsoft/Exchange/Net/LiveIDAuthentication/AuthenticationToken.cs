using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.LiveIDAuthentication
{
	internal sealed class AuthenticationToken : BaseAuthenticationToken
	{
		public AuthenticationToken(string rawToken, DateTime expiry, string binarySecret, string puid)
		{
			AuthenticationToken.ThrowIfInvalid(rawToken);
			ArgumentValidator.ThrowIfNullOrEmpty("puid", puid);
			this.rawToken = rawToken;
			this.expiry = expiry;
			this.binarySecret = binarySecret;
			this.puid = puid;
		}

		public string RawToken
		{
			get
			{
				return this.rawToken;
			}
		}

		public DateTime Expiry
		{
			get
			{
				return this.expiry;
			}
		}

		public bool IsExpired
		{
			get
			{
				return this.expiry.ToUniversalTime() <= DateTime.UtcNow;
			}
		}

		public string Ticket
		{
			get
			{
				if (!this.tokenized)
				{
					this.Tokenize();
					this.tokenized = true;
				}
				return this.ticket;
			}
		}

		public string Passport
		{
			get
			{
				if (!this.tokenized)
				{
					this.Tokenize();
					this.tokenized = true;
				}
				return this.passport;
			}
		}

		public string BinarySecret
		{
			get
			{
				return this.binarySecret;
			}
		}

		public string PUID
		{
			get
			{
				return this.puid;
			}
		}

		public string UrlEncodedTicket
		{
			get
			{
				if (!this.ticketEncoded)
				{
					this.urlEncodedTicket = HttpUtility.UrlEncode(this.Ticket);
					this.ticketEncoded = true;
				}
				return this.urlEncodedTicket;
			}
		}

		public string EncodedQueryStringTicket
		{
			get
			{
				return "t=" + this.UrlEncodedTicket;
			}
		}

		public override string ToString()
		{
			return this.rawToken;
		}

		private static void ThrowIfInvalid(string rawToken)
		{
			if (rawToken == null || !rawToken.Contains("&") || !rawToken.Contains("t=") || !rawToken.Contains("p="))
			{
				throw new ArgumentException("Invalid token", "rawToken");
			}
		}

		private void Tokenize()
		{
			string[] array = this.rawToken.Split(new char[]
			{
				'&'
			});
			foreach (string text in array)
			{
				string text2 = text.Substring(0, text.IndexOf('='));
				string text3 = text.Substring(text.IndexOf('=') + 1);
				string a;
				if ((a = text2.ToLowerInvariant()) != null)
				{
					if (!(a == "t"))
					{
						if (a == "p")
						{
							this.passport = text3;
						}
					}
					else
					{
						this.ticket = text3;
					}
				}
			}
		}

		private readonly string rawToken;

		private readonly DateTime expiry;

		private string ticket;

		private string passport;

		private bool tokenized;

		private string binarySecret;

		private string puid;

		private string urlEncodedTicket;

		private bool ticketEncoded;
	}
}
