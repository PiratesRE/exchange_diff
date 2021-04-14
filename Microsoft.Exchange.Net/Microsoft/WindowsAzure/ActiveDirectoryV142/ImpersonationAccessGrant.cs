using System;
using System.CodeDom.Compiler;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("objectId")]
	public class ImpersonationAccessGrant
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static ImpersonationAccessGrant CreateImpersonationAccessGrant(string objectId)
		{
			return new ImpersonationAccessGrant
			{
				objectId = objectId
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string clientId
		{
			get
			{
				return this._clientId;
			}
			set
			{
				this._clientId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string consentType
		{
			get
			{
				return this._consentType;
			}
			set
			{
				this._consentType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? expiryTime
		{
			get
			{
				return this._expiryTime;
			}
			set
			{
				this._expiryTime = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string objectId
		{
			get
			{
				return this._objectId;
			}
			set
			{
				this._objectId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string principalId
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
		public string resourceId
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
		public string scope
		{
			get
			{
				return this._scope;
			}
			set
			{
				this._scope = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? startTime
		{
			get
			{
				return this._startTime;
			}
			set
			{
				this._startTime = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _clientId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _consentType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _expiryTime;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _objectId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _principalId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _resourceId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _scope;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _startTime;
	}
}
