using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public enum UploadHandlers
	{
		None,
		UMAutoAttendantService,
		UMDialPlanService,
		AddExtensionService,
		OrgAddExtensionService,
		UserPhotoService,
		FileEncodeUploadHandler,
		MigrationCsvUploadHandler,
		FingerprintUploadHandler
	}
}
