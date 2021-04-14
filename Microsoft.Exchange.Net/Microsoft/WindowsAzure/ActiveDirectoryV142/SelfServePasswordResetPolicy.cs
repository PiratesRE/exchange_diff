using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class SelfServePasswordResetPolicy
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string enforcedRegistrationEnablement
		{
			get
			{
				return this._enforcedRegistrationEnablement;
			}
			set
			{
				this._enforcedRegistrationEnablement = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? enforcedRegistrationIntervalInDays
		{
			get
			{
				return this._enforcedRegistrationIntervalInDays;
			}
			set
			{
				this._enforcedRegistrationIntervalInDays = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _enforcedRegistrationEnablement;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _enforcedRegistrationIntervalInDays;
	}
}
