using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Assistants
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IMailboxAssistantSettings_Implementation_ : IMailboxAssistantSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IMailboxAssistantSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IMailboxAssistantSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IMailboxAssistantSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IMailboxAssistantSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IMailboxAssistantSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public bool Enabled
		{
			get
			{
				if (this.dataAccessor._Enabled_ValueProvider_ != null)
				{
					return this.dataAccessor._Enabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Enabled_MaterializedValue_;
			}
		}

		public TimeSpan MailboxNotInterestingLogInterval
		{
			get
			{
				if (this.dataAccessor._MailboxNotInterestingLogInterval_ValueProvider_ != null)
				{
					return this.dataAccessor._MailboxNotInterestingLogInterval_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MailboxNotInterestingLogInterval_MaterializedValue_;
			}
		}

		public bool SpreadLoad
		{
			get
			{
				if (this.dataAccessor._SpreadLoad_ValueProvider_ != null)
				{
					return this.dataAccessor._SpreadLoad_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SpreadLoad_MaterializedValue_;
			}
		}

		public bool SlaMonitoringEnabled
		{
			get
			{
				if (this.dataAccessor._SlaMonitoringEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._SlaMonitoringEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SlaMonitoringEnabled_MaterializedValue_;
			}
		}

		public bool CompletionMonitoringEnabled
		{
			get
			{
				if (this.dataAccessor._CompletionMonitoringEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._CompletionMonitoringEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._CompletionMonitoringEnabled_MaterializedValue_;
			}
		}

		public bool ActiveDatabaseProcessingMonitoringEnabled
		{
			get
			{
				if (this.dataAccessor._ActiveDatabaseProcessingMonitoringEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._ActiveDatabaseProcessingMonitoringEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ActiveDatabaseProcessingMonitoringEnabled_MaterializedValue_;
			}
		}

		public float SlaUrgentThreshold
		{
			get
			{
				if (this.dataAccessor._SlaUrgentThreshold_ValueProvider_ != null)
				{
					return this.dataAccessor._SlaUrgentThreshold_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SlaUrgentThreshold_MaterializedValue_;
			}
		}

		public float SlaNonUrgentThreshold
		{
			get
			{
				if (this.dataAccessor._SlaNonUrgentThreshold_ValueProvider_ != null)
				{
					return this.dataAccessor._SlaNonUrgentThreshold_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SlaNonUrgentThreshold_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IMailboxAssistantSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
