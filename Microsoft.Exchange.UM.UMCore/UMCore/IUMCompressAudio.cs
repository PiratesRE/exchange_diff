using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IUMCompressAudio
	{
		ITempFile CompressedAudioFile { get; set; }

		AudioCodecEnum AudioCodec { get; }

		string FileToCompressPath { get; }
	}
}
