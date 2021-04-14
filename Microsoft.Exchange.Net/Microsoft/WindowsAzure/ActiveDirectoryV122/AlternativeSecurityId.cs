using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV122
{
	public class AlternativeSecurityId
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? type
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
		public string identityProvider
		{
			get
			{
				return this._identityProvider;
			}
			set
			{
				this._identityProvider = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public byte[] key
		{
			get
			{
				if (this._key != null)
				{
					return (byte[])this._key.Clone();
				}
				return null;
			}
			set
			{
				this._key = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _type;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _identityProvider;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private byte[] _key;
	}
}
