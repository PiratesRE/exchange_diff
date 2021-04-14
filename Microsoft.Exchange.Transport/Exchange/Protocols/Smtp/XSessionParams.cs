using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XSessionParams
	{
		public XSessionParams(Guid mdbGuid, XSessionType sessionType = XSessionType.Undefined)
		{
			ArgumentValidator.ThrowIfInvalidValue<Guid>("mdbGuid", mdbGuid, (Guid x) => x != Guid.Empty);
			this.MdbGuid = mdbGuid;
			this.Type = sessionType;
		}

		public Guid MdbGuid { get; private set; }

		public XSessionType Type { get; private set; }
	}
}
