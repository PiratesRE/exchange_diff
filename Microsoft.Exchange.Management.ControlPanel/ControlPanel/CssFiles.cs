using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.UI;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class CssFiles
	{
		static CssFiles()
		{
			CssFiles.cultureAwareCssFileTable[31748] = (CssFiles.cultureAwareCssFileTable[3076] = (CssFiles.cultureAwareCssFileTable[5124] = (CssFiles.cultureAwareCssFileTable[1028] = new CssFiles.CssFile("main_zht.css", true))));
			CssFiles.cultureAwareCssFileTable[4] = (CssFiles.cultureAwareCssFileTable[4100] = (CssFiles.cultureAwareCssFileTable[2052] = new CssFiles.CssFile("main_zhs.css", true)));
			CssFiles.cultureAwareCssFileTable[17] = (CssFiles.cultureAwareCssFileTable[1041] = new CssFiles.CssFile("main_ja.css", true));
			CssFiles.cultureAwareCssFileTable[18] = (CssFiles.cultureAwareCssFileTable[1042] = new CssFiles.CssFile("main_ko.css", true));
			CssFiles.cultureAwareCssFileTable[1066] = new CssFiles.CssFile("main_vi.css", true);
			CssFiles.cultureAwareCssFileTable[1098] = (CssFiles.cultureAwareCssFileTable[1100] = new CssFiles.CssFile("main_in.css", true));
			CssFiles.cultureAwareCssFileTable[1056] = new CssFiles.CssFile("main_ur.css", true);
			CssFiles.nameToCssFileTable = new Dictionary<string, CssFiles.CssFile>(StringComparer.OrdinalIgnoreCase);
			CssFiles.nameToCssFileTable.Add(CssFiles.navCombine.FileName, CssFiles.navCombine);
			CssFiles.nameToCssFileTable.Add(CssFiles.homePageSpriteCss.FileName, CssFiles.homePageSpriteCss);
			CssFiles.nameToCssFileTable.Add(CssFiles.voicemailSpriteCss.FileName, CssFiles.voicemailSpriteCss);
			CssFiles.nameToCssFileTable.Add(CssFiles.editorSpriteCss.FileName, CssFiles.editorSpriteCss);
			CssFiles.nameToCssFileTable.Add(CssFiles.editorStyleCss.FileName, CssFiles.editorStyleCss);
		}

		public static void RenderCssLinks(Control control, IEnumerable<string> cssFiles)
		{
			bool isRtl = RtlUtil.IsRtl;
			CssFiles.CssFile cssFile = null;
			if (!CssFiles.cultureAwareCssFileTable.TryGetValue(CultureInfo.CurrentUICulture.LCID, out cssFile))
			{
				cssFile = CssFiles.mainDefaultCss;
			}
			CssFiles.OutputCssLink(control, cssFile, isRtl);
			if (cssFiles != null)
			{
				foreach (string text in cssFiles)
				{
					CssFiles.CssFile cssFile2 = CssFiles.nameToCssFileTable[text];
					if (cssFile2 == null)
					{
						throw new InvalidOperationException(string.Format("File name {0} isn't map to any predefined CssFile. Make sure you type the correct css file name.", text));
					}
					CssFiles.OutputCssLink(control, cssFile2, isRtl);
				}
			}
		}

		private static void OutputCssLink(Control control, CssFiles.CssFile cssFile, bool isRtl)
		{
			CssFiles.OutputCssLink(control.Page.Response.Output, ThemeResource.GetThemeResource(control.Page, (isRtl && cssFile.RtlFileName != null) ? cssFile.RtlFileName : cssFile.FileName));
		}

		private static void OutputCssLink(TextWriter writer, string cssFileUrl)
		{
			writer.Write("<link href=\"");
			writer.Write(cssFileUrl);
			writer.Write("\" type=\"text/css\" rel=\"stylesheet\" />");
		}

		public static CssFiles.CssFile HighContrastCss
		{
			get
			{
				return CssFiles.highContrastCss;
			}
		}

		public static string ToUrl(this CssFiles.CssFile cssFile, Control control)
		{
			return ThemeResource.GetThemeResource(control, (RtlUtil.IsRtl && cssFile.RtlFileName != null) ? cssFile.RtlFileName : cssFile.FileName);
		}

		public const string NavCombine = "NavCombine.css";

		public const string HomePageSprite = "HomePageSprite.css";

		public const string VoicemailSprite = "VoicemailSprite.css";

		public const string EditorSprite = "nbsprite1.mouse.css";

		public const string EditorStyle = "EditorStyles.mouse.css";

		public const string HighContrast = "HighContrast.css";

		private static CssFiles.CssFile mainDefaultCss = new CssFiles.CssFile("main_default.css", true);

		private static CssFiles.CssFile navCombine = new CssFiles.CssFile("NavCombine.css", true);

		private static CssFiles.CssFile homePageSpriteCss = new CssFiles.CssFile("HomePageSprite.css", true);

		private static CssFiles.CssFile voicemailSpriteCss = new CssFiles.CssFile("VoicemailSprite.css", true);

		private static CssFiles.CssFile editorSpriteCss = new CssFiles.CssFile("nbsprite1.mouse.css", false);

		private static CssFiles.CssFile editorStyleCss = new CssFiles.CssFile("EditorStyles.mouse.css", false);

		private static CssFiles.CssFile highContrastCss = new CssFiles.CssFile("HighContrast.css", true);

		private static Dictionary<int, CssFiles.CssFile> cultureAwareCssFileTable = new Dictionary<int, CssFiles.CssFile>();

		private static Dictionary<string, CssFiles.CssFile> nameToCssFileTable;

		public class CssFile
		{
			public CssFile(string fileName, bool hasRtlFile = true)
			{
				this.FileName = fileName;
				if (hasRtlFile)
				{
					this.RtlFileName = Path.GetFileNameWithoutExtension(fileName) + "-rtl" + Path.GetExtension(fileName);
				}
			}

			public string FileName { get; private set; }

			public string RtlFileName { get; private set; }
		}
	}
}
