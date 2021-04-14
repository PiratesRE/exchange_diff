using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationOwaClientServerComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationOwaClientServerComponent() : base("OwaClientServer")
		{
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "FolderBasedClutter", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "FlightsView", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "O365Header", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "OwaVersioning", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "AutoSubscribeNewGroupMembers", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "XOWAHolidayCalendars", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "AttachmentsFilePicker", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "GroupRegionalConfiguration", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "DocCollab", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "OwaPublicFolders", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "O365ParityHeader", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "ModernMail", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "SmimeConversation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "ActiveViewConvergence", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "ModernGroupsWorkingSet", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "InlinePreview", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "PeopleCentricTriage", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "ChangeLayout", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "SuperStart", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "SuperNormal", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "FasterPhoto", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "NotificationBroker", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "ModernGroupsNewArchitecture", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "SuperSort", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "AutoSubscribeSetByDefault", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "SafeHtml", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "Weather", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "ModernGroups", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "ModernAttachments", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "OWAPLTPerf", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaClientServer.settings.ini", "O365G2Header", typeof(IFeature), false));
		}

		public VariantConfigurationSection FolderBasedClutter
		{
			get
			{
				return base["FolderBasedClutter"];
			}
		}

		public VariantConfigurationSection FlightsView
		{
			get
			{
				return base["FlightsView"];
			}
		}

		public VariantConfigurationSection O365Header
		{
			get
			{
				return base["O365Header"];
			}
		}

		public VariantConfigurationSection OwaVersioning
		{
			get
			{
				return base["OwaVersioning"];
			}
		}

		public VariantConfigurationSection AutoSubscribeNewGroupMembers
		{
			get
			{
				return base["AutoSubscribeNewGroupMembers"];
			}
		}

		public VariantConfigurationSection XOWAHolidayCalendars
		{
			get
			{
				return base["XOWAHolidayCalendars"];
			}
		}

		public VariantConfigurationSection AttachmentsFilePicker
		{
			get
			{
				return base["AttachmentsFilePicker"];
			}
		}

		public VariantConfigurationSection GroupRegionalConfiguration
		{
			get
			{
				return base["GroupRegionalConfiguration"];
			}
		}

		public VariantConfigurationSection DocCollab
		{
			get
			{
				return base["DocCollab"];
			}
		}

		public VariantConfigurationSection OwaPublicFolders
		{
			get
			{
				return base["OwaPublicFolders"];
			}
		}

		public VariantConfigurationSection O365ParityHeader
		{
			get
			{
				return base["O365ParityHeader"];
			}
		}

		public VariantConfigurationSection ModernMail
		{
			get
			{
				return base["ModernMail"];
			}
		}

		public VariantConfigurationSection SmimeConversation
		{
			get
			{
				return base["SmimeConversation"];
			}
		}

		public VariantConfigurationSection ActiveViewConvergence
		{
			get
			{
				return base["ActiveViewConvergence"];
			}
		}

		public VariantConfigurationSection ModernGroupsWorkingSet
		{
			get
			{
				return base["ModernGroupsWorkingSet"];
			}
		}

		public VariantConfigurationSection InlinePreview
		{
			get
			{
				return base["InlinePreview"];
			}
		}

		public VariantConfigurationSection PeopleCentricTriage
		{
			get
			{
				return base["PeopleCentricTriage"];
			}
		}

		public VariantConfigurationSection ChangeLayout
		{
			get
			{
				return base["ChangeLayout"];
			}
		}

		public VariantConfigurationSection SuperStart
		{
			get
			{
				return base["SuperStart"];
			}
		}

		public VariantConfigurationSection SuperNormal
		{
			get
			{
				return base["SuperNormal"];
			}
		}

		public VariantConfigurationSection FasterPhoto
		{
			get
			{
				return base["FasterPhoto"];
			}
		}

		public VariantConfigurationSection NotificationBroker
		{
			get
			{
				return base["NotificationBroker"];
			}
		}

		public VariantConfigurationSection ModernGroupsNewArchitecture
		{
			get
			{
				return base["ModernGroupsNewArchitecture"];
			}
		}

		public VariantConfigurationSection SuperSort
		{
			get
			{
				return base["SuperSort"];
			}
		}

		public VariantConfigurationSection AutoSubscribeSetByDefault
		{
			get
			{
				return base["AutoSubscribeSetByDefault"];
			}
		}

		public VariantConfigurationSection SafeHtml
		{
			get
			{
				return base["SafeHtml"];
			}
		}

		public VariantConfigurationSection Weather
		{
			get
			{
				return base["Weather"];
			}
		}

		public VariantConfigurationSection ModernGroups
		{
			get
			{
				return base["ModernGroups"];
			}
		}

		public VariantConfigurationSection ModernAttachments
		{
			get
			{
				return base["ModernAttachments"];
			}
		}

		public VariantConfigurationSection OWAPLTPerf
		{
			get
			{
				return base["OWAPLTPerf"];
			}
		}

		public VariantConfigurationSection O365G2Header
		{
			get
			{
				return base["O365G2Header"];
			}
		}
	}
}
