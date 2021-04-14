using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class AddressBookItemList : ListViewContents
	{
		public AddressBookItemList(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, UserContext userContext) : base(viewDescriptor, sortedColumn, sortOrder, false, userContext)
		{
			base.AddProperty(ADObjectSchema.ObjectCategory);
			base.AddProperty(ADRecipientSchema.RecipientType);
		}

		protected override bool RenderItemRowStyle(TextWriter writer, int itemIndex)
		{
			RecipientType recipientType = (RecipientType)base.DataSource.GetItemProperty(itemIndex, ADRecipientSchema.RecipientType);
			return Utilities.IsADDistributionList(recipientType);
		}
	}
}
