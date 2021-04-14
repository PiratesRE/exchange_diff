using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("objectId")]
	public class Policy : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static Policy CreatePolicy(string objectId, Collection<string> policyDetail)
		{
			Policy policy = new Policy();
			policy.objectId = objectId;
			if (policyDetail == null)
			{
				throw new ArgumentNullException("policyDetail");
			}
			policy.policyDetail = policyDetail;
			return policy;
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
		public int? policyType
		{
			get
			{
				return this._policyType;
			}
			set
			{
				this._policyType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> policyDetail
		{
			get
			{
				return this._policyDetail;
			}
			set
			{
				this._policyDetail = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? tenantDefaultPolicy
		{
			get
			{
				return this._tenantDefaultPolicy;
			}
			set
			{
				this._tenantDefaultPolicy = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> policyAppliedTo
		{
			get
			{
				return this._policyAppliedTo;
			}
			set
			{
				if (value != null)
				{
					this._policyAppliedTo = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _policyType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _policyDetail = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _tenantDefaultPolicy;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _policyAppliedTo = new Collection<DirectoryObject>();
	}
}
