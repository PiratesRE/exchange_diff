using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Assistants
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_ICalendarRepairLoggerSettings_Implementation_ : ICalendarRepairLoggerSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public bool InsightLogEnabled
		{
			get
			{
				return this._InsightLogEnabled_MaterializedValue_;
			}
		}

		public string InsightLogDirectoryName
		{
			get
			{
				return this._InsightLogDirectoryName_MaterializedValue_;
			}
		}

		public TimeSpan InsightLogFileAgeInDays
		{
			get
			{
				return this._InsightLogFileAgeInDays_MaterializedValue_;
			}
		}

		public ulong InsightLogDirectorySizeLimit
		{
			get
			{
				return this._InsightLogDirectorySizeLimit_MaterializedValue_;
			}
		}

		public ulong InsightLogFileSize
		{
			get
			{
				return this._InsightLogFileSize_MaterializedValue_;
			}
		}

		public ulong InsightLogCacheSize
		{
			get
			{
				return this._InsightLogCacheSize_MaterializedValue_;
			}
		}

		public TimeSpan InsightLogFlushIntervalInSeconds
		{
			get
			{
				return this._InsightLogFlushIntervalInSeconds_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _InsightLogEnabled_MaterializedValue_;

		internal string _InsightLogDirectoryName_MaterializedValue_;

		internal TimeSpan _InsightLogFileAgeInDays_MaterializedValue_ = default(TimeSpan);

		internal ulong _InsightLogDirectorySizeLimit_MaterializedValue_;

		internal ulong _InsightLogFileSize_MaterializedValue_;

		internal ulong _InsightLogCacheSize_MaterializedValue_;

		internal TimeSpan _InsightLogFlushIntervalInSeconds_MaterializedValue_ = default(TimeSpan);
	}
}
