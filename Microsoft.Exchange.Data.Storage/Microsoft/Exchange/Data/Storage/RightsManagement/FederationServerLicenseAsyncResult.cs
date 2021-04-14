using System;
using System.Xml;
using Microsoft.com.IPC.WSService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FederationServerLicenseAsyncResult : UseLicenseAsyncResult
	{
		public FederationServerLicenseAsyncResult(RmsClientManagerContext context, Uri licenseUri, XmlNode[] issuanceLicense, LicenseIdentity[] identities, LicenseResponse[] responses, object callerState, AsyncCallback callerCallback) : base(context, licenseUri, issuanceLicense, callerState, callerCallback)
		{
			RmsClientManagerUtils.ThrowOnNullOrEmptyArrayArgument("identities", identities);
			this.identities = identities;
			this.responses = responses;
		}

		public ServerLicenseWCFManager Manager
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

		public ExternalRMSServerInfo ServerInfo
		{
			get
			{
				return this.serverInfo;
			}
			set
			{
				this.serverInfo = value;
			}
		}

		public XrmlCertificateChain Rac
		{
			get
			{
				return this.rac;
			}
			set
			{
				this.rac = value;
			}
		}

		public LicenseIdentity[] Identities
		{
			get
			{
				return this.identities;
			}
		}

		public LicenseResponse[] Responses
		{
			get
			{
				return this.responses;
			}
			set
			{
				this.responses = value;
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

		private readonly LicenseIdentity[] identities;

		private ServerLicenseWCFManager manager;

		private ExternalRMSServerInfo serverInfo;

		private XrmlCertificateChain rac;

		private LicenseResponse[] responses;
	}
}
