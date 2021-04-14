using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Win32;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal static class UserContacts
	{
		public static string[] GetEmailAddresses(MailboxSession itemStore)
		{
			return UserContacts.GetEmailAddressesFromContacts(itemStore);
		}

		private static string[] GetEmailAddressesFromContacts(MailboxSession itemStore)
		{
			List<string> list = new List<string>();
			PropertyDefinition[] dataColumns = new PropertyDefinition[]
			{
				ItemSchema.Id,
				ContactSchema.Email1EmailAddress,
				ContactSchema.Email2EmailAddress,
				ContactSchema.Email3EmailAddress,
				StoreObjectSchema.ItemClass,
				ContactSchema.Email1AddrType,
				ContactSchema.Email2AddrType,
				ContactSchema.Email3AddrType
			};
			List<StoreObjectId> contactsFolders = UserContacts.GetContactsFolders(itemStore);
			foreach (StoreObjectId storeObjectId in contactsFolders)
			{
				try
				{
					using (Folder folder = Folder.Bind(itemStore, storeObjectId))
					{
						using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, dataColumns))
						{
							for (;;)
							{
								object[][] rows = queryResult.GetRows(100);
								if (rows.GetLength(0) == 0)
								{
									break;
								}
								foreach (object[] array2 in rows)
								{
									string text = array2[4] as string;
									if (!string.IsNullOrEmpty(text))
									{
										if (ObjectClass.IsContact(text))
										{
											int num = 5;
											int j = 1;
											while (j <= 3)
											{
												string text2 = array2[j] as string;
												string a = array2[num] as string;
												if (!(a == "EX") && !string.IsNullOrEmpty(text2))
												{
													string text3 = text2.Trim();
													if (text3.Length > 0)
													{
														list.Add(text3);
													}
												}
												j++;
												num++;
											}
										}
										else if (ObjectClass.IsDistributionList(text))
										{
											VersionedId versionedId = array2[0] as VersionedId;
											if (versionedId != null)
											{
												Participant[] array3 = DistributionList.ExpandDeep(itemStore, versionedId.ObjectId);
												foreach (Participant participant in array3)
												{
													if (!(participant.RoutingType == "EX") && !string.IsNullOrEmpty(participant.EmailAddress))
													{
														string text4 = participant.EmailAddress.Trim();
														if (text4.Length > 0)
														{
															list.Add(text4);
														}
													}
												}
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
					UserContacts.Tracer.TraceDebug<IExchangePrincipal, StoreObjectId, ObjectNotFoundException>(0L, "Mailbox:{0}: Exception while binding or query folder={0}, while mining emails for known contacts, exception={2}", itemStore.MailboxOwner, storeObjectId, arg);
				}
			}
			return UserContacts.SortAndRemoveDuplicates(list.ToArray(), itemStore);
		}

		private static List<StoreObjectId> GetContactsFolders(MailboxSession itemStore)
		{
			List<StoreObjectId> list = new List<StoreObjectId>();
			PropertyDefinition[] dataColumns = new PropertyDefinition[]
			{
				FolderSchema.Id,
				StoreObjectSchema.ContainerClass,
				FolderSchema.FolderHierarchyDepth
			};
			int num = -1;
			bool flag = false;
			using (Folder folder = Folder.Bind(itemStore, itemStore.GetDefaultFolderId(DefaultFolderType.Root)))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, dataColumns))
				{
					for (;;)
					{
						object[][] rows = queryResult.GetRows(100);
						if (rows.GetLength(0) == 0)
						{
							break;
						}
						foreach (object[] array2 in rows)
						{
							string text = array2[1] as string;
							int num2 = (int)array2[2];
							VersionedId versionedId = array2[0] as VersionedId;
							if (versionedId != null && !string.IsNullOrEmpty(text))
							{
								StoreObjectId objectId = versionedId.ObjectId;
								if (!flag)
								{
									if (num != -1)
									{
										if (num2 != num)
										{
											goto IL_E4;
										}
										flag = true;
										num = -1;
									}
									else if (objectId.Equals(itemStore.GetDefaultFolderId(DefaultFolderType.DeletedItems)))
									{
										num = num2;
										goto IL_E4;
									}
								}
								if (ObjectClass.IsContactsFolder(text))
								{
									list.Add(objectId);
								}
							}
							IL_E4:;
						}
					}
				}
			}
			return list;
		}

		private static string[] SortAndRemoveDuplicates(string[] emailAddresses, MailboxSession itemStore)
		{
			if (emailAddresses.Length <= 1)
			{
				return emailAddresses;
			}
			Array.Sort<string>(emailAddresses, StringComparer.OrdinalIgnoreCase);
			string[] array = new string[emailAddresses.Length];
			array[0] = emailAddresses[0];
			int num = 1;
			for (int i = 1; i < emailAddresses.Length; i++)
			{
				if (string.Compare(emailAddresses[i], emailAddresses[i - 1], StringComparison.OrdinalIgnoreCase) != 0)
				{
					array[num] = emailAddresses[i];
					num++;
				}
			}
			int num2 = 2000;
			try
			{
				object value = Registry.GetValue(UserContacts.OOFRregistryKeyPath, UserContacts.maxContactsName, 2000);
				if (value != null && value is int)
				{
					num2 = (int)value;
				}
				else
				{
					num2 = 2000;
				}
			}
			catch (IOException)
			{
			}
			catch (SecurityException)
			{
			}
			if (num > num2)
			{
				UserContacts.Tracer.TraceDebug<int, int>(0L, "Contact list length, exceeding max {0}, trucating to {1}", num, num2);
				Globals.OOFLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_OOFTooManyContacts, itemStore.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), new object[]
				{
					itemStore.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(),
					num2.ToString()
				});
				num = num2;
				Array.Resize<string>(ref array, num);
				return array;
			}
			if (num == emailAddresses.Length)
			{
				return emailAddresses;
			}
			Array.Resize<string>(ref array, num);
			return array;
		}

		private const int ExpectedAverageEmailAddressLength = 20;

		private const int MaxNumberOfContacts = 2000;

		private static readonly string OOFRregistryKeyPath = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Services\\MSExchangeMailboxAssistants\\OOF";

		private static readonly string maxContactsName = "MaxContacts";

		private static readonly Trace Tracer = ExTraceGlobals.OOFTracer;
	}
}
