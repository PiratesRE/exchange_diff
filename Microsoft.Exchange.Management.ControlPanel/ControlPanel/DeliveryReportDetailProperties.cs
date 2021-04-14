using System;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DeliveryReportDetailProperties : Properties
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.deliveryReportDetailsPane == null)
			{
				this.FindDeliveryReportDetailsPane();
			}
			PowerShellResults<RecipientTrackingEventRow> powerShellResults = base.Results as PowerShellResults<RecipientTrackingEventRow>;
			if (powerShellResults != null && powerShellResults.SucceededWithValue)
			{
				this.RenderEventDetails(powerShellResults);
				return;
			}
			base.ContentContainer.Visible = false;
		}

		private void RenderEventDetails(PowerShellResults<RecipientTrackingEventRow> results)
		{
			RecipientTrackingEventRow recipientTrackingEventRow = results.Output[0];
			TrackingEventType trackingEventType = TrackingEventType.None;
			foreach (TrackingEventRow trackingEventRow in recipientTrackingEventRow.Events)
			{
				if (trackingEventType != TrackingEventType.None)
				{
					if (trackingEventType != trackingEventRow.TrackingEvent)
					{
						HtmlGenericControl child = new HtmlGenericControl("p");
						this.deliveryReportDetailsPane.Controls.Add(child);
					}
					else
					{
						this.AddHtmlBreaks(2);
					}
				}
				if (trackingEventType != trackingEventRow.TrackingEvent)
				{
					EncodingLabel encodingLabel = new EncodingLabel();
					encodingLabel.Text = trackingEventRow.EventTypeDescription;
					this.deliveryReportDetailsPane.Controls.Add(encodingLabel);
					this.AddHtmlBreaks(1);
				}
				if (!string.IsNullOrEmpty(trackingEventRow.EventDateTime) || !string.IsNullOrEmpty(trackingEventRow.Server))
				{
					StringBuilder stringBuilder = new StringBuilder();
					EncodingLabel encodingLabel2 = new EncodingLabel();
					if (!string.IsNullOrEmpty(trackingEventRow.EventDateTime))
					{
						stringBuilder.Append(trackingEventRow.EventDateTime);
					}
					if (!string.IsNullOrEmpty(trackingEventRow.Server))
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.AppendFormat(" {0}", trackingEventRow.Server);
						}
						else
						{
							stringBuilder.Append(trackingEventRow.Server);
						}
					}
					encodingLabel2.Text = stringBuilder.ToString();
					this.deliveryReportDetailsPane.Controls.Add(encodingLabel2);
					this.AddHtmlBreaks(1);
				}
				EncodingLabel encodingLabel3 = new EncodingLabel();
				encodingLabel3.Text = trackingEventRow.EventDescription;
				this.deliveryReportDetailsPane.Controls.Add(encodingLabel3);
				if (trackingEventRow.TrackingEvent == TrackingEventType.Fail)
				{
					encodingLabel3.CssClass = "ErrorText";
					if (trackingEventRow.EventData != null && trackingEventRow.EventData.Length > 0)
					{
						this.AddHtmlBreaks(1);
						HtmlGenericControl htmlGenericControl = new HtmlGenericControl("p");
						htmlGenericControl.Attributes.Add("class", "failedDeliveryDetailText");
						EncodingLabel encodingLabel4 = new EncodingLabel();
						encodingLabel4.Text = trackingEventRow.EventData.StringArrayJoin("\n");
						htmlGenericControl.Controls.Add(encodingLabel4);
						this.deliveryReportDetailsPane.Controls.Add(htmlGenericControl);
					}
				}
				if (trackingEventRow.TrackingEvent == TrackingEventType.Expand && trackingEventRow.EventData.Length > 0)
				{
					this.AddEditGroupLink(trackingEventRow.EventData[0]);
				}
				trackingEventType = trackingEventRow.TrackingEvent;
			}
		}

		private void AddHtmlBreaks(int breaks)
		{
			for (int i = 0; i < breaks; i++)
			{
				Literal literal = new Literal();
				literal.Text = "<br />";
				this.deliveryReportDetailsPane.Controls.Add(literal);
			}
		}

		private void FindDeliveryReportDetailsPane()
		{
			this.deliveryReportDetailsPane = (HtmlGenericControl)base.ContentContainer.FindControl("deliveryReportDetailsPane");
		}

		private void AddEditGroupLink(string groupId)
		{
			if (RbacPrincipal.Current.IsInRole("Get-DistributionGroup@R:MyDistributionGroups+Get-Group@R:MyDistributionGroups") && RbacPrincipal.Current.IsInRole("Get-Recipient?Identity&Filter@R:MyDistributionGroups"))
			{
				RecipientTrackingDetails recipientTrackingDetails = new RecipientTrackingDetails();
				RecipientRow ownedDistributionGroup = recipientTrackingDetails.GetOwnedDistributionGroup(groupId);
				if (ownedDistributionGroup != null)
				{
					Literal literal = new Literal();
					literal.Text = "<br />";
					this.deliveryReportDetailsPane.Controls.Add(literal);
					string arg = "BuildCenteredWindowFeatureString(600, 540, GlobalVariables.FeaturesForPopups)";
					HtmlAnchor htmlAnchor = new HtmlAnchor();
					htmlAnchor.HRef = "~/MyGroups/EditDistributionGroup.aspx?id=" + ownedDistributionGroup.Identity.RawIdentity;
					htmlAnchor.Target = "editGroups";
					htmlAnchor.Attributes.Add("onclick", string.Format("PopupWindowManager.OpenWindow('{0}', '{1}', {2});return false;", base.ResolveClientUrl(htmlAnchor.HRef), "editGroups", arg));
					htmlAnchor.Title = OwaOptionStrings.EditGroups;
					htmlAnchor.InnerText = OwaOptionStrings.EditGroups;
					this.deliveryReportDetailsPane.Controls.Add(htmlAnchor);
				}
			}
		}

		private const string EditGroupsRole = "Get-DistributionGroup@R:MyDistributionGroups+Get-Group@R:MyDistributionGroups";

		protected HtmlGenericControl deliveryReportDetailsPane;
	}
}
