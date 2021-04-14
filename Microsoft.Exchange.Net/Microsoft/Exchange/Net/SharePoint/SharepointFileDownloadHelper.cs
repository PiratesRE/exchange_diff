using System;
using System.IO;

namespace Microsoft.Exchange.Net.SharePoint
{
	public delegate void SharepointFileDownloadHelper(string fileName, Stream inputStream, int contentLength, string contentType);
}
