using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal struct ProcessingResult
	{
		public ResultCode Code;

		public FrameSourceType SourceType;

		public string RawErrorFrame;

		public string Package;

		public string Function;

		public int Line;

		public int Column;
	}
}
