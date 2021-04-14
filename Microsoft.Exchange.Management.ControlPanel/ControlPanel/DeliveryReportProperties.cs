using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DeliveryReportProperties : Properties
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string text = this.Context.Request.QueryString["recip"];
			if (base.ObjectIdentity == null)
			{
				string text2 = this.Context.Request.QueryString["MsgId"];
				string text3 = this.Context.Request.QueryString["Mbx"];
				if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3))
				{
					PowerShellResults<MessageTrackingSearchResultRow> messageTrackingReportSearchResults = this.GetMessageTrackingReportSearchResults(text2, text3);
					if (messageTrackingReportSearchResults.Succeeded && messageTrackingReportSearchResults.Output.Length > 0)
					{
						base.ObjectIdentity = new Identity(new RecipientMessageTrackingReportId(messageTrackingReportSearchResults.Output[0].Identity.RawIdentity, text).RawIdentity, text);
						return;
					}
					base.Results = messageTrackingReportSearchResults;
					base.UseSetObject = false;
				}
			}
		}

		private PowerShellResults<MessageTrackingSearchResultRow> GetMessageTrackingReportSearchResults(string msgId, string mailboxId)
		{
			MessageTrackingSearch messageTrackingSearch = new MessageTrackingSearch();
			return messageTrackingSearch.GetList(new MessageTrackingSearchFilter
			{
				MessageEntryId = msgId,
				Identity = Identity.FromIdParameter(mailboxId)
			}, null);
		}
	}
}
