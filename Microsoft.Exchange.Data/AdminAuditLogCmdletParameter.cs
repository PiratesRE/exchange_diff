using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class AdminAuditLogCmdletParameter
	{
		private AdminAuditLogCmdletParameter()
		{
		}

		public string Name { get; private set; }

		public string Value { get; private set; }

		public static AdminAuditLogCmdletParameter Parse(string propertyValue)
		{
			if (propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue");
			}
			AdminAuditLogCmdletParameter adminAuditLogCmdletParameter = new AdminAuditLogCmdletParameter();
			int num = propertyValue.IndexOf('=');
			if (num > 0)
			{
				adminAuditLogCmdletParameter.Name = propertyValue.Substring(0, num).Trim();
				adminAuditLogCmdletParameter.Value = propertyValue.Substring(num + 1).Trim();
				return adminAuditLogCmdletParameter;
			}
			throw new ArgumentException(DataStrings.AdminAuditLogInvalidParameterOrModifiedProperty(propertyValue));
		}

		public override int GetHashCode()
		{
			if (this.Name != null)
			{
				return this.Name.ToUpperInvariant().GetHashCode();
			}
			return string.Empty.GetHashCode();
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}
