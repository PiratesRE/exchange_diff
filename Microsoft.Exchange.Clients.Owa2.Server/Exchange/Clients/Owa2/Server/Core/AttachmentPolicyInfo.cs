using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AttachmentPolicyInfo
	{
		public AttachmentPolicyLevel Level { get; set; }

		public bool IsViewableInBrowser { get; set; }

		public bool ForceBrowserViewingFirst { get; set; }
	}
}
