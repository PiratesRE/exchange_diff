using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IDiskSystemCheckUtilsWrapper
	{
		bool IsFilePathOnLockedVolume(string path, out Exception ex);
	}
}
