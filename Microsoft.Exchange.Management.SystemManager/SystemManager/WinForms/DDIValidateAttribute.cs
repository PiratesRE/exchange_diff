using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class DDIValidateAttribute : DDIAttribute, IDDIValidator
	{
		public DDIValidateAttribute(string description) : base(description)
		{
		}

		public virtual List<string> Validate(object target, PageConfigurableProfile profile)
		{
			return new List<string>();
		}
	}
}
