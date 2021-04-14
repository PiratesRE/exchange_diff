using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class TransportRuleFilter : SearchTextFilter
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Get-TransportRule";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		[DataMember]
		public string DlpPolicy
		{
			get
			{
				return ((string)base["DlpPolicy"]) ?? string.Empty;
			}
			set
			{
				base["DlpPolicy"] = value;
			}
		}

		protected override string[] FilterableProperties
		{
			get
			{
				return TransportRuleFilter.filterableProperties;
			}
		}

		protected override SearchTextFilterType FilterType
		{
			get
			{
				return SearchTextFilterType.Contains;
			}
		}

		private static string[] filterableProperties = new string[]
		{
			"Description"
		};
	}
}
