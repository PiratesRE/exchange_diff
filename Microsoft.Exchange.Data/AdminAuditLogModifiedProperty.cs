using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class AdminAuditLogModifiedProperty
	{
		private AdminAuditLogModifiedProperty()
		{
		}

		public string Name { get; private set; }

		public string NewValue { get; internal set; }

		public string OldValue { get; internal set; }

		public static AdminAuditLogModifiedProperty Parse(string propertyValue, bool newValue)
		{
			if (propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue");
			}
			AdminAuditLogModifiedProperty adminAuditLogModifiedProperty = new AdminAuditLogModifiedProperty();
			int num = propertyValue.IndexOf('=');
			if (num > 0)
			{
				adminAuditLogModifiedProperty.Name = propertyValue.Substring(0, num).Trim();
				if (newValue)
				{
					adminAuditLogModifiedProperty.NewValue = propertyValue.Substring(num + 1).Trim();
				}
				else
				{
					adminAuditLogModifiedProperty.OldValue = propertyValue.Substring(num + 1).Trim();
				}
				return adminAuditLogModifiedProperty;
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
