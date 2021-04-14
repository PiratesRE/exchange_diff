using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public class StoreIntegrityCheckJobIdentity : ObjectId, IEquatable<StoreIntegrityCheckJobIdentity>
	{
		public StoreIntegrityCheckJobIdentity()
		{
		}

		public StoreIntegrityCheckJobIdentity(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			string[] array = identity.Split(StoreIntegrityCheckJobIdentity.Separator, StringSplitOptions.None);
			if (array == null || array.Length < 2 || array.Length > 3)
			{
				throw new InvalidIntegrityCheckJobIdentity(identity);
			}
			if (!Guid.TryParse(array[0], out this.databaseGuid) || !Guid.TryParse(array[1], out this.requestGuid))
			{
				throw new InvalidIntegrityCheckJobIdentity(identity);
			}
			if (array.Length == 3 && !Guid.TryParse(array[2], out this.jobGuid))
			{
				throw new InvalidIntegrityCheckJobIdentity(identity);
			}
		}

		public StoreIntegrityCheckJobIdentity(byte[] identity) : this(Encoding.UTF8.GetString(identity))
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
		}

		public StoreIntegrityCheckJobIdentity(Guid databaseGuid, Guid requestGuid)
		{
			this.databaseGuid = databaseGuid;
			this.requestGuid = requestGuid;
		}

		public StoreIntegrityCheckJobIdentity(Guid databaseGuid, Guid requestGuid, Guid jobGuid)
		{
			this.databaseGuid = databaseGuid;
			this.requestGuid = requestGuid;
			this.jobGuid = jobGuid;
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public Guid RequestGuid
		{
			get
			{
				return this.requestGuid;
			}
		}

		public Guid JobGuid
		{
			get
			{
				return this.jobGuid;
			}
		}

		public override byte[] GetBytes()
		{
			return Encoding.UTF8.GetBytes(this.ToString());
		}

		public override string ToString()
		{
			if (!(this.databaseGuid != Guid.Empty) || !(this.requestGuid != Guid.Empty))
			{
				return string.Empty;
			}
			if (this.jobGuid != Guid.Empty)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", new object[]
				{
					this.databaseGuid,
					StoreIntegrityCheckJobIdentity.Separator[0],
					this.requestGuid,
					StoreIntegrityCheckJobIdentity.Separator[0],
					this.jobGuid
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
			{
				this.databaseGuid,
				StoreIntegrityCheckJobIdentity.Separator[0],
				this.requestGuid
			});
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as StoreIntegrityCheckJobIdentity);
		}

		public bool Equals(StoreIntegrityCheckJobIdentity other)
		{
			return object.ReferenceEquals(this, other) || (other != null && !(this.jobGuid != other.jobGuid) && !(this.requestGuid != other.requestGuid) && !(this.databaseGuid != other.databaseGuid));
		}

		public override int GetHashCode()
		{
			if (this.jobGuid != Guid.Empty)
			{
				return this.jobGuid.GetHashCode();
			}
			if (this.requestGuid != Guid.Empty)
			{
				return this.requestGuid.GetHashCode();
			}
			return 0;
		}

		private static char[] Separator = new char[]
		{
			'\\'
		};

		private readonly Guid databaseGuid;

		private readonly Guid requestGuid;

		private readonly Guid jobGuid;
	}
}
