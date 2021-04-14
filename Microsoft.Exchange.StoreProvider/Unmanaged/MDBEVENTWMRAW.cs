using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal struct MDBEVENTWMRAW
	{
		public Guid guidMailbox;

		public Guid guidConsumer;

		public ulong eventCounter;
	}
}
