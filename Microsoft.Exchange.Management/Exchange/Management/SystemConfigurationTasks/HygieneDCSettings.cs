using System;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class HygieneDCSettings
	{
		public static bool GetFfoDCPublicIPAddresses(out MultiValuedProperty<IPRange> ffoIPs)
		{
			ffoIPs = null;
			IConfigurable obj = null;
			if (HygieneDCSettings.InvokeFindDCSettingsMethod(out obj))
			{
				ffoIPs = (HygieneDCSettings.ffoDCPublicIPsProperty.GetValue(obj, null) as MultiValuedProperty<IPRange>);
				return true;
			}
			return false;
		}

		public static bool GetSettings(out MultiValuedProperty<IPRange> ffoIPs, out MultiValuedProperty<SmtpX509IdentifierEx> ffoSmtpCerts, out MultiValuedProperty<ServiceProviderSettings> serviceProviderSettings)
		{
			ffoIPs = null;
			ffoSmtpCerts = null;
			serviceProviderSettings = null;
			IConfigurable obj = null;
			if (HygieneDCSettings.InvokeFindDCSettingsMethod(out obj))
			{
				ffoIPs = (HygieneDCSettings.ffoDCPublicIPsProperty.GetValue(obj, null) as MultiValuedProperty<IPRange>);
				ffoSmtpCerts = (HygieneDCSettings.ffoFrontDoorSmtpCertificatesProperty.GetValue(obj, null) as MultiValuedProperty<SmtpX509IdentifierEx>);
				serviceProviderSettings = (HygieneDCSettings.serviceProvidersProperty.GetValue(obj, null) as MultiValuedProperty<ServiceProviderSettings>);
				return true;
			}
			return false;
		}

		private static bool InvokeFindDCSettingsMethod(out IConfigurable dcSettings)
		{
			dcSettings = null;
			HygieneDCSettings.BuildMethodsAndProperties();
			try
			{
				MethodBase methodBase = HygieneDCSettings.findDCSettingsMethod;
				object obj = HygieneDCSettings.globalConfigSession;
				object[] array = new object[4];
				array[2] = false;
				IConfigurable[] array2 = methodBase.Invoke(obj, array) as IConfigurable[];
				if (array2 != null)
				{
					dcSettings = array2.FirstOrDefault<IConfigurable>();
				}
				if (dcSettings == null)
				{
					TaskLogger.LogError(new Exception("Find<DataCenterSettings> method returned empty result"));
					return false;
				}
			}
			catch (TargetInvocationException ex)
			{
				TaskLogger.LogError(ex.InnerException);
				return false;
			}
			return true;
		}

		private static void BuildMethodsAndProperties()
		{
			if (HygieneDCSettings.globalConfigSession == null)
			{
				Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.Data");
				Type type = assembly.GetType("Microsoft.Exchange.Hygiene.Data.GlobalConfig.GlobalSystemConfigSession");
				MethodInfo method = type.GetMethod("Find", BindingFlags.Instance | BindingFlags.NonPublic);
				Type type2 = assembly.GetType("Microsoft.Exchange.Hygiene.Data.GlobalConfig.DataCenterSettings");
				HygieneDCSettings.findDCSettingsMethod = method.MakeGenericMethod(new Type[]
				{
					type2
				});
				HygieneDCSettings.ffoDCPublicIPsProperty = type2.GetProperty("FfoDataCenterPublicIPAddresses", BindingFlags.Instance | BindingFlags.Public);
				HygieneDCSettings.ffoFrontDoorSmtpCertificatesProperty = type2.GetProperty("FfoFrontDoorSmtpCertificates", BindingFlags.Instance | BindingFlags.Public);
				HygieneDCSettings.serviceProvidersProperty = type2.GetProperty("ServiceProviders", BindingFlags.Instance | BindingFlags.Public);
				HygieneDCSettings.globalConfigSession = Activator.CreateInstance(type, true);
			}
		}

		private static object globalConfigSession;

		private static MethodInfo findDCSettingsMethod;

		private static PropertyInfo ffoDCPublicIPsProperty;

		private static PropertyInfo ffoFrontDoorSmtpCertificatesProperty;

		private static PropertyInfo serviceProvidersProperty;
	}
}
