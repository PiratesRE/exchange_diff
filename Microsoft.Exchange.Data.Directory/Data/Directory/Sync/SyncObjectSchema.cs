using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal abstract class SyncObjectSchema : ADObjectSchema
	{
		public SyncObjectSchema()
		{
			SyncObjectSchema.InitializeSyncPropertyCollections(base.AllProperties, out this.allLinkedProperties, out this.allForwardSyncProperties, out this.allForwardSyncLinkedProperties, out this.allBackSyncProperties, out this.allShadowProperties);
		}

		public ReadOnlyCollection<SyncPropertyDefinition> AllLinkedProperties
		{
			get
			{
				return this.allLinkedProperties;
			}
		}

		public ReadOnlyCollection<SyncPropertyDefinition> AllForwardSyncProperties
		{
			get
			{
				return this.allForwardSyncProperties;
			}
		}

		public ReadOnlyCollection<SyncPropertyDefinition> AllForwardSyncLinkedProperties
		{
			get
			{
				return this.allForwardSyncLinkedProperties;
			}
		}

		public ReadOnlyCollection<SyncPropertyDefinition> AllBackSyncProperties
		{
			get
			{
				return this.allBackSyncProperties;
			}
		}

		public ReadOnlyCollection<SyncPropertyDefinition> AllShadowProperties
		{
			get
			{
				return this.allShadowProperties;
			}
		}

		public abstract DirectoryObjectClass DirectoryObjectClass { get; }

		public static void InitializeSyncPropertyCollections(ICollection<PropertyDefinition> allProperties, out ReadOnlyCollection<SyncPropertyDefinition> allLinkedProperties, out ReadOnlyCollection<SyncPropertyDefinition> allForwardSyncProperties, out ReadOnlyCollection<SyncPropertyDefinition> allForwardSyncLinkedProperties, out ReadOnlyCollection<SyncPropertyDefinition> allBackSyncProperties, out ReadOnlyCollection<SyncPropertyDefinition> allShadowProperties)
		{
			List<SyncPropertyDefinition> list = new List<SyncPropertyDefinition>();
			List<SyncPropertyDefinition> list2 = new List<SyncPropertyDefinition>();
			List<SyncPropertyDefinition> list3 = new List<SyncPropertyDefinition>();
			List<SyncPropertyDefinition> list4 = new List<SyncPropertyDefinition>();
			List<SyncPropertyDefinition> list5 = new List<SyncPropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in allProperties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				SyncPropertyDefinition syncPropertyDefinition = adpropertyDefinition as SyncPropertyDefinition;
				if (syncPropertyDefinition != null)
				{
					if (syncPropertyDefinition.IsSyncLink)
					{
						list.Add(syncPropertyDefinition);
						if (syncPropertyDefinition.IsForwardSync)
						{
							list3.Add(syncPropertyDefinition);
						}
					}
					if (syncPropertyDefinition.IsForwardSync)
					{
						list2.Add(syncPropertyDefinition);
					}
					if (syncPropertyDefinition.IsBackSync)
					{
						list4.Add(syncPropertyDefinition);
					}
					if (syncPropertyDefinition.IsShadow)
					{
						list5.Add(syncPropertyDefinition);
					}
				}
			}
			allLinkedProperties = new ReadOnlyCollection<SyncPropertyDefinition>(list.ToArray());
			allForwardSyncProperties = new ReadOnlyCollection<SyncPropertyDefinition>(list2.ToArray());
			allForwardSyncLinkedProperties = new ReadOnlyCollection<SyncPropertyDefinition>(list3.ToArray());
			allBackSyncProperties = new ReadOnlyCollection<SyncPropertyDefinition>(list4.ToArray());
			allShadowProperties = new ReadOnlyCollection<SyncPropertyDefinition>(list5.ToArray());
		}

		public static SyncPropertyDefinition All = new SyncPropertyDefinition("All", null, typeof(bool), typeof(bool), SyncPropertyDefinitionFlags.Ignore, SyncPropertyDefinition.InitialSyncPropertySetVersion, false);

		public static SyncPropertyDefinition ContextId = new SyncPropertyDefinition("ContextId", null, typeof(string), typeof(string), SyncPropertyDefinitionFlags.Immutable, SyncPropertyDefinition.InitialSyncPropertySetVersion, string.Empty);

		public static SyncPropertyDefinition Deleted = new SyncPropertyDefinition(ADDirSyncResultSchema.IsDeleted, null, typeof(bool), SyncPropertyDefinitionFlags.Ignore | SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.BackSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ObjectId = new SyncPropertyDefinition(ADRecipientSchema.ExternalDirectoryObjectId, null, typeof(string), SyncPropertyDefinitionFlags.Immutable, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition SyncObjectId = new SyncPropertyDefinition("SyncObjectId", null, typeof(SyncObjectId), typeof(object), SyncPropertyDefinitionFlags.Immutable | SyncPropertyDefinitionFlags.TaskPopulated, SyncPropertyDefinition.InitialSyncPropertySetVersion, null);

		public static SyncPropertyDefinition LastKnownParent = new SyncPropertyDefinition(DeletedObjectSchema.LastKnownParent, null, typeof(string), SyncPropertyDefinitionFlags.BackSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition UsnChanged = new SyncPropertyDefinition(ADRecipientSchema.UsnChanged, null, typeof(long), SyncPropertyDefinitionFlags.Immutable, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition PropertyValidationErrors = new SyncPropertyDefinition("PropertyValidationErrors", null, typeof(ValidationError), typeof(object), SyncPropertyDefinitionFlags.Ignore | SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.InitialSyncPropertySetVersion, null);

		public static SyncPropertyDefinition FaultInServiceInstance = new SyncPropertyDefinition("FaultInServiceInstance", null, typeof(ServiceInstanceId), typeof(object), SyncPropertyDefinitionFlags.Ignore | SyncPropertyDefinitionFlags.TaskPopulated, SyncPropertyDefinition.InitialSyncPropertySetVersion, null);

		private ReadOnlyCollection<SyncPropertyDefinition> allLinkedProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allForwardSyncProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allForwardSyncLinkedProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allBackSyncProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allShadowProperties;
	}
}
