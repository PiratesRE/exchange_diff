using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_IDiskLatencyMonitorSettings_Implementation_ : IDiskLatencyMonitorSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public TimeSpan FixedTimeAverageWindowBucket
		{
			get
			{
				return this._FixedTimeAverageWindowBucket_MaterializedValue_;
			}
		}

		public int FixedTimeAverageNumberOfBuckets
		{
			get
			{
				return this._FixedTimeAverageNumberOfBuckets_MaterializedValue_;
			}
		}

		public TimeSpan ResourceHealthPollerInterval
		{
			get
			{
				return this._ResourceHealthPollerInterval_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal TimeSpan _FixedTimeAverageWindowBucket_MaterializedValue_ = default(TimeSpan);

		internal int _FixedTimeAverageNumberOfBuckets_MaterializedValue_;

		internal TimeSpan _ResourceHealthPollerInterval_MaterializedValue_ = default(TimeSpan);
	}
}
