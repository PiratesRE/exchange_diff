using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.PushNotifications
{
	public class ProxyCmdletBaseClass : SetSystemConfigurationObjectTask<PushNotificationAppIdParameter, PushNotificationApp>
	{
		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
				return configurationSession.GetOrgContainerId().GetDescendantId(PushNotificationApp.RdnContainer);
			}
		}

		public override PushNotificationAppIdParameter Identity
		{
			get
			{
				return ProxyCmdletBaseClass.ProxyIdentity;
			}
		}

		private protected string CurrentProxyStatus { protected get; private set; }

		protected override IConfigurable PrepareDataObject()
		{
			PushNotificationApp pushNotificationApp = (PushNotificationApp)base.PrepareDataObject();
			this.CurrentProxyStatus = ((pushNotificationApp != null && pushNotificationApp.Enabled != null) ? pushNotificationApp.Enabled.Value.ToString() : false.ToString());
			return pushNotificationApp;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			this.WriteResult(this.DataObject);
		}

		protected void WriteResult(IConfigurable dataObject)
		{
			PushNotificationApp pushNotificationApp = dataObject as PushNotificationApp;
			base.WriteObject(new PushNotificationProxyPresentationObject(pushNotificationApp));
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 103, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\PushNotifications\\ProxyCmdletBaseClass.cs");
		}

		private static readonly PushNotificationAppIdParameter ProxyIdentity = new PushNotificationAppIdParameter(PushNotificationCannedApp.OnPremProxy.Name);
	}
}
