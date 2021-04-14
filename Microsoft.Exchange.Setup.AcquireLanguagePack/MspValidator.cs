using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class MspValidator : ValidatorBase
	{
		public MspValidator(string[] msps, string msiFileName, string bundleFileName, string localXMLVersioningFileName, Action<object> callback)
		{
			this.msps = msps;
			this.msiFileName = msiFileName;
			this.bundleFileName = bundleFileName;
			this.localXMLVersioningFileName = localXMLVersioningFileName;
			base.Callback = callback;
		}

		public override bool Validate()
		{
			base.InvokeCallback(Strings.VerifyingMsps);
			ValidationHelper.ThrowIfNullOrEmpty<string>(this.msps, "this.msps");
			ValidationHelper.ThrowIfNullOrEmpty(this.msiFileName, "this.msiFileName");
			if (!File.Exists(this.msiFileName))
			{
				base.InvokeCallback(Strings.NotExist(this.msiFileName));
				return false;
			}
			string[] array = this.msps;
			int i = 0;
			while (i < array.Length)
			{
				string text = array[i];
				bool result;
				if (!File.Exists(text))
				{
					base.InvokeCallback(Strings.NotExist(text));
					result = false;
				}
				else if (!MspUtility.VerifyMspSignature(text))
				{
					base.InvokeCallback(Strings.MspValidationFailedOn("VerifyMspSignature"));
					result = false;
				}
				else
				{
					if (!MspUtility.IsMspInterimUpdate(text))
					{
						i++;
						continue;
					}
					base.InvokeCallback(Strings.MspValidationFailedOn("IsMspInterimUpdate"));
					result = false;
				}
				return result;
			}
			List<string> applicableMsps = MspUtility.GetApplicableMsps(this.msiFileName, true, this.msps);
			if (applicableMsps == null || applicableMsps.Count != this.msps.Length)
			{
				base.InvokeCallback(Strings.MspValidationFailedOn("GetApplicableMsps"));
				return false;
			}
			if (!string.IsNullOrEmpty(this.localXMLVersioningFileName))
			{
				string location;
				string pathToLangPackBundleXML;
				if (string.IsNullOrEmpty(this.bundleFileName))
				{
					location = Assembly.GetExecutingAssembly().Location;
					pathToLangPackBundleXML = this.localXMLVersioningFileName;
				}
				else
				{
					location = this.bundleFileName;
					Directory.CreateDirectory(ValidatorBase.TempPath);
					pathToLangPackBundleXML = LanguagePackXmlHelper.ExtractXMLFromBundle(this.bundleFileName, ValidatorBase.TempPath);
				}
				if (!MspUtility.IsMspCompatibleWithLanguagPack(applicableMsps[applicableMsps.Count - 1], location, this.localXMLVersioningFileName, pathToLangPackBundleXML))
				{
					base.InvokeCallback(Strings.MspValidationFailedOn("IsMspCompatibleWithLanguagPack"));
					return false;
				}
			}
			this.validatedFiles = applicableMsps;
			return true;
		}

		private readonly string[] msps;

		private readonly string msiFileName;

		private readonly string bundleFileName;

		private readonly string localXMLVersioningFileName;
	}
}
