using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IRedirectTargetChooser
	{
		string SubscriberLogId { get; }

		bool GetTargetServer(out string fqdn, out int port);

		void HandleServerNotFound();
	}
}
