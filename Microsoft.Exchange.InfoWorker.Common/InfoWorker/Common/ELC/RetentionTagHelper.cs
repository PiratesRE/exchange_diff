using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RetentionTagHelper
	{
		internal static void ReadPolicyFromFolder(Folder folder, out object policy, out object flags, out object period, out object archivePolicy, out object archivePeriod, out object compactDefaultPolicy)
		{
			policy = folder.GetValueOrDefault<object>(StoreObjectSchema.PolicyTag);
			period = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionPeriod);
			flags = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			archivePolicy = folder.GetValueOrDefault<object>(StoreObjectSchema.ArchiveTag);
			archivePeriod = folder.GetValueOrDefault<object>(StoreObjectSchema.ArchivePeriod);
			compactDefaultPolicy = folder.GetValueOrDefault<object>(FolderSchema.RetentionTagEntryId);
		}

		internal static void ApplyPolicy(MailboxSession mailboxSession, object policy, object flags, object period, object archivePolicy, object archivePeriod, object compactDefaultPolicy, MessageItem messageItem, bool keepRetentionTag)
		{
			if (policy == null && compactDefaultPolicy == null)
			{
				using (Folder folder = Folder.Bind(mailboxSession, messageItem.ParentId, new PropertyDefinition[]
				{
					StoreObjectSchema.PolicyTag,
					StoreObjectSchema.RetentionPeriod,
					StoreObjectSchema.RetentionFlags,
					StoreObjectSchema.ArchiveTag,
					StoreObjectSchema.ArchivePeriod,
					FolderSchema.RetentionTagEntryId
				}))
				{
					RetentionTagHelper.ReadPolicyFromFolder(folder, out policy, out flags, out period, out archivePolicy, out archivePeriod, out compactDefaultPolicy);
				}
			}
			bool flag = false;
			bool flag2 = false;
			ExDateTime d = messageItem.ReceivedTime;
			if (d == ExDateTime.MinValue)
			{
				d = messageItem.SentTime;
				if (d == ExDateTime.MinValue)
				{
					d = ExDateTime.Now;
				}
			}
			if (archivePolicy != null && archivePolicy is byte[] && archivePeriod != null && archivePeriod is int)
			{
				flag2 = true;
			}
			if (policy != null && policy is byte[] && period != null && period is int)
			{
				flag = true;
			}
			if (flag2)
			{
				try
				{
					messageItem[ItemSchema.ArchiveDate] = d.AddDays((double)((int)archivePeriod));
				}
				catch (ArgumentOutOfRangeException)
				{
					messageItem[ItemSchema.ArchiveDate] = DateTime.MaxValue;
				}
				messageItem[StoreObjectSchema.ArchiveTag] = (archivePolicy as byte[]);
			}
			else
			{
				messageItem.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.ArchiveTag
				});
				messageItem.DeleteProperties(new PropertyDefinition[]
				{
					ItemSchema.ArchiveDate
				});
			}
			Guid guid;
			int num;
			RetentionTagHelper.GetDefaultPolicyInfo(mailboxSession, messageItem, compactDefaultPolicy, out guid, out num);
			if (!keepRetentionTag)
			{
				if (flag)
				{
					try
					{
						messageItem[ItemSchema.RetentionDate] = d.AddDays((double)((int)period));
					}
					catch (ArgumentOutOfRangeException)
					{
						messageItem[ItemSchema.RetentionDate] = DateTime.MaxValue;
					}
					messageItem[StoreObjectSchema.PolicyTag] = (policy as byte[]);
				}
				else
				{
					if (num > 0)
					{
						try
						{
							messageItem[ItemSchema.RetentionDate] = d.AddDays((double)num);
							goto IL_223;
						}
						catch (ArgumentOutOfRangeException)
						{
							messageItem[ItemSchema.RetentionDate] = DateTime.MaxValue;
							goto IL_223;
						}
					}
					messageItem.DeleteProperties(new PropertyDefinition[]
					{
						ItemSchema.RetentionDate
					});
					IL_223:
					if (!guid.Equals(Guid.Empty))
					{
						messageItem[StoreObjectSchema.PolicyTag] = guid.ToByteArray();
					}
					else
					{
						messageItem.DeleteProperties(new PropertyDefinition[]
						{
							StoreObjectSchema.PolicyTag
						});
						if (!flag2)
						{
							return;
						}
					}
				}
			}
			else if (!flag2)
			{
				return;
			}
			CompositeProperty compositeProperty = new CompositeProperty(num, d.UniversalTime);
			messageItem[ItemSchema.StartDateEtc] = compositeProperty.GetBytes(true);
			messageItem[StoreObjectSchema.RetentionFlags] = 0;
		}

		internal static void ApplyPolicy(MailboxSession mailboxSession, object policy, object flags, object period, object archivePolicy, object archivePeriod, object compactDefaultPolicy, MessageItem messageItem)
		{
			RetentionTagHelper.ApplyPolicy(mailboxSession, policy, flags, period, archivePolicy, archivePeriod, compactDefaultPolicy, messageItem, false);
		}

		private static void GetDefaultPolicyInfo(MailboxSession mailboxSession, MessageItem messageItem, object compactDefaultPolicy, out Guid defaultGuid, out int periodInStartDateEtc)
		{
			defaultGuid = Guid.Empty;
			periodInStartDateEtc = 0;
			Guid guid = Guid.Empty;
			Guid guid2 = Guid.Empty;
			if (compactDefaultPolicy != null & compactDefaultPolicy is byte[])
			{
				Dictionary<Guid, StoreTagData> dictionary = MrmFaiFormatter.Deserialize(compactDefaultPolicy as byte[], mailboxSession.MailboxOwner);
				List<ElcPolicySettings> list = new List<ElcPolicySettings>();
				Dictionary<string, ContentSetting> itemClassToPolicyMapping = new Dictionary<string, ContentSetting>();
				foreach (StoreTagData storeTagData in dictionary.Values)
				{
					if (storeTagData.Tag.Type == ElcFolderType.All)
					{
						foreach (ContentSetting contentSetting in storeTagData.ContentSettings.Values)
						{
							ElcPolicySettings.ParseContentSettings(list, contentSetting);
							if (contentSetting.MessageClass.Equals(ElcMessageClass.AllMailboxContent, StringComparison.CurrentCultureIgnoreCase))
							{
								guid2 = storeTagData.Tag.RetentionId;
							}
							else if (contentSetting.MessageClass.Equals(ElcMessageClass.VoiceMail, StringComparison.CurrentCultureIgnoreCase))
							{
								guid = storeTagData.Tag.RetentionId;
							}
						}
					}
				}
				string effectiveItemClass = ElcPolicySettings.GetEffectiveItemClass(messageItem.ClassName);
				if (effectiveItemClass.StartsWith(ElcMessageClass.VoiceMail.TrimEnd(new char[]
				{
					'*'
				}), StringComparison.OrdinalIgnoreCase) && !guid.Equals(Guid.Empty))
				{
					defaultGuid = guid;
				}
				else if (!guid2.Equals(Guid.Empty))
				{
					defaultGuid = guid2;
				}
				ContentSetting applyingPolicy = ElcPolicySettings.GetApplyingPolicy(list, messageItem.ClassName, itemClassToPolicyMapping);
				if (applyingPolicy != null && applyingPolicy.RetentionEnabled && applyingPolicy.AgeLimitForRetention != null)
				{
					periodInStartDateEtc = (int)applyingPolicy.AgeLimitForRetention.Value.TotalDays;
					return;
				}
				if (applyingPolicy == null)
				{
					defaultGuid = Guid.Empty;
				}
			}
		}
	}
}
