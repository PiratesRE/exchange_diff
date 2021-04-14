using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RoleEntryIdParameter : ADIdParameter
	{
		public RoleEntryIdParameter()
		{
		}

		public RoleEntryIdParameter(string identity) : base(identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			int num = identity.LastIndexOf('\\');
			if (num == -1 || num == 0 || num == identity.Length - 1)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat(base.GetType().ToString()), "identity");
			}
			this.cmdletOrScriptName = identity.Substring(num + 1);
			this.roleId = new RoleIdParameter(identity.Substring(0, num));
		}

		public RoleEntryIdParameter(RoleIdParameter roleId, string cmdletOrScriptName, ManagementRoleEntryType type) : base(roleId.RawIdentity)
		{
			if (string.IsNullOrEmpty(cmdletOrScriptName))
			{
				this.cmdletOrScriptName = "*";
			}
			else
			{
				this.cmdletOrScriptName = cmdletOrScriptName;
			}
			this.roleId = roleId;
			this.Type = type;
		}

		public RoleEntryIdParameter(ADObjectId adObjectId, string cmdletOrScriptName, ManagementRoleEntryType type) : this(new RoleIdParameter(adObjectId), cmdletOrScriptName, type)
		{
		}

		public RoleEntryIdParameter(ExchangeRoleEntryPresentation roleEntry) : this(roleEntry.Role, roleEntry.Name, roleEntry.Type)
		{
			this.Parameters = new string[roleEntry.Parameters.Count];
			this.PSSnapinName = roleEntry.PSSnapinName;
			roleEntry.Parameters.CopyTo(this.Parameters, 0);
		}

		public RoleEntryIdParameter(ExchangeRole role) : this(new RoleIdParameter(role.Id), null, ManagementRoleEntryType.All)
		{
		}

		public RoleEntryIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal string CmdletOrScriptName
		{
			get
			{
				return this.cmdletOrScriptName;
			}
		}

		internal RoleIdParameter RoleId
		{
			get
			{
				return this.roleId;
			}
		}

		internal ManagementRoleEntryType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		internal string[] Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = value;
			}
		}

		internal string PSSnapinName
		{
			get
			{
				return this.snapinName;
			}
			set
			{
				this.snapinName = value;
			}
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					this.additionalFilter
				});
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		public static RoleEntryIdParameter Parse(string identity)
		{
			return new RoleEntryIdParameter(identity);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}\\{1}", this.RoleId.ToString(), this.CmdletOrScriptName);
			switch (this.Type)
			{
			case ManagementRoleEntryType.Cmdlet:
			case ManagementRoleEntryType.Script:
			case ManagementRoleEntryType.ApplicationPermission:
				stringBuilder.AppendFormat(", type {0}", this.Type);
				break;
			}
			if (!string.IsNullOrEmpty(this.PSSnapinName))
			{
				stringBuilder.AppendFormat(", PSSnapinName {0}", this.PSSnapinName);
			}
			if (this.Parameters != null)
			{
				stringBuilder.AppendFormat(", Parameters: {0}", string.Join(",", this.Parameters));
			}
			return stringBuilder.ToString();
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ExchangeRoleEntryPresentation) && typeof(T) != typeof(ExchangeRole))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			notFoundReason = null;
			if (typeof(T) == typeof(ExchangeRole))
			{
				return this.RoleId.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			}
			List<ExchangeRoleEntryPresentation> list = new List<ExchangeRoleEntryPresentation>();
			this.additionalFilter = this.CreateCmdletAndParametersFilter();
			IEnumerable<ExchangeRole> objects = this.roleId.GetObjects<ExchangeRole>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			foreach (ExchangeRole exchangeRole in objects)
			{
				foreach (RoleEntry roleEntry in exchangeRole.RoleEntries)
				{
					if (RBACHelper.DoesRoleEntryMatchNameAndParameters(roleEntry, this.Type, this.CmdletOrScriptName, this.Parameters, this.PSSnapinName))
					{
						IEnumerator<ExchangeRole> enumerator;
						list.Add(new ExchangeRoleEntryPresentation(enumerator.Current, roleEntry));
					}
				}
			}
			return (IEnumerable<T>)list;
		}

		private QueryFilter CreateCmdletAndParametersFilter()
		{
			List<QueryFilter> list = new List<QueryFilter>();
			if ((this.Type & ManagementRoleEntryType.Cmdlet) != (ManagementRoleEntryType)0)
			{
				list.Add(RBACHelper.ConstructRoleEntryFilter(this.CmdletOrScriptName, ManagementRoleEntryType.Cmdlet, this.PSSnapinName));
			}
			if ((this.Type & ManagementRoleEntryType.Script) != (ManagementRoleEntryType)0)
			{
				list.Add(new AndFilter(new QueryFilter[]
				{
					RBACHelper.ScriptEnabledRoleEntryTypeFilter,
					RBACHelper.ConstructRoleEntryFilter(this.CmdletOrScriptName, ManagementRoleEntryType.Script)
				}));
			}
			if ((this.Type & ManagementRoleEntryType.ApplicationPermission) != (ManagementRoleEntryType)0)
			{
				list.Add(new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleSchema.RoleType, RoleType.ApplicationImpersonation),
					RBACHelper.ConstructRoleEntryFilter(this.cmdletOrScriptName, ManagementRoleEntryType.ApplicationPermission)
				}));
			}
			List<QueryFilter> list2 = new List<QueryFilter>();
			if (1 < list.Count)
			{
				list2.Add(new OrFilter(list.ToArray()));
			}
			else if (1 == list.Count)
			{
				list2.Add(list[0]);
			}
			if (this.Parameters != null)
			{
				list2.Add(RBACHelper.ConstructRoleEntryParameterFilter(this.Parameters));
			}
			if (1 < list2.Count)
			{
				return new AndFilter(list2.ToArray());
			}
			if (1 == list2.Count)
			{
				return list2[0];
			}
			return null;
		}

		private RoleIdParameter roleId;

		private string cmdletOrScriptName;

		private ManagementRoleEntryType type = ManagementRoleEntryType.All;

		private string[] parameters;

		private string snapinName;

		private QueryFilter additionalFilter;
	}
}
