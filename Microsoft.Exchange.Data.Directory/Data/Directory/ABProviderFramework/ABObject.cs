using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ABObject : ABRawEntry
	{
		protected ABObject(ABSession ownerSession, ABPropertyDefinitionCollection properties) : base(ownerSession, properties)
		{
		}

		public ABObjectId Id
		{
			get
			{
				return (ABObjectId)this[ABObjectSchema.Id];
			}
		}

		public abstract ABObjectSchema Schema { get; }

		public bool CanEmail
		{
			get
			{
				return (bool)this[ABObjectSchema.CanEmail];
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[ABObjectSchema.LegacyExchangeDN];
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[ABObjectSchema.DisplayName];
			}
		}

		public string Alias
		{
			get
			{
				return (string)this[ABObjectSchema.Alias];
			}
		}

		public string EmailAddress
		{
			get
			{
				return (string)this[ABObjectSchema.EmailAddress];
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetType().Name + " = {");
			if (this.Id == null)
			{
				stringBuilder.AppendLine("  Id = <null>");
			}
			else
			{
				stringBuilder.AppendLine("  Id = " + this.Id.ToString());
			}
			if (!string.IsNullOrEmpty(this.DisplayName))
			{
				stringBuilder.AppendLine("  DisplayName = " + this.DisplayName);
			}
			if (!string.IsNullOrEmpty(this.LegacyExchangeDN))
			{
				stringBuilder.AppendLine("  LegacyExchangeDN = " + this.LegacyExchangeDN);
			}
			if (!string.IsNullOrEmpty(this.EmailAddress))
			{
				stringBuilder.AppendLine("  EmailAddress = " + this.EmailAddress);
			}
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}

		protected override bool InternalTryGetValue(ABPropertyDefinition property, out object value)
		{
			if (property == ABObjectSchema.Alias)
			{
				value = this.GetAlias();
				return true;
			}
			if (property == ABObjectSchema.DisplayName)
			{
				value = this.GetDisplayName();
				return true;
			}
			if (property == ABObjectSchema.LegacyExchangeDN)
			{
				value = this.GetLegacyExchangeDN();
				return true;
			}
			if (property == ABObjectSchema.CanEmail)
			{
				value = this.GetCanEmail();
				return true;
			}
			if (property == ABObjectSchema.Id)
			{
				value = this.GetId();
				return true;
			}
			if (property == ABObjectSchema.EmailAddress)
			{
				value = this.GetEmailAddress();
				return true;
			}
			return base.InternalTryGetValue(property, out value);
		}

		protected abstract string GetAlias();

		protected abstract string GetDisplayName();

		protected abstract string GetLegacyExchangeDN();

		protected abstract bool GetCanEmail();

		protected abstract ABObjectId GetId();

		protected abstract string GetEmailAddress();
	}
}
