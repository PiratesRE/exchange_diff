using System;

namespace Microsoft.Exchange.Servicelets.RPCHTTP
{
	public sealed class VirtualDirectorySecuritySettings
	{
		public bool Anonymous
		{
			get
			{
				return this.anonymous;
			}
			set
			{
				this.anonymous = value;
			}
		}

		public bool Basic
		{
			get
			{
				return this.basic;
			}
			set
			{
				this.basic = value;
			}
		}

		public bool Windows
		{
			get
			{
				return this.windows;
			}
			set
			{
				this.windows = value;
			}
		}

		public bool ClientCertificateMapping
		{
			get
			{
				return this.clientCertificateMapping;
			}
			set
			{
				this.clientCertificateMapping = value;
			}
		}

		public bool Digest
		{
			get
			{
				return this.digest;
			}
			set
			{
				this.digest = value;
			}
		}

		public bool IisClientCertificateMapping
		{
			get
			{
				return this.iisClientCertificateMapping;
			}
			set
			{
				this.iisClientCertificateMapping = value;
			}
		}

		public bool NtlmProvider
		{
			get
			{
				return this.ntlmProvider;
			}
			set
			{
				this.windows = (this.windows || value);
				this.ntlmProvider = value;
			}
		}

		public bool NegotiateProvider
		{
			get
			{
				return this.negotiateProvider;
			}
			set
			{
				this.windows = (this.windows || value);
				this.negotiateProvider = value;
			}
		}

		private bool anonymous;

		private bool basic;

		private bool windows;

		private bool clientCertificateMapping;

		private bool digest;

		private bool iisClientCertificateMapping;

		private bool ntlmProvider;

		private bool negotiateProvider;
	}
}
