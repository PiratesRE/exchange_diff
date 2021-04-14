using System;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.Storage.IPFiltering;

namespace Microsoft.Exchange.Transport
{
	internal class IPPermissionImpl : IPPermission
	{
		public override void AddRestriction(IPAddress ipAddress, TimeSpan expiration, string comments)
		{
			if (expiration.TotalHours > 48.0)
			{
				throw new ArgumentException("Expiration of entry must be within 48 hours of creation time");
			}
			IPvxAddress pvxAddress = new IPvxAddress(ipAddress);
			int type = 548;
			IPFilterRange range = new IPFilterRange(-1, pvxAddress, pvxAddress, DateTime.UtcNow + expiration, type, comments);
			IPFilterLists.AddRestriction(range);
		}

		public override PermissionCheckResults Check(IPAddress ipAddress)
		{
			IPvxAddress address = new IPvxAddress(ipAddress);
			bool flag = IPFilterLists.AddressAllowList.Search(address) != null;
			if (flag)
			{
				return PermissionCheckResults.Allow;
			}
			IPFilterRange ipfilterRange = IPFilterLists.AddressDenyList.Search(address);
			if (!(ipfilterRange != null))
			{
				return PermissionCheckResults.None;
			}
			if (!ipfilterRange.AdminCreated)
			{
				return PermissionCheckResults.MachineDeny;
			}
			return PermissionCheckResults.AdministratorDeny;
		}
	}
}
