using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PhotoRequestOutboundSender : IPhotoRequestOutboundSender
	{
		public PhotoRequestOutboundSender(IPhotoRequestOutboundAuthenticator authenticator)
		{
			ArgumentValidator.ThrowIfNull("authenticator", authenticator);
			this.authenticator = authenticator;
		}

		public HttpWebResponse SendAndGetResponse(HttpWebRequest request)
		{
			return this.authenticator.AuthenticateAndGetResponse(request);
		}

		private readonly IPhotoRequestOutboundAuthenticator authenticator;
	}
}
