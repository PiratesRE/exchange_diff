using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class ValidationError : ProviderError
	{
		public static ValidationError[] None
		{
			get
			{
				return new ValidationError[0];
			}
		}

		public ValidationError(LocalizedString description)
		{
			this.description = description;
		}

		public ValidationError(LocalizedString description, string propertyName)
		{
			this.description = description;
			this.propertyName = propertyName;
		}

		public ValidationError(LocalizedString description, PropertyDefinition propertyDefinition)
		{
			this.description = description;
			if (propertyDefinition != null)
			{
				this.propertyName = propertyDefinition.Name;
			}
		}

		public LocalizedString Description
		{
			get
			{
				return this.description;
			}
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public bool Equals(ValidationError other)
		{
			return other != null && string.Equals(this.Description, other.Description, StringComparison.OrdinalIgnoreCase) && string.Equals(this.PropertyName, other.PropertyName, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ValidationError);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				this.hashCode = ((this.Description ?? string.Empty).ToLowerInvariant().GetHashCode() ^ (this.PropertyName ?? string.Empty).ToLowerInvariant().GetHashCode());
			}
			return this.hashCode;
		}

		public static LocalizedString CombineErrorDescriptions(List<ValidationError> errors)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < errors.Count; i++)
			{
				if (i == errors.Count - 1)
				{
					stringBuilder.Append(errors[i].Description);
				}
				else
				{
					stringBuilder.AppendFormat("{0}\r\n", errors[i].Description);
				}
			}
			return new LocalizedString(stringBuilder.ToString());
		}

		private readonly string propertyName;

		private LocalizedString description;

		private int hashCode;
	}
}
