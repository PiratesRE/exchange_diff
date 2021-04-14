using System;
using System.ComponentModel;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public class IfInRole : BranchActivity
	{
		public IfInRole()
		{
		}

		protected IfInRole(IfInRole activity) : base(activity)
		{
			this.Role = activity.Role;
		}

		public override Activity Clone()
		{
			return new IfInRole(this);
		}

		[DefaultValue(null)]
		[DDIValidRole]
		public string Role { get; set; }

		protected override bool CalculateCondition(DataRow input, DataTable dataTable)
		{
			string[] array = this.Role.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (base.RbacChecker.IsInRole(array[i]))
				{
					return true;
				}
			}
			return false;
		}

		internal override bool HasPermission(DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			if (base.CheckCondition(input, dataTable))
			{
				return base.Then == null || base.Then.HasPermission(input, dataTable, store, updatingVariable);
			}
			return base.Else == null || base.Else.HasPermission(input, dataTable, store, updatingVariable);
		}

		internal override bool? FindAndCheckPermission(Func<Activity, bool> predicate, DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			bool? flag = null;
			bool? flag2 = null;
			bool? result = null;
			if (base.Then != null)
			{
				flag = base.Then.FindAndCheckPermission(predicate, input, dataTable, store, updatingVariable);
			}
			if (base.Else != null)
			{
				flag2 = base.Else.FindAndCheckPermission(predicate, input, dataTable, store, updatingVariable);
			}
			if (base.CheckCondition(input, dataTable))
			{
				result = flag;
				if (result == null && flag2 != null)
				{
					result = new bool?(false);
				}
			}
			else
			{
				result = flag2;
				if (result == null && flag != null)
				{
					result = new bool?(false);
				}
			}
			return result;
		}
	}
}
