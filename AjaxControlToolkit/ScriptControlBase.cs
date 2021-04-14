using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.Web.UI;

namespace AjaxControlToolkit
{
	public class ScriptControlBase : ScriptControl, INamingContainer, IControlResolver, IPostBackDataHandler, ICallbackEventHandler, IClientStateManager
	{
		public ScriptControlBase(HtmlTextWriterTag tag) : this(false, tag)
		{
		}

		protected ScriptControlBase() : this(false)
		{
		}

		protected ScriptControlBase(string tag) : this(false, tag)
		{
		}

		protected ScriptControlBase(bool enableClientState)
		{
			this.enableClientState = enableClientState;
		}

		protected ScriptControlBase(bool enableClientState, HtmlTextWriterTag tag)
		{
			this.tagKey = tag;
			this.enableClientState = enableClientState;
		}

		protected ScriptControlBase(bool enableClientState, string tag)
		{
			this.tagKey = HtmlTextWriterTag.Unknown;
			this.tagName = tag;
			this.enableClientState = enableClientState;
		}

		[DefaultValue("")]
		public virtual string ScriptPath
		{
			get
			{
				return (string)(this.ViewState["ScriptPath"] ?? string.Empty);
			}
			set
			{
				this.ViewState["ScriptPath"] = value;
			}
		}

		protected virtual string ClientControlType
		{
			get
			{
				ClientScriptResourceAttribute clientScriptResourceAttribute = (ClientScriptResourceAttribute)TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
				return clientScriptResourceAttribute.ComponentType;
			}
		}

		protected virtual bool SupportsClientState
		{
			get
			{
				return this.enableClientState;
			}
		}

		protected ScriptManager ScriptManager
		{
			get
			{
				this.EnsureScriptManager();
				return this.scriptManager;
			}
		}

		protected string ClientStateFieldID
		{
			get
			{
				if (this.cachedClientStateFieldID == null)
				{
					this.cachedClientStateFieldID = this.ClientID + "_ClientState";
				}
				return this.cachedClientStateFieldID;
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return this.tagKey;
			}
		}

		protected override string TagName
		{
			get
			{
				if (this.tagName == null && this.TagKey != HtmlTextWriterTag.Unknown)
				{
					this.tagName = Enum.Format(typeof(HtmlTextWriterTag), this.TagKey, "G").ToLower(CultureInfo.InvariantCulture);
				}
				return this.tagName;
			}
		}

		private void EnsureScriptManager()
		{
			if (this.scriptManager == null)
			{
				this.scriptManager = ScriptManager.GetCurrent(this.Page);
				if (this.scriptManager == null)
				{
					throw new HttpException(Resources.E_NoScriptManager);
				}
			}
		}

		public override Control FindControl(string id)
		{
			Control control = base.FindControl(id);
			if (control != null)
			{
				return control;
			}
			for (Control namingContainer = this.NamingContainer; namingContainer != null; namingContainer = namingContainer.NamingContainer)
			{
				control = namingContainer.FindControl(id);
				if (control != null)
				{
					return control;
				}
			}
			return null;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			base.EnsureID();
			this.EnsureScriptManager();
			if (this.SupportsClientState)
			{
				this.Page.ClientScript.RegisterHiddenField(this.ClientStateFieldID, this.SaveClientState());
				this.Page.RegisterRequiresPostBack(this);
			}
			ScriptObjectBuilder.RegisterCssReferences(this);
		}

		protected virtual void LoadClientState(string clientState)
		{
		}

		protected virtual string SaveClientState()
		{
			return null;
		}

		protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			if (this.SupportsClientState)
			{
				string text = postCollection[this.ClientStateFieldID];
				if (!string.IsNullOrEmpty(text))
				{
					this.LoadClientState(text);
				}
			}
			return false;
		}

		protected virtual void RaisePostDataChangedEvent()
		{
		}

		protected sealed override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			if (!this.Visible)
			{
				return null;
			}
			base.EnsureID();
			List<ScriptDescriptor> list = this.CreateScriptDescriptors();
			this.BuildScriptDescriptor((ScriptComponentDescriptor)list[0]);
			return list;
		}

		protected virtual List<ScriptDescriptor> CreateScriptDescriptors()
		{
			List<ScriptDescriptor> list = new List<ScriptDescriptor>(1);
			ScriptControlDescriptor item = new ScriptControlDescriptor(this.ClientControlType, this.ClientID);
			list.Add(item);
			return list;
		}

		protected virtual void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
		}

		protected override IEnumerable<ScriptReference> GetScriptReferences()
		{
			if (!this.Visible)
			{
				return null;
			}
			List<ScriptReference> list = new List<ScriptReference>();
			list.AddRange(ScriptObjectBuilder.GetScriptReferences(base.GetType()));
			if (this.ScriptPath.Length > 0)
			{
				list.Add(new ScriptReference(this.ScriptPath));
			}
			return list;
		}

		protected virtual string GetCallbackResult()
		{
			string text = this.callbackArgument;
			this.callbackArgument = null;
			return ScriptObjectBuilder.ExecuteCallbackMethod(this, text);
		}

		protected virtual void RaiseCallbackEvent(string eventArgument)
		{
			this.callbackArgument = eventArgument;
		}

		public Control ResolveControl(string controlId)
		{
			return this.FindControl(controlId);
		}

		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			return this.LoadPostData(postDataKey, postCollection);
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent()
		{
			this.RaisePostDataChangedEvent();
		}

		string ICallbackEventHandler.GetCallbackResult()
		{
			return this.GetCallbackResult();
		}

		void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
		{
			this.RaiseCallbackEvent(eventArgument);
		}

		bool IClientStateManager.SupportsClientState
		{
			get
			{
				return this.SupportsClientState;
			}
		}

		void IClientStateManager.LoadClientState(string clientState)
		{
			this.LoadClientState(clientState);
		}

		string IClientStateManager.SaveClientState()
		{
			return this.SaveClientState();
		}

		private ScriptManager scriptManager;

		private bool enableClientState;

		private string cachedClientStateFieldID;

		private string callbackArgument;

		private string tagName;

		private HtmlTextWriterTag tagKey;
	}
}
