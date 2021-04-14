using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public static class MoveItemHelper
	{
		public static ApplicationElement GetApplicationElementFromStoreType(string type)
		{
			if (ObjectClass.IsGenericFolder(type))
			{
				return ApplicationElement.Folder;
			}
			return ApplicationElement.Item;
		}

		public static NavigationModule GetNavigationModuleFromStoreType(string type)
		{
			StoreObjectType objectType = ObjectClass.GetObjectType(type);
			if (objectType == StoreObjectType.Contact || objectType == StoreObjectType.DistributionList || objectType == StoreObjectType.ContactsFolder)
			{
				return NavigationModule.Contacts;
			}
			if (objectType == StoreObjectType.CalendarFolder || objectType == StoreObjectType.CalendarItem)
			{
				return NavigationModule.Calendar;
			}
			return NavigationModule.Mail;
		}
	}
}
