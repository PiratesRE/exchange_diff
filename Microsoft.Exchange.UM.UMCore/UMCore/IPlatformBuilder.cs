using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IPlatformBuilder
	{
		BaseUMVoipPlatform CreateVoipPlatform();

		BaseCallRouterPlatform CreateCallRouterVoipPlatform(LocalizedString serviceName, LocalizedString serverName, UMADSettings config);

		PlatformSipUri CreateSipUri(string uri);

		bool TryCreateSipUri(string uriString, out PlatformSipUri sipUri);

		PlatformSipUri CreateSipUri(SipUriScheme scheme, string user, string host);

		PlatformSignalingHeader CreateSignalingHeader(string name, string value);

		bool TryCreateOfflineTranscriber(CultureInfo transcriptionLanguage, out BaseUMOfflineTranscriber transcriber);

		bool TryCreateMobileRecognizer(Guid requestId, CultureInfo culture, SpeechRecognitionEngineType engineType, int maxAlternates, out IMobileRecognizer recognizer);
	}
}
