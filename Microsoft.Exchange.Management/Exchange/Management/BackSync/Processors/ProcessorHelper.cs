using System;
using System.Collections;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal static class ProcessorHelper
	{
		internal static ADObjectId GetTenantOU(PropertyBag propertyBag)
		{
			bool flag = ProcessorHelper.IsDeletedObject(propertyBag);
			bool flag2 = ProcessorHelper.IsObjectOrganizationUnit(propertyBag);
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			ADObjectId adobjectId2 = flag2 ? adobjectId : adobjectId.Parent;
			if (flag)
			{
				ADObjectId adobjectId3 = (ADObjectId)propertyBag[SyncObjectSchema.LastKnownParent];
				if (adobjectId3 == null)
				{
					adobjectId3 = adobjectId.DomainId.GetChildId("CN", "LostAndFound");
					ExTraceGlobals.BackSyncTracer.TraceWarning<string>((long)SyncConfiguration.TraceId, "ProcessorHelper::GetTenantOU for id {0}. Change for deleted object does not include lastKnowParent. This are not properly auth restored objects in the past prior to MSO tenants. Ignoring.", adobjectId.ToString());
				}
				adobjectId2 = (flag2 ? adobjectId : adobjectId3);
			}
			if (adobjectId2.Rdn.Equals(ProcessorHelper.SoftDeletedContainerName))
			{
				adobjectId2 = adobjectId2.Parent;
			}
			return adobjectId2;
		}

		internal static bool IsDeletedObject(PropertyBag propertyBag)
		{
			return (bool)propertyBag[ADDirSyncResultSchema.IsDeleted] || (bool)propertyBag[SyncObjectSchema.Deleted];
		}

		internal static bool IsUserObject(PropertyBag propertyBag)
		{
			DirectoryObjectClass directoryObjectClass = DirectoryObjectClass.Account;
			if (propertyBag.Contains(ADObjectSchema.ObjectClass))
			{
				directoryObjectClass = SyncRecipient.GetRecipientType(propertyBag);
			}
			return directoryObjectClass == DirectoryObjectClass.User;
		}

		internal static bool IsObjectOrganizationUnit(PropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			bool flag = !adobjectId.Rdn.Equals(ProcessorHelper.SoftDeletedContainerName) && adobjectId.Rdn.Prefix.Equals("OU", StringComparison.InvariantCultureIgnoreCase);
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool, string>((long)SyncConfiguration.TraceId, "ProcessorHelper::IsObjectOrganizationUnit return {0} ({1})", flag, adobjectId.ToString());
			return flag;
		}

		internal static void TracePropertBag(string hint, PropertyBag bag)
		{
			IDictionaryEnumerator dictionaryEnumerator = bag.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				if (dictionaryEnumerator.Value != null)
				{
					ADPropertyDefinition property = (ADPropertyDefinition)dictionaryEnumerator.Key;
					ProcessorHelper.TracePropertyValue(hint, property, dictionaryEnumerator.Value);
				}
			}
		}

		private static void TracePropertyValue(string hint, ADPropertyDefinition property, object value)
		{
			string arg = string.Empty;
			if (property.IsMultivalued)
			{
				if (value != null)
				{
					IList list = (IList)value;
					StringBuilder stringBuilder = new StringBuilder();
					foreach (object obj in list)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(";");
						}
						stringBuilder.AppendFormat("{0}", obj.ToString());
					}
					arg = stringBuilder.ToString();
				}
				else
				{
					arg = "NULL";
				}
			}
			else
			{
				arg = ((value != null) ? value.ToString() : "NULL");
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<string, string, string>((long)SyncConfiguration.TraceId, "<{0}> PropertyValue - {1}: {2}", hint, property.ToString(), arg);
		}

		internal static bool IsSoftDeletedObject(PropertyBag propertyBag)
		{
			ADObjectId id = (ADObjectId)propertyBag[ADObjectSchema.Id];
			return !ProcessorHelper.IsObjectOrganizationUnit(propertyBag) && ProcessorHelper.IsSoftDeletedObject(id);
		}

		internal static bool IsSoftDeletedObject(ADObjectId id)
		{
			if (id != null)
			{
				ADObjectId parent = id.Parent;
				if (parent.Rdn.Equals(ProcessorHelper.SoftDeletedContainerName))
				{
					return true;
				}
			}
			return false;
		}

		private const string OuRdnPrefix = "OU";

		private static AdName SoftDeletedContainerName = new AdName("OU", "Soft Deleted Objects");
	}
}
