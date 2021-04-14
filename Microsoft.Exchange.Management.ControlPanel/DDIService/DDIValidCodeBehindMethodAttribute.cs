using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIValidCodeBehindMethodAttribute : DDIValidateAttribute
	{
		public DDIValidCodeBehindMethodAttribute() : base("DDIValidCodeBehindMethodAttribute")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			if (target != null && !(target is string))
			{
				throw new ArgumentException("DDIValidCodeBehindMethodAttribute can only be applied to String property");
			}
			List<string> list = new List<string>();
			string methodName = target as string;
			if (!string.IsNullOrEmpty(methodName))
			{
				Type @class = profile.Class;
				if (@class == null)
				{
					list.Add(string.Format("Code behind method {0} is used, but the code hehind class in not specified.", methodName));
				}
				else if ((from c in profile.Class.GetMethods()
				where c.Name == methodName
				select c).Count<MethodInfo>() != 1)
				{
					list.Add(string.Format("Code behind method {0} was NOT found or defined multiple times in class {1}.", methodName, @class));
				}
				else
				{
					MethodInfo method = profile.Class.GetMethod(methodName, new Type[]
					{
						typeof(DataRow),
						typeof(DataTable),
						typeof(DataObjectStore)
					});
					if (method == null)
					{
						method = profile.Class.GetMethod(methodName, new Type[]
						{
							typeof(DataRow),
							typeof(DataTable),
							typeof(DataObjectStore),
							typeof(PowerShellResults[])
						});
						if (method == null)
						{
							throw new NotImplementedException("The specified method " + methodName + " should implement one of the two signatures: (DataRow, DataTable, DataObjectStore) or (DataRow, DataTable, DataObjectStore, PowerShellResults[]) .");
						}
					}
				}
			}
			return list;
		}
	}
}
