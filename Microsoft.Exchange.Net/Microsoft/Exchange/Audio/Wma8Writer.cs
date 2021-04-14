using System;

namespace Microsoft.Exchange.Audio
{
	internal class Wma8Writer : WmaWriter
	{
		internal Wma8Writer(string outputFile, WaveFormat waveFormat) : this(outputFile, waveFormat, WmaCodec.Wma9Voice)
		{
		}

		internal Wma8Writer(string outputFile, WaveFormat waveFormat, WmaCodec codec) : base(outputFile, waveFormat, codec)
		{
		}

		protected override string Wma9VoiceProfileString
		{
			get
			{
				return "<profile version=\"589824\" storageformat=\"1\" name=\"UMWma\" description=\"Streams: 1 audio\"> <streamconfig majortype=\"{73647561-0000-0010-8000-00AA00389B71}\" streamnumber=\"1\" streamname=\"Audio1\" inputname=\"\" bitrate=\"8000\" bufferwindow=\"3000\" reliabletransport=\"0\" decodercomplexity=\"\" rfc1766langid=\"en-us\" > <wmmediatype subtype=\"{0000000A-0000-0010-8000-00AA00389B71}\" bfixedsizesamples=\"1\" btemporalcompression=\"1\" lsamplesize=\"3435973836\"> <waveformatex wFormatTag=\"10\" nChannels=\"1\" nSamplesPerSec=\"8000\" nAvgBytesPerSec=\"1000\" nBlockAlign=\"300\" wBitsPerSample=\"16\" codecdata=\"100004000000000000000000000000000000CF881A0005C92D499019400000000000000000000000000000000000\"/> </wmmediatype> </streamconfig> </profile> \0";
			}
		}

		protected override string WmaPcmProfileString
		{
			get
			{
				return "<profile version=\"589824\" storageformat=\"1\" name=\"PCM\" description=\"Streams: 1 audio\"><streamconfig majortype=\"{73647561-0000-0010-8000-00AA00389B71}\" streamnumber=\"1\" streamname=\"Audio1\" inputname=\"\" bitrate=\"128000\" bufferwindow=\"0\" reliabletransport=\"0\" decodercomplexity=\"\" rfc1766langid=\"en-us\"><wmmediatype subtype=\"{00000001-0000-0010-8000-00AA00389B71}\" bfixedsizesamples=\"1\" btemporalcompression=\"0\" lsamplesize=\"2\"><waveformatex wFormatTag=\"1\" nChannels=\"1\" nSamplesPerSec=\"8000\" nAvgBytesPerSec=\"16000\" nBlockAlign=\"2\" wBitsPerSample=\"16\" codecdata=\"00000000000062FE7A31C707020E30003000\" /></wmmediatype></streamconfig></profile> \0";
			}
		}
	}
}
