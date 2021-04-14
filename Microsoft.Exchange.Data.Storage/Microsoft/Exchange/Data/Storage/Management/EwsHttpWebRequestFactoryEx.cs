using System;
using System.Net;
using System.Net.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EwsHttpWebRequestFactoryEx : IEwsHttpWebRequestFactory
	{
		IEwsHttpWebRequest IEwsHttpWebRequestFactory.CreateRequest(Uri uri)
		{
			return new EwsHttpWebRequestEx(uri)
			{
				ServerCertificateValidationCallback = this.ServerCertificateValidationCallback
			};
		}

		IEwsHttpWebResponse IEwsHttpWebRequestFactory.CreateExceptionResponse(WebException exception)
		{
			return this.originalFactory.CreateExceptionResponse(exception);
		}

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }

		private readonly IEwsHttpWebRequestFactory originalFactory = new EwsHttpWebRequestFactory();
	}
}
