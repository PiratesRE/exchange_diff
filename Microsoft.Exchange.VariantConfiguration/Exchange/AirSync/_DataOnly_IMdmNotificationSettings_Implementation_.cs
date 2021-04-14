using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.AirSync
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IMdmNotificationSettings_Implementation_ : IMdmNotificationSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return null;
			}
		}

		IVariantObjectInstance IVariantObjectInstanceProvider.GetVariantObjectInstance(VariantContextSnapshot context)
		{
			return this;
		}

		public string Name
		{
			get
			{
				return this._Name_MaterializedValue_;
			}
		}

		public Uri EnrollmentUrl
		{
			get
			{
				return this._EnrollmentUrl_MaterializedValue_;
			}
		}

		public Uri ComplianceStatusUrl
		{
			get
			{
				return this._ComplianceStatusUrl_MaterializedValue_;
			}
		}

		public string ADRegistrationServiceHost
		{
			get
			{
				return this._ADRegistrationServiceHost_MaterializedValue_;
			}
		}

		public Uri EnrollmentUrlWithBasicSteps
		{
			get
			{
				return this._EnrollmentUrlWithBasicSteps_MaterializedValue_;
			}
		}

		public string ActivationUrlWithBasicSteps
		{
			get
			{
				return this._ActivationUrlWithBasicSteps_MaterializedValue_;
			}
		}

		public TimeSpan DeviceStatusCacheExpirationInternal
		{
			get
			{
				return this._DeviceStatusCacheExpirationInternal_MaterializedValue_;
			}
		}

		public TimeSpan NegativeDeviceStatusCacheExpirationInterval
		{
			get
			{
				return this._NegativeDeviceStatusCacheExpirationInterval_MaterializedValue_;
			}
		}

		public bool PolicyEvaluationEnabled
		{
			get
			{
				return this._PolicyEvaluationEnabled_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal Uri _EnrollmentUrl_MaterializedValue_;

		internal Uri _ComplianceStatusUrl_MaterializedValue_;

		internal string _ADRegistrationServiceHost_MaterializedValue_;

		internal Uri _EnrollmentUrlWithBasicSteps_MaterializedValue_;

		internal string _ActivationUrlWithBasicSteps_MaterializedValue_;

		internal TimeSpan _DeviceStatusCacheExpirationInternal_MaterializedValue_ = default(TimeSpan);

		internal TimeSpan _NegativeDeviceStatusCacheExpirationInterval_MaterializedValue_ = default(TimeSpan);

		internal bool _PolicyEvaluationEnabled_MaterializedValue_;
	}
}
