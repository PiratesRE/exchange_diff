using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[DefaultProperty("WizardPages")]
	[DefaultEvent("CurrentPageChanged")]
	public class Wizard : ExchangeUserControl
	{
		public Wizard()
		{
			this.wizardPages = (TypedControlCollection<WizardPage>)base.Controls;
			this.wizardPages.ControlMoved += new ControlMovedEventHandler(this.wizardPages_ControlMoved);
			this.sharedContextFlags = new DataContextFlags();
			this.help = new Command();
			this.help.Name = "help";
			this.help.Text = Strings.Help;
			this.help.Execute += delegate(object param0, EventArgs param1)
			{
				this.OnHelpRequested(new HelpEventArgs(Point.Empty));
			};
			this.reset = new Command();
			this.reset.Name = "reset";
			this.reset.Text = Strings.Reset;
			this.reset.Execute += delegate(object param0, EventArgs param1)
			{
				if (this.reset.Enabled)
				{
					(this.CurrentPage as ISupportResetWizardPage).Reset();
				}
			};
			this.back = new Command();
			this.back.Name = "back";
			this.back.Text = Strings.Back;
			this.back.Execute += delegate(object param0, EventArgs param1)
			{
				if (this.back.Enabled && this.CurrentPage.NotifyGoBack())
				{
					this.CurrentPageIndex--;
				}
			};
			this.next = new Command();
			this.next.Name = "next";
			this.next.Text = Strings.Next;
			this.next.Execute += delegate(object param0, EventArgs param1)
			{
				if (this.next.Enabled && this.CurrentPage.NotifyGoForward())
				{
					this.CurrentPageIndex++;
				}
			};
			this.finish = new Command();
			this.finish.Name = "finish";
			this.finish.Text = Strings.Finish;
			this.finish.Execute += delegate(object param0, EventArgs param1)
			{
				if (this.CanFinish())
				{
					this.CloseParentForm(DialogResult.OK);
				}
			};
			this.cancel = new Command();
			this.cancel.Name = "cancel";
			this.cancel.Text = Strings.Cancel;
			this.cancel.Execute += delegate(object param0, EventArgs param1)
			{
				if (this.CanCancel())
				{
					this.CloseParentForm(DialogResult.Cancel);
				}
			};
			base.Name = "Wizard";
			this.TabStop = false;
			this.UpdateWizardButtons();
		}

		private void CloseParentForm(DialogResult dialogResult)
		{
			Form parentForm = base.ParentForm;
			if (parentForm != null)
			{
				parentForm.DialogResult = dialogResult;
				parentForm.Close();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Command Help
		{
			get
			{
				return this.help;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command Reset
		{
			get
			{
				return this.reset;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command Back
		{
			get
			{
				return this.back;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command Next
		{
			get
			{
				return this.next;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command Finish
		{
			get
			{
				return this.finish;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command Cancel
		{
			get
			{
				return this.cancel;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return WizardPage.defaultSize;
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool TabStop
		{
			get
			{
				return base.TabStop;
			}
			set
			{
				base.TabStop = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[Description("Wizard Pages. Use the collection editor to add or remove pages and set the CurrentPageIndex to design each page.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Wizard")]
		public TypedControlCollection<WizardPage> WizardPages
		{
			get
			{
				return this.wizardPages;
			}
		}

		protected override Control.ControlCollection CreateControlsInstance()
		{
			return new TypedControlCollection<WizardPage>(this);
		}

		[DefaultValue(null)]
		public DataContext Context
		{
			get
			{
				return this.dataContext;
			}
			set
			{
				if (value != this.Context)
				{
					DataContext context = this.Context;
					this.dataContext = value;
					foreach (object obj in this.WizardPages)
					{
						WizardPage wizardPage = (WizardPage)obj;
						if (wizardPage.Context == context)
						{
							wizardPage.Context = value;
						}
					}
				}
			}
		}

		private void wizardPages_ControlMoved(object sender, ControlMovedEventArgs e)
		{
			this.OnWizardPageMoved(e);
		}

		protected virtual void OnWizardPageMoved(ControlMovedEventArgs e)
		{
			if (this.WizardPageMoved != null)
			{
				this.WizardPageMoved.Invoke(this, e);
			}
		}

		[Category("Wizard")]
		public event ControlMovedEventHandler WizardPageMoved;

		protected override void OnControlAdded(ControlEventArgs e)
		{
			Extensions.EnsureDoubleBuffer(e.Control);
			WizardPage wizardPage = (WizardPage)e.Control;
			e.Control.Visible = false;
			e.Control.Dock = DockStyle.Fill;
			if (wizardPage.ParentPage != null && (!base.Controls.Contains(wizardPage.ParentPage) || !wizardPage.ParentPage.ChildPages.Contains(wizardPage)))
			{
				throw new NotSupportedException();
			}
			wizardPage.ChildPages.ListChanged += this.wizardPage_ChildPagesListChanged;
			if (wizardPage.ParentPage != null)
			{
				int num = base.Controls.IndexOf(wizardPage.ParentPage) + 1;
				for (int i = 0; i < wizardPage.ParentPage.ChildPages.IndexOf(wizardPage); i++)
				{
					WizardPage wizardPage2 = wizardPage.ParentPage.ChildPages[i];
					num += wizardPage2.GetChildCount() + 1;
				}
				base.Controls.SetChildIndex(wizardPage, num);
			}
			if (wizardPage.Context == null)
			{
				wizardPage.Context = this.Context;
			}
			this.sharedContextFlags.Pages.Add(wizardPage);
			this.OnWizardPageAdded(e);
			if (1 == this.WizardPages.Count)
			{
				this.CurrentPageIndex = 0;
			}
			foreach (WizardPage value in wizardPage.ChildPages)
			{
				base.Controls.Add(value);
			}
			base.OnControlAdded(e);
			this.UpdateWizardButtons();
		}

		protected virtual void OnWizardPageAdded(ControlEventArgs e)
		{
			if (this.WizardPageAdded != null)
			{
				this.WizardPageAdded(this, e);
			}
		}

		[Category("Wizard")]
		public event ControlEventHandler WizardPageAdded;

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			WizardPage wizardPage = (WizardPage)e.Control;
			wizardPage.ChildPages.ListChanged -= this.wizardPage_ChildPagesListChanged;
			if (wizardPage.ParentPage != null && base.Controls.Contains(wizardPage.ParentPage) && wizardPage.ParentPage.ChildPages.Contains(wizardPage))
			{
				throw new NotSupportedException();
			}
			foreach (WizardPage value in wizardPage.ChildPages)
			{
				base.Controls.Remove(value);
			}
			if (this.CurrentPage == wizardPage)
			{
				this.CurrentPage = ((this.WizardPages.Count > 0) ? this.WizardPages[0] : null);
			}
			else
			{
				this.UpdateWizardButtons();
			}
			this.sharedContextFlags.Pages.Remove(wizardPage);
			this.OnWizardPageRemoved(e);
			base.OnControlRemoved(e);
			this.UpdateWizardButtons();
		}

		protected virtual void OnWizardPageRemoved(ControlEventArgs e)
		{
			if (this.WizardPageRemoved != null)
			{
				this.WizardPageRemoved(this, e);
			}
		}

		[Category("Wizard")]
		public event ControlEventHandler WizardPageRemoved;

		[Category("Wizard")]
		public event EventHandler CurrentPageChanged;

		protected virtual void OnCurrentPageChanged()
		{
			if (this.CurrentPageChanged != null)
			{
				this.CurrentPageChanged(this, EventArgs.Empty);
			}
		}

		[Category("Wizard")]
		[Browsable(false)]
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public WizardPage CurrentPage
		{
			get
			{
				return this.currentPage;
			}
			set
			{
				if (value != this.CurrentPage)
				{
					base.SuspendLayout();
					try
					{
						if (this.CurrentPage != null)
						{
							if (!this.CurrentPage.OnKillActive())
							{
								return;
							}
							this.CurrentPage.CanGoBackChanged -= this.UpdateButtons;
							this.CurrentPage.CanGoForwardChanged -= this.UpdateButtons;
							this.CurrentPage.CanFinishChanged -= this.UpdateButtons;
							this.CurrentPage.CanCancelChanged -= this.UpdateButtons;
							this.CurrentPage.NextButtonTextChanged -= this.NextButtonChanged;
							this.CurrentPage.TextChanged -= this.CurrentPage_TextChanged;
							this.CurrentPage.Visible = false;
						}
						this.currentPage = value;
						this.UpdateWizardButtons();
						if (this.CurrentPage != null)
						{
							this.CurrentPage.CanGoBackChanged += this.UpdateButtons;
							this.CurrentPage.CanGoForwardChanged += this.UpdateButtons;
							this.CurrentPage.CanFinishChanged += this.UpdateButtons;
							this.CurrentPage.CanCancelChanged += this.UpdateButtons;
							this.CurrentPage.NextButtonTextChanged += this.NextButtonChanged;
							this.CurrentPage.TextChanged += this.CurrentPage_TextChanged;
							this.CurrentPage.Visible = true;
							this.CurrentPage.OnSetActive();
							this.Text = this.CurrentPage.Text;
							this.TabStop = this.CurrentPage.TabStop;
							this.next.Text = this.CurrentPage.NextButtonText;
							this.CurrentPage.Select();
						}
						else
						{
							this.Text = "";
							this.TabStop = false;
							this.next.Text = Strings.Next;
						}
						this.OnCurrentPageChanged();
					}
					finally
					{
						base.ResumeLayout(true);
					}
				}
			}
		}

		[Category("Wizard")]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Index of the current Wizard Page. Use the collection editor in WizardPages to add or remove pages and set the CurrentPageIndex to design each page.")]
		public int CurrentPageIndex
		{
			get
			{
				return this.WizardPages.IndexOf(this.CurrentPage);
			}
			set
			{
				this.CurrentPage = this.WizardPages[value];
			}
		}

		private void CurrentPage_TextChanged(object sender, EventArgs e)
		{
			this.Text = this.CurrentPage.Text;
		}

		private void NextButtonChanged(object sender, EventArgs e)
		{
			this.next.Text = this.CurrentPage.NextButtonText;
		}

		private void UpdateButtons(object sender, EventArgs e)
		{
			this.UpdateWizardButtons();
		}

		protected void UpdateWizardButtons()
		{
			this.OnUpdatingButtons(EventArgs.Empty);
			try
			{
				if (this.WizardPages.Count != 0)
				{
					bool flag = this.WizardPages.Count > 1;
					bool flag2 = this.CurrentPage == this.WizardPages[0];
					bool flag3 = this.CurrentPage == this.WizardPages[this.WizardPages.Count - 1];
					this.back.Enabled = (!flag2 && flag && this.CurrentPage.CanGoBack);
					this.next.Enabled = (!flag3 && flag && this.CurrentPage.CanGoForward);
					this.finish.Enabled = this.CurrentPage.CanFinish;
					this.cancel.Enabled = this.CurrentPage.CanCancel;
					this.back.Visible = flag;
					this.next.Visible = (!flag3 && flag);
					this.finish.Visible = flag3;
					this.reset.Visible = (this.CurrentPage is ISupportResetWizardPage);
				}
				else
				{
					this.back.Visible = false;
					this.next.Visible = false;
					this.finish.Visible = false;
					this.back.Enabled = false;
					this.next.Enabled = false;
					this.finish.Enabled = false;
					this.cancel.Enabled = true;
				}
			}
			finally
			{
				this.OnButtonsUpdated(EventArgs.Empty);
			}
		}

		protected virtual void OnUpdatingButtons(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[Wizard.EventUpdatingButtons];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler UpdatingButtons
		{
			add
			{
				base.Events.AddHandler(Wizard.EventUpdatingButtons, value);
			}
			remove
			{
				base.Events.RemoveHandler(Wizard.EventUpdatingButtons, value);
			}
		}

		protected virtual void OnButtonsUpdated(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[Wizard.EventButtonsUpdated];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler ButtonsUpdated
		{
			add
			{
				base.Events.AddHandler(Wizard.EventButtonsUpdated, value);
			}
			remove
			{
				base.Events.RemoveHandler(Wizard.EventButtonsUpdated, value);
			}
		}

		public bool CanCancel()
		{
			bool result = true;
			if (this.CurrentPage != null)
			{
				result = this.CurrentPage.NotifyCancel();
			}
			return result;
		}

		public bool CanFinish()
		{
			bool result = false;
			if (this.CurrentPage != null && (this.CurrentPage.OnKillActive() || this.CurrentPage.NotifyFinish()))
			{
				result = true;
			}
			return result;
		}

		private void wizardPage_ChildPagesListChanged(object sender, ListChangedEventArgs e)
		{
			WizardPageCollection wizardPageCollection = (WizardPageCollection)sender;
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				base.Controls.Add(wizardPageCollection[e.NewIndex]);
				return;
			}
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				int num = base.Controls.IndexOf(wizardPageCollection.ParentPage) + 1;
				for (int i = 0; i < e.NewIndex; i++)
				{
					num += wizardPageCollection[i].GetChildCount() + 1;
				}
				base.Controls.RemoveAt(num);
				return;
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				int j = base.Controls.Count;
				while (j > 0)
				{
					j--;
					WizardPage wizardPage = (WizardPage)base.Controls[j];
					if (wizardPage.ParentPage == wizardPageCollection.ParentPage)
					{
						base.Controls.Remove(wizardPage);
					}
				}
				foreach (WizardPage value in wizardPageCollection)
				{
					base.Controls.Add(value);
				}
			}
		}

		internal bool IsDirty
		{
			get
			{
				bool result = false;
				foreach (object obj in this.WizardPages)
				{
					WizardPage wizardPage = (WizardPage)obj;
					if (wizardPage.IsHandleCreated && wizardPage.IsDirty)
					{
						result = true;
						break;
					}
				}
				return result;
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDataChanged
		{
			get
			{
				return this.isDataChanged;
			}
			internal set
			{
				this.isDataChanged = value;
			}
		}

		private const string WizardCategory = "Wizard";

		private TypedControlCollection<WizardPage> wizardPages;

		private DataContextFlags sharedContextFlags;

		private Command help;

		private Command reset;

		private Command back;

		private Command next;

		private Command finish;

		private Command cancel;

		private DataContext dataContext;

		private WizardPage currentPage;

		private static readonly object EventUpdatingButtons = new object();

		private static readonly object EventButtonsUpdated = new object();

		private bool isDataChanged;
	}
}
