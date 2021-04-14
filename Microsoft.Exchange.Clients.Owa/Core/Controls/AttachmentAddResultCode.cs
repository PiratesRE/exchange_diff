using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public enum AttachmentAddResultCode
	{
		NoError,
		IrresolvableConflictWhenSaving,
		AttachmentExcceedsSizeLimit,
		ItemExcceedsSizeLimit,
		InsertingNonImageAttachment,
		GeneralErrorWhenSaving
	}
}
