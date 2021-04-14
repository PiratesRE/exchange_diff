using System;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.Extension
{
	[Serializable]
	public class App : XsoMailboxConfigurationObject
	{
		public App()
		{
		}

		public App(DefaultStateForUser? defaultStateForUser, string marketplaceAssetID, string marketplaceContentMarket, string providerName, Uri iconURL, string extensionId, string version, ExtensionType? type, ExtensionInstallScope? scope, RequestedCapabilities? requirements, string displayName, string description, bool enabled, string manifestXml, ADObjectId mailboxOwnerId, string eToken, EntitlementTokenData eTokenData, string appStatus)
		{
			this.DefaultStateForUser = defaultStateForUser;
			this.MarketplaceAssetID = marketplaceAssetID;
			this.MarketplaceContentMarket = marketplaceContentMarket;
			this.ProviderName = providerName;
			this.IconURL = iconURL;
			this.AppId = extensionId;
			this.AppVersion = version;
			this.Type = type;
			this.Scope = scope;
			this.Requirements = requirements;
			this.DisplayName = displayName;
			this.Description = description;
			this.Enabled = enabled;
			this.ManifestXml = manifestXml;
			base.MailboxOwnerId = mailboxOwnerId;
			this.Etoken = eToken;
			this.AppStatus = appStatus;
			if (eTokenData != null)
			{
				base.SetExchangeVersion(ExchangeObjectVersion.Current);
				this.LicensePurchaser = eTokenData.LicensePurchaser;
				this.EtokenExpirationDate = eTokenData.EtokenExpirationDate.ToString();
				this.LicenseType = new LicenseType?((LicenseType)eTokenData.LicenseType);
				this.SeatsPurchased = eTokenData.SeatsPurchased.ToString();
			}
		}

		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return App.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal bool IsDownloadOnly { get; set; }

		public string AppStatus
		{
			get
			{
				return (string)this[OWAExtensionSchema.AppStatus];
			}
			set
			{
				this[OWAExtensionSchema.AppStatus] = value;
			}
		}

		public string MarketplaceAssetID
		{
			get
			{
				return (string)this[OWAExtensionSchema.MarketplaceAssetID];
			}
			set
			{
				this[OWAExtensionSchema.MarketplaceAssetID] = value;
			}
		}

		public string MarketplaceContentMarket
		{
			get
			{
				return (string)this[OWAExtensionSchema.MarketplaceContentMarket];
			}
			set
			{
				this[OWAExtensionSchema.MarketplaceContentMarket] = value;
			}
		}

		public string ProviderName
		{
			get
			{
				return (string)this[OWAExtensionSchema.ProviderName];
			}
			set
			{
				this[OWAExtensionSchema.ProviderName] = value;
			}
		}

		public Uri IconURL
		{
			get
			{
				return (Uri)this[OWAExtensionSchema.IconURL];
			}
			set
			{
				this[OWAExtensionSchema.IconURL] = value;
			}
		}

		public string AppId
		{
			get
			{
				return (string)this[OWAExtensionSchema.AppId];
			}
			set
			{
				this[OWAExtensionSchema.AppId] = value;
			}
		}

		public ExtensionType? Type
		{
			get
			{
				return (ExtensionType?)this[OWAExtensionSchema.Type];
			}
			set
			{
				this[OWAExtensionSchema.Type] = value;
			}
		}

		public string AppVersion
		{
			get
			{
				return (string)this[OWAExtensionSchema.AppVersion];
			}
			set
			{
				this[OWAExtensionSchema.AppVersion] = value;
			}
		}

		public ExtensionInstallScope? Scope
		{
			get
			{
				return (ExtensionInstallScope?)this[OWAExtensionSchema.Scope];
			}
			set
			{
				this[OWAExtensionSchema.Scope] = value;
			}
		}

		public RequestedCapabilities? Requirements
		{
			get
			{
				return (RequestedCapabilities?)this[OWAExtensionSchema.Requirements];
			}
			set
			{
				this[OWAExtensionSchema.Requirements] = value;
			}
		}

		public DefaultStateForUser? DefaultStateForUser
		{
			get
			{
				return (DefaultStateForUser?)this[OWAExtensionSchema.DefaultStateForUser];
			}
			set
			{
				this[OWAExtensionSchema.DefaultStateForUser] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[OWAExtensionSchema.Enabled];
			}
			set
			{
				this[OWAExtensionSchema.Enabled] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[OWAExtensionSchema.DisplayName];
			}
			set
			{
				this[OWAExtensionSchema.DisplayName] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[OWAExtensionSchema.Description];
			}
			set
			{
				this[OWAExtensionSchema.Description] = value;
			}
		}

		public string ManifestXml
		{
			get
			{
				return (string)this[OWAExtensionSchema.ManifestXml];
			}
			set
			{
				this[OWAExtensionSchema.ManifestXml] = value;
			}
		}

		public string EtokenExpirationDate
		{
			get
			{
				return (string)this[OWAExtensionSchema.EtokenExpirationDate];
			}
			set
			{
				this[OWAExtensionSchema.EtokenExpirationDate] = value;
			}
		}

		public LicenseType? LicenseType
		{
			get
			{
				return (LicenseType?)this[OWAExtensionSchema.LicenseType];
			}
			set
			{
				this[OWAExtensionSchema.LicenseType] = value;
			}
		}

		public string SeatsPurchased
		{
			get
			{
				return (string)this[OWAExtensionSchema.SeatsPurchased];
			}
			set
			{
				this[OWAExtensionSchema.SeatsPurchased] = value;
			}
		}

		public string LicensePurchaser
		{
			get
			{
				return (string)this[OWAExtensionSchema.LicensePurchaser];
			}
			set
			{
				this[OWAExtensionSchema.LicensePurchaser] = value;
			}
		}

		public string Etoken
		{
			get
			{
				return (string)this[OWAExtensionSchema.Etoken];
			}
			set
			{
				this[OWAExtensionSchema.Etoken] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return (ObjectId)this[OWAExtensionSchema.Identity];
			}
		}

		internal virtual ExtensionData GetExtensionDataForInstall(IRecipientSession adRecipientSession)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.PreserveWhitespace = true;
			safeXmlDocument.LoadXml(this.ManifestXml);
			return ExtensionData.CreateForXmlStorage(this.AppId, this.MarketplaceAssetID, this.MarketplaceContentMarket, this.Type, this.Scope, this.Enabled, this.AppVersion, DisableReasonType.NotDisabled, safeXmlDocument, this.AppStatus, this.Etoken);
		}

		internal static object IdentityGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[XsoMailboxConfigurationObjectSchema.MailboxOwnerId];
			string extensionId = (string)propertyBag[OWAExtensionSchema.AppId];
			string displayName = (string)propertyBag[OWAExtensionSchema.DisplayName];
			if (adobjectId != null)
			{
				return new AppId(adobjectId, displayName, extensionId);
			}
			return null;
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			if (!string.IsNullOrEmpty(this.DisplayName))
			{
				return this.DisplayName;
			}
			return base.ToString();
		}

		private static OWAExtensionSchema schema = ObjectSchema.GetInstance<OWAExtensionSchema>();
	}
}
