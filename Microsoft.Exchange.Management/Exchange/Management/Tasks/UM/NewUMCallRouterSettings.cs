using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("New", "UMCallRouterSettings")]
	public sealed class NewUMCallRouterSettings : NewFixedNameSystemConfigurationObjectTask<SIPFEServerConfiguration>
	{
		protected override ObjectId RootId
		{
			get
			{
				ServerIdParameter serverIdParameter = ServerIdParameter.Parse(Environment.MachineName);
				this.localServer = (Server)base.GetDataObject<Server>(serverIdParameter, base.DataSession as IConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
				return SIPFEServerConfiguration.GetRootId(this.localServer);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			SIPFEServerConfiguration sipfeserverConfiguration = (SIPFEServerConfiguration)base.PrepareDataObject();
			ADObjectId adobjectId = this.RootId as ADObjectId;
			sipfeserverConfiguration.SetId(adobjectId.GetChildId(sipfeserverConfiguration.Name));
			sipfeserverConfiguration.VersionNumber = this.localServer.VersionNumber;
			sipfeserverConfiguration.NetworkAddress = this.localServer.NetworkAddress;
			sipfeserverConfiguration.CurrentServerRole = this.localServer.CurrentServerRole;
			return sipfeserverConfiguration;
		}

		protected override void InternalProcessRecord()
		{
			if (base.DataSession.Read<SIPFEServerConfiguration>(this.DataObject.Id) == null)
			{
				base.InternalProcessRecord();
			}
		}

		internal SIPFEServerConfiguration DefaultConfiguration
		{
			get
			{
				return this.DataObject;
			}
		}

		private Server localServer;
	}
}
