using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.UnifiedGroups
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FederatedDirectoryLogConfiguration : LogConfigurationBase
	{
		public static FederatedDirectoryLogConfiguration Default
		{
			get
			{
				if (FederatedDirectoryLogConfiguration.defaultInstance == null)
				{
					FederatedDirectoryLogConfiguration.defaultInstance = new FederatedDirectoryLogConfiguration();
				}
				return FederatedDirectoryLogConfiguration.defaultInstance;
			}
		}

		protected override string Component
		{
			get
			{
				return "FederatedDirectory";
			}
		}

		protected override string Type
		{
			get
			{
				return "Federated Directory Log";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ModernGroupsTracer;
			}
		}

		private static FederatedDirectoryLogConfiguration defaultInstance;
	}
}
