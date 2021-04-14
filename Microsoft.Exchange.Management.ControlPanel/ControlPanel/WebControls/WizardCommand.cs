using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class WizardCommand : PopupCommand
	{
		public WizardCommand() : this(null, CommandSprite.SpriteId.NONE)
		{
		}

		public WizardCommand(string text, CommandSprite.SpriteId imageID) : base(text, imageID)
		{
			this.Name = "New";
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override Size DialogSize
		{
			get
			{
				return base.DialogSize;
			}
			set
			{
				base.DialogSize = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override bool Resizable
		{
			get
			{
				return base.Resizable;
			}
			set
			{
				base.Resizable = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool UseDefaultWindow
		{
			get
			{
				return base.UseDefaultWindow;
			}
			set
			{
				base.UseDefaultWindow = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowAddressBar
		{
			get
			{
				return base.ShowAddressBar;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ShowMenuBar
		{
			get
			{
				return base.ShowMenuBar;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowStatusBar
		{
			get
			{
				return base.ShowStatusBar;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowToolBar
		{
			get
			{
				return base.ShowToolBar;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		public BindingCollection QueryParameters
		{
			get
			{
				return this.queryParameters;
			}
		}

		public override string NavigateUrl
		{
			get
			{
				this.UpdateNavigateUrlIfRequired();
				return base.NavigateUrl;
			}
			set
			{
				base.NavigateUrl = value;
			}
		}

		private void UpdateNavigateUrlIfRequired()
		{
			if (!this.isQuertyParametersInitializied && this.QueryParameters.Count > 0)
			{
				string text = base.NavigateUrl;
				if (string.IsNullOrEmpty(text))
				{
					throw new ArgumentException("NavigateUrl must be specified, QueryParameters is used.");
				}
				foreach (Binding binding in this.QueryParameters)
				{
					StaticBinding staticBinding = (StaticBinding)binding;
					if (staticBinding.HasValue || !staticBinding.Optional)
					{
						if (staticBinding.Value is Identity)
						{
							text = EcpUrl.AppendQueryParameter(text, staticBinding.Name, ((Identity)staticBinding.Value).RawIdentity);
						}
						else
						{
							text = EcpUrl.AppendQueryParameter(text, staticBinding.Name, staticBinding.Value.ToString());
						}
					}
				}
				base.NavigateUrl = text;
				this.isQuertyParametersInitializied = true;
			}
		}

		private BindingCollection queryParameters = new BindingCollection();

		private bool isQuertyParametersInitializied;
	}
}
