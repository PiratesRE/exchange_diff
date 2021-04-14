using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IResourceSettings_Implementation_ : IResourceSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public int MaxConcurrency
		{
			get
			{
				return this._MaxConcurrency_MaterializedValue_;
			}
		}

		public int DiscretionaryUnderloaded
		{
			get
			{
				return this._DiscretionaryUnderloaded_MaterializedValue_;
			}
		}

		public int DiscretionaryOverloaded
		{
			get
			{
				return this._DiscretionaryOverloaded_MaterializedValue_;
			}
		}

		public int DiscretionaryCritical
		{
			get
			{
				return this._DiscretionaryCritical_MaterializedValue_;
			}
		}

		public int InternalMaintenanceUnderloaded
		{
			get
			{
				return this._InternalMaintenanceUnderloaded_MaterializedValue_;
			}
		}

		public int InternalMaintenanceOverloaded
		{
			get
			{
				return this._InternalMaintenanceOverloaded_MaterializedValue_;
			}
		}

		public int InternalMaintenanceCritical
		{
			get
			{
				return this._InternalMaintenanceCritical_MaterializedValue_;
			}
		}

		public int CustomerExpectationUnderloaded
		{
			get
			{
				return this._CustomerExpectationUnderloaded_MaterializedValue_;
			}
		}

		public int CustomerExpectationOverloaded
		{
			get
			{
				return this._CustomerExpectationOverloaded_MaterializedValue_;
			}
		}

		public int CustomerExpectationCritical
		{
			get
			{
				return this._CustomerExpectationCritical_MaterializedValue_;
			}
		}

		public int UrgentUnderloaded
		{
			get
			{
				return this._UrgentUnderloaded_MaterializedValue_;
			}
		}

		public int UrgentOverloaded
		{
			get
			{
				return this._UrgentOverloaded_MaterializedValue_;
			}
		}

		public int UrgentCritical
		{
			get
			{
				return this._UrgentCritical_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal int _MaxConcurrency_MaterializedValue_;

		internal int _DiscretionaryUnderloaded_MaterializedValue_;

		internal int _DiscretionaryOverloaded_MaterializedValue_;

		internal int _DiscretionaryCritical_MaterializedValue_;

		internal int _InternalMaintenanceUnderloaded_MaterializedValue_;

		internal int _InternalMaintenanceOverloaded_MaterializedValue_;

		internal int _InternalMaintenanceCritical_MaterializedValue_;

		internal int _CustomerExpectationUnderloaded_MaterializedValue_;

		internal int _CustomerExpectationOverloaded_MaterializedValue_;

		internal int _CustomerExpectationCritical_MaterializedValue_;

		internal int _UrgentUnderloaded_MaterializedValue_;

		internal int _UrgentOverloaded_MaterializedValue_;

		internal int _UrgentCritical_MaterializedValue_;
	}
}
