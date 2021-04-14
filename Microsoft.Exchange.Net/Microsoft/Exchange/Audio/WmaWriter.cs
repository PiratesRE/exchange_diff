using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	internal abstract class WmaWriter : IDisposable
	{
		protected WmaWriter(string outputFile, WaveFormat waveFormat, WmaCodec codec)
		{
			this.Codec = codec;
			WmaWriter.profileManager.LoadProfileByData(this.ProfileString, out this.profile);
			this.writer = WindowsMediaNativeMethods.CreateWriter();
			this.writer.SetProfile(this.profile);
			this.writer.SetOutputFilename(outputFile);
			this.waveFormat = waveFormat;
			this.filePath = outputFile;
			this.SetInputProperties();
		}

		internal string ProfileString
		{
			get
			{
				switch (this.Codec)
				{
				case WmaCodec.Wma9Voice:
					return this.Wma9VoiceProfileString;
				case WmaCodec.Pcm:
					return this.WmaPcmProfileString;
				default:
					throw new NotImplementedException();
				}
			}
		}

		internal WmaCodec Codec { get; private set; }

		internal int BufferSize
		{
			get
			{
				int num = (this.Codec == WmaCodec.Wma9Voice) ? (5 * this.waveFormat.AvgBytesPerSec) : (this.waveFormat.AvgBytesPerSec / 2);
				return num + num % (int)this.waveFormat.BlockAlign;
			}
		}

		internal string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		protected abstract string Wma9VoiceProfileString { get; }

		protected abstract string WmaPcmProfileString { get; }

		public void Dispose()
		{
			if (this.profile != null)
			{
				Marshal.ReleaseComObject(this.profile);
				this.profile = null;
			}
			if (this.writer != null)
			{
				try
				{
					this.writer.EndWriting();
				}
				finally
				{
					Marshal.ReleaseComObject(this.writer);
					this.writer = null;
				}
			}
		}

		internal static WmaWriter Create(string outputFile, WaveFormat waveFormat)
		{
			return WmaWriter.Create(outputFile, waveFormat, WmaCodec.Wma9Voice);
		}

		internal static WmaWriter Create(string outputFile, WaveFormat waveFormat, WmaCodec codec)
		{
			if (waveFormat.SamplesPerSec == 8000)
			{
				return new Wma8Writer(outputFile, WaveFormat.Pcm8WaveFormat, codec);
			}
			if (waveFormat.SamplesPerSec == 16000)
			{
				return new Wma16Writer(outputFile, WaveFormat.Pcm16WaveFormat, codec);
			}
			throw new UnsupportedAudioFormat(outputFile);
		}

		internal void Write(byte[] buffer, int count)
		{
			INSSBuffer inssbuffer;
			this.writer.AllocateSample((uint)count, out inssbuffer);
			using (WindowsMediaBuffer windowsMediaBuffer = new WindowsMediaBuffer(inssbuffer))
			{
				windowsMediaBuffer.Write(buffer, count);
				this.writer.WriteSample(0U, this.sampleTime * 10000UL, 0U, inssbuffer);
				this.sampleTime += (ulong)((long)count * 1000L / (long)this.waveFormat.AvgBytesPerSec);
			}
			inssbuffer = null;
		}

		private void SetInputProperties()
		{
			IWMInputMediaProps iwminputMediaProps;
			this.writer.GetInputProps(0U, out iwminputMediaProps);
			Guid guid;
			iwminputMediaProps.GetType(out guid);
			WindowsMediaNativeMethods.WM_MEDIA_TYPE wm_MEDIA_TYPE;
			wm_MEDIA_TYPE.majortype = WindowsMediaNativeMethods.MediaTypes.WMMEDIATYPE_Audio;
			wm_MEDIA_TYPE.subtype = WindowsMediaNativeMethods.MediaTypes.WMMEDIASUBTYPE_PCM;
			wm_MEDIA_TYPE.bFixedSizeSamples = true;
			wm_MEDIA_TYPE.bTemporalCompression = false;
			wm_MEDIA_TYPE.lSampleSize = (uint)this.waveFormat.BlockAlign;
			wm_MEDIA_TYPE.formattype = WindowsMediaNativeMethods.MediaTypes.WMFORMAT_WaveFormatEx;
			wm_MEDIA_TYPE.pUnk = IntPtr.Zero;
			wm_MEDIA_TYPE.cbFormat = 16U;
			GCHandle gchandle = GCHandle.Alloc(this.waveFormat, GCHandleType.Pinned);
			try
			{
				wm_MEDIA_TYPE.pbFormat = gchandle.AddrOfPinnedObject();
				iwminputMediaProps.SetMediaType(ref wm_MEDIA_TYPE);
			}
			finally
			{
				gchandle.Free();
			}
			this.writer.SetInputProps(0U, iwminputMediaProps);
			this.writer.BeginWriting();
		}

		private const int NumBuffersPerByteRate = 10;

		private const int MilliSecondsTo100NSFactor = 10000;

		private const int SecondsToMilliSecondsFactor = 1000;

		private static IWMProfileManager profileManager = WindowsMediaNativeMethods.CreateProfileManager();

		private IWMWriter writer;

		private IWMProfile profile;

		private WaveFormat waveFormat;

		private ulong sampleTime;

		private string filePath;
	}
}
