using System;
using System.Web.Configuration;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.HttpProxy
{
	public class OutlookCN : OwaPage
	{
		protected string IcpLink
		{
			get
			{
				if (OutlookCN.icpLink == null)
				{
					OutlookCN.icpLink = WebConfigurationManager.AppSettings["GallatinIcpLink"];
				}
				return OutlookCN.icpLink;
			}
		}

		private const string IcpLinkAppSetting = "GallatinIcpLink";

		private static string icpLink;
	}
}
