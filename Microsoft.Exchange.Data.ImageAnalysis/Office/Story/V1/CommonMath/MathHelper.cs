using System;

namespace Microsoft.Office.Story.V1.CommonMath
{
	internal static class MathHelper
	{
		public static float ChannelLuminance(float intensity)
		{
			if ((double)intensity <= 0.03928)
			{
				return (float)((double)intensity / 12.92);
			}
			return (float)Math.Pow(((double)intensity + 0.055) / 1.055, 2.4);
		}

		public static float ContrastRatio(double firstLuminance, double secondLuminance)
		{
			double num = Math.Min(firstLuminance, secondLuminance);
			double num2 = Math.Max(firstLuminance, secondLuminance);
			return (float)((num2 + 0.05) / (num + 0.05));
		}

		public static float Clamp(this float value, float min, float max)
		{
			value = ((value > max) ? max : value);
			value = ((value < min) ? min : value);
			return value;
		}

		public static bool ExactEquals(this float first, float second)
		{
			if (float.IsNaN(first))
			{
				return float.IsNaN(second);
			}
			if (float.IsNaN(second))
			{
				return float.IsNaN(first);
			}
			return first == second;
		}
	}
}
