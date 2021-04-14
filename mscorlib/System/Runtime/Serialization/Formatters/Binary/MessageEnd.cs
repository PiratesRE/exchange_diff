using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal sealed class MessageEnd : IStreamable
	{
		internal MessageEnd()
		{
		}

		public void Write(__BinaryWriter sout)
		{
			sout.WriteByte(11);
		}

		[SecurityCritical]
		public void Read(__BinaryParser input)
		{
		}

		public void Dump()
		{
		}

		public void Dump(Stream sout)
		{
		}

		[Conditional("_LOGGING")]
		private void DumpInternal(Stream sout)
		{
			if (BCLDebug.CheckEnabled("BINARY") && sout != null && sout.CanSeek)
			{
				long length = sout.Length;
			}
		}
	}
}
