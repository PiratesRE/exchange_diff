using System;
using System.IO;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal sealed class SerializedAccessTokenBody : RequestBody
	{
		public SerializedAccessTokenBody(SerializedAccessToken accessToken)
		{
			if (accessToken == null)
			{
				throw new ArgumentNullException("accessToken");
			}
			this.accessToken = accessToken;
		}

		public sealed override void Write(Stream writeStream)
		{
			this.accessToken.Serialize(writeStream);
		}

		private SerializedAccessToken accessToken;
	}
}
