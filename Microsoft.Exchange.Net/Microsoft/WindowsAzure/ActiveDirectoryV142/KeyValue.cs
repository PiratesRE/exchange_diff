using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class KeyValue
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string key
		{
			get
			{
				return this._key;
			}
			set
			{
				this._key = value;
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
		private string _key;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _value;
	}
}
