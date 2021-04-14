using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class DagNetworkObjectId : ObjectId
	{
		public string DagName
		{
			get
			{
				return this.m_dagName;
			}
			set
			{
				if (this.m_fullName != null)
				{
					this.m_fullName = null;
				}
				this.m_dagName = value;
			}
		}

		public string NetName
		{
			get
			{
				return this.m_netName;
			}
			set
			{
				if (this.m_fullName != null)
				{
					this.m_fullName = null;
				}
				this.m_netName = value;
			}
		}

		public string FullName
		{
			get
			{
				if (this.m_fullName == null)
				{
					this.m_fullName = DagNetworkObjectId.BuildCompositeName(this.DagName, this.NetName);
				}
				return this.m_fullName;
			}
		}

		public override byte[] GetBytes()
		{
			if (this.DagName == null)
			{
				return null;
			}
			return Encoding.UTF8.GetBytes(this.FullName);
		}

		public DagNetworkObjectId(string dagName, string netName)
		{
			this.m_dagName = dagName;
			this.m_netName = netName;
		}

		public DagNetworkObjectId(string compositeName)
		{
			string[] array = compositeName.Split(new char[]
			{
				'\\'
			});
			if (array.Length >= 1)
			{
				this.m_dagName = array[0];
				if (array.Length > 1)
				{
					this.m_netName = array[1];
				}
			}
		}

		public override bool Equals(object obj)
		{
			DagNetworkObjectId dagNetworkObjectId = obj as DagNetworkObjectId;
			return dagNetworkObjectId != null && this.NetName.Equals(dagNetworkObjectId.NetName) && this.DagName.Equals(dagNetworkObjectId.DagName);
		}

		public override int GetHashCode()
		{
			return this.NetName.GetHashCode() ^ this.DagName.GetHashCode();
		}

		private static string BuildCompositeName(string dagName, string netName)
		{
			ExAssert.RetailAssert(!string.IsNullOrEmpty(dagName), "dagName must be provided");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(dagName);
			if (!string.IsNullOrEmpty(netName))
			{
				stringBuilder.Append('\\');
				stringBuilder.Append(netName);
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return this.FullName;
		}

		internal const char ElementSeparatorChar = '\\';

		private string m_dagName;

		private string m_netName;

		private string m_fullName;
	}
}
