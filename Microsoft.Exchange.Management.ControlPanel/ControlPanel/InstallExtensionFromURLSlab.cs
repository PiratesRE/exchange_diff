using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(ExtensionWizardProperties))]
	[RequiredScript(typeof(BaseForm))]
	[RequiredScript(typeof(ExtensionWizardForm))]
	[RequiredScript(typeof(UploadPackageStep))]
	[ClientScriptResource("InstallExtensionFromURLSlab", "Microsoft.Exchange.Management.ControlPanel.Client.Extension.js")]
	public class InstallExtensionFromURLSlab : SlabControl, IScriptControl
	{
		public InstallExtensionFromURLSlab()
		{
			Util.RequireUpdateProgressPopUp(this);
		}

		public string ServiceUrl { get; set; }

		public string InstallMarketplaceAssetID { get; set; }

		public string MarketplaceQueryMarket { get; set; }

		public string Scope { get; set; }

		public string Etoken { get; set; }

		public string DeploymentId { get; set; }

		public string MarketplaceServicesUrl
		{
			get
			{
				return ExtensionUtility.MarketplaceServicesUrl;
			}
		}

		public string MarketplacePageBaseUrl
		{
			get
			{
				return ExtensionUtility.MarketplaceLandingPageUrl;
			}
		}

		public string Clid
		{
			get
			{
				return ExtensionUtility.Clid;
			}
		}

		public string FullVersion
		{
			get
			{
				return ExtensionUtility.ClientFullVersion;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InstallMarketplaceAssetID = this.Context.Request.QueryString["AssetID"];
			this.MarketplaceQueryMarket = this.Context.Request.QueryString["LC"];
			this.Scope = this.Context.Request.QueryString["Scope"];
			this.DeploymentId = this.Context.Request.QueryString["DeployId"];
			this.Etoken = this.GetClientTokenParameter(this.Context.Request.RawUrl);
			if (!string.IsNullOrWhiteSpace(this.InstallMarketplaceAssetID) && !string.IsNullOrWhiteSpace(this.MarketplaceQueryMarket))
			{
				return;
			}
			EcpEventLogConstants.Tuple_MissingRequiredParameterDetected.LogPeriodicEvent(EcpEventLogExtensions.GetPeriodicKeyPerUser(), new object[]
			{
				EcpEventLogExtensions.GetUserNameToLog(),
				this.Context.GetRequestUrlForLog(),
				(this.InstallMarketplaceAssetID != null) ? this.InstallMarketplaceAssetID : string.Empty,
				(this.MarketplaceQueryMarket != null) ? this.MarketplaceQueryMarket : string.Empty
			});
			ErrorHandlingUtil.TransferToErrorPage("badofficecallback");
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<InstallExtensionFromURLSlab>(this);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.ID != null)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			}
			base.Render(writer);
			ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
		}

		public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			ClientScriptResourceAttribute clientScriptResourceAttribute = (ClientScriptResourceAttribute)TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor(clientScriptResourceAttribute.ComponentType, this.ClientID);
			this.BuildScriptDescriptor(scriptControlDescriptor);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		public IEnumerable<ScriptReference> GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(base.GetType());
		}

		private void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddProperty("ServiceUrl", this.ServiceUrl);
			descriptor.AddProperty("MarketplaceQueryMarket", this.MarketplaceQueryMarket);
			descriptor.AddProperty("InstallMarketplaceAssetID", this.InstallMarketplaceAssetID);
			descriptor.AddProperty("DeploymentID", this.DeploymentId);
			descriptor.AddProperty("MarketplaceServicesUrl", this.MarketplaceServicesUrl);
			descriptor.AddProperty("MarketplacePageBaseUrl", this.MarketplacePageBaseUrl);
			descriptor.AddProperty("MarketplaceClid", this.Clid);
			descriptor.AddProperty("FullVersion", this.FullVersion);
			descriptor.AddProperty("Scope", this.Scope);
			descriptor.AddProperty("InstallEtoken", this.Etoken);
		}

		private string GetClientTokenParameter(string url)
		{
			string result = string.Empty;
			int num = url.IndexOf("ClientToken=", StringComparison.OrdinalIgnoreCase);
			if (num > 0 && (url[num - 1] == '&' || url[num - 1] == '?'))
			{
				int num2 = num + "ClientToken=".Length;
				int num3 = url.IndexOf('&', num2);
				int length = (num3 > 0) ? (num3 - num2) : (url.Length - num2);
				result = url.Substring(num2, length);
			}
			return result;
		}
	}
}
