using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.OData.Model;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal abstract class DocumentPublisher
	{
		public DocumentPublisher(HttpContext httpContext, ServiceModel serviceModel)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			ArgumentValidator.ThrowIfNull("serviceModel", serviceModel);
			this.HttpContext = httpContext;
			this.ServiceModel = serviceModel;
		}

		public HttpContext HttpContext { get; private set; }

		public ServiceModel ServiceModel { get; private set; }

		public Task Publish()
		{
			return Task.Factory.StartNew(new Action(this.InternalPublish));
		}

		private void InternalPublish()
		{
			ResponseMessageWriter responseMessageWriter = new ResponseMessageWriter(this.HttpContext, this.ServiceModel);
			using (ODataMessageWriter odataMessageWriter = responseMessageWriter.CreateODataMessageWriter(null, false))
			{
				try
				{
					this.WriteDocument(odataMessageWriter);
				}
				catch (ODataContentTypeException innerException)
				{
					throw new InvalidContentTypeException(innerException);
				}
			}
		}

		protected abstract void WriteDocument(ODataMessageWriter odataWriter);
	}
}
