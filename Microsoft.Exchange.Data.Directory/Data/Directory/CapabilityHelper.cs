using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class CapabilityHelper
	{
		internal static Capability? GetSKUCapability(MultiValuedProperty<Capability> capabilities)
		{
			foreach (Capability capability in capabilities)
			{
				if (CapabilityHelper.RootSKUCapabilities.Contains(capability))
				{
					return new Capability?(capability);
				}
			}
			return null;
		}

		internal static string TransformCapabilityString(string capabilityString)
		{
			if (CapabilityHelper.ffoOffers.Contains(capabilityString.ToUpper()))
			{
				return "BPOS_S_EopStandardAddOn";
			}
			if (CapabilityHelper.ffoPremiumOffers.Contains(capabilityString.ToUpper()))
			{
				return "BPOS_S_EopPremiumAddOn";
			}
			return capabilityString;
		}

		internal static void SetSKUCapability(Capability? skuCapability, MultiValuedProperty<Capability> capabilities)
		{
			if (skuCapability != null && !CapabilityHelper.RootSKUCapabilities.Contains(skuCapability.Value))
			{
				throw new ArgumentOutOfRangeException("skuCapability", skuCapability.Value, DirectoryStrings.ExArgumentOutOfRangeException("skuCapability", skuCapability.Value));
			}
			if (skuCapability != null && capabilities.Contains(skuCapability.Value))
			{
				return;
			}
			if (capabilities.Count > 0)
			{
				foreach (Capability item in CapabilityHelper.RootSKUCapabilities)
				{
					capabilities.Remove(item);
					if (capabilities.Count == 0)
					{
						break;
					}
				}
			}
			if (skuCapability != null)
			{
				capabilities.Add(skuCapability.Value);
			}
		}

		internal static void SetSKUCapabilities(string sourceName, MultiValuedProperty<Capability> sourceCapabilities, MultiValuedProperty<Capability> targetCapabilities)
		{
			List<Capability> rootSKUCapabilities = CapabilityHelper.GetRootSKUCapabilities(sourceCapabilities);
			CapabilityHelper.SetSKUCapability((rootSKUCapabilities.Count == 0) ? null : new Capability?(rootSKUCapabilities[0]), targetCapabilities);
			foreach (Capability item in sourceCapabilities)
			{
				if (CapabilityHelper.AddOnSKUCapabilities.Contains(item) && !targetCapabilities.Contains(item))
				{
					targetCapabilities.Add(item);
				}
			}
		}

		internal static void SetAddOnSKUCapabilities(MultiValuedProperty<Capability> sourceCapabilities, MultiValuedProperty<Capability> targetCapabilities)
		{
			if (!sourceCapabilities.All((Capability c) => CapabilityHelper.IsAddOnSKUCapability(c)))
			{
				throw new ArgumentOutOfRangeException("sourceCapabilities", sourceCapabilities, DirectoryStrings.ExArgumentOutOfRangeException("sourceCapabilities", sourceCapabilities));
			}
			foreach (Capability item in from c in CapabilityHelper.AddOnSKUCapabilities
			where targetCapabilities.Contains(c)
			select c)
			{
				targetCapabilities.Remove(item);
			}
			foreach (Capability item2 in sourceCapabilities)
			{
				targetCapabilities.Add(item2);
			}
		}

		internal static void SetTenantSKUCapabilities(MultiValuedProperty<Capability> sourceCapabilities, MultiValuedProperty<Capability> targetCapabilities)
		{
			if (!sourceCapabilities.All((Capability c) => CapabilityHelper.IsAllowedSKUCapability(c)))
			{
				throw new ArgumentOutOfRangeException("sourceCapabilities", sourceCapabilities, DirectoryStrings.ExArgumentOutOfRangeException("sourceCapabilities", sourceCapabilities));
			}
			foreach (Capability item in from c in CapabilityHelper.AllowedSKUCapabilities
			where targetCapabilities.Contains(c)
			select c)
			{
				targetCapabilities.Remove(item);
			}
			foreach (Capability item2 in sourceCapabilities)
			{
				targetCapabilities.Add(item2);
			}
		}

		internal static bool HasBposSKUCapability(MultiValuedProperty<Capability> capabilities)
		{
			return CapabilityHelper.GetSKUCapability(capabilities) != null;
		}

		internal static bool AllowMailboxLogon(Capability? skuCapability, bool? skuAssigned, DateTime? whenMailboxCreated)
		{
			if (skuCapability != null && CapabilityHelper.IsFreeSkuCapability(skuCapability.Value))
			{
				return true;
			}
			if (skuAssigned != null)
			{
				return skuAssigned.Value;
			}
			return CapabilityHelper.IsWithinGracePeriod(whenMailboxCreated);
		}

		private static List<Capability> GetRootSKUCapabilities(MultiValuedProperty<Capability> capabilities)
		{
			List<Capability> list = new List<Capability>();
			foreach (Capability item in capabilities)
			{
				if (CapabilityHelper.RootSKUCapabilities.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		internal static Capability[] AllowedSKUCapabilities
		{
			get
			{
				if (CapabilityHelper.allowedSKUCapabilities == null)
				{
					CapabilityHelper.allowedSKUCapabilities = CapabilityHelper.AllowedSKUCapabilitiesList.ToArray();
				}
				return CapabilityHelper.allowedSKUCapabilities;
			}
		}

		internal static bool IsAllowedSKUCapability(Capability capability)
		{
			return CapabilityHelper.AllowedSKUCapabilitiesList.Contains(capability);
		}

		internal static bool IsRootSKUCapability(Capability capability)
		{
			return CapabilityHelper.RootSKUCapabilities.Contains(capability);
		}

		internal static bool IsAddOnSKUCapability(Capability capability)
		{
			return CapabilityHelper.AddOnSKUCapabilities.Contains(capability);
		}

		internal static Guid GetSKUCapabilityGuid(Capability capability)
		{
			if (CapabilityHelper.SkuCapabilityGuidMap.ContainsKey(capability))
			{
				return CapabilityHelper.SkuCapabilityGuidMap[capability];
			}
			return Guid.Empty;
		}

		internal static bool GetIsLicensingEnforcedInOrg(OrganizationId organizationId)
		{
			OrganizationProperties organizationProperties;
			return !OrganizationPropertyCache.TryGetOrganizationProperties(organizationId, out organizationProperties) || organizationProperties.IsLicensingEnforced;
		}

		private static bool IsFreeSkuCapability(Capability skuCapability)
		{
			return CapabilityHelper.FreeSKUCapabilities.Contains(skuCapability);
		}

		private static bool IsWithinGracePeriod(DateTime? whenMailboxCreated)
		{
			return whenMailboxCreated != null && DateTime.UtcNow < whenMailboxCreated.Value + CapabilityHelper.gracePeriod;
		}

		private static IDictionary<Capability, Guid> SkuCapabilityGuidMap { get; set; } = new Dictionary<Capability, Guid>();

		private static List<Capability> AllowedSKUCapabilitiesList { get; set; } = new List<Capability>();

		private static List<Capability> RootSKUCapabilities { get; set; } = new List<Capability>();

		private static List<Capability> AddOnSKUCapabilities { get; set; } = new List<Capability>();

		private static List<Capability> FreeSKUCapabilities { get; set; } = new List<Capability>();

		static CapabilityHelper()
		{
			foreach (FieldInfo fieldInfo in typeof(Capability).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField))
			{
				SKUCapabilityAttribute[] array = (SKUCapabilityAttribute[])fieldInfo.GetCustomAttributes(typeof(SKUCapabilityAttribute), false);
				if (array != null && array.Length > 0)
				{
					Capability capability = (Capability)fieldInfo.GetValue(null);
					CapabilityHelper.SkuCapabilityGuidMap[capability] = array[0].Guid;
					CapabilityHelper.AllowedSKUCapabilitiesList.Add(capability);
					if (array[0].AddOnSKU)
					{
						CapabilityHelper.AddOnSKUCapabilities.Add(capability);
					}
					else
					{
						CapabilityHelper.RootSKUCapabilities.Add(capability);
					}
					if (array[0].Free)
					{
						CapabilityHelper.FreeSKUCapabilities.Add(capability);
					}
				}
			}
		}

		internal const string ParameterSKUCapability = "SKUCapability";

		internal const string ParameterAddOnSKUCapability = "AddOnSKUCapability";

		internal const string ParameterTenantSKUCapability = "TenantSKUCapability";

		internal static readonly string[] ffoOffers = new string[]
		{
			"EXCHANGE ONLINE PROTECTION ENTERPRISE SUBSCRIPTION"
		};

		internal static readonly string[] ffoPremiumOffers = new string[]
		{
			"EXCHANGE ONLINE PROTECTION ENTERPRISE PREMIUM SUBSCRIPTION"
		};

		private static Capability[] allowedSKUCapabilities;

		private static readonly TimeSpan gracePeriod = TimeSpan.FromDays(30.0);
	}
}
