using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class ShadowServerResponseInfo
	{
		public ShadowServerResponseInfo(string shadowServer, SmtpResponse response)
		{
			this.shadowServer = shadowServer;
			this.response = response;
		}

		public string ShadowServer
		{
			get
			{
				return this.shadowServer;
			}
		}

		public SmtpResponse Response
		{
			get
			{
				return this.response;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}={1}", this.shadowServer, this.response);
		}

		private readonly string shadowServer;

		private readonly SmtpResponse response;
	}
}
