using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	[DataServiceKey("objectId")]
	public class Group : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static Group CreateGroup(string objectId, Collection<string> exchangeResources, Collection<ProvisioningError> provisioningErrors, Collection<string> proxyAddresses, Collection<string> sharepointResources)
		{
			Group group = new Group();
			group.objectId = objectId;
			if (exchangeResources == null)
			{
				throw new ArgumentNullException("exchangeResources");
			}
			group.exchangeResources = exchangeResources;
			if (provisioningErrors == null)
			{
				throw new ArgumentNullException("provisioningErrors");
			}
			group.provisioningErrors = provisioningErrors;
			if (proxyAddresses == null)
			{
				throw new ArgumentNullException("proxyAddresses");
			}
			group.proxyAddresses = proxyAddresses;
			if (sharepointResources == null)
			{
				throw new ArgumentNullException("sharepointResources");
			}
			group.sharepointResources = sharepointResources;
			return group;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> exchangeResources
		{
			get
			{
				return this._exchangeResources;
			}
			set
			{
				this._exchangeResources = value;
			}
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
		public bool? dirSyncEnabled
		{
			get
			{
				return this._dirSyncEnabled;
			}
			set
			{
				this._dirSyncEnabled = value;
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
		public string groupType
		{
			get
			{
				return this._groupType;
			}
			set
			{
				this._groupType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? isPublic
		{
			get
			{
				return this._isPublic;
			}
			set
			{
				this._isPublic = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? lastDirSyncTime
		{
			get
			{
				return this._lastDirSyncTime;
			}
			set
			{
				this._lastDirSyncTime = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string mail
		{
			get
			{
				return this._mail;
			}
			set
			{
				this._mail = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string mailNickname
		{
			get
			{
				return this._mailNickname;
			}
			set
			{
				this._mailNickname = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? mailEnabled
		{
			get
			{
				return this._mailEnabled;
			}
			set
			{
				this._mailEnabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<ProvisioningError> provisioningErrors
		{
			get
			{
				return this._provisioningErrors;
			}
			set
			{
				this._provisioningErrors = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> proxyAddresses
		{
			get
			{
				return this._proxyAddresses;
			}
			set
			{
				this._proxyAddresses = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? securityEnabled
		{
			get
			{
				return this._securityEnabled;
			}
			set
			{
				this._securityEnabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> sharepointResources
		{
			get
			{
				return this._sharepointResources;
			}
			set
			{
				this._sharepointResources = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectAccessGrant> directAccessGrants
		{
			get
			{
				return this._directAccessGrants;
			}
			set
			{
				if (value != null)
				{
					this._directAccessGrants = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> pendingMembers
		{
			get
			{
				return this._pendingMembers;
			}
			set
			{
				if (value != null)
				{
					this._pendingMembers = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> allowAccessTo
		{
			get
			{
				return this._allowAccessTo;
			}
			set
			{
				if (value != null)
				{
					this._allowAccessTo = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> hasAccessTo
		{
			get
			{
				return this._hasAccessTo;
			}
			set
			{
				if (value != null)
				{
					this._hasAccessTo = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _exchangeResources = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _description;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _dirSyncEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _groupType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _isPublic;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _lastDirSyncTime;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _mail;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _mailNickname;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _mailEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ProvisioningError> _provisioningErrors = new Collection<ProvisioningError>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _proxyAddresses = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _securityEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _sharepointResources = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectAccessGrant> _directAccessGrants = new Collection<DirectAccessGrant>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _pendingMembers = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _allowAccessTo = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _hasAccessTo = new Collection<DirectoryObject>();
	}
}
