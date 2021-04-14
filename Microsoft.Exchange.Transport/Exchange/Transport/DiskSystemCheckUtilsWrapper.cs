using System;
using Microsoft.Exchange.Common.Bitlocker.Utilities;

namespace Microsoft.Exchange.Transport
{
	internal sealed class DiskSystemCheckUtilsWrapper : IDiskSystemCheckUtilsWrapper
	{
		public bool IsFilePathOnLockedVolume(string path, out Exception ex)
		{
			return BitlockerUtil.IsFilePathOnLockedVolume(path, out ex);
		}
	}
}
