using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Security.RightsManagement.SOAP.Server;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AcquireServerInfoAsyncResult : RightsManagementAsyncResult
	{
		public AcquireServerInfoAsyncResult(RmsClientManagerContext context, Uri licenseUri, object callerState, AsyncCallback callerCallback) : base(context, callerState, callerCallback)
		{
			ArgumentValidator.ThrowIfNull("licenseUri", licenseUri);
			this.licenseUri = licenseUri;
			this.serverInfo = new ExternalRMSServerInfo(licenseUri);
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

		public ServiceLocationResponse[] ServiceLocationResponses
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

		public ServerWSManager ServerWSManager
		{
			get
			{
				return this.serverWSManager;
			}
			set
			{
				this.serverWSManager = value;
			}
		}

		public HttpClient HttpClient
		{
			get
			{
				return this.httpClient;
			}
			set
			{
				this.httpClient = value;
			}
		}

		public Uri ServerLicensingMExUri
		{
			get
			{
				return this.serverLicensingMExUri;
			}
			set
			{
				this.serverLicensingMExUri = value;
			}
		}

		public Uri CertificationMExUri
		{
			get
			{
				return this.certificationMExUri;
			}
			set
			{
				this.certificationMExUri = value;
			}
		}

		public void Release()
		{
			if (this.ServerWSManager != null)
			{
				this.ServerWSManager.Dispose();
				this.ServerWSManager = null;
			}
			if (this.httpClient != null)
			{
				this.httpClient.Dispose();
				this.httpClient = null;
			}
		}

		private ServerWSManager serverWSManager;

		private Uri licenseUri;

		private ServiceLocationResponse[] responses;

		private HttpClient httpClient;

		private ExternalRMSServerInfo serverInfo;

		private Uri serverLicensingMExUri;

		private Uri certificationMExUri;
	}
}
