using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.HolidayCalendars
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IHostSettings_Implementation_ : IHostSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public string Endpoint
		{
			get
			{
				return this._Endpoint_MaterializedValue_;
			}
		}

		public int Timeout
		{
			get
			{
				return this._Timeout_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal string _Endpoint_MaterializedValue_;

		internal int _Timeout_MaterializedValue_;
	}
}
