using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICoreItemContext
	{
		void GetContextCharsetDetectionData(StringBuilder stringBuilder, CharsetDetectionDataFlags flags);
	}
}
