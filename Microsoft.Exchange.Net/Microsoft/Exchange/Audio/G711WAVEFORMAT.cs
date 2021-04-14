using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
	internal class G711WAVEFORMAT : WaveFormat
	{
		internal G711WAVEFORMAT(G711Format format) : base(8000, 8, 1)
		{
			base.FormatTag = (short)format;
		}
	}
}
