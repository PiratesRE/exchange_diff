using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMessage : IDisposable
	{
		IPropertyBag PropertyBag { get; }

		bool IsAssociated { get; }

		IEnumerable<IRecipient> GetRecipients();

		IRecipient CreateRecipient();

		void RemoveRecipient(int rowId);

		IEnumerable<IAttachmentHandle> GetAttachments();

		IAttachment CreateAttachment();

		void Save();

		void SetLongTermId(StoreLongTermId longTermId);
	}
}
