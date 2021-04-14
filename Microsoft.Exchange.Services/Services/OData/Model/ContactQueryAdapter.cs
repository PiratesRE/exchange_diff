using System;
using System.Linq;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ContactQueryAdapter : EwsQueryAdapter
	{
		public ContactQueryAdapter(ContactSchema entitySchema, ODataQueryOptions odataQueryOptions) : base(entitySchema, odataQueryOptions)
		{
		}

		public bool FindNeedsReread
		{
			get
			{
				return base.RequestedProperties.Intersect(ContactQueryAdapter.PropertiesSkippedForFind).Count<PropertyDefinition>() > 0;
			}
		}

		public ItemResponseShape GetResponseShape(bool findOnly = false)
		{
			if (findOnly && this.FindNeedsReread)
			{
				return ContactQueryAdapter.IdOnlyResponseType;
			}
			PropertyPath[] requestedPropertyPaths = base.GetRequestedPropertyPaths();
			return new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, false, requestedPropertyPaths);
		}

		public static readonly ContactQueryAdapter Default = new ContactQueryAdapter(ContactSchema.SchemaInstance, ODataQueryOptions.Empty);

		public static readonly ItemResponseShape IdOnlyResponseType = new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, false, null);

		public static readonly PropertyDefinition[] PropertiesSkippedForFind = new PropertyDefinition[]
		{
			ItemSchema.Body
		};
	}
}
