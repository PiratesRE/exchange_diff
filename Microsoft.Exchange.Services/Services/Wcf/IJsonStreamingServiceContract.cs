using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ServiceContract]
	public interface IJsonStreamingServiceContract
	{
		[WebGet]
		[OperationContract(AsyncPattern = true)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[JsonResponseOptions(IsCacheable = true)]
		IAsyncResult BeginGetUserPhoto(string email, UserPhotoSize size, bool isPreview, bool fallbackToClearImage, AsyncCallback callback, object state);

		Stream EndGetUserPhoto(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[JsonResponseOptions(IsCacheable = true)]
		[WebGet]
		IAsyncResult BeginGetPeopleICommunicateWith(AsyncCallback callback, object state);

		Stream EndGetPeopleICommunicateWith(IAsyncResult result);
	}
}
