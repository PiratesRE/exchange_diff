using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class ProxyUri
	{
		private ProxyUri(string uriString)
		{
			this.uriString = uriString;
		}

		private ProxyUri(string uriString, Uri failbackUrl)
		{
			this.uriString = uriString;
			this.failbackUrl = failbackUrl;
		}

		public static ProxyUri Create(string uriString)
		{
			return ProxyUri.Create(uriString, null);
		}

		public static ProxyUri Create(string uriString, Uri failbackUrl)
		{
			if (uriString == null)
			{
				throw new ArgumentNullException("uriString");
			}
			return new ProxyUri(uriString, failbackUrl);
		}

		internal bool Parse()
		{
			this.isParsed = true;
			this.uri = ProxyUtilities.TryCreateCasUri(this.uriString, this.needVdirValidation);
			if (this.uri == null)
			{
				return false;
			}
			this.isValid = true;
			return true;
		}

		public bool NeedVdirValidation
		{
			get
			{
				return this.needVdirValidation;
			}
			set
			{
				this.needVdirValidation = value;
			}
		}

		public bool IsParsed
		{
			get
			{
				return this.isParsed;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		public Uri FailbackUrl
		{
			get
			{
				return this.failbackUrl;
			}
		}

		internal ProxyPingResult ProxyPingResult
		{
			get
			{
				return this.proxyPingResult;
			}
			set
			{
				this.proxyPingResult = value;
			}
		}

		public override string ToString()
		{
			return this.uriString;
		}

		private string uriString;

		private Uri uri;

		private Uri failbackUrl;

		private ProxyPingResult proxyPingResult;

		private bool isParsed;

		private bool isValid;

		private bool needVdirValidation = true;
	}
}
