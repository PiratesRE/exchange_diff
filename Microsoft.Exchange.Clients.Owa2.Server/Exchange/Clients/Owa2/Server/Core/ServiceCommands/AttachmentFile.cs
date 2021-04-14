using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal struct AttachmentFile
	{
		public AttachmentFile(string fileName, string url)
		{
			this.FileName = fileName;
			this.FileURL = url;
		}

		public readonly string FileName;

		public readonly string FileURL;
	}
}
