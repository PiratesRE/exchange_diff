using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderIdTranslator
	{
		public FolderIdTranslator(FolderHierarchy sourceHierarchy, FolderHierarchy targetHierarchy)
		{
			this.sourceHierarchy = sourceHierarchy;
			this.targetHierarchy = targetHierarchy;
		}

		public byte[] TranslateFolderId(byte[] sourceEntryId)
		{
			if (sourceEntryId == null)
			{
				return null;
			}
			FolderMapping folderMapping = (FolderMapping)this.sourceHierarchy[sourceEntryId];
			if (folderMapping == null || folderMapping.TargetFolder == null)
			{
				return null;
			}
			return folderMapping.TargetFolder.EntryId;
		}

		public byte[] TranslateTargetFolderId(byte[] targetEntryId)
		{
			if (targetEntryId == null)
			{
				return null;
			}
			FolderMapping folderMapping = (FolderMapping)this.targetHierarchy[targetEntryId];
			if (folderMapping == null)
			{
				return null;
			}
			byte[] array = folderMapping.FolderRec[this.targetHierarchy.SourceEntryIDPtag] as byte[];
			if (array != null)
			{
				return array;
			}
			if (folderMapping.SourceFolder == null)
			{
				return null;
			}
			return folderMapping.SourceFolder.EntryId;
		}

		public byte[][] TranslateFolderIds(byte[][] sourceFolderIds)
		{
			if (sourceFolderIds == null)
			{
				return null;
			}
			byte[][] array = new byte[sourceFolderIds.Length][];
			for (int i = 0; i < sourceFolderIds.Length; i++)
			{
				array[i] = (this.TranslateFolderId(sourceFolderIds[i]) ?? sourceFolderIds[i]);
			}
			return array;
		}

		public void TranslateRestriction(RestrictionData r)
		{
			if (r != null)
			{
				r.EnumeratePropValues(new CommonUtils.EnumPropValueDelegate(this.TranslatePropValue));
			}
		}

		public void TranslateRules(RuleData[] ra)
		{
			if (ra != null)
			{
				foreach (RuleData ruleData in ra)
				{
					ruleData.Enumerate(null, new CommonUtils.EnumPropValueDelegate(this.TranslatePropValue), null);
				}
			}
		}

		private void TranslatePropValue(PropValueData pvd)
		{
			PropTag propTag = (PropTag)pvd.PropTag;
			if (propTag != PropTag.ParentEntryId)
			{
				if (propTag == PropTag.ReplyTemplateID)
				{
					pvd.Value = null;
					return;
				}
				if (propTag != PropTag.RuleFolderEntryID)
				{
					return;
				}
			}
			byte[] array = pvd.Value as byte[];
			if (array != null)
			{
				pvd.Value = (this.TranslateFolderId(array) ?? array);
			}
		}

		private FolderHierarchy sourceHierarchy;

		private FolderHierarchy targetHierarchy;
	}
}
