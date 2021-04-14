using System;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	public interface INotifyCallback
	{
		void BecomePame();

		void RevokePame();

		NotificationResponse DatabaseMoveNeeded(Guid dbId, string currentActiveFqdn, bool mountDesired);
	}
}
