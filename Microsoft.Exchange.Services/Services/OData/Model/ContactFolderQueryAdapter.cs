using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ContactFolderQueryAdapter : EwsQueryAdapter
	{
		public ContactFolderQueryAdapter(ContactFolderSchema entitySchema, ODataQueryOptions odataQueryOptions) : base(entitySchema, odataQueryOptions)
		{
		}

		public FolderResponseShape GetResponseShape(bool findOnly = false)
		{
			return new FolderResponseShape(ShapeEnum.IdOnly, base.GetRequestedPropertyPaths());
		}

		public static readonly ContactFolderQueryAdapter Default = new ContactFolderQueryAdapter(ContactFolderSchema.SchemaInstance, ODataQueryOptions.Empty);
	}
}
