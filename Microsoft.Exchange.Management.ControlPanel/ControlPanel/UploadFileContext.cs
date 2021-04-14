using System;
using System.IO;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class UploadFileContext
	{
		public UploadFileContext(string fileName, Stream fileStream)
		{
			this.FileName = fileName;
			this.FileStream = fileStream;
		}

		public string FileName { get; private set; }

		public Stream FileStream { get; private set; }
	}
}
