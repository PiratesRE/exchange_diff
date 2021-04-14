using System;
using System.IO;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class ThemeFileInfoAttribute : Attribute
	{
		internal ThemeFileInfoAttribute() : this(string.Empty, ThemeFileInfoFlags.None, null)
		{
		}

		internal ThemeFileInfoAttribute(string name) : this(name, ThemeFileInfoFlags.None, null)
		{
		}

		internal ThemeFileInfoAttribute(string name, ThemeFileInfoFlags themeFileInfoFlags) : this(name, themeFileInfoFlags, null)
		{
		}

		internal ThemeFileInfoAttribute(string name, ThemeFileInfoFlags themeFileInfoFlags, string fallbackImageName)
		{
			this.name = name;
			this.themeFileInfoFlags = themeFileInfoFlags;
			this.fallbackImageName = fallbackImageName;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string FallbackImageName
		{
			get
			{
				return this.fallbackImageName;
			}
		}

		public bool UseCssSprites
		{
			get
			{
				if (string.IsNullOrEmpty(this.Name))
				{
					return false;
				}
				string extension = Path.GetExtension(this.Name);
				return !string.IsNullOrEmpty(extension) && (extension.Equals(".gif", StringComparison.InvariantCultureIgnoreCase) || extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase)) && !Utilities.IsFlagSet((int)this.themeFileInfoFlags, 1);
			}
		}

		public bool PhaseII
		{
			get
			{
				return this.UseCssSprites && Utilities.IsFlagSet((int)this.themeFileInfoFlags, 2);
			}
		}

		public bool IsResource
		{
			get
			{
				return Utilities.IsFlagSet((int)this.themeFileInfoFlags, 4);
			}
		}

		private string name;

		private ThemeFileInfoFlags themeFileInfoFlags;

		private string fallbackImageName;
	}
}
