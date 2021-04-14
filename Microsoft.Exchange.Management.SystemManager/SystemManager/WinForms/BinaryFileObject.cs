using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class BinaryFileObject
	{
		public BinaryFileObject(string fileName, byte[] fileData)
		{
			this.FileName = fileName;
			this.FileData = fileData;
		}

		public string FileName { get; private set; }

		public byte[] FileData { get; private set; }
	}
}
