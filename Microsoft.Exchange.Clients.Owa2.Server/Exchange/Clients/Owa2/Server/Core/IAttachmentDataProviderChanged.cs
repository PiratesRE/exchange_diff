using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public interface IAttachmentDataProviderChanged
	{
		event AttachmentDataProviderChangedEventHandler AttachmentDataProviderChanged;
	}
}
