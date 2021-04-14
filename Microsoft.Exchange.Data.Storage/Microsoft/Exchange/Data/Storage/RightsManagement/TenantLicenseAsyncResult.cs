using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TenantLicenseAsyncResult : RightsManagementAsyncResult
	{
		public TenantLicenseAsyncResult(RmsClientManagerContext context, object callerState, AsyncCallback callerCallback) : base(context, callerState, callerCallback)
		{
		}

		public XmlNode[] Rac
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

		public ServerCertificationWSManager ServerCertificationManager
		{
			get
			{
				return this.serverCertificationManager;
			}
			set
			{
				this.serverCertificationManager = value;
			}
		}

		public PublishWSManager PublishManager
		{
			get
			{
				return this.publishManager;
			}
			set
			{
				this.publishManager = value;
			}
		}

		public void ReleaseWsManagers()
		{
			if (this.ServerCertificationManager != null)
			{
				this.ServerCertificationManager.Dispose();
				this.ServerCertificationManager = null;
			}
			if (this.PublishManager != null)
			{
				this.PublishManager.Dispose();
				this.PublishManager = null;
			}
		}

		private ServerCertificationWSManager serverCertificationManager;

		private PublishWSManager publishManager;

		private XmlNode[] rac;
	}
}
