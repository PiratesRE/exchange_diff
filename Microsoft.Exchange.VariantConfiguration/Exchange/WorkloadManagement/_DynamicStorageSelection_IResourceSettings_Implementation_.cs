using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IResourceSettings_Implementation_ : IResourceSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IResourceSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IResourceSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IResourceSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IResourceSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IResourceSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public int MaxConcurrency
		{
			get
			{
				if (this.dataAccessor._MaxConcurrency_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxConcurrency_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxConcurrency_MaterializedValue_;
			}
		}

		public int DiscretionaryUnderloaded
		{
			get
			{
				if (this.dataAccessor._DiscretionaryUnderloaded_ValueProvider_ != null)
				{
					return this.dataAccessor._DiscretionaryUnderloaded_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DiscretionaryUnderloaded_MaterializedValue_;
			}
		}

		public int DiscretionaryOverloaded
		{
			get
			{
				if (this.dataAccessor._DiscretionaryOverloaded_ValueProvider_ != null)
				{
					return this.dataAccessor._DiscretionaryOverloaded_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DiscretionaryOverloaded_MaterializedValue_;
			}
		}

		public int DiscretionaryCritical
		{
			get
			{
				if (this.dataAccessor._DiscretionaryCritical_ValueProvider_ != null)
				{
					return this.dataAccessor._DiscretionaryCritical_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DiscretionaryCritical_MaterializedValue_;
			}
		}

		public int InternalMaintenanceUnderloaded
		{
			get
			{
				if (this.dataAccessor._InternalMaintenanceUnderloaded_ValueProvider_ != null)
				{
					return this.dataAccessor._InternalMaintenanceUnderloaded_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InternalMaintenanceUnderloaded_MaterializedValue_;
			}
		}

		public int InternalMaintenanceOverloaded
		{
			get
			{
				if (this.dataAccessor._InternalMaintenanceOverloaded_ValueProvider_ != null)
				{
					return this.dataAccessor._InternalMaintenanceOverloaded_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InternalMaintenanceOverloaded_MaterializedValue_;
			}
		}

		public int InternalMaintenanceCritical
		{
			get
			{
				if (this.dataAccessor._InternalMaintenanceCritical_ValueProvider_ != null)
				{
					return this.dataAccessor._InternalMaintenanceCritical_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._InternalMaintenanceCritical_MaterializedValue_;
			}
		}

		public int CustomerExpectationUnderloaded
		{
			get
			{
				if (this.dataAccessor._CustomerExpectationUnderloaded_ValueProvider_ != null)
				{
					return this.dataAccessor._CustomerExpectationUnderloaded_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._CustomerExpectationUnderloaded_MaterializedValue_;
			}
		}

		public int CustomerExpectationOverloaded
		{
			get
			{
				if (this.dataAccessor._CustomerExpectationOverloaded_ValueProvider_ != null)
				{
					return this.dataAccessor._CustomerExpectationOverloaded_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._CustomerExpectationOverloaded_MaterializedValue_;
			}
		}

		public int CustomerExpectationCritical
		{
			get
			{
				if (this.dataAccessor._CustomerExpectationCritical_ValueProvider_ != null)
				{
					return this.dataAccessor._CustomerExpectationCritical_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._CustomerExpectationCritical_MaterializedValue_;
			}
		}

		public int UrgentUnderloaded
		{
			get
			{
				if (this.dataAccessor._UrgentUnderloaded_ValueProvider_ != null)
				{
					return this.dataAccessor._UrgentUnderloaded_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._UrgentUnderloaded_MaterializedValue_;
			}
		}

		public int UrgentOverloaded
		{
			get
			{
				if (this.dataAccessor._UrgentOverloaded_ValueProvider_ != null)
				{
					return this.dataAccessor._UrgentOverloaded_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._UrgentOverloaded_MaterializedValue_;
			}
		}

		public int UrgentCritical
		{
			get
			{
				if (this.dataAccessor._UrgentCritical_ValueProvider_ != null)
				{
					return this.dataAccessor._UrgentCritical_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._UrgentCritical_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IResourceSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
