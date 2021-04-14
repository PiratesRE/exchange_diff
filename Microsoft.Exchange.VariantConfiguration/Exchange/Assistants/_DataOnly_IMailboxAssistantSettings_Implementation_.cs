using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Assistants
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_IMailboxAssistantSettings_Implementation_ : IMailboxAssistantSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public bool Enabled
		{
			get
			{
				return this._Enabled_MaterializedValue_;
			}
		}

		public TimeSpan MailboxNotInterestingLogInterval
		{
			get
			{
				return this._MailboxNotInterestingLogInterval_MaterializedValue_;
			}
		}

		public bool SpreadLoad
		{
			get
			{
				return this._SpreadLoad_MaterializedValue_;
			}
		}

		public bool SlaMonitoringEnabled
		{
			get
			{
				return this._SlaMonitoringEnabled_MaterializedValue_;
			}
		}

		public bool CompletionMonitoringEnabled
		{
			get
			{
				return this._CompletionMonitoringEnabled_MaterializedValue_;
			}
		}

		public bool ActiveDatabaseProcessingMonitoringEnabled
		{
			get
			{
				return this._ActiveDatabaseProcessingMonitoringEnabled_MaterializedValue_;
			}
		}

		public float SlaUrgentThreshold
		{
			get
			{
				return this._SlaUrgentThreshold_MaterializedValue_;
			}
		}

		public float SlaNonUrgentThreshold
		{
			get
			{
				return this._SlaNonUrgentThreshold_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal TimeSpan _MailboxNotInterestingLogInterval_MaterializedValue_ = default(TimeSpan);

		internal bool _SpreadLoad_MaterializedValue_;

		internal bool _SlaMonitoringEnabled_MaterializedValue_;

		internal bool _CompletionMonitoringEnabled_MaterializedValue_;

		internal bool _ActiveDatabaseProcessingMonitoringEnabled_MaterializedValue_;

		internal float _SlaUrgentThreshold_MaterializedValue_;

		internal float _SlaNonUrgentThreshold_MaterializedValue_;
	}
}
