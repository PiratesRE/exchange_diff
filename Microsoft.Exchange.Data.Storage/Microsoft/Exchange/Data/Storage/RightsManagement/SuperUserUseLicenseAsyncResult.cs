using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SuperUserUseLicenseAsyncResult : UseLicenseAsyncResult
	{
		public SuperUserUseLicenseAsyncResult(RmsClientManagerContext context, Uri licenseUri, XmlNode[] issuanceLicense, object callerState, AsyncCallback callerCallback) : base(context, licenseUri, issuanceLicense, callerState, callerCallback)
		{
		}

		public LicenseWSManager Manager
		{
			get
			{
				return this.manager;
			}
			set
			{
				this.manager = value;
			}
		}

		public string UseLicense
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

		public override void ReleaseWebManager()
		{
			if (this.manager != null)
			{
				this.manager.Dispose();
				this.manager = null;
			}
		}

		private LicenseWSManager manager;

		private string useLicense;
	}
}
