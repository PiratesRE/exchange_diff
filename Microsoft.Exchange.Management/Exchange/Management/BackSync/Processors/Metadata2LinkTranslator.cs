using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal sealed class Metadata2LinkTranslator : PipelineProcessor
	{
		public Metadata2LinkTranslator(IDataProcessor next) : base(next)
		{
			this.InitializePropertiesToTranslate();
		}

		protected override bool ProcessInternal(PropertyBag propertyBag)
		{
			Dictionary<SyncPropertyDefinition, List<SyncLink>> dictionary = new Dictionary<SyncPropertyDefinition, List<SyncLink>>();
			MultiValuedProperty<LinkMetadata> multiValuedProperty = (MultiValuedProperty<LinkMetadata>)propertyBag[ADRecipientSchema.LinkMetadata];
			foreach (LinkMetadata linkMetadata in multiValuedProperty)
			{
				if (this.propertiesToTranslate.ContainsKey(linkMetadata.AttributeName))
				{
					SyncPropertyDefinition key = this.propertiesToTranslate[linkMetadata.AttributeName];
					if (!dictionary.ContainsKey(key))
					{
						dictionary[key] = new List<SyncLink>();
					}
					dictionary[key].Add(Metadata2LinkTranslator.ConvertLinkMetadataToSyncLink(linkMetadata));
				}
			}
			foreach (KeyValuePair<SyncPropertyDefinition, List<SyncLink>> keyValuePair in dictionary)
			{
				propertyBag.SetField(keyValuePair.Key, new MultiValuedProperty<SyncLink>(true, keyValuePair.Key, keyValuePair.Value));
			}
			return true;
		}

		private static SyncLink ConvertLinkMetadataToSyncLink(LinkMetadata metadata)
		{
			return new SyncLink(new ADObjectId(metadata.TargetDistinguishedName), metadata.IsDeleted ? LinkState.Removed : LinkState.Added);
		}

		private void InitializePropertiesToTranslate()
		{
			this.propertiesToTranslate = new Dictionary<string, SyncPropertyDefinition>();
			foreach (SyncPropertyDefinition syncPropertyDefinition in SyncSchema.Instance.AllBackSyncLinkedProperties)
			{
				this.propertiesToTranslate[syncPropertyDefinition.LdapDisplayName] = syncPropertyDefinition;
			}
			foreach (SyncPropertyDefinition syncPropertyDefinition2 in SyncSchema.Instance.AllBackSyncShadowLinkedProperties)
			{
				this.propertiesToTranslate[syncPropertyDefinition2.LdapDisplayName] = syncPropertyDefinition2;
			}
		}

		private Dictionary<string, SyncPropertyDefinition> propertiesToTranslate;
	}
}
