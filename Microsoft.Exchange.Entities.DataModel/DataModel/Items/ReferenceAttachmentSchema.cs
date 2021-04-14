using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public sealed class ReferenceAttachmentSchema : AttachmentSchema
	{
		public TypedPropertyDefinition<string> PathNameProperty
		{
			get
			{
				return ReferenceAttachmentSchema.StaticPathNameProperty;
			}
		}

		public TypedPropertyDefinition<string> ProviderEndpointUrlProperty
		{
			get
			{
				return ReferenceAttachmentSchema.StaticProviderEndpointUrlProperty;
			}
		}

		public TypedPropertyDefinition<string> ProviderTypeProperty
		{
			get
			{
				return ReferenceAttachmentSchema.StaticProviderTypeProperty;
			}
		}

		private static readonly TypedPropertyDefinition<string> StaticPathNameProperty = new TypedPropertyDefinition<string>("ReferenceAttachment.PathName", null, true);

		private static readonly TypedPropertyDefinition<string> StaticProviderEndpointUrlProperty = new TypedPropertyDefinition<string>("ReferenceAttachment.ProviderEndpointUrl", null, true);

		private static readonly TypedPropertyDefinition<string> StaticProviderTypeProperty = new TypedPropertyDefinition<string>("ReferenceAttachment.ProviderType", null, true);
	}
}
