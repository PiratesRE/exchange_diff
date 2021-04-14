using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms.Properties
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		public Settings()
		{
			this.visualEffectsCommand = new Command();
			this.visualEffectsNeverCommand = new Command();
			this.visualEffectsAutomaticCommand = new Command();
			this.visualEffectsCommand.Name = "visualEffectsCommand";
			this.visualEffectsCommand.Text = Strings.VisualEffects;
			this.visualEffectsCommand.Commands.AddRange(new Command[]
			{
				this.visualEffectsNeverCommand,
				this.visualEffectsAutomaticCommand
			});
			this.visualEffectsNeverCommand.Name = "visualEffectsNeverCommand";
			this.visualEffectsNeverCommand.Text = Strings.VisualEffectsNever;
			this.visualEffectsNeverCommand.Execute += delegate(object param0, EventArgs param1)
			{
				this.EnableVisualEffects = "Never";
			};
			this.visualEffectsAutomaticCommand.Name = "visualEffectsAutomaticCommand";
			this.visualEffectsAutomaticCommand.Text = Strings.VisualEffectsAutomatic;
			this.visualEffectsAutomaticCommand.Execute += delegate(object param0, EventArgs param1)
			{
				this.EnableVisualEffects = "Automatic";
			};
			this.OnPropertyChanged(this, new PropertyChangedEventArgs("EnableVisualEffects"));
		}

		protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.visualEffectsNeverCommand.Checked = (0 == StringComparer.InvariantCultureIgnoreCase.Compare("Never", this.EnableVisualEffects));
			this.visualEffectsAutomaticCommand.Checked = !this.visualEffectsNeverCommand.Checked;
			base.OnPropertyChanged(sender, e);
			this.Save();
		}

		public bool UseVisualEffects
		{
			get
			{
				if (this.visualEffectsNeverCommand.Checked)
				{
					return false;
				}
				if (this.visualEffectsAutomaticCommand.Checked)
				{
					return !SystemInformation.HighContrast;
				}
				return SystemInformation.DragFullWindows || !SystemInformation.TerminalServerSession;
			}
		}

		public Command VisualEffectsCommands
		{
			get
			{
				return this.visualEffectsCommand;
			}
		}

		private const string Never = "Never";

		private const string Automatic = "Automatic";

		private Command visualEffectsCommand;

		private Command visualEffectsNeverCommand;

		private Command visualEffectsAutomaticCommand;
	}
}
