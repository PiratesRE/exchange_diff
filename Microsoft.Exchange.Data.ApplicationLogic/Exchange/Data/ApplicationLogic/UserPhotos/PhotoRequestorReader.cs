using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoRequestorReader
	{
		public PhotoPrincipal Read(HttpContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			return (PhotoPrincipal)context.Items["Photo.Requestor"];
		}

		public bool EnabledInFasterPhotoFlight(HttpContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			return Convert.ToBoolean(context.Items["Photo.Requestor.EnabledInFasterPhotoFlightHttpContextKey"]);
		}

		public const string HttpContextKey = "Photo.Requestor";

		public const string EnabledInFasterPhotoFlightHttpContextKey = "Photo.Requestor.EnabledInFasterPhotoFlightHttpContextKey";
	}
}
