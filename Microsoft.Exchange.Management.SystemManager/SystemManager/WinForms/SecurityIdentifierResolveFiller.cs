using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class SecurityIdentifierResolveFiller : AbstractDataTableFiller
	{
		public SecurityIdentifierResolveFiller(string sidColumn, string userColumn)
		{
			if (string.IsNullOrEmpty(sidColumn))
			{
				throw new ArgumentNullException("sidColumn");
			}
			if (string.IsNullOrEmpty(userColumn))
			{
				throw new ArgumentNullException("userColumn");
			}
			this.sidColumn = sidColumn;
			this.userColumn = userColumn;
		}

		public override ICommandBuilder CommandBuilder
		{
			get
			{
				return null;
			}
		}

		public override void BuildCommand(string searchText, object[] pipeline, DataRow row)
		{
			this.BuildCommandWithScope(searchText, pipeline, row, null);
		}

		public override void BuildCommandWithScope(string searchText, object[] pipeline, DataRow row, object scope)
		{
			if (!DBNull.Value.Equals(row["SidToUserFriendlyNameMap"]))
			{
				this.sidToUserFriendlyNameMap = (IDictionary<SecurityIdentifier, string>)row["SidToUserFriendlyNameMap"];
				this.pipeline = pipeline;
				return;
			}
			throw new NotSupportedException("Input valude for SidToUserFriendlyNameMap is mandantory.");
		}

		protected override void OnFill(DataTable table)
		{
			table.BeginLoadData();
			foreach (SecurityIdentifier securityIdentifier in this.pipeline)
			{
				DataRow dataRow = table.NewRow();
				dataRow[this.sidColumn] = securityIdentifier;
				dataRow[this.userColumn] = this.sidToUserFriendlyNameMap[securityIdentifier];
				table.Rows.Add(dataRow);
			}
			table.EndLoadData();
		}

		public override object Clone()
		{
			return new SecurityIdentifierResolveFiller(this.sidColumn, this.userColumn);
		}

		private IDictionary<SecurityIdentifier, string> sidToUserFriendlyNameMap;

		private object[] pipeline;

		private string userColumn;

		private string sidColumn;
	}
}
