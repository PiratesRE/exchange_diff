using System;
using System.Net;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class IncorrectUrlRequestException : AirSyncPermanentException
	{
		internal IncorrectUrlRequestException(HttpStatusCode httpStatusCode, string headerNameIn, string headerValueIn) : base(httpStatusCode, StatusCode.None, null, false)
		{
			this.headerName = headerNameIn;
			this.headerValue = headerValueIn;
		}

		internal string HeaderName
		{
			get
			{
				return this.headerName;
			}
			set
			{
				this.headerName = value;
			}
		}

		internal string HeaderValue
		{
			get
			{
				return this.headerValue;
			}
			set
			{
				this.headerValue = value;
			}
		}

		private string headerName;

		private string headerValue;
	}
}
