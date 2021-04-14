using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIValidCommandTextAttribute : DDIValidateAttribute
	{
		public DDIValidCommandTextAttribute() : base("DDIValidCommandTextAttribute")
		{
		}

		private void UpdateCmdlets()
		{
			IEnumerable<Assembly> enumerable = from assemb in AppDomain.CurrentDomain.GetAssemblies()
			where CmdletAssemblyHelper.IsCmdletAssembly(assemb.GetName().Name)
			select assemb;
			if (enumerable.Count<Assembly>() == 0)
			{
				throw new ArgumentException("No any cmdlet assembly was loaded");
			}
			DDIValidCommandTextAttribute.Cmdlets.Add("New-EdgeSubscription".ToUpper());
			foreach (Assembly assembly in enumerable)
			{
				IEnumerable<object> enumerable2 = from type in DDIValidationHelper.GetAssemblyTypes(assembly)
				where !type.IsAbstract && type.GetCustomAttributes(typeof(CmdletAttribute), false).Count<object>() == 1
				select type.GetCustomAttributes(typeof(CmdletAttribute), false)[0];
				foreach (object obj in enumerable2)
				{
					CmdletAttribute cmdletAttribute = (CmdletAttribute)obj;
					DDIValidCommandTextAttribute.Cmdlets.Add((cmdletAttribute.VerbName + "-" + cmdletAttribute.NounName).ToUpper());
				}
			}
		}

		public override List<string> Validate(object target, Service profile)
		{
			if (target != null && !(target is string))
			{
				throw new ArgumentException("DDIValidCommandTextAttribute can only be applied to String property");
			}
			if (DDIValidCommandTextAttribute.Cmdlets.Count == 0)
			{
				this.UpdateCmdlets();
			}
			List<string> list = new List<string>();
			string text = target as string;
			if (!string.IsNullOrEmpty(text) && !DDIValidCommandTextAttribute.Cmdlets.Contains(text.ToUpper()))
			{
				list.Add(string.Format("{0} is not a valid cmdlet name", target));
			}
			return list;
		}

		internal static List<string> Cmdlets = new List<string>();
	}
}
