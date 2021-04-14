using System;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class FingerprintUploadHandler : DataSourceService, IUploadHandler
	{
		public Type SetParameterType
		{
			get
			{
				return typeof(FingerprintUploadParameters);
			}
		}

		public PowerShellResults ProcessUpload(UploadFileContext context, WebServiceParameters parameters)
		{
			parameters.FaultIfNull();
			if (parameters is FingerprintUploadParameters)
			{
				FingerprintUploadParameters fingerprintUploadParameters = (FingerprintUploadParameters)parameters;
				byte[] array = new byte[context.FileStream.Length];
				context.FileStream.Read(array, 0, array.Length);
				fingerprintUploadParameters.FileData = array;
				string description = context.FileName.Substring(context.FileName.LastIndexOf("\\") + 1);
				fingerprintUploadParameters.Description = description;
				return base.NewObject<Fingerprint, FingerprintUploadParameters>("New-Fingerprint", fingerprintUploadParameters);
			}
			return null;
		}

		public int MaxFileSize
		{
			get
			{
				return FingerprintUploadHandler.maxFileSize;
			}
		}

		private static int maxFileSize = AppConfigLoader.GetConfigIntValue("MaxFileSize", 0, 20971520, 20971520);
	}
}
