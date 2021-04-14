using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Cryptography;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class AlternateServiceAccountCredential : ConfigurableObject, IComparable<AlternateServiceAccountCredential>, IDisposable
	{
		private AlternateServiceAccountCredential(string registryValueName, Exception parseError) : base(new SimpleProviderPropertyBag())
		{
			this.registryValueName = registryValueName;
			this.propertyBag.SetField(SimpleProviderObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2010);
			this.parseError = parseError;
		}

		private AlternateServiceAccountCredential(string registryValueName, Exception parseError, bool canSave, DateTime whenAddedUtc, string domain, string userName, SecureString password) : this(registryValueName, parseError)
		{
			this[AlternateServiceAccountCredentialSchema.WhenAddedUTC] = whenAddedUtc;
			this[AlternateServiceAccountCredentialSchema.Domain] = domain;
			this[AlternateServiceAccountCredentialSchema.UserName] = userName;
			this.password = password;
			this.propertyBag.SetIsReadOnly(!canSave);
		}

		public string Domain
		{
			get
			{
				return (string)this[AlternateServiceAccountCredentialSchema.Domain];
			}
		}

		public string UserName
		{
			get
			{
				return (string)this[AlternateServiceAccountCredentialSchema.UserName];
			}
		}

		public string QualifiedUserName
		{
			get
			{
				return this.Domain + "\\" + this.UserName;
			}
		}

		public PSCredential Credential
		{
			get
			{
				if (this.Password == null)
				{
					return null;
				}
				return new PSCredential(this.QualifiedUserName, this.Password);
			}
		}

		public DateTime? WhenAdded
		{
			get
			{
				return (DateTime?)this[AlternateServiceAccountCredentialSchema.WhenAdded];
			}
		}

		public DateTime? WhenAddedUTC
		{
			get
			{
				return (DateTime?)this[AlternateServiceAccountCredentialSchema.WhenAddedUTC];
			}
		}

		public override string ToString()
		{
			return (this.WhenAdded != null) ? DirectoryStrings.AlternateServiceAccountCredentialDisplayFormat(this.IsValid ? string.Empty : DirectoryStrings.AlternateServiceAccountCredentialIsInvalid, this.WhenAdded.Value, this.Domain, this.UserName) : DirectoryStrings.AlternateServiceAccountCredentialIsInvalid;
		}

		public int CompareTo(AlternateServiceAccountCredential other)
		{
			if (other == null)
			{
				return 1;
			}
			if (this.WhenAdded == null || other.WhenAdded == null)
			{
				return (this.WhenAdded != null).CompareTo(other.WhenAdded != null);
			}
			int num = -this.WhenAdded.Value.CompareTo(other.WhenAdded.Value);
			if (num == 0)
			{
				return this.GetHashCode().CompareTo(other.GetHashCode());
			}
			return num;
		}

		public void Dispose()
		{
			if (this.password != null)
			{
				this.password.Dispose();
				this.password = null;
			}
		}

		public override bool IsValid
		{
			get
			{
				return base.IsValid && this.parseError == null;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return AlternateServiceAccountCredential.schema;
			}
		}

		internal Exception ParseError
		{
			get
			{
				return this.parseError;
			}
		}

		internal SecureString Password
		{
			get
			{
				return this.password;
			}
		}

		internal static AlternateServiceAccountCredential Create(TimeSpan randomizationTimeStampDelta, PSCredential credential)
		{
			if (credential == null)
			{
				throw new ArgumentNullException("credential");
			}
			AlternateServiceAccountConfiguration.EnsureCanDoCryptoOperations();
			string domain;
			string userName;
			AlternateServiceAccountCredential.ParseQualifiedUserName(credential.UserName, out domain, out userName);
			DateTime whenAddedUtc = DateTime.UtcNow + randomizationTimeStampDelta;
			return new AlternateServiceAccountCredential(AlternateServiceAccountCredential.GetRegistryValueName(whenAddedUtc), null, true, whenAddedUtc, domain, userName, credential.Password);
		}

		internal static IEnumerable<AlternateServiceAccountCredential> LoadFromRegistry(RegistryKey rootKey, bool decryptPasswords)
		{
			if (decryptPasswords)
			{
				AlternateServiceAccountConfiguration.EnsureCanDoCryptoOperations();
			}
			return from valueName in rootKey.GetValueNames()
			select AlternateServiceAccountCredential.LoadFromRegistry(rootKey, valueName, decryptPasswords) into credential
			where credential != null
			select credential;
		}

		internal static object WhenAddedGetter(IPropertyBag propertyBag)
		{
			DateTime? dateTime = propertyBag[AlternateServiceAccountCredentialSchema.WhenAddedUTC] as DateTime?;
			if (dateTime == null)
			{
				return null;
			}
			return dateTime.Value.ToLocalTime();
		}

		internal void ApplyPassword(SecureString password)
		{
			if (this.password != null)
			{
				throw new InvalidOperationException("Password has already been set");
			}
			this.password = password;
		}

		internal void Remove(RegistryKey rootKey)
		{
			this.propertyBag.SetIsReadOnly(true);
			rootKey.DeleteValue(this.registryValueName, false);
		}

		internal bool TryAuthenticate(out SecurityStatus authenticationStatus)
		{
			bool result;
			using (AuthenticationContext authenticationContext = new AuthenticationContext())
			{
				authenticationStatus = authenticationContext.LogonUser(this.QualifiedUserName, this.Password);
				result = (authenticationStatus == SecurityStatus.OK);
			}
			return result;
		}

		internal void SaveToRegistry(RegistryKey rootKey)
		{
			if (this.Password == null)
			{
				throw new InvalidOperationException("SaveToRegistry cannot be called on deserialized instances.");
			}
			base.CheckWritable();
			if (rootKey.GetValue(this.registryValueName) != null)
			{
				throw new DataSourceTransientException(DirectoryStrings.FailedToWriteAlternateServiceAccountConfigToRegistry(AlternateServiceAccountCredential.GetRegistryValueDisplayPath(rootKey.Name, this.registryValueName)));
			}
			string value = string.Format("{0}\\{1}\\{2}", this.Domain, this.UserName, Convert.ToBase64String(CapiNativeMethods.DPAPIEncryptData(this.Password, (CapiNativeMethods.DPAPIFlags)0U)));
			rootKey.SetValue(this.registryValueName, value);
			this.propertyBag.SetIsReadOnly(true);
		}

		private static string GetRegistryValueName(DateTime whenAddedUtc)
		{
			return whenAddedUtc.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
		}

		private static string GetRegistryValueDisplayPath(string keyName, string valueName)
		{
			return string.Format("{0}\\@{1}", keyName, valueName);
		}

		private static AlternateServiceAccountCredential LoadFromRegistry(RegistryKey rootKey, string valueName, bool decryptPasswords)
		{
			string text = rootKey.GetValue(valueName) as string;
			if (text == null)
			{
				return null;
			}
			DateTime dateTime;
			Match match;
			if (!DateTime.TryParseExact(valueName, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out dateTime) || !(match = AlternateServiceAccountCredential.regexRegValue.Match(text)).Success)
			{
				return new AlternateServiceAccountCredential(valueName, new DataValidationException(new ObjectValidationError(DirectoryStrings.FailedToParseAlternateServiceAccountCredential, null, AlternateServiceAccountCredential.GetRegistryValueDisplayPath(rootKey.Name, valueName))));
			}
			byte[] array;
			Exception ex;
			try
			{
				array = Convert.FromBase64String(match.Groups["password"].Value);
				ex = null;
			}
			catch (FormatException ex2)
			{
				ex = ex2;
				array = null;
			}
			SecureString secureString = null;
			if (decryptPasswords && array != null)
			{
				try
				{
					secureString = CapiNativeMethods.DPAPIDecryptDataToSecureString(array, (CapiNativeMethods.DPAPIFlags)0U);
					secureString.MakeReadOnly();
				}
				catch (CryptographicException innerException)
				{
					ex = new DataSourceOperationException(DirectoryStrings.FailedToReadAlternateServiceAccountConfigFromRegistry(AlternateServiceAccountCredential.GetRegistryValueDisplayPath(rootKey.Name, valueName)), innerException);
				}
			}
			return new AlternateServiceAccountCredential(valueName, ex, false, dateTime.ToUniversalTime(), match.Groups["domain"].Value, match.Groups["userName"].Value, secureString);
		}

		private static void ParseQualifiedUserName(string qualifiedUserName, out string domain, out string userName)
		{
			Match match = AlternateServiceAccountCredential.regexDomainUser.Match(qualifiedUserName);
			if (match.Success)
			{
				domain = match.Groups["domain"].Value;
				userName = match.Groups["userName"].Value;
				return;
			}
			throw new DataValidationException(new PropertyValidationError(DirectoryStrings.AlternateServiceAccountCredentialQualifiedUserNameWrongFormat, AlternateServiceAccountCredentialSchema.UserName, qualifiedUserName));
		}

		private const string RegistryValueNameFormat = "yyyy-MM-ddTHH:mm:ss.fff";

		private const string QualifiedUserNamePattern = "(?'domain'[^\\\\]*)\\\\(?'userName'[^\\\\]+)";

		private static readonly AlternateServiceAccountCredentialSchema schema = new AlternateServiceAccountCredentialSchema();

		private static readonly Regex regexDomainUser = new Regex("^(?'domain'[^\\\\]*)\\\\(?'userName'[^\\\\]+)$", RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

		private static readonly Regex regexRegValue = new Regex("^(?'domain'[^\\\\]*)\\\\(?'userName'[^\\\\]+)\\\\(?'password'.+)$", RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

		private readonly Exception parseError;

		[NonSerialized]
		private readonly string registryValueName;

		[NonSerialized]
		private SecureString password;

		internal static readonly StringComparer UserNameComparer = StringComparer.OrdinalIgnoreCase;
	}
}
