using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.AirSync
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IMdmNotificationSettings_DataAccessor_ : VariantObjectDataAccessorBase<IMdmNotificationSettings, _DynamicStorageSelection_IMdmNotificationSettings_Implementation_, _DynamicStorageSelection_IMdmNotificationSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal Uri _EnrollmentUrl_MaterializedValue_;

		internal ValueProvider<Uri> _EnrollmentUrl_ValueProvider_;

		internal Uri _ComplianceStatusUrl_MaterializedValue_;

		internal ValueProvider<Uri> _ComplianceStatusUrl_ValueProvider_;

		internal string _ADRegistrationServiceHost_MaterializedValue_;

		internal ValueProvider<string> _ADRegistrationServiceHost_ValueProvider_;

		internal Uri _EnrollmentUrlWithBasicSteps_MaterializedValue_;

		internal ValueProvider<Uri> _EnrollmentUrlWithBasicSteps_ValueProvider_;

		internal string _ActivationUrlWithBasicSteps_MaterializedValue_;

		internal ValueProvider<string> _ActivationUrlWithBasicSteps_ValueProvider_;

		internal TimeSpan _DeviceStatusCacheExpirationInternal_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _DeviceStatusCacheExpirationInternal_ValueProvider_;

		internal TimeSpan _NegativeDeviceStatusCacheExpirationInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _NegativeDeviceStatusCacheExpirationInterval_ValueProvider_;

		internal bool _PolicyEvaluationEnabled_MaterializedValue_;

		internal ValueProvider<bool> _PolicyEvaluationEnabled_ValueProvider_;
	}
}
