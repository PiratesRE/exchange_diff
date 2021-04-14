using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	public class ImpersonationAccessGrantType
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string impersonated
		{
			get
			{
				return this._impersonated;
			}
			set
			{
				this._impersonated = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string impersonator
		{
			get
			{
				return this._impersonator;
			}
			set
			{
				this._impersonator = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _impersonated;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _impersonator;
	}
}
