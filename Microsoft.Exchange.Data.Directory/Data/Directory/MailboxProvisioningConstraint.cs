using System;
using System.Management.Automation;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	public class MailboxProvisioningConstraint : XMLSerializableBase, IMailboxProvisioningConstraint
	{
		public MailboxProvisioningConstraint()
		{
		}

		public MailboxProvisioningConstraint(string value)
		{
			this.Value = value;
		}

		[XmlText]
		public string Value
		{
			get
			{
				return this.value ?? string.Empty;
			}
			set
			{
				this.value = value;
				this.filter = null;
			}
		}

		[XmlIgnore]
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.value);
			}
		}

		public static object ConvertValueFromString(object valueToConvert, Type resultType)
		{
			string text = valueToConvert as string;
			bool flag;
			if (resultType == typeof(bool) && bool.TryParse(text, out flag))
			{
				return flag;
			}
			if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				bool flag2 = text == null || "null".Equals(text, StringComparison.OrdinalIgnoreCase) || "$null".Equals(text, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					return null;
				}
			}
			return LanguagePrimitives.ConvertTo(text, resultType);
		}

		public bool IsMatch(MailboxProvisioningAttributes attributes)
		{
			if (!string.IsNullOrEmpty(this.Value) && this.filter == null)
			{
				this.filter = this.ParseFilter();
			}
			return string.IsNullOrEmpty(this.Value) || OpathFilterEvaluator.FilterMatches(this.filter, attributes.PropertyBag);
		}

		public override string ToString()
		{
			return this.Value;
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj is MailboxProvisioningConstraint && this.Equals((MailboxProvisioningConstraint)obj)));
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public bool TryValidate(out InvalidMailboxProvisioningConstraintException validationException)
		{
			validationException = null;
			try
			{
				this.ParseFilter();
			}
			catch (InvalidMailboxProvisioningConstraintException ex)
			{
				validationException = ex;
			}
			return validationException == null;
		}

		private QueryFilter ParseFilter()
		{
			ObjectSchema instance = ObjectSchema.GetInstance<MailboxProvisioningAttributesSchema>();
			Exception ex = null;
			QueryFilter result = null;
			try
			{
				QueryParser queryParser = new QueryParser(this.Value, instance, QueryParser.Capabilities.All, null, new QueryParser.ConvertValueFromStringDelegate(MailboxProvisioningConstraint.ConvertValueFromString));
				result = queryParser.ParseTree;
			}
			catch (ParsingNonFilterablePropertyException ex2)
			{
				ex = ex2;
			}
			catch (ParsingException ex3)
			{
				ex = ex3;
			}
			catch (ArgumentOutOfRangeException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				throw new InvalidMailboxProvisioningConstraintException(ex.Message, ex);
			}
			return result;
		}

		private bool Equals(MailboxProvisioningConstraint other)
		{
			return string.Equals(this.Value, other.Value);
		}

		private string value;

		private QueryFilter filter;
	}
}
