using System;
using System.CodeDom.Compiler;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	[DataServiceKey("objectId")]
	public class DirectAccessGrant : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static DirectAccessGrant CreateDirectAccessGrant(string objectId, Guid permissionId)
		{
			return new DirectAccessGrant
			{
				objectId = objectId,
				permissionId = permissionId
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? creationTimestamp
		{
			get
			{
				return this._creationTimestamp;
			}
			set
			{
				this._creationTimestamp = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid permissionId
		{
			get
			{
				return this._permissionId;
			}
			set
			{
				this._permissionId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string principalDisplayName
		{
			get
			{
				return this._principalDisplayName;
			}
			set
			{
				this._principalDisplayName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid? principalId
		{
			get
			{
				return this._principalId;
			}
			set
			{
				this._principalId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string principalType
		{
			get
			{
				return this._principalType;
			}
			set
			{
				this._principalType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string resourceDisplayName
		{
			get
			{
				return this._resourceDisplayName;
			}
			set
			{
				this._resourceDisplayName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid? resourceId
		{
			get
			{
				return this._resourceId;
			}
			set
			{
				this._resourceId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _creationTimestamp;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid _permissionId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _principalDisplayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _principalId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _principalType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _resourceDisplayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _resourceId;
	}
}
