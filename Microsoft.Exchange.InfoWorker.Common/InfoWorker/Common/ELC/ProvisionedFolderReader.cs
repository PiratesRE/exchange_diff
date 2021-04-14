using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal static class ProvisionedFolderReader
	{
		internal static void GetProvisionedFoldersFromMailbox(MailboxSession mbxSession, bool orgOnly, out MailboxFolderData elcRootFolderData, out List<MailboxFolderData> provisionedFolders)
		{
			provisionedFolders = null;
			elcRootFolderData = null;
			string text = mbxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			try
			{
				using (Folder folder = Folder.Bind(mbxSession, DefaultFolderType.Root, ProvisionedFolderReader.ElcFolderProps))
				{
					object[] properties = folder.GetProperties(ProvisionedFolderReader.ElcFolderProps);
					MailboxFolderData folderData = ProvisionedFolderReader.GetFolderData(properties, text);
					if (folderData != null)
					{
						ProvisionedFolderReader.Tracer.TraceDebug<string>(0L, "Mailbox '{0}' has the All-Others policy", text);
						provisionedFolders = new List<MailboxFolderData>();
						provisionedFolders.Add(folderData);
					}
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, ProvisionedFolderReader.ElcFolderProps))
					{
						queryResult.SeekToOffset(SeekReference.OriginBeginning, 0);
						if (queryResult.EstimatedRowCount < 1)
						{
							ProvisionedFolderReader.Tracer.TraceDebug<string>(0L, "'{0}': No folders exist in the mailbox.", text);
						}
						else
						{
							for (;;)
							{
								object[][] rows = queryResult.GetRows(100);
								if (rows.Length <= 0)
								{
									break;
								}
								for (int i = 0; i < rows.GetLength(0); i++)
								{
									object[] propValues = rows[i];
									MailboxFolderData folderData2 = ProvisionedFolderReader.GetFolderData(propValues, text);
									if (folderData2 != null)
									{
										if ((folderData2.Flags & (ELCFolderFlags.Provisioned | ELCFolderFlags.Protected | ELCFolderFlags.ELCRoot)) == (ELCFolderFlags.Provisioned | ELCFolderFlags.Protected | ELCFolderFlags.ELCRoot))
										{
											elcRootFolderData = folderData2;
										}
										else if (orgOnly && !folderData2.IsOrganizationalFolder())
										{
											ProvisionedFolderReader.Tracer.TraceDebug<string, string>(0L, "'{0}': Skipping default provisioned folder '{1}' in mailbox because the caller has requested org-only.", text, folderData2.Name);
										}
										else
										{
											if (provisionedFolders == null)
											{
												provisionedFolders = new List<MailboxFolderData>();
											}
											provisionedFolders.Add(folderData2);
											ProvisionedFolderReader.Tracer.TraceDebug<string, string>(0L, "'{0}': has provisioned folder '{1}'", text, folderData2.Name);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (ObjectNotFoundException arg)
			{
				ProvisionedFolderReader.Tracer.TraceError<string, ObjectNotFoundException>(0L, "'{0}': Unable to get the list of provisioned folders from the mailbox - error '{1}'.", text, arg);
				elcRootFolderData = null;
				provisionedFolders = null;
			}
		}

		internal static void GetElcRootFolderInfo(MailboxSession mailboxSession, out StoreObjectId elcRootId, out string elcRootFolderName, out string elcHomePageUrl)
		{
			string arg = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			elcRootId = null;
			elcRootFolderName = null;
			elcHomePageUrl = null;
			PropertyDefinition[] propsToReturn = new PropertyDefinition[]
			{
				FolderSchema.Id,
				StoreObjectSchema.DisplayName,
				FolderSchema.FolderHomePageUrl
			};
			try
			{
				using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.ElcRoot, propsToReturn))
				{
					elcRootId = folder.Id.ObjectId;
					elcRootFolderName = folder.DisplayName;
					elcHomePageUrl = (folder.TryGetProperty(FolderSchema.FolderHomePageUrl) as string);
					ProvisionedFolderReader.Tracer.TraceDebug<string, string, string>(0L, "'{0}': ELC Root Folder exists. Name: '{1}', HomePageUrl: '{2}'", arg, elcRootFolderName, elcHomePageUrl);
				}
			}
			catch (ObjectNotFoundException)
			{
				ProvisionedFolderReader.Tracer.TraceDebug<string>(0L, "'{0}': ELC Root Folder does not exist.", arg);
			}
		}

		private static MailboxFolderData GetFolderData(object[] propValues, string mailbox)
		{
			MailboxFolderData mailboxFolderData = null;
			if (!(propValues[3] is PropertyError) || !(propValues[5] is PropertyError))
			{
				ELCFolderFlags flags = ELCFolderFlags.None;
				if (!(propValues[5] is PropertyError) && propValues[5] is int)
				{
					flags = (ELCFolderFlags)propValues[5];
				}
				mailboxFolderData = new MailboxFolderData();
				mailboxFolderData.Flags = flags;
				if (propValues[3] is string)
				{
					string g = (string)propValues[3];
					Guid elcFolderGuid;
					if (!GuidHelper.TryParseGuid(g, out elcFolderGuid))
					{
						ProvisionedFolderReader.Tracer.TraceDebug<string, string>(0L, "'{0}': has a badly formatted ELC PolicyId string on provisioned folder '{1}'", mailbox, mailboxFolderData.Name);
					}
					else
					{
						mailboxFolderData.ElcFolderGuid = elcFolderGuid;
					}
				}
				if (!(propValues[4] is PropertyError) && propValues[4] is string)
				{
					mailboxFolderData.Comment = (string)propValues[4];
				}
				if (!(propValues[0] is PropertyError) && propValues[0] is VersionedId)
				{
					mailboxFolderData.Id = (VersionedId)propValues[0];
				}
				if (!(propValues[1] is PropertyError) && propValues[1] is StoreObjectId)
				{
					mailboxFolderData.ParentId = (StoreObjectId)propValues[1];
				}
				if (!(propValues[2] is PropertyError) && propValues[2] is string)
				{
					mailboxFolderData.Name = (string)propValues[2];
				}
				if (!(propValues[6] is PropertyError) && propValues[6] is int)
				{
					mailboxFolderData.FolderQuota = (int)propValues[6];
				}
				if (!(propValues[7] is PropertyError) && propValues[7] is string)
				{
					mailboxFolderData.Url = (string)propValues[7];
				}
				if (!(propValues[8] is PropertyError) && propValues[8] is string)
				{
					mailboxFolderData.LocalizedName = (string)propValues[8];
				}
			}
			return mailboxFolderData;
		}

		internal static readonly PropertyDefinition[] ElcFolderProps = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.ParentItemId,
			StoreObjectSchema.DisplayName,
			FolderSchema.ELCPolicyIds,
			FolderSchema.ELCFolderComment,
			FolderSchema.AdminFolderFlags,
			FolderSchema.FolderQuota,
			FolderSchema.FolderHomePageUrl,
			FolderSchema.ElcFolderLocalizedName
		};

		private static readonly Trace Tracer = ExTraceGlobals.ELCTracer;

		internal enum FolderPropIndex
		{
			indexFolderId,
			indexParentId,
			indexDisplayName,
			indexELCFolderId,
			indexELCFolderComment,
			indexAdminFlags,
			indexFolderQuota,
			indexFolderHomePage,
			indexLocalizedName
		}
	}
}
