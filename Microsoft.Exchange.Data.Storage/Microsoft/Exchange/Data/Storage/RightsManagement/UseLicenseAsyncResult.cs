using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UseLicenseAsyncResult : RightsManagementAsyncResult
	{
		public UseLicenseAsyncResult(RmsClientManagerContext context, Uri licenseUri, object callerState, AsyncCallback callerCallback) : base(context, callerState, callerCallback)
		{
			this.licenseUri = licenseUri;
		}

		public UseLicenseAsyncResult(RmsClientManagerContext context, Uri licenseUri, XmlNode[] issuanceLicense, object callerState, AsyncCallback callerCallback) : base(context, callerState, callerCallback)
		{
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			RmsClientManagerUtils.ThrowOnNullOrEmptyArrayArgument("issuanceLicense", issuanceLicense);
			this.licenseUri = licenseUri;
			this.issuanceLicense = issuanceLicense;
		}

		public Uri LicenseUri
		{
			get
			{
				return this.licenseUri;
			}
		}

		public XmlNode[] IssuanceLicense
		{
			get
			{
				return this.issuanceLicense;
			}
		}

		public virtual void ReleaseWebManager()
		{
		}

		private readonly Uri licenseUri;

		private readonly XmlNode[] issuanceLicense;
	}
}
