using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	public class LocatorServiceValidator
	{
		public static string GetDiagnosticInfo(ResponseBase response)
		{
			if (response != null)
			{
				return response.TransactionID + ";" + response.Diagnostics;
			}
			return null;
		}

		public static bool IsExpectedResponse(FindTenantResponse expected, FindTenantResponse actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "FindTenantResponse", out message, () => LocatorServiceValidator.IsExpectedResponse(expected.TenantInfo, actual.TenantInfo, out message));
			validationMessage = LocatorServiceValidator.AddDiagnosticInfo(actual, message);
			return result;
		}

		public static bool IsExpectedResponse(FindDomainResponse expected, FindDomainResponse actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "FindDomainResponse", out message, () => LocatorServiceValidator.IsExpectedResponse(expected.DomainInfo, actual.DomainInfo, out message) && LocatorServiceValidator.IsExpectedResponse(expected.TenantInfo, actual.TenantInfo, out message));
			validationMessage = LocatorServiceValidator.AddDiagnosticInfo(actual, message);
			return result;
		}

		public static bool IsExpectedResponse(FindDomainsResponse expected, FindDomainsResponse actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "FindDomainsResponse", out message, () => LocatorServiceValidator.IsExpectedResponse(expected.DomainsResponse, actual.DomainsResponse, out message));
			validationMessage = LocatorServiceValidator.AddDiagnosticInfo(actual.DomainsResponse.FirstOrDefault<FindDomainResponse>(), message);
			return result;
		}

		public static bool IsExpectedResponse(FindUserResponse expected, FindUserResponse actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "FindUserResponse", out message, () => LocatorServiceValidator.IsExpectedResponse(expected.UserInfo, actual.UserInfo, out message) && LocatorServiceValidator.IsExpectedResponse(expected.TenantInfo, actual.TenantInfo, out message));
			validationMessage = LocatorServiceValidator.AddDiagnosticInfo(actual, message);
			return result;
		}

		private static bool IsExpectedResponse(FindDomainResponse[] expected, FindDomainResponse[] actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "FindDomainResponse[]", out message, delegate
			{
				FindDomainResponse[] array = (from domain in expected
				where domain.DomainInfo != null
				select domain).OrderBy((FindDomainResponse domain) => domain.DomainInfo.DomainName, StringComparer.OrdinalIgnoreCase).ToArray<FindDomainResponse>();
				FindDomainResponse[] array2 = (from domain in actual
				where domain.DomainInfo != null
				select domain).OrderBy((FindDomainResponse domain) => domain.DomainInfo.DomainName, StringComparer.OrdinalIgnoreCase).ToArray<FindDomainResponse>();
				if (array.Length != array2.Length)
				{
					message = string.Format("Unexpected domains returned, expected:'{0}', actual:'{1}'", string.Join(",", from e in expected
					select e.DomainInfo.DomainName), string.Join(",", from a in actual
					select a.DomainInfo.DomainName));
					return false;
				}
				for (int i = 0; i < array2.Length; i++)
				{
					if (!LocatorServiceValidator.IsExpectedResponse(array[i], array2[i], out message))
					{
						return false;
					}
				}
				return true;
			});
			validationMessage = message;
			return result;
		}

		private static bool ValidateWithNullCheck(object expected, object actual, string objectName, out string validationMessage, Func<bool> function)
		{
			validationMessage = null;
			if (expected != null && actual != null)
			{
				return function();
			}
			if (expected == null && actual == null)
			{
				return true;
			}
			validationMessage = string.Format("{0} is {1}, and expected value is {2}", objectName, (actual == null) ? "null" : "non-null", (expected == null) ? "null" : "non-null");
			return false;
		}

		private static bool IsExpectedResponse(UserInfo expected, UserInfo actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "UserInfo", out message, delegate
			{
				if (!expected.UserKey.Equals(actual.UserKey, StringComparison.OrdinalIgnoreCase))
				{
					message = string.Format("Unexpected UserKey, expected:{0}, actual:{1}", expected.UserKey, actual.UserKey);
					return false;
				}
				if (!expected.MSAUserName.Equals(actual.MSAUserName, StringComparison.OrdinalIgnoreCase))
				{
					message = string.Format("Unexpected MSAUserName, expected:{0}, actual:{1}", expected.MSAUserName, actual.MSAUserName);
					return false;
				}
				return true;
			});
			validationMessage = message;
			return result;
		}

		private static bool IsExpectedResponse(DomainInfo expected, DomainInfo actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "DomainInfo", out message, delegate
			{
				if (!expected.DomainKey.Equals(actual.DomainKey, StringComparison.OrdinalIgnoreCase))
				{
					message = string.Format("Unexpected DomainKey, expected:{0}, actual:{1}", expected.DomainKey, actual.DomainKey);
					return false;
				}
				if (!expected.DomainName.Equals(actual.DomainName, StringComparison.OrdinalIgnoreCase))
				{
					message = string.Format("Unexpected DomainName, expected:{0}, actual:{1}", expected.DomainName, actual.DomainName);
					return false;
				}
				if (!LocatorServiceValidator.IsExpectedResponse(expected.NoneExistNamespaces, actual.NoneExistNamespaces, out message))
				{
					message = string.Format("Non-Existing namespaces check failed for Domain '{0}': {1}", actual.DomainName, message);
					return false;
				}
				if (!LocatorServiceValidator.IsExpectedResponse(expected.Properties, actual.Properties, out message))
				{
					message = string.Format("Properties check failed for Domain '{0}': {1}", actual.DomainName, message);
					return false;
				}
				return true;
			});
			validationMessage = message;
			return result;
		}

		private static bool IsExpectedResponse(TenantInfo expected, TenantInfo actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "TenantInfo", out message, delegate
			{
				if (expected.TenantId != actual.TenantId)
				{
					message = string.Format("Unexpected TenantId, expected:{0}, actual:{1}", expected.TenantId, actual.TenantId);
					return false;
				}
				if (!LocatorServiceValidator.IsExpectedResponse(expected.NoneExistNamespaces, actual.NoneExistNamespaces, out message))
				{
					message = string.Format("Non-Existing namespaces check failed for Tenant '{0}': {1}", actual.TenantId, message);
					return false;
				}
				if (!LocatorServiceValidator.IsExpectedResponse(expected.Properties, actual.Properties, out message))
				{
					message = string.Format("Properties check failed for Tenant '{0}': {1}", actual.TenantId, message);
					return false;
				}
				return true;
			});
			validationMessage = message;
			return result;
		}

		private static bool IsExpectedResponse(KeyValuePair<string, string>[] expected, KeyValuePair<string, string>[] actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "Properties", out message, delegate
			{
				if (expected.Length != actual.Length)
				{
					message = string.Format("Unexpected properties returned, expected:'{0}', actual:'{1}'", string.Join(",", from e in expected
					select e.Key), string.Join(",", from a in actual
					select a.Key));
					return false;
				}
				bool flag = false;
				KeyValuePair<string, string>[] array = expected.OrderBy((KeyValuePair<string, string> value) => value.Key, StringComparer.OrdinalIgnoreCase).ToArray<KeyValuePair<string, string>>();
				KeyValuePair<string, string>[] array2 = actual.OrderBy((KeyValuePair<string, string> value) => value.Key, StringComparer.OrdinalIgnoreCase).ToArray<KeyValuePair<string, string>>();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Unexpected property values returned:");
				for (int i = 0; i < actual.Length; i++)
				{
					if (!string.Equals(array[i].Key, array2[i].Key, StringComparison.OrdinalIgnoreCase) || !string.Equals(array[i].Value, array2[i].Value, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						stringBuilder.AppendFormat("'{0}', expected:{1}, actual:{2}\n", array[i].Key, array[i].Value, array2[i].Value);
					}
				}
				if (flag)
				{
					message = stringBuilder.ToString();
					return false;
				}
				return true;
			});
			validationMessage = message;
			return result;
		}

		private static bool IsExpectedResponse(string[] expected, string[] actual, out string validationMessage)
		{
			string message = null;
			bool result = LocatorServiceValidator.ValidateWithNullCheck(expected, actual, "string[]", out message, delegate
			{
				bool flag = false;
				if (expected.Length == actual.Length)
				{
					IEnumerable<string> first = expected.OrderBy((string value) => value, StringComparer.OrdinalIgnoreCase);
					IEnumerable<string> second = actual.OrderBy((string value) => value, StringComparer.OrdinalIgnoreCase);
					flag = first.SequenceEqual(second, StringComparer.OrdinalIgnoreCase);
				}
				if (flag)
				{
					return true;
				}
				message = string.Format("Unexpected values returned, expected:'{0}', actual:'{1}'", string.Join(",", expected), string.Join(",", actual));
				return false;
			});
			validationMessage = message;
			return result;
		}

		private static string AddDiagnosticInfo(ResponseBase response, string message)
		{
			string diagnosticInfo = LocatorServiceValidator.GetDiagnosticInfo(response);
			if (!string.IsNullOrWhiteSpace(diagnosticInfo))
			{
				return message + Environment.NewLine + diagnosticInfo;
			}
			return message;
		}
	}
}
