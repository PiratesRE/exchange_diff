using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IPhotoRequestOutboundAuthenticator
	{
		HttpWebResponse AuthenticateAndGetResponse(HttpWebRequest request);
	}
}
