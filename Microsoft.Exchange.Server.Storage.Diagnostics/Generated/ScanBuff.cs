using System;

namespace Microsoft.Exchange.Server.Storage.Diagnostics.Generated
{
	public abstract class ScanBuff
	{
		public abstract int Pos { get; set; }

		public abstract int Read();

		public abstract int Peek();

		public abstract int ReadPos { get; }

		public abstract string GetString(int b, int e);

		public const int EOF = -1;
	}
}
