using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal sealed class ScopeNotificationUploadState
	{
		internal ScopeNotificationRawData Data { get; set; }

		internal int LastUploadedHealthState { get; set; }

		internal DateTime LastSucceededUpload { get; set; }

		internal DateTime LastFailedUpload { get; set; }
	}
}
