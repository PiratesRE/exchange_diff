using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MyContactsFolderValidation : SearchFolderValidation
	{
		internal MyContactsFolderValidation(ContactsSearchFolderCriteria contactsSearchFolderCriteria) : base(new IValidator[]
		{
			new MatchMapiFolderType(FolderType.Search)
		})
		{
			this.contactsSearchFolderCriteria = contactsSearchFolderCriteria;
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			base.SetPropertiesInternal(context, folder);
			folder.Save();
			SearchFolder searchFolder = (SearchFolder)folder;
			StoreObjectId[] folderScope;
			if (MailboxSession.DefaultFoldersToForceInit != null && MailboxSession.DefaultFoldersToForceInit.Contains(DefaultFolderType.MyContacts))
			{
				folderScope = this.contactsSearchFolderCriteria.GetExistingDefaultFolderScope(context);
			}
			else
			{
				folderScope = this.contactsSearchFolderCriteria.GetDefaultFolderScope(context.Session, true);
			}
			SearchFolderCriteria searchFolderCriteria = ContactsSearchFolderCriteria.CreateSearchCriteria(folderScope);
			ContactsSearchFolderCriteria.ApplyContinuousSearchFolderCriteria(XSOFactory.Default, context.Session, searchFolder, searchFolderCriteria);
			bool flag = context.Session.MailboxOwner.RecipientTypeDetails != RecipientTypeDetails.GroupMailbox || !context.Session.ClientInfoString.Contains("Client=WebServices;Action=ConfigureGroupMailbox");
			MyContactsFolderValidation.Tracer.TraceDebug<string, RecipientTypeDetails, bool>((long)context.Session.GetHashCode(), "SearchFolder criteria applied. ClientInfoString={0}, RecipientTypeDetails={1}, ShouldWaitForNotification={2}", context.Session.ClientInfoString, context.Session.MailboxOwner.RecipientTypeDetails, flag);
			if (flag)
			{
				ContactsSearchFolderCriteria.WaitForSearchFolderPopulation(XSOFactory.Default, context.Session, searchFolder);
			}
			folder.Load(null);
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, StoreObjectId folderId, Dictionary<string, DefaultFolderManager.FolderData> folderDataDictionary)
		{
			if (!base.EnsureIsValid(context, folderId, folderDataDictionary))
			{
				return false;
			}
			string idString = Convert.ToBase64String(folderId.ProviderLevelItemId);
			return base.BindAndValidateFolder(context, folderId, idString);
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			if (!base.EnsureIsValid(context, folder))
			{
				return false;
			}
			SearchFolder searchFolder = folder as SearchFolder;
			if (searchFolder == null)
			{
				return false;
			}
			if (SearchFolderValidation.TryGetSearchCriteria(searchFolder) == null)
			{
				this.SetPropertiesInternal(context, folder);
			}
			return true;
		}

		private static readonly Trace Tracer = ExTraceGlobals.MyContactsFolderTracer;

		private readonly ContactsSearchFolderCriteria contactsSearchFolderCriteria;
	}
}
