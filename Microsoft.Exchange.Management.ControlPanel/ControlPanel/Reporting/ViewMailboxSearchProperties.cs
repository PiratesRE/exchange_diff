using System;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Security.Application;

namespace Microsoft.Exchange.Management.ControlPanel.Reporting
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Reporting.js")]
	public class ViewMailboxSearchProperties : Properties
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			MultiLineLabel multiLineLabel = (MultiLineLabel)base.ContentContainer.FindControl("lblKeywordStatisticsInformation");
			Repeater repeater = (Repeater)base.ContentContainer.FindControl("keywordHitsRepeater");
			HtmlTableCell htmlTableCell = (HtmlTableCell)base.ContentContainer.FindControl("errorDetailsLink");
			PowerShellResults<MailboxSearch> powerShellResults = base.Results as PowerShellResults<MailboxSearch>;
			if (powerShellResults != null && powerShellResults.SucceededWithValue)
			{
				MailboxSearch mailboxSearch = powerShellResults.Output[0];
				repeater.DataSource = mailboxSearch.KeywordHits;
				repeater.DataBind();
				if (string.IsNullOrEmpty(mailboxSearch.Icon))
				{
					WebControl webControl = (WebControl)base.ContentContainer.FindControl("imgStatus");
					webControl.Visible = false;
				}
				Label label = (Label)base.ContentContainer.FindControl("lblInformation_label");
				label.Visible = false;
				MultiLineLabel multiLineLabel2 = (MultiLineLabel)base.ContentContainer.FindControl("lblInformation");
				multiLineLabel2.Visible = false;
				this.HideOrShowKeywordStatisticsPagingUI(false);
				string leftPart = this.Context.GetRequestUrl().GetLeftPart(UriPartial.Authority);
				string arg = Encoder.UrlEncode(Encoder.HtmlEncode(mailboxSearch.Identity.RawIdentity));
				HtmlAnchor htmlAnchor = new HtmlAnchor();
				htmlAnchor.InnerHtml = Strings.ViewAllDetails;
				string arg2 = string.Format("{0}/ecp/Reporting/ViewMailboxSearchErrors.aspx?Id={1}", leftPart, arg);
				htmlAnchor.HRef = "#";
				htmlAnchor.Attributes.Add("onclick", string.Format("window.open(\"{0}\",\"popup\",\"width=800,height=600,sscrollbars=yes,resizable=no,toolbar=no,directories=no,location=center,menubar=no,status=yes\"); return false", arg2));
				if (htmlTableCell != null && mailboxSearch.TotalKnownErrors + mailboxSearch.TotalUndefinedErrors > 0)
				{
					htmlTableCell.Controls.Add(htmlAnchor);
				}
				htmlAnchor.Dispose();
				if (mailboxSearch.MailboxSearch.EstimateOnly)
				{
					if (mailboxSearch.MailboxSearch.Status == SearchState.EstimateInProgress || mailboxSearch.MailboxSearch.Status == SearchState.Queued)
					{
						repeater.Visible = false;
						if (mailboxSearch.MailboxSearch.IncludeKeywordStatistics && !mailboxSearch.MailboxSearch.KeywordStatisticsDisabled)
						{
							multiLineLabel.Text = Strings.RetrievingStatistics;
						}
						else
						{
							Label label2 = (Label)base.ContentContainer.FindControl("lblKeywordStatistics");
							label2.Text = string.Empty;
						}
					}
					else
					{
						LinkButton linkButton = (LinkButton)base.ContentContainer.FindControl("lnkStartFullStatsSearch");
						linkButton.Visible = false;
						if (mailboxSearch.MailboxSearch.KeywordStatisticsDisabled)
						{
							Label label3 = (Label)base.ContentContainer.FindControl("lblKeywordStatistics");
							label3.Text = string.Empty;
							repeater.Visible = false;
							label.Visible = true;
							multiLineLabel2.Visible = true;
						}
						else if (mailboxSearch.IsFullStatsSearchAllowed)
						{
							multiLineLabel.Visible = false;
							Label label4 = (Label)base.ContentContainer.FindControl("lblRetrievingStatistics");
							label4.Visible = true;
							linkButton.OnClientClick = string.Format("MailboxSearchUtil.StartFullStatsMailboxSearchHandler('{0}', '{1}', '{2}', '{3}')", new object[]
							{
								linkButton.ClientID,
								label4.ClientID,
								mailboxSearch.Identity.RawIdentity,
								HttpUtility.JavaScriptStringEncode(mailboxSearch.Identity.DisplayName)
							});
							repeater.Visible = false;
							linkButton.Visible = true;
						}
						else if (mailboxSearch.SearchQuery == null || mailboxSearch.SearchQuery == string.Empty)
						{
							multiLineLabel.Text = Strings.KeywordStatisticsEmptyQuery;
							repeater.Visible = false;
						}
						else
						{
							if (mailboxSearch.ExcludeDuplicateMessages)
							{
								multiLineLabel.Text = Strings.DuplicatesNotExcluded;
							}
							Label label5 = (Label)base.ContentContainer.FindControl("lblKeywordStatisticsPagingInfo");
							if (label5 != null)
							{
								int num = mailboxSearch.StatisticsStartIndex + Math.Min(24, mailboxSearch.TotalKeywords - mailboxSearch.StatisticsStartIndex);
								label5.Text = string.Format(Strings.KeywordStatisticsPagingInfo, mailboxSearch.StatisticsStartIndex, num, mailboxSearch.TotalKeywords);
								LinkButton linkButton2 = (LinkButton)base.ContentContainer.FindControl("lnkKeywordStatisticsNavigationPrevious");
								LinkButton linkButton3 = (LinkButton)base.ContentContainer.FindControl("lnkKeywordStatisticsNavigationNext");
								Label label6 = (Label)base.ContentContainer.FindControl("lblRetrievingStatisticsForPaging");
								label6.Visible = true;
								linkButton2.Enabled = (mailboxSearch.StatisticsStartIndex > 25);
								if (linkButton2.Enabled)
								{
									linkButton2.OnClientClick = string.Format("MailboxSearchUtil.KeywordStatisticsPaginationSearch('{0}', '{1}', '{2}', '{3}', '{4}', {5})", new object[]
									{
										linkButton2.ClientID,
										linkButton3.ClientID,
										label6.ClientID,
										mailboxSearch.Identity.RawIdentity,
										HttpUtility.JavaScriptStringEncode(mailboxSearch.Identity.DisplayName),
										mailboxSearch.StatisticsStartIndex - 25
									});
								}
								linkButton3.Enabled = (num < mailboxSearch.TotalKeywords);
								if (linkButton3.Enabled)
								{
									linkButton3.OnClientClick = string.Format("MailboxSearchUtil.KeywordStatisticsPaginationSearch('{0}', '{1}', '{2}', '{3}', '{4}', {5})", new object[]
									{
										linkButton2.ClientID,
										linkButton3.ClientID,
										label6.ClientID,
										mailboxSearch.Identity.RawIdentity,
										HttpUtility.JavaScriptStringEncode(mailboxSearch.Identity.DisplayName),
										1 + num
									});
								}
								this.HideOrShowKeywordStatisticsPagingUI(true);
							}
						}
					}
				}
				else
				{
					Label label7 = (Label)base.ContentContainer.FindControl("lblKeywordStatistics");
					label7.Text = string.Empty;
					repeater.Visible = false;
				}
				if (mailboxSearch.MailboxSearch.PreviewDisabled)
				{
					label.Visible = true;
					multiLineLabel2.Visible = true;
				}
				this.rowResultMailbox = (HtmlControl)base.ContentContainer.FindControl("rowResultMailbox");
				if (this.rowResultMailbox != null)
				{
					this.rowResultMailbox.Visible = !string.IsNullOrEmpty(mailboxSearch.ResultsLink);
				}
				if (string.IsNullOrEmpty(mailboxSearch.InPlaceHoldErrors))
				{
					base.ContentContainer.FindControl("InPlaceHoldErrorLabelContainer").Visible = false;
				}
				if (!RbacPrincipal.Current.IsInRole("LegalHold"))
				{
					base.ContentContainer.FindControl("InPlaceHoldSection").Visible = false;
				}
				if (!RbacPrincipal.Current.IsInRole("MailboxSearch"))
				{
					base.ContentContainer.FindControl("DiscoverySection").Visible = false;
				}
				if (string.IsNullOrEmpty(multiLineLabel.Text))
				{
					multiLineLabel.Visible = false;
				}
			}
		}

		private void HideOrShowKeywordStatisticsPagingUI(bool visible)
		{
			HtmlTableRow htmlTableRow = (HtmlTableRow)base.ContentContainer.FindControl("rowKeywordStatisticsNavigation");
			if (htmlTableRow != null)
			{
				htmlTableRow.Visible = visible;
			}
		}

		private const int MaxKeywordsOnSinglePage = 25;

		protected HtmlControl rowResultMailbox;

		protected HyperLink previewResultsLink;
	}
}
