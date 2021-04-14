using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class Stream : ServerObject
	{
		internal Stream(Logon logon) : base(logon)
		{
		}

		public static Stream Create(Stream propertyStream, PropertyType propertyType, Logon logon, StreamSource streamSource)
		{
			return new PropertyStream(propertyStream, propertyType, logon, streamSource);
		}

		public static Stream CreateNull(Logon logon)
		{
			return new NullStream(logon);
		}

		public abstract void Commit();

		public abstract uint GetSize();

		public abstract ArraySegment<byte> Read(ushort requestedSize);

		public abstract long Seek(StreamSeekOrigin streamSeekOrigin, long offset);

		public abstract void SetSize(ulong size);

		public abstract ushort Write(ArraySegment<byte> data);

		public abstract ulong CopyToStream(Stream destinationStream, ulong bytesToCopy);

		public abstract void CheckCanWrite();
	}
}
