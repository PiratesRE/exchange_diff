using System;
using System.Collections;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class AddressBookDataSource : ListViewDataSource
	{
		public AddressBookDataSource(Hashtable properties, string searchString, AddressBookBase addressBook, AddressBookBase.RecipientCategory recipientCategory, UserContext userContext) : base(properties)
		{
			if (addressBook == null)
			{
				throw new ArgumentNullException("addresslist");
			}
			this.addressBook = addressBook;
			this.searchString = searchString;
			this.recipientCategory = recipientCategory;
			this.userContext = userContext;
		}

		public override void LoadData(int startRange, int endRange)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ADDataSource.LoadData(Start)");
			int lcid = Culture.GetUserCulture().LCID;
			string preferredDC = this.userContext.PreferredDC;
			ExTraceGlobals.MailCallTracer.TraceDebug<string>((long)this.GetHashCode(), "AddressBookDataSource.LoadData: preferred DC in user context = '{0}'", preferredDC);
			if (startRange < 1)
			{
				throw new ArgumentOutOfRangeException("startRange", "startRange must be greater than 0");
			}
			if (endRange < startRange)
			{
				throw new ArgumentOutOfRangeException("endRange", "endRange must be greater than or equal to startRange");
			}
			PropertyDefinition[] properties = base.CreateProperyTable();
			int num = endRange - startRange + 1;
			int pagesToSkip = startRange / num;
			string cookie = null;
			if (DirectoryAssistance.IsEmptyAddressList(this.userContext))
			{
				base.Items = new object[0][];
			}
			else if (!string.IsNullOrEmpty(this.searchString))
			{
				base.Items = AddressBookBase.PagedSearch(DirectoryAssistance.IsVirtualAddressList(this.userContext) ? this.userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN : null, DirectoryAssistance.IsVirtualAddressList(this.userContext) ? null : this.addressBook, this.userContext.ExchangePrincipal.MailboxInfo.OrganizationId, this.recipientCategory, this.searchString, ref cookie, pagesToSkip, num, out this.itemsTouched, ref lcid, ref preferredDC, properties);
			}
			else
			{
				ExTraceGlobals.MailCallTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "AddressBookDataSource.LoadData: browse: OrganizationId of address book = '{0}'", this.addressBook.OrganizationId);
				int num2;
				base.Items = this.addressBook.BrowseTo(ref cookie, this.userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN, ref lcid, ref preferredDC, startRange, num + 1, out num2, DirectoryAssistance.IsVirtualAddressList(this.userContext), properties);
				if (base.Items != null && base.Items.Length < num + 1)
				{
					cookie = null;
				}
			}
			this.userContext.PreferredDC = preferredDC;
			ExTraceGlobals.MailCallTracer.TraceDebug<string>((long)this.GetHashCode(), "AddressBookDataSource.LoadData: stamped preferred DC = '{0}' onto user context.", preferredDC);
			base.Cookie = cookie;
			base.StartRange = startRange;
			if (base.Items == null || base.Items.Length == 0)
			{
				base.EndRange = 0;
				return;
			}
			if (base.Items.Length < num)
			{
				base.EndRange = startRange + base.Items.Length - 1;
				return;
			}
			base.EndRange = endRange;
		}

		public override int TotalCount
		{
			get
			{
				return this.itemsTouched;
			}
		}

		private AddressBookBase.RecipientCategory recipientCategory;

		private AddressBookBase addressBook;

		private string searchString;

		private UserContext userContext;

		private int itemsTouched;
	}
}
