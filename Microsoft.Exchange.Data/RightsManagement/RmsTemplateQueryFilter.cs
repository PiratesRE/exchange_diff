using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.RightsManagement
{
	internal sealed class RmsTemplateQueryFilter : QueryFilter
	{
		public RmsTemplateQueryFilter(Guid id, string name)
		{
			this.id = id;
			this.name = name;
		}

		public override void ToString(StringBuilder sb)
		{
			string arg = string.IsNullOrEmpty(this.name) ? "*" : this.name;
			sb.AppendFormat("(Id={0}) || (Name={1})", this.id, arg);
		}

		internal bool Match(RmsTemplate template)
		{
			if (template == null)
			{
				return false;
			}
			if (this.id.Equals(template.Id) || string.IsNullOrEmpty(this.name) || string.Equals(this.name, template.Name, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			foreach (CultureInfo locale in RmsTemplate.SupportedClientLanguages)
			{
				if (string.Equals(this.name, template.GetName(locale), StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		internal static readonly RmsTemplateQueryFilter MatchAll = new RmsTemplateQueryFilter(Guid.Empty, null);

		private Guid id;

		private string name;
	}
}
