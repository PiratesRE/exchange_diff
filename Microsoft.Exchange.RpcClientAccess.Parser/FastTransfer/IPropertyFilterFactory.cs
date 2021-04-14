using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPropertyFilterFactory
	{
		IPropertyFilter GetAttachmentCopyToFilter(bool isTopLevel);

		IMessagePropertyFilter GetMessageCopyToFilter(bool isTopLevel);

		IPropertyFilter GetFolderCopyToFilter();

		IPropertyFilter GetCopyFolderFilter();

		IPropertyFilter GetCopySubfolderFilter();

		IPropertyFilter GetAttachmentFilter(bool isTopLevel);

		IPropertyFilter GetEmbeddedMessageFilter(bool isTopLevel);

		IPropertyFilter GetMessageFilter(bool isTopLevel);

		IPropertyFilter GetRecipientFilter();

		IPropertyFilter GetIcsHierarchyFilter(bool includeFid, bool includeChangeNumber);
	}
}
