using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Forefront.Reporting.Common
{
	public class QueryCompiler
	{
		internal QueryCompiler(OnDemandQueryType queryType, string queryStringIn, PIIHashingDelegate piiHashingDelegate, LSHDelegate lshDelegate)
		{
			if (string.IsNullOrWhiteSpace(queryStringIn))
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.EmptySearchDefinition);
			}
			this.QueryString = queryStringIn;
			this.Type = queryType;
			this.QueryTimeRange = new List<Tuple<DateTime, DateTime>>();
			if (piiHashingDelegate != null)
			{
				this.HashEnable = true;
				this.hashingDelegate = piiHashingDelegate;
				this.lshDelegate = lshDelegate;
			}
			else
			{
				this.HashEnable = false;
			}
			this.Compile();
		}

		internal QueryCompiler(OnDemandQueryType queryType, string queryStringIn) : this(queryType, queryStringIn, null, null)
		{
		}

		internal List<Tuple<DateTime, DateTime>> QueryTimeRange { get; private set; }

		internal string CompiledCode { get; private set; }

		internal string PreFilteringCode { get; private set; }

		internal string QueryString { get; private set; }

		internal bool HashEnable { get; private set; }

		internal OnDemandQueryType Type { get; private set; }

		internal static void ValidateQuery(Guid requestID, PropertyAccessDelegate propertyDelegate)
		{
			Guid a = (Guid)propertyDelegate(OnDemandQueryRequestSchema.TenantId);
			if (a == Constants.TenantIDForSystemCount)
			{
				return;
			}
			new QueryCompiler((OnDemandQueryType)propertyDelegate(OnDemandQueryRequestSchema.QueryType), (string)propertyDelegate(OnDemandQueryRequestSchema.QueryDefinition));
		}

		internal string GetPIIHash(string valueString)
		{
			if (this.HashEnable)
			{
				return this.hashingDelegate(valueString);
			}
			return valueString;
		}

		internal string GetLSH(string valueString)
		{
			if (this.HashEnable)
			{
				return this.lshDelegate(valueString);
			}
			return valueString;
		}

		private void Compile()
		{
			QueryGroupField queryGroupField = new QueryGroupField(this.QueryString, this);
			string arg = queryGroupField.Compile();
			switch (this.Type)
			{
			case OnDemandQueryType.DLP:
				this.CompiledCode = string.Format("if(CFRTools.IsDLP(tra_etr) && {0})", arg);
				goto IL_AA;
			case OnDemandQueryType.Rule:
				this.CompiledCode = string.Format("if(CFRTools.IsTransportRule(tra_etr) && {0})", arg);
				goto IL_AA;
			case OnDemandQueryType.AntiSpam:
				this.CompiledCode = string.Format("if(CFRTools.IsSpam(sfa_sum) && {0})", arg);
				goto IL_AA;
			case OnDemandQueryType.AntiVirus:
				this.CompiledCode = string.Format("if(CFRTools.IsMalware(ama_sum) && {0})", arg);
				goto IL_AA;
			}
			if (!queryGroupField.HasOptionalCriterion)
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.DoesNotMeetMinimalQueryRequirement);
			}
			this.CompiledCode = string.Format("if{0}", arg);
			IL_AA:
			string arg2 = string.Join(" || ", from range in this.QueryTimeRange
			select string.Format("(originTimeTicks >= {0} && originTimeTicks <= {1})", range.Item1.Ticks, range.Item2.Ticks));
			this.PreFilteringCode = string.Format("if({0})", arg2);
		}

		internal const string MsgStatusHolderString = "%MsgStatus%";

		private PIIHashingDelegate hashingDelegate;

		private LSHDelegate lshDelegate;
	}
}
