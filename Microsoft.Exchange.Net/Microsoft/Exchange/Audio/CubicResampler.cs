using System;

namespace Microsoft.Exchange.Audio
{
	internal abstract class CubicResampler
	{
		internal static bool CanResample(PcmReader pcmReader, PcmWriter pcmWriter)
		{
			return pcmWriter.WaveFormat.SamplesPerSec == 2 * pcmReader.WaveFormat.SamplesPerSec;
		}

		internal static bool TryResample(PcmReader pcmReader, PcmWriter pcmWriter)
		{
			if (CubicResampler.CanResample(pcmReader, pcmWriter))
			{
				CubicResampler.Resample(pcmReader, pcmWriter);
				return true;
			}
			return false;
		}

		internal static void Resample(PcmReader pcmReader, PcmWriter pcmWriter)
		{
			if (!CubicResampler.CanResample(pcmReader, pcmWriter))
			{
				throw new InvalidOperationException();
			}
			byte[] array = new byte[65536];
			byte[] buffer = new byte[131072];
			double num2;
			double y;
			double num = y = (num2 = 0.0);
			for (;;)
			{
				int num3 = pcmReader.Read(array, array.Length);
				if (num3 <= 0)
				{
					break;
				}
				int i = 0;
				int num4 = 0;
				while (i < num3)
				{
					double num5 = (double)CubicResampler.ReadPcmSample(array, i);
					i += 2;
					double sample = CubicResampler.CubicInterpolate(y, num, num2, num5, 0.5);
					CubicResampler.WritePcmSample(buffer, num4, sample);
					num4 += 2;
					CubicResampler.WritePcmSample(buffer, num4, num2);
					num4 += 2;
					y = num;
					num = num2;
					num2 = num5;
				}
				pcmWriter.Write(buffer, num4);
			}
		}

		private static double CubicInterpolate(double y0, double y1, double y2, double y3, double x)
		{
			double num = x * x;
			double num2 = y3 - y2 - y0 + y1;
			double num3 = y0 - y1 - num2;
			double num4 = y2 - y0;
			return num2 * x * num + num3 * num + num4 * x + y1;
		}

		private static void WritePcmSample(byte[] buffer, int position, double sample)
		{
			short num;
			if (sample > 32767.0)
			{
				num = short.MaxValue;
			}
			else if (sample < -32768.0)
			{
				num = short.MinValue;
			}
			else
			{
				num = (short)sample;
			}
			buffer[position] = (byte)(num & 255);
			buffer[position + 1] = (byte)(num >> 8);
		}

		private static short ReadPcmSample(byte[] buffer, int position)
		{
			return (short)((int)buffer[position] | (int)buffer[position + 1] << 8);
		}

		private const int CbPcmSrcBuffer = 65536;
	}
}
