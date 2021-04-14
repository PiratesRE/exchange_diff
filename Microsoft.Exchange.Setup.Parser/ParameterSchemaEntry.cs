using System;

namespace Microsoft.Exchange.Setup.Parser
{
	internal class ParameterSchemaEntry
	{
		public ParameterSchemaEntry(string name) : this(name, ParameterType.MustHaveValue)
		{
		}

		public ParameterSchemaEntry(string name, ParameterType parameterType) : this(name, parameterType, SetupOperations.AllSetupOperations)
		{
		}

		public ParameterSchemaEntry(string name, ParameterType parameterType, SetupOperations validOperations) : this(name, parameterType, validOperations, SetupRoles.AllRoles)
		{
		}

		public ParameterSchemaEntry(string name, ParameterType parameterType, SetupOperations validOperations, SetupRoles validRoles) : this(name, parameterType, validOperations, validRoles, new ParseMethod(ParameterSchemaEntry.DefaultParser))
		{
		}

		public ParameterSchemaEntry(string name, ParameterType parameterType, SetupOperations validOperations, SetupRoles validRoles, ParseMethod parseMethod)
		{
			this.name = name;
			this.parameterType = parameterType;
			this.validOperations = validOperations;
			this.validRoles = validRoles;
			this.parseMethod = parseMethod;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public ParameterType ParameterType
		{
			get
			{
				return this.parameterType;
			}
		}

		public SetupOperations ValidOperations
		{
			get
			{
				return this.validOperations;
			}
		}

		public SetupRoles ValidRoles
		{
			get
			{
				return this.validRoles;
			}
		}

		public ParseMethod ParseMethod
		{
			get
			{
				return this.parseMethod;
			}
		}

		public static object DefaultParser(string s)
		{
			return s;
		}

		private readonly string name;

		private readonly ParameterType parameterType;

		private readonly SetupOperations validOperations;

		private readonly SetupRoles validRoles;

		private readonly ParseMethod parseMethod;
	}
}
