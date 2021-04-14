using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_ICmdletSettings_Implementation_ : ICmdletSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_ICmdletSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_ICmdletSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_ICmdletSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_ICmdletSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_ICmdletSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public IList<string> AllFlightingParams
		{
			get
			{
				if (this.dataAccessor._AllFlightingParams_ValueProvider_ != null)
				{
					return this.dataAccessor._AllFlightingParams_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._AllFlightingParams_MaterializedValue_;
			}
		}

		public IList<string> Params0
		{
			get
			{
				if (this.dataAccessor._Params0_ValueProvider_ != null)
				{
					return this.dataAccessor._Params0_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params0_MaterializedValue_;
			}
		}

		public IList<string> Params1
		{
			get
			{
				if (this.dataAccessor._Params1_ValueProvider_ != null)
				{
					return this.dataAccessor._Params1_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params1_MaterializedValue_;
			}
		}

		public IList<string> Params2
		{
			get
			{
				if (this.dataAccessor._Params2_ValueProvider_ != null)
				{
					return this.dataAccessor._Params2_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params2_MaterializedValue_;
			}
		}

		public IList<string> Params3
		{
			get
			{
				if (this.dataAccessor._Params3_ValueProvider_ != null)
				{
					return this.dataAccessor._Params3_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params3_MaterializedValue_;
			}
		}

		public IList<string> Params4
		{
			get
			{
				if (this.dataAccessor._Params4_ValueProvider_ != null)
				{
					return this.dataAccessor._Params4_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params4_MaterializedValue_;
			}
		}

		public IList<string> Params5
		{
			get
			{
				if (this.dataAccessor._Params5_ValueProvider_ != null)
				{
					return this.dataAccessor._Params5_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params5_MaterializedValue_;
			}
		}

		public IList<string> Params6
		{
			get
			{
				if (this.dataAccessor._Params6_ValueProvider_ != null)
				{
					return this.dataAccessor._Params6_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params6_MaterializedValue_;
			}
		}

		public IList<string> Params7
		{
			get
			{
				if (this.dataAccessor._Params7_ValueProvider_ != null)
				{
					return this.dataAccessor._Params7_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params7_MaterializedValue_;
			}
		}

		public IList<string> Params8
		{
			get
			{
				if (this.dataAccessor._Params8_ValueProvider_ != null)
				{
					return this.dataAccessor._Params8_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params8_MaterializedValue_;
			}
		}

		public IList<string> Params9
		{
			get
			{
				if (this.dataAccessor._Params9_ValueProvider_ != null)
				{
					return this.dataAccessor._Params9_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Params9_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_ICmdletSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
