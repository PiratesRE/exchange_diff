using System;
using System.IO;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class UpdatesValidator : ValidatorBase
	{
		public UpdatesValidator(string mspsPath, string msiFileName, string bundleFileName, string localXMLVersioningFileName, Action<object> callback)
		{
			this.mspsPath = mspsPath;
			this.msiFileName = msiFileName;
			this.bundleFileName = bundleFileName;
			this.localXMLVersioningFileName = localXMLVersioningFileName;
			base.Callback = callback;
		}

		public override bool Validate()
		{
			base.InvokeCallback(Strings.VerifyingUpdates);
			ValidationHelper.ThrowIfNullOrEmpty(this.mspsPath, "this.mspsPath");
			ValidationHelper.ThrowIfNullOrEmpty(this.msiFileName, "this.msiFileName");
			ValidationHelper.ThrowIfNullOrEmpty(this.bundleFileName, "this.bundleFileName");
			bool flag = true;
			if (File.Exists(this.bundleFileName))
			{
				using (LanguagePackValidator languagePackValidator = new LanguagePackValidator(this.bundleFileName, this.localXMLVersioningFileName, base.Callback))
				{
					flag = languagePackValidator.Validate();
					if (flag)
					{
						this.validatedFiles.AddRange(languagePackValidator.ValidatedFiles);
					}
				}
			}
			bool flag2 = true;
			if (Directory.Exists(this.mspsPath))
			{
				string[] files = Directory.GetFiles(this.mspsPath, "*.msp", SearchOption.TopDirectoryOnly);
				if (files != null && files.Length > 0)
				{
					using (MspValidator mspValidator = new MspValidator(files, this.msiFileName, File.Exists(this.bundleFileName) ? this.bundleFileName : null, File.Exists(this.localXMLVersioningFileName) ? this.localXMLVersioningFileName : null, base.Callback))
					{
						flag2 = mspValidator.Validate();
						if (flag2)
						{
							this.validatedFiles.AddRange(mspValidator.ValidatedFiles);
						}
					}
				}
			}
			return flag && flag2;
		}

		private readonly string mspsPath;

		private readonly string msiFileName;

		private readonly string bundleFileName;

		private readonly string localXMLVersioningFileName;
	}
}
