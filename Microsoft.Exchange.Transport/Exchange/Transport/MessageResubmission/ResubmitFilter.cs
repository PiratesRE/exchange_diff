using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Transport.MessageResubmission
{
	internal class ResubmitFilter
	{
		internal bool FromAddressChecking
		{
			get
			{
				return !string.IsNullOrEmpty(this.fromAddress);
			}
		}

		internal bool ToAddressChecking
		{
			get
			{
				return !string.IsNullOrEmpty(this.toAddress);
			}
		}

		private ResubmitFilter(string fromAddress, string toAddress)
		{
			this.fromAddress = fromAddress;
			this.toAddress = toAddress;
		}

		internal bool ValidateStringParam(ResubmitFilter.FilterParameterType paramType, string value)
		{
			string value2;
			switch (paramType)
			{
			case ResubmitFilter.FilterParameterType.ToAddress:
				value2 = this.toAddress;
				break;
			case ResubmitFilter.FilterParameterType.FromAddress:
				value2 = this.fromAddress;
				break;
			default:
				throw new ArgumentException("Inproper parameter type pssed to ResubmitFilter.ValidateStringParam", paramType.ToString());
			}
			return string.IsNullOrEmpty(value2) || value.Equals(value2, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool TryBuild(string conditionstring, out ResubmitFilter generatedFilter)
		{
			generatedFilter = null;
			Dictionary<string, string> dictionary;
			try
			{
				dictionary = (from part in conditionstring.Trim().Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries)
				select part.Split(new char[]
				{
					'='
				})).ToDictionary((string[] split) => split[0].ToLower().Trim(), (string[] split) => split[1].Trim());
			}
			catch
			{
				return false;
			}
			string text;
			dictionary.TryGetValue("fromaddress", out text);
			string text2;
			dictionary.TryGetValue("toaddress", out text2);
			generatedFilter = new ResubmitFilter(text, text2);
			return true;
		}

		private readonly string fromAddress;

		private readonly string toAddress;

		internal enum FilterParameterType : byte
		{
			ToAddress,
			FromAddress
		}
	}
}
