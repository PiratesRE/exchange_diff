using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class EcpContentPage : EcpPage, IScriptControl, IThemable
	{
		public EcpContentPage()
		{
			this.ShowHelp = true;
		}

		[DefaultValue(true)]
		public bool ShowHelp { get; protected set; }

		[DefaultValue(false)]
		public bool SkipXssFilter { get; set; }

		public string IncludeCssFiles { get; set; }

		[DefaultValue(false)]
		public bool SkipBEParamCheck { get; set; }

		protected ScriptManager ScriptManager
		{
			get
			{
				if (this.scriptManager == null)
				{
					this.scriptManager = ScriptManager.GetCurrent(this);
				}
				return this.scriptManager;
			}
		}

		public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			ClientScriptResourceAttribute clientScriptResourceAttribute = (ClientScriptResourceAttribute)TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
			if (!clientScriptResourceAttribute.ComponentType.IsNullOrBlank())
			{
				ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor(clientScriptResourceAttribute.ComponentType, "aspnetForm");
				this.BuildScriptDescriptor(scriptControlDescriptor);
				return new ScriptDescriptor[]
				{
					scriptControlDescriptor
				};
			}
			return null;
		}

		public virtual IEnumerable<ScriptReference> GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(base.GetType());
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!this.SkipBEParamCheck)
			{
				this.Context.ThrowIfViewOptionsWithBEParam(base.FeatureSet);
			}
			base.ClientScript.RegisterStartupScript(typeof(EcpContentPage), "HideLoadingGif", "ContentPageUtil.HideLoadingGif();", true);
			if (this.Page.Master != null)
			{
				MasterPage master = this.Page.Master;
				while (master.Master != null)
				{
					master = master.Master;
				}
				CommonMaster commonMaster = master as CommonMaster;
				if (commonMaster != null)
				{
					commonMaster.FeatureSet = base.FeatureSet;
				}
			}
			if (!string.IsNullOrEmpty(this.IncludeCssFiles))
			{
				((CommonMaster)this.Page.Master).AddCssFiles(this.IncludeCssFiles);
			}
			if (base.IsPostBack)
			{
				this.Context.CheckCanaryForPostBack("ecpCanary");
			}
		}

		protected override void OnPreInit(EventArgs e)
		{
			bool showHelp;
			if (bool.TryParse(base.Request.QueryString["showhelp"], out showHelp))
			{
				this.ShowHelp = showHelp;
			}
			base.OnPreInit(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.ScriptManager.RegisterScriptControl<EcpContentPage>(this);
			HttpCookie canaryCookie = HttpContext.Current.GetCanaryCookie();
			string hiddenFieldInitialValue = (canaryCookie == null) ? null : canaryCookie.Value;
			ScriptManager.RegisterHiddenField(this, "ecpCanary", hiddenFieldInitialValue);
			base.ClientScript.RegisterOnSubmitStatement(typeof(EcpContentPage), "SuppressSubmit", "return false;\r\n");
			if (this.SkipXssFilter)
			{
				base.Response.Headers.Add("X-XSS-Protection", "0");
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (!base.DesignMode && !this.ScriptManager.IsInAsyncPostBack)
			{
				this.ScriptManager.RegisterScriptDescriptors(this);
			}
			base.Render(writer);
		}

		protected virtual void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
		}

		protected override void SavePageStateToPersistenceMedium(object state)
		{
			if (this.EnableViewState)
			{
				base.SavePageStateToPersistenceMedium(state);
			}
		}

		public const string FormClientID = "aspnetForm";

		private const string EcpCanaryId = "ecpCanary";

		private ScriptManager scriptManager;
	}
}
