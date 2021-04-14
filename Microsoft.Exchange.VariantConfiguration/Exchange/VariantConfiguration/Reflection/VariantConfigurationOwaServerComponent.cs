using System;
using Microsoft.Exchange.Flighting;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationOwaServerComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationOwaServerComponent() : base("OwaServer")
		{
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OwaMailboxSessionCloning", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "PeopleCentricConversation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OwaSessionDataPreload", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "ShouldSkipAdfsGroupReadOnFrontend", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "XOWABirthdayAssistant", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "InlineExploreSettings", typeof(IInlineExploreSettings), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "InferenceUI", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OwaHttpHandler", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "FlightFormat", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "AndroidPremium", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "ModernConversationPrep", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OptimizedParticipantResolver", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OwaHostNameSwitch", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OwaVNext", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OWAEdgeMode", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OwaCompositeSessionData", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "ReportJunk", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OwaClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "OwaServerLogonActivityLogging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaServer.settings.ini", "InlineExploreUI", typeof(IFeature), false));
		}

		public VariantConfigurationSection OwaMailboxSessionCloning
		{
			get
			{
				return base["OwaMailboxSessionCloning"];
			}
		}

		public VariantConfigurationSection PeopleCentricConversation
		{
			get
			{
				return base["PeopleCentricConversation"];
			}
		}

		public VariantConfigurationSection OwaSessionDataPreload
		{
			get
			{
				return base["OwaSessionDataPreload"];
			}
		}

		public VariantConfigurationSection ShouldSkipAdfsGroupReadOnFrontend
		{
			get
			{
				return base["ShouldSkipAdfsGroupReadOnFrontend"];
			}
		}

		public VariantConfigurationSection XOWABirthdayAssistant
		{
			get
			{
				return base["XOWABirthdayAssistant"];
			}
		}

		public VariantConfigurationSection InlineExploreSettings
		{
			get
			{
				return base["InlineExploreSettings"];
			}
		}

		public VariantConfigurationSection InferenceUI
		{
			get
			{
				return base["InferenceUI"];
			}
		}

		public VariantConfigurationSection OwaHttpHandler
		{
			get
			{
				return base["OwaHttpHandler"];
			}
		}

		public VariantConfigurationSection FlightFormat
		{
			get
			{
				return base["FlightFormat"];
			}
		}

		public VariantConfigurationSection AndroidPremium
		{
			get
			{
				return base["AndroidPremium"];
			}
		}

		public VariantConfigurationSection ModernConversationPrep
		{
			get
			{
				return base["ModernConversationPrep"];
			}
		}

		public VariantConfigurationSection OptimizedParticipantResolver
		{
			get
			{
				return base["OptimizedParticipantResolver"];
			}
		}

		public VariantConfigurationSection OwaHostNameSwitch
		{
			get
			{
				return base["OwaHostNameSwitch"];
			}
		}

		public VariantConfigurationSection OwaVNext
		{
			get
			{
				return base["OwaVNext"];
			}
		}

		public VariantConfigurationSection OWAEdgeMode
		{
			get
			{
				return base["OWAEdgeMode"];
			}
		}

		public VariantConfigurationSection OwaCompositeSessionData
		{
			get
			{
				return base["OwaCompositeSessionData"];
			}
		}

		public VariantConfigurationSection ReportJunk
		{
			get
			{
				return base["ReportJunk"];
			}
		}

		public VariantConfigurationSection OwaClientAccessRulesEnabled
		{
			get
			{
				return base["OwaClientAccessRulesEnabled"];
			}
		}

		public VariantConfigurationSection OwaServerLogonActivityLogging
		{
			get
			{
				return base["OwaServerLogonActivityLogging"];
			}
		}

		public VariantConfigurationSection InlineExploreUI
		{
			get
			{
				return base["InlineExploreUI"];
			}
		}
	}
}
