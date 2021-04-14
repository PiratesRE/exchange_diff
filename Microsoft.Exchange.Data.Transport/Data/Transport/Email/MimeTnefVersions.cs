using System;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal struct MimeTnefVersions
	{
		public MimeTnefVersions(PureMimeMessage mimeMessage, MimePart tnefPart)
		{
			this.RootPartVersion = mimeMessage.Version;
			if (tnefPart != null)
			{
				this.TnefPartVersion = tnefPart.Version;
				return;
			}
			this.TnefPartVersion = -1;
		}

		public int RootPartVersion;

		public int TnefPartVersion;
	}
}
