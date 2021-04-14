using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAttachment : IDisposable
	{
		IPropertyBag PropertyBag { get; }

		bool IsEmbeddedMessage { get; }

		IMessage GetEmbeddedMessage();

		void Save();

		int AttachmentNumber { get; }
	}
}
