using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.VariantConfiguration.Settings
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IOrganizationNameSettings_Implementation_ : IOrganizationNameSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IOrganizationNameSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IOrganizationNameSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IOrganizationNameSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IOrganizationNameSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IOrganizationNameSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public IList<string> OrgNames
		{
			get
			{
				if (this.dataAccessor._OrgNames_ValueProvider_ != null)
				{
					return this.dataAccessor._OrgNames_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._OrgNames_MaterializedValue_;
			}
		}

		public IList<string> DogfoodOrgNames
		{
			get
			{
				if (this.dataAccessor._DogfoodOrgNames_ValueProvider_ != null)
				{
					return this.dataAccessor._DogfoodOrgNames_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DogfoodOrgNames_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IOrganizationNameSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
