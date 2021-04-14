using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ValidationRules;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ScopeSet
	{
		public ScopeSet(ADScope recipientReadScope, IList<ADScopeCollection> recipientWriteScopes, ADScope configReadScope, ADScope configWriteScope) : this(recipientReadScope, recipientWriteScopes, null, configReadScope, configWriteScope, null, null)
		{
		}

		public ScopeSet(ADScope recipientReadScope, IList<ADScopeCollection> recipientWriteScopes, IList<ADScope> exclusiveRecipientScopes, ADScope configReadScope, ADScope configWriteScope, Dictionary<string, IList<ADScopeCollection>> objectSpecificConfigWriteScopes, Dictionary<string, ADScopeCollection> objectSpecificExclusiveConfigWriteScopes) : this(recipientReadScope, recipientWriteScopes, exclusiveRecipientScopes, configReadScope, configWriteScope, objectSpecificConfigWriteScopes, objectSpecificExclusiveConfigWriteScopes, null)
		{
		}

		public ScopeSet(ADScope recipientReadScope, IList<ADScopeCollection> recipientWriteScopes, IList<ADScope> exclusiveRecipientScopes, ADScope configReadScope, ADScope configWriteScope, Dictionary<string, IList<ADScopeCollection>> objectSpecificConfigWriteScopes, Dictionary<string, ADScopeCollection> objectSpecificExclusiveConfigWriteScopes, IList<ValidationRule> applicableValidationRules)
		{
			this.recipientReadScope = (recipientReadScope ?? ADScope.Empty);
			this.recipientWriteScopes = new ReadOnlyCollection<ADScopeCollection>(recipientWriteScopes ?? ((IList<ADScopeCollection>)ADScopeCollection.EmptyArray));
			this.exclusiveRecipientScopes = ((exclusiveRecipientScopes == null || exclusiveRecipientScopes.Count == 0) ? ADScopeCollection.Empty : new ADScopeCollection(exclusiveRecipientScopes));
			this.configReadScope = (configReadScope ?? ADScope.Empty);
			this.configWriteScope = (configWriteScope ?? ADScope.Empty);
			if (this.configWriteScope != ADScope.NoAccess && this.configReadScope == ADScope.NoAccess)
			{
				throw new ArgumentException("configReadScope must be granted when configWriteScope is allowed");
			}
			this.objectSpecificConfigWriteScopes = objectSpecificConfigWriteScopes;
			if (this.objectSpecificConfigWriteScopes != null && this.configWriteScope == ADScope.NoAccess)
			{
				throw new ArgumentException("configWriteScope must be granted when objectSpecificConfigWriteScopes is defined");
			}
			this.objectSpecificExclusiveConfigWriteScopes = objectSpecificExclusiveConfigWriteScopes;
			this.validationRules = ((applicableValidationRules == null) ? new List<ValidationRule>(0) : applicableValidationRules);
		}

		internal static ScopeSet GetOrgWideDefaultScopeSet(OrganizationId organizationId)
		{
			return ScopeSet.GetOrgWideDefaultScopeSet(organizationId, null);
		}

		internal static ScopeSet GetOrgWideDefaultScopeSet(OrganizationId organizationId, QueryFilter recipientReadFilter)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			ADScopeCollection item = new ADScopeCollection(new List<ADScope>
			{
				new ADScope(organizationId.OrganizationalUnit, null)
			});
			IList<ADScopeCollection> list = new List<ADScopeCollection>();
			list.Add(item);
			return new ScopeSet(new ADScope(organizationId.OrganizationalUnit, recipientReadFilter), list, new ADScope(organizationId.ConfigurationUnit, null), null);
		}

		internal static ScopeSet GetAllTenantsDefaultScopeSet(string partitionFqdn)
		{
			ADScope item = new ADScope(ADSession.GetHostedOrganizationsRoot(partitionFqdn), null);
			ADScopeCollection item2 = new ADScopeCollection(new List<ADScope>(1)
			{
				item
			});
			return new ScopeSet(item, new List<ADScopeCollection>(1)
			{
				item2
			}, new ADScope(ADSession.GetHostedOrganizationsRoot(partitionFqdn), null), null);
		}

		public static ScopeSet ResolveUnderScope(OrganizationId organizationId, ScopeSet scopeSet)
		{
			return ScopeSet.ResolveUnderScope(organizationId, scopeSet, true);
		}

		internal static ScopeSet ResolveUnderScope(OrganizationId organizationId, ScopeSet scopeSet, bool checkOrgScope)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (organizationId.OrganizationalUnit == null || organizationId.ConfigurationUnit == null)
			{
				throw new ArgumentException("Invalid under scope organization provided");
			}
			if (scopeSet == null)
			{
				return ScopeSet.GetOrgWideDefaultScopeSet(organizationId);
			}
			if (checkOrgScope)
			{
				if (scopeSet.RecipientReadScope.Root != null && !organizationId.OrganizationalUnit.IsDescendantOf(scopeSet.RecipientReadScope.Root))
				{
					throw new ADScopeException(DirectoryStrings.ExceptionOrgScopeNotInUserScope(organizationId.OrganizationalUnit.ToString(), scopeSet.RecipientReadScope.Root.ToString()), null);
				}
				if (scopeSet.ConfigReadScope.Root != null && !organizationId.ConfigurationUnit.Parent.IsDescendantOf(scopeSet.ConfigReadScope.Root.Parent))
				{
					throw new ADScopeException(DirectoryStrings.ExceptionOrgScopeNotInUserScope(organizationId.ConfigurationUnit.Parent.ToString(), scopeSet.ConfigReadScope.Root.Parent.ToString()), null);
				}
			}
			return new ScopeSet(new ADScope(organizationId.OrganizationalUnit, (scopeSet.RecipientReadScope != null) ? scopeSet.RecipientReadScope.Filter : null), scopeSet.RecipientWriteScopes, scopeSet.exclusiveRecipientScopes, new ADScope(organizationId.ConfigurationUnit, (scopeSet.ConfigReadScope != null) ? scopeSet.ConfigReadScope.Filter : null), new ADScope(organizationId.ConfigurationUnit, (scopeSet.configWriteScope != null) ? scopeSet.configWriteScope.Filter : null), scopeSet.objectSpecificConfigWriteScopes, scopeSet.objectSpecificExclusiveConfigWriteScopes, scopeSet.validationRules);
		}

		public ADScope RecipientReadScope
		{
			get
			{
				return this.recipientReadScope;
			}
		}

		public IList<ADScopeCollection> RecipientWriteScopes
		{
			get
			{
				return this.recipientWriteScopes;
			}
		}

		public ADScopeCollection ExclusiveRecipientScopes
		{
			get
			{
				return this.exclusiveRecipientScopes;
			}
		}

		public ADScope ConfigReadScope
		{
			get
			{
				return this.configReadScope;
			}
		}

		public ADScope ConfigWriteScope
		{
			get
			{
				return this.configWriteScope;
			}
		}

		public IList<ValidationRule> ValidationRules
		{
			get
			{
				return this.validationRules;
			}
		}

		public IList<ADScopeCollection> GetConfigWriteScopes(string className)
		{
			if (className == null)
			{
				throw new ArgumentNullException("className");
			}
			IList<ADScopeCollection> result;
			if (this.objectSpecificConfigWriteScopes != null && this.objectSpecificConfigWriteScopes.TryGetValue(className, out result))
			{
				return result;
			}
			return null;
		}

		public ADScopeCollection GetExclusiveConfigWriteScopes(string className)
		{
			if (className == null)
			{
				throw new ArgumentNullException("className");
			}
			ADScopeCollection result;
			if (this.objectSpecificExclusiveConfigWriteScopes != null && this.objectSpecificExclusiveConfigWriteScopes.TryGetValue(className, out result))
			{
				return result;
			}
			return null;
		}

		internal LocalizedString ToLocalizedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(DirectoryStrings.RecipientReadScope);
			stringBuilder.Append("{");
			if (this.RecipientReadScope != null)
			{
				stringBuilder.Append(this.RecipientReadScope.ToString());
			}
			stringBuilder.Append("}");
			stringBuilder.Append(", ");
			stringBuilder.Append(DirectoryStrings.RecipientWriteScopes);
			stringBuilder.Append("{");
			if (this.RecipientWriteScopes != null)
			{
				foreach (ADScopeCollection adscopeCollection in this.RecipientWriteScopes)
				{
					stringBuilder.Append(adscopeCollection.ToString());
				}
			}
			stringBuilder.Append("}");
			stringBuilder.Append(", ");
			stringBuilder.Append(DirectoryStrings.ConfigReadScope);
			stringBuilder.Append("{");
			if (this.ConfigReadScope != null)
			{
				stringBuilder.Append(this.ConfigReadScope.ToString());
			}
			stringBuilder.Append("}");
			stringBuilder.Append(", ");
			stringBuilder.Append(DirectoryStrings.ConfigWriteScopes);
			stringBuilder.Append("{");
			if (this.ConfigWriteScope != null)
			{
				stringBuilder.Append(this.ConfigWriteScope.ToString());
				stringBuilder.Append(", ");
			}
			if (this.objectSpecificConfigWriteScopes != null)
			{
				foreach (KeyValuePair<string, IList<ADScopeCollection>> keyValuePair in this.objectSpecificConfigWriteScopes)
				{
					foreach (ADScopeCollection adscopeCollection2 in keyValuePair.Value)
					{
						stringBuilder.Append(adscopeCollection2.ToString());
						stringBuilder.Append(", ");
					}
				}
			}
			stringBuilder.Append("}");
			stringBuilder.Append(", ");
			stringBuilder.Append(DirectoryStrings.ExclusiveRecipientScopes);
			stringBuilder.Append("{");
			if (this.ExclusiveRecipientScopes != null)
			{
				stringBuilder.Append(this.ExclusiveRecipientScopes.ToString());
			}
			stringBuilder.Append("}");
			stringBuilder.Append(", ");
			stringBuilder.Append(DirectoryStrings.ExclusiveConfigScopes);
			stringBuilder.Append("{");
			if (this.objectSpecificExclusiveConfigWriteScopes != null)
			{
				foreach (KeyValuePair<string, ADScopeCollection> keyValuePair2 in this.objectSpecificExclusiveConfigWriteScopes)
				{
					stringBuilder.Append(keyValuePair2.Value.ToString());
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append("}");
			return new LocalizedString(stringBuilder.ToString());
		}

		private ADScope recipientReadScope;

		private ReadOnlyCollection<ADScopeCollection> recipientWriteScopes;

		private ADScopeCollection exclusiveRecipientScopes;

		private ADScope configReadScope;

		private ADScope configWriteScope;

		private Dictionary<string, IList<ADScopeCollection>> objectSpecificConfigWriteScopes;

		private Dictionary<string, ADScopeCollection> objectSpecificExclusiveConfigWriteScopes;

		private IList<ValidationRule> validationRules;
	}
}
