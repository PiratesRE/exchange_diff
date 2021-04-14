using System;
using System.Linq;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class SlabBinding
	{
		public SlabBinding()
		{
			this.Configurations = new SlabConfiguration[0];
			this.Dependencies = new string[0];
			this.Features = new string[0];
			this.PackagedSources = new SlabSourceFile[0];
			this.Sources = new SlabSourceFile[0];
			this.Styles = new SlabStyleFile[0];
			this.PackagedStrings = new SlabStringFile[0];
			this.Strings = new SlabStringFile[0];
			this.Fonts = new SlabFontFile[0];
			this.Images = new SlabImageFile[0];
		}

		public bool IsDefault
		{
			get
			{
				return string.IsNullOrWhiteSpace(this.Scope) && (this.Features == null || this.Features.Length == 0);
			}
		}

		public string Scope { get; set; }

		public SlabConfiguration[] Configurations { get; set; }

		public SlabSourceFile[] Sources { get; set; }

		public SlabSourceFile[] PackagedSources { get; set; }

		public SlabStyleFile[] Styles { get; set; }

		public SlabStringFile[] Strings { get; set; }

		public SlabStringFile[] PackagedStrings { get; set; }

		public SlabFontFile[] Fonts { get; set; }

		public SlabImageFile[] Images { get; set; }

		public string[] Dependencies { get; set; }

		public string[] Features { get; set; }

		public bool Implement(string[] features)
		{
			if (features == null)
			{
				throw new ArgumentNullException("features");
			}
			return features.All((string f) => this.Features.Contains(f, StringComparer.OrdinalIgnoreCase));
		}
	}
}
