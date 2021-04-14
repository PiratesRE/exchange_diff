using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal abstract class RuleActionMoveCopyData : RuleActionData
	{
		public RuleActionMoveCopyData()
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public byte[] FolderEntryID { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public byte[] StoreEntryID { get; set; }

		public RuleActionMoveCopyData(RuleAction.MoveCopy ruleAction) : base(ruleAction)
		{
			this.FolderEntryID = ruleAction.FolderEntryID;
			this.StoreEntryID = ruleAction.StoreEntryID;
		}

		public static RuleActionMoveCopyData ConvertToUplevel(RuleActionMoveCopyData downlevelRA, bool folderIsLocal)
		{
			RuleActionMoveData ruleActionMoveData = downlevelRA as RuleActionMoveData;
			if (ruleActionMoveData != null)
			{
				if (folderIsLocal)
				{
					return new RuleActionInMailboxMoveData
					{
						FolderEntryID = ruleActionMoveData.FolderEntryID
					};
				}
				return new RuleActionExternalMoveData
				{
					FolderEntryID = ruleActionMoveData.FolderEntryID,
					StoreEntryID = ruleActionMoveData.StoreEntryID
				};
			}
			else
			{
				RuleActionCopyData ruleActionCopyData = downlevelRA as RuleActionCopyData;
				if (folderIsLocal)
				{
					return new RuleActionInMailboxCopyData
					{
						FolderEntryID = ruleActionCopyData.FolderEntryID
					};
				}
				return new RuleActionExternalCopyData
				{
					FolderEntryID = ruleActionCopyData.FolderEntryID,
					StoreEntryID = ruleActionCopyData.StoreEntryID
				};
			}
		}

		public static RuleActionMoveCopyData ConvertToDownlevel(RuleActionMoveCopyData uplevelRA)
		{
			RuleActionInMailboxMoveCopyData ruleActionInMailboxMoveCopyData = uplevelRA as RuleActionInMailboxMoveCopyData;
			if (ruleActionInMailboxMoveCopyData != null)
			{
				RuleActionInMailboxMoveData ruleActionInMailboxMoveData = ruleActionInMailboxMoveCopyData as RuleActionInMailboxMoveData;
				if (ruleActionInMailboxMoveData != null)
				{
					return new RuleActionMoveData
					{
						FolderEntryID = ruleActionInMailboxMoveData.FolderEntryID,
						StoreEntryID = RuleAction.MoveCopy.InThisStoreBytes
					};
				}
				RuleActionInMailboxCopyData ruleActionInMailboxCopyData = ruleActionInMailboxMoveCopyData as RuleActionInMailboxCopyData;
				return new RuleActionCopyData
				{
					FolderEntryID = ruleActionInMailboxCopyData.FolderEntryID,
					StoreEntryID = RuleAction.MoveCopy.InThisStoreBytes
				};
			}
			else
			{
				RuleActionExternalMoveCopyData ruleActionExternalMoveCopyData = uplevelRA as RuleActionExternalMoveCopyData;
				RuleActionExternalMoveData ruleActionExternalMoveData = ruleActionExternalMoveCopyData as RuleActionExternalMoveData;
				if (ruleActionExternalMoveData != null)
				{
					return new RuleActionMoveData
					{
						FolderEntryID = ruleActionExternalMoveData.FolderEntryID,
						StoreEntryID = ruleActionExternalMoveData.StoreEntryID
					};
				}
				RuleActionExternalCopyData ruleActionExternalCopyData = ruleActionExternalMoveCopyData as RuleActionExternalCopyData;
				return new RuleActionCopyData
				{
					FolderEntryID = ruleActionExternalCopyData.FolderEntryID,
					StoreEntryID = ruleActionExternalCopyData.StoreEntryID
				};
			}
		}

		protected override void EnumPropValuesInternal(CommonUtils.EnumPropValueDelegate del)
		{
			base.EnumPropValuesInternal(del);
			PropValueData propValueData = new PropValueData(PropTag.RuleFolderEntryID, this.FolderEntryID);
			del(propValueData);
			this.FolderEntryID = (byte[])propValueData.Value;
		}

		protected override string ToStringInternal()
		{
			return string.Format("FolderEID:{0}, StoreEID:{1}", TraceUtils.DumpEntryId(this.FolderEntryID), TraceUtils.DumpEntryId(this.StoreEntryID));
		}
	}
}
