using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class SearchObjectId : MailboxStoreIdentity, IEquatable<SearchObjectId>
	{
		public SearchObjectId(ADObjectId mailboxOwnerId, ObjectType objectType, Guid guid) : base(mailboxOwnerId)
		{
			this.objectType = objectType;
			this.guid = guid;
		}

		public SearchObjectId(SearchObjectId identity, ObjectType objectType) : this(identity.MailboxOwnerId, objectType, identity.Guid)
		{
		}

		public SearchObjectId(ADObjectId mailboxOwnerId, ObjectType objectType) : this(mailboxOwnerId, objectType, Guid.Empty)
		{
		}

		public SearchObjectId(ADObjectId mailboxOwnerId) : this(mailboxOwnerId, ObjectType.SearchObject, Guid.Empty)
		{
		}

		public SearchObjectId() : this(null, ObjectType.SearchObject, Guid.Empty)
		{
		}

		private SearchObjectId(SerializationInfo info, StreamingContext context)
		{
			this.ObjectType = (ObjectType)info.GetValue("ObjectType", typeof(ObjectType));
			this.Guid = (Guid)info.GetValue("Guid", typeof(Guid));
		}

		public static bool TryParse(string input, out SearchObjectId instance)
		{
			instance = null;
			if (string.IsNullOrEmpty(input) || input.Length < 6)
			{
				return false;
			}
			string rawType = null;
			int num = input.IndexOf('\\');
			string g;
			if (num != -1)
			{
				rawType = input.Substring(0, num);
				g = input.Substring(num + 1);
			}
			else
			{
				g = input;
			}
			ObjectType objectType = ObjectType.SearchObject;
			if (!string.IsNullOrEmpty(rawType))
			{
				string text = Enum.GetNames(typeof(ObjectType)).SingleOrDefault((string x) => x.Equals(rawType, StringComparison.OrdinalIgnoreCase));
				if (text == null)
				{
					return false;
				}
				objectType = (ObjectType)Enum.Parse(typeof(ObjectType), text, true);
			}
			Guid guid;
			if (GuidHelper.TryParseGuid(g, out guid))
			{
				instance = new SearchObjectId(null, objectType, guid);
				return true;
			}
			return false;
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
			internal set
			{
				this.guid = value;
			}
		}

		public ObjectType ObjectType
		{
			get
			{
				return this.objectType;
			}
			internal set
			{
				this.objectType = value;
			}
		}

		public string ConfigurationName
		{
			get
			{
				return this.ObjectType.ToString() + "." + (this.Guid.Equals(Guid.Empty) ? string.Empty : this.Guid.ToString().Replace("-", string.Empty));
			}
		}

		public string DistinguishedName
		{
			get
			{
				return string.Concat(new string[]
				{
					base.MailboxOwnerId.DistinguishedName,
					"/",
					this.ObjectType.ToString(),
					"/",
					this.Guid.ToString()
				});
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.Equals(Guid.Empty);
			}
		}

		public override byte[] GetBytes()
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			return this.ObjectType.ToString() + "\\" + this.Guid.ToString();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SearchObjectId);
		}

		public override int GetHashCode()
		{
			return this.ObjectType.GetHashCode() ^ this.Guid.GetHashCode();
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ObjectType", this.ObjectType);
			info.AddValue("Guid", this.Guid);
		}

		public bool Equals(SearchObjectId other)
		{
			return this.ObjectType == other.ObjectType && this.Guid.Equals(other.Guid);
		}

		private ObjectType objectType;

		private Guid guid;

		private static class SerializationMemberNames
		{
			public const string ObjectType = "ObjectType";

			public const string Guid = "Guid";
		}
	}
}
