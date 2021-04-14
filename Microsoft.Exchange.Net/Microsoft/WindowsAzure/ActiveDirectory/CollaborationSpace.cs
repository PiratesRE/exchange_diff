using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	[DataServiceKey("objectId")]
	public class CollaborationSpace : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static CollaborationSpace CreateCollaborationSpace(string objectId, Collection<string> allowAccessTo)
		{
			CollaborationSpace collaborationSpace = new CollaborationSpace();
			collaborationSpace.objectId = objectId;
			if (allowAccessTo == null)
			{
				throw new ArgumentNullException("allowAccessTo");
			}
			collaborationSpace.allowAccessTo = allowAccessTo;
			return collaborationSpace;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? accountEnabled
		{
			get
			{
				return this._accountEnabled;
			}
			set
			{
				this._accountEnabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> allowAccessTo
		{
			get
			{
				return this._allowAccessTo;
			}
			set
			{
				this._allowAccessTo = value;
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
		public string userPrincipalName
		{
			get
			{
				return this._userPrincipalName;
			}
			set
			{
				this._userPrincipalName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid? changeMarker
		{
			get
			{
				return this._changeMarker;
			}
			set
			{
				this._changeMarker = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? provisioningSince
		{
			get
			{
				return this._provisioningSince;
			}
			set
			{
				this._provisioningSince = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _accountEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _allowAccessTo = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _description;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _mail;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _mailNickname;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _userPrincipalName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _changeMarker;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _provisioningSince;
	}
}
