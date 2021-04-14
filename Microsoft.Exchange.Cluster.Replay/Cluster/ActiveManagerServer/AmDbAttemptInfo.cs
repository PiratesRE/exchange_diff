using System;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbAttemptInfo
	{
		internal AmDbAttemptInfo(Guid guid, AmDbActionCode actionCode, DateTime attemptTime)
		{
			this.Guid = guid;
			this.ActionCode = actionCode;
			this.LastAttemptTime = attemptTime;
		}

		internal Guid Guid { get; set; }

		internal AmDbActionCode ActionCode { get; set; }

		internal DateTime LastAttemptTime { get; set; }
	}
}
