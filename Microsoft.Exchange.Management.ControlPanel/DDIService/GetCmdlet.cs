using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	public class GetCmdlet : OutputObjectCmdlet, INotAccessChecker
	{
		public GetCmdlet()
		{
			base.AllowExceuteThruHttpGetRequest = true;
		}

		protected GetCmdlet(GetCmdlet activity) : base(activity)
		{
			this.SingletonObject = activity.SingletonObject;
		}

		public override Activity Clone()
		{
			return new GetCmdlet(this);
		}

		[DefaultValue(false)]
		public bool SingletonObject
		{
			get
			{
				return this.singletonObject;
			}
			set
			{
				if (value)
				{
					base.IdentityVariable = string.Empty;
				}
				this.singletonObject = value;
			}
		}

		internal override bool HasPermission(DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			List<string> list = (from c in this.GetEffectiveParameters(input, dataTable, store)
			select c.Name).ToList<string>();
			if (!this.SingletonObject && !list.Contains(base.IdentityName, StringComparer.OrdinalIgnoreCase))
			{
				list.Add(base.IdentityName);
			}
			return this.CheckPermission(store, list, updatingVariable);
		}

		protected override string GetVerb()
		{
			return "Get-";
		}

		protected override void DoPreRunCore(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
			if (!this.SingletonObject)
			{
				DDIHelper.Trace("Map identity parameter : {0} - {1}", new object[]
				{
					base.IdentityName,
					base.IdentityVariable
				});
				Parameter item = new Parameter
				{
					Name = base.IdentityName,
					Reference = base.IdentityVariable,
					Type = ParameterType.Mandatory
				};
				if (!base.Parameters.Contains(item))
				{
					base.Parameters.Add(item);
				}
			}
			base.DoPreRunCore(input, dataTable, store, codeBehind);
		}

		public override IList<Parameter> GetEffectiveParameters(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			IList<Parameter> list = base.GetEffectiveParameters(input, dataTable, store) ?? new List<Parameter>();
			string commandText = base.GetCommandText(store);
			if (!this.SingletonObject && GetCmdlet.SupportReadFromDcCmdlets.Contains(commandText, StringComparer.OrdinalIgnoreCase) && !DDIHelper.IsFFO())
			{
				Parameter item = new Parameter
				{
					Name = "ReadFromDomainController",
					Type = ParameterType.Switch
				};
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private static readonly ICollection<string> SupportReadFromDcCmdlets = new string[]
		{
			"Get-Contact",
			"Get-DistributionGroup",
			"Get-DynamicDistributionGroup",
			"Get-Group",
			"Get-Mailbox",
			"Get-MailContact",
			"Get-MailUser",
			"Get-Recipient",
			"Get-SiteMailbox",
			"Get-User"
		};

		private bool singletonObject;
	}
}
