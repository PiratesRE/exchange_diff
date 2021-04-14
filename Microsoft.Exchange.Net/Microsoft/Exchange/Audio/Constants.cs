using System;

namespace Microsoft.Exchange.Audio
{
	internal class Constants
	{
		internal const int WmaBufferTimeSEC = 5;

		internal const uint WaveFormatExSize = 16U;

		internal const double DefaultNormalizationLevel = 0.088;

		internal const double NoiseFloorLevel = 9E-05;

		internal const string ProtectedVoiceMailExtensionWav = ".umrmwav";

		internal const string ProtectedVoiceMailExtensionWma = ".umrmwma";

		internal const string ProtectedVoiceMailExtensionMp3 = ".umrmmp3";

		internal const string WmaFileExtension = ".wma";

		internal const string WaveFileExtension = ".wav";

		internal const string Mp3FileExtension = ".mp3";
	}
}
