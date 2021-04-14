using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PolicyTagHelper
	{
		public static PropertyDefinition[] ArchiveProperties
		{
			get
			{
				return PolicyTagHelper.ArchivePropertyArray;
			}
		}

		public static PropertyDefinition[] RetentionProperties
		{
			get
			{
				return PolicyTagHelper.RetentionPropertyArray;
			}
		}

		public static PropertyDefinition[] FolderProperties
		{
			get
			{
				return PolicyTagHelper.FolderPropertyArray;
			}
		}

		public static void SetPolicyTagForArchiveOnFolder(PolicyTag policyTag, Folder folder)
		{
			folder[StoreObjectSchema.ArchiveTag] = policyTag.PolicyGuid.ToByteArray();
			folder[StoreObjectSchema.ArchivePeriod] = (int)policyTag.TimeSpanForRetention.TotalDays;
			object valueOrDefault = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			RetentionAndArchiveFlags retentionAndArchiveFlags;
			if (valueOrDefault != null && valueOrDefault is int)
			{
				retentionAndArchiveFlags = (RetentionAndArchiveFlags)((int)valueOrDefault | 16);
			}
			else
			{
				retentionAndArchiveFlags = RetentionAndArchiveFlags.ExplictArchiveTag;
			}
			folder[StoreObjectSchema.RetentionFlags] = (int)retentionAndArchiveFlags;
			if (folder.GetValueOrDefault<object>(StoreObjectSchema.ExplicitArchiveTag) != null)
			{
				folder.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.ExplicitArchiveTag
				});
			}
		}

		public static void SetPolicyTagForArchiveOnNewFolder(PolicyTag policyTag, Folder folder)
		{
			folder[StoreObjectSchema.ArchiveTag] = policyTag.PolicyGuid.ToByteArray();
			folder[StoreObjectSchema.ArchivePeriod] = (int)policyTag.TimeSpanForRetention.TotalDays;
			object valueOrDefault = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			RetentionAndArchiveFlags retentionAndArchiveFlags;
			if (valueOrDefault != null && valueOrDefault is int)
			{
				retentionAndArchiveFlags = (RetentionAndArchiveFlags)((int)valueOrDefault | 16);
			}
			else
			{
				retentionAndArchiveFlags = RetentionAndArchiveFlags.ExplictArchiveTag;
			}
			folder[StoreObjectSchema.RetentionFlags] = (int)retentionAndArchiveFlags;
		}

		public static void ClearPolicyTagForArchiveOnFolder(Folder folder)
		{
			if (folder.GetValueOrDefault<object>(StoreObjectSchema.ArchiveTag) != null)
			{
				folder.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.ArchiveTag
				});
			}
			if (folder.GetValueOrDefault<object>(StoreObjectSchema.ArchivePeriod) != null)
			{
				folder.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.ArchivePeriod
				});
			}
			object valueOrDefault = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			if (valueOrDefault != null && valueOrDefault is int)
			{
				RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)((int)valueOrDefault & -17);
				folder[StoreObjectSchema.RetentionFlags] = (int)retentionAndArchiveFlags;
			}
		}

		public static Guid? GetPolicyTagForArchiveFromFolder(Folder folder, out bool isExplicit)
		{
			Guid? result = null;
			byte[] array = (byte[])folder.GetValueOrDefault<object>(StoreObjectSchema.ArchiveTag);
			if (array != null && array.Length == PolicyTagHelper.SizeOfGuid)
			{
				result = new Guid?(new Guid(array));
			}
			object valueOrDefault = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			if (valueOrDefault != null && valueOrDefault is int)
			{
				isExplicit = (((int)valueOrDefault & 16) != 0);
			}
			else
			{
				isExplicit = false;
			}
			return result;
		}

		public static void SetPolicyTagForArchiveOnItem(PolicyTag policyTag, StoreObject item)
		{
			item[StoreObjectSchema.ArchiveTag] = policyTag.PolicyGuid.ToByteArray();
			object valueOrDefault = item.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			if (valueOrDefault != null && valueOrDefault is int)
			{
				RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)((int)valueOrDefault & -33);
				item[StoreObjectSchema.RetentionFlags] = (int)retentionAndArchiveFlags;
			}
			else
			{
				item[StoreObjectSchema.RetentionFlags] = 0;
			}
			CompositeRetentionProperty compositeRetentionProperty = null;
			byte[] valueOrDefault2 = item.GetValueOrDefault<byte[]>(ItemSchema.StartDateEtc);
			if (valueOrDefault2 != null)
			{
				try
				{
					compositeRetentionProperty = CompositeRetentionProperty.Parse(valueOrDefault2, true);
				}
				catch (ArgumentException)
				{
					compositeRetentionProperty = null;
				}
			}
			if (compositeRetentionProperty == null)
			{
				compositeRetentionProperty = new CompositeRetentionProperty();
				compositeRetentionProperty.Integer = (int)policyTag.TimeSpanForRetention.TotalDays;
				valueOrDefault = item.GetValueOrDefault<object>(InternalSchema.ReceivedTime);
				if (valueOrDefault == null)
				{
					valueOrDefault = item.GetValueOrDefault<object>(StoreObjectSchema.CreationTime);
				}
				if (valueOrDefault == null)
				{
					compositeRetentionProperty.Date = new DateTime?((DateTime)ExDateTime.Now);
				}
				else
				{
					compositeRetentionProperty.Date = new DateTime?((DateTime)((ExDateTime)valueOrDefault));
				}
				item[InternalSchema.StartDateEtc] = compositeRetentionProperty.GetBytes(true);
			}
			if (policyTag.TimeSpanForRetention.TotalDays > 0.0)
			{
				long fileTime = 0L;
				try
				{
					fileTime = compositeRetentionProperty.Date.Value.AddDays(policyTag.TimeSpanForRetention.TotalDays).ToFileTimeUtc();
				}
				catch (ArgumentOutOfRangeException)
				{
					fileTime = 0L;
				}
				item[InternalSchema.ArchivePeriod] = (int)policyTag.TimeSpanForRetention.TotalDays;
				DateTime dateTime = DateTime.FromFileTimeUtc(fileTime);
				item[InternalSchema.ArchiveDate] = dateTime;
			}
			else if (item.GetValueOrDefault<object>(InternalSchema.ArchiveDate) != null)
			{
				item.DeleteProperties(new PropertyDefinition[]
				{
					InternalSchema.ArchiveDate
				});
			}
			if (item.GetValueOrDefault<object>(StoreObjectSchema.ExplicitArchiveTag) != null)
			{
				item.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.ExplicitArchiveTag
				});
			}
		}

		public static void SetPolicyTagForArchiveOnNewItem(PolicyTag policyTag, StoreObject item)
		{
			item[StoreObjectSchema.ArchiveTag] = policyTag.PolicyGuid.ToByteArray();
			item[StoreObjectSchema.RetentionFlags] = 0;
			CompositeRetentionProperty setStartDateEtc = PolicyTagHelper.GetSetStartDateEtc(policyTag, item);
			if (policyTag.TimeSpanForRetention.TotalDays > 0.0)
			{
				item[InternalSchema.ArchivePeriod] = (int)policyTag.TimeSpanForRetention.TotalDays;
				item[InternalSchema.ArchiveDate] = PolicyTagHelper.CalculateExecutionDate(setStartDateEtc, policyTag.TimeSpanForRetention.TotalDays);
			}
		}

		public static void SetKeepInPlaceForArchiveOnItem(StoreObject item)
		{
			PolicyTagHelper.ClearPolicyTagForArchiveOnItem(item);
			object valueOrDefault = item.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			RetentionAndArchiveFlags retentionAndArchiveFlags;
			if (valueOrDefault != null && valueOrDefault is int)
			{
				retentionAndArchiveFlags = (RetentionAndArchiveFlags)((int)valueOrDefault | 32);
			}
			else
			{
				retentionAndArchiveFlags = RetentionAndArchiveFlags.KeepInPlace;
			}
			item[StoreObjectSchema.RetentionFlags] = (int)retentionAndArchiveFlags;
		}

		public static void ClearPolicyTagForArchiveOnItem(StoreObject item)
		{
			if (item.GetValueOrDefault<object>(StoreObjectSchema.ArchiveTag) != null)
			{
				item.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.ArchiveTag
				});
			}
			if (item.GetValueOrDefault<object>(StoreObjectSchema.ArchivePeriod) != null)
			{
				item.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.ArchivePeriod
				});
			}
			if (item.GetValueOrDefault<object>(InternalSchema.ArchiveDate) != null)
			{
				item.DeleteProperties(new PropertyDefinition[]
				{
					InternalSchema.ArchiveDate
				});
			}
			object valueOrDefault = item.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			if (valueOrDefault != null && valueOrDefault is int)
			{
				RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)((int)valueOrDefault & -33);
				item[StoreObjectSchema.RetentionFlags] = (int)retentionAndArchiveFlags;
			}
		}

		public static Guid? GetPolicyTagForArchiveFromItem(StoreObject item, out bool isExplicit, out bool isKeptInPlace, out DateTime? moveToArchive)
		{
			isExplicit = false;
			moveToArchive = null;
			Guid? result = null;
			byte[] array = (byte[])item.GetValueOrDefault<object>(InternalSchema.ArchiveTag);
			if (array != null && array.Length == PolicyTagHelper.SizeOfGuid)
			{
				result = new Guid?(new Guid(array));
			}
			object valueOrDefault = item.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			if (valueOrDefault != null && valueOrDefault is int)
			{
				isKeptInPlace = (((int)valueOrDefault & 32) != 0);
			}
			else
			{
				isKeptInPlace = false;
			}
			isExplicit = (item.GetValueOrDefault<object>(StoreObjectSchema.ArchivePeriod) != null);
			moveToArchive = (DateTime?)item.GetValueAsNullable<ExDateTime>(ItemSchema.ArchiveDate);
			return result;
		}

		public static void SetPolicyTagForDeleteOnFolder(PolicyTag policyTag, Folder folder)
		{
			folder[StoreObjectSchema.PolicyTag] = policyTag.PolicyGuid.ToByteArray();
			folder[StoreObjectSchema.RetentionPeriod] = (int)policyTag.TimeSpanForRetention.TotalDays;
			object valueOrDefault = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			int num = 9;
			RetentionAndArchiveFlags retentionAndArchiveFlags;
			if (valueOrDefault != null && valueOrDefault is int)
			{
				retentionAndArchiveFlags = (RetentionAndArchiveFlags)((int)valueOrDefault | num);
			}
			else
			{
				retentionAndArchiveFlags = (RetentionAndArchiveFlags)num;
			}
			folder[StoreObjectSchema.RetentionFlags] = (int)retentionAndArchiveFlags;
			if (folder.GetValueOrDefault<object>(StoreObjectSchema.ExplicitPolicyTag) != null)
			{
				folder.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.ExplicitPolicyTag
				});
			}
		}

		public static void SetPolicyTagForDeleteOnNewFolder(PolicyTag policyTag, Folder folder)
		{
			folder[StoreObjectSchema.PolicyTag] = policyTag.PolicyGuid.ToByteArray();
			folder[StoreObjectSchema.RetentionPeriod] = (int)policyTag.TimeSpanForRetention.TotalDays;
			object valueOrDefault = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			int num = 9;
			RetentionAndArchiveFlags retentionAndArchiveFlags;
			if (valueOrDefault != null && valueOrDefault is int)
			{
				retentionAndArchiveFlags = (RetentionAndArchiveFlags)((int)valueOrDefault | num);
			}
			else
			{
				retentionAndArchiveFlags = (RetentionAndArchiveFlags)num;
			}
			folder[StoreObjectSchema.RetentionFlags] = (int)retentionAndArchiveFlags;
		}

		public static void ClearPolicyTagForDeleteOnFolder(Folder folder)
		{
			if (folder.GetValueOrDefault<object>(StoreObjectSchema.PolicyTag) != null)
			{
				folder.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.PolicyTag
				});
			}
			if (folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionPeriod) != null)
			{
				folder.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.RetentionPeriod
				});
			}
			object valueOrDefault = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			if (valueOrDefault != null && valueOrDefault is int)
			{
				RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)((int)valueOrDefault & -2);
				retentionAndArchiveFlags &= ~RetentionAndArchiveFlags.PersonalTag;
				folder[StoreObjectSchema.RetentionFlags] = (int)retentionAndArchiveFlags;
			}
		}

		public static Guid? GetPolicyTagForDeleteFromFolder(Folder folder, out bool isExplicit)
		{
			Guid? result = null;
			byte[] array = (byte[])folder.GetValueOrDefault<object>(StoreObjectSchema.PolicyTag);
			if (array != null && array.Length == PolicyTagHelper.SizeOfGuid)
			{
				result = new Guid?(new Guid(array));
			}
			object valueOrDefault = folder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags);
			if (valueOrDefault != null && valueOrDefault is int)
			{
				isExplicit = (((int)valueOrDefault & 1) != 0);
			}
			else
			{
				isExplicit = false;
			}
			return result;
		}

		public static void SetPolicyTagForDeleteOnItem(PolicyTag policyTag, StoreObject item)
		{
			item[StoreObjectSchema.PolicyTag] = policyTag.PolicyGuid.ToByteArray();
			CompositeRetentionProperty compositeRetentionProperty = null;
			byte[] valueOrDefault = item.GetValueOrDefault<byte[]>(ItemSchema.StartDateEtc);
			if (valueOrDefault != null)
			{
				try
				{
					compositeRetentionProperty = CompositeRetentionProperty.Parse(valueOrDefault, true);
				}
				catch (ArgumentException)
				{
					compositeRetentionProperty = null;
				}
			}
			if (compositeRetentionProperty == null)
			{
				compositeRetentionProperty = new CompositeRetentionProperty();
				compositeRetentionProperty.Integer = (int)policyTag.TimeSpanForRetention.TotalDays;
				object valueOrDefault2 = item.GetValueOrDefault<object>(InternalSchema.ReceivedTime);
				if (valueOrDefault2 == null)
				{
					valueOrDefault2 = item.GetValueOrDefault<object>(StoreObjectSchema.CreationTime);
				}
				if (valueOrDefault2 == null)
				{
					compositeRetentionProperty.Date = new DateTime?((DateTime)ExDateTime.Now);
				}
				else
				{
					compositeRetentionProperty.Date = new DateTime?((DateTime)((ExDateTime)valueOrDefault2));
				}
				item[InternalSchema.StartDateEtc] = compositeRetentionProperty.GetBytes(true);
			}
			long fileTime = 0L;
			try
			{
				fileTime = compositeRetentionProperty.Date.Value.AddDays(policyTag.TimeSpanForRetention.TotalDays).ToFileTimeUtc();
			}
			catch (ArgumentOutOfRangeException)
			{
				fileTime = 0L;
			}
			item[InternalSchema.RetentionPeriod] = (int)policyTag.TimeSpanForRetention.TotalDays;
			DateTime dateTime = DateTime.FromFileTimeUtc(fileTime);
			item[InternalSchema.RetentionDate] = dateTime;
			if (item.GetValueOrDefault<object>(StoreObjectSchema.ExplicitPolicyTag) != null)
			{
				item.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.ExplicitPolicyTag
				});
			}
		}

		public static void SetPolicyTagForDeleteOnNewItem(PolicyTag policyTag, StoreObject item)
		{
			item[StoreObjectSchema.PolicyTag] = policyTag.PolicyGuid.ToByteArray();
			item[StoreObjectSchema.RetentionFlags] = 0;
			CompositeRetentionProperty setStartDateEtc = PolicyTagHelper.GetSetStartDateEtc(policyTag, item);
			if (policyTag.TimeSpanForRetention.TotalDays > 0.0)
			{
				item[InternalSchema.RetentionPeriod] = (int)policyTag.TimeSpanForRetention.TotalDays;
				item[InternalSchema.RetentionDate] = PolicyTagHelper.CalculateExecutionDate(setStartDateEtc, policyTag.TimeSpanForRetention.TotalDays);
			}
		}

		public static void ClearPolicyTagForDeleteOnItem(StoreObject item)
		{
			if (item.GetValueOrDefault<object>(StoreObjectSchema.PolicyTag) != null)
			{
				item.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.PolicyTag
				});
			}
			if (item.GetValueOrDefault<object>(StoreObjectSchema.RetentionPeriod) != null)
			{
				item.DeleteProperties(new PropertyDefinition[]
				{
					StoreObjectSchema.RetentionPeriod
				});
			}
			if (item.GetValueOrDefault<object>(InternalSchema.RetentionDate) != null)
			{
				item.DeleteProperties(new PropertyDefinition[]
				{
					InternalSchema.RetentionDate
				});
			}
		}

		public static Guid? GetPolicyTagForDeleteFromItem(StoreObject item, out bool isExplicit, out DateTime? deleteTime)
		{
			isExplicit = false;
			deleteTime = null;
			Guid? result = null;
			byte[] array = (byte[])item.GetValueOrDefault<object>(InternalSchema.PolicyTag);
			if (array != null && array.Length == PolicyTagHelper.SizeOfGuid)
			{
				result = new Guid?(new Guid(array));
			}
			isExplicit = (item.GetValueOrDefault<object>(StoreObjectSchema.RetentionPeriod) != null);
			deleteTime = (DateTime?)item.GetValueAsNullable<ExDateTime>(ItemSchema.RetentionDate);
			return result;
		}

		public static void SetRetentionProperties(StoreObject item, ExDateTime dateToExpireOn, int retentionPeriodInDays)
		{
			if (!(item is Item))
			{
				throw new ArgumentException("item must be of type Item. It cannot be null, a folder or anything else.");
			}
			if (retentionPeriodInDays <= 0)
			{
				throw new ArgumentException("retentionPeriodInDays must be greater than 0. The minimum value allowed is 1 day.");
			}
			item[StoreObjectSchema.PolicyTag] = PolicyTagHelper.SystemCleanupTagGuid.ToByteArray();
			item[ItemSchema.RetentionDate] = (DateTime)dateToExpireOn.ToUtc();
			item[StoreObjectSchema.RetentionPeriod] = retentionPeriodInDays;
			item[StoreObjectSchema.RetentionFlags] = 64;
		}

		public static void ApplyPolicyToFolder(MailboxSession session, CoreFolder coreFolder)
		{
			if (coreFolder.Origin == Origin.Existing)
			{
				PropertyDefinition[] folderProperties = PolicyTagHelper.FolderProperties;
				for (int i = 0; i < folderProperties.Length; i++)
				{
					if (coreFolder.PropertyBag.IsPropertyDirty(folderProperties[i]))
					{
						int num = 0;
						object obj = coreFolder.PropertyBag.TryGetProperty(InternalSchema.RetentionFlags);
						if (obj is int)
						{
							num = (int)obj;
						}
						num = FlagsMan.SetNeedRescan(num);
						coreFolder.PropertyBag.SetProperty(InternalSchema.RetentionFlags, num);
						return;
					}
				}
				return;
			}
			int num2 = 0;
			object obj2 = coreFolder.PropertyBag.TryGetProperty(InternalSchema.RetentionFlags);
			if (obj2 is int)
			{
				num2 = (int)obj2;
			}
			num2 = FlagsMan.SetNeedRescan(num2);
			coreFolder.PropertyBag.SetProperty(InternalSchema.RetentionFlags, num2);
		}

		private static CompositeRetentionProperty GetSetStartDateEtc(PolicyTag policyTag, StoreObject item)
		{
			CompositeRetentionProperty compositeRetentionProperty = null;
			byte[] array = null;
			try
			{
				array = item.GetValueOrDefault<byte[]>(InternalSchema.StartDateEtc);
			}
			catch (NotInBagPropertyErrorException)
			{
				array = null;
			}
			if (array != null)
			{
				try
				{
					compositeRetentionProperty = CompositeRetentionProperty.Parse(array, true);
				}
				catch (ArgumentException)
				{
					compositeRetentionProperty = null;
				}
			}
			if (compositeRetentionProperty == null)
			{
				compositeRetentionProperty = new CompositeRetentionProperty();
				compositeRetentionProperty.Integer = (int)policyTag.TimeSpanForRetention.TotalDays;
				object valueOrDefault = item.GetValueOrDefault<object>(InternalSchema.ReceivedTime);
				if (valueOrDefault == null)
				{
					valueOrDefault = item.GetValueOrDefault<object>(StoreObjectSchema.CreationTime);
				}
				if (valueOrDefault == null)
				{
					compositeRetentionProperty.Date = new DateTime?((DateTime)ExDateTime.Now);
				}
				else
				{
					compositeRetentionProperty.Date = new DateTime?((DateTime)((ExDateTime)valueOrDefault));
				}
				item[InternalSchema.StartDateEtc] = compositeRetentionProperty.GetBytes(true);
			}
			return compositeRetentionProperty;
		}

		private static DateTime CalculateExecutionDate(CompositeRetentionProperty startDateEtc, double policyDays)
		{
			long fileTime = 0L;
			if (startDateEtc != null && startDateEtc.Date != null)
			{
				try
				{
					fileTime = startDateEtc.Date.Value.AddDays(policyDays).ToFileTimeUtc();
				}
				catch (ArgumentOutOfRangeException)
				{
					fileTime = 0L;
				}
			}
			return DateTime.FromFileTimeUtc(fileTime);
		}

		public static readonly Guid SystemCleanupTagGuid = new Guid("CCE0D6E6-B69B-410a-907D-06DC2071AB58");

		private static readonly int SizeOfGuid = Marshal.SizeOf(Guid.NewGuid());

		private static readonly PropertyDefinition[] FolderPropertyArray = new PropertyDefinition[]
		{
			InternalSchema.PolicyTag,
			InternalSchema.ArchiveTag,
			InternalSchema.RetentionPeriod,
			InternalSchema.ArchivePeriod,
			InternalSchema.RetentionFlags
		};

		private static readonly PropertyDefinition[] ArchivePropertyArray = new PropertyDefinition[]
		{
			InternalSchema.ArchiveTag,
			InternalSchema.ArchiveDate,
			InternalSchema.ArchivePeriod,
			InternalSchema.RetentionFlags,
			InternalSchema.StartDateEtc,
			InternalSchema.ExplicitArchiveTag
		};

		private static readonly PropertyDefinition[] RetentionPropertyArray = new PropertyDefinition[]
		{
			InternalSchema.PolicyTag,
			InternalSchema.RetentionDate,
			InternalSchema.RetentionPeriod,
			InternalSchema.RetentionFlags,
			InternalSchema.StartDateEtc,
			InternalSchema.ExplicitPolicyTag
		};
	}
}
