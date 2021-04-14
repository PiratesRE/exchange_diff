using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class SelfServePasswordResetData
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? lastRegisteredTime
		{
			get
			{
				return this._lastRegisteredTime;
			}
			set
			{
				this._lastRegisteredTime = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _lastRegisteredTime;
	}
}
