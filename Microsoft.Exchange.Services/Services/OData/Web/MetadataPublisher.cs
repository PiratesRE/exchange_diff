using System;
using System.Web;
using Microsoft.Exchange.Services.OData.Model;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class MetadataPublisher : DocumentPublisher
	{
		public MetadataPublisher(HttpContext httpContext, ServiceModel serviceModel) : base(httpContext, serviceModel)
		{
		}

		protected override void WriteDocument(ODataMessageWriter odataWriter)
		{
			odataWriter.WriteMetadataDocument();
		}
	}
}
