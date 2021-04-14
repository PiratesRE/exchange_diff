using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class LdapPolicy
	{
		internal LdapPolicy(string policyName, int value)
		{
			if (string.IsNullOrEmpty(policyName))
			{
				throw new ArgumentNullException("policyName");
			}
			this.PolicyName = policyName;
			this.Value = value;
		}

		public string PolicyName { get; private set; }

		public int Value { get; private set; }

		public static LdapPolicy Parse(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException("input");
			}
			string[] array = input.Split(LdapPolicy.PolicySeparatorArray, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 2)
			{
				throw new FormatException(DataStrings.InvalidFormat);
			}
			return new LdapPolicy(array[0].Trim(), int.Parse(array[1]));
		}

		public string ToADString()
		{
			return this.PolicyName + "=" + this.Value;
		}

		public override string ToString()
		{
			return this.ToADString();
		}

		private const string PolicySeparator = "=";

		private static readonly string[] PolicySeparatorArray = new string[]
		{
			"="
		};
	}
}
