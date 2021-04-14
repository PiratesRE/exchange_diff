using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class MailingAddressDropDownList : DropDownList
	{
		public MailingAddressDropDownList(string id, PhysicalAddressType addressType)
		{
			int num = (int)addressType;
			base..ctor(id, num.ToString(), null);
			this.addressType = addressType;
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			Utilities.HtmlEncode(ContactUtilities.GetPhysicalAddressString(this.addressType), writer);
		}

		protected override DropDownListItem[] CreateListItems()
		{
			DropDownListItem[] array = new DropDownListItem[MailingAddressDropDownList.physicalAddressTypes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new DropDownListItem((int)MailingAddressDropDownList.physicalAddressTypes[i], ContactUtilities.GetPhysicalAddressString(MailingAddressDropDownList.physicalAddressTypes[i]), false);
			}
			return array;
		}

		private static readonly PhysicalAddressType[] physicalAddressTypes = new PhysicalAddressType[]
		{
			PhysicalAddressType.None,
			PhysicalAddressType.Home,
			PhysicalAddressType.Business,
			PhysicalAddressType.Other
		};

		private PhysicalAddressType addressType;
	}
}
