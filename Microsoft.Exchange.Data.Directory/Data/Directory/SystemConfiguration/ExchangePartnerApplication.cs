using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public class ExchangePartnerApplication
	{
		private ExchangePartnerApplication(string appId, string appName, string authMetadataUrl, string[] actAsPermissions)
		{
			this.appName = appName;
			this.appId = appId;
			this.authMetadataUrl = authMetadataUrl;
			this.actAsPermissions = actAsPermissions;
		}

		private ExchangePartnerApplication(string appId, string appName, string[] roles)
		{
			this.appName = appName;
			this.appId = appId;
			this.roles = roles;
		}

		public string AppName
		{
			get
			{
				return this.appName;
			}
		}

		public string AppId
		{
			get
			{
				return this.appId;
			}
		}

		public string[] Roles
		{
			get
			{
				return this.roles;
			}
		}

		public string AuthMetadataUrl
		{
			get
			{
				return this.authMetadataUrl;
			}
		}

		public string[] ActAsPermissions
		{
			get
			{
				return this.actAsPermissions;
			}
		}

		private string appName;

		private string appId;

		private string[] roles;

		private string authMetadataUrl;

		private string[] actAsPermissions;

		public static readonly ExchangePartnerApplication[] Office365CrossServiceFirstPartyAppList = new ExchangePartnerApplication[]
		{
			new ExchangePartnerApplication(WellknownPartnerApplicationIdentifiers.Lync, "Lync Online", new string[]
			{
				"UserApplication",
				"ArchiveApplication"
			}),
			new ExchangePartnerApplication(WellknownPartnerApplicationIdentifiers.SharePoint, "SharePoint Online", new string[]
			{
				"UserApplication",
				"LegalHoldApplication",
				"Mailbox Search",
				"TeamMailboxLifecycleApplication",
				"Legal Hold",
				"ExchangeCrossServiceIntegration"
			}),
			new ExchangePartnerApplication(WellknownPartnerApplicationIdentifiers.CRM, "CRM Online", new string[]
			{
				"UserApplication",
				"PartnerDelegatedTenantManagement"
			}),
			new ExchangePartnerApplication(WellknownPartnerApplicationIdentifiers.Intune, "Intune Online", new string[]
			{
				"PartnerDelegatedTenantManagement"
			}),
			new ExchangePartnerApplication(WellknownPartnerApplicationIdentifiers.OfficeServiceManager, "Office ServiceManager", new string[]
			{
				"UserApplication"
			}),
			new ExchangePartnerApplication(WellknownPartnerApplicationIdentifiers.ExchangeOnlineProtection, "Exchange Online Protection", new string[]
			{
				"PartnerDelegatedTenantManagement"
			}),
			new ExchangePartnerApplication(WellknownPartnerApplicationIdentifiers.Office365Portal, "Office 365 Portal Online", new string[]
			{
				"UserApplication",
				"Organization Configuration"
			}),
			new ExchangePartnerApplication(WellknownPartnerApplicationIdentifiers.MicrosoftErp, "Microsoft.Erp", new string[]
			{
				"UserApplication"
			})
		};
	}
}
