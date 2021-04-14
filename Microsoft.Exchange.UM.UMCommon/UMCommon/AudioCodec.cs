using System;
using System.Collections.Generic;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class AudioCodec
	{
		static AudioCodec()
		{
			AudioCodec audioCodec = new AudioCodec(AudioCodecEnum.Mp3, "audio/mp3", true, true, 16000, ".mp3", ".umrmmp3");
			AudioCodec.codecTable.Add(audioCodec.Codec, audioCodec);
			audioCodec = new AudioCodec(AudioCodecEnum.Wma, "audio/wma", false, true, 8000, ".wma", ".umrmwma");
			AudioCodec.codecTable.Add(audioCodec.Codec, audioCodec);
			audioCodec = new AudioCodec(AudioCodecEnum.Gsm, "audio/wav", true, false, 8000, ".wav", ".umrmwav");
			AudioCodec.codecTable.Add(audioCodec.Codec, audioCodec);
			audioCodec = new AudioCodec(AudioCodecEnum.G711, "audio/wav", true, false, 8000, ".wav", ".umrmwav");
			AudioCodec.codecTable.Add(audioCodec.Codec, audioCodec);
		}

		private AudioCodec(AudioCodecEnum codec, string mimeType, bool supportsACM, bool supportsWideband, int samplingRate, string extension, string extensionForProtectedContent)
		{
			this.Codec = codec;
			this.MimeType = mimeType;
			this.SupportsACM = supportsACM;
			this.SupportsWideband = supportsWideband;
			this.SamplingRate = samplingRate;
			this.FileExtension = extension;
			this.FileExtensionForProtectedContent = extensionForProtectedContent;
		}

		internal AudioCodecEnum Codec { get; private set; }

		internal string FileExtension { get; private set; }

		internal string FileExtensionForProtectedContent { get; private set; }

		internal string MimeType { get; private set; }

		internal bool SupportsACM { get; private set; }

		internal bool SupportsWideband { get; private set; }

		internal int SamplingRate { get; private set; }

		internal static AudioCodec GetAudioCodec(AudioCodecEnum codec)
		{
			AudioCodec result;
			if (!AudioCodec.codecTable.TryGetValue(codec, out result))
			{
				throw new ArgumentException("codec");
			}
			return result;
		}

		internal static bool IsACMSupported(AudioCodecEnum codec)
		{
			return AudioCodec.GetAudioCodec(codec).SupportsACM;
		}

		internal static bool IsWidebandSupported(AudioCodecEnum codec)
		{
			return AudioCodec.GetAudioCodec(codec).SupportsWideband;
		}

		internal static string GetMimeType(AudioCodecEnum codec)
		{
			return AudioCodec.GetAudioCodec(codec).MimeType;
		}

		internal static string GetFileExtension(AudioCodecEnum codec)
		{
			return AudioCodec.GetAudioCodec(codec).FileExtension;
		}

		internal static string GetFileExtensionForProtectedContent(AudioCodecEnum codec)
		{
			return AudioCodec.GetAudioCodec(codec).FileExtensionForProtectedContent;
		}

		internal static bool ShouldResample(PcmReader inputReader, AudioCodecEnum codec, out WaveFormat resampleFormat)
		{
			resampleFormat = null;
			AudioCodec audioCodec = AudioCodec.GetAudioCodec(codec);
			if (!audioCodec.SupportsACM)
			{
				throw new ArgumentException("codec");
			}
			if (inputReader.WaveFormat.SamplesPerSec != audioCodec.SamplingRate)
			{
				resampleFormat = ((audioCodec.SamplingRate == WaveFormat.Pcm16WaveFormat.SamplesPerSec) ? WaveFormat.Pcm16WaveFormat : WaveFormat.Pcm8WaveFormat);
			}
			return resampleFormat != null;
		}

		private static Dictionary<AudioCodecEnum, AudioCodec> codecTable = new Dictionary<AudioCodecEnum, AudioCodec>();
	}
}
