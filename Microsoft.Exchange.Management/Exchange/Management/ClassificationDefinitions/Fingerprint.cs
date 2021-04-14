using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	public sealed class Fingerprint
	{
		public string Value { get; private set; }

		public uint ShingleCount { get; private set; }

		public string Description { get; private set; }

		public Fingerprint(string fingerprintValue, uint shingleCount, string description)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("fingerprintValue", fingerprintValue);
			ArgumentValidator.ThrowIfInvalidValue<uint>("shingleCount", shingleCount, (uint count) => count > 0U);
			try
			{
				Convert.FromBase64String(fingerprintValue);
			}
			catch (FormatException innerException)
			{
				throw new ErrorInvalidFingerprintException(fingerprintValue, innerException);
			}
			this.Value = fingerprintValue;
			this.ShingleCount = shingleCount;
			this.Description = description;
			this.ActualDescription = description;
		}

		public static Fingerprint Parse(string input)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("input", input);
			string s = string.Empty;
			string description = string.Empty;
			string text = string.Empty;
			int num = input.LastIndexOf(":", input.Length - 1);
			if (num >= 0)
			{
				text = input.Substring(num + 1);
				int num2 = input.LastIndexOf(":", num - 1);
				if (num2 >= 0)
				{
					s = input.Substring(num2 + 1, num - num2 - 1);
					description = input.Substring(0, num2);
				}
				else
				{
					s = input.Substring(0, num);
				}
				uint num3;
				if (!string.IsNullOrEmpty(text) && uint.TryParse(s, out num3) && num3 > 0U)
				{
					return new Fingerprint(text, num3, description);
				}
			}
			throw new ErrorInvalidFingerprintException(input);
		}

		public override string ToString()
		{
			return string.Join(":", new object[]
			{
				this.Description,
				this.ShingleCount,
				this.Value
			});
		}

		internal XElement ToXElement()
		{
			XElement xelement = new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Fingerprint"), new object[]
			{
				new XAttribute("id", this.Identity),
				new XAttribute("shingleCount", this.ShingleCount),
				new XAttribute("threshold", 50),
				this.Value
			});
			xelement.SetAttributeValue("description", this.Description);
			return xelement;
		}

		internal static Fingerprint FromXElement(XElement element)
		{
			ArgumentValidator.ThrowIfNull("element", element);
			if (!"Fingerprint".Equals(element.Name.LocalName, StringComparison.Ordinal))
			{
				throw new ErrorInvalidFingerprintException(element.ToString());
			}
			string attributeValue = XmlProcessingUtils.GetAttributeValue(element, "id");
			uint num = 0U;
			uint.TryParse(XmlProcessingUtils.GetAttributeValue(element, "shingleCount"), out num);
			string value = element.Value;
			string attributeValue2 = XmlProcessingUtils.GetAttributeValue(element, "description");
			if (num <= 0U || string.IsNullOrEmpty(attributeValue) || string.IsNullOrEmpty(value))
			{
				throw new ErrorInvalidFingerprintException(element.ToString());
			}
			return new Fingerprint(value, num, attributeValue2)
			{
				Identity = attributeValue
			};
		}

		private const string Separator = ":";

		internal static readonly Fingerprint.FingerprintComparer Comparer = new Fingerprint.FingerprintComparer();

		[NonSerialized]
		internal string ActualDescription;

		[NonSerialized]
		internal string Identity;

		internal sealed class FingerprintComparer : EqualityComparer<Fingerprint>
		{
			public override bool Equals(Fingerprint left, Fingerprint right)
			{
				if (!object.ReferenceEquals(left, right))
				{
					if (left == null || right == null)
					{
						return false;
					}
					if (left.ShingleCount != right.ShingleCount)
					{
						return false;
					}
					if (!string.Equals(left.Value, right.Value, StringComparison.Ordinal))
					{
						return false;
					}
				}
				return true;
			}

			public override int GetHashCode(Fingerprint fingerprint)
			{
				if (fingerprint != null)
				{
					return string.Join(":", new object[]
					{
						fingerprint.ShingleCount,
						fingerprint.Value
					}).GetHashCode();
				}
				return 0;
			}
		}
	}
}
