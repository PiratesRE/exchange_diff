using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NoCloseStream : BaseStream
	{
		public NoCloseStream(Stream stream) : base(stream)
		{
		}

		public override void Close()
		{
		}
	}
}
