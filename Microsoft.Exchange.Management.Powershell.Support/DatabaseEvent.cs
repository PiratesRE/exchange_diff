using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public sealed class DatabaseEvent : IConfigurable
	{
		ObjectId IConfigurable.Identity
		{
			get
			{
				return null;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return new ValidationError[0];
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.New;
			}
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			DatabaseEvent databaseEvent = source as DatabaseEvent;
			if (databaseEvent == null)
			{
				throw new NotImplementedException(string.Format("Cannot copy changes from type {0}.", source.GetType()));
			}
			this.mapiEvent = databaseEvent.mapiEvent;
			this.databaseId = databaseEvent.databaseId;
			this.server = databaseEvent.server;
			this.isDatabaseCopyActive = databaseEvent.isDatabaseCopyActive;
		}

		void IConfigurable.ResetChangeTracking()
		{
		}

		public DatabaseEvent()
		{
		}

		internal DatabaseEvent(MapiEvent mapiEvent, DatabaseId databaseId, Server server, bool isDatabaseCopyActive)
		{
			this.Instantiate(mapiEvent, databaseId, server, isDatabaseCopyActive);
		}

		internal void Instantiate(MapiEvent mapiEvent, DatabaseId databaseId, Server server, bool isDatabaseCopyActive)
		{
			if (mapiEvent == null)
			{
				throw new ArgumentNullException("mapiEvent");
			}
			if (null == databaseId)
			{
				throw new ArgumentNullException("databaseId");
			}
			this.mapiEvent = mapiEvent;
			this.databaseId = databaseId;
			this.server = server;
			this.isDatabaseCopyActive = isDatabaseCopyActive;
		}

		public long Counter
		{
			get
			{
				return this.mapiEvent.EventCounter;
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return this.mapiEvent.CreateTime;
			}
		}

		public string ItemType
		{
			get
			{
				return this.mapiEvent.ItemType.ToString();
			}
		}

		public string EventName
		{
			get
			{
				return this.mapiEvent.EventMask.ToString();
			}
		}

		public string Flags
		{
			get
			{
				return this.mapiEvent.EventFlags.ToString();
			}
		}

		public Guid? MailboxGuid
		{
			get
			{
				if (Guid.Empty != this.mapiEvent.MailboxGuid)
				{
					return new Guid?(this.mapiEvent.MailboxGuid);
				}
				return null;
			}
		}

		public string ObjectClass
		{
			get
			{
				return this.mapiEvent.ObjectClass;
			}
		}

		public MapiEntryId ItemEntryId
		{
			get
			{
				byte[] itemEntryId = this.mapiEvent.ItemEntryId;
				if (itemEntryId == null || 0 >= itemEntryId.Length)
				{
					return null;
				}
				return new MapiEntryId(itemEntryId);
			}
		}

		public MapiEntryId ParentEntryId
		{
			get
			{
				byte[] parentEntryId = this.mapiEvent.ParentEntryId;
				if (parentEntryId == null || 0 >= parentEntryId.Length)
				{
					return null;
				}
				return new MapiEntryId(parentEntryId);
			}
		}

		public MapiEntryId OldItemEntryId
		{
			get
			{
				byte[] oldItemEntryId = this.mapiEvent.OldItemEntryId;
				if (oldItemEntryId == null || 0 >= oldItemEntryId.Length)
				{
					return null;
				}
				return new MapiEntryId(oldItemEntryId);
			}
		}

		public MapiEntryId OldParentEntryId
		{
			get
			{
				byte[] oldParentEntryId = this.mapiEvent.OldParentEntryId;
				if (oldParentEntryId == null || 0 >= oldParentEntryId.Length)
				{
					return null;
				}
				return new MapiEntryId(oldParentEntryId);
			}
		}

		public long ItemCount
		{
			get
			{
				return this.mapiEvent.ItemCount;
			}
		}

		public long UnreadItemCount
		{
			get
			{
				return this.mapiEvent.UnreadItemCount;
			}
		}

		public ulong ExtendedFlags
		{
			get
			{
				return (ulong)this.mapiEvent.ExtendedEventFlags;
			}
		}

		public string ClientCategory
		{
			get
			{
				return this.mapiEvent.ClientType.ToString();
			}
		}

		public string PrincipalName
		{
			get
			{
				SecurityIdentifier sid = this.mapiEvent.Sid;
				if (null != sid)
				{
					if (this.principalName == null)
					{
						string arg;
						string arg2;
						if (DatabaseEvent.GetAccountNameAndType(sid, out arg, out arg2))
						{
							this.principalName = string.Format("{0}\\{1}", arg, arg2);
							if (SuppressingPiiContext.NeedPiiSuppression)
							{
								this.principalName = SuppressingPiiData.Redact(this.principalName);
							}
						}
						else
						{
							this.principalName = sid.ToString();
						}
					}
					return this.principalName;
				}
				return null;
			}
		}

		public SecurityIdentifier PrincipalSid
		{
			get
			{
				return this.mapiEvent.Sid;
			}
		}

		public Server Server
		{
			get
			{
				return this.server;
			}
		}

		public DatabaseId Database
		{
			get
			{
				return this.databaseId;
			}
		}

		public bool IsDatabaseCopyActive
		{
			get
			{
				return this.isDatabaseCopyActive;
			}
		}

		public long DocumentId
		{
			get
			{
				return (long)this.mapiEvent.DocumentId;
			}
		}

		public Guid? UnifiedMailboxGuid
		{
			get
			{
				if (Guid.Empty != this.mapiEvent.UnifiedMailboxGuid)
				{
					return new Guid?(this.mapiEvent.UnifiedMailboxGuid);
				}
				return null;
			}
		}

		private static bool GetAccountNameAndType(SecurityIdentifier sid, out string domainName, out string accountName)
		{
			string systemName = null;
			byte[] array = new byte[sid.BinaryLength];
			sid.GetBinaryForm(array, 0);
			uint capacity = 64U;
			uint capacity2 = 64U;
			StringBuilder stringBuilder = new StringBuilder((int)capacity);
			StringBuilder stringBuilder2 = new StringBuilder((int)capacity2);
			int num = 0;
			int num2;
			if (!DatabaseEvent.LookupAccountSid(systemName, array, stringBuilder, ref capacity, stringBuilder2, ref capacity2, out num2) && (num = Marshal.GetLastWin32Error()) == 122)
			{
				stringBuilder = new StringBuilder((int)capacity);
				stringBuilder2 = new StringBuilder((int)capacity2);
				DatabaseEvent.LookupAccountSid(systemName, array, stringBuilder, ref capacity, stringBuilder2, ref capacity2, out num2);
				num = Marshal.GetLastWin32Error();
			}
			if (num == 0)
			{
				domainName = stringBuilder2.ToString();
				accountName = stringBuilder.ToString();
				return true;
			}
			if (num == 8)
			{
				throw MapiExceptionHelper.OutOfMemoryException("LookupAccountSid failure.");
			}
			accountName = "=unknown=";
			domainName = "=unknown=";
			return false;
		}

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "LookupAccountSidW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LookupAccountSid(string systemName, byte[] sid, StringBuilder accountName, ref uint accountNameLength, StringBuilder domainName, ref uint domainNameLength, out int usage);

		private MapiEvent mapiEvent;

		private string principalName;

		private DatabaseId databaseId;

		private Server server;

		private bool isDatabaseCopyActive;
	}
}
