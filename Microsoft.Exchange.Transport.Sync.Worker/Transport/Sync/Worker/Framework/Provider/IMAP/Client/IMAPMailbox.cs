using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPMailbox
	{
		internal IMAPMailbox(string nameOnTheWire)
		{
			this.nameOnTheWire = nameOnTheWire;
			this.name = nameOnTheWire;
			this.IsSelectable = true;
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal string NameOnTheWire
		{
			get
			{
				return this.nameOnTheWire;
			}
		}

		internal char? Separator
		{
			get
			{
				return this.separator;
			}
			set
			{
				this.separator = value;
			}
		}

		internal bool IsSelectable
		{
			get
			{
				return this.selectable;
			}
			set
			{
				this.selectable = value;
			}
		}

		internal bool? HasChildren
		{
			get
			{
				return this.hasChildren;
			}
			set
			{
				this.hasChildren = value;
			}
		}

		internal bool NoInferiors
		{
			get
			{
				return this.noInferiors;
			}
			set
			{
				this.noInferiors = value;
			}
		}

		internal bool IsWritable
		{
			get
			{
				return this.writable;
			}
			set
			{
				this.writable = value;
			}
		}

		internal IMAPMailFlags PermanentFlags
		{
			get
			{
				return this.permanentFlags;
			}
			set
			{
				this.permanentFlags = value;
			}
		}

		internal long? UidValidity
		{
			get
			{
				return this.uidValidity;
			}
			set
			{
				this.uidValidity = value;
			}
		}

		internal long? UidNext
		{
			get
			{
				return this.uidNext;
			}
			set
			{
				this.uidNext = value;
			}
		}

		internal int? NumberOfMessages
		{
			get
			{
				return this.numberOfMessages;
			}
			set
			{
				this.numberOfMessages = value;
			}
		}

		internal string ParentFolderPath
		{
			get
			{
				string text = null;
				int lastIndexOfSeparator = this.GetLastIndexOfSeparator();
				if (lastIndexOfSeparator >= 0)
				{
					text = this.Name.Substring(0, lastIndexOfSeparator);
				}
				IMAPUtils.FromModifiedUTF7(text, out text);
				return text;
			}
		}

		internal string ShortFolderName
		{
			get
			{
				string text = this.Name;
				int lastIndexOfSeparator = this.GetLastIndexOfSeparator();
				if (lastIndexOfSeparator >= 0)
				{
					text = this.Name.Substring(lastIndexOfSeparator + 1);
				}
				IMAPUtils.FromModifiedUTF7(text, out text);
				return text;
			}
		}

		internal static void EnsureDefaultFolderMappingTable(SyncLogSession log)
		{
			if (IMAPMailbox.hasCreatedMappingTable)
			{
				return;
			}
			lock (IMAPMailbox.mappingTableLock)
			{
				if (IMAPMailbox.hasCreatedMappingTable)
				{
					return;
				}
				IMAPMailbox.BuildDefaultFolderMappings();
				IMAPMailbox.hasCreatedMappingTable = true;
			}
			if (log != null)
			{
				foreach (KeyValuePair<string, KeyedPair<string, DefaultFolderType>> keyValuePair in IMAPMailbox.preferredDefaultFolderMappings)
				{
					log.LogDebugging((TSLID)870UL, IMAPClient.Tracer, "Preferred Mapping: {0} => {1}", new object[]
					{
						keyValuePair.Key,
						keyValuePair.Value.Second
					});
				}
				foreach (KeyValuePair<string, DefaultFolderType> keyValuePair2 in IMAPMailbox.secondaryDefaultFolderMappings)
				{
					log.LogDebugging((TSLID)871UL, IMAPClient.Tracer, "Default Mapping: {0} => {1}", new object[]
					{
						keyValuePair2.Key,
						keyValuePair2.Value
					});
				}
			}
		}

		internal static DefaultFolderType GetDefaultFolderType(string mailboxName, out bool preferredMapping, out bool exactCaseSensitiveMatch)
		{
			DefaultFolderType result = DefaultFolderType.None;
			preferredMapping = false;
			exactCaseSensitiveMatch = false;
			KeyedPair<string, DefaultFolderType> keyedPair;
			if (IMAPMailbox.preferredDefaultFolderMappings.TryGetValue(mailboxName, out keyedPair))
			{
				preferredMapping = true;
				result = keyedPair.Second;
				exactCaseSensitiveMatch = (0 == string.Compare(mailboxName, keyedPair.First, StringComparison.Ordinal));
			}
			else
			{
				IMAPMailbox.secondaryDefaultFolderMappings.TryGetValue(mailboxName, out result);
			}
			return result;
		}

		internal static DefaultFolderType GetDefaultFolderType(string mailboxName)
		{
			bool flag = false;
			bool flag2 = false;
			return IMAPMailbox.GetDefaultFolderType(mailboxName, out flag, out flag2);
		}

		internal void Rename(string newName)
		{
			this.name = newName;
		}

		private static void BuildDefaultFolderMappings()
		{
			IMAPMailbox.preferredDefaultFolderMappings = new Dictionary<string, KeyedPair<string, DefaultFolderType>>(6, StringComparer.OrdinalIgnoreCase);
			IMAPMailbox.secondaryDefaultFolderMappings = new Dictionary<string, DefaultFolderType>(14, StringComparer.OrdinalIgnoreCase);
			IMAPMailbox.preferredDefaultFolderMappings.Add(IMAPMailbox.Inbox, new KeyedPair<string, DefaultFolderType>(IMAPMailbox.Inbox, DefaultFolderType.Inbox));
			IMAPMailbox.AddDefaultMappings();
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
					IMAPMailbox.AddDefaultMappings();
					goto IL_73;
				}
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		private static void AddPreferredMapping(string folderNameUTF7, DefaultFolderType type)
		{
			IMAPMailbox.preferredDefaultFolderMappings[folderNameUTF7] = new KeyedPair<string, DefaultFolderType>(folderNameUTF7, type);
		}

		private static void AddDefaultMappings()
		{
			IMAPMailbox.AddPreferredMapping(IMAPUtils.ToModifiedUTF7(Strings.IMAPDeletedItems), DefaultFolderType.DeletedItems);
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPUtils.ToModifiedUTF7(Strings.IMAPDeletedMessages)] = DefaultFolderType.DeletedItems;
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPUtils.ToModifiedUTF7(Strings.IMAPTrash)] = DefaultFolderType.DeletedItems;
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPMailbox.InboxPrefix + IMAPUtils.ToModifiedUTF7(Strings.IMAPTrash)] = DefaultFolderType.DeletedItems;
			IMAPMailbox.secondaryDefaultFolderMappings["[Gmail]/" + IMAPUtils.ToModifiedUTF7(Strings.IMAPTrash)] = DefaultFolderType.DeletedItems;
			IMAPMailbox.AddPreferredMapping(IMAPUtils.ToModifiedUTF7(Strings.IMAPDrafts), DefaultFolderType.Drafts);
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPUtils.ToModifiedUTF7(Strings.IMAPDraft)] = DefaultFolderType.Drafts;
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPMailbox.InboxPrefix + IMAPUtils.ToModifiedUTF7(Strings.IMAPDrafts)] = DefaultFolderType.Drafts;
			IMAPMailbox.secondaryDefaultFolderMappings["[Gmail]/" + IMAPUtils.ToModifiedUTF7(Strings.IMAPDrafts)] = DefaultFolderType.Drafts;
			IMAPMailbox.AddPreferredMapping(IMAPUtils.ToModifiedUTF7(Strings.IMAPJunkEmail), DefaultFolderType.JunkEmail);
			IMAPMailbox.AddPreferredMapping("[Gmail]/" + IMAPUtils.ToModifiedUTF7(Strings.IMAPSpam), DefaultFolderType.JunkEmail);
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPUtils.ToModifiedUTF7(Strings.IMAPSpam)] = DefaultFolderType.JunkEmail;
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPUtils.ToModifiedUTF7(Strings.IMAPJunk)] = DefaultFolderType.JunkEmail;
			IMAPMailbox.secondaryDefaultFolderMappings["[Gmail]/" + IMAPUtils.ToModifiedUTF7(Strings.IMAPAllMail)] = DefaultFolderType.JunkEmail;
			IMAPMailbox.AddPreferredMapping(IMAPUtils.ToModifiedUTF7(Strings.IMAPSentItems), DefaultFolderType.SentItems);
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPUtils.ToModifiedUTF7(Strings.IMAPSentMessages)] = DefaultFolderType.SentItems;
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPUtils.ToModifiedUTF7(Strings.IMAPSent)] = DefaultFolderType.SentItems;
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPMailbox.InboxPrefix + IMAPUtils.ToModifiedUTF7(Strings.IMAPSentItems)] = DefaultFolderType.SentItems;
			IMAPMailbox.secondaryDefaultFolderMappings[IMAPMailbox.InboxPrefix + IMAPUtils.ToModifiedUTF7(Strings.IMAPSent)] = DefaultFolderType.SentItems;
			IMAPMailbox.secondaryDefaultFolderMappings["[Gmail]/" + IMAPUtils.ToModifiedUTF7(Strings.IMAPSentMail)] = DefaultFolderType.SentItems;
		}

		private int GetLastIndexOfSeparator()
		{
			int result = -1;
			if (!string.IsNullOrEmpty(this.Name))
			{
				char value = (this.Separator != null) ? this.Separator.Value : IMAPFolder.DefaultHierarchySeparator;
				result = this.Name.LastIndexOf(value);
			}
			return result;
		}

		private const int NumPreferredMappings = 6;

		private const int NumSecondaryMappings = 14;

		private const string GmailPrefix = "[Gmail]/";

		internal static readonly string Inbox = "INBOX";

		private static readonly string InboxPrefix = IMAPMailbox.Inbox + ".";

		private static Dictionary<string, KeyedPair<string, DefaultFolderType>> preferredDefaultFolderMappings;

		private static Dictionary<string, DefaultFolderType> secondaryDefaultFolderMappings;

		private static object mappingTableLock = new object();

		private static bool hasCreatedMappingTable = false;

		private readonly string nameOnTheWire;

		private string name;

		private char? separator;

		private bool selectable;

		private bool? hasChildren;

		private bool noInferiors;

		private bool writable = true;

		private IMAPMailFlags permanentFlags;

		private long? uidValidity;

		private long? uidNext;

		private int? numberOfMessages;
	}
}
