using System;

namespace Microsoft.Exchange.Audio
{
	internal class Wma16Writer : WmaWriter
	{
		internal Wma16Writer(string outputFile, WaveFormat waveFormat) : this(outputFile, waveFormat, WmaCodec.Wma9Voice)
		{
		}

		internal Wma16Writer(string outputFile, WaveFormat waveFormat, WmaCodec codec) : base(outputFile, waveFormat, codec)
		{
		}

		protected override string Wma9VoiceProfileString
		{
			get
			{
				return "<profile version=\"589824\" storageformat=\"1\" name=\"UMWma16\" description=\"\"> <streamconfig majortype=\"{73647561-0000-0010-8000-00AA00389B71}\" streamnumber=\"1\" streamname=\"Audio Stream\" inputname=\"Audio409\" bitrate=\"16000\" bufferwindow=\"-1\" reliabletransport=\"0\" decodercomplexity=\"\" rfc1766langid=\"en-us\" > <wmmediatype subtype=\"{00000161-0000-0010-8000-00AA00389B71}\" bfixedsizesamples=\"1\" btemporalcompression=\"0\" lsamplesize=\"640\"> <waveformatex wFormatTag=\"353\" nChannels=\"1\" nSamplesPerSec=\"16000\" nAvgBytesPerSec=\"2000\" nBlockAlign=\"640\" wBitsPerSample=\"16\" codecdata=\"002200002E0080070000\"/> </wmmediatype> </streamconfig> </profile> \0";
			}
		}

		protected override string WmaPcmProfileString
		{
			get
			{
				return "<profile version=\"589824\" storageformat=\"1\" name=\"PCM\" description=\"Streams: 1 audio\"><streamconfig majortype=\"{73647561-0000-0010-8000-00AA00389B71}\" streamnumber=\"1\" streamname=\"Audio1\" inputname=\"\" bitrate=\"256000\" bufferwindow=\"0\" reliabletransport=\"0\" decodercomplexity=\"\" rfc1766langid=\"en-us\"><wmmediatype subtype=\"{00000001-0000-0010-8000-00AA00389B71}\" bfixedsizesamples=\"1\" btemporalcompression=\"0\" lsamplesize=\"2\"><waveformatex wFormatTag=\"1\" nChannels=\"1\" nSamplesPerSec=\"16000\" nAvgBytesPerSec=\"32000\" nBlockAlign=\"2\" wBitsPerSample=\"16\" codecdata=\"00000000000065FE7B37C707020030534C02\" /></wmmediatype></streamconfig></profile> \0";
			}
		}
	}
}
