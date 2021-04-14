using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class UserConfigurationPropertyDefinition
	{
		internal UserConfigurationPropertyDefinition(string name, Type type, UserConfigurationPropertyDefinition.Validate validate, Guid guid)
		{
			this.name = name;
			this.type = type;
			this.guid = guid;
			this.validate = validate;
			this.hashCode = (this.guid.GetHashCode() ^ this.name.ToLowerInvariant().GetHashCode());
		}

		internal UserConfigurationPropertyDefinition(string name, Type type, UserConfigurationPropertyDefinition.Validate validate) : this(name, type, validate, UserConfigurationPropertyDefinition.publicStringsGuid)
		{
		}

		internal UserConfigurationPropertyDefinition(string name, Type type) : this(name, type, new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyDefinition.DefaultValidateCallback), UserConfigurationPropertyDefinition.publicStringsGuid)
		{
		}

		internal UserConfigurationPropertyDefinition.Validate GetValidatedProperty
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
			UserConfigurationPropertyDefinition userConfigurationPropertyDefinition = value as UserConfigurationPropertyDefinition;
			return userConfigurationPropertyDefinition != null && (string.Equals(this.name, userConfigurationPropertyDefinition.name, StringComparison.OrdinalIgnoreCase) && this.guid.Equals(userConfigurationPropertyDefinition.guid)) && this.type.Equals(userConfigurationPropertyDefinition.type);
		}

		private static object DefaultValidateCallback(object value)
		{
			return value;
		}

		private static readonly Guid publicStringsGuid = new Guid("00020329-0000-0000-C000-000000000046");

		private readonly int hashCode;

		private readonly string name;

		private readonly Guid guid;

		private UserConfigurationPropertyDefinition.Validate validate;

		private Type type;

		internal delegate object Validate(object originalValue);
	}
}
