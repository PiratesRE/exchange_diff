using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DeliveryReportPage : BaseForm
	{
		protected override void OnLoad(EventArgs e)
		{
			this.wrapperPanel.PreRender += this.WrapperPanel_PreRender;
			string text = base.Request.QueryString["HelpId"];
			if (!string.IsNullOrEmpty(text))
			{
				base.HelpId = text;
			}
		}

		private void WrapperPanel_PreRender(object sender, EventArgs e)
		{
			if (this.deliveryReportProperties.ObjectIdentity != null)
			{
				this.SetupFilterBindings();
				string strA = base.Request.QueryString["isowa"];
				string text = this.GetDeliveryReportUrl();
				if (string.Compare(strA, "n", true) == 0 || !RbacPrincipal.Current.RbacConfiguration.ExecutingUserIsAllowedOWA)
				{
					NavigateCommand navigateCommand = this.deliveryReportDetailsPane.Commands[0] as NavigateCommand;
					navigateCommand.NavigateUrl = "mailto:?subject={0}&body={1}";
					text += "?isowa=n";
				}
				else
				{
					this.recipientSummary.IsOWA = true;
				}
				this.recipientSummary.DeliveryReportUrl = text;
				this.recipientSummary.DeliveryStatusDataSourceRefreshMethod = this.deliveryStatusDataSource.RefreshWebServiceMethod;
				return;
			}
			this.wrapperPanel.Visible = false;
		}

		private void SetupFilterBindings()
		{
			StaticBinding staticBinding = new StaticBinding();
			staticBinding.Name = "Identity";
			RecipientMessageTrackingReportId recipientMessageTrackingReportId = RecipientMessageTrackingReportId.Parse(this.deliveryReportProperties.ObjectIdentity);
			staticBinding.Value = recipientMessageTrackingReportId.MessageTrackingReportId;
			if (this.wrapperPanel == null)
			{
				this.wrapperPanel = (DockPanel)this.deliveryReportProperties.FindControl("wrapperPanel");
			}
			this.deliveryStatusDataSource = (WebServiceListSource)this.wrapperPanel.Controls[0].FindControl("deliveryStatusDataSource");
			this.deliveryStatusDataSource.FilterParameters.Add(staticBinding);
			ComponentBinding componentBinding = new ComponentBinding(this.recipientSummary, "Status");
			componentBinding.Name = "RecipientStatus";
			this.deliveryStatusDataSource.FilterParameters.Add(componentBinding);
			if (!string.IsNullOrEmpty(recipientMessageTrackingReportId.Recipient))
			{
				StaticBinding staticBinding2 = new StaticBinding();
				staticBinding2.Name = "Recipients";
				staticBinding2.Value = recipientMessageTrackingReportId.Recipient;
				this.deliveryStatusDataSource.FilterParameters.Add(staticBinding2);
				return;
			}
			ComponentBinding componentBinding2 = new ComponentBinding(this.toAddress, "value");
			componentBinding2.Name = "Recipients";
			this.deliveryStatusDataSource.FilterParameters.Add(componentBinding2);
		}

		private string GetDeliveryReportUrl()
		{
			string text = (string)base.Cache["DeliveryReportUrl"];
			if (text != null)
			{
				return text;
			}
			lock (this.cacheLock)
			{
				text = (string)base.Cache["DeliveryReportUrl"];
				if (text == null)
				{
					Uri ecpexternalUrl = this.GetECPExternalUrl();
					if (ecpexternalUrl != null)
					{
						text = new Uri(ecpexternalUrl, this.Context.GetRequestUrlAbsolutePath()).ToString();
					}
					else
					{
						text = EcpUrl.ResolveClientUrl(this.Context.GetRequestUrl().GetLeftPart(UriPartial.Path));
					}
					if (text != null)
					{
						base.Cache["DeliveryReportUrl"] = text;
					}
				}
			}
			return text;
		}

		private Uri GetECPExternalUrl()
		{
			Uri result = null;
			RbacPrincipal rbacPrincipal = RbacPrincipal.Current;
			if (rbacPrincipal.IsInRole("Mailbox"))
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromDirectoryObjectId((IRecipientSession)((RecipientObjectResolver)RecipientObjectResolver.Instance).CreateAdSession(), rbacPrincipal.ExecutingUserId, RemotingOptions.LocalConnectionsOnly);
				try
				{
					result = FrontEndLocator.GetFrontEndEcpUrl(exchangePrincipal);
				}
				catch (ServerNotFoundException)
				{
				}
			}
			return result;
		}

		private const string CacheKey = "DeliveryReportUrl";

		protected DeliveryReportProperties deliveryReportProperties;

		protected WebServiceListSource deliveryStatusDataSource;

		protected DockPanel wrapperPanel;

		protected DeliveryReportSummary recipientSummary;

		protected RecipientPickerControl toAddress;

		protected DetailsPane deliveryReportDetailsPane;

		private object cacheLock = new object();
	}
}
