using System;
using System.IO;
using System.IO.Compression;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class FileEncodeUploadHandler : IUploadHandler
	{
		public static Stream DecodeContent(string content)
		{
			return new GZipStream(new MemoryStream(Convert.FromBase64String(content)), CompressionMode.Decompress);
		}

		public virtual Type SetParameterType
		{
			get
			{
				return typeof(BaseWebServiceParameters);
			}
		}

		public virtual PowerShellResults ProcessUpload(UploadFileContext context, WebServiceParameters parameters)
		{
			PowerShellResults result;
			using (MemoryStream memoryStream = new MemoryStream(Math.Max((int)context.FileStream.Length / 2, 1024)))
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					context.FileStream.CopyTo(gzipStream);
				}
				result = new PowerShellResults<EncodedFile>
				{
					Output = new EncodedFile[]
					{
						new EncodedFile
						{
							FileContent = Convert.ToBase64String(memoryStream.ToArray())
						}
					}
				};
			}
			return result;
		}

		public virtual int MaxFileSize
		{
			get
			{
				return 10485760;
			}
		}
	}
}
