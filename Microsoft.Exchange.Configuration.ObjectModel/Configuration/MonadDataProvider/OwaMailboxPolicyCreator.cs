using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class OwaMailboxPolicyCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"WhenChanged",
				"DistinguishedName",
				"ActiveSyncIntegrationEnabled",
				"AllAddressListsEnabled",
				"CalendarEnabled",
				"ContactsEnabled",
				"JournalEnabled",
				"JunkEmailEnabled",
				"RemindersAndNotificationsEnabled",
				"NotesEnabled",
				"PremiumClientEnabled",
				"SearchFoldersEnabled",
				"SignaturesEnabled",
				"SpellCheckerEnabled",
				"TasksEnabled",
				"ThemeSelectionEnabled",
				"UMIntegrationEnabled",
				"ChangePasswordEnabled",
				"RulesEnabled",
				"PublicFoldersEnabled",
				"SMimeEnabled",
				"RecoverDeletedItemsEnabled",
				"InstantMessagingEnabled",
				"TextMessagingEnabled",
				"DirectFileAccessOnPublicComputersEnabled",
				"WebReadyDocumentViewingOnPublicComputersEnabled",
				"ForceWebReadyDocumentViewingFirstOnPublicComputers",
				"UNCAccessOnPublicComputersEnabled",
				"WSSAccessOnPublicComputersEnabled",
				"DirectFileAccessOnPrivateComputersEnabled",
				"WebReadyDocumentViewingOnPrivateComputersEnabled",
				"ForceWebReadyDocumentViewingFirstOnPrivateComputers",
				"UNCAccessOnPrivateComputersEnabled",
				"WSSAccessOnPrivateComputersEnabled",
				"WebReadyDocumentViewingForAllSupportedTypes",
				"WebReadyFileTypes",
				"WebReadyDocumentViewingSupportedFileTypes",
				"WebReadyMimeTypes",
				"WebReadyDocumentViewingSupportedMimeTypes",
				"ActionForUnknownFileAndMIMETypes",
				"AllowedFileTypes",
				"AllowedMimeTypes",
				"BlockedFileTypes",
				"BlockedMimeTypes",
				"ForceSaveFileTypes",
				"ForceSaveMimeTypes"
			};
		}

		protected override void FillProperties(Type type, PSObject psObject, object dummyObject, IList<string> properties)
		{
			ConfigurableObject configurableObject = dummyObject as ConfigurableObject;
			if (configurableObject == null)
			{
				throw new ArgumentException("This method only support ConfigurableObject; please override this method if not this type!");
			}
			this.FillProperty(type, psObject, configurableObject, "DistinguishedName");
			base.FillProperties(type, psObject, dummyObject, properties);
		}
	}
}
