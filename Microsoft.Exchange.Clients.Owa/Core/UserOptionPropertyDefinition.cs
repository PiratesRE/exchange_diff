using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class UserOptionPropertyDefinition
	{
		internal UserOptionPropertyDefinition(string name, Type type, UserOptionPropertyDefinition.Validate validate, Guid guid)
		{
			this.name = name;
			this.type = type;
			this.guid = guid;
			this.validate = validate;
			this.hashCode = (this.guid.GetHashCode() ^ this.name.GetHashCode());
		}

		internal UserOptionPropertyDefinition(string name, Type type, UserOptionPropertyDefinition.Validate validate) : this(name, type, validate, UserOptionPropertyDefinition.publicStringsGuid)
		{
		}

		internal UserOptionPropertyDefinition(string name, Type type) : this(name, type, new UserOptionPropertyDefinition.Validate(UserOptionPropertyDefinition.DefaultValidateCallback), UserOptionPropertyDefinition.publicStringsGuid)
		{
		}

		internal UserOptionPropertyDefinition.Validate GetValidatedProperty
		{
			get
			{
				return this.validate;
			}
		}

		internal string PropertyName
		{
			get
			{
				return this.name;
			}
		}

		internal Type PropertyType
		{
			get
			{
				return this.type;
			}
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public override bool Equals(object value)
		{
			UserOptionPropertyDefinition userOptionPropertyDefinition = value as UserOptionPropertyDefinition;
			return userOptionPropertyDefinition != null && (string.Equals(this.name, userOptionPropertyDefinition.name, StringComparison.OrdinalIgnoreCase) && this.guid.Equals(userOptionPropertyDefinition.guid)) && this.type.Equals(userOptionPropertyDefinition.type);
		}

		private static object DefaultValidateCallback(object value)
		{
			return value;
		}

		private static readonly Guid publicStringsGuid = new Guid("00020329-0000-0000-C000-000000000046");

		private UserOptionPropertyDefinition.Validate validate;

		private int hashCode;

		private string name;

		private Type type;

		private Guid guid;

		internal delegate object Validate(object originalValue);
	}
}
