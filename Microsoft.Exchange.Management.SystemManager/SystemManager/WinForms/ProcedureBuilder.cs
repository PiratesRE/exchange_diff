using System;
using System.Data;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Directory;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ProcedureBuilder : ICommandBuilder
	{
		public string ResolveProperty { get; set; }

		public ExchangeCommandBuilderSearch SearchType { get; set; }

		public string BuildCommand(string commandText, string searchText, object[] pipeline, DataRow row)
		{
			throw new NotImplementedException();
		}

		public string BuildCommandWithScope(string commandText, string searchText, object[] pipeline, DataRow row, object scope)
		{
			throw new NotImplementedException();
		}

		public string NamePropertyFilter { get; set; }

		public bool UseFilterToResolveNonId { get; set; }

		internal MonadCommand BuildProcedure(string commandText, string searchText, object[] pipeline, DataRow row)
		{
			return this.BuildProcedureWithScope(commandText, searchText, pipeline, row, null);
		}

		internal MonadCommand BuildProcedureWithScope(string commandText, string searchText, object[] pipeline, DataRow row, object scope)
		{
			MonadCommand monadCommand = this.InnerBuildProcedureCore(commandText, row);
			if (!string.IsNullOrEmpty(searchText))
			{
				switch (this.SearchType)
				{
				case 0:
					monadCommand.Parameters.AddWithValue("ANR", searchText);
					break;
				case 1:
					monadCommand.Parameters.AddWithValue("Identity", string.Format("*{0}*", searchText));
					break;
				}
			}
			if (!string.IsNullOrEmpty(scope as string))
			{
				monadCommand.Parameters.AddWithValue("OrganizationalUnit", scope);
			}
			if (this.UseFilterToResolveNonId)
			{
				string pipelineFilterStringNotResolveIdentity = this.GetPipelineFilterStringNotResolveIdentity(pipeline);
				if (!string.IsNullOrEmpty(pipelineFilterStringNotResolveIdentity))
				{
					string value = pipelineFilterStringNotResolveIdentity;
					if (monadCommand.Parameters.Contains("Filter"))
					{
						value = string.Format("({0}) -and ({1})", pipelineFilterStringNotResolveIdentity, monadCommand.Parameters["Filter"].Value);
						monadCommand.Parameters["Filter"].Value = value;
					}
					else
					{
						monadCommand.Parameters.AddWithValue("Filter", value);
					}
				}
			}
			return monadCommand;
		}

		private string GetPipelineFilterStringNotResolveIdentity(object[] pipeline)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (pipeline != null && pipeline.Length > 0)
			{
				for (int i = 0; i < pipeline.Length; i++)
				{
					ADObjectId adobjectId = pipeline[i] as ADObjectId;
					string item = string.Format((pipeline.Length - 1 == i) ? "{0} -eq '{1}'" : "{0} -eq '{1}' -or ", this.ResolveProperty, (adobjectId != null) ? adobjectId.DistinguishedName.ToQuotationEscapedString() : pipeline[i].ToQuotationEscapedString());
					stringBuilder.Append(item.ToQuotationEscapedString());
				}
			}
			return stringBuilder.ToString();
		}

		internal abstract MonadCommand InnerBuildProcedureCore(string commandText, DataRow row);
	}
}
