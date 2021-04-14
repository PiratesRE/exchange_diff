using System;
using System.CodeDom.Compiler;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	[DataServiceKey("objectId")]
	public class RoleTemplate : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static RoleTemplate CreateRoleTemplate(string objectId)
		{
			return new RoleTemplate
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
		private string _description;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;
	}
}
