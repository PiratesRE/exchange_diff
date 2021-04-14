using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class LogonIdentifier
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _type;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _value;
	}
}
