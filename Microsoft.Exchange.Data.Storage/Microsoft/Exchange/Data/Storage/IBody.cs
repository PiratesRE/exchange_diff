using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IBody
	{
		BodyFormat Format { get; }

		bool IsBodyDefined { get; }

		int GetLastNBytesAsString(int lastNBytesToRead, out string readString);

		void CopyBodyInjectingText(IBody targetBody, BodyInjectionFormat injectionFormat, string prefixInjectionText, string suffixInjectionText);

		Stream OpenWriteStream(BodyWriteConfiguration configuration);
	}
}
