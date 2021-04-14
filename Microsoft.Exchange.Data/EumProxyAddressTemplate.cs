using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class EumProxyAddressTemplate : ProxyAddressTemplate
	{
		public EumProxyAddressTemplate(string addressTemplate, bool isPrimaryAddress) : base(ProxyAddressPrefix.UM, addressTemplate, isPrimaryAddress)
		{
			if (!EumProxyAddressTemplate.IsValidEumAddressTemplate(addressTemplate))
			{
				throw new ArgumentOutOfRangeException(DataStrings.InvalidEumAddressTemplateFormat(addressTemplate), null);
			}
		}

		public static bool IsValidEumAddressTemplate(string eumAddressTemplate)
		{
			if (eumAddressTemplate == null)
			{
				throw new ArgumentNullException("eumAddressTemplate");
			}
			int num = eumAddressTemplate.IndexOf("phone-context=");
			bool result = false;
			if (num != -1)
			{
				string value = eumAddressTemplate.Substring(num + "phone-context=".Length + 1);
				result = !string.IsNullOrEmpty(value);
			}
			return result;
		}
	}
}
