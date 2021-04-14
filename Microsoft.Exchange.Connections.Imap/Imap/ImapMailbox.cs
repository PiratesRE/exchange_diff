using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class ImapMailbox
	{
		public ImapMailbox(string nameOnTheWire)
		{
			this.IsWritable = true;
			this.NameOnTheWire = nameOnTheWire;
			this.Name = nameOnTheWire;
			this.IsSelectable = true;
		}

		public string Name { get; set; }

		public string NameOnTheWire { get; private set; }

		public char? Separator { get; set; }

		public bool IsSelectable { get; set; }

		public bool? HasChildren { get; set; }

		public bool NoInferiors { get; set; }

		public bool IsWritable { get; set; }

		public ImapMailFlags PermanentFlags { get; set; }

		public long? UidValidity { get; set; }

		public long? UidNext { get; set; }

		public int? NumberOfMessages { get; set; }

		public string ParentFolderPath
		{
			get
			{
				string text = null;
				int lastIndexOfSeparator = this.GetLastIndexOfSeparator();
				if (lastIndexOfSeparator >= 0)
				{
					text = this.Name.Substring(0, lastIndexOfSeparator);
				}
				ImapUtilities.FromModifiedUTF7(text, out text);
				return text;
			}
		}

		public string ShortFolderName
		{
			get
			{
				string text = this.Name;
				int lastIndexOfSeparator = this.GetLastIndexOfSeparator();
				if (lastIndexOfSeparator >= 0)
				{
					text = this.Name.Substring(lastIndexOfSeparator + 1);
				}
				ImapUtilities.FromModifiedUTF7(text, out text);
				return text;
			}
		}

		internal static void EnsureDefaultFolderMappingTable(ILog log = null)
		{
			if (ImapMailbox.hasCreatedMappingTable)
			{
				return;
			}
			lock (ImapMailbox.MappingTableLock)
			{
				if (ImapMailbox.hasCreatedMappingTable)
				{
					return;
				}
				ImapMailbox.BuildDefaultFolderMappings();
				ImapMailbox.hasCreatedMappingTable = true;
			}
			if (log != null)
			{
				foreach (KeyValuePair<string, KeyedPair<string, ImapDefaultFolderType>> keyValuePair in ImapMailbox.preferredDefaultFolderMappings)
				{
					log.Debug("Preferred Mapping: {0} => {1}", new object[]
					{
						keyValuePair.Key,
						keyValuePair.Value.Second
					});
				}
				foreach (KeyValuePair<string, ImapDefaultFolderType> keyValuePair2 in ImapMailbox.secondaryDefaultFolderMappings)
				{
					log.Debug("Default Mapping: {0} => {1}", new object[]
					{
						keyValuePair2.Key,
						keyValuePair2.Value
					});
				}
			}
		}

		internal static ImapDefaultFolderType GetDefaultFolderType(string mailboxName, out bool preferredMapping, out bool exactCaseSensitiveMatch)
		{
			ImapDefaultFolderType result = ImapDefaultFolderType.None;
			preferredMapping = false;
			exactCaseSensitiveMatch = false;
			KeyedPair<string, ImapDefaultFolderType> keyedPair;
			if (ImapMailbox.preferredDefaultFolderMappings.TryGetValue(mailboxName, out keyedPair))
			{
				preferredMapping = true;
				result = keyedPair.Second;
				exactCaseSensitiveMatch = (0 == string.Compare(mailboxName, keyedPair.First, StringComparison.Ordinal));
			}
			else
			{
				ImapMailbox.secondaryDefaultFolderMappings.TryGetValue(mailboxName, out result);
			}
			return result;
		}

		internal static ImapDefaultFolderType GetDefaultFolderType(string mailboxName)
		{
			bool flag = false;
			bool flag2 = false;
			return ImapMailbox.GetDefaultFolderType(mailboxName, out flag, out flag2);
		}

		internal void Rename(string newName)
		{
			this.Name = newName;
		}

		private static void BuildDefaultFolderMappings()
		{
			ImapMailbox.preferredDefaultFolderMappings = new Dictionary<string, KeyedPair<string, ImapDefaultFolderType>>(6, StringComparer.OrdinalIgnoreCase);
			ImapMailbox.secondaryDefaultFolderMappings = new Dictionary<string, ImapDefaultFolderType>(14, StringComparer.OrdinalIgnoreCase);
			ImapMailbox.preferredDefaultFolderMappings.Add(ImapMailbox.Inbox, new KeyedPair<string, ImapDefaultFolderType>(ImapMailbox.Inbox, ImapDefaultFolderType.Inbox));
			ImapMailbox.AddDefaultMappings();
			CultureInfo[] installedLanguagePackCultures = LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.Server);
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				CultureInfo[] array = installedLanguagePackCultures;
				int i = 0;
				while (i < array.Length)
				{
					CultureInfo currentUICulture = array[i];
					try
					{
						Thread.CurrentThread.CurrentUICulture = currentUICulture;
					}
					catch (NotSupportedException)
					{
						goto IL_73;
					}
					goto IL_6E;
					IL_73:
					i++;
					continue;
					IL_6E:
					ImapMailbox.AddDefaultMappings();
					goto IL_73;
				}
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		private static void AddPreferredMapping(string folderNameUTF7, ImapDefaultFolderType type)
		{
			ImapMailbox.preferredDefaultFolderMappings[folderNameUTF7] = new KeyedPair<string, ImapDefaultFolderType>(folderNameUTF7, type);
		}

		private static void AddDefaultMappings()
		{
			ImapMailbox.AddPreferredMapping(ImapUtilities.ToModifiedUTF7(CXStrings.ImapDeletedItems), ImapDefaultFolderType.DeletedItems);
			ImapMailbox.secondaryDefaultFolderMappings[ImapUtilities.ToModifiedUTF7(CXStrings.ImapDeletedMessages)] = ImapDefaultFolderType.DeletedItems;
			ImapMailbox.secondaryDefaultFolderMappings[ImapUtilities.ToModifiedUTF7(CXStrings.ImapTrash)] = ImapDefaultFolderType.DeletedItems;
			ImapMailbox.secondaryDefaultFolderMappings[ImapMailbox.InboxPrefix + ImapUtilities.ToModifiedUTF7(CXStrings.ImapTrash)] = ImapDefaultFolderType.DeletedItems;
			ImapMailbox.secondaryDefaultFolderMappings["[Gmail]/" + ImapUtilities.ToModifiedUTF7(CXStrings.ImapTrash)] = ImapDefaultFolderType.DeletedItems;
			ImapMailbox.AddPreferredMapping(ImapUtilities.ToModifiedUTF7(CXStrings.ImapDrafts), ImapDefaultFolderType.Drafts);
			ImapMailbox.secondaryDefaultFolderMappings[ImapUtilities.ToModifiedUTF7(CXStrings.ImapDraft)] = ImapDefaultFolderType.Drafts;
			ImapMailbox.secondaryDefaultFolderMappings[ImapMailbox.InboxPrefix + ImapUtilities.ToModifiedUTF7(CXStrings.ImapDrafts)] = ImapDefaultFolderType.Drafts;
			ImapMailbox.secondaryDefaultFolderMappings["[Gmail]/" + ImapUtilities.ToModifiedUTF7(CXStrings.ImapDrafts)] = ImapDefaultFolderType.Drafts;
			ImapMailbox.AddPreferredMapping(ImapUtilities.ToModifiedUTF7(CXStrings.ImapJunkEmail), ImapDefaultFolderType.JunkEmail);
			ImapMailbox.AddPreferredMapping("[Gmail]/" + ImapUtilities.ToModifiedUTF7(CXStrings.ImapSpam), ImapDefaultFolderType.JunkEmail);
			ImapMailbox.secondaryDefaultFolderMappings[ImapUtilities.ToModifiedUTF7(CXStrings.ImapSpam)] = ImapDefaultFolderType.JunkEmail;
			ImapMailbox.secondaryDefaultFolderMappings[ImapUtilities.ToModifiedUTF7(CXStrings.ImapJunk)] = ImapDefaultFolderType.JunkEmail;
			ImapMailbox.secondaryDefaultFolderMappings["[Gmail]/" + ImapUtilities.ToModifiedUTF7(CXStrings.ImapAllMail)] = ImapDefaultFolderType.JunkEmail;
			ImapMailbox.AddPreferredMapping(ImapUtilities.ToModifiedUTF7(CXStrings.ImapSentItems), ImapDefaultFolderType.SentItems);
			ImapMailbox.secondaryDefaultFolderMappings[ImapUtilities.ToModifiedUTF7(CXStrings.ImapSentMessages)] = ImapDefaultFolderType.SentItems;
			ImapMailbox.secondaryDefaultFolderMappings[ImapUtilities.ToModifiedUTF7(CXStrings.ImapSent)] = ImapDefaultFolderType.SentItems;
			ImapMailbox.secondaryDefaultFolderMappings[ImapMailbox.InboxPrefix + ImapUtilities.ToModifiedUTF7(CXStrings.ImapSentItems)] = ImapDefaultFolderType.SentItems;
			ImapMailbox.secondaryDefaultFolderMappings[ImapMailbox.InboxPrefix + ImapUtilities.ToModifiedUTF7(CXStrings.ImapSent)] = ImapDefaultFolderType.SentItems;
			ImapMailbox.secondaryDefaultFolderMappings["[Gmail]/" + ImapUtilities.ToModifiedUTF7(CXStrings.ImapSentMail)] = ImapDefaultFolderType.SentItems;
		}

		private int GetLastIndexOfSeparator()
		{
			int result = -1;
			if (!string.IsNullOrEmpty(this.Name))
			{
				char value = (this.Separator != null) ? this.Separator.Value : '/';
				result = this.Name.LastIndexOf(value);
			}
			return result;
		}

		private const int NumPreferredMappings = 6;

		private const int NumSecondaryMappings = 14;

		private const string GmailPrefix = "[Gmail]/";

		internal static readonly string Inbox = "INBOX";

		private static readonly string InboxPrefix = ImapMailbox.Inbox + ".";

		private static readonly object MappingTableLock = new object();

		private static Dictionary<string, KeyedPair<string, ImapDefaultFolderType>> preferredDefaultFolderMappings;

		private static Dictionary<string, ImapDefaultFolderType> secondaryDefaultFolderMappings;

		private static bool hasCreatedMappingTable = false;
	}
}
