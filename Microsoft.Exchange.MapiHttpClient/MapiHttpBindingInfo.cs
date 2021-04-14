using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MapiHttpBindingInfo
	{
		public MapiHttpBindingInfo(string serverFqdn, ICredentials credentials) : this(serverFqdn, Constants.DefaultHttpsPort, true, credentials, true, null)
		{
		}

		public MapiHttpBindingInfo(string serverFqdn, ICredentials credentials, string mailboxId) : this(serverFqdn, Constants.DefaultHttpsPort, true, credentials, true, mailboxId)
		{
		}

		public MapiHttpBindingInfo(string serverFqdn, int port, bool useSsl, ICredentials credentials, bool ignoreCertificateErrors, string mailboxId)
		{
			this.ServerFqdn = serverFqdn;
			this.Port = port;
			this.UseSsl = useSsl;
			this.Credentials = credentials;
			this.IgnoreCertificateErrors = ignoreCertificateErrors;
			this.MailboxId = mailboxId;
		}

		public TimeSpan? Expiration
		{
			get
			{
				return this.expiration;
			}
			set
			{
				this.expiration = value;
			}
		}

		public bool KeepContextsAlive
		{
			get
			{
				return this.keepContextsAlive;
			}
			set
			{
				this.keepContextsAlive = value;
			}
		}

		public WebHeaderCollection AdditionalHttpHeaders
		{
			get
			{
				return this.additionalHttpHeaders;
			}
			set
			{
				this.additionalHttpHeaders = value;
			}
		}

		internal bool ShouldTrimVdirPath { get; set; }

		internal string BuildRequestPath(ref string originalVdirPath)
		{
			string text = originalVdirPath;
			if (this.ShouldTrimVdirPath)
			{
				text = text.TrimEnd(new char[]
				{
					'/'
				});
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "/";
			}
			else if (text[0] != '/')
			{
				text = "/" + text;
			}
			string text2 = string.IsNullOrEmpty(this.MailboxId) ? "useMailboxOfAuthenticatedUser=true" : string.Format("mailboxId={0}", this.MailboxId);
			string result;
			if ((this.UseSsl && this.Port == Constants.DefaultHttpsPort) || (!this.UseSsl && this.Port == Constants.DefaultHttpPort))
			{
				result = string.Format("{0}{1}{2}{3}?{4}", new object[]
				{
					this.UseSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp,
					Uri.SchemeDelimiter,
					this.ServerFqdn,
					text,
					text2
				});
			}
			else
			{
				result = string.Format("{0}{1}{2}:{3}{4}?{5}", new object[]
				{
					this.UseSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp,
					Uri.SchemeDelimiter,
					this.ServerFqdn,
					this.Port,
					text,
					text2
				});
			}
			originalVdirPath = text;
			return result;
		}

		public readonly string ServerFqdn;

		public readonly int Port;

		public readonly bool UseSsl;

		public readonly ICredentials Credentials;

		public readonly bool IgnoreCertificateErrors;

		public readonly string MailboxId;

		private TimeSpan? expiration = null;

		private bool keepContextsAlive = true;

		private WebHeaderCollection additionalHttpHeaders = new WebHeaderCollection();
	}
}
