using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DelegateUser : IEquatable<DelegateUser>
	{
		public static DelegateUser Create(IExchangePrincipal delegatePrincipal, IDictionary<DefaultFolderType, PermissionLevel> permissions)
		{
			if (delegatePrincipal == null)
			{
				throw new ArgumentNullException("delegatePrincipal");
			}
			if (delegatePrincipal.ObjectId.IsNullOrEmpty())
			{
				throw new ArgumentException("Incomplete ExchangePrincipal not valid for delegate", "delegatePrincipal");
			}
			if (permissions == null)
			{
				throw new ArgumentNullException("permissions");
			}
			foreach (PermissionLevel permissionLevel in permissions.Values)
			{
				if (!DelegateUser.DelegatePermissionsDictionary.IsSupportedPermissionLevel(permissionLevel))
				{
					throw new ArgumentException("The PermissionLevel is not valid for a DelegateUser", permissionLevel.ToString());
				}
			}
			return DelegateUser.InternalCreate(delegatePrincipal, permissions);
		}

		internal static DelegateUser InternalCreate(IExchangePrincipal delegatePrincipal, IDictionary<DefaultFolderType, PermissionLevel> permissions)
		{
			if (permissions == null)
			{
				throw new ArgumentNullException("permissions");
			}
			DelegateUser delegateUser = new DelegateUser(delegatePrincipal, new DelegateUser.DelegatePermissionsDictionary(permissions));
			if (delegatePrincipal != null)
			{
				delegateUser.name = delegatePrincipal.MailboxInfo.DisplayName;
			}
			return delegateUser;
		}

		internal static DelegateUser InternalCreate(string displayName, string primarySmtpAddress, IDictionary<DefaultFolderType, PermissionLevel> permissions)
		{
			if (permissions == null)
			{
				throw new ArgumentNullException("permissions cannot be null");
			}
			if (string.IsNullOrEmpty(primarySmtpAddress))
			{
				throw new ArgumentNullException("primarySmtpAddress cannot be null or empty");
			}
			if (string.IsNullOrEmpty(displayName))
			{
				throw new ArgumentNullException("displayName cannot be null or empty");
			}
			return new DelegateUser(null, new DelegateUser.DelegatePermissionsDictionary(permissions))
			{
				name = displayName,
				primarySmtpAddress = primarySmtpAddress
			};
		}

		private DelegateUser(IExchangePrincipal delegatePrincipal, DelegateUser.DelegatePermissionsDictionary permissions)
		{
			this.delegatePrincipal = delegatePrincipal;
			this.permissions = permissions;
		}

		public IExchangePrincipal Delegate
		{
			get
			{
				return this.delegatePrincipal;
			}
		}

		public string PrimarySmtpAddress
		{
			get
			{
				if (this.delegatePrincipal == null)
				{
					return this.primarySmtpAddress;
				}
				return this.delegatePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			internal set
			{
				this.name = value;
			}
		}

		public bool ReceivesMeetingMessageCopies
		{
			get
			{
				return this.receivesMeetingMessageCopies;
			}
			set
			{
				this.receivesMeetingMessageCopies = value;
			}
		}

		public bool CanViewPrivateItems
		{
			get
			{
				return this.canViewPrivateItems;
			}
			set
			{
				this.canViewPrivateItems = value;
			}
		}

		public IDictionary<DefaultFolderType, PermissionLevel> FolderPermissions
		{
			get
			{
				return this.permissions;
			}
		}

		public DelegateProblems Problems
		{
			get
			{
				return this.problems;
			}
			internal set
			{
				this.problems = value;
			}
		}

		public bool Equals(DelegateUser other)
		{
			if (other == null)
			{
				return false;
			}
			if (other.Delegate == null)
			{
				return this.Delegate == null;
			}
			if (this.Delegate == null)
			{
				return false;
			}
			if (other.Problems == DelegateProblems.NoADUser || this.Problems == DelegateProblems.NoADUser)
			{
				return other.Problems == this.Problems && this.LegacyDistinguishedName != null && other.LegacyDistinguishedName != null && this.LegacyDistinguishedName.Equals(other.LegacyDistinguishedName, StringComparison.OrdinalIgnoreCase);
			}
			return this.Delegate.LegacyDn.Equals(other.Delegate.LegacyDn, StringComparison.OrdinalIgnoreCase);
		}

		public ADRecipient ADRecipient
		{
			get
			{
				return this.adRecipient;
			}
			set
			{
				this.adRecipient = value;
			}
		}

		internal string LegacyDistinguishedName
		{
			get
			{
				return this.legacyDistinguishedName;
			}
			set
			{
				this.legacyDistinguishedName = value;
			}
		}

		internal int Flags2
		{
			get
			{
				return this.flags2;
			}
			set
			{
				this.flags2 = value;
			}
		}

		internal bool IsReceiveMeetingMessageCopiesValid
		{
			get
			{
				return !this.ReceivesMeetingMessageCopies || this.permissions.HasEditorCalendarRights;
			}
		}

		public void Validate()
		{
			if (!this.IsReceiveMeetingMessageCopiesValid)
			{
				if ((this.Problems & DelegateProblems.InvalidReceiveMeetingMessageCopies) == DelegateProblems.None)
				{
					ExTraceGlobals.DelegateTracer.TraceError<string>((long)this.GetHashCode(), "DelegateUser::Validate. An error occurred while validating. Delegate {0} isn't an editor on Calendar, so the delegate can't get meeting message copies.", this.Name);
					throw new InvalidReceiveMeetingMessageCopiesException(this.Name);
				}
				ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUser::Validate. ReceivesMeetingMessageCopies = false");
				this.ReceivesMeetingMessageCopies = false;
			}
		}

		private string primarySmtpAddress;

		private readonly IExchangePrincipal delegatePrincipal;

		private string name;

		private string legacyDistinguishedName;

		private ADRecipient adRecipient;

		private readonly DelegateUser.DelegatePermissionsDictionary permissions;

		private bool receivesMeetingMessageCopies;

		private bool canViewPrivateItems;

		private int flags2;

		private DelegateProblems problems;

		internal class DelegatePermissionsDictionary : IDictionary<DefaultFolderType, PermissionLevel>, ICollection<KeyValuePair<DefaultFolderType, PermissionLevel>>, IEnumerable<KeyValuePair<DefaultFolderType, PermissionLevel>>, IEnumerable
		{
			internal DelegatePermissionsDictionary(IDictionary<DefaultFolderType, PermissionLevel> permissions)
			{
				foreach (KeyValuePair<DefaultFolderType, PermissionLevel> keyValuePair in permissions)
				{
					DelegateUser.DelegatePermissionsDictionary.Validate(keyValuePair.Key, keyValuePair.Value);
				}
				this.data = new Dictionary<DefaultFolderType, PermissionLevel>(permissions);
			}

			internal bool HasEditorCalendarRights
			{
				get
				{
					bool flag = this.ContainsKey(DefaultFolderType.Calendar) && this[DefaultFolderType.Calendar] == PermissionLevel.Editor;
					ExTraceGlobals.DelegateTracer.TraceDebug<bool>((long)this.GetHashCode(), "DelegateUser::HasEditorCalendarRights value: {0}", flag);
					return flag;
				}
			}

			private static void Validate(DefaultFolderType key, PermissionLevel value)
			{
				if (!DelegateUser.DelegatePermissionsDictionary.IsSupportedDefaultFolder(key))
				{
					ExTraceGlobals.DelegateTracer.TraceError<DefaultFolderType>(0L, "DelegateUser::Validate The folder is not supported for granting DelegateUser permissions. {0}", key);
					throw new ArgumentException("The folder is not supported for granting DelegateUser permissions.", key.ToString());
				}
			}

			private static bool IsSupportedDefaultFolder(DefaultFolderType folderType)
			{
				foreach (DefaultFolderType defaultFolderType in DelegateUserCollection.Folders)
				{
					if (folderType == defaultFolderType)
					{
						return true;
					}
				}
				return false;
			}

			internal static bool IsSupportedPermissionLevel(PermissionLevel permission)
			{
				return permission == PermissionLevel.None || permission == PermissionLevel.Author || permission == PermissionLevel.Editor || permission == PermissionLevel.Reviewer;
			}

			public void Add(DefaultFolderType key, PermissionLevel value)
			{
				DelegateUser.DelegatePermissionsDictionary.Validate(key, value);
				this.data.Add(key, value);
			}

			public bool ContainsKey(DefaultFolderType key)
			{
				return this.data.ContainsKey(key);
			}

			public void Clear()
			{
				this.data.Clear();
			}

			public int Count
			{
				get
				{
					return this.data.Count;
				}
			}

			public ICollection<DefaultFolderType> Keys
			{
				get
				{
					return this.data.Keys;
				}
			}

			public bool Remove(DefaultFolderType key)
			{
				return this.data.Remove(key);
			}

			public bool TryGetValue(DefaultFolderType key, out PermissionLevel value)
			{
				return this.data.TryGetValue(key, out value);
			}

			public ICollection<PermissionLevel> Values
			{
				get
				{
					return this.data.Values;
				}
			}

			public PermissionLevel this[DefaultFolderType key]
			{
				get
				{
					return this.data[key];
				}
				set
				{
					DelegateUser.DelegatePermissionsDictionary.Validate(key, value);
					this.data[key] = value;
				}
			}

			public void Add(KeyValuePair<DefaultFolderType, PermissionLevel> item)
			{
				DelegateUser.DelegatePermissionsDictionary.Validate(item.Key, item.Value);
				((ICollection<KeyValuePair<DefaultFolderType, PermissionLevel>>)this.data).Add(item);
			}

			public bool Contains(KeyValuePair<DefaultFolderType, PermissionLevel> item)
			{
				return ((ICollection<KeyValuePair<DefaultFolderType, PermissionLevel>>)this.data).Contains(item);
			}

			public void CopyTo(KeyValuePair<DefaultFolderType, PermissionLevel>[] array, int arrayIndex)
			{
				((ICollection<KeyValuePair<DefaultFolderType, PermissionLevel>>)this.data).CopyTo(array, arrayIndex);
			}

			public bool IsReadOnly
			{
				get
				{
					return ((ICollection<KeyValuePair<DefaultFolderType, PermissionLevel>>)this.data).IsReadOnly;
				}
			}

			public bool Remove(KeyValuePair<DefaultFolderType, PermissionLevel> item)
			{
				return ((ICollection<KeyValuePair<DefaultFolderType, PermissionLevel>>)this.data).Remove(item);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.data.GetEnumerator();
			}

			public IEnumerator<KeyValuePair<DefaultFolderType, PermissionLevel>> GetEnumerator()
			{
				return ((IEnumerable<KeyValuePair<DefaultFolderType, PermissionLevel>>)this.data).GetEnumerator();
			}

			private Dictionary<DefaultFolderType, PermissionLevel> data;
		}
	}
}
