using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class MessageDetailsDialog : OwaPage
	{
		protected override void OnLoad(EventArgs e)
		{
			this.message = Utilities.GetItemForRequest<MessageItem>(base.OwaContext, out this.parentItem, true, new PropertyDefinition[]
			{
				MessageItemSchema.TransportMessageHeaders
			});
			base.OnLoad(e);
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.parentItem != null)
			{
				this.parentItem.Dispose();
				this.parentItem = null;
			}
			if (this.message != null)
			{
				this.message.Dispose();
				this.message = null;
			}
		}

		protected void RenderImportance()
		{
			if (this.message == null)
			{
				return;
			}
			switch (this.message.Importance)
			{
			case Importance.Low:
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(1502599728));
				return;
			case Importance.Normal:
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(1690472495));
				return;
			case Importance.High:
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-77932258));
				return;
			default:
				return;
			}
		}

		protected void RenderSensitivity()
		{
			if (this.message == null)
			{
				return;
			}
			switch (this.message.Sensitivity)
			{
			case Sensitivity.Normal:
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(1690472495));
				return;
			case Sensitivity.Personal:
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(567923294));
				return;
			case Sensitivity.Private:
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1268489823));
				return;
			case Sensitivity.CompanyConfidential:
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-819101664));
				return;
			default:
				return;
			}
		}

		protected void RenderTransportMessageHeaders()
		{
			if (this.message == null)
			{
				return;
			}
			try
			{
				using (Stream stream = this.message.OpenPropertyStream(MessageItemSchema.TransportMessageHeaders, PropertyOpenMode.ReadOnly))
				{
					byte[] array = new byte[1024];
					int count;
					while ((count = stream.Read(array, 0, 1024)) > 0)
					{
						Utilities.HtmlEncode(Encoding.Unicode.GetString(array, 0, count), base.Response.Output);
					}
				}
			}
			catch (StoragePermanentException)
			{
			}
		}

		private MessageItem message;

		private Item parentItem;
	}
}
