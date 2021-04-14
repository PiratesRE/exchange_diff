using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(ManagementScopeRow))]
	public class ManagementScopeRow : DropDownItemData
	{
		public static bool IsDefaultScope(string scopeId)
		{
			return scopeId == ManagementScopeRow.DefaultScopeId;
		}

		public static bool IsMultipleScope(string scopeId)
		{
			return scopeId == string.Empty;
		}

		public static ManagementScopeRow DefaultRow
		{
			get
			{
				return new ManagementScopeRow();
			}
		}

		public ManagementScopeRow(ManagementScope managementScope) : base(managementScope)
		{
			this.ManagementScope = managementScope;
			base.Text = managementScope.Name;
			base.Value = managementScope.Name;
		}

		private ManagementScopeRow()
		{
			base.Text = Strings.DefaultScope;
			base.Value = ManagementScopeRow.DefaultScopeId;
		}

		protected ManagementScope ManagementScope { get; set; }

		[DataMember]
		public string DisplayName
		{
			get
			{
				return base.Text;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return base.Text;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public ScopeRestrictionType ScopeRestrictionType
		{
			get
			{
				if (this.ManagementScope != null)
				{
					return this.ManagementScope.ScopeRestrictionType;
				}
				return ScopeRestrictionType.RecipientScope;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public static readonly string DefaultScopeId = Guid.Empty.ToString();
	}
}
