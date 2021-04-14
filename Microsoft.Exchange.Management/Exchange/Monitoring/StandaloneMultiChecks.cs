using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class StandaloneMultiChecks : MultiReplicationCheck
	{
		public StandaloneMultiChecks(string serverName, IEventManager eventManager, string momEventSource, uint ignoreTransientErrorsThreshold) : base(serverName, eventManager, momEventSource, null, null, null, ignoreTransientErrorsThreshold)
		{
		}

		protected override void Initialize()
		{
			this.m_Checks = new IReplicationCheck[]
			{
				new ReplayServiceCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)),
				new ActiveManagerCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold)),
				new ServerLocatorServiceCheck(this.m_ServerName, this.m_EventManager, this.m_MomEventSource, new uint?(this.m_IgnoreTransientErrorsThreshold))
			};
		}
	}
}
