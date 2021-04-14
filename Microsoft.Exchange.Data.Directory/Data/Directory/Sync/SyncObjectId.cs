using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	public class SyncObjectId : ObjectId, IEquatable<SyncObjectId>
	{
		public SyncObjectId(string contextId, string objectId, DirectoryObjectClass objectClass)
		{
			if (contextId == null)
			{
				throw new ArgumentNullException("contextId");
			}
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (objectClass == DirectoryObjectClass.Company && !contextId.Equals(objectId, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(DirectoryStrings.InvalidSyncCompanyId(string.Format("{0}_Company_{1}", contextId, objectId)), "objectId");
			}
			this.ContextId = contextId;
			this.ObjectId = objectId;
			this.ObjectClass = objectClass;
		}

		public string ContextId { get; private set; }

		public string ObjectId { get; private set; }

		public DirectoryObjectClass ObjectClass { get; private set; }

		public static bool operator ==(SyncObjectId left, SyncObjectId right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return true;
			}
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		public static bool operator !=(SyncObjectId left, SyncObjectId right)
		{
			return !(left == right);
		}

		public static SyncObjectId Parse(string identity)
		{
			string[] identityElements = SyncObjectId.GetIdentityElements(identity);
			if (identityElements.Length != 3)
			{
				throw new ArgumentException(DirectoryStrings.InvalidSyncObjectId(identity), "Identity");
			}
			string contextId = identityElements[0];
			DirectoryObjectClass objectClass = (DirectoryObjectClass)Enum.Parse(typeof(DirectoryObjectClass), identityElements[1]);
			string objectId = identityElements[2];
			return new SyncObjectId(contextId, objectId, objectClass);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SyncObjectId);
		}

		public bool Equals(SyncObjectId syncObjectId)
		{
			return !(syncObjectId == null) && (this.ObjectClass == syncObjectId.ObjectClass && StringComparer.OrdinalIgnoreCase.Equals(this.ObjectId, syncObjectId.ObjectId)) && StringComparer.OrdinalIgnoreCase.Equals(this.ContextId, syncObjectId.ContextId);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", new object[]
			{
				this.ContextId,
				this.ObjectClass,
				this.ObjectId
			});
		}

		public override int GetHashCode()
		{
			return this.ObjectClass.GetHashCode() ^ this.ObjectId.ToLowerInvariant().GetHashCode() ^ this.ContextId.ToLowerInvariant().GetHashCode();
		}

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this.ToString());
		}

		public DirectoryObjectIdentity ToMsoIdentity()
		{
			return new DirectoryObjectIdentity
			{
				ContextId = this.ContextId,
				ObjectId = this.ObjectId,
				ObjectClass = this.ObjectClass
			};
		}

		internal static string[] GetIdentityElements(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			string[] array = identity.Split(new char[]
			{
				'_'
			});
			if (array.Length > 3)
			{
				throw new ArgumentException(DirectoryStrings.InvalidSyncObjectId(identity), "Identity");
			}
			return array;
		}
	}
}
