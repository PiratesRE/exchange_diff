using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class FailedMSOSyncObjectIdParameter : CompoundSyncObjectIdParameter
	{
		public FailedMSOSyncObjectIdParameter(string identity) : base(identity)
		{
			if (!base.IsServiceInstanceDefinied && base.ServiceInstanceIdentity != "*")
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("identity"), "identity");
			}
		}

		public FailedMSOSyncObjectIdParameter(CompoundSyncObjectId compoundSyncObjectId) : base(compoundSyncObjectId)
		{
		}

		public FailedMSOSyncObjectIdParameter()
		{
		}

		public override IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			notFoundReason = null;
			QueryFilter objectFilter = this.GetObjectFilter();
			return session.FindPaged<T>(objectFilter, rootId, !base.IsServiceInstanceDefinied, null, 0);
		}

		private static void AddSyncObjectIdFilter(List<QueryFilter> filters, string elementValue, ADPropertyDefinition elementProperty)
		{
			if (!string.IsNullOrEmpty(elementValue) && "*" != elementValue)
			{
				if (elementValue.Contains("*"))
				{
					filters.Add(new TextFilter(elementProperty, elementValue, MatchOptions.WildcardString, MatchFlags.IgnoreCase));
					return;
				}
				object propertyValue = elementValue;
				if (elementProperty.Type == typeof(DirectoryObjectClass))
				{
					propertyValue = Enum.Parse(typeof(DirectoryObjectClass), elementValue, true);
				}
				filters.Add(new ComparisonFilter(ComparisonOperator.Equal, elementProperty, propertyValue));
			}
		}

		private QueryFilter GetObjectFilter()
		{
			string elementValue;
			string elementValue2;
			string elementValue3;
			base.GetSyncObjectIdElements(out elementValue, out elementValue2, out elementValue3);
			List<QueryFilter> list = new List<QueryFilter>();
			FailedMSOSyncObjectIdParameter.AddSyncObjectIdFilter(list, elementValue, FailedMSOSyncObjectSchema.ContextId);
			FailedMSOSyncObjectIdParameter.AddSyncObjectIdFilter(list, elementValue2, FailedMSOSyncObjectSchema.ObjectId);
			FailedMSOSyncObjectIdParameter.AddSyncObjectIdFilter(list, elementValue3, FailedMSOSyncObjectSchema.ExternalDirectoryObjectClass);
			if (list.Count > 0)
			{
				return new AndFilter(list.ToArray());
			}
			return null;
		}
	}
}
