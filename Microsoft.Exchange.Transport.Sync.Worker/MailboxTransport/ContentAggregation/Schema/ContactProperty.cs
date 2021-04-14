using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Properties.XSO;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ContactProperty<T> : XSOPropertyBase<T>
	{
		protected ContactProperty(IXSOPropertyManager propertyManager, params PropertyDefinition[] propertyDefinitions) : base(propertyManager, propertyDefinitions)
		{
		}

		internal static Func<Item, bool> IsItemNewDelegate
		{
			set
			{
				ContactProperty<T>.isItemNewDelegate = value;
			}
		}

		protected bool IsItemNew(Item item)
		{
			if (ContactProperty<T>.isItemNewDelegate == null)
			{
				return item.Id == null;
			}
			return ContactProperty<T>.isItemNewDelegate(item);
		}

		private static Func<Item, bool> isItemNewDelegate;
	}
}
