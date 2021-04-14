using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ServiceContract]
	public interface IEWSStreamingContract
	{
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract(AsyncPattern = true)]
		[WebGet]
		IAsyncResult BeginGetUserPhoto(string email, UserPhotoSize size, AsyncCallback callback, object state);

		Stream EndGetUserPhoto(IAsyncResult result);

		[WebGet]
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetPeopleICommunicateWith(AsyncCallback callback, object state);

		Stream EndGetPeopleICommunicateWith(IAsyncResult result);
	}
}
