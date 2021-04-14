using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SystemManager.WinForms.Properties;
using Microsoft.ManagementGUI.Commands;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public static class Theme
	{
		static Theme()
		{
			SystemEvents.DisplaySettingsChanged += Theme.OnVisualSettingsChanged;
			SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(Theme.OnVisualSettingsChanged);
			SystemEvents.SessionSwitch += new SessionSwitchEventHandler(Theme.OnVisualSettingsChanged);
			Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Theme.OnVisualSettingsChanged);
		}

		public static bool UseVisualEffects
		{
			get
			{
				return Settings.Default.UseVisualEffects;
			}
		}

		public static event EventHandler UseVisualEffectsChanged;

		private static void OnVisualSettingsChanged(object sender, EventArgs e)
		{
			EventHandler useVisualEffectsChanged = Theme.UseVisualEffectsChanged;
			if (useVisualEffectsChanged != null)
			{
				useVisualEffectsChanged(null, EventArgs.Empty);
			}
		}

		public static Command VisualEffectsCommands
		{
			get
			{
				return Settings.Default.VisualEffectsCommands;
			}
		}

		public static ControlStyles UserPaintStyle
		{
			get
			{
				return ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer;
			}
		}
	}
}
