using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class ContactsListViewContents : ListViewContents
	{
		private static Dictionary<PropertyDefinition, ThemeFileId> CreatePropertyIconMap()
		{
			Dictionary<PropertyDefinition, ThemeFileId> dictionary = new Dictionary<PropertyDefinition, ThemeFileId>();
			dictionary[ContactSchema.PrimaryTelephoneNumber] = ThemeFileId.PrimaryPhone;
			dictionary[ContactSchema.BusinessPhoneNumber] = ThemeFileId.WorkPhone;
			dictionary[ContactSchema.BusinessPhoneNumber2] = ThemeFileId.WorkPhone;
			dictionary[ContactSchema.MobilePhone] = ThemeFileId.MobilePhone;
			dictionary[ContactSchema.HomePhone] = ThemeFileId.HomePhone;
			dictionary[ContactSchema.HomePhone2] = ThemeFileId.HomePhone;
			dictionary[ContactSchema.HomeFax] = ThemeFileId.Fax;
			dictionary[ContactSchema.WorkFax] = ThemeFileId.Fax;
			dictionary[ContactSchema.OtherFax] = ThemeFileId.Fax;
			dictionary[ContactSchema.AssistantPhoneNumber] = ThemeFileId.None;
			dictionary[ContactSchema.CallbackPhone] = ThemeFileId.None;
			dictionary[ContactSchema.CarPhone] = ThemeFileId.None;
			dictionary[ContactSchema.Pager] = ThemeFileId.None;
			dictionary[ContactSchema.OtherTelephone] = ThemeFileId.None;
			dictionary[ContactSchema.RadioPhone] = ThemeFileId.None;
			dictionary[ContactSchema.TtyTddPhoneNumber] = ThemeFileId.None;
			dictionary[ContactSchema.InternationalIsdnNumber] = ThemeFileId.None;
			dictionary[ContactSchema.OrganizationMainPhone] = ThemeFileId.None;
			return dictionary;
		}

		private static Dictionary<PropertyDefinition, Strings.IDs> CreatePropertyAltMap()
		{
			Dictionary<PropertyDefinition, Strings.IDs> dictionary = new Dictionary<PropertyDefinition, Strings.IDs>();
			dictionary[ContactSchema.PrimaryTelephoneNumber] = 1442239260;
			dictionary[ContactSchema.BusinessPhoneNumber] = 346027136;
			dictionary[ContactSchema.BusinessPhoneNumber2] = 873918106;
			dictionary[ContactSchema.MobilePhone] = 1158653436;
			dictionary[ContactSchema.HomePhone] = -1844864953;
			dictionary[ContactSchema.HomePhone2] = 1714659233;
			dictionary[ContactSchema.HomeFax] = 1180016964;
			dictionary[ContactSchema.WorkFax] = -11305699;
			dictionary[ContactSchema.OtherFax] = -679895069;
			return dictionary;
		}

		public ContactsListViewContents(ViewDescriptor viewDescriptor, ColumnId sortedColumn, SortOrder sortOrder, bool showFolderNameTooltip, UserContext userContext) : base(viewDescriptor, sortedColumn, sortOrder, showFolderNameTooltip, userContext)
		{
			base.AddProperty(StoreObjectSchema.DisplayName);
			base.AddProperty(StoreObjectSchema.ItemClass);
			base.AddProperty(ContactSchema.SelectedPreferredPhoneNumber);
			if (Utilities.IsJapanese)
			{
				base.AddProperty(ContactSchema.YomiFirstName);
				base.AddProperty(ContactSchema.YomiLastName);
			}
			base.AddProperty(ContactSchema.BusinessPhoneNumber);
			base.AddProperty(ContactSchema.HomePhone);
			base.AddProperty(ContactSchema.MobilePhone);
			foreach (ContactPropertyInfo contactPropertyInfo in ContactUtilities.PhoneNumberProperties)
			{
				base.AddProperty(contactPropertyInfo.PropertyDefinition);
			}
		}

		public static Dictionary<PropertyDefinition, ThemeFileId> PropertyIconMap
		{
			get
			{
				return ContactsListViewContents.propertyIconMap;
			}
		}

		public static Dictionary<PropertyDefinition, Strings.IDs> PropertyAltMap
		{
			get
			{
				return ContactsListViewContents.propertyAltMap;
			}
		}

		protected override bool RenderItemRowStyle(TextWriter writer, int itemIndex)
		{
			string itemClass = base.DataSource.GetItemProperty(itemIndex, StoreObjectSchema.ItemClass) as string;
			return ObjectClass.IsDistributionList(itemClass);
		}

		private static Dictionary<PropertyDefinition, ThemeFileId> propertyIconMap = ContactsListViewContents.CreatePropertyIconMap();

		private static Dictionary<PropertyDefinition, Strings.IDs> propertyAltMap = ContactsListViewContents.CreatePropertyAltMap();
	}
}
