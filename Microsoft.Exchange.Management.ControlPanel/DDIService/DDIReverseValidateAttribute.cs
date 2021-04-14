using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DDIReverseValidateAttribute : DDIDecoratorAttribute
	{
		public DDIReverseValidateAttribute() : base("DDIReverseValidateAttribute")
		{
		}

		public string ErrorMessage { get; set; }

		public override List<string> Validate(object target, Service profile)
		{
			List<string> result = new List<string>();
			if (target == null || string.IsNullOrEmpty(target.ToString()))
			{
				return result;
			}
			DDIValidateAttribute ddiattribute = base.GetDDIAttribute();
			if (ddiattribute == null)
			{
				throw new ArgumentException(string.Format("{0} is not a valid DDIAttribute", base.AttributeType));
			}
			List<string> list = ddiattribute.Validate(target, profile);
			if (list.Count <= 0)
			{
				return new List<string>
				{
					string.Format("{0} {1}", target, this.ErrorMessage)
				};
			}
			return result;
		}
	}
}
