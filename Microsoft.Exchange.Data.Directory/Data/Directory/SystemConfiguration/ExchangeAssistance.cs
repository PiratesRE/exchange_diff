using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ExchangeAssistance : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeAssistance.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeAssistance.mostDerivedClass;
			}
		}

		[Parameter]
		public bool ExchangeHelpAppOnline
		{
			get
			{
				return (bool)this[ExchangeAssistanceSchema.ExchangeHelpAppOnline];
			}
			set
			{
				this[ExchangeAssistanceSchema.ExchangeHelpAppOnline] = value;
			}
		}

		[Parameter]
		public Uri ControlPanelHelpURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.ControlPanelHelpURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.ControlPanelHelpURL] = value;
			}
		}

		[Parameter]
		public Uri ControlPanelFeedbackURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.ControlPanelFeedbackURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.ControlPanelFeedbackURL] = value;
			}
		}

		[Parameter]
		public bool ControlPanelFeedbackEnabled
		{
			get
			{
				return (bool)this[ExchangeAssistanceSchema.ControlPanelFeedbackEnabled];
			}
			set
			{
				this[ExchangeAssistanceSchema.ControlPanelFeedbackEnabled] = value;
			}
		}

		[Parameter]
		public Uri ManagementConsoleHelpURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.ManagementConsoleHelpURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.ManagementConsoleHelpURL] = value;
			}
		}

		[Parameter]
		public Uri ManagementConsoleFeedbackURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.ManagementConsoleFeedbackURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.ManagementConsoleFeedbackURL] = value;
			}
		}

		[Parameter]
		public bool ManagementConsoleFeedbackEnabled
		{
			get
			{
				return (bool)this[ExchangeAssistanceSchema.ManagementConsoleFeedbackEnabled];
			}
			set
			{
				this[ExchangeAssistanceSchema.ManagementConsoleFeedbackEnabled] = value;
			}
		}

		[Parameter]
		public Uri OWAHelpURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.OWAHelpURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.OWAHelpURL] = value;
			}
		}

		[Parameter]
		public Uri OWAFeedbackURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.OWAFeedbackURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.OWAFeedbackURL] = value;
			}
		}

		[Parameter]
		public bool OWAFeedbackEnabled
		{
			get
			{
				return (bool)this[ExchangeAssistanceSchema.OWAFeedbackEnabled];
			}
			set
			{
				this[ExchangeAssistanceSchema.OWAFeedbackEnabled] = value;
			}
		}

		[Parameter]
		public Uri OWALightHelpURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.OWALightHelpURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.OWALightHelpURL] = value;
			}
		}

		[Parameter]
		public Uri OWALightFeedbackURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.OWALightFeedbackURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.OWALightFeedbackURL] = value;
			}
		}

		[Parameter]
		public bool OWALightFeedbackEnabled
		{
			get
			{
				return (bool)this[ExchangeAssistanceSchema.OWALightFeedbackEnabled];
			}
			set
			{
				this[ExchangeAssistanceSchema.OWALightFeedbackEnabled] = value;
			}
		}

		[Parameter]
		public Uri WindowsLiveAccountPageURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.WindowsLiveAccountPageURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.WindowsLiveAccountPageURL] = value;
			}
		}

		[Parameter]
		public bool WindowsLiveAccountURLEnabled
		{
			get
			{
				return (bool)this[ExchangeAssistanceSchema.WindowsLiveAccountURLEnabled];
			}
			set
			{
				this[ExchangeAssistanceSchema.WindowsLiveAccountURLEnabled] = value;
			}
		}

		[Parameter]
		public Uri PrivacyStatementURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.PrivacyStatementURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.PrivacyStatementURL] = value;
			}
		}

		[Parameter]
		public bool PrivacyLinkDisplayEnabled
		{
			get
			{
				return (bool)this[ExchangeAssistanceSchema.PrivacyLinkDisplayEnabled];
			}
			set
			{
				this[ExchangeAssistanceSchema.PrivacyLinkDisplayEnabled] = value;
			}
		}

		[Parameter]
		public Uri CommunityURL
		{
			get
			{
				return (Uri)this[ExchangeAssistanceSchema.CommunityURL];
			}
			set
			{
				this[ExchangeAssistanceSchema.CommunityURL] = value;
			}
		}

		[Parameter]
		public bool CommunityLinkDisplayEnabled
		{
			get
			{
				return (bool)this[ExchangeAssistanceSchema.CommunityLinkDisplayEnabled];
			}
			set
			{
				this[ExchangeAssistanceSchema.CommunityLinkDisplayEnabled] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			internal set
			{
				base.Name = value;
			}
		}

		private static ExchangeAssistanceSchema schema = ObjectSchema.GetInstance<ExchangeAssistanceSchema>();

		private static string mostDerivedClass = "msExchExchangeAssistance";
	}
}
