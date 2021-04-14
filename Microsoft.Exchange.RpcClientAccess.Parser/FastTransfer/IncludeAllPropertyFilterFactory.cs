using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IncludeAllPropertyFilterFactory : IPropertyFilterFactory
	{
		private IncludeAllPropertyFilterFactory()
		{
		}

		public IPropertyFilter GetAttachmentCopyToFilter(bool isTopLevel)
		{
			return IncludeAllPropertyFilter.Instance;
		}

		public IMessagePropertyFilter GetMessageCopyToFilter(bool isTopLevel)
		{
			return IncludeAllMessagePropertyFilter.Instance;
		}

		public IPropertyFilter GetFolderCopyToFilter()
		{
			return IncludeAllPropertyFilter.Instance;
		}

		public IPropertyFilter GetCopySubfolderFilter()
		{
			return IncludeAllPropertyFilter.Instance;
		}

		public IPropertyFilter GetCopyFolderFilter()
		{
			return IncludeAllPropertyFilter.Instance;
		}

		public IPropertyFilter GetAttachmentFilter(bool isTopLevel)
		{
			return IncludeAllPropertyFilter.Instance;
		}

		public IPropertyFilter GetEmbeddedMessageFilter(bool isTopLevel)
		{
			return IncludeAllPropertyFilter.Instance;
		}

		public IPropertyFilter GetMessageFilter(bool isTopLevel)
		{
			return IncludeAllPropertyFilter.Instance;
		}

		public IPropertyFilter GetRecipientFilter()
		{
			return IncludeAllPropertyFilter.Instance;
		}

		public IPropertyFilter GetIcsHierarchyFilter(bool includeFid, bool includeChangeNumber)
		{
			return IncludeAllPropertyFilter.Instance;
		}

		public static IPropertyFilterFactory Instance = new IncludeAllPropertyFilterFactory();
	}
}
