using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UserAgentPhotoExpiresHeader
	{
		private UserAgentPhotoExpiresHeader()
		{
		}

		public string ComputeExpiresHeader(DateTime utcNow, HttpStatusCode statusCode, PhotosConfiguration configuration)
		{
			if (statusCode <= HttpStatusCode.NotModified)
			{
				if (statusCode != HttpStatusCode.OK)
				{
					switch (statusCode)
					{
					case HttpStatusCode.Found:
					case HttpStatusCode.NotModified:
						break;
					case HttpStatusCode.SeeOther:
						goto IL_64;
					default:
						goto IL_64;
					}
				}
				return UserAgentPhotoExpiresHeader.FormatTimestampForExpiresHeader(utcNow.Add(configuration.UserAgentCacheTimeToLive));
			}
			if (statusCode == HttpStatusCode.NotFound)
			{
				return UserAgentPhotoExpiresHeader.FormatTimestampForExpiresHeader(utcNow.Add(configuration.UserAgentCacheTimeToLiveNotFound));
			}
			if (statusCode != HttpStatusCode.InternalServerError)
			{
			}
			IL_64:
			return string.Empty;
		}

		private static string FormatTimestampForExpiresHeader(DateTime timeStamp)
		{
			return timeStamp.ToString(CultureInfo.InvariantCulture.DateTimeFormat.RFC1123Pattern, CultureInfo.InvariantCulture);
		}

		internal static readonly UserAgentPhotoExpiresHeader Default = new UserAgentPhotoExpiresHeader();
	}
}
