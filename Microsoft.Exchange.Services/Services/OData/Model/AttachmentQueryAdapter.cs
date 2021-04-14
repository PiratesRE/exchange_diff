using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class AttachmentQueryAdapter : EwsQueryAdapter
	{
		public AttachmentQueryAdapter(AttachmentSchema entitySchema, ODataQueryOptions odataQueryOptions) : base(entitySchema, odataQueryOptions)
		{
		}

		public static readonly AttachmentQueryAdapter Default = new AttachmentQueryAdapter(AttachmentSchema.SchemaInstance, ODataQueryOptions.Empty);

		public static readonly AttachmentResponseShape AttachmentResponseShape = new AttachmentResponseShape();
	}
}
