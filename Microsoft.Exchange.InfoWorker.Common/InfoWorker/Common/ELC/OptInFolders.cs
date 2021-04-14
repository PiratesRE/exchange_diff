using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class OptInFolders
	{
		internal OptInFolders(ExchangePrincipal mailboxToAccess, string accessingUser, WindowsPrincipal accessingPrincipal, IBudget ewsBudget)
		{
			if (accessingPrincipal == null)
			{
				throw new ArgumentNullException("accessingPrincipal");
			}
			if (accessingUser == null)
			{
				throw new ArgumentNullException("accessingUser");
			}
			if (mailboxToAccess == null)
			{
				throw new ArgumentNullException("mailboxToAccess");
			}
			this.accessingPrincipal = accessingPrincipal;
			this.accessingUser = accessingUser;
			this.mailboxToAccess = mailboxToAccess;
			this.budget = ewsBudget;
		}

		internal OptInFolders(ExchangePrincipal mailboxToAccess, string accessingUser, ClientSecurityContext clientSecurityContext, IBudget ewsBudget)
		{
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			if (accessingUser == null)
			{
				throw new ArgumentNullException("accessingUser");
			}
			if (mailboxToAccess == null)
			{
				throw new ArgumentNullException("mailboxToAccess");
			}
			this.clientSecurityContext = clientSecurityContext;
			this.accessingUser = accessingUser;
			this.mailboxToAccess = mailboxToAccess;
			this.budget = ewsBudget;
		}

		internal VersionedId[] CreateOrganizationalFolders(string[] folderNames)
		{
			OptInFolders.<>c__DisplayClass2 CS$<>8__locals1 = new OptInFolders.<>c__DisplayClass2();
			CS$<>8__locals1.folderNames = folderNames;
			if (CS$<>8__locals1.folderNames == null)
			{
				throw new ArgumentNullException();
			}
			if (CS$<>8__locals1.folderNames.Length == 0)
			{
				throw new ArgumentException("The number of input folders cannot be zero.");
			}
			if (CS$<>8__locals1.folderNames.Length > 1000)
			{
				throw new ArgumentException("The number of input folders exceeded allowable limit.");
			}
			List<VersionedId> list = new List<VersionedId>();
			int folderIndex;
			for (folderIndex = 0; folderIndex < CS$<>8__locals1.folderNames.GetLength(0); folderIndex++)
			{
				int length = Array.FindAll<string>(CS$<>8__locals1.folderNames, (string matchedFolderName) => string.Compare(CS$<>8__locals1.folderNames[folderIndex], matchedFolderName, StringComparison.OrdinalIgnoreCase) == 0).GetLength(0);
				if (length > 1)
				{
					OptInFolders.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Input folder list contains duplicate entry of '{0}'.", CS$<>8__locals1.folderNames[folderIndex]);
					throw new ELCDuplicateFolderNamesArgumentException(CS$<>8__locals1.folderNames[folderIndex]);
				}
			}
			this.ConfirmCanActAsOwner();
			OptInFolders.Tracer.TraceDebug<string>((long)this.GetHashCode(), "WindowsPrincipal validation for mailbox '{0}' successful.", this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString());
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.mailboxToAccess, CultureInfo.InvariantCulture, "Client=ELC;Action=Create Managed Folders"))
			{
				if (this.budget != null)
				{
					mailboxSession.AccountingObject = this.budget;
				}
				Folder folder = null;
				try
				{
					List<AdFolderData> adFoldersFromFolderNames = this.GetAdFoldersFromFolderNames(CS$<>8__locals1.folderNames);
					MailboxFolderData mailboxFolderData = null;
					OptInFolders.Tracer.TraceDebug<int, string>((long)this.GetHashCode(), "Need to create '{0}' Managed Custom Folders for mailbox '{1}'.", adFoldersFromFolderNames.Count, this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString());
					List<MailboxFolderData> list2;
					ProvisionedFolderReader.GetProvisionedFoldersFromMailbox(mailboxSession, true, out mailboxFolderData, out list2);
					if (list2 != null && list2.Count != 0)
					{
						using (List<AdFolderData>.Enumerator enumerator = adFoldersFromFolderNames.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								AdFolderData folderToCreate = enumerator.Current;
								int num = list2.FindIndex((MailboxFolderData mbxFolder) => mbxFolder.ElcFolderGuid == folderToCreate.Folder.Guid);
								if (num >= 0)
								{
									OptInFolders.Tracer.TraceDebug<string, SmtpAddress>((long)this.GetHashCode(), "Folder '{0}' exists in mailbox '{1}'.", folderToCreate.Folder.FolderName, this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress);
									throw new ELCOrgFolderExistsException(folderToCreate.Folder.FolderName);
								}
							}
						}
					}
					VersionedId versionedId = (mailboxFolderData == null) ? null : mailboxFolderData.Id;
					if (versionedId == null)
					{
						string elcRootUrl = AdFolderReader.GetElcRootUrl(this.budget);
						folder = ProvisionedFolderCreator.CreateELCRootFolder(mailboxSession, elcRootUrl);
						if (folder == null)
						{
							OptInFolders.Tracer.TraceError<SmtpAddress>((long)this.GetHashCode(), "{0}: Unable to create the ELC root folder in the mailbox.", this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress);
							throw new ELCRootFailureException(Strings.descFailedToCreateELCRoot(mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()), null);
						}
						versionedId = folder.Id;
					}
					if (versionedId == null)
					{
						OptInFolders.Tracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "Unable to create the Managed Folders root folder in mailbox '{0}'.", this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress);
						throw new ELCRootFailureException(Strings.descFailedToCreateELCRoot(this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString()), null);
					}
					try
					{
						if (folder == null)
						{
							folder = Folder.Bind(mailboxSession, versionedId);
						}
					}
					catch (ObjectNotFoundException innerException)
					{
						OptInFolders.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Failed to bind to the Managed Folders root folder in mailbox '{0}'.", this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString());
						throw new ELCRootFailureException(Strings.descCannotBindToElcRootFolder(versionedId.ToString(), this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString()), innerException);
					}
					foreach (AdFolderData adFolderInfo in adFoldersFromFolderNames)
					{
						ProvisionedFolderCreator.CreateOneELCFolderInMailbox(adFolderInfo, mailboxSession, folder);
					}
					list = this.GetIdsFromFolderNames(mailboxSession, CS$<>8__locals1.folderNames);
				}
				finally
				{
					if (folder != null)
					{
						folder.Dispose();
						folder = null;
					}
				}
			}
			return list.ToArray();
		}

		private void ConfirmCanActAsOwner()
		{
			if (this.accessingPrincipal != null)
			{
				WindowsIdentity windowsIdentity = this.accessingPrincipal.Identity as WindowsIdentity;
				using (MailboxSession mailboxSession = MailboxSession.OpenWithBestAccess(this.mailboxToAccess, this.accessingUser, this.accessingPrincipal, CultureInfo.InvariantCulture, "Client=ELC;Action=Create Managed Folders"))
				{
					if (this.budget != null)
					{
						mailboxSession.AccountingObject = this.budget;
					}
					if (!mailboxSession.CanActAsOwner)
					{
						OptInFolders.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "The authenticated user does not have owner rights to the mailbox.Authenticated User: {0}, Mailbox: {1}", windowsIdentity.Name, this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString());
						throw new AccessDeniedException(ServerStrings.IsNotMailboxOwner(windowsIdentity.User, this.mailboxToAccess.Sid));
					}
					return;
				}
			}
			using (MailboxSession mailboxSession2 = MailboxSession.OpenWithBestAccess(this.mailboxToAccess, this.accessingUser, this.clientSecurityContext, CultureInfo.InvariantCulture, "Client=ELC;Action=Create Managed Folders"))
			{
				if (this.budget != null)
				{
					mailboxSession2.AccountingObject = this.budget;
				}
				if (!mailboxSession2.CanActAsOwner)
				{
					OptInFolders.Tracer.TraceDebug<SecurityIdentifier, string>((long)this.GetHashCode(), "The impersonated user does not have owner rights to the mailbox.Impersonated User Sid: {0}, Mailbox: {1}", this.clientSecurityContext.UserSid, this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString());
					throw new AccessDeniedException(ServerStrings.IsNotMailboxOwner(this.clientSecurityContext.UserSid, this.mailboxToAccess.Sid));
				}
			}
		}

		private List<AdFolderData> GetAdFoldersFromFolderNames(string[] folderNames)
		{
			OptInFolders.<>c__DisplayClassb CS$<>8__locals1 = new OptInFolders.<>c__DisplayClassb();
			CS$<>8__locals1.folderNames = folderNames;
			List<AdFolderData> list = new List<AdFolderData>();
			List<ELCFolder> allFolders = AdFolderReader.GetAllFolders(true, this.budget);
			int folderIndex;
			for (folderIndex = 0; folderIndex < CS$<>8__locals1.folderNames.GetLength(0); folderIndex++)
			{
				ELCFolder elcfolder = allFolders.Find((ELCFolder matchedFolder) => string.Compare(CS$<>8__locals1.folderNames[folderIndex], matchedFolder.FolderName, StringComparison.OrdinalIgnoreCase) == 0);
				if (elcfolder == null)
				{
					throw new ELCNoMatchingOrgFoldersException(CS$<>8__locals1.folderNames[folderIndex]);
				}
				list.Add(new AdFolderData
				{
					Folder = elcfolder
				});
			}
			return list;
		}

		private List<VersionedId> GetIdsFromFolderNames(MailboxSession mailboxSession, string[] folderNames)
		{
			OptInFolders.<>c__DisplayClass11 CS$<>8__locals1 = new OptInFolders.<>c__DisplayClass11();
			CS$<>8__locals1.folderNames = folderNames;
			List<VersionedId> list = new List<VersionedId>();
			List<MailboxFolderData> list2 = null;
			MailboxFolderData mailboxFolderData = null;
			ProvisionedFolderReader.GetProvisionedFoldersFromMailbox(mailboxSession, true, out mailboxFolderData, out list2);
			if (list2 == null || list2.Count == 0)
			{
				throw new ELCPartialCompletionException(Strings.descFailedToCreateOneOrMoreOrganizationalFolders(Strings.descFailedToGetOrganizationalFoldersForMailbox(this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString())));
			}
			OptInFolders.Tracer.TraceDebug<int, string>((long)this.GetHashCode(), "Retrieved '{0}' Managed Custom Folder(s) for mailbox '{1}'.", list2.Count, this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString());
			int folderIndex;
			for (folderIndex = 0; folderIndex < CS$<>8__locals1.folderNames.GetLength(0); folderIndex++)
			{
				int num = list2.FindIndex((MailboxFolderData matchedFolder) => string.Compare(CS$<>8__locals1.folderNames[folderIndex], matchedFolder.Name, StringComparison.OrdinalIgnoreCase) == 0);
				if (num == -1)
				{
					throw new ELCPartialCompletionException(Strings.descFailedToCreateOneOrMoreOrganizationalFolders(Strings.descCannotFindOrganizationalFolderInMailbox(CS$<>8__locals1.folderNames[folderIndex], this.mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString())));
				}
				list.Add(list2[num].Id);
			}
			return list;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ELCTracer;

		private WindowsPrincipal accessingPrincipal;

		private ClientSecurityContext clientSecurityContext;

		private ExchangePrincipal mailboxToAccess;

		private string accessingUser;

		private IBudget budget;
	}
}
