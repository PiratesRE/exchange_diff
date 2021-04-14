using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;

namespace AjaxControlToolkit
{
	[PersistChildren(false)]
	[Themeable(true)]
	[ParseChildren(true)]
	public abstract class ExtenderControlBase : ExtenderControl, IControlResolver
	{
		public event ResolveControlEventHandler ResolveControlID;

		[Browsable(true)]
		public override string SkinID
		{
			get
			{
				return base.SkinID;
			}
			set
			{
				base.SkinID = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return !this.isDisposed && this.GetPropertyValue<bool>("Enabled", true);
			}
			set
			{
				this.SetPropertyValue<bool>("Enabled", value);
			}
		}

		public string ScriptPath
		{
			get
			{
				return this.GetPropertyValue<string>("ScriptPath", null);
			}
			set
			{
				if (!this.AllowScriptPath)
				{
					throw new InvalidOperationException("This class does not allow setting of ScriptPath.");
				}
				this.SetPropertyValue<string>("ScriptPath", value);
			}
		}

		public string BehaviorID
		{
			get
			{
				string propertyValue = this.GetPropertyValue<string>("BehaviorID", string.Empty);
				if (!string.IsNullOrEmpty(propertyValue))
				{
					return propertyValue;
				}
				return this.ClientID;
			}
			set
			{
				this.SetPropertyValue<string>("BehaviorID", value);
			}
		}

