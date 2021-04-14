using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIValidCommandTextAttribute : DDIValidateAttribute
	{
		public DDIValidCommandTextAttribute() : base("DDIValidCommandTextAttribute")
		{
		}

		private void UpdateCmdlets()
		{
			IEnumerable<Assembly> source = from assemb in AppDomain.CurrentDomain.GetAssemblies()
			where CmdletAssemblyHelper.IsCmdletAssembly(assemb.GetName().Name)
			select assemb;
			if (source.Count<Assembly>() == 0)
			{
				throw new ArgumentException("Microsoft.Exchange.Management dll is not loaded");
			}
			DDIValidCommandTextAttribute.cmdlets.Add("New-EdgeSubscription".ToUpper());
			IEnumerable<object> enumerable = from type in DDIValidationHelper.GetAssemblyTypes(source.First<Assembly>())
			where !type.IsAbstract && type.GetCustomAttributes(typeof(CmdletAttribute), false).Count<object>() == 1
			select type.GetCustomAttributes(typeof(CmdletAttribute), false)[0];
			foreach (object obj in enumerable)
			{
				CmdletAttribute cmdletAttribute = (CmdletAttribute)obj;
				DDIValidCommandTextAttribute.cmdlets.Add((cmdletAttribute.VerbName + "-" + cmdletAttribute.NounName).ToUpper());
			}
		}

		public override List<string> Validate(object target, PageConfigurableProfile profile)
		{
			if (target != null && !(target is string))
			{
				throw new ArgumentException("DDIValidCommandTextAttribute can only be applied to String property");
			}
			if (DDIValidCommandTextAttribute.cmdlets.Count == 0)
			{
				this.UpdateCmdlets();
			}
			List<string> list = new List<string>();
			string text = target as string;
			if (!string.IsNullOrEmpty(text) && !DDIValidCommandTextAttribute.cmdlets.Contains(text.ToUpper()))
			{
				list.Add(string.Format("{0} is not a valid cmdlet name", target));
			}
			return list;
		}

		internal static List<string> cmdlets = new List<string>();
	}
}
