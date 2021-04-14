using System;
using System.Data;
using System.IO;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeCommandBuilder : ICommandBuilder
	{
		public IExchangeScopeBuilder ScopeBuilder
		{
			get
			{
				return this.scopeBuilder;
			}
			set
			{
				this.scopeBuilder = value;
			}
		}

		public ExchangeCommandBuilder() : this(null)
		{
		}

		public ExchangeCommandBuilder(IExchangeCommandFilterBuilder filterBuilder)
		{
			this.filterBuilder = filterBuilder;
		}

		public string BuildCommandWithScope(string commandText, string searchText, object[] pipeline, DataRow row, object scope)
		{
			return this.BuildCommandCore(commandText, searchText, pipeline, row, this.scopeBuilder.BuildScope(scope));
		}

		public string BuildCommand(string commandText, string searchText, object[] pipeline, DataRow row)
		{
			return this.BuildCommandCore(commandText, searchText, pipeline, row, null);
		}

		private string BuildCommandCore(string commandText, string searchText, object[] pipeline, DataRow row, string scopeSetting)
		{
			bool flag = false;
			if (string.IsNullOrEmpty(commandText))
			{
				throw new ArgumentNullException("Cannot build a command without name.");
			}
			string text = null;
			string value = null;
			string text2 = null;
			if (this.FilterBuilder != null)
			{
				this.FilterBuilder.BuildFilter(out text, out value, out text2, row);
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text3 = this.GetPipelineStringWhenResolveIdentity(pipeline);
			if (!string.IsNullOrEmpty(text3))
			{
				stringBuilder.AppendFormat("{0}", text3);
			}
			stringBuilder.Append(commandText);
			if (!string.IsNullOrEmpty(searchText))
			{
				switch (this.SearchType)
				{
				case 0:
					stringBuilder.AppendFormat(" -ANR '{0}'", searchText.ToQuotationEscapedString());
					break;
				case 1:
					stringBuilder.AppendFormat(" -Identity '*{0}*'", searchText.ToQuotationEscapedString());
					break;
				}
			}
			if (!string.IsNullOrEmpty(scopeSetting))
			{
				stringBuilder.Append(" " + scopeSetting);
			}
			if (!string.IsNullOrEmpty(text))
			{
				stringBuilder.AppendFormat(" {0}", text);
			}
			if (this.UseFilterToResolveNonId)
			{
				text3 = this.GetPipelineFilterStringNotResolveIdentity(pipeline);
				if (!string.IsNullOrEmpty(text3))
				{
					stringBuilder.AppendFormat(" {0}", text3);
				}
			}
			if (!string.IsNullOrEmpty(value))
			{
				stringBuilder.Append(value);
				flag = true;
			}
			if (this.searchType == 2 && !string.IsNullOrEmpty(this.NamePropertyFilter) && !string.IsNullOrEmpty(searchText))
			{
				stringBuilder.AppendFormat(" | Filter-PropertyStringContains -Property '{0}' -SearchText '{1}'", this.NamePropertyFilter.ToQuotationEscapedString(), searchText.ToQuotationEscapedString());
				flag = true;
			}
			if (!this.resolveForIdentity() && !this.UseFilterToResolveNonId && pipeline != null && pipeline.Length > 0)
			{
				stringBuilder.AppendFormat(" | Filter-PropertyInObjects -ResolveProperty '{0}' -inputObjects {1}", this.ResolveProperty.ToQuotationEscapedString(), this.GetPipelineWhereStringNotResolveIdentity(pipeline));
				flag = true;
			}
			if (flag && !WinformsHelper.IsRemoteEnabled())
			{
				string str = Path.Combine(ConfigurationContext.Setup.RemoteScriptPath, "ConsoleInitialize.ps1");
				stringBuilder.Insert(0, ". '" + str + "';");
			}
			return stringBuilder.ToString();
		}

		public ExchangeCommandBuilderSearch SearchType
		{
			get
			{
				return this.searchType;
			}
			set
			{
				this.searchType = value;
			}
		}

		public string ResolveProperty
		{
			get
			{
				return this.resolveProperty;
			}
			set
			{
				this.resolveProperty = value;
			}
		}

		public bool UseFilterToResolveNonId
		{
			get
			{
				return this.useFilterToResolveNonId;
			}
			set
			{
				this.useFilterToResolveNonId = value;
			}
		}

		public string NamePropertyFilter { get; set; }

		public IExchangeCommandFilterBuilder FilterBuilder
		{
			get
			{
				return this.filterBuilder;
			}
			set
			{
				this.filterBuilder = value;
			}
		}

		internal bool resolveForIdentity()
		{
			return string.IsNullOrEmpty(this.ResolveProperty) || string.Equals("Id", this.ResolveProperty, StringComparison.InvariantCultureIgnoreCase) || string.Equals("Identity", this.ResolveProperty, StringComparison.InvariantCultureIgnoreCase);
		}

		private string GetPipelineStringWhenResolveIdentity(object[] pipeline)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (pipeline != null && pipeline.Length > 0 && this.resolveForIdentity())
			{
				for (int i = 0; i < pipeline.Length; i++)
				{
					stringBuilder.AppendFormat((pipeline.Length - 1 != i) ? "'{0}'," : "'{0}' | ", pipeline[i].ToSustainedString());
				}
			}
			return stringBuilder.ToString();
		}

		private string GetPipelineWhereStringNotResolveIdentity(object[] pipeline)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (pipeline != null && pipeline.Length > 0)
			{
				for (int i = 0; i < pipeline.Length; i++)
				{
					stringBuilder.AppendFormat((pipeline.Length - 1 != i) ? "'{0}'," : "'{0}'", pipeline[i].ToQuotationEscapedString());
				}
			}
			return stringBuilder.ToString();
		}

		private string GetPipelineFilterStringNotResolveIdentity(object[] pipeline)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (pipeline != null && pipeline.Length > 0 && !this.resolveForIdentity())
			{
				stringBuilder.Append("-Filter '");
				for (int i = 0; i < pipeline.Length; i++)
				{
					ADObjectId adobjectId = pipeline[i] as ADObjectId;
					string item = string.Format((pipeline.Length - 1 == i) ? "{0} -eq '{1}'" : "{0} -eq '{1}' -or ", this.ResolveProperty, (adobjectId != null) ? adobjectId.DistinguishedName.ToQuotationEscapedString() : pipeline[i].ToQuotationEscapedString());
					stringBuilder.Append(item.ToQuotationEscapedString());
				}
				stringBuilder.Append("'");
			}
			return stringBuilder.ToString();
		}

		private ExchangeCommandBuilderSearch searchType = 1;

		private string resolveProperty;

		private IExchangeCommandFilterBuilder filterBuilder;

		private IExchangeScopeBuilder scopeBuilder = new ExchangeOUScopeBuilder();

		private bool useFilterToResolveNonId;
	}
}
