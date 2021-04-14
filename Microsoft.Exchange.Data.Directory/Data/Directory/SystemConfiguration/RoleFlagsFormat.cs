using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class RoleFlagsFormat
	{
		internal static object GetRoleState(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ExchangeRoleSchema.RoleFlags];
			int num2 = num & 1;
			return (RoleState)num2;
		}

		internal static void SetRoleState(object value, IPropertyBag propertyBag)
		{
			RoleState roleState = (RoleState)value;
			int num = (int)propertyBag[ExchangeRoleSchema.RoleFlags];
			num |= 1;
			if (roleState == RoleState.Usable)
			{
				num &= -2;
			}
			propertyBag[ExchangeRoleSchema.RoleFlags] = num;
		}

		internal static object GetIsEndUserRole(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ExchangeRoleSchema.RoleFlags];
			int num2 = num >> 31 & 1;
			return num2 != 0;
		}

		internal static void SetIsEndUserRole(object value, IPropertyBag propertyBag)
		{
			uint num = ((bool)value) ? 1U : 0U;
			int num2 = (int)propertyBag[ExchangeRoleSchema.RoleFlags];
			propertyBag[ExchangeRoleSchema.RoleFlags] = ((num2 & int.MaxValue) | (int)((int)(num & 1U) << 31));
		}

		internal static QueryFilter IsEndUserRoleFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ExchangeRoleSchema.RoleFlags, (ulong)int.MinValue));
		}

		private static uint GetScopeBits(IPropertyBag propertyBag, ScopeLocation scopeLocation)
		{
			int num = (int)propertyBag[ExchangeRoleSchema.RoleFlags];
			return (uint)num >> RoleFlagsFormat.ScopeShifts[(int)scopeLocation] & 31U;
		}

		private static void SetScopeBits(uint valueToSet, IPropertyBag propertyBag, ScopeLocation scopeLocation)
		{
			int num = (int)propertyBag[ExchangeRoleSchema.RoleFlags];
			propertyBag[ExchangeRoleSchema.RoleFlags] = (int)(((ulong)num & (ulong)(~(31L << (RoleFlagsFormat.ScopeShifts[(int)scopeLocation] & 31)))) | (ulong)((ulong)(valueToSet & 31U) << RoleFlagsFormat.ScopeShifts[(int)scopeLocation]));
		}

		internal static GetterDelegate ScopeTypeGetterDelegate(ScopeLocation scopeLocation)
		{
			return (IPropertyBag propertyBag) => (ScopeType)RoleFlagsFormat.GetScopeBits(propertyBag, scopeLocation);
		}

		internal static SetterDelegate ScopeTypeSetterDelegate(ScopeLocation scopeLocation)
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				RoleFlagsFormat.SetScopeBits((uint)((ScopeType)value), propertyBag, scopeLocation);
			};
		}

		internal static object GetIsDeprecated(IPropertyBag propertyBag)
		{
			return RoleState.Deprecated.Equals(RoleFlagsFormat.GetRoleState(propertyBag));
		}

		internal const int IsEndUserRoleShift = 31;

		internal const int ScopeMask = 31;

		internal const int RoleStateMask = 1;

		internal const int IsEndUserRoleMask = 1;

		internal static readonly int[] ScopeShifts = new int[]
		{
			24,
			17,
			10,
			3
		};
	}
}
