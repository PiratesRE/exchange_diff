using System;

namespace Microsoft.Exchange.LogUploader
{
	public class AuditUploaderDictionaryKey : IEquatable<AuditUploaderDictionaryKey>
	{
		public AuditUploaderDictionaryKey(string component, string tenant, string user, string operation)
		{
			this.Component = ((component == null) ? AuditUploaderDictionaryKey.WildCard : component.ToLower());
			this.Tenant = ((tenant == null) ? AuditUploaderDictionaryKey.WildCard : tenant.ToLower());
			this.User = ((user == null) ? AuditUploaderDictionaryKey.WildCard : user.ToLower());
			this.Operation = ((operation == null) ? AuditUploaderDictionaryKey.WildCard : operation.ToLower());
		}

		public string Component { get; set; }

		public string Tenant { get; set; }

		public string User { get; set; }

		public string Operation { get; set; }

		public string this[int i]
		{
			set
			{
				switch (i)
				{
				case 0:
					this.Component = value;
					return;
				case 1:
					this.Tenant = value;
					return;
				case 2:
					this.User = value;
					return;
				case 3:
					this.Operation = value;
					return;
				default:
					return;
				}
			}
		}

		public override int GetHashCode()
		{
			return this.Component.GetHashCode() ^ this.Tenant.GetHashCode() ^ this.User.GetHashCode() ^ this.Operation.GetHashCode();
		}

		public bool Equals(AuditUploaderDictionaryKey key)
		{
			return key.Component.Equals(this.Component, StringComparison.OrdinalIgnoreCase) && key.Tenant.Equals(this.Tenant, StringComparison.OrdinalIgnoreCase) && key.User.Equals(this.User, StringComparison.OrdinalIgnoreCase) && key.Operation.Equals(this.Operation, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object key)
		{
			return this.Equals(key as AuditUploaderDictionaryKey);
		}

		public void CopyFrom(AuditUploaderDictionaryKey source)
		{
			this.Component = source.Component;
			this.Tenant = source.Tenant;
			this.User = source.User;
			this.Operation = source.Operation;
		}

		public static readonly int NumberOfFields = 4;

		public static readonly string WildCard = "*";
	}
}
