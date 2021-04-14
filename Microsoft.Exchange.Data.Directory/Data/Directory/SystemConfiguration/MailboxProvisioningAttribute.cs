using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType("MPA")]
	[Serializable]
	public sealed class MailboxProvisioningAttribute : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "K")]
		public string Key { get; set; }

		[XmlAttribute(AttributeName = "V")]
		public string Value { get; set; }

		public override string ToString()
		{
			return string.Format("{0}={1}", this.Key, this.Value);
		}

		public static MailboxProvisioningAttribute Parse(string attribute)
		{
			if (string.IsNullOrWhiteSpace(attribute))
			{
				throw new InvalidMailboxProvisioningAttributeException(DirectoryStrings.ErrorInvalidMailboxProvisioningAttribute(""));
			}
			string[] array = attribute.Split(new char[]
			{
				'='
			});
			if (array.Length != 2)
			{
				throw new InvalidMailboxProvisioningAttributeException(DirectoryStrings.ErrorInvalidMailboxProvisioningAttribute(attribute));
			}
			if (!MailboxProvisioningAttribute.RegexConstraintValidation.IsMatch(array[0]) || !MailboxProvisioningAttribute.RegexConstraintValidation.IsMatch(array[1]))
			{
				throw new InvalidMailboxProvisioningAttributeException(DirectoryStrings.ErrorInvalidMailboxProvisioningAttribute(attribute));
			}
			return new MailboxProvisioningAttribute
			{
				Key = array[0],
				Value = array[1]
			};
		}

		private bool Equals(MailboxProvisioningAttribute other)
		{
			return string.Equals(this.Key, other.Key) && string.Equals(this.Value, other.Value);
		}

		private static readonly Regex RegexConstraintValidation = new Regex("^[a-z0-9]{1,128}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
