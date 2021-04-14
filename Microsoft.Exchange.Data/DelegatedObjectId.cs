using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class DelegatedObjectId : ObjectId, IEquatable<DelegatedObjectId>
	{
		public DelegatedObjectId() : this(string.Empty, string.Empty)
		{
		}

		public DelegatedObjectId(string userName) : this(userName, string.Empty)
		{
		}

		public DelegatedObjectId(string userName, string orgName)
		{
			this.orgName = orgName;
			this.userName = userName;
		}

		public string Organization
		{
			get
			{
				return this.orgName;
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		public override byte[] GetBytes()
		{
			return this.GetBytes(Encoding.Unicode);
		}

		public byte[] GetBytes(Encoding encoding)
		{
			int byteCount = encoding.GetByteCount(this.toStringValue);
			byte[] array = new byte[byteCount];
			encoding.GetBytes(this.toStringValue, 0, this.toStringValue.Length, array, 0);
			return array;
		}

		public override string ToString()
		{
			if (this.toStringValue == null)
			{
				if (!string.IsNullOrEmpty(this.orgName) && !string.IsNullOrEmpty(this.userName))
				{
					this.toStringValue = this.orgName + DelegatedObjectId.Separator + this.userName;
				}
				else if (!string.IsNullOrEmpty(this.userName))
				{
					this.toStringValue = this.userName;
				}
				else
				{
					this.toStringValue = string.Empty;
				}
			}
			return this.toStringValue;
		}

		public override bool Equals(object obj)
		{
			return obj != null && this.Equals(obj as DelegatedObjectId);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public bool Equals(DelegatedObjectId other)
		{
			return other != null && this.ToString().Equals(other.ToString());
		}

		private string orgName;

		private string userName;

		private volatile string toStringValue;

		private static string Separator = "\\";
	}
}
