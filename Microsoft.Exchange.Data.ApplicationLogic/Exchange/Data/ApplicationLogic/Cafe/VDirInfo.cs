using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class VDirInfo
	{
		internal Uri ExternalUri { get; private set; }

		internal string Path { get; private set; }

		internal VDirInfo(Uri externalUri)
		{
			ArgumentValidator.ThrowIfNull("externalUri", externalUri);
			this.ExternalUri = externalUri;
			this.Path = this.ExternalUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
		}
	}
}
