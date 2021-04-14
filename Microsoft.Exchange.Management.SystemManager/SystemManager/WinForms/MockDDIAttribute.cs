using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public class MockDDIAttribute : DDIValidateAttribute
	{
		public bool ReportError { get; set; }

		public MockDDIAttribute() : base("MockDDIAttribute")
		{
		}

		public override List<string> Validate(object target, PageConfigurableProfile profile)
		{
			List<string> list = new List<string>();
			if (this.ReportError)
			{
				list.Add("MockDDIAttribute error: " + profile.Name);
			}
			return list;
		}
	}
}
