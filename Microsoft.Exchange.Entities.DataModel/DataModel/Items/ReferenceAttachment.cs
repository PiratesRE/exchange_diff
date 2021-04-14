using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public class ReferenceAttachment : Attachment<ReferenceAttachmentSchema>, IReferenceAttachment, IAttachment, IEntity, IPropertyChangeTracker<PropertyDefinition>
	{
		public string PathName
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.PathNameProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.PathNameProperty, value);
			}
		}

		public string ProviderEndpointUrl
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.ProviderEndpointUrlProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.ProviderEndpointUrlProperty, value);
			}
		}

		public string ProviderType
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.ProviderTypeProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.ProviderTypeProperty, value);
			}
		}

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<ReferenceAttachment, string> PathName = new EntityPropertyAccessor<ReferenceAttachment, string>(SchematizedObject<ReferenceAttachmentSchema>.SchemaInstance.PathNameProperty, (ReferenceAttachment referenceAttachment) => referenceAttachment.PathName, delegate(ReferenceAttachment referenceAttachment, string pathName)
			{
				referenceAttachment.PathName = pathName;
			});

			public static readonly EntityPropertyAccessor<ReferenceAttachment, string> ProviderEndpointUrl = new EntityPropertyAccessor<ReferenceAttachment, string>(SchematizedObject<ReferenceAttachmentSchema>.SchemaInstance.ProviderEndpointUrlProperty, (ReferenceAttachment referenceAttachment) => referenceAttachment.ProviderEndpointUrl, delegate(ReferenceAttachment referenceAttachment, string providerEndpointUrl)
			{
				referenceAttachment.ProviderEndpointUrl = providerEndpointUrl;
			});

			public static readonly EntityPropertyAccessor<ReferenceAttachment, string> ProviderType = new EntityPropertyAccessor<ReferenceAttachment, string>(SchematizedObject<ReferenceAttachmentSchema>.SchemaInstance.ProviderTypeProperty, (ReferenceAttachment referenceAttachment) => referenceAttachment.ProviderType, delegate(ReferenceAttachment referenceAttachment, string providerType)
			{
				referenceAttachment.ProviderType = providerType;
			});
		}
	}
}
