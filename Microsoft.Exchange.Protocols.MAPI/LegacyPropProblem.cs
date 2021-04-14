using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public struct LegacyPropProblem
	{
		public override string ToString()
		{
			return string.Format("[idx: {0}, tag: {1:X}, error: {2:X}]", this.Idx, this.PropTag, this.ErrorCode);
		}

		public int Idx;

		public uint PropTag;

		public ErrorCodeValue ErrorCode;
	}
}
