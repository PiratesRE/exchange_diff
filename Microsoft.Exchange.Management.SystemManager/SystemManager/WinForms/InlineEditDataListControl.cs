using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class InlineEditDataListControl : DataListControl, IFormatModeProvider, IBindableComponent, IComponent, IDisposable
	{
		public InlineEditDataListControl()
		{
			base.Name = "InlineEditDataListControl";
			this.addCommand = new Command();
			this.addCommand.Name = "add";
			this.addCommand.Text = Strings.ListEditAdd;
			this.addCommand.Description = Strings.ListEditAddDescription;
			this.addCommand.Execute += this.addCommand_Execute;
			this.addCommand.Enabled = false;
			this.addCommand.Icon = Icons.Add;
			base.DataListView.AllowRemove = true;
			base.DataListView.ContextMenu.MenuItems.AddRange(new MenuItem[]
			{
				new CommandMenuItem(base.DataListView.InlineEditCommand, base.Components),
				new CommandMenuItem(base.DataListView.RemoveCommand, base.Components)
			});
			base.DataListView.SelectionChanged += this.DataListView_SelectionChanged;
			base.DataListView.BeforeLabelEdit += this.DataListView_BeforeLabelEdit;
			base.DataListView.AfterLabelEdit += this.DataListView_AfterLabelEdit;
			base.DataListView.LabelEdit = false;
			base.EditTextBox.AcceptsReturn = true;
			base.EditTextBox.Cue = Strings.ListEditAddCue;
			base.EditTextBox.Enabled = false;
			base.HandleCreated += delegate(object param0, EventArgs param1)
			{
				base.EditTextBox.Visible = true;
			};
			base.EditTextBox.TextChanged += this.textBox_TextChanged;
			base.EditTextBox.KeyPress += this.textBox_KeyPress;
			base.EditTextBox.FormatModeChanged += delegate(object param0, EventArgs param1)
			{
				this.OnFormatModeChanged(EventArgs.Empty);
			};
			base.ToolStripItems.AddRange(new ToolStripItem[]
			{
				new CommandToolStripButton(this.addCommand),
				new CommandToolStripButton(base.DataListView.InlineEditCommand),
				new CommandToolStripButton(base.DataListView.RemoveCommand)
			});
			base.EditTextBox.MouseClick += delegate(object param0, MouseEventArgs param1)
			{
				base.EditTextBox.Focus();
			};
			new TextBoxConstraintProvider(this, "DataSource", base.EditTextBox);
		}

		[DefaultValue("")]
		public string TextBoxText
		{
			get
			{
				return base.EditTextBox.Text;
			}
			set
			{
				base.EditTextBox.Text = value;
			}
		}

		public string TextBoxCue
		{
			get
			{
				return base.EditTextBox.Cue;
			}
			set
			{
				base.EditTextBox.Cue = value;
			}
		}

		private bool ShouldSerializeTextBoxCue()
		{
			return Strings.ListEditAddCue != this.TextBoxCue;
		}

		private void ResetTextBoxCue()
		{
			this.TextBoxCue = Strings.ListEditAddCue;
		}

		private void addCommand_Execute(object sender, EventArgs e)
		{
			object obj = this.ParseString(base.EditTextBox.Text);
			if (obj != null && this.InternalAddValue(obj))
			{
				this.TextBoxText = "";
			}
			base.EditTextBox.Select();
			base.EditTextBox.SelectAll();
		}

		private void DataListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Label))
			{
				object obj = this.ParseString(e.Label);
				if (obj != null)
				{
					base.InternalEditValue(e.Item, obj, true);
				}
			}
			else if (e.Label != null)
			{
				base.ShowErrorAsync(Strings.InlineEditCannotBlankEntry);
			}
			e.CancelEdit = true;
		}

		private void DataListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
		{
			HandleRef handleRef = new HandleRef(base.DataListView, base.DataListView.Handle);
			IntPtr handle = UnsafeNativeMethods.SendMessage(handleRef, 4120, (IntPtr)0, (IntPtr)0);
			HandleRef handleRef2 = new HandleRef(base.DataListView, handle);
			UnsafeNativeMethods.SendMessage(handleRef2, 197, (IntPtr)base.EditTextBox.MaxLength, (IntPtr)0);
		}

		private void DataListView_SelectionChanged(object sender, EventArgs e)
		{
			if (base.DataList.Count == 0)
			{
				base.EditTextBox.Focus();
			}
		}

		protected override void OnDataSourceChanged(EventArgs e)
		{
			this.ResetParseFunctionality();
			base.OnDataSourceChanged(e);
		}

		private void ResetParseFunctionality()
		{
			bool flag = null != base.DataList;
			bool flag2 = flag && this.CanParse;
			bool flag3 = flag && !base.DataList.IsFixedSize;
			base.EditTextBox.Enabled = (flag2 && flag3);
			base.DataListView.LabelEdit = flag2;
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keyData)
		{
			return (keyData != Keys.Return || !base.EditTextBox.Focused || string.IsNullOrEmpty(base.EditTextBox.Text)) && base.ProcessDialogKey(keyData);
		}

		private void textBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				e.Handled = true;
				if (this.addCommand.Enabled)
				{
					this.addCommand.Invoke();
				}
			}
		}

		private void textBox_TextChanged(object sender, EventArgs e)
		{
			this.addCommand.Enabled = (base.EditTextBox.Enabled && !string.IsNullOrEmpty(base.EditTextBox.Text));
		}

		protected virtual bool CanParse
		{
			get
			{
				if (base.Events[InlineEditDataListControl.EventParse] != null)
				{
					return true;
				}
				MethodInfo method = base.ItemType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					typeof(string)
				}, null);
				if (null != method)
				{
					return true;
				}
				TypeConverter converter = TypeDescriptor.GetConverter(base.ItemType);
				if (converter != null && converter.CanConvertFrom(typeof(string)))
				{
					return true;
				}
				ConstructorInfo constructor = base.ItemType.GetConstructor(new Type[]
				{
					typeof(string)
				});
				return null != constructor;
			}
		}

		private object ParseString(string value)
		{
			try
			{
				ConvertEventHandler convertEventHandler = (ConvertEventHandler)base.Events[InlineEditDataListControl.EventParse];
				if (convertEventHandler != null)
				{
					ConvertEventArgs convertEventArgs = new ConvertEventArgs(value, base.ItemType);
					convertEventHandler(this, convertEventArgs);
					return convertEventArgs.Value;
				}
				MethodInfo method = base.ItemType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					typeof(string)
				}, null);
				if (null != method)
				{
					try
					{
						return method.Invoke(null, new object[]
						{
							value
						});
					}
					catch (TargetInvocationException ex)
					{
						throw new FormatException(ex.InnerException.Message, ex.InnerException);
					}
				}
				TypeConverter converter = TypeDescriptor.GetConverter(base.ItemType);
				if (converter != null && converter.CanConvertFrom(typeof(string)))
				{
					return converter.ConvertFrom(null, CultureInfo.CurrentUICulture, value);
				}
				ConstructorInfo constructor = base.ItemType.GetConstructor(new Type[]
				{
					typeof(string)
				});
				if (null != constructor)
				{
					try
					{
						return constructor.Invoke(new object[]
						{
							value
						});
					}
					catch (TargetInvocationException ex2)
					{
						throw new FormatException(ex2.InnerException.Message, ex2.InnerException);
					}
				}
				return value;
			}
			catch (FormatException ex3)
			{
				base.ShowErrorAsync(ex3.Message);
			}
			catch (ArgumentException ex4)
			{
				base.ShowErrorAsync(ex4.Message);
			}
			return null;
		}

		public event ConvertEventHandler Parse
		{
			add
			{
				base.Events.AddHandler(InlineEditDataListControl.EventParse, value);
				this.ResetParseFunctionality();
			}
			remove
			{
				base.Events.RemoveHandler(InlineEditDataListControl.EventParse, value);
				this.ResetParseFunctionality();
			}
		}

		[DefaultValue(null)]
		[Browsable(false)]
		public Command AddCommand
		{
			get
			{
				return this.addCommand;
			}
		}

		[DefaultValue(0)]
		public DisplayFormatMode FormatMode
		{
			get
			{
				return base.EditTextBox.FormatMode;
			}
			set
			{
				base.EditTextBox.FormatMode = value;
			}
		}

		protected virtual void OnFormatModeChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[InlineEditDataListControl.EventFormatModeChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler FormatModeChanged
		{
			add
			{
				base.Events.AddHandler(InlineEditDataListControl.EventFormatModeChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(InlineEditDataListControl.EventFormatModeChanged, value);
			}
		}

		void IFormatModeProvider.add_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged += A_1;
		}

		void IFormatModeProvider.remove_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged -= A_1;
		}

		private Command addCommand;

		private static readonly object EventParse = new object();

		private static readonly object EventFormatModeChanged = new object();
	}
}
