using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class ConnectedDomain
	{
		public ConnectedDomain(string organizationName, string adminGroupName, Guid routingGroupGuid, AddressSpace addressSpace)
		{
			this.organizationName = organizationName;
			this.adminGroupName = adminGroupName;
			this.routingGroupGuid = routingGroupGuid;
			this.addressSpace = addressSpace;
		}

		public string PrintableName
		{
			get
			{
				return this.ToString();
			}
		}

		public string OrganizationName
		{
			get
			{
				return this.organizationName;
			}
		}

		public string AdminGroupName
		{
			get
			{
				return this.adminGroupName;
			}
		}

		public Guid RoutingGroupGuid
		{
			get
			{
				return this.routingGroupGuid;
			}
		}

		public AddressSpace AddressSpace
		{
			get
			{
				return this.addressSpace;
			}
		}

		public int Cost
		{
			get
			{
				return this.addressSpace.Cost;
			}
		}

		public static ConnectedDomain Parse(string s)
		{
			ConnectedDomain result;
			if (!ConnectedDomain.TryParse(s, out result))
			{
				throw new FormatException(DataStrings.InvalidConnectedDomainFormat(s));
			}
			return result;
		}

		public static bool TryParse(string s, out ConnectedDomain connectedDomain)
		{
			connectedDomain = null;
			int num = s.IndexOf(ConnectedDomain.Separator);
			if (-1 == num)
			{
				return false;
			}
			string text = s.Substring(0, num);
			int num2 = s.IndexOf(ConnectedDomain.Separator, num + 1);
			if (-1 == num2)
			{
				return false;
			}
			string text2 = s.Substring(num + 1, num2 - num - 1);
			int num3 = s.IndexOf(ConnectedDomain.Separator, num2 + 1);
			if (-1 == num3 || num3 - num2 < 4)
			{
				return false;
			}
			string g = s.Substring(num2 + 1, num3 - num2 - 1);
			Guid guid;
			if (!GuidHelper.TryParseGuid(g, out guid))
			{
				return false;
			}
			int num4 = 1;
			int num5 = s.IndexOf(ConnectedDomain.Separator, num3 + 1);
			if (-1 != num5)
			{
				string s2 = s.Substring(num3 + 1, num5 - num3 - 1);
				if (!int.TryParse(s2, out num4) || num4 < 1 || num4 > 100)
				{
					return false;
				}
			}
			else
			{
				num5 = num3;
			}
			string address = s.Substring(num5 + 1, s.Length - num5 - 1);
			AddressSpace addressSpace;
			if (!AddressSpace.TryParse(address, out addressSpace, false))
			{
				return false;
			}
			addressSpace.Cost = num4;
			connectedDomain = new ConnectedDomain(text, text2, guid, addressSpace);
			return true;
		}

		public bool Equals(ConnectedDomain value)
		{
			return object.ReferenceEquals(this, value) || (this.organizationName.Equals(value.organizationName, StringComparison.OrdinalIgnoreCase) && this.adminGroupName.Equals(value.adminGroupName, StringComparison.OrdinalIgnoreCase) && this.routingGroupGuid.Equals(value.routingGroupGuid) && this.addressSpace.Equals(value.addressSpace));
		}

		public override bool Equals(object comparand)
		{
			ConnectedDomain connectedDomain = comparand as ConnectedDomain;
			return connectedDomain != null && this.Equals(connectedDomain);
		}

		public override int GetHashCode()
		{
			return this.organizationName.GetHashCode() ^ this.adminGroupName.GetHashCode() ^ this.routingGroupGuid.GetHashCode() ^ this.addressSpace.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}{1}{2}{1}{{{3}}}{1}{4}{1}{5}:{6}", new object[]
			{
				this.organizationName,
				ConnectedDomain.Separator,
				this.adminGroupName,
				this.routingGroupGuid,
				this.addressSpace.Cost,
				this.addressSpace.Type,
				this.addressSpace.Address
			});
		}

		private static readonly char Separator = Convert.ToChar(167);

		private string organizationName;

		private string adminGroupName;

		private Guid routingGroupGuid;

		private AddressSpace addressSpace;
	}
}
