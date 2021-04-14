using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	internal class WmaReader : IDisposable
	{
		internal WmaReader(string fileName)
		{
			this.reader = WindowsMediaNativeMethods.CreateSyncReader();
			try
			{
				this.reader.Open(fileName);
				this.Initialize();
			}
			catch
			{
				try
				{
					this.reader.Close();
				}
				finally
				{
					Marshal.ReleaseComObject(this.reader);
					this.reader = null;
				}
				throw;
			}
		}

		internal WaveFormat Format
		{
			get
			{
				return new WaveFormat(this.waveFormat.SamplesPerSec, (int)this.waveFormat.BitsPerSample, (int)this.waveFormat.Channels);
			}
		}

		internal int SampleSize
		{
			get
			{
				return (int)this.sampleSize;
			}
		}

		public void Dispose()
		{
			if (this.reader != null)
			{
				try
				{
					this.reader.Close();
				}
				finally
				{
					Marshal.ReleaseComObject(this.reader);
					this.reader = null;
				}
			}
			if (this.bufferReader != null)
			{
				this.bufferReader.Dispose();
			}
		}

		internal int Read(byte[] buffer, int numBytes)
		{
			int i = 0;
			if (this.reader != null)
			{
				if (this.fileSize > 0L && this.fileSize - this.position < (long)numBytes)
				{
					numBytes = (int)(this.fileSize - this.position);
				}
				if (this.bufferReader != null)
				{
					i += this.bufferReader.Read(buffer, 0, numBytes);
				}
				while (i < numBytes)
				{
					INSSBuffer inssBuffer = null;
					ulong num = 0UL;
					ulong num2 = 0UL;
					uint num3 = 0U;
					try
					{
						this.reader.GetNextSample(this.outputStream, out inssBuffer, out num, out num2, out num3, out this.outputNumber, out this.outputStream);
					}
					catch (COMException ex)
					{
						if (ex.ErrorCode == -1072886833)
						{
							break;
						}
						throw;
					}
					this.bufferReader = new WindowsMediaBuffer(inssBuffer);
					inssBuffer = null;
					i += this.bufferReader.Read(buffer, i, numBytes - i);
				}
				if (this.bufferReader != null && this.bufferReader.Position >= this.bufferReader.Length)
				{
					this.bufferReader.Dispose();
					this.bufferReader = null;
				}
				this.position += (long)i;
				return i;
			}
			return i;
		}

		private void Initialize()
		{
			IWMOutputMediaProps iwmoutputMediaProps = null;
			uint num = 0U;
			this.reader.GetOutputCount(out num);
			if (num != 1U)
			{
				throw new InvalidWmaFormatException();
			}
			this.reader.GetOutputFormat(0U, 0U, out iwmoutputMediaProps);
			uint cb = 0U;
			iwmoutputMediaProps.GetMediaType(IntPtr.Zero, ref cb);
			IntPtr intPtr = Marshal.AllocCoTaskMem((int)cb);
			try
			{
				iwmoutputMediaProps.GetMediaType(intPtr, ref cb);
				WindowsMediaNativeMethods.WM_MEDIA_TYPE wm_MEDIA_TYPE = (WindowsMediaNativeMethods.WM_MEDIA_TYPE)Marshal.PtrToStructure(intPtr, typeof(WindowsMediaNativeMethods.WM_MEDIA_TYPE));
				if (!(wm_MEDIA_TYPE.majortype == WindowsMediaNativeMethods.MediaTypes.WMMEDIATYPE_Audio) || !(wm_MEDIA_TYPE.formattype == WindowsMediaNativeMethods.MediaTypes.WMFORMAT_WaveFormatEx) || wm_MEDIA_TYPE.cbFormat < 16U)
				{
					throw new InvalidWmaFormatException();
				}
				this.sampleSize = wm_MEDIA_TYPE.lSampleSize;
				this.waveFormat = new WaveFormat(8000, 16, 1);
				Marshal.PtrToStructure(wm_MEDIA_TYPE.pbFormat, this.waveFormat);
			}
			finally
			{
				Marshal.FreeCoTaskMem(intPtr);
				Marshal.ReleaseComObject(iwmoutputMediaProps);
				iwmoutputMediaProps = null;
			}
			this.fileSize = (long)this.GetFileSize();
		}

		private ulong GetFileSize()
		{
			IWMHeaderInfo iwmheaderInfo = this.reader as IWMHeaderInfo;
			ushort num = 0;
			ushort num2 = 8;
			WindowsMediaNativeMethods.WMT_ATTR_DATATYPE wmt_ATTR_DATATYPE = WindowsMediaNativeMethods.WMT_ATTR_DATATYPE.WMT_TYPE_QWORD;
			ulong num3 = 0UL;
			try
			{
				ulong num4 = 0UL;
				iwmheaderInfo.GetAttributeByName(ref num, "Duration", out wmt_ATTR_DATATYPE, ref num4, ref num2);
				num3 = num4 * (ulong)((long)this.waveFormat.AvgBytesPerSec) / 10000000UL;
				num3 -= num3 % (ulong)((long)this.waveFormat.BlockAlign);
			}
			finally
			{
			}
			return num3;
		}

		private const ulong SecondsToNSFactor = 10000000UL;

		private const string Duration = "Duration";

		private const int NsENoMoreSamples = -1072886833;

		private IWMSyncReader reader;

		private WaveFormat waveFormat;

		private WindowsMediaBuffer bufferReader;

		private long position;

		private long fileSize = -1L;

		private uint sampleSize;

		private ushort outputStream;

		private uint outputNumber;
	}
}
