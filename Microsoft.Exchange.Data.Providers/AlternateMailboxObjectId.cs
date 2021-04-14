using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Providers
{
	[Serializable]
	internal class AlternateMailboxObjectId : ObjectId
	{
		internal Guid? UserId
		{
			get
			{
				return this.m_userId;
			}
			set
			{
				this.m_userId = value;
			}
		}

		public string UserName
		{
			get
			{
				return this.m_userName;
			}
			set
			{
				if (this.m_fullName != null)
				{
					this.m_fullName = null;
				}
				this.m_userName = value;
			}
		}

		internal Guid? AmId
		{
			get
			{
				return this.m_amId;
			}
			set
			{
				this.m_amId = value;
			}
		}

		public string AmName
		{
			get
			{
				return this.m_amName;
			}
			set
			{
				if (this.m_fullName != null)
				{
					this.m_fullName = null;
				}
				this.m_amName = value;
			}
		}

		public string FullName
		{
			get
			{
				if (this.m_fullName == null)
				{
					this.m_fullName = AlternateMailboxObjectId.BuildCompositeName(this.UserName, this.AmName);
				}
				return this.m_fullName;
			}
		}

		public override byte[] GetBytes()
		{
			if (this.UserName == null)
			{
				return null;
			}
			return Encoding.UTF8.GetBytes(this.FullName);
		}

		internal AlternateMailboxObjectId(string userName, string amName, Guid userId, Guid? amId)
		{
			this.m_userName = userName;
			this.m_amName = amName;
			this.m_userId = new Guid?(userId);
			this.m_amId = amId;
		}

		public AlternateMailboxObjectId(string userName, string amName)
		{
			this.m_userName = userName;
			this.m_amName = amName;
		}

		public AlternateMailboxObjectId(string compositeName)
		{
			int num = compositeName.LastIndexOf('\\');
			if (num == -1)
			{
				this.m_userName = compositeName;
				return;
			}
			if (num != compositeName.Length - 1)
			{
				this.m_amName = compositeName.Substring(num + 1);
			}
			if (num != 0)
			{
				this.m_userName = compositeName.Substring(0, num);
			}
		}

		private static string BuildCompositeName(string userName, string amName)
		{
			ExAssert.RetailAssert(!string.IsNullOrEmpty(userName), "userName must be provided");
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append(userName);
			if (!string.IsNullOrEmpty(amName))
			{
				stringBuilder.Append('\\');
				stringBuilder.Append(amName);
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return this.FullName;
		}

		internal const char ElementSeparatorChar = '\\';

		private Guid? m_userId;

		private string m_userName;

		private Guid? m_amId;

		private string m_amName;

		private string m_fullName;
	}
}