		[Obsolete("WARNING: ProfileBindings are disabled for this Toolkit release pending technical issues.  We hope to re-enable this in an upcoming release")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ProfilePropertyBindingCollection ProfileBindings
		{
			get
			{
				if (this.profileBindings == null)
				{
					this.profileBindings = new ProfilePropertyBindingCollection();
				}
				return this.profileBindings;
			}
		}

		protected virtual bool AllowScriptPath
		{
			get
			{
				return true;
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

		protected Control TargetControl
		{
			get
			{
				return this.FindControlHelper(base.TargetControlID);
			}
		}

		public override void Dispose()
		{
			this.isDisposed = true;
			base.Dispose();
		}

		public override Control FindControl(string id)
		{
			return this.FindControlHelper(id);
		}

		public virtual void EnsureValid()
		{
			this.CheckIfValid(true);
		}

		public Control ResolveControl(string controlId)
		{
			return this.FindControl(controlId);
		}

		internal IEnumerable<ScriptReference> EnsureScripts()
		{
			List<ScriptReference> list = new List<ScriptReference>();
			list.AddRange(ScriptObjectBuilder.GetScriptReferences(base.GetType(), null != this.ScriptPath));
			string scriptPath = this.ScriptPath;
			if (!string.IsNullOrEmpty(scriptPath))
			{
				list.Add(new ScriptReference(scriptPath));
			}
			return list;
		}

		protected static void SuppressUnusedParameterWarning(object unused)
		{
			if (unused != null)
			{
				unused.GetType();
			}
		}

		protected Control FindControlHelper(string id)
		{
			Control control;
			if (this.findControlHelperCache.ContainsKey(id))
			{
				control = this.findControlHelperCache[id];
			}
			else
			{
				control = base.FindControl(id);
				Control namingContainer = this.NamingContainer;
				while (control == null && namingContainer != null)
				{
					control = namingContainer.FindControl(id);
					namingContainer = namingContainer.NamingContainer;
				}
				if (control == null)
				{
					ResolveControlEventArgs resolveControlEventArgs = new ResolveControlEventArgs(id);
					this.OnResolveControlID(resolveControlEventArgs);
					control = resolveControlEventArgs.Control;
				}
				if (control != null)
				{
					this.findControlHelperCache[id] = control;
				}
			}
			return control;
		}

		protected string GetClientID(string controlId)
		{
			Control control = this.FindControlHelper(controlId);
			if (control != null)
			{
				controlId = control.ClientID;
			}
			return controlId;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.Page != null)
			{
				this.Page.VerifyRenderingInServerForm(this);
			}
			base.Render(writer);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ScriptObjectBuilder.RegisterCssReferences(this);
		}

		protected virtual void OnResolveControlID(ResolveControlEventArgs e)
		{
			if (this.ResolveControlID != null)
			{
				this.ResolveControlID(this, e);
			}
		}

		protected virtual void RenderInnerScript(ScriptBehaviorDescriptor descriptor)
		{
		}

		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
		{
			if (!this.Enabled || !this.TargetControl.Visible)
			{
				return null;
			}
			ScriptBehaviorDescriptor scriptBehaviorDescriptor = new ScriptBehaviorDescriptor(this.ClientControlType, targetControl.ClientID);
			this.BuildScriptDescriptor(scriptBehaviorDescriptor);
			this.RenderInnerScript(scriptBehaviorDescriptor);
			return new List<ScriptDescriptor>(new ScriptDescriptor[]
			{
				scriptBehaviorDescriptor
			});
		}

		protected virtual void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddProperty("id", this.BehaviorID);
		}

		protected override IEnumerable<ScriptReference> GetScriptReferences()
		{
			if (this.Enabled)
			{
				return this.EnsureScripts();
			}
			return null;
		}

		protected virtual bool CheckIfValid(bool throwException)
		{
			bool result = true;
			foreach (object obj in TypeDescriptor.GetProperties(this))
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
				if (propertyDescriptor.Attributes[typeof(RequiredPropertyAttribute)] != null && (propertyDescriptor.GetValue(this) == null || !propertyDescriptor.ShouldSerializeValue(this)))
				{
					result = false;
					if (throwException)
					{
						throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "{0} missing required {1} property value for {2}.", new object[]
						{
							base.GetType().ToString(),
							propertyDescriptor.Name,
							this.ID
						}), propertyDescriptor.Name);
					}
				}
			}
			return result;
		}

		protected V GetPropertyValue<V>(string propertyName, V nullValue)
		{
			if (this.ViewState[propertyName] == null)
			{
				return nullValue;
			}
			return (V)((object)this.ViewState[propertyName]);
		}

		protected void SetPropertyValue<V>(string propertyName, V value)
		{
			this.ViewState[propertyName] = value;
		}

		[Obsolete("Use GetPropertyValue<V> instead")]
		protected string GetPropertyStringValue(string propertyName)
		{
			return this.GetPropertyValue<string>(propertyName, string.Empty);
		}

		[Obsolete("Use SetPropertyValue<V> instead")]
		protected void SetPropertyStringValue(string propertyName, string value)
		{
			this.SetPropertyValue<string>(propertyName, value);
		}

		[Obsolete("Use GetPropertyValue<V> instead")]
		protected int GetPropertyIntValue(string propertyName)
		{
			return this.GetPropertyValue<int>(propertyName, 0);
		}

		[Obsolete("Use SetPropertyValue<V> instead")]
		protected void SetPropertyIntValue(string propertyName, int value)
		{
			this.SetPropertyValue<int>(propertyName, value);
		}

		[Obsolete("Use GetPropertyValue<V> instead")]
		protected bool GetPropertyBoolValue(string propertyName)
		{
			return this.GetPropertyValue<bool>(propertyName, false);
		}

		[Obsolete("Use SetPropertyValue<V> instead")]
		protected void SetPropertyBoolValue(string propertyName, bool value)
		{
			this.SetPropertyValue<bool>(propertyName, value);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		private bool ShouldSerializeProfileBindings()
		{
			return false;
		}

		internal static string[] ForceSerializationProps = new string[]
		{
			"ClientStateFieldID"
		};

		private static string[] noSerializeProps = new string[]
		{
			"TargetControlID",
			"ProfileBindings"
		};

		private Dictionary<string, Control> findControlHelperCache = new Dictionary<string, Control>();

		private bool isDisposed;

		private ProfilePropertyBindingCollection profileBindings;
	}
}
