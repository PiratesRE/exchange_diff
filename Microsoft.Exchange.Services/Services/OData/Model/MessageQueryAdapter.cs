using System;
using System.Linq;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class MessageQueryAdapter : EwsQueryAdapter
	{
		public MessageQueryAdapter(MessageSchema entitySchema, ODataQueryOptions odataQueryOptions) : base(entitySchema, odataQueryOptions)
		{
		}

		public ItemResponseShape GetResponseShape(bool findOnly = false)
		{
			if (findOnly && this.FindNeedsReread)
			{
				return MessageQueryAdapter.IdOnlyResponseType;
			}
			bool includeMimeContent = false;
			PropertyPath[] array = base.GetRequestedPropertyPaths();
			if (base.ODataQueryOptions.Expands(ItemSchema.Attachments.Name) && !base.RequestedProperties.Contains(ItemSchema.HasAttachments))
			{
				array = array.Concat(new PropertyPath[]
				{
					ItemSchema.HasAttachments.EwsPropertyProvider.GetEwsPropertyProvider(base.EntitySchema).PropertyInformation.PropertyPath
				}).ToArray<PropertyPath>();
			}
			return new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, includeMimeContent, array);
		}

		public bool FindNeedsReread
		{
			get
			{
				return base.RequestedProperties.Intersect(MessageQueryAdapter.PropertiesSkippedForFind).Count<PropertyDefinition>() > 0;
			}
		}

		public static readonly MessageQueryAdapter Default = new MessageQueryAdapter(MessageSchema.SchemaInstance, ODataQueryOptions.Empty);

		public static readonly ItemResponseShape IdOnlyResponseType = new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, false, null);

		public static readonly PropertyDefinition[] PropertiesSkippedForFind = new PropertyDefinition[]
		{
			ItemSchema.Body,
			MessageSchema.UniqueBody,
			MessageSchema.ToRecipients,
			MessageSchema.CcRecipients,
			MessageSchema.BccRecipients,
			MessageSchema.EventId
		};
	}
}
