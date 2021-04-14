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
				return !string.IsNullOrEmpty(extension) && (extension.Equals(".gif", StringComparison.OrdinalIgnoreCase) || extension.Equals(".png", StringComparison.OrdinalIgnoreCase)) && !ThemeFileInfoAttribute.IsFlagSet(this.themeFileInfoFlags, ThemeFileInfoFlags.LooseImage);
			}
		}

		public bool PhaseII
		{
			get
			{
				return this.UseCssSprites && ThemeFileInfoAttribute.IsFlagSet(this.themeFileInfoFlags, ThemeFileInfoFlags.PhaseII);
			}
		}

		public bool IsResource
		{
			get
			{
				return ThemeFileInfoAttribute.IsFlagSet(this.themeFileInfoFlags, ThemeFileInfoFlags.Resource);
			}
		}

		private static bool IsFlagSet(ThemeFileInfoFlags valueToTest, ThemeFileInfoFlags flag)
		{
			return (valueToTest & flag) == flag;
		}

		private string name;

		private ThemeFileInfoFlags themeFileInfoFlags;

		private string fallbackImageName;
	}
}
