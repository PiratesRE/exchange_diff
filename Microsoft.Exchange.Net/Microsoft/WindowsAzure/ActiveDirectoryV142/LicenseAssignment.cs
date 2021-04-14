using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class LicenseAssignment
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid? accountId
		{
			get
			{
				return this._accountId;
			}
			set
			{
				this._accountId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid? skuId
		{
			get
			{
				return this._skuId;
			}
			set
			{
				this._skuId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _accountId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _skuId;
	}
}
