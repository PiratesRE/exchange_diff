using System;
using System.CodeDom.Compiler;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV122
{
	[DataServiceKey("objectId")]
	public class DirectoryLinkChange : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static DirectoryLinkChange CreateDirectoryLinkChange(string objectId)
		{
			return new DirectoryLinkChange
			{
				objectId = objectId
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string associationType
		{
			get
			{
				return this._associationType;
			}
			set
			{
				this._associationType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string sourceObjectId
		{
			get
			{
				return this._sourceObjectId;
			}
			set
			{
				this._sourceObjectId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string sourceObjectType
		{
			get
			{
				return this._sourceObjectType;
			}
			set
			{
				this._sourceObjectType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string sourceObjectUri
		{
			get
			{
				return this._sourceObjectUri;
			}
			set
			{
				this._sourceObjectUri = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string targetObjectId
		{
			get
			{
				return this._targetObjectId;
			}
			set
			{
				this._targetObjectId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string targetObjectType
		{
			get
			{
				return this._targetObjectType;
			}
			set
			{
				this._targetObjectType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string targetObjectUri
		{
			get
			{
				return this._targetObjectUri;
			}
			set
			{
				this._targetObjectUri = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _associationType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _sourceObjectId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _sourceObjectType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _sourceObjectUri;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _targetObjectId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _targetObjectType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _targetObjectUri;
	}
}
