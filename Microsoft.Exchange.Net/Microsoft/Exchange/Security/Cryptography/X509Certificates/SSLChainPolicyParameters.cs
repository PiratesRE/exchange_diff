using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class SSLChainPolicyParameters : ChainPolicyParameters
	{
		public SSLChainPolicyParameters(string name, ChainPolicyOptions options, SSLPolicyAuthorizationType type) : this(name, options, SSLPolicyAuthorizationOptions.None, type)
		{
		}

		public SSLChainPolicyParameters(string name, ChainPolicyOptions options, SSLPolicyAuthorizationOptions policy, SSLPolicyAuthorizationType type) : base(options)
		{
			this.serverName = name;
			this.type = type;
			this.options = policy;
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
			set
			{
				this.serverName = value;
			}
		}

		public SSLPolicyAuthorizationOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		public SSLPolicyAuthorizationType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		private string serverName;

		private SSLPolicyAuthorizationOptions options;

		private SSLPolicyAuthorizationType type;
	}
}
