using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FolderQueryAdapter : EwsQueryAdapter
	{
		public FolderQueryAdapter(FolderSchema entitySchema, ODataQueryOptions odataQueryOptions) : base(entitySchema, odataQueryOptions)
		{
		}

		public FolderResponseShape GetResponseShape()
		{
			return new FolderResponseShape(ShapeEnum.IdOnly, base.GetRequestedPropertyPaths());
		}

		public static readonly FolderQueryAdapter Default = new FolderQueryAdapter(FolderSchema.SchemaInstance, ODataQueryOptions.Empty);
	}
}
