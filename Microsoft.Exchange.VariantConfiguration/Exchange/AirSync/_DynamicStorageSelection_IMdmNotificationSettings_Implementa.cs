using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.AirSync
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IMdmNotificationSettings_Implementation_ : IMdmNotificationSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IMdmNotificationSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IMdmNotificationSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IMdmNotificationSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IMdmNotificationSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IMdmNotificationSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
		{
			this.dataAccessor = dataAccessor;
			this.context = context;
		}

		public string Name
		{
			get
			{
				return this.dataAccessor._Name_MaterializedValue_;
			}
		}

		public Uri EnrollmentUrl
		{
			get
			{
				if (this.dataAccessor._EnrollmentUrl_ValueProvider_ != null)
				{
					return this.dataAccessor._EnrollmentUrl_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._EnrollmentUrl_MaterializedValue_;
			}
		}

		public Uri ComplianceStatusUrl
		{
			get
			{
				if (this.dataAccessor._ComplianceStatusUrl_ValueProvider_ != null)
				{
					return this.dataAccessor._ComplianceStatusUrl_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ComplianceStatusUrl_MaterializedValue_;
			}
		}

		public string ADRegistrationServiceHost
		{
			get
			{
				if (this.dataAccessor._ADRegistrationServiceHost_ValueProvider_ != null)
				{
					return this.dataAccessor._ADRegistrationServiceHost_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ADRegistrationServiceHost_MaterializedValue_;
			}
		}

		public Uri EnrollmentUrlWithBasicSteps
		{
			get
			{
				if (this.dataAccessor._EnrollmentUrlWithBasicSteps_ValueProvider_ != null)
				{
					return this.dataAccessor._EnrollmentUrlWithBasicSteps_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._EnrollmentUrlWithBasicSteps_MaterializedValue_;
			}
		}

		public string ActivationUrlWithBasicSteps
		{
			get
			{
				if (this.dataAccessor._ActivationUrlWithBasicSteps_ValueProvider_ != null)
				{
					return this.dataAccessor._ActivationUrlWithBasicSteps_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ActivationUrlWithBasicSteps_MaterializedValue_;
			}
		}

		public TimeSpan DeviceStatusCacheExpirationInternal
		{
			get
			{
				if (this.dataAccessor._DeviceStatusCacheExpirationInternal_ValueProvider_ != null)
				{
					return this.dataAccessor._DeviceStatusCacheExpirationInternal_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DeviceStatusCacheExpirationInternal_MaterializedValue_;
			}
		}

		public TimeSpan NegativeDeviceStatusCacheExpirationInterval
		{
			get
			{
				if (this.dataAccessor._NegativeDeviceStatusCacheExpirationInterval_ValueProvider_ != null)
				{
					return this.dataAccessor._NegativeDeviceStatusCacheExpirationInterval_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NegativeDeviceStatusCacheExpirationInterval_MaterializedValue_;
			}
		}

		public bool PolicyEvaluationEnabled
		{
			get
			{
				if (this.dataAccessor._PolicyEvaluationEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._PolicyEvaluationEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._PolicyEvaluationEnabled_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IMdmNotificationSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
