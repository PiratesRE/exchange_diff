using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class EmailDropDownList : DropDownList
	{
		public EmailDropDownList(string id, ContactPropertyInfo selectedEmailProperty) : base(id, selectedEmailProperty.Id, null)
		{
			this.selectedEmailProperty = selectedEmailProperty;
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			writer.Write(LocalizedStrings.GetHtmlEncoded(this.selectedEmailProperty.Label));
		}

		protected override DropDownListItem[] CreateListItems()
		{
			int num = ContactUtilities.EmailAddressProperties.Length;
			DropDownListItem[] array = new DropDownListItem[num];
			for (int i = 0; i < num; i++)
			{
				ContactPropertyInfo contactPropertyInfo = ContactUtilities.EmailAddressProperties[i];
				array[i] = new DropDownListItem(contactPropertyInfo.Id, contactPropertyInfo.Label);
			}
			return array;
		}

		private ContactPropertyInfo selectedEmailProperty;
	}
}
