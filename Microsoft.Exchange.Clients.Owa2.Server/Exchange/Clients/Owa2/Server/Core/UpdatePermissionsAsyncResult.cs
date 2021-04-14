using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class UpdatePermissionsAsyncResult
	{
		public AttachmentResultCode ResultCode { get; set; }

		public Dictionary<string, IEnumerable<IUserSharingResult>> ResultsDictionary { get; set; }
	}
}
