using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationOwaClientComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationOwaClientComponent() : base("OwaClient")
		{
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "TopNSuggestions", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "O365ShellPlus", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "XOWABirthdayCalendar", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "CalendarSearchSurvey", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "LWX", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "FlagPlus", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "PALDogfoodEnforcement", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "EnableFBL", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "ModernGroupsQuotedText", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "InstantEventCreate", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "BuildGreenLightSurveyFlight", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "XOWAShowPersonaCardOnHover", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "CalendarEventSearch", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "InstantSearch", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "Like", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "iOSSharePointRichTextEditor", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "ModernGroupsTrendingConversations", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "AttachmentsHub", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "LocationReminder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "OWADiagnostics", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "DeleteGroupConversation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "Oops", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "DisableAnimations", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "UnifiedMailboxUI", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "XOWAUnifiedForms", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "O365ShellCore", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "XOWAFrequentContacts", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "InstantPopout", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "Water", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "EmailReminders", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "ProposeNewTime", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "EnableAnimations", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "SuperMailLink", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "OwaFlow", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "OptionsLimited", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "XOWACalendar", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "SuperSwipe", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "XOWASuperCommand", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "OWADelayedBinding", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "SharePointOneDrive", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "SendLinkClickedSignalToSP", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "XOWAAwesomeReadingPane", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "OrganizationBrowser", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "O365Miniatures", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "ModernGroupsSurveyGroupA", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "OwaPublicFolderFavorites", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "MailSatisfactionSurvey", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "QuickCapture", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "OwaLinkPrefetch", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "Options", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "SuppressPushNotificationsWhenOof", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "AndroidCED", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "InstantPopout2", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "LanguageQualitySurvey", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "O365Panorama", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "ShowClientWatson", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "HelpPanel", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "InstantSearchAlpha", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "MowaInternalFeedback", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "XOWATasks", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "XOWAEmoji", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "ContextualApps", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "SuperZoom", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "AgavePerformance", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "ComposeBread1", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClient.settings.ini", "WorkingSetAgent", typeof(IFeature), false));
		}

		public VariantConfigurationSection TopNSuggestions
		{
			get
			{
				return base["TopNSuggestions"];
			}
		}

		public VariantConfigurationSection O365ShellPlus
		{
			get
			{
				return base["O365ShellPlus"];
			}
		}

		public VariantConfigurationSection XOWABirthdayCalendar
		{
			get
			{
				return base["XOWABirthdayCalendar"];
			}
		}

		public VariantConfigurationSection CalendarSearchSurvey
		{
			get
			{
				return base["CalendarSearchSurvey"];
			}
		}

		public VariantConfigurationSection LWX
		{
			get
			{
				return base["LWX"];
			}
		}

		public VariantConfigurationSection FlagPlus
		{
			get
			{
				return base["FlagPlus"];
			}
		}

		public VariantConfigurationSection PALDogfoodEnforcement
		{
			get
			{
				return base["PALDogfoodEnforcement"];
			}
		}

		public VariantConfigurationSection EnableFBL
		{
			get
			{
				return base["EnableFBL"];
			}
		}

		public VariantConfigurationSection ModernGroupsQuotedText
		{
			get
			{
				return base["ModernGroupsQuotedText"];
			}
		}

		public VariantConfigurationSection InstantEventCreate
		{
			get
			{
				return base["InstantEventCreate"];
			}
		}

		public VariantConfigurationSection BuildGreenLightSurveyFlight
		{
			get
			{
				return base["BuildGreenLightSurveyFlight"];
			}
		}

		public VariantConfigurationSection XOWAShowPersonaCardOnHover
		{
			get
			{
				return base["XOWAShowPersonaCardOnHover"];
			}
		}

		public VariantConfigurationSection CalendarEventSearch
		{
			get
			{
				return base["CalendarEventSearch"];
			}
		}

		public VariantConfigurationSection InstantSearch
		{
			get
			{
				return base["InstantSearch"];
			}
		}

		public VariantConfigurationSection Like
		{
			get
			{
				return base["Like"];
			}
		}

		public VariantConfigurationSection iOSSharePointRichTextEditor
		{
			get
			{
				return base["iOSSharePointRichTextEditor"];
			}
		}

		public VariantConfigurationSection ModernGroupsTrendingConversations
		{
			get
			{
				return base["ModernGroupsTrendingConversations"];
			}
		}

		public VariantConfigurationSection AttachmentsHub
		{
			get
			{
				return base["AttachmentsHub"];
			}
		}

		public VariantConfigurationSection LocationReminder
		{
			get
			{
				return base["LocationReminder"];
			}
		}

		public VariantConfigurationSection OWADiagnostics
		{
			get
			{
				return base["OWADiagnostics"];
			}
		}

		public VariantConfigurationSection DeleteGroupConversation
		{
			get
			{
				return base["DeleteGroupConversation"];
			}
		}

		public VariantConfigurationSection Oops
		{
			get
			{
				return base["Oops"];
			}
		}

		public VariantConfigurationSection DisableAnimations
		{
			get
			{
				return base["DisableAnimations"];
			}
		}

		public VariantConfigurationSection UnifiedMailboxUI
		{
			get
			{
				return base["UnifiedMailboxUI"];
			}
		}

		public VariantConfigurationSection XOWAUnifiedForms
		{
			get
			{
				return base["XOWAUnifiedForms"];
			}
		}

		public VariantConfigurationSection O365ShellCore
		{
			get
			{
				return base["O365ShellCore"];
			}
		}

		public VariantConfigurationSection XOWAFrequentContacts
		{
			get
			{
				return base["XOWAFrequentContacts"];
			}
		}

		public VariantConfigurationSection InstantPopout
		{
			get
			{
				return base["InstantPopout"];
			}
		}

		public VariantConfigurationSection Water
		{
			get
			{
				return base["Water"];
			}
		}

		public VariantConfigurationSection EmailReminders
		{
			get
			{
				return base["EmailReminders"];
			}
		}

		public VariantConfigurationSection ProposeNewTime
		{
			get
			{
				return base["ProposeNewTime"];
			}
		}

		public VariantConfigurationSection EnableAnimations
		{
			get
			{
				return base["EnableAnimations"];
			}
		}

		public VariantConfigurationSection SuperMailLink
		{
			get
			{
				return base["SuperMailLink"];
			}
		}

		public VariantConfigurationSection OwaFlow
		{
			get
			{
				return base["OwaFlow"];
			}
		}

		public VariantConfigurationSection OptionsLimited
		{
			get
			{
				return base["OptionsLimited"];
			}
		}

		public VariantConfigurationSection XOWACalendar
		{
			get
			{
				return base["XOWACalendar"];
			}
		}

		public VariantConfigurationSection SuperSwipe
		{
			get
			{
				return base["SuperSwipe"];
			}
		}

		public VariantConfigurationSection XOWASuperCommand
		{
			get
			{
				return base["XOWASuperCommand"];
			}
		}

		public VariantConfigurationSection OWADelayedBinding
		{
			get
			{
				return base["OWADelayedBinding"];
			}
		}

		public VariantConfigurationSection SharePointOneDrive
		{
			get
			{
				return base["SharePointOneDrive"];
			}
		}

		public VariantConfigurationSection SendLinkClickedSignalToSP
		{
			get
			{
				return base["SendLinkClickedSignalToSP"];
			}
		}

		public VariantConfigurationSection XOWAAwesomeReadingPane
		{
			get
			{
				return base["XOWAAwesomeReadingPane"];
			}
		}

		public VariantConfigurationSection OrganizationBrowser
		{
			get
			{
				return base["OrganizationBrowser"];
			}
		}

		public VariantConfigurationSection O365Miniatures
		{
			get
			{
				return base["O365Miniatures"];
			}
		}

		public VariantConfigurationSection ModernGroupsSurveyGroupA
		{
			get
			{
				return base["ModernGroupsSurveyGroupA"];
			}
		}

		public VariantConfigurationSection OwaPublicFolderFavorites
		{
			get
			{
				return base["OwaPublicFolderFavorites"];
			}
		}

		public VariantConfigurationSection MailSatisfactionSurvey
		{
			get
			{
				return base["MailSatisfactionSurvey"];
			}
		}

		public VariantConfigurationSection QuickCapture
		{
			get
			{
				return base["QuickCapture"];
			}
		}

		public VariantConfigurationSection OwaLinkPrefetch
		{
			get
			{
				return base["OwaLinkPrefetch"];
			}
		}

		public VariantConfigurationSection Options
		{
			get
			{
				return base["Options"];
			}
		}

		public VariantConfigurationSection SuppressPushNotificationsWhenOof
		{
			get
			{
				return base["SuppressPushNotificationsWhenOof"];
			}
		}

		public VariantConfigurationSection AndroidCED
		{
			get
			{
				return base["AndroidCED"];
			}
		}

		public VariantConfigurationSection InstantPopout2
		{
			get
			{
				return base["InstantPopout2"];
			}
		}

		public VariantConfigurationSection LanguageQualitySurvey
		{
			get
			{
				return base["LanguageQualitySurvey"];
			}
		}

		public VariantConfigurationSection O365Panorama
		{
			get
			{
				return base["O365Panorama"];
			}
		}

		public VariantConfigurationSection ShowClientWatson
		{
			get
			{
				return base["ShowClientWatson"];
			}
		}

		public VariantConfigurationSection HelpPanel
		{
			get
			{
				return base["HelpPanel"];
			}
		}

		public VariantConfigurationSection InstantSearchAlpha
		{
			get
			{
				return base["InstantSearchAlpha"];
			}
		}

		public VariantConfigurationSection MowaInternalFeedback
		{
			get
			{
				return base["MowaInternalFeedback"];
			}
		}

		public VariantConfigurationSection XOWATasks
		{
			get
			{
				return base["XOWATasks"];
			}
		}

		public VariantConfigurationSection XOWAEmoji
		{
			get
			{
				return base["XOWAEmoji"];
			}
		}

		public VariantConfigurationSection ContextualApps
		{
			get
			{
				return base["ContextualApps"];
			}
		}

		public VariantConfigurationSection SuperZoom
		{
			get
			{
				return base["SuperZoom"];
			}
		}

		public VariantConfigurationSection AgavePerformance
		{
			get
			{
				return base["AgavePerformance"];
			}
		}

		public VariantConfigurationSection ComposeBread1
		{
			get
			{
				return base["ComposeBread1"];
			}
		}

		public VariantConfigurationSection WorkingSetAgent
		{
			get
			{
				return base["WorkingSetAgent"];
			}
		}
	}
}
