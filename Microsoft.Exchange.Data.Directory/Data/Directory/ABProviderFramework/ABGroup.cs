using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ABGroup : ABObject
	{
		public ABGroup(ABSession ownerSession) : base(ownerSession, ABGroup.allGroupPropertiesCollection)
		{
		}

		public override ABObjectSchema Schema
		{
			get
			{
				return ABGroup.schema;
			}
		}

		public ABObjectId OwnerId
		{
			get
			{
				return (ABObjectId)base[ABGroupSchema.OwnerId];
			}
		}

		public int? MembersCount
		{
			get
			{
				return (int?)base[ABGroupSchema.MembersCount];
			}
		}

		public bool? HiddenMembership
		{
			get
			{
				return (bool?)base[ABGroupSchema.HiddenMembership];
			}
		}

		protected virtual ABObjectId GetOwnerId()
		{
			return null;
		}

		protected virtual int? GetMembersCount()
		{
			return null;
		}

		protected virtual bool? GetHiddenMembership()
		{
			return null;
		}

		protected override bool InternalTryGetValue(ABPropertyDefinition property, out object value)
		{
			if (property == ABGroupSchema.OwnerId)
			{
				value = this.GetOwnerId();
				return true;
			}
			if (property == ABGroupSchema.MembersCount)
			{
				value = this.GetMembersCount();
				return true;
			}
			if (property == ABGroupSchema.HiddenMembership)
			{
				value = this.GetHiddenMembership();
				return true;
			}
			return base.InternalTryGetValue(property, out value);
		}

		private static ABGroupSchema schema = new ABGroupSchema();

		private static ABPropertyDefinitionCollection allGroupPropertiesCollection = ABPropertyDefinitionCollection.FromPropertyDefinitionCollection(ABGroup.schema.AllProperties);
	}
}
