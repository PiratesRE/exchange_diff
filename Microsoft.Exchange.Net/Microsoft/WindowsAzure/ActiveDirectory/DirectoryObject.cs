using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	[DataServiceKey("objectId")]
	public class DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static DirectoryObject CreateDirectoryObject(string objectId)
		{
			return new DirectoryObject
			{
				objectId = objectId
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string objectType
		{
			get
			{
				return this._objectType;
			}
			set
			{
				this._objectType = value;
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
		public DateTime? softDeletionTimestamp
		{
			get
			{
				return this._softDeletionTimestamp;
			}
			set
			{
				this._softDeletionTimestamp = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DirectoryObject createdOnBehalfOf
		{
			get
			{
				return this._createdOnBehalfOf;
			}
			set
			{
				this._createdOnBehalfOf = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> createdObjects
		{
			get
			{
				return this._createdObjects;
			}
			set
			{
				if (value != null)
				{
					this._createdObjects = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DirectoryObject manager
		{
			get
			{
				return this._manager;
			}
			set
			{
				this._manager = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> directReports
		{
			get
			{
				return this._directReports;
			}
			set
			{
				if (value != null)
				{
					this._directReports = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> members
		{
			get
			{
				return this._members;
			}
			set
			{
				if (value != null)
				{
					this._members = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> memberOf
		{
			get
			{
				return this._memberOf;
			}
			set
			{
				if (value != null)
				{
					this._memberOf = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> owners
		{
			get
			{
				return this._owners;
			}
			set
			{
				if (value != null)
				{
					this._owners = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> ownedObjects
		{
			get
			{
				return this._ownedObjects;
			}
			set
			{
				if (value != null)
				{
					this._ownedObjects = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _objectType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _objectId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _softDeletionTimestamp;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DirectoryObject _createdOnBehalfOf;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _createdObjects = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DirectoryObject _manager;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _directReports = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _members = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _memberOf = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _owners = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _ownedObjects = new Collection<DirectoryObject>();
	}
}
