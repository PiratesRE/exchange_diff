using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc
{
	public class MapiVersionConversion
	{
		public static void Normalize(IntPtr pVersion, short[] normalizedVersion)
		{
			short[] array = new short[3];
			Marshal.Copy(pVersion, array, 0, 3);
			MapiVersionConversion.Normalize(array, normalizedVersion);
		}

		public unsafe static void Normalize(short[] version, short[] normalizedVersion)
		{
			if ((ushort)((int)version[1] & 32768) != 0)
			{
				normalizedVersion[0] = (short)(*(ref version[0] + 1L));
				normalizedVersion[1] = (version[0] & 255);
				normalizedVersion[2] = (version[1] & short.MaxValue);
				normalizedVersion[3] = version[2];
			}
			else
			{
				normalizedVersion[0] = version[0];
				normalizedVersion[1] = 0;
				normalizedVersion[2] = version[1];
				normalizedVersion[3] = version[2];
			}
		}

		public static void Legacy(short[] normalizedVersion, IntPtr pVersion, short delta)
		{
			short[] array = new short[3];
			MapiVersionConversion.Legacy(normalizedVersion, array, delta);
			Marshal.Copy(array, 0, pVersion, 3);
		}

		public static void Legacy(short[] normalizedVersion, short[] version, short delta)
		{
			version[0] = (short)((int)normalizedVersion[0] << 8 | (int)normalizedVersion[1]);
			version[1] = (normalizedVersion[2] | short.MinValue);
			version[2] = normalizedVersion[3] + delta;
		}
	}
}
