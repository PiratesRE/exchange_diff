using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Interop
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct DetectEncodingInfo
	{
		public uint LangID;

		public uint CodePage;

		public int DocPercent;

		public int Confidence;
	}
}
