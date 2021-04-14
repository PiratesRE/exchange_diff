using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IDiskLatencyMonitorSettings_Implementation_ : IDiskLatencyMonitorSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IDiskLatencyMonitorSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IDiskLatencyMonitorSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IDiskLatencyMonitorSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IDiskLatencyMonitorSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IDiskLatencyMonitorSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public TimeSpan FixedTimeAverageWindowBucket
		{
			get
			{
				if (this.dataAccessor._FixedTimeAverageWindowBucket_ValueProvider_ != null)
				{
					return this.dataAccessor._FixedTimeAverageWindowBucket_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._FixedTimeAverageWindowBucket_MaterializedValue_;
			}
		}

		public int FixedTimeAverageNumberOfBuckets
		{
			get
			{
				if (this.dataAccessor._FixedTimeAverageNumberOfBuckets_ValueProvider_ != null)
				{
					return this.dataAccessor._FixedTimeAverageNumberOfBuckets_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._FixedTimeAverageNumberOfBuckets_MaterializedValue_;
			}
		}

		public TimeSpan ResourceHealthPollerInterval
		{
			get
			{
				if (this.dataAccessor._ResourceHealthPollerInterval_ValueProvider_ != null)
				{
					return this.dataAccessor._ResourceHealthPollerInterval_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ResourceHealthPollerInterval_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IDiskLatencyMonitorSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
