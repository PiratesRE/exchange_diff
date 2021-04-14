using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IContentsSynchronizerClient : IDisposable
	{
		void SetProgressInformation(ProgressInformation progressInformation);

		IMessageChangeClient UploadMessageChange();

		IPropertyBag LoadDeletionPropertyBag();

		IPropertyBag LoadReadUnreadPropertyBag();

		IIcsState LoadFinalState();
	}
}
