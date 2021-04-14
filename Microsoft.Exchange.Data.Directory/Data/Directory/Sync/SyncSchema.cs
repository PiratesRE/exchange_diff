using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncSchema : ADPropertyUnionSchema
	{
		public static SyncSchema Instance
		{
			get
			{
				if (SyncSchema.instance == null)
				{
					SyncSchema.instance = ObjectSchema.GetInstance<SyncSchema>();
				}
				return SyncSchema.instance;
			}
		}

		public override ReadOnlyCollection<ADObjectSchema> ObjectSchemas
		{
			get
			{
				return SyncSchema.AllSyncSchemas;
			}
		}

		public ReadOnlyCollection<SyncPropertyDefinition> AllLinkedProperties
		{
			get
			{
				return this.allLinkedProperties;
			}
		}

		public ReadOnlyCollection<SyncPropertyDefinition> AllBackSyncLinkedProperties
		{
			get
			{
				return this.allBackSyncLinkedProperties;
			}
		}

		public ReadOnlyCollection<SyncPropertyDefinition> AllBackSyncShadowLinkedProperties
		{
			get
			{
				return this.allBackSyncShadowLinkedProperties;
			}
		}

		public ReadOnlyCollection<SyncPropertyDefinition> AllBackSyncShadowBaseProperties
		{
			get
			{
				return this.allBackSyncShadowBaseProperties;
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

		public ReadOnlyCollection<SyncPropertyDefinition> AllBackSyncBaseProperties
		{
			get
			{
				return this.allBackSyncBaseProperties;
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

		public SyncSchema()
		{
			SyncObjectSchema.InitializeSyncPropertyCollections(base.AllProperties, out this.allLinkedProperties, out this.allForwardSyncProperties, out this.allForwardSyncLinkedProperties, out this.allBackSyncProperties, out this.allShadowProperties);
			this.InitializeBackSyncPropertyCollections();
		}

		private void InitializeBackSyncPropertyCollections()
		{
			List<SyncPropertyDefinition> list = new List<SyncPropertyDefinition>();
			List<SyncPropertyDefinition> list2 = new List<SyncPropertyDefinition>();
			List<SyncPropertyDefinition> list3 = new List<SyncPropertyDefinition>();
			List<SyncPropertyDefinition> list4 = new List<SyncPropertyDefinition>();
			foreach (SyncPropertyDefinition syncPropertyDefinition in this.allBackSyncProperties)
			{
				if (syncPropertyDefinition.IsSyncLink)
				{
					list.Add(syncPropertyDefinition);
				}
				else
				{
					list2.Add(syncPropertyDefinition);
				}
			}
			foreach (SyncPropertyDefinition syncPropertyDefinition2 in this.allShadowProperties)
			{
				if (syncPropertyDefinition2.IsSyncLink)
				{
					list3.Add(syncPropertyDefinition2);
				}
				else
				{
					list4.Add(syncPropertyDefinition2);
				}
			}
			this.allBackSyncLinkedProperties = new ReadOnlyCollection<SyncPropertyDefinition>(list);
			this.allBackSyncBaseProperties = new ReadOnlyCollection<SyncPropertyDefinition>(list2);
			this.allBackSyncShadowLinkedProperties = new ReadOnlyCollection<SyncPropertyDefinition>(list3);
			this.allBackSyncShadowBaseProperties = new ReadOnlyCollection<SyncPropertyDefinition>(list4);
		}

		internal bool TryGetLinkedPropertyDefinitionByMsoPropertyName(string propertyName, out SyncPropertyDefinition propertyDefinition)
		{
			propertyDefinition = null;
			foreach (SyncPropertyDefinition syncPropertyDefinition in this.allLinkedProperties)
			{
				if (propertyName.Equals(syncPropertyDefinition.MsoPropertyName, StringComparison.OrdinalIgnoreCase))
				{
					propertyDefinition = syncPropertyDefinition;
					return true;
				}
			}
			return false;
		}

		private static readonly ReadOnlyCollection<ADObjectSchema> AllSyncSchemas = new ReadOnlyCollection<ADObjectSchema>(new ADObjectSchema[]
		{
			ObjectSchema.GetInstance<SyncGroupSchema>(),
			ObjectSchema.GetInstance<SyncCompanySchema>(),
			ObjectSchema.GetInstance<SyncContactSchema>(),
			ObjectSchema.GetInstance<SyncUserSchema>()
		});

		private static SyncSchema instance;

		private ReadOnlyCollection<SyncPropertyDefinition> allLinkedProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allForwardSyncProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allForwardSyncLinkedProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allBackSyncProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allBackSyncLinkedProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allBackSyncShadowLinkedProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allBackSyncShadowBaseProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allBackSyncBaseProperties;

		private ReadOnlyCollection<SyncPropertyDefinition> allShadowProperties;
	}
}
