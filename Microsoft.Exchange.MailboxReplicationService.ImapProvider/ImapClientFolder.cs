using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Connections.Imap;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class ImapClientFolder
	{
		public ImapClientFolder(ImapMailbox folder)
		{
			this.folder = folder;
		}

		public ImapClientFolder(string folderName)
		{
			this.folder = new ImapMailbox(folderName);
		}

		public string Name
		{
			get
			{
				return this.folder.Name;
			}
		}

		public bool IsSelectable
		{
			get
			{
				return this.folder.IsSelectable;
			}
			set
			{
				this.folder.IsSelectable = value;
			}
		}

		public ImapDefaultFolderType DefaultFolderType { get; private set; }

		public WellKnownFolderType WellKnownFolderType { get; private set; }

		public char? Separator
		{
			get
			{
				return this.folder.Separator;
			}
		}

		public uint UidNext
		{
			get
			{
				return (uint)this.folder.UidNext.Value;
			}
		}

		public uint UidValidity
		{
			get
			{
				return (uint)this.folder.UidValidity.Value;
			}
		}

		public int? NumberOfMessages
		{
			get
			{
				return this.folder.NumberOfMessages;
			}
		}

		public string ParentFolderPath
		{
			get
			{
				return this.folder.ParentFolderPath;
			}
		}

		public string ShortFolderName
		{
			get
			{
				return this.folder.ShortFolderName;
			}
		}

		public ImapMailFlags SupportedFlags
		{
			get
			{
				return this.folder.PermanentFlags;
			}
		}

		public static void FindWellKnownFolders(List<ImapClientFolder> folders)
		{
			Dictionary<ImapDefaultFolderType, ImapClientFolder> dictionary = new Dictionary<ImapDefaultFolderType, ImapClientFolder>();
			foreach (ImapClientFolder imapClientFolder in folders)
			{
				bool flag;
				bool flag2;
				imapClientFolder.DefaultFolderType = ImapClientFolder.GetDefaultFolderType(imapClientFolder.Name, out flag, out flag2);
				if (imapClientFolder.DefaultFolderType != ImapDefaultFolderType.None && (flag2 || flag) && !dictionary.ContainsKey(imapClientFolder.DefaultFolderType))
				{
					dictionary.Add(imapClientFolder.DefaultFolderType, imapClientFolder);
				}
			}
			foreach (ImapClientFolder imapClientFolder2 in folders)
			{
				imapClientFolder2.WellKnownFolderType = WellKnownFolderType.None;
				if (imapClientFolder2.DefaultFolderType != ImapDefaultFolderType.None && !dictionary.ContainsKey(imapClientFolder2.DefaultFolderType))
				{
					dictionary.Add(imapClientFolder2.DefaultFolderType, imapClientFolder2);
				}
			}
			foreach (KeyValuePair<ImapDefaultFolderType, ImapClientFolder> keyValuePair in dictionary)
			{
				WellKnownFolderType wellKnownFolderType;
				if (ImapClientFolder.DefaultToWellKnownFolderMapping.TryGetValue(keyValuePair.Key, out wellKnownFolderType))
				{
					ImapClientFolder value = keyValuePair.Value;
					value.WellKnownFolderType = wellKnownFolderType;
				}
			}
		}

		public void SelectImapFolder(ImapConnection imapConnection)
		{
			imapConnection.SelectImapMailbox(this.folder);
		}

		public List<ImapMessageRec> LookupMessages(ImapConnection imapConnection, List<uint> uidList)
		{
			this.SelectImapFolder(imapConnection);
			LookupMessagesParams lookupParams = new LookupMessagesParams(uidList);
			IEnumerable<ImapMessageRec> collection = this.LookupMessagesInfoFromImapServer(imapConnection, lookupParams);
			List<ImapMessageRec> list = new List<ImapMessageRec>(collection);
			list.Sort();
			return list;
		}

		public List<ImapMessageRec> EnumerateMessages(ImapConnection imapConnection, FetchMessagesFlags flags, int? highFetchValue = null, int? lowFetchValue = null)
		{
			ImapMailbox imapMailbox = imapConnection.SelectImapMailbox(this.folder);
			int? numberOfMessages = imapMailbox.NumberOfMessages;
			if (numberOfMessages == null || numberOfMessages.Value == 0)
			{
				MrsTracer.Provider.Debug("Imap folder {0} does not have any messages to be enumerated", new object[]
				{
					this.Name
				});
				return new List<ImapMessageRec>(0);
			}
			int highFetchValue2 = highFetchValue ?? numberOfMessages.Value;
			int lowFetchValue2 = lowFetchValue ?? 1;
			EnumerateMessagesParams enumerateParams = new EnumerateMessagesParams(highFetchValue2, lowFetchValue2, flags);
			IEnumerable<ImapMessageRec> collection = this.EnumerateMessagesInfoFromImapServer(imapConnection, enumerateParams);
			List<ImapMessageRec> list = new List<ImapMessageRec>(collection);
			list.Sort();
			return list;
		}

		private static ImapDefaultFolderType GetDefaultFolderType(string folderName, out bool preferredMapping, out bool exactCaseSensitiveMatch)
		{
			return ImapMailbox.GetDefaultFolderType(folderName, out preferredMapping, out exactCaseSensitiveMatch);
		}

		private IEnumerable<ImapMessageRec> EnumerateMessagesInfoFromImapServer(ImapConnection imapConnection, EnumerateMessagesParams enumerateParams)
		{
			if (enumerateParams.LowFetchValue > enumerateParams.HighFetchValue)
			{
				return new List<ImapMessageRec>(0);
			}
			ImapResultData messageInfoByRange = imapConnection.GetMessageInfoByRange(enumerateParams.LowFetchValue.ToString(CultureInfo.InvariantCulture), enumerateParams.HighFetchValue.ToString(CultureInfo.InvariantCulture), enumerateParams.FetchMessagesFlags.HasFlag(FetchMessagesFlags.FetchByUid), enumerateParams.FetchMessagesFlags.HasFlag(FetchMessagesFlags.IncludeExtendedData) ? ImapConnection.MessageInfoDataItemsForNewMessages : ImapConnection.MessageInfoDataItemsForChangesOnly);
			return this.GetImapMessageRecsFromResultData(messageInfoByRange, enumerateParams.FetchMessagesFlags);
		}

		private IEnumerable<ImapMessageRec> LookupMessagesInfoFromImapServer(ImapConnection imapConnection, LookupMessagesParams lookupParams)
		{
			ImapResultData messageInfoByRange = imapConnection.GetMessageInfoByRange(lookupParams.GetUidFetchString(), null, lookupParams.FetchMessagesFlags.HasFlag(FetchMessagesFlags.FetchByUid), lookupParams.FetchMessagesFlags.HasFlag(FetchMessagesFlags.IncludeExtendedData) ? ImapConnection.MessageInfoDataItemsForNewMessages : ImapConnection.MessageInfoDataItemsForChangesOnly);
			return this.GetImapMessageRecsFromResultData(messageInfoByRange, lookupParams.FetchMessagesFlags);
		}

		private IEnumerable<ImapMessageRec> GetImapMessageRecsFromResultData(ImapResultData resultData, FetchMessagesFlags fetchFlags)
		{
			IList<string> messageUids = resultData.MessageUids;
			IList<ImapMailFlags> messageFlags = resultData.MessageFlags;
			bool flag = messageUids != null && messageUids.Count > 0 && messageFlags != null && messageFlags.Count > 0;
			if (!flag)
			{
				return new List<ImapMessageRec>(0);
			}
			if (fetchFlags.HasFlag(FetchMessagesFlags.IncludeExtendedData))
			{
				return ImapExtendedMessageRec.FromImapResultData(this.folder, resultData);
			}
			return ImapMessageRec.FromImapResultData(resultData);
		}

		public static readonly Dictionary<ImapDefaultFolderType, WellKnownFolderType> DefaultToWellKnownFolderMapping = new Dictionary<ImapDefaultFolderType, WellKnownFolderType>
		{
			{
				ImapDefaultFolderType.Inbox,
				WellKnownFolderType.Inbox
			},
			{
				ImapDefaultFolderType.DeletedItems,
				WellKnownFolderType.DeletedItems
			},
			{
				ImapDefaultFolderType.Drafts,
				WellKnownFolderType.Drafts
			},
			{
				ImapDefaultFolderType.SentItems,
				WellKnownFolderType.SentItems
			},
			{
				ImapDefaultFolderType.JunkEmail,
				WellKnownFolderType.JunkEmail
			}
		};

		private readonly ImapMailbox folder;
	}
}
