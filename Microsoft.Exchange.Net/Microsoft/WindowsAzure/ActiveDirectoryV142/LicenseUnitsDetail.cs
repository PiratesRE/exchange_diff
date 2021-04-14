using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class LicenseUnitsDetail
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? enabled
		{
			get
			{
				return this._enabled;
			}
			set
			{
				this._enabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? suspended
		{
			get
			{
				return this._suspended;
			}
			set
			{
				this._suspended = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? warning
		{
			get
			{
				return this._warning;
			}
			set
			{
				this._warning = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _enabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _suspended;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _warning;
	}
}
