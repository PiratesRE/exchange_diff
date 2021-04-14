using System;
using System.CodeDom.Compiler;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	[DataServiceKey("objectId")]
	public class Role : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static Role CreateRole(string objectId)
		{
			return new Role
			{
				objectId = objectId
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string description
		{
			get
			{
				return this._description;
			}
			set
			{
				this._description = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string displayName
		{
			get
			{
				return this._displayName;
			}
			set
			{
				this._displayName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? isSystem
		{
			get
			{
				return this._isSystem;
			}
			set
			{
				this._isSystem = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? roleDisabled
		{
			get
			{
				return this._roleDisabled;
			}
			set
			{
				this._roleDisabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _description;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _isSystem;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _roleDisabled;
	}
}
