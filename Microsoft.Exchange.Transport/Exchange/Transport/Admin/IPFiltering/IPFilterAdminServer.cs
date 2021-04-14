using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.IPFilter;
using Microsoft.Exchange.Transport.Storage.IPFiltering;

namespace Microsoft.Exchange.Transport.Admin.IPFiltering
{
	internal sealed class IPFilterAdminServer : IPFilterRpcServer
	{
		public override int Add(IPFilterRange item)
		{
			int result;
			try
			{
				ulong high;
				ulong low;
				item.GetLowerBound(out high, out low);
				IPvxAddress start = new IPvxAddress(high, low);
				item.GetUpperBound(out high, out low);
				IPvxAddress end = new IPvxAddress(high, low);
				IPFilterRange ipfilterRange = new IPFilterRange(-1, start, end, item.ExpiresOn, item.Flags, item.Comment);
				bool flag;
				if (ipfilterRange.PolicyType == PolicyType.Allow)
				{
					flag = IPFilterLists.AddressAllowList.ContainsAdminIPRange(ipfilterRange);
				}
				else
				{
					flag = IPFilterLists.AddressDenyList.ContainsAdminIPRange(ipfilterRange);
				}
				if (flag)
				{
					result = -1;
				}
				else
				{
					result = IPFilterLists.AddRestriction(ipfilterRange);
				}
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
				result = -1;
			}
			return result;
		}

		public override int Remove(int identity, int filter)
		{
			int result;
			try
			{
				result = IPFilterLists.AdminRemove(identity, filter);
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
				result = -1;
			}
			return result;
		}

		public override IPFilterRange[] GetItems(int startIdentity, int flags, ulong highBits, ulong lowBits, int count)
		{
			IPFilterRange[] result;
			try
			{
				IPvxAddress address = new IPvxAddress(highBits, lowBits);
				List<IPFilterRow> list = IPFilterLists.AdminGetItems(startIdentity, flags, address, count);
				IPFilterRange[] array = new IPFilterRange[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					IPFilterRow ipfilterRow = list[i];
					IPFilterRange ipfilterRange = new IPFilterRange();
					ipfilterRange.Identity = ipfilterRow.Identity;
					ipfilterRange.Flags = ipfilterRow.TypeFlags;
					ipfilterRange.ExpiresOn = ipfilterRow.ExpiresOn;
					ipfilterRange.Comment = ipfilterRow.Comment;
					ipfilterRange.SetLowerBound((ulong)(ipfilterRow.LowerBound >> 64), (ulong)ipfilterRow.LowerBound);
					ipfilterRange.SetUpperBound((ulong)(ipfilterRow.UpperBound >> 64), (ulong)ipfilterRow.UpperBound);
					array[i] = ipfilterRange;
				}
				result = array;
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
				result = null;
			}
			return result;
		}
	}
}
