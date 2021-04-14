using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Utils
	{
		public static string MessageClassFromFileExtension(string extension)
		{
			string text = null;
			Utils.extensionMap.TryGetValue(extension, out text);
			return text ?? "IPM.Document.txtfile";
		}

		public static bool IsValidSharePointFileNameChar(char c)
		{
			if (!Utils.invalidSharePointFileNameCharsSetInitialized)
			{
				lock (Utils.invalidSharePointFileNameCharsSyncObj)
				{
					if (!Utils.invalidSharePointFileNameCharsSetInitialized)
					{
						Utils.invalidSharePointFileNameCharsSet = new HashSet<char>();
						foreach (char item in Utils.InvalidSharePointFileNameChars)
						{
							Utils.invalidSharePointFileNameCharsSet.Add(item);
						}
						Utils.invalidSharePointFileNameCharsSetInitialized = true;
					}
				}
			}
			return !Utils.invalidSharePointFileNameCharsSet.Contains(c);
		}

		public static string FilterInvalidSharePointFileNameChars(string input, int maxLength)
		{
			int num = 0;
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				if (Utils.IsValidSharePointFileNameChar(input[i]))
				{
					if (num + 1 > maxLength)
					{
						break;
					}
					if (input[i] == '.')
					{
						if (stringBuilder.Length == 0)
						{
							goto IL_BB;
						}
						int num2 = i + 1;
						while (num2 < input.Length && (input[num2] == '.' || !Utils.IsValidSharePointFileNameChar(input[num2])))
						{
							num2++;
						}
						if (num2 == input.Length)
						{
							break;
						}
						if (num2 - i == 1)
						{
							if (num + 2 <= maxLength)
							{
								stringBuilder.Append('.');
								stringBuilder.Append(input[num2]);
								num += 2;
								i = num2;
								goto IL_BB;
							}
							break;
						}
						else
						{
							i = num2;
						}
					}
					stringBuilder.Append(input[i]);
					num++;
				}
				IL_BB:;
			}
			if (stringBuilder.Length == 0)
			{
				return string.Empty;
			}
			return stringBuilder.ToString();
		}

		public static Uri GetParentUri(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException("input");
			}
			Uri input2 = new Uri(input);
			return Utils.GetParentUri(input2);
		}

		public static Uri GetParentUri(Uri input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			int num = input.AbsoluteUri.LastIndexOf('/');
			if (num == -1)
			{
				return null;
			}
			return new Uri(input.AbsoluteUri.Substring(0, num));
		}

		public static string GetFileNameFromUri(Uri input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			int num = input.AbsoluteUri.LastIndexOf('/');
			if (num == -1 || num == input.AbsoluteUri.Length - 1)
			{
				return null;
			}
			return input.AbsoluteUri.Substring(num + 1, input.AbsoluteUri.Length - 1 - num);
		}

		public static bool IsDefaultFolderId(StoreSession session, StoreObjectId folderId, DefaultFolderType defaultFolderType)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				return false;
			}
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(defaultFolderType);
			return defaultFolderId != null && folderId.Equals(defaultFolderId);
		}

		public static bool IsTeamMailbox(StoreSession session)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			return mailboxSession != null && mailboxSession.MailboxOwner.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox;
		}

		public static bool IsBlockedTeamMailboxSession(StoreSession session)
		{
			return !string.IsNullOrEmpty(session.ClientInfoString) && (session.ClientInfoString.IndexOf("OWA", StringComparison.OrdinalIgnoreCase) != -1 || session.ClientInfoString.IndexOf("POP3/IMAP4", StringComparison.OrdinalIgnoreCase) != -1 || session.ClientInfoString.IndexOf("ActiveSync", StringComparison.OrdinalIgnoreCase) != -1 || session.ClientInfoString.IndexOf("WebServices", StringComparison.OrdinalIgnoreCase) != -1);
		}

		public static StoreObjectId EnsureSyncIssueFolder(MailboxSession session)
		{
			if (!Utils.IsTeamMailbox(session))
			{
				throw new ArgumentException("session must be associated with a TeamMailbox");
			}
			StoreObjectId storeObjectId = session.GetDefaultFolderId(DefaultFolderType.DocumentSyncIssues);
			if (storeObjectId == null)
			{
				storeObjectId = session.CreateDefaultFolder(DefaultFolderType.DocumentSyncIssues);
			}
			return storeObjectId;
		}

		public static bool HasFolderUriChanged(Folder folder, Uri newFolderUri)
		{
			string valueOrDefault = folder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedUrl, string.Empty);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				return true;
			}
			Uri x = new Uri(valueOrDefault);
			return !UriComparer.IsEqual(x, newFolderUri);
		}

		public static void AddUpdateHistoryEntry(CoreItem message, string source, string user, ExDateTime time)
		{
			string[] valueOrDefault = message.PropertyBag.GetValueOrDefault<string[]>(InternalSchema.LinkedItemUpdateHistory, null);
			List<string> list = (valueOrDefault != null) ? new List<string>(valueOrDefault) : new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(source);
			stringBuilder.Append(";");
			stringBuilder.Append(user);
			stringBuilder.Append(";");
			stringBuilder.Append(time.ToString());
			list.Add(stringBuilder.ToString());
			if (list.Count > 100)
			{
				message.PropertyBag[InternalSchema.LinkedItemUpdateHistory] = list.Skip(list.Count - 100).ToArray<string>();
				return;
			}
			message.PropertyBag[InternalSchema.LinkedItemUpdateHistory] = list.ToArray();
		}

		public static void ReleaseAndCloseMailboxOperationSemaphore(Semaphore semaphore, string name, bool close)
		{
			if (semaphore != null)
			{
				try
				{
					semaphore.Release();
				}
				catch (IOException innerException)
				{
					throw new StorageTransientException(new LocalizedString(string.Format("Failed to release named semaphore for mailbox {0} because of IOException", name)), innerException);
				}
				catch (UnauthorizedAccessException innerException2)
				{
					throw new StoragePermanentException(new LocalizedString(string.Format("Failed to release named semaphore for mailbox {0} because of UnauthorizedAccessException", name)), innerException2);
				}
				catch (SemaphoreFullException)
				{
				}
				finally
				{
					if (close)
					{
						semaphore.Dispose();
					}
				}
			}
		}

		public static void CommitMailboxOperationUnderNamedSemaphore(Guid mailboxGuid, Action action)
		{
			string name = null;
			Semaphore semaphore = null;
			try
			{
				semaphore = Utils.GetMailboxOperationSemaphore(mailboxGuid, out name);
				semaphore.WaitOne(Constants.MailboxSemaphoreTimeout);
				action();
			}
			finally
			{
				Utils.ReleaseAndCloseMailboxOperationSemaphore(semaphore, name, true);
			}
		}

		public static string GetUserAgentStringForSiteMailboxRequests()
		{
			if (!Utils.userAgentStringInitialized)
			{
				lock (Utils.userAgentStringInitLock)
				{
					if (!Utils.userAgentStringInitialized)
					{
						try
						{
							FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Util.GetAssemblyLocation());
							Utils.userAgentString = "Exchange/" + versionInfo.FileVersion + "/SiteMailbox";
						}
						catch (FileNotFoundException)
						{
							Utils.userAgentString = "Exchange/UnknownVersion/SiteMailbox";
						}
					}
					Utils.userAgentStringInitialized = true;
				}
			}
			return Utils.userAgentString;
		}

		public static LinkedItemProps FindFolderBySharePointId(MailboxSession session, Folder rootFolder, Guid sharePointId, PerformanceCounter performanceCounter)
		{
			performanceCounter.Start(OperationType.FolderLookupById);
			Guid? valueAsNullable = rootFolder.PropertyBag.GetValueAsNullable<Guid>(FolderSchema.LinkedId);
			if (valueAsNullable != null && valueAsNullable == sharePointId)
			{
				performanceCounter.Stop(OperationType.FolderLookupById, 1);
				return new LinkedItemProps(rootFolder.Id.ObjectId, rootFolder.ParentId);
			}
			ComparisonFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.LinkedId, sharePointId);
			using (QueryResult queryResult = rootFolder.FolderQuery(FolderQueryFlags.DeepTraversal, queryFilter, null, new PropertyDefinition[]
			{
				FolderSchema.LinkedId,
				FolderSchema.Id,
				StoreObjectSchema.ParentItemId
			}))
			{
				object[][] rows = queryResult.GetRows(1);
				if (rows.Length == 1)
				{
					performanceCounter.Stop(OperationType.FolderLookupById, 1);
					return new LinkedItemProps(((VersionedId)rows[0][1]).ObjectId, (StoreObjectId)rows[0][2]);
				}
			}
			performanceCounter.Stop(OperationType.FolderLookupById, 1);
			return null;
		}

		public static LinkedItemProps FindFolderBySharePointUri(MailboxSession session, Folder rootFolder, Uri sharePointUri, PerformanceCounter performanceCounter)
		{
			performanceCounter.Start(OperationType.FolderLookupByUri);
			string text = rootFolder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedUrl, null);
			if (!string.IsNullOrEmpty(text))
			{
				Uri uri = null;
				try
				{
					uri = new Uri(text);
				}
				catch (UriFormatException)
				{
				}
				if (uri != null && UriComparer.IsEqual(uri, sharePointUri))
				{
					performanceCounter.Stop(OperationType.FolderLookupByUri, 1);
					return new LinkedItemProps(rootFolder.Id.ObjectId, rootFolder.ParentId);
				}
			}
			using (QueryResult queryResult = rootFolder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, new PropertyDefinition[]
			{
				FolderSchema.LinkedUrl,
				FolderSchema.Id,
				StoreObjectSchema.ParentItemId
			}))
			{
				object[][] array = null;
				int i;
				for (;;)
				{
					array = queryResult.GetRows(10000);
					for (i = 0; i < array.Length; i++)
					{
						text = (array[i][0] as string);
						if (!string.IsNullOrEmpty(text))
						{
							Uri x = null;
							try
							{
								x = new Uri(text);
							}
							catch (UriFormatException)
							{
								goto IL_FF;
							}
							if (UriComparer.IsEqual(x, sharePointUri))
							{
								goto Block_11;
							}
						}
						IL_FF:;
					}
					if (array.Length == 0)
					{
						goto Block_13;
					}
				}
				Block_11:
				performanceCounter.Stop(OperationType.FolderLookupByUri, 1);
				return new LinkedItemProps(((VersionedId)array[i][1]).ObjectId, (StoreObjectId)array[i][2]);
				Block_13:;
			}
			performanceCounter.Stop(OperationType.FolderLookupByUri, 1);
			return null;
		}

		public static LinkedItemProps FindItemBySharePointId(MailboxSession session, Guid sharePointId, PerformanceCounter performanceCounter)
		{
			if (performanceCounter != null)
			{
				performanceCounter.Start(OperationType.FileLookupById);
			}
			object[] array = AllItemsFolderHelper.FindItemBySharePointId(session, sharePointId);
			if (array != null)
			{
				return new LinkedItemProps((StoreObjectId)array[0], (StoreObjectId)array[1], (Uri)array[2]);
			}
			if (performanceCounter != null)
			{
				performanceCounter.Stop(OperationType.FileLookupById, 1);
			}
			return null;
		}

		public static void TriggerTestInducedException(ChangedItem item)
		{
			if (item.InducedException != null)
			{
				throw item.InducedException;
			}
		}

		public static EnqueueResult[] TriggerSiteMailboxSync(IExchangePrincipal siteMailbox, string syncClientString, bool syncDocumentOnly)
		{
			List<EnqueueResult> list = new List<EnqueueResult>();
			EnqueueResult enqueueResult = RpcClientWrapper.EnqueueTeamMailboxSyncRequest(siteMailbox.MailboxInfo.Location.ServerFqdn, siteMailbox.MailboxInfo.MailboxGuid, QueueType.TeamMailboxDocumentSync, siteMailbox.MailboxInfo.OrganizationId, syncClientString, null, SyncOption.Default);
			enqueueResult.Type = QueueType.TeamMailboxDocumentSync;
			list.Add(enqueueResult);
			if (!syncDocumentOnly)
			{
				enqueueResult = RpcClientWrapper.EnqueueTeamMailboxSyncRequest(siteMailbox.MailboxInfo.Location.ServerFqdn, siteMailbox.MailboxInfo.MailboxGuid, QueueType.TeamMailboxMembershipSync, siteMailbox.MailboxInfo.OrganizationId, syncClientString, null, SyncOption.Default);
				enqueueResult.Type = QueueType.TeamMailboxMembershipSync;
				list.Add(enqueueResult);
			}
			return list.ToArray();
		}

		public static string GetSyncClientString(string clientName)
		{
			string arg = "Unknown";
			try
			{
				arg = Environment.MachineName;
			}
			catch (InvalidOperationException)
			{
			}
			return string.Format("{0}_{1}", clientName, arg);
		}

		public static Semaphore GetMailboxOperationSemaphore(Guid mailboxGuid, out string name)
		{
			name = "SiteMailboxSyncSemaphore_" + mailboxGuid.ToString();
			Semaphore result;
			try
			{
				SemaphoreSecurity semaphoreSecurity = new SemaphoreSecurity();
				SemaphoreAccessRule rule = new SemaphoreAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), SemaphoreRights.FullControl, AccessControlType.Allow);
				semaphoreSecurity.AddAccessRule(rule);
				bool flag = false;
				result = new Semaphore(1, 1, name, ref flag, semaphoreSecurity);
			}
			catch (IOException innerException)
			{
				throw new StorageTransientException(new LocalizedString(string.Format("Failed to create named semaphore for mailbox {0} because of IOException", name)), innerException);
			}
			catch (UnauthorizedAccessException innerException2)
			{
				throw new StoragePermanentException(new LocalizedString(string.Format("Failed to create named semaphore for mailbox {0} because of UnauthorizedAccessException", name)), innerException2);
			}
			catch (WaitHandleCannotBeOpenedException innerException3)
			{
				throw new StoragePermanentException(new LocalizedString(string.Format("Failed to create named semaphore for mailbox {0} because of WaitHandleCannotBeOpenedException", name)), innerException3);
			}
			return result;
		}

		public const string MsgFileClass = "IPM.Document.msgfile";

		private const int MaxUpdateHistory = 100;

		public static char[] InvalidSharePointFileNameChars = new char[]
		{
			'#',
			'"',
			'%',
			'&',
			'*',
			':',
			'<',
			'>',
			'?',
			'\\',
			'/',
			'{',
			'}',
			'|',
			'~'
		};

		private static bool userAgentStringInitialized = false;

		private static object userAgentStringInitLock = new object();

		private static string userAgentString;

		private static HashSet<char> invalidSharePointFileNameCharsSet;

		private static bool invalidSharePointFileNameCharsSetInitialized = false;

		private static object invalidSharePointFileNameCharsSyncObj = new object();

		private static Dictionary<string, string> extensionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"accdb",
				"IPM.Document.Access.Application.14"
			},
			{
				"accdt",
				"IPM.Document.Access.ACCDTFile.15"
			},
			{
				"accde",
				"IPM.Document.Access.ACCDEFile.15"
			},
			{
				"accdr",
				"IPM.Document.Access.ACCDRFile.15"
			},
			{
				"ascx",
				"IPM.Document.SharePointDesigner.ascx.15.0"
			},
			{
				"aspx",
				"IPM.Document.SharePointDesigner.aspx.15.0"
			},
			{
				"master",
				"IPM.Document.SharePointDesigner.master.15.0"
			},
			{
				"doc",
				"IPM.Document.Word.Document.8"
			},
			{
				"docm",
				"IPM.Document.Word.DocumentMacroEnabled.12"
			},
			{
				"docx",
				"IPM.Document.Word.Document.12"
			},
			{
				"dot",
				"IPM.Document.Word.Template.8"
			},
			{
				"dotm",
				"IPM.Document.Word.TemplateMacroEnabled.12"
			},
			{
				"dotx",
				"IPM.Document.Word.Template.12"
			},
			{
				"odt",
				"IPM.Document.Word.OpenDocumentText.12"
			},
			{
				"mht",
				"IPM.Document.mhtmlfile"
			},
			{
				"mhtml",
				"IPM.Document.*mhtl"
			},
			{
				"mpp",
				"IPM.Document.MSProject.Project.9"
			},
			{
				"mpt",
				"IPM.Document.MSProject.Template"
			},
			{
				"xlsx",
				"IPM.Document.Excel.Sheet.12"
			},
			{
				"xlsm",
				"IPM.Document.Excel.SheetMacroEnabled.12"
			},
			{
				"xlsb",
				"IPM.Document.Excel.SheetBinaryMacroEnabled.12"
			},
			{
				"xltx",
				"IPM.Document.Excel.Template"
			},
			{
				"xltm",
				"IPM.Document.Excel.TemplateMacroEnabled"
			},
			{
				"xlt",
				"IPM.Document.Excel.Template.8"
			},
			{
				"csv",
				"IPM.Document.Excel.CSV"
			},
			{
				"ods",
				"IPM.Document.Excel.OpenDocumentSpreadsheet.12"
			},
			{
				"one",
				"IPM.Document.OneNote.Section.1"
			},
			{
				"onepkg",
				"IPM.Document.OneNote.Package"
			},
			{
				"onetoc2",
				"IPM.Document.OneNote.TableOfContents.12"
			},
			{
				"pot",
				"IPM.Document.PowerPoint.Template.8"
			},
			{
				"potm",
				"IPM.Document.PowerPoint.TemplateMacroEnabled.12"
			},
			{
				"potx",
				"IPM.Document.PowerPoint.Template.12"
			},
			{
				"ppa",
				"IPM.Document.PowerPoint.Addin.8"
			},
			{
				"ppam",
				"IPM.Document.PowerPoint.Addin.12"
			},
			{
				"ppt",
				"IPM.Document.PowerPoint.Show.8"
			},
			{
				"pptm",
				"IPM.Document.PowerPoint.ShowMacroEnabled.12"
			},
			{
				"pptx",
				"IPM.Document.PowerPoint.Show.12"
			},
			{
				"pps",
				"IPM.Document.PowerPoint.SlideShow.8"
			},
			{
				"ppsm",
				"IPM.Document.PowerPoint.SlideShowMacroEnabled.12"
			},
			{
				"ppsx",
				"IPM.Document.PowerPoint.SlideShow.12"
			},
			{
				"odp",
				"IPM.Document.PowerPoint.OpenDocumentPresentation.12"
			},
			{
				"pub",
				"IPM.Document.Publisher.Document.14"
			},
			{
				"vdw",
				"IPM.Document.Visio.WebDrawing.14"
			},
			{
				"vdx",
				"IPM.Document.Visio.Drawing.11"
			},
			{
				"vsd",
				"IPM.Document.Visio.Drawing.11"
			},
			{
				"vss",
				"IPM.Document.Visio.Stencil.11"
			},
			{
				"vst",
				"IPM.Document.Visio.Template.11"
			},
			{
				"vsx",
				"IPM.Document.Visio.Stencil.11"
			},
			{
				"vtx",
				"IPM.Document.Visio.Template.11"
			},
			{
				"vsdx",
				"IPM.Document.Visio.Drawing.15"
			},
			{
				"vsdm",
				"IPM.Document.Visio.DrawingMacroEnabled.15"
			},
			{
				"vssx",
				"IPM.Document.Visio.Stencil.15"
			},
			{
				"vstx",
				"IPM.Document.Visio.Template.15"
			},
			{
				"xsn",
				"IPM.Document.InfoPath.Solution.4"
			},
			{
				"jpg",
				"IPM.Document.jpegfile"
			},
			{
				"txt",
				"IPM.Document.txtfile"
			},
			{
				"htm",
				"IPM.Document.htmlfile"
			},
			{
				"html",
				"IPM.Document.htmlfile"
			},
			{
				"msg",
				"IPM.Document.msgfile"
			},
			{
				"pdf",
				"IPM.Document.AcroExch.Document.7"
			},
			{
				"gif",
				"IPM.Document.giffile"
			},
			{
				"png",
				"IPM.Document.pngfile"
			},
			{
				"tif",
				"IPM.Document.TIFImage.Document"
			},
			{
				"bmp",
				"IPM.Document.Paint.Picture"
			},
			{
				"rtf",
				"IPM.Document.Word.RTF.8"
			},
			{
				"xml",
				"IPM.Document.xmlfile"
			},
			{
				"wmv",
				"IPM.Document.WMP11.AssocFile.WMV"
			},
			{
				"mp4",
				"IPM.Document.WMP11.AssocFile.MP4"
			},
			{
				"xps",
				"IPM.Document.Windows.XPSReachViewer"
			},
			{
				"zip",
				"IPM.Document.CompressedFolder"
			}
		};
	}
}
