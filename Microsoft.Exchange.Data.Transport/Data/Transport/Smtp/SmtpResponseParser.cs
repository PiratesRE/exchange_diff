using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	internal static class SmtpResponseParser
	{
		public static bool Split(string text, out string[] result)
		{
			result = null;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (text.Length < 3)
			{
				return false;
			}
			List<string> lines = SmtpResponseParser.SplitLines(text);
			return SmtpResponseParser.Split(lines, out result);
		}

		public static bool Split(List<string> lines, out string[] result)
		{
			return SmtpResponseParser.ParseResponseArray(lines, out result);
		}

		public static bool IsValidStatusCode(string status)
		{
			return !string.IsNullOrEmpty(status) && status.Length >= 3 && (status.Length <= 3 || status[3] == '-' || status[3] == ' ') && (char.IsDigit(status[0]) && char.IsDigit(status[1])) && char.IsDigit(status[2]);
		}

		private static bool ParseResponseArray(List<string> lines, out string[] result)
		{
			result = null;
			if (lines == null || lines.Count == 0)
			{
				return false;
			}
			if (!SmtpResponseParser.IsValidStatusCode(lines[0]))
			{
				return false;
			}
			string text = lines[0].Substring(0, 3);
			EnhancedStatusCodeImpl enhancedStatusCodeImpl;
			string text2 = EnhancedStatusCodeImpl.TryParse(lines[0], 4, out enhancedStatusCodeImpl) ? enhancedStatusCodeImpl.Value : string.Empty;
			if (!SmtpResponseParser.ValidateLines(lines, text, ref text2))
			{
				return false;
			}
			int num = 4;
			if (!string.IsNullOrEmpty(text2))
			{
				num += text2.Length;
				if (num < lines[0].Length && lines[0][num] == ' ')
				{
					num++;
				}
			}
			List<string> list = new List<string>(lines.Count);
			for (int i = 0; i < lines.Count; i++)
			{
				string text3 = (lines[i].Length > num) ? lines[i].Substring(num) : string.Empty;
				if (!string.IsNullOrEmpty(text3))
				{
					list.Add(text3);
				}
			}
			if (list.Count == 0 && lines.Count > 1)
			{
				return false;
			}
			result = new string[list.Count + 2];
			result[0] = text;
			result[1] = text2;
			list.CopyTo(result, 2);
			return true;
		}

		private static bool ValidateLines(List<string> lines, string statusCode, ref string enhancedStatusCode)
		{
			for (int i = 0; i < lines.Count; i++)
			{
				SmtpResponseParser.ValidationResult validationResult = SmtpResponseParser.ValidateLine(lines[i], statusCode, ref enhancedStatusCode);
				if (validationResult == SmtpResponseParser.ValidationResult.Error)
				{
					return false;
				}
				if (validationResult == SmtpResponseParser.ValidationResult.LastLine && i != lines.Count - 1)
				{
					return false;
				}
				if (validationResult == SmtpResponseParser.ValidationResult.HasMoreLines && i == lines.Count - 1)
				{
					return false;
				}
			}
			return true;
		}

		private static SmtpResponseParser.ValidationResult ValidateLine(string line, string statusCode, ref string enhancedStatusCode)
		{
			bool flag = false;
			if (line.Length < 3)
			{
				return SmtpResponseParser.ValidationResult.Error;
			}
			if (string.CompareOrdinal(statusCode, 0, line, 0, 3) != 0)
			{
				return SmtpResponseParser.ValidationResult.Error;
			}
			if (line.Length == 3)
			{
				enhancedStatusCode = string.Empty;
				return SmtpResponseParser.ValidationResult.LastLine;
			}
			if (line[3] == ' ')
			{
				flag = true;
			}
			else if (line[3] != '-')
			{
				return SmtpResponseParser.ValidationResult.Error;
			}
			int num = 0;
			if (!string.IsNullOrEmpty(enhancedStatusCode))
			{
				EnhancedStatusCodeImpl enhancedStatusCodeImpl;
				string text = EnhancedStatusCodeImpl.TryParse(line, 4, out enhancedStatusCodeImpl) ? enhancedStatusCodeImpl.Value : null;
				if (string.IsNullOrEmpty(text) || !text.Equals(enhancedStatusCode, StringComparison.Ordinal))
				{
					enhancedStatusCode = string.Empty;
				}
				num = enhancedStatusCode.Length;
			}
			for (int i = 4 + num; i < line.Length; i++)
			{
				if (line[i] < '\u0001' || line[i] > '\u007f')
				{
					return SmtpResponseParser.ValidationResult.Error;
				}
			}
			if (!flag)
			{
				return SmtpResponseParser.ValidationResult.HasMoreLines;
			}
			return SmtpResponseParser.ValidationResult.LastLine;
		}

		private static List<string> SplitLines(string response)
		{
			List<string> list = new List<string>();
			int num = 0;
			int num2;
			while ((num2 = response.IndexOf("\r\n", num, StringComparison.Ordinal)) != -1)
			{
				list.Add(response.Substring(num, num2 - num));
				num = num2 + "\r\n".Length;
				if (num >= response.Length)
				{
					break;
				}
			}
			if (num < response.Length)
			{
				list.Add(response.Substring(num));
			}
			return list;
		}

		private const string CRLF = "\r\n";

		private enum ValidationResult
		{
			Error,
			LastLine,
			HasMoreLines
		}
	}
}
