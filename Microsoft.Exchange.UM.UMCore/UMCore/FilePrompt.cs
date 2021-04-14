using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FilePrompt : Prompt
	{
		public FilePrompt()
		{
		}

		internal FilePrompt(string fileName, CultureInfo culture) : base(fileName, culture)
		{
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"wave",
				this.filename,
				this.filename,
				this.filename
			});
		}

		internal static string BuildAudioSsml(string fileName, string prosodyRate)
		{
			return string.Format(CultureInfo.InvariantCulture, "<prosody rate=\"{0}\"><audio src=\"{1}\" /></prosody>", new object[]
			{
				prosodyRate,
				SpeechUtils.XmlEncode(fileName)
			});
		}

		internal override string ToSsml()
		{
			return FilePrompt.BuildAudioSsml(this.filename, base.ProsodyRate);
		}

		protected override void InternalInitialize()
		{
			this.filename = Path.Combine(Util.WavPathFromCulture(base.Culture), base.Config.PromptName);
			if (!GlobalActivityManager.ConfigClass.RecordingFileNameCache.Contains(this.filename) && !File.Exists(this.filename))
			{
				throw new FileNotFoundException(this.filename);
			}
		}

		internal const string SSMLTemplate = "<audio src=\"{0}\" />";

		internal const string SSMLProsodyTemplate = "<prosody rate=\"{0}\"><audio src=\"{1}\" /></prosody>";

		private string filename;
	}
}
