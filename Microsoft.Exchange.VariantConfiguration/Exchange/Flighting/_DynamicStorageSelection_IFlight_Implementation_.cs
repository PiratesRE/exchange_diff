using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IFlight_Implementation_ : IFlight, IDataAccessorBackedObject<_DynamicStorageSelection_IFlight_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IFlight_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IFlight_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IFlight_DataAccessor_>.Initialize(_DynamicStorageSelection_IFlight_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public string Rotate
		{
			get
			{
				if (this.dataAccessor._Rotate_ValueProvider_ != null)
				{
					return this.dataAccessor._Rotate_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Rotate_MaterializedValue_;
			}
		}

		public string Ramp
		{
			get
			{
				if (this.dataAccessor._Ramp_ValueProvider_ != null)
				{
					return this.dataAccessor._Ramp_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Ramp_MaterializedValue_;
			}
		}

		public string RampSeed
		{
			get
			{
				if (this.dataAccessor._RampSeed_ValueProvider_ != null)
				{
					return this.dataAccessor._RampSeed_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._RampSeed_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IFlight_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
