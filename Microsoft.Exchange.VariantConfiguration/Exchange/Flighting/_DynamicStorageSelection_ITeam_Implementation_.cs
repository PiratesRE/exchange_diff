using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_ITeam_Implementation_ : ITeam, IDataAccessorBackedObject<_DynamicStorageSelection_ITeam_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_ITeam_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_ITeam_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_ITeam_DataAccessor_>.Initialize(_DynamicStorageSelection_ITeam_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public bool IsMember
		{
			get
			{
				if (this.dataAccessor._IsMember_ValueProvider_ != null)
				{
					return this.dataAccessor._IsMember_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._IsMember_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_ITeam_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
