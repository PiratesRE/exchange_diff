using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	internal class ServerHealthInfo
	{
		public AmServerName ServerName { get; private set; }

		public StateTransitionInfo ServerFoundInAD { get; private set; }

		public StateTransitionInfo CriticalForMaintainingAvailability { get; private set; }

		public StateTransitionInfo CriticalForMaintainingRedundancy { get; private set; }

		public StateTransitionInfo CriticalForRestoringAvailability { get; private set; }

		public StateTransitionInfo CriticalForRestoringRedundancy { get; private set; }

		public StateTransitionInfo HighForRestoringAvailability { get; private set; }

		public StateTransitionInfo HighForRestoringRedundancy { get; private set; }

		public ServerHealthInfo(AmServerName serverName)
		{
			this.ServerName = serverName;
			this.ServerFoundInAD = new StateTransitionInfo();
			this.CriticalForMaintainingAvailability = new StateTransitionInfo();
			this.CriticalForMaintainingRedundancy = new StateTransitionInfo();
			this.CriticalForRestoringAvailability = new StateTransitionInfo();
			this.CriticalForRestoringRedundancy = new StateTransitionInfo();
			this.HighForRestoringAvailability = new StateTransitionInfo();
			this.HighForRestoringRedundancy = new StateTransitionInfo();
		}

		public ServerHealthInfoPersisted ConvertToSerializable()
		{
			return new ServerHealthInfoPersisted(this.ServerName.Fqdn)
			{
				ServerFoundInAD = this.ServerFoundInAD.ConvertToSerializable(),
				CriticalForMaintainingAvailability = this.CriticalForMaintainingAvailability.ConvertToSerializable(),
				CriticalForMaintainingRedundancy = this.CriticalForMaintainingRedundancy.ConvertToSerializable(),
				CriticalForRestoringAvailability = this.CriticalForRestoringAvailability.ConvertToSerializable(),
				CriticalForRestoringRedundancy = this.CriticalForRestoringRedundancy.ConvertToSerializable(),
				HighForRestoringAvailability = this.HighForRestoringAvailability.ConvertToSerializable(),
				HighForRestoringRedundancy = this.HighForRestoringRedundancy.ConvertToSerializable()
			};
		}

		public void InitializeFromSerializable(ServerHealthInfoPersisted ship)
		{
			this.ServerFoundInAD = StateTransitionInfo.ConstructFromPersisted(ship.ServerFoundInAD);
			this.CriticalForMaintainingAvailability = StateTransitionInfo.ConstructFromPersisted(ship.CriticalForMaintainingAvailability);
			this.CriticalForMaintainingRedundancy = StateTransitionInfo.ConstructFromPersisted(ship.CriticalForMaintainingRedundancy);
			this.CriticalForRestoringAvailability = StateTransitionInfo.ConstructFromPersisted(ship.CriticalForRestoringAvailability);
			this.CriticalForRestoringRedundancy = StateTransitionInfo.ConstructFromPersisted(ship.CriticalForRestoringRedundancy);
			this.HighForRestoringAvailability = StateTransitionInfo.ConstructFromPersisted(ship.HighForRestoringAvailability);
			this.HighForRestoringRedundancy = StateTransitionInfo.ConstructFromPersisted(ship.HighForRestoringRedundancy);
		}
	}
}
