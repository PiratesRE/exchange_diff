using System;
using System.Security;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	public static class Datacenter
	{
		public static bool IsLiveIDForExchangeLogin(bool wrapException)
		{
			return Datacenter.IsMicrosoftHostedOnly(wrapException);
		}

		public static bool IsMicrosoftHostedOnly(bool wrapException)
		{
			return Datacenter.IsFeatureEnabled(wrapException, new Func<bool>(DatacenterRegistry.IsMicrosoftHostedOnly));
		}

		public static bool TreatPreReqErrorsAsWarnings(bool wrapException)
		{
			return Datacenter.IsFeatureEnabled(wrapException, new Func<bool>(DatacenterRegistry.TreatPreReqErrorsAsWarnings));
		}

		public static bool IsDatacenterDedicated(bool wrapException)
		{
			return Datacenter.IsFeatureEnabled(wrapException, new Func<bool>(DatacenterRegistry.IsDatacenterDedicated));
		}

		public static bool IsPartnerHostedOnly(bool wrapException)
		{
			return Datacenter.IsFeatureEnabled(wrapException, new Func<bool>(DatacenterRegistry.IsPartnerHostedOnly));
		}

		public static bool IsRunningInExchangeDatacenter(bool defaultValue)
		{
			bool result = defaultValue;
			try
			{
				result = Datacenter.IsMicrosoftHostedOnly(true);
			}
			catch (CannotDetermineExchangeModeException)
			{
			}
			return result;
		}

		public static bool IsForefrontForOfficeDatacenter()
		{
			return DatacenterRegistry.IsForefrontForOffice();
		}

		public static bool IsGallatinDatacenter()
		{
			return DatacenterRegistry.IsGallatinDatacenter();
		}

		public static bool IsFFOGallatinDatacenter()
		{
			return DatacenterRegistry.IsFFOGallatinDatacenter();
		}

		public static bool IsDatacenterDedicated()
		{
			return DatacenterRegistry.IsDatacenterDedicated();
		}

		public static bool IsMultiTenancyEnabled()
		{
			try
			{
				if (Datacenter.IsPartnerHostedOnly(true))
				{
					return true;
				}
			}
			catch (CannotDetermineExchangeModeException)
			{
			}
			try
			{
				if (Datacenter.IsMicrosoftHostedOnly(true))
				{
					return true;
				}
			}
			catch (CannotDetermineExchangeModeException)
			{
			}
			return false;
		}

		public static Datacenter.ExchangeSku GetExchangeSku()
		{
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				return Datacenter.ExchangeSku.ExchangeDatacenter;
			}
			if (Datacenter.IsPartnerHostedOnly(true))
			{
				return Datacenter.ExchangeSku.PartnerHosted;
			}
			if (Datacenter.IsDatacenterDedicated(true))
			{
				return Datacenter.ExchangeSku.DatacenterDedicated;
			}
			return Datacenter.ExchangeSku.Enterprise;
		}

		private static bool IsFeatureEnabled(bool wrapException, Func<bool> feature)
		{
			bool result = false;
			if (wrapException)
			{
				Exception ex = null;
				try
				{
					result = feature();
				}
				catch (DatacenterInvalidRegistryException ex2)
				{
					ex = ex2;
				}
				catch (SecurityException ex3)
				{
					ex = ex3;
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					throw new CannotDetermineExchangeModeException(ex.Message, ex);
				}
			}
			else
			{
				result = feature();
			}
			return result;
		}

		public enum ExchangeSku
		{
			Enterprise,
			ExchangeDatacenter,
			PartnerHosted,
			ForefrontForOfficeDatacenter,
			DatacenterDedicated
		}
	}
}
