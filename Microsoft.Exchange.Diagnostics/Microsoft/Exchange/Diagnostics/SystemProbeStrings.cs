using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class SystemProbeStrings
	{
		static SystemProbeStrings()
		{
			SystemProbeStrings.stringIDs.Add(3063416394U, "InvalidGuidInDecryptedText");
			SystemProbeStrings.stringIDs.Add(1270928367U, "EncryptedDataNotValidBase64String");
			SystemProbeStrings.stringIDs.Add(1976115009U, "NullEncryptedData");
			SystemProbeStrings.stringIDs.Add(3719046686U, "CertificateNotFound");
			SystemProbeStrings.stringIDs.Add(2483075962U, "InvalidTimeInDecryptedText");
			SystemProbeStrings.stringIDs.Add(4144305424U, "EncryptedDataCannotBeDecrypted");
			SystemProbeStrings.stringIDs.Add(4289353018U, "CertificateNotSigned");
		}

		public static LocalizedString InvalidGuidInDecryptedText
		{
			get
			{
				return new LocalizedString("InvalidGuidInDecryptedText", SystemProbeStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EncryptedDataNotValidBase64String
		{
			get
			{
				return new LocalizedString("EncryptedDataNotValidBase64String", SystemProbeStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullEncryptedData
		{
			get
			{
				return new LocalizedString("NullEncryptedData", SystemProbeStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CertificateTimeNotValid(string start, string end)
		{
			return new LocalizedString("CertificateTimeNotValid", SystemProbeStrings.ResourceManager, new object[]
			{
				start,
				end
			});
		}

		public static LocalizedString CertificateNotFound
		{
			get
			{
				return new LocalizedString("CertificateNotFound", SystemProbeStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTimeInDecryptedText
		{
			get
			{
				return new LocalizedString("InvalidTimeInDecryptedText", SystemProbeStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EncryptedDataCannotBeDecrypted
		{
			get
			{
				return new LocalizedString("EncryptedDataCannotBeDecrypted", SystemProbeStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CertificateNotSigned
		{
			get
			{
				return new LocalizedString("CertificateNotSigned", SystemProbeStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(SystemProbeStrings.IDs key)
		{
			return new LocalizedString(SystemProbeStrings.stringIDs[(uint)key], SystemProbeStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(7);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Diagnostics.SystemProbeStrings", typeof(SystemProbeStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidGuidInDecryptedText = 3063416394U,
			EncryptedDataNotValidBase64String = 1270928367U,
			NullEncryptedData = 1976115009U,
			CertificateNotFound = 3719046686U,
			InvalidTimeInDecryptedText = 2483075962U,
			EncryptedDataCannotBeDecrypted = 4144305424U,
			CertificateNotSigned = 4289353018U
		}

		private enum ParamIDs
		{
			CertificateTimeNotValid
		}
	}
}
