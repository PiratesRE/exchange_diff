using System;
using System.CodeDom.Compiler;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("featureId")]
	public class EnabledFeature
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static EnabledFeature CreateEnabledFeature(string featureId)
		{
			return new EnabledFeature
			{
				featureId = featureId
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string featureId
		{
			get
			{
				return this._featureId;
			}
			set
			{
				this._featureId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string featureName
		{
			get
			{
				return this._featureName;
			}
			set
			{
				this._featureName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _featureId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _featureName;
	}
}
