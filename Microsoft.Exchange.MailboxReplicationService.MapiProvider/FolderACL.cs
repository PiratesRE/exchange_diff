using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderACL
	{
		public FolderACL(PropValueData[][] aclData)
		{
			this.aces = new List<FolderACL.FolderACE>();
			this.defaultACE = null;
			this.anonymousACE = null;
			this.byEntryId = new EntryIdMap<FolderACL.FolderACE>();
			if (aclData != null)
			{
				int i = 0;
				while (i < aclData.Length)
				{
					PropValueData[] aceData = aclData[i];
					FolderACL.FolderACE folderACE = new FolderACL.FolderACE(aceData);
					switch (folderACE.AceType)
					{
					case FolderACEType.Invalid:
						IL_8F:
						i++;
						continue;
					case FolderACEType.Regular:
						this.byEntryId[folderACE.MemberEntryId] = folderACE;
						break;
					case FolderACEType.Default:
						this.defaultACE = folderACE;
						break;
					case FolderACEType.Anonymous:
						this.anonymousACE = folderACE;
						break;
					}
					this.aces.Add(folderACE);
					goto IL_8F;
				}
			}
		}

		public List<FolderACL.FolderACE> Aces
		{
			get
			{
				return this.aces;
			}
		}

		public FolderACL.FolderACE DefaultACE
		{
			get
			{
				return this.defaultACE;
			}
		}

		public FolderACL.FolderACE AnonymousACE
		{
			get
			{
				return this.anonymousACE;
			}
		}

		public EntryIdMap<FolderACL.FolderACE> ByEntryId
		{
			get
			{
				return this.byEntryId;
			}
		}

		public FolderACL.FolderACE FindMatchingACE(FolderACL.FolderACE ace)
		{
			switch (ace.AceType)
			{
			case FolderACEType.Regular:
			{
				FolderACL.FolderACE result;
				if (this.ByEntryId.TryGetValue(ace.MemberEntryId, out result))
				{
					return result;
				}
				break;
			}
			case FolderACEType.Default:
				return this.DefaultACE;
			case FolderACEType.Anonymous:
				return this.AnonymousACE;
			}
			return null;
		}

		public RowEntry[] UpdateExisting(PropValueData[][] existingAclData)
		{
			FolderACL folderACL = new FolderACL(existingAclData);
			List<RowEntry> list = new List<RowEntry>();
			foreach (FolderACL.FolderACE folderACE in folderACL.Aces)
			{
				if (this.FindMatchingACE(folderACE) == null)
				{
					list.Add(FolderACL.FolderACE.Remove(folderACE.MemberId));
				}
			}
			foreach (FolderACL.FolderACE folderACE2 in this.Aces)
			{
				FolderACL.FolderACE folderACE3 = folderACL.FindMatchingACE(folderACE2);
				if (folderACE3 != null)
				{
					if (folderACE2.MemberRights != folderACE3.MemberRights)
					{
						list.Add(FolderACL.FolderACE.Update(folderACE3.MemberId, folderACE2.MemberRights));
					}
				}
				else
				{
					list.Add(FolderACL.FolderACE.Add(folderACE2.MemberEntryId, folderACE2.MemberRights));
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list.ToArray();
		}

		private List<FolderACL.FolderACE> aces;

		private FolderACL.FolderACE defaultACE;

		private FolderACL.FolderACE anonymousACE;

		private EntryIdMap<FolderACL.FolderACE> byEntryId;

		public class FolderACE
		{
			public FolderACE(PropValueData[] aceData)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				if (aceData != null)
				{
					foreach (PropValueData propValueData in aceData)
					{
						PropTag propTag = (PropTag)propValueData.PropTag;
						if (propTag != PropTag.EntryId)
						{
							if (propTag != PropTag.MemberId)
							{
								if (propTag == PropTag.MemberRights)
								{
									this.memberRights = (int)propValueData.Value;
									flag3 = true;
								}
							}
							else
							{
								this.memberId = (long)propValueData.Value;
								flag2 = true;
							}
						}
						else
						{
							this.memberEntryId = (propValueData.Value as byte[]);
							flag = true;
						}
					}
				}
				if (!flag2 || !flag || !flag3)
				{
					this.aceType = FolderACEType.Invalid;
					return;
				}
				if (this.memberId == -1L)
				{
					this.aceType = FolderACEType.Anonymous;
					return;
				}
				if (this.memberEntryId == null || this.memberEntryId.Length == 0)
				{
					this.aceType = FolderACEType.Default;
					return;
				}
				this.aceType = FolderACEType.Regular;
			}

			public FolderACEType AceType
			{
				get
				{
					return this.aceType;
				}
			}

			public long MemberId
			{
				get
				{
					return this.memberId;
				}
			}

			public byte[] MemberEntryId
			{
				get
				{
					return this.memberEntryId;
				}
			}

			public int MemberRights
			{
				get
				{
					return this.memberRights;
				}
			}

			public static RowEntry Update(long memberId, int rights)
			{
				PropValue[] propValues = new PropValue[]
				{
					new PropValue(PropTag.MemberId, memberId),
					new PropValue(PropTag.MemberRights, rights)
				};
				return RowEntry.Modify(propValues);
			}

			public static RowEntry Remove(long memberId)
			{
				PropValue[] propValues = new PropValue[]
				{
					new PropValue(PropTag.MemberId, memberId)
				};
				return RowEntry.Remove(propValues);
			}

			public static RowEntry Add(byte[] memberEntryId, int rights)
			{
				PropValue[] propValues = new PropValue[]
				{
					new PropValue(PropTag.EntryId, memberEntryId),
					new PropValue(PropTag.MemberRights, rights)
				};
				return RowEntry.Add(propValues);
			}

			private FolderACEType aceType;

			private long memberId;

			private byte[] memberEntryId;

			private int memberRights;
		}
	}
}
