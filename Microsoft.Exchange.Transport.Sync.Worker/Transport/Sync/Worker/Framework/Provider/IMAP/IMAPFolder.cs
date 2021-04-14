using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPFolder
	{
		internal IMAPFolder(IMAPMailbox mailbox)
		{
			this.Mailbox = mailbox;
		}

		internal long? NumberOfMessages
		{
			get
			{
				return this.numberOfMessages;
			}
			set
			{
				this.numberOfMessages = value;
				this.changed = true;
			}
		}

		internal long? NewNumberOfMessages
		{
			get
			{
				return this.newNumberOfMessages;
			}
			set
			{
				this.newNumberOfMessages = value;
				this.changed = true;
			}
		}

		internal long Uniqueness
		{
			get
			{
				if (this.uniqueness == null)
				{
					Random random = new Random(ExDateTime.Now.Millisecond);
					int num = random.Next();
					num ^= this.Name.GetHashCode();
					this.uniqueness = new long?((long)((ulong)Math.Abs(num)));
					this.changed = true;
				}
				return this.uniqueness.Value;
			}
			set
			{
				this.uniqueness = new long?(value);
				this.changed = true;
			}
		}

		internal long? LowSyncValue
		{
			get
			{
				return this.lowSyncValue;
			}
			set
			{
				this.lowSyncValue = value;
				this.changed = true;
			}
		}

		internal long NewLowSyncValue
		{
			get
			{
				if (this.newLowSyncValue == null)
				{
					return (long)((ulong)-1);
				}
				return this.newLowSyncValue.Value;
			}
			set
			{
				this.newLowSyncValue = new long?(value);
				this.changed = true;
			}
		}

		internal long? HighSyncValue
		{
			get
			{
				return this.highSyncValue;
			}
			set
			{
				this.highSyncValue = value;
				this.changed = true;
			}
		}

		internal long NewHighSyncValue
		{
			get
			{
				if (this.newHighSyncValue == null)
				{
					return 0L;
				}
				return this.newHighSyncValue.Value;
			}
			set
			{
				this.newHighSyncValue = new long?(value);
				this.changed = true;
			}
		}

		internal long? ValidityValue
		{
			get
			{
				return this.validityValue;
			}
			set
			{
				if (this.validityValue != null && value != this.validityValue)
				{
					this.lowSyncValue = null;
					this.highSyncValue = null;
					this.newLowSyncValue = null;
					this.newHighSyncValue = null;
				}
				this.validityValue = value;
				this.changed = true;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal IMAPMailbox Mailbox
		{
			get
			{
				return this.imapMailbox;
			}
			set
			{
				this.imapMailbox = value;
				this.name = this.imapMailbox.Name;
			}
		}

		internal string CloudId
		{
			get
			{
				return this.name;
			}
		}

		internal bool Visited
		{
			get
			{
				return this.visited;
			}
			set
			{
				this.visited = value;
			}
		}

		internal bool HasCloudVersionChanged
		{
			get
			{
				return this.changed;
			}
		}

		internal static string GetShortFolderName(IMAPFolder folder, string fullFolderName)
		{
			string result;
			if (!string.IsNullOrEmpty(fullFolderName))
			{
				char? c = (folder == null) ? new char?(IMAPFolder.DefaultHierarchySeparator) : folder.Mailbox.Separator;
				if (c != null)
				{
					int num = fullFolderName.LastIndexOf(c.Value);
					if (num >= 0)
					{
						IMAPUtils.FromModifiedUTF7(fullFolderName.Substring(num + 1), out result);
						return result;
					}
				}
			}
			IMAPUtils.FromModifiedUTF7(fullFolderName, out result);
			return result;
		}

		internal static string CreateNewMailboxName(char separator, SyncFolder syncFolder, string parentCloudFolderId)
		{
			string result = null;
			if (syncFolder != null && syncFolder.DisplayName != null)
			{
				string text = IMAPUtils.ToModifiedUTF7(syncFolder.DisplayName);
				if (parentCloudFolderId == null || parentCloudFolderId.Equals(IMAPFolder.RootCloudFolderId))
				{
					if (text.LastIndexOf(separator) < 0)
					{
						result = text;
					}
				}
				else
				{
					result = parentCloudFolderId + separator + text;
				}
			}
			return result;
		}

		internal bool TryGetParentCloudFolderId(char separator, string cloudId, out string parentFolderName)
		{
			if (cloudId == null)
			{
				cloudId = this.CloudId;
			}
			int num = cloudId.LastIndexOf(separator);
			if (num > 0)
			{
				parentFolderName = cloudId.Substring(0, num);
				return true;
			}
			parentFolderName = null;
			return false;
		}

		internal void RenameMailboxName(string newName)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newName", newName);
			this.Mailbox.Rename(newName);
			this.name = newName;
		}

		internal void ReparentMailboxName(string oldParent, string newParent)
		{
			string newName = newParent + this.Name.Substring(oldParent.Length);
			this.RenameMailboxName(newName);
		}

		internal bool IsChildOfCloudFolder(string potentialParentCloudFolderId)
		{
			char? separator = this.Mailbox.Separator;
			char? c = separator;
			int? num = (c != null) ? new int?((int)c.GetValueOrDefault()) : null;
			return num != null && this.CloudId.StartsWith(potentialParentCloudFolderId + separator, StringComparison.OrdinalIgnoreCase);
		}

		internal bool UpdateFolderCloudVersion(bool transientFailures)
		{
			bool result = false;
			if (!transientFailures)
			{
				if (this.newLowSyncValue != null && this.newHighSyncValue != null)
				{
					if (this.lowSyncValue == null)
					{
						this.lowSyncValue = new long?(this.newLowSyncValue.Value);
					}
					if (this.highSyncValue == null)
					{
						this.highSyncValue = new long?(this.newHighSyncValue.Value);
					}
					if (this.newLowSyncValue.Value > this.highSyncValue.Value || this.newHighSyncValue.Value < this.lowSyncValue.Value)
					{
						this.highSyncValue = new long?(this.newHighSyncValue.Value);
						this.lowSyncValue = new long?(this.newLowSyncValue.Value);
						result = true;
					}
					else
					{
						this.highSyncValue = new long?(Math.Max(this.highSyncValue.Value, this.newHighSyncValue.Value));
						this.lowSyncValue = new long?(Math.Min(this.lowSyncValue.Value, this.newLowSyncValue.Value));
					}
				}
				if (this.lowSyncValue == 1L)
				{
					this.numberOfMessages = this.newNumberOfMessages;
				}
			}
			return result;
		}

		internal string GenerateFolderCloudVersion()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4}", new object[]
			{
				this.Uniqueness,
				(this.validityValue != null) ? this.validityValue.Value.ToString(CultureInfo.InvariantCulture) : "NIL",
				(this.lowSyncValue != null) ? this.lowSyncValue.Value.ToString(CultureInfo.InvariantCulture) : "NIL",
				(this.highSyncValue != null) ? this.highSyncValue.Value.ToString(CultureInfo.InvariantCulture) : "NIL",
				(this.numberOfMessages != null) ? this.numberOfMessages.Value.ToString(CultureInfo.InvariantCulture) : "NIL"
			});
		}

		internal void InitializeFromCloudFolder(string cloudId, string cloudVersion)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("cloudId", cloudId);
			if (this.name != null && this.name != cloudId)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid cloudId, does not match mailbox name: [CloudId={0}] [MailboxName={1}]", new object[]
				{
					cloudId,
					this.name
				}));
			}
			this.name = cloudId;
			this.uniqueness = null;
			this.validityValue = null;
			this.lowSyncValue = null;
			this.highSyncValue = null;
			this.numberOfMessages = null;
			if (cloudVersion != null && cloudVersion != "DeletedFolder")
			{
				string[] array = cloudVersion.Split(new char[]
				{
					' '
				});
				if (array.Length != 4 && array.Length != 5)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid cloudVersion, could not parse: {0}", new object[]
					{
						cloudVersion
					}));
				}
				if (array[0] != "NIL")
				{
					this.uniqueness = new long?((long)((ulong)uint.Parse(array[0], CultureInfo.InvariantCulture)));
				}
				if (array[1] != "NIL")
				{
					this.validityValue = new long?((long)((ulong)uint.Parse(array[1], CultureInfo.InvariantCulture)));
				}
				if (array[2] != "NIL")
				{
					this.lowSyncValue = new long?((long)((ulong)uint.Parse(array[2], CultureInfo.InvariantCulture)));
				}
				if (array[3] != "NIL")
				{
					this.highSyncValue = new long?((long)((ulong)uint.Parse(array[3], CultureInfo.InvariantCulture)));
				}
				if (array.Length == 5 && array[4] != "NIL")
				{
					this.numberOfMessages = new long?((long)((ulong)uint.Parse(array[4], CultureInfo.InvariantCulture)));
				}
			}
			this.changed = false;
		}

		internal const string InboxCloudFolderId = "INBOX";

		internal const string NilValue = "NIL";

		internal const string DeletedCloudVersionSentinel = "DeletedFolder";

		private const char TokenDelimiter = ' ';

		internal static readonly char DefaultHierarchySeparator = '/';

		internal static readonly string RootCloudFolderId = "&&ROOT";

		private IMAPMailbox imapMailbox;

		private long? uniqueness;

		private long? lowSyncValue;

		private long? highSyncValue;

		private long? newLowSyncValue;

		private long? newHighSyncValue;

		private long? validityValue;

		private string name;

		private bool visited;

		private bool changed;

		private long? numberOfMessages;

		private long? newNumberOfMessages;
	}
}
