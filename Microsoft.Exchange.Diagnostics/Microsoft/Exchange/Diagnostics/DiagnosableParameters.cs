using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct DiagnosableParameters
	{
		private DiagnosableParameters(string argument, bool allowRestrictedData, bool unlimited, string userIdentity)
		{
			this.argument = argument;
			this.allowRestrictedData = allowRestrictedData;
			this.unlimited = unlimited;
			this.userIdentity = userIdentity;
		}

		public string Argument
		{
			get
			{
				return this.argument;
			}
		}

		public bool AllowRestrictedData
		{
			get
			{
				return this.allowRestrictedData;
			}
		}

		public bool Unlimited
		{
			get
			{
				return this.unlimited;
			}
		}

		public string UserIdentity
		{
			get
			{
				return this.userIdentity;
			}
		}

		public static DiagnosableParameters Create(string argument, bool allowRestrictedData, bool unlimited, string userIdentity)
		{
			return new DiagnosableParameters(argument ?? string.Empty, allowRestrictedData, unlimited, userIdentity ?? string.Empty);
		}

		private readonly string argument;

		private readonly bool allowRestrictedData;

		private readonly bool unlimited;

		private readonly string userIdentity;
	}
}
