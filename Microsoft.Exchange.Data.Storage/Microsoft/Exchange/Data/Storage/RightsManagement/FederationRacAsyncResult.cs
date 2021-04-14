using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FederationRacAsyncResult : RightsManagementAsyncResult
	{
		public FederationRacAsyncResult(RmsClientManagerContext context, Uri licenseUri, object callerState, AsyncCallback callerCallback) : base(context, callerState, callerCallback)
		{
			this.licenseUri = licenseUri;
		}

		public Uri LicenseUri
		{
			get
			{
				return this.licenseUri;
			}
			set
			{
				this.licenseUri = value;
			}
		}

		public ServerCertificationWCFManager Manager
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

		public void ReleaseWebManager()
		{
			if (this.manager != null)
			{
				this.manager.Dispose();
				this.manager = null;
			}
		}

		private Uri licenseUri;

		private ServerCertificationWCFManager manager;

		private ExternalRMSServerInfo serverInfo;
	}
}
