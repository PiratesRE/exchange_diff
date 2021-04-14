using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	public sealed class DataClassificationIdParameter : ClassificationRuleCollectionIdParameter
	{
		private DataClassificationIdParameter(string rawIdentity) : base(rawIdentity)
		{
		}

		private DataClassificationIdParameter(string ruleCollectionObjectIdentity, string dataClassificationId) : base(ruleCollectionObjectIdentity)
		{
			ExAssert.RetailAssert(!string.IsNullOrEmpty(dataClassificationId), "The data classification ID passed to DataClassificationIdParameter must be validated in Parse()");
			this.dataClassificationIdentityFilter = dataClassificationId;
			this.InitializeDataClassificationMatchFilter();
		}

		public DataClassificationIdParameter()
		{
		}

		public new static DataClassificationIdParameter Parse(string identity)
		{
			DataClassificationIdParameter dataClassificationIdParameter = new DataClassificationIdParameter(identity);
			return new DataClassificationIdParameter(dataClassificationIdParameter.IsHierarchical ? ClassificationDefinitionUtils.CreateHierarchicalIdentityString(dataClassificationIdParameter.OrganizationName, "*") : "*", dataClassificationIdParameter.FriendlyName);
		}

		internal override void Initialize(ObjectId objectId)
		{
			ADObjectId adobjectId = objectId as ADObjectId;
			if (adobjectId == null || base.InternalADObjectId != null)
			{
				base.Initialize(objectId);
				return;
			}
			if (!string.IsNullOrEmpty(adobjectId.DistinguishedName))
			{
				throw new ArgumentException(Strings.DataClassificationDnIdentityNotSupported, "objectId");
			}
		}

		public override string ToString()
		{
			string result;
			if (this.dataClassificationIdentityFilter != null)
			{
				result = (base.IsHierarchical ? ClassificationDefinitionUtils.CreateHierarchicalIdentityString(base.OrganizationName, this.dataClassificationIdentityFilter) : this.dataClassificationIdentityFilter);
			}
			else
			{
				result = base.ToString();
			}
			return result;
		}

		public string DataClassificationIdentity
		{
			get
			{
				return this.dataClassificationIdentityFilter;
			}
		}

		private void InitializeDataClassificationMatchFilter()
		{
			this.resultsFilter = null;
			TextFilter nameQueryFilter = this.IsWildcardDefined(this.dataClassificationIdentityFilter) ? ((TextFilter)base.CreateWildcardFilter(ADObjectSchema.Name, this.dataClassificationIdentityFilter)) : new TextFilter(ADObjectSchema.Name, this.dataClassificationIdentityFilter, MatchOptions.FullString, MatchFlags.Default);
			DataClassificationIdentityMatcher dataClassificationIdentityMatcher = DataClassificationIdentityMatcher.CreateFrom(nameQueryFilter, this.dataClassificationIdentityFilter);
			if (dataClassificationIdentityMatcher != null)
			{
				this.resultsFilter = dataClassificationIdentityMatcher;
			}
		}

		internal IEnumerable<DataClassificationPresentationObject> FilterResults(IEnumerable<DataClassificationPresentationObject> unfilteredResults)
		{
			if (this.resultsFilter != null)
			{
				return unfilteredResults.Where(new Func<DataClassificationPresentationObject, bool>(this.resultsFilter.Match));
			}
			return unfilteredResults;
		}

		private const string AllRuleCollectionObjectIdentityQuery = "*";

		private readonly string dataClassificationIdentityFilter;

		private ClassificationIdentityMatcher<DataClassificationPresentationObject> resultsFilter;
	}
}
