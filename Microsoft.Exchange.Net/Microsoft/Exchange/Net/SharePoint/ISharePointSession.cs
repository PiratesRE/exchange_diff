using System;
using System.Collections.Specialized;
using System.IO;

namespace Microsoft.Exchange.Net.SharePoint
{
	public interface ISharePointSession
	{
		bool DoesFileExist(string fileUrl);

		string UploadFile(string fileUrl, Stream inStream, Action heartbeat, out NameValueCollection propertyBag);

		void DownloadFile(string fileUrl, SharepointFileDownloadHelper writeStream);
	}
}
