using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ADConfigurationManager
	{
		public ADConfigurationManager(ITopologyConfigurationSession configSession = null)
		{
			this.ConfigSession = (configSession ?? DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 29, ".ctor", "f:\\15.00.1497\\sources\\dev\\PushNotifications\\src\\publishers\\Configuration\\ADConfigurationManager.cs"));
		}

		public ITopologyConfigurationSession ConfigSession { get; set; }

		public virtual IEnumerable<PushNotificationApp> GetAllPushNotficationApps()
		{
			return this.ConfigSession.FindAllPaged<PushNotificationApp>();
		}

		public virtual void Save(PushNotificationApp pushApp)
		{
			this.ConfigSession.Save(pushApp);
		}
	}
}
