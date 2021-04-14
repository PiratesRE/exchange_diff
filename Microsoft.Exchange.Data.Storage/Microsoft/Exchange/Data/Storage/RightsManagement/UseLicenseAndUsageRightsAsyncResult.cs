using System;
using System.Security.Principal;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class UseLicenseAndUsageRightsAsyncResult : RightsManagementAsyncResult
	{
		internal UseLicenseAndUsageRightsAsyncResult(RmsClientManagerContext context, Uri licensingUri, string publishLicense, XmlNode[] publishLicenseAsXmlNodes, string userIdentity, SecurityIdentifier userSid, bool isSuperUser, object callerState, AsyncCallback callerCallback) : base(context, callerState, callerCallback)
		{
			ArgumentValidator.ThrowIfNull("licensingUri", licensingUri);
			ArgumentValidator.ThrowIfNullOrEmpty("publishLicense", publishLicense);
			ArgumentValidator.ThrowIfNull("publishLicenseAsXmlNodes", publishLicenseAsXmlNodes);
			ArgumentValidator.ThrowIfNull("userIdentity", userIdentity);
			ArgumentValidator.ThrowIfNull("userSid", userSid);
			this.licensingUri = licensingUri;
			this.publishLicense = publishLicense;
			this.publishLicenseAsXmlNodes = publishLicenseAsXmlNodes;
			this.userIdentity = userIdentity;
			this.userSid = userSid;
			this.isSuperUser = isSuperUser;
		}

		internal string PublishLicense
		{
			get
			{
				return this.publishLicense;
			}
		}

		internal XmlNode[] PublishLicenseAsXmlNodes
		{
			get
			{
				return this.publishLicenseAsXmlNodes;
			}
		}

		internal Uri LicensingUri
		{
			get
			{
				return this.licensingUri;
			}
		}

		internal string UserIdentity
		{
			get
			{
				return this.userIdentity;
			}
		}

		internal bool IsSuperUser
		{
			get
			{
				return this.isSuperUser;
			}
		}

		internal SecurityIdentifier UserSid
		{
			get
			{
				return this.userSid;
			}
		}

		internal string UseLicense
		{
			get
			{
				return this.useLicense;
			}
			set
			{
				this.useLicense = value;
			}
		}

		internal ContentRight UsageRights
		{
			get
			{
				return this.usageRights;
			}
			set
			{
				this.usageRights = value;
			}
		}

		internal ExDateTime ExpiryTime
		{
			get
			{
				return this.expiryTime;
			}
			set
			{
				this.expiryTime = value;
			}
		}

		internal byte[] DRMPropsSignature
		{
			get
			{
				return this.drmPropsSignature;
			}
			set
			{
				this.drmPropsSignature = value;
			}
		}

		private readonly string publishLicense;

		private readonly XmlNode[] publishLicenseAsXmlNodes;

		private readonly Uri licensingUri;

		private readonly string userIdentity;

		private readonly SecurityIdentifier userSid;

		private readonly bool isSuperUser;

		private string useLicense;

		private ContentRight usageRights;

		private ExDateTime expiryTime;

		private byte[] drmPropsSignature;
	}
}
