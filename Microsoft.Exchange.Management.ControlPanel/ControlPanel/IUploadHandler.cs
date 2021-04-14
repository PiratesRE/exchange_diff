using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public interface IUploadHandler
	{
		Type SetParameterType { get; }

		PowerShellResults ProcessUpload(UploadFileContext context, WebServiceParameters parameters);

		int MaxFileSize { get; }
	}
}
