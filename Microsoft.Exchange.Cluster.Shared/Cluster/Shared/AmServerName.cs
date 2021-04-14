using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class AmServerName : IEquatable<AmServerName>, IComparable<AmServerName>
	{
		public static StringComparer Comparer
		{
			get
			{
				return StringComparer.OrdinalIgnoreCase;
			}
		}

		public static StringComparison Comparison
		{
			get
			{
				return StringComparison.OrdinalIgnoreCase;
			}
		}

		internal AmServerName()
		{
			this.Initialize(string.Empty, string.Empty, null);
		}

		internal AmServerName(string netbiosName, string domainSuffix)
		{
			this.Initialize(netbiosName, domainSuffix, null);
		}

		internal AmServerName(string serverName) : this(serverName, true)
		{
		}

		internal AmServerName(string serverName, bool throwOnFqdnError)
		{
			if (!string.IsNullOrEmpty(serverName))
			{
				int num = serverName.IndexOf(".");
				if (num == -1)
				{
					string fqdn = AmServerNameCache.Instance.GetFqdn(serverName, throwOnFqdnError);
					num = fqdn.IndexOf(".");
					SharedDiag.RetailAssert(num != -1, "fqdn resolution should have thrown", new object[0]);
					serverName = fqdn;
				}
				string netbiosName = serverName.Substring(0, num);
				int num2 = num + 1;
				string dnsSuffix = serverName.Substring(num2, serverName.Length - num2);
				this.Initialize(netbiosName, dnsSuffix, serverName);
				return;
			}
			this.Initialize(string.Empty, string.Empty, null);
		}

		internal AmServerName(ADObjectId serverId) : this(serverId.Name)
		{
		}

		internal AmServerName(Server server) : this(server.Fqdn)
		{
		}

		internal AmServerName(MiniServer miniServer) : this(miniServer.Fqdn)
		{
		}

		internal AmServerName(AmServerName other) : this(other.Fqdn)
		{
		}

		internal static AmServerName Empty
		{
			get
			{
				return AmServerName.sm_emptyInstance;
			}
		}

		internal static void TestResetLocalComputerName()
		{
			AmServerName.sm_localComputerName = null;
		}

		internal static AmServerName LocalComputerName
		{
			get
			{
				if (AmServerName.sm_localComputerName == null)
				{
					try
					{
						AmServerName.sm_localComputerName = new AmServerName(SharedDependencies.ManagementClassHelper.LocalComputerFqdn);
					}
					catch (CannotGetComputerNameException ex)
					{
						throw new AmGetFqdnFailedADErrorException("AmServerName.LocalComputerName", ex.Message, ex);
					}
				}
				return AmServerName.sm_localComputerName;
			}
		}

		internal bool IsEmpty { get; private set; }

		internal string NetbiosName { get; private set; }

		internal string DnsSuffix { get; private set; }

		internal string Fqdn
		{
			get
			{
				return this.m_fqdn;
			}
			private set
			{
				this.m_fqdn = value;
			}
		}

		internal bool IsLocalComputerName
		{
			get
			{
				return AmServerName.IsEqual(this, AmServerName.LocalComputerName);
			}
		}

		public static string GetSimpleName(string nodeName)
		{
			if (string.IsNullOrEmpty(nodeName))
			{
				return MachineName.Local;
			}
			if (nodeName.Contains("."))
			{
				return nodeName.Substring(0, nodeName.IndexOf('.'));
			}
			return nodeName;
		}

		public bool Equals(AmServerName dst)
		{
			return AmServerName.IsEqual(this, dst);
		}

		public int CompareTo(AmServerName other)
		{
			if (other == null)
			{
				return 1;
			}
			return string.Compare(this.Fqdn, other.Fqdn, AmServerName.Comparison);
		}

		public override bool Equals(object obj)
		{
			AmServerName amServerName = obj as AmServerName;
			return amServerName != null && AmServerName.IsEqual(this, amServerName);
		}

		public override string ToString()
		{
			return this.Fqdn;
		}

		public override int GetHashCode()
		{
			return SharedHelper.GetStringIHashCode(this.Fqdn);
		}

		internal static bool IsEqual(AmServerName src, AmServerName dst)
		{
			return object.ReferenceEquals(src, dst) || (src != null && dst != null && SharedHelper.StringIEquals(src.Fqdn, dst.Fqdn));
		}

		internal static bool IsNullOrEmpty(AmServerName serverName)
		{
			return serverName == null || serverName.IsEmpty;
		}

		internal static bool IsArrayEquals(AmServerName[] left, AmServerName[] right)
		{
			if (left == right)
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			if (left.Length != right.Length)
			{
				return false;
			}
			for (int i = 0; i < left.Length; i++)
			{
				if (!AmServerName.IsEqual(left[i], right[i]))
				{
					return false;
				}
			}
			return true;
		}

		private void Initialize(string netbiosName, string dnsSuffix, string fqdn)
		{
			this.NetbiosName = netbiosName.ToLowerInvariant();
			this.DnsSuffix = dnsSuffix.ToLowerInvariant();
			if (string.IsNullOrEmpty(this.NetbiosName) && string.IsNullOrEmpty(this.DnsSuffix))
			{
				this.Fqdn = string.Empty;
				this.IsEmpty = true;
				return;
			}
			if (fqdn == null)
			{
				this.Fqdn = string.Format("{0}.{1}", this.NetbiosName, this.DnsSuffix);
			}
			else
			{
				this.Fqdn = fqdn;
			}
			this.IsEmpty = false;
		}

		private static AmServerName sm_emptyInstance = new AmServerName();

		private static AmServerName sm_localComputerName;

		private string m_fqdn;
	}
}
