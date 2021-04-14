using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Security.OAuth
{
	public sealed class HttpAuthenticationChallenge
	{
		internal HttpAuthenticationChallenge(string scheme)
		{
			if (string.IsNullOrWhiteSpace(scheme))
			{
				throw new ArgumentNullException("scheme");
			}
			this._scheme = scheme;
			this._parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		public string Scheme
		{
			get
			{
				return this._scheme;
			}
		}

		public string Realm
		{
			get
			{
				return this.GetParameter(Constants.ChallengeTokens.Realm);
			}
		}

		public string TrustedIssuers
		{
			get
			{
				return this.GetParameter(Constants.ChallengeTokens.TrustedIssuers);
			}
		}

		public string ClientId
		{
			get
			{
				return this.GetParameter(Constants.ChallengeTokens.ClientId);
			}
		}

		public string GetParameter(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			string result = null;
			this._parameters.TryGetValue(name, out result);
			return result;
		}

		internal void AddParameter(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!this._parameters.ContainsKey(name))
			{
				this._parameters.Add(name, value);
			}
		}

		public override bool Equals(object obj)
		{
			HttpAuthenticationChallenge httpAuthenticationChallenge = obj as HttpAuthenticationChallenge;
			return httpAuthenticationChallenge != null && (this.ClientId == httpAuthenticationChallenge.ClientId && this.Realm == httpAuthenticationChallenge.Realm) && this.TrustedIssuers == httpAuthenticationChallenge.TrustedIssuers;
		}

		public override int GetHashCode()
		{
			int num = this.ClientId.GetHashCode();
			if (!string.IsNullOrEmpty(this.Realm))
			{
				num ^= this.Realm.GetHashCode();
			}
			if (!string.IsNullOrEmpty(this.TrustedIssuers))
			{
				num ^= this.TrustedIssuers.GetHashCode();
			}
			return num;
		}

		private readonly string _scheme;

		private Dictionary<string, string> _parameters;
	}
}
