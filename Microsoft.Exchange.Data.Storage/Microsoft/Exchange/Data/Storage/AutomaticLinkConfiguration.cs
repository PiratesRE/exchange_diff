using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AutomaticLinkConfiguration
	{
		public static bool IsOWAEnabled
		{
			get
			{
				return AutomaticLinkConfiguration.IsComponentEnabled(AutomaticLinkConfiguration.Components.OWA);
			}
		}

		public static bool IsMOMTEnabled
		{
			get
			{
				return AutomaticLinkConfiguration.IsComponentEnabled(AutomaticLinkConfiguration.Components.MOMT);
			}
		}

		public static bool IsBulkEnabled
		{
			get
			{
				return AutomaticLinkConfiguration.IsComponentEnabled(AutomaticLinkConfiguration.Components.Bulk);
			}
		}

		internal static void EnableAll()
		{
			AutomaticLinkConfiguration.enableAutomaticLinking = new AutomaticLinkConfiguration.Components?(AutomaticLinkConfiguration.Components.All);
		}

		internal static void EnableDefault()
		{
			AutomaticLinkConfiguration.enableAutomaticLinking = new AutomaticLinkConfiguration.Components?(AutomaticLinkConfiguration.Components.Default);
		}

		internal static void Enable(AutomaticLinkConfiguration.Components components)
		{
			AutomaticLinkConfiguration.enableAutomaticLinking |= components;
		}

		internal static void Disable(AutomaticLinkConfiguration.Components components)
		{
			AutomaticLinkConfiguration.enableAutomaticLinking &= ~components;
		}

		private static bool IsComponentEnabled(AutomaticLinkConfiguration.Components component)
		{
			if (AutomaticLinkConfiguration.enableAutomaticLinking == null)
			{
				AutomaticLinkConfiguration.enableAutomaticLinking = new AutomaticLinkConfiguration.Components?((AutomaticLinkConfiguration.Components)Util.GetRegistryValueOrDefault(AutomaticLinkConfiguration.RegistryKeysLocation, AutomaticLinkConfiguration.EnableAutomaticLinkingValueName, 5, AutomaticLinkConfiguration.Tracer));
			}
			return (AutomaticLinkConfiguration.enableAutomaticLinking & component) != (AutomaticLinkConfiguration.Components)0;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		internal static readonly string RegistryKeysLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\People";

		internal static readonly string EnableAutomaticLinkingValueName = "EnableAutomaticLinking";

		private static AutomaticLinkConfiguration.Components? enableAutomaticLinking;

		internal enum Components
		{
			OWA = 1,
			MOMT,
			Bulk = 4,
			Default,
			All = 7
		}
	}
}
