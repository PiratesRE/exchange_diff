using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	public abstract class DDIValidateAttribute : DDIAttribute, IDDIValidator
	{
		public DDIValidateAttribute(string description) : base(description)
		{
		}

		public virtual List<string> Validate(object target, Service profile)
		{
			return new List<string>();
		}

		public const string ArgumentKey_CodeBehind = "CodeBehind";

		public const string ArgumentKey_Xaml = "Xaml";

		public const string ArgumentKey_SchemaName = "SchemaName";
	}
}
