using System;
using System.Collections;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FaxTransferReferredByHeaderHandler : ReferredByHeaderHandler
	{
		internal PlatformSipUri SerializeFaxTransferUri(string recipient, string context)
		{
			if (string.IsNullOrEmpty(recipient) || string.IsNullOrEmpty(context))
			{
				throw new ArgumentNullException("FaxTransferUri");
			}
			return base.FrameHeader(new Hashtable
			{
				{
					"msExchUMFaxRecipient",
					recipient
				},
				{
					"msExchUMContext",
					context
				}
			});
		}

		private const string FaxRecipient = "msExchUMFaxRecipient";

		private const string UMContext = "msExchUMContext";
	}
}
