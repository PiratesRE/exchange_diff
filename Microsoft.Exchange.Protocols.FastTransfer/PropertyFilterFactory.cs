using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyFilterFactory : IPropertyFilterFactory
	{
		public PropertyFilterFactory(ICollection<PropertyTag> excludedPropertyTags)
		{
			this.propertyCopyToFilterTopLevel = new ExcludingPropertyFilter(excludedPropertyTags);
			this.messageCopyToFilterTopLevel = new MessagePropertyFilter(excludedPropertyTags);
		}

		public IPropertyFilter GetAttachmentCopyToFilter(bool isTopLevel)
		{
			if (isTopLevel)
			{
				return this.propertyCopyToFilterTopLevel;
			}
			return IncludeAllPropertyFilter.Instance;
		}

		public IMessagePropertyFilter GetMessageCopyToFilter(bool isTopLevel)
		{
			if (isTopLevel)
			{
				return this.messageCopyToFilterTopLevel;
			}
			return IncludeAllMessagePropertyFilter.Instance;
		}

		public IPropertyFilter GetFolderCopyToFilter()
		{
			return this.propertyCopyToFilterTopLevel;
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

		private IPropertyFilter propertyCopyToFilterTopLevel;

		private IMessagePropertyFilter messageCopyToFilterTopLevel;

		public static IPropertyFilterFactory IncludeAllFactory = new PropertyFilterFactory(Array<PropertyTag>.Empty);
	}
}
