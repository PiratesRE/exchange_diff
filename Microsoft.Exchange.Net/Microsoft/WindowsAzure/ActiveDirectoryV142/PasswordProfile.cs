using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class PasswordProfile
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string password
		{
			get
			{
				return this._password;
			}
			set
			{
				this._password = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? forceChangePasswordNextLogin
		{
			get
			{
				return this._forceChangePasswordNextLogin;
			}
			set
			{
				this._forceChangePasswordNextLogin = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _password;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _forceChangePasswordNextLogin;
	}
}
