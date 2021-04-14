using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MapiAclTableAdapter
	{
		internal MapiAclTableAdapter(IModifyTable modifyTable)
		{
			Util.ThrowOnNullArgument(modifyTable, "modifyTable");
			this.modifyTable = modifyTable;
		}

		internal StoreSession Session
		{
			get
			{
				return this.modifyTable.Session;
			}
		}

		internal AclTableEntry[] GetAll()
		{
			List<AclTableEntry> list = new List<AclTableEntry>();
			using (IQueryResult queryResult = this.modifyTable.GetQueryResult(null, MapiAclTableAdapter.PropertiesToRead))
			{
				bool flag;
				do
				{
					object[][] rows = queryResult.GetRows(int.MaxValue, out flag);
					foreach (object[] row in rows)
					{
						list.Add(MapiAclTableAdapter.LoadFromRawData(row));
					}
				}
				while (flag);
			}
			this.allEntriesCached = list.ToArray();
			return list.ToArray();
		}

		internal AclTableEntry GetByMemberId(long memberId)
		{
			if (this.allEntriesCached == null)
			{
				this.GetAll();
			}
			foreach (AclTableEntry aclTableEntry in this.allEntriesCached)
			{
				if (aclTableEntry.MemberId == memberId)
				{
					return aclTableEntry;
				}
			}
			return null;
		}

		internal ADParticipantEntryId TryGetParticipantEntryId(byte[] memberEntryId)
		{
			if (memberEntryId != null)
			{
				return ParticipantEntryId.TryFromEntryId(memberEntryId) as ADParticipantEntryId;
			}
			return null;
		}

		internal ExternalUser TryGetExternalUser(byte[] memberEntryId, ref ExternalUserCollection externalUsers)
		{
			if (memberEntryId != null)
			{
				MailboxSession mailboxSession = this.Session as MailboxSession;
				if (externalUsers == null && mailboxSession != null)
				{
					externalUsers = mailboxSession.GetExternalUsers();
				}
				if (externalUsers != null)
				{
					try
					{
						byte[] binaryForm = null;
						StoreSession session = this.Session;
						bool flag = false;
						try
						{
							if (session != null)
							{
								session.BeginMapiCall();
								session.BeginServerHealthCall();
								flag = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							binaryForm = MapiStore.GetLocalDirectorySIDFromAddressBookEntryId(memberEntryId);
						}
						catch (MapiPermanentException ex)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.InvalidPermissionsEntry, ex, session, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("ACL table has invalid entry id.", new object[0]),
								ex
							});
						}
						catch (MapiRetryableException ex2)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.InvalidPermissionsEntry, ex2, session, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("ACL table has invalid entry id.", new object[0]),
								ex2
							});
						}
						finally
						{
							try
							{
								if (session != null)
								{
									session.EndMapiCall();
									if (flag)
									{
										session.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
						SecurityIdentifier sid = new SecurityIdentifier(binaryForm, 0);
						return externalUsers.FindExternalUser(sid);
					}
					catch (ObjectNotFoundException)
					{
					}
				}
			}
			return null;
		}

		internal void AddPermissionEntry(byte[] memberEntryId, MemberRights memberRights)
		{
			this.modifyTable.AddRow(new PropValue[]
			{
				new PropValue(PermissionSchema.MemberEntryId, memberEntryId),
				new PropValue(PermissionSchema.MemberRights, memberRights)
			});
		}

		internal void ModifyPermissionEntry(long memberId, MemberRights memberRights)
		{
			this.modifyTable.ModifyRow(new PropValue[]
			{
				new PropValue(PermissionSchema.MemberId, memberId),
				new PropValue(PermissionSchema.MemberRights, memberRights)
			});
		}

		internal void RemovePermissionEntry(long memberId)
		{
			this.modifyTable.RemoveRow(new PropValue[]
			{
				new PropValue(PermissionSchema.MemberId, memberId)
			});
		}

		internal void ApplyPendingChanges(bool suppressRestriction)
		{
			if (suppressRestriction)
			{
				this.modifyTable.SuppressRestriction();
			}
			this.modifyTable.ApplyPendingChanges();
		}

		private static AclTableEntry LoadFromRawData(object[] row)
		{
			return new AclTableEntry(PropertyBag.CheckPropertyValue<long>(InternalSchema.MemberId, row[0]), PropertyBag.CheckPropertyValue<byte[]>(InternalSchema.MemberEntryId, row[1], null), PropertyBag.CheckPropertyValue<string>(InternalSchema.MemberName, row[2], string.Empty), PropertyBag.CheckPropertyValue<MemberRights>(InternalSchema.MemberRights, row[3], MemberRights.None));
		}

		private static readonly PropertyDefinition[] PropertiesToRead = new PropertyDefinition[]
		{
			InternalSchema.MemberId,
			InternalSchema.MemberEntryId,
			InternalSchema.MemberName,
			InternalSchema.MemberRights
		};

		private readonly IModifyTable modifyTable;

		private AclTableEntry[] allEntriesCached;

		private enum PropertiesToReadIndex
		{
			MemberId,
			MemberEntryId,
			MemberName,
			MemberRights
		}
	}
}
