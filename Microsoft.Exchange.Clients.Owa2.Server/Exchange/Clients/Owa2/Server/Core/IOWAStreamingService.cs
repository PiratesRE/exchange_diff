using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ServiceContract]
	public interface IOWAStreamingService : IJsonStreamingServiceContract
	{
		[JsonRequestFormat(Format = JsonRequestFormat.QueryString)]
		[WebGet]
		[JsonResponseOptions(IsCacheable = true)]
		[OperationContract]
		Stream GetFileAttachment(string id, bool isImagePreview, bool asDataUri);

		[WebGet]
		[OperationContract]
		[JsonRequestFormat(Format = JsonRequestFormat.QueryString)]
		[JsonResponseOptions(IsCacheable = true)]
		Stream GetAllAttachmentsAsZip(string id);

		[OperationContract]
		[WebGet]
		[JsonResponseOptions(IsCacheable = true)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		Stream GetPersonaPhoto(string personId, string adObjectId, string email, string singleSourceId, UserPhotoSize size);
	}
}
