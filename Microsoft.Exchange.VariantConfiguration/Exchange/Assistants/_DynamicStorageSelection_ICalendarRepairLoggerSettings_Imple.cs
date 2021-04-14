using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Assistants
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_ICalendarRepairLoggerSettings_Implementation_ : ICalendarRepairLoggerSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_ICalendarRepairLoggerSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_ICalendarRepairLoggerSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_ICalendarRepairLoggerSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_ICalendarRepairLoggerSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_ICalendarRepairLoggerSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public bool InsightLogEnabled
		{
			get
			{
				if (this.dataAccessor._InsightLogEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._InsightLogEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InsightLogEnabled_MaterializedValue_;
			}
		}

		public string InsightLogDirectoryName
		{
			get
			{
				if (this.dataAccessor._InsightLogDirectoryName_ValueProvider_ != null)
				{
					return this.dataAccessor._InsightLogDirectoryName_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InsightLogDirectoryName_MaterializedValue_;
			}
		}

		public TimeSpan InsightLogFileAgeInDays
		{
			get
			{
				if (this.dataAccessor._InsightLogFileAgeInDays_ValueProvider_ != null)
				{
					return this.dataAccessor._InsightLogFileAgeInDays_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InsightLogFileAgeInDays_MaterializedValue_;
			}
		}

		public ulong InsightLogDirectorySizeLimit
		{
			get
			{
				if (this.dataAccessor._InsightLogDirectorySizeLimit_ValueProvider_ != null)
				{
					return this.dataAccessor._InsightLogDirectorySizeLimit_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InsightLogDirectorySizeLimit_MaterializedValue_;
			}
		}

		public ulong InsightLogFileSize
		{
			get
			{
				if (this.dataAccessor._InsightLogFileSize_ValueProvider_ != null)
				{
					return this.dataAccessor._InsightLogFileSize_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InsightLogFileSize_MaterializedValue_;
			}
		}

		public ulong InsightLogCacheSize
		{
			get
			{
				if (this.dataAccessor._InsightLogCacheSize_ValueProvider_ != null)
				{
					return this.dataAccessor._InsightLogCacheSize_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InsightLogCacheSize_MaterializedValue_;
			}
		}

		public TimeSpan InsightLogFlushIntervalInSeconds
		{
			get
			{
				if (this.dataAccessor._InsightLogFlushIntervalInSeconds_ValueProvider_ != null)
				{
					return this.dataAccessor._InsightLogFlushIntervalInSeconds_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InsightLogFlushIntervalInSeconds_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_ICalendarRepairLoggerSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
