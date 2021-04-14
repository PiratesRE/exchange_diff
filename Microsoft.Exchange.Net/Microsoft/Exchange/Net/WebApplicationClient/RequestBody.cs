using System;
using System.IO;
using System.Net.Mime;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public abstract class RequestBody
	{
		public ContentType ContentType { get; set; }

		public abstract void Write(Stream writeStream);

		public static class MediaTypes
		{
			public const string FormUrlEncoded = "application/x-www-form-urlencoded";
		}
	}
}
