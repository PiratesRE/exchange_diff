using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal abstract class ChainPolicyParameters
	{
		protected ChainPolicyParameters(ChainPolicyOptions options)
		{
			this.flags = options;
		}

		public ChainPolicyOptions Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		private ChainPolicyOptions flags;
	}
}
