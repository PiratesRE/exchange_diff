using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.CabUtility;
using Microsoft.Exchange.Setup.SignatureVerification;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class LanguagePackValidator : ValidatorBase
	{
		public LanguagePackValidator(string bundleFileName, string localXMLVersioningFileName, Action<object> callback)
		{
			this.bundleFileName = bundleFileName;
			this.localXMLVersioningFileName = localXMLVersioningFileName;
			base.Callback = callback;
		}

		public override bool Validate()
		{
			base.InvokeCallback(Strings.VerifyingLangPackBundle);
			ValidationHelper.ThrowIfNullOrEmpty(this.bundleFileName, "this.bundleFileName");
			if (!File.Exists(this.bundleFileName))
			{
				base.InvokeCallback(Strings.NotExist(this.bundleFileName));
				return false;
			}
			Exception ex = null;
			try
			{
				FileInfo fileInfo = new FileInfo(this.bundleFileName);
				if (fileInfo.Length < 1L)
				{
					throw new LanguagePackBundleLoadException(Strings.InvalidFile(this.bundleFileName));
				}
				using (DiskSpaceValidator diskSpaceValidator = new DiskSpaceValidator(500000000L, Environment.GetEnvironmentVariable("windir"), base.Callback))
				{
					if (!diskSpaceValidator.Validate())
					{
						throw new LanguagePackBundleLoadException(Strings.NotEnoughDiskSpace);
					}
				}
				if (!this.VerifyBundleSignature(true))
				{
					base.InvokeCallback(Strings.SignatureVerificationFailed(this.bundleFileName));
				}
				Directory.CreateDirectory(ValidatorBase.TempPath);
				string languageBundleXMLVersioningFileName = LanguagePackXmlHelper.ExtractXMLFromBundle(this.bundleFileName, ValidatorBase.TempPath);
				this.VerifyExchangeVersionInRange(languageBundleXMLVersioningFileName);
			}
			catch (UnauthorizedAccessException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (LanguagePackBundleLoadException ex4)
			{
				ex = ex4;
			}
			catch (SignatureVerificationException ex5)
			{
				ex = ex5;
			}
			catch (LPVersioningValueException ex6)
			{
				ex = ex6;
			}
			catch (CabUtilityWrapperException ex7)
			{
				ex = ex7;
			}
			if (ex == null)
			{
				return true;
			}
			base.InvokeCallback(ex);
			return false;
		}

		private void VerifyExchangeVersionInRange(string languageBundleXMLVersioningFileName)
		{
			if (!File.Exists(languageBundleXMLVersioningFileName))
			{
				throw new LanguagePackBundleLoadException(Strings.IncompatibleBundle);
			}
			LanguagePackVersion languagePackVersion = new LanguagePackVersion(this.localXMLVersioningFileName, languageBundleXMLVersioningFileName);
			string fileVersion = FileVersionInfo.GetVersionInfo(this.bundleFileName).FileVersion;
			if (fileVersion == null)
			{
				throw new LanguagePackBundleLoadException(Strings.IncompatibleBundle);
			}
			if (!languagePackVersion.IsExchangeInApplicableRange(new Version(fileVersion)))
			{
				throw new LanguagePackBundleLoadException(Strings.IncompatibleBundle);
			}
		}

		private bool VerifyBundleSignature(bool checkIfMicrosoftTrusted)
		{
			string location = Assembly.GetExecutingAssembly().Location;
			SignVerfWrapper signVerfWrapper = new SignVerfWrapper();
			bool result;
			try
			{
				bool flag;
				if (signVerfWrapper.VerifyEmbeddedSignature(location, false))
				{
					if (checkIfMicrosoftTrusted)
					{
						flag = signVerfWrapper.IsFileMicrosoftTrusted(this.bundleFileName, true);
					}
					else
					{
						flag = signVerfWrapper.VerifyEmbeddedSignature(this.bundleFileName, true);
					}
				}
				else
				{
					flag = true;
				}
				this.validatedFiles.Add(this.bundleFileName);
				result = flag;
			}
			catch (SignatureVerificationException obj)
			{
				base.InvokeCallback(obj);
				result = false;
			}
			return result;
		}

		private readonly string bundleFileName;

		private readonly string localXMLVersioningFileName;
	}
}
