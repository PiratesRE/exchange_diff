using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.TextProcessing.Boomerang
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_IBoomerangSettings_Implementation_ : IBoomerangSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public string MasterKeyRegistryPath
		{
			get
			{
				return this._MasterKeyRegistryPath_MaterializedValue_;
			}
		}

		public string MasterKeyRegistryKeyName
		{
			get
			{
				return this._MasterKeyRegistryKeyName_MaterializedValue_;
			}
		}

		public uint NumberOfValidIntervalsInDays
		{
			get
			{
				return this._NumberOfValidIntervalsInDays_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal string _MasterKeyRegistryPath_MaterializedValue_;

		internal string _MasterKeyRegistryKeyName_MaterializedValue_;

		internal uint _NumberOfValidIntervalsInDays_MaterializedValue_;
	}
}
