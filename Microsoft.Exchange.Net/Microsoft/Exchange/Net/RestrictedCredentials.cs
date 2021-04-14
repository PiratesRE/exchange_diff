using System;
using System.Net;

namespace Microsoft.Exchange.Net
{
	internal class RestrictedCredentials : ICredentials
	{
		public RestrictedCredentials(ICredentials credentials, Func<Uri, string, bool> criteria)
		{
			if (credentials == null)
			{
				throw new ArgumentNullException("credentials");
			}
			if (criteria == null)
			{
				throw new ArgumentNullException("criteria");
			}
			this.credentials = credentials;
			this.criteria = criteria;
		}

		public RestrictedCredentials(ICredentials credentials, Func<string, bool> criteria) : this(credentials, (Uri uri, string authtype) => criteria(authtype))
		{
			if (criteria == null)
			{
				throw new ArgumentNullException("criteria");
			}
		}

		public NetworkCredential GetCredential(Uri uri, string authType)
		{
			if (this.criteria(uri, authType))
			{
				return this.credentials.GetCredential(uri, authType);
			}
			return null;
		}

		private readonly ICredentials credentials;

		private readonly Func<Uri, string, bool> criteria;
	}
}
