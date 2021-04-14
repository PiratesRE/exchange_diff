using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class TTSVolumeMap
	{
		internal static int GetVolume(CultureInfo culture)
		{
			return LocConfig.Instance[culture].General.TTSVolume;
		}
	}
}
