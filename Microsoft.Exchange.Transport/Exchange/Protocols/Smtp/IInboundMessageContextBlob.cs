using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface IInboundMessageContextBlob
	{
		bool IsMandatory { get; }

		string Name { get; }

		ByteQuantifiedSize MaxBlobSize { get; }

		bool IsAdvertised(IEhloOptions ehloOptions);

		void DeserializeBlob(Stream stream, SmtpInSessionState sessionState, long blobSize);

		bool VerifyPermission(Permission permission);
	}
}
