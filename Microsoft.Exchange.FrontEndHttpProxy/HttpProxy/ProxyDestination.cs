using System;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class ProxyDestination
	{
		internal ProxyDestination(int version, int portToUse, string[] allDestinations, string[] destinationsInService)
		{
			if (allDestinations == null)
			{
				throw new ArgumentNullException("allDestinations can't be null!");
			}
			if (destinationsInService == null)
			{
				throw new ArgumentNullException("destinationsInServices can't be null!");
			}
			if (allDestinations.Length == 0)
			{
				throw new ArgumentException("allDestinations must have at least one server!");
			}
			this.version = version;
			this.port = portToUse;
			this.serverFqdnList = allDestinations;
			this.inServiceServerFqdnList = destinationsInService;
			this.isFixedDestination = false;
		}

		internal ProxyDestination(int version, int portToUse, string fqdn)
		{
			this.version = version;
			this.port = portToUse;
			this.serverFqdnList = new string[]
			{
				fqdn
			};
			this.inServiceServerFqdnList = null;
			this.isFixedDestination = true;
		}

		internal int Port
		{
			get
			{
				return this.port;
			}
		}

		internal int Version
		{
			get
			{
				return this.version;
			}
		}

		internal bool IsDynamicTarget
		{
			get
			{
				return !this.isFixedDestination && this.version < Server.E15MinVersion && this.version >= Server.E2007MinVersion;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Is fixed = {0}; Port = {1}; Servers = (", this.isFixedDestination, this.Port);
			for (int i = 0; i < this.serverFqdnList.Length - 1; i++)
			{
				stringBuilder.AppendFormat("{0},", this.serverFqdnList[i]);
			}
			if (this.serverFqdnList.Length > 0)
			{
				stringBuilder.AppendFormat("{0}); Servers in service = (", this.serverFqdnList[this.serverFqdnList.Length - 1]);
			}
			else
			{
				stringBuilder.Append("); Servers in service = (");
			}
			if (this.inServiceServerFqdnList == null || this.inServiceServerFqdnList.Length == 0)
			{
				stringBuilder.Append(")");
			}
			else if (this.inServiceServerFqdnList != null)
			{
				for (int j = 0; j < this.inServiceServerFqdnList.Length - 1; j++)
				{
					stringBuilder.AppendFormat("{0},", this.inServiceServerFqdnList[j]);
				}
				stringBuilder.AppendFormat("{0})", this.inServiceServerFqdnList[this.inServiceServerFqdnList.Length - 1]);
			}
			else
			{
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}

		internal string GetHostName(int key)
		{
			if (this.isFixedDestination)
			{
				return this.serverFqdnList[0];
			}
			string text = null;
			checked
			{
				if (this.serverFqdnList.Length > 0)
				{
					text = this.serverFqdnList[(int)((IntPtr)(unchecked((ulong)key % (ulong)((long)this.serverFqdnList.Length))))];
					if (!this.inServiceServerFqdnList.Contains(text))
					{
						text = null;
						if (this.inServiceServerFqdnList.Length > 0)
						{
							text = this.inServiceServerFqdnList[(int)((IntPtr)(unchecked((ulong)key % (ulong)((long)this.inServiceServerFqdnList.Length))))];
						}
					}
				}
				return text;
			}
		}

		private readonly bool isFixedDestination;

		private readonly int port;

		private readonly int version;

		private string[] serverFqdnList;

		private string[] inServiceServerFqdnList;
	}
}
