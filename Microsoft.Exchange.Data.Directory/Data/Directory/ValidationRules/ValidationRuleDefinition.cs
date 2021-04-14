using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class ValidationRuleDefinition
	{
		public ValidationRuleDefinition(string name, string feature, ValidationRuleSkus applicableSku, List<RoleEntry> applicableRoleEntries, List<Capability> restrictedCapabilities, List<Capability> overridingAllowCapabilities, ValidationErrorStringProvider errorStringProvider)
		{
			if (applicableRoleEntries == null)
			{
				throw new ArgumentNullException("applicableRoleEntries");
			}
			if (restrictedCapabilities == null)
			{
				throw new ArgumentNullException("restrictedCapabilities");
			}
			if (overridingAllowCapabilities == null)
			{
				throw new ArgumentNullException("overridingAllowCapabilities");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrEmpty(feature))
			{
				throw new ArgumentNullException("feature");
			}
			if (errorStringProvider == null)
			{
				throw new ArgumentNullException("errorStringProvider");
			}
			this.Name = name;
			this.Feature = feature;
			this.ApplicableSku = applicableSku;
			this.ApplicableRoleEntries = applicableRoleEntries;
			this.ErrorString = errorStringProvider;
			this.ApplicableRoleEntries.Sort(RoleEntryComparer.Instance);
			restrictedCapabilities.Sort();
			this.RestrictedCapabilities = new ReadOnlyCollection<Capability>(restrictedCapabilities);
			overridingAllowCapabilities.Sort();
			this.OverridingAllowCapabilities = new ReadOnlyCollection<Capability>(overridingAllowCapabilities);
		}

		public ValidationRuleDefinition(string name, string feature, ValidationRuleSkus applicableSku, List<RoleEntry> applicableRoleEntries, List<Capability> restrictedCapabilities, List<Capability> overridingAllowCapabilities, ValidationErrorStringProvider errorStringProvider, List<ValidationRuleExpression> expressions) : this(name, feature, applicableSku, applicableRoleEntries, restrictedCapabilities, overridingAllowCapabilities, errorStringProvider)
		{
			if (expressions == null)
			{
				throw new ArgumentNullException("expressions");
			}
			this.Expressions = expressions;
		}

		public string Name { get; private set; }

		public string Feature { get; private set; }

		public List<ValidationRuleExpression> Expressions { get; private set; }

		public List<RoleEntry> ApplicableRoleEntries { get; protected set; }

		public ReadOnlyCollection<Capability> RestrictedCapabilities { get; private set; }

		public ReadOnlyCollection<Capability> OverridingAllowCapabilities { get; private set; }

		public ValidationErrorStringProvider ErrorString { get; private set; }

		public ValidationRuleSkus ApplicableSku { get; private set; }

		public bool IsRuleApplicable(RoleEntry targetRoleEntry, out RoleEntry matchingRoleEntry)
		{
			if (null == targetRoleEntry)
			{
				throw new ArgumentNullException("targetRoleEntry");
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<RoleEntry, string>((long)this.GetHashCode(), "Entering ValidationRuleDefinition.IsRuleApplicable({0}). Rule name: '{1}'", targetRoleEntry, this.Name);
			matchingRoleEntry = null;
			int num = this.ApplicableRoleEntries.BinarySearch(targetRoleEntry, RoleEntry.NameComparer);
			if (num < 0)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<RoleEntry, string>((long)this.GetHashCode(), "ValidationRuleDefinition.IsRuleApplicable({0}) returns false. Rule name: '{1}'. Cmdlet not applicable.", targetRoleEntry, this.Name);
				return false;
			}
			RoleEntry roleEntry = this.ApplicableRoleEntries[num];
			matchingRoleEntry = roleEntry.IntersectParameters(targetRoleEntry);
			if (matchingRoleEntry.Parameters.Count > 0)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<RoleEntry, string>((long)this.GetHashCode(), "ValidationRuleDefinition.IsRuleApplicable({0}) returns true. Rule name: '{1}'. Parameters match.", targetRoleEntry, this.Name);
				return true;
			}
			if (targetRoleEntry.Parameters.Count == 0 && roleEntry.Parameters.Contains("_RestrictionDefinedForAllParameters"))
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<RoleEntry, string>((long)this.GetHashCode(), "ValidationRuleDefinition.IsRuleApplicable({0}) returns true. Rule name: '{1}'. No parameters specified but restriction defined for all parameters.", targetRoleEntry, this.Name);
				return true;
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<RoleEntry, string>((long)this.GetHashCode(), "ValidationRuleDefinition.IsRuleApplicable({0}) returns false. Rule name: '{1}'. Parameters doesn't match. ", targetRoleEntry, this.Name);
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				throw new NullReferenceException("obj");
			}
			ValidationRuleDefinition validationRuleDefinition = obj as ValidationRuleDefinition;
			if (validationRuleDefinition == null)
			{
				return false;
			}
			if (!this.Name.Equals(validationRuleDefinition.Name, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (validationRuleDefinition.RestrictedCapabilities.Count != this.RestrictedCapabilities.Count)
			{
				return false;
			}
			for (int i = 0; i < this.RestrictedCapabilities.Count<Capability>(); i++)
			{
				if (!validationRuleDefinition.RestrictedCapabilities[i].Equals(this.RestrictedCapabilities[i]))
				{
					return false;
				}
			}
			if (validationRuleDefinition.ApplicableRoleEntries.Count != this.ApplicableRoleEntries.Count)
			{
				return false;
			}
			for (int j = 0; j < this.ApplicableRoleEntries.Count; j++)
			{
				if (!this.ApplicableRoleEntries[j].Equals(validationRuleDefinition.ApplicableRoleEntries[j]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private const string SpecialParameterForRestrictionRequiredForAllParameters = "_RestrictionDefinedForAllParameters";
	}
}
