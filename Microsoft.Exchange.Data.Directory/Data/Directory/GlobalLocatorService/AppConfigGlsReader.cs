using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class AppConfigGlsReader : IGlobalLocatorServiceReader
	{
		public static bool AppConfigOverrideExists()
		{
			return ConfigurationManager.AppSettings["GlsFindDomainOverride"] != null;
		}

		public AppConfigGlsReader()
		{
			AppConfigGlsReader.ParseAndConstructResultObjects(ConfigurationManager.AppSettings["GlsFindDomainOverride"], out this.findDomainResult, out this.findDomainsResult, out this.findTenantResult);
		}

		public bool TenantExists(Guid tenantId, Namespace[] ns)
		{
			throw new NotImplementedException();
		}

		public bool DomainExists(SmtpDomain domain, Namespace[] ns)
		{
			throw new NotImplementedException();
		}

		public FindTenantResult FindTenant(Guid tenantId, TenantProperty[] tenantProperties)
		{
			return this.findTenantResult;
		}

		public FindDomainResult FindDomain(SmtpDomain domain, DomainProperty[] domainProperties, TenantProperty[] tenantProperties)
		{
			return this.findDomainResult;
		}

		public FindDomainsResult FindDomains(SmtpDomain[] domains, DomainProperty[] domainProperties, TenantProperty[] tenantProperties)
		{
			return this.findDomainsResult;
		}

		public IAsyncResult BeginTenantExists(Guid tenantId, Namespace[] ns, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginDomainExists(SmtpDomain domain, Namespace[] ns, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginFindTenant(Guid tenantId, TenantProperty[] tenantProperties, AsyncCallback callback, object asyncState)
		{
			return AppConfigGlsReader.BeginAsyncOperation(callback, asyncState);
		}

		public IAsyncResult BeginFindDomain(SmtpDomain domain, DomainProperty[] domainProperties, TenantProperty[] tenantProperties, AsyncCallback callback, object asyncState)
		{
			return AppConfigGlsReader.BeginAsyncOperation(callback, asyncState);
		}

		public IAsyncResult BeginFindDomains(SmtpDomain[] domains, DomainProperty[] domainProperties, TenantProperty[] tenantProperties, AsyncCallback callback, object asyncState)
		{
			return AppConfigGlsReader.BeginAsyncOperation(callback, asyncState);
		}

		public bool EndTenantExists(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public bool EndDomainExists(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public FindTenantResult EndFindTenant(IAsyncResult asyncResult)
		{
			return this.findTenantResult;
		}

		public FindDomainResult EndFindDomain(IAsyncResult asyncResult)
		{
			return this.findDomainResult;
		}

		public FindDomainsResult EndFindDomains(IAsyncResult asyncResult)
		{
			return this.findDomainsResult;
		}

		private static IAsyncResult BeginAsyncOperation(AsyncCallback callback, object asyncState)
		{
			AsyncResult asyncResult = new AsyncResult(callback, asyncState);
			if (callback != null)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(AppConfigGlsReader.CallbackCallerThreadStart), new Tuple<AsyncCallback, IAsyncResult>(callback, asyncResult));
			}
			return asyncResult;
		}

		private static void CallbackCallerThreadStart(object state)
		{
			Tuple<AsyncCallback, IAsyncResult> tuple = (Tuple<AsyncCallback, IAsyncResult>)state;
			AsyncCallback item = tuple.Item1;
			IAsyncResult item2 = tuple.Item2;
			item(item2);
		}

		private static void ParseAndConstructResultObjects(string rawAppConfigValue, out FindDomainResult findDomainResult, out FindDomainsResult findDomainsResult, out FindTenantResult findTenantResult)
		{
			if (string.IsNullOrEmpty(rawAppConfigValue))
			{
				throw new ArgumentNullException("rawAppConfigValue");
			}
			char[] separator = new char[]
			{
				' '
			};
			char[] separator2 = new char[]
			{
				':'
			};
			char[] separator3 = new char[]
			{
				'='
			};
			char[] separator4 = new char[]
			{
				','
			};
			char[] trimChars = new char[]
			{
				'[',
				']'
			};
			string[] array = rawAppConfigValue.Split(separator);
			string text = array[0];
			string text2 = array[1];
			string text3 = array[2];
			string[] array2 = text.Split(separator2);
			string[] array3 = text2.Split(separator2);
			string[] array4 = text3.Split(separator2);
			string text4 = array2[0];
			string text5 = array3[0];
			string text6 = array4[0];
			string g = array2[1];
			string text7 = array3[1];
			string text8 = array4[1];
			AppConfigGlsReader.Assert(text4.Equals("TenantId", StringComparison.OrdinalIgnoreCase), "incorrect key name for TenantId");
			AppConfigGlsReader.Assert(text5.Equals("TenantProperties", StringComparison.OrdinalIgnoreCase), "incorrect key name for TenantProperties");
			AppConfigGlsReader.Assert(text6.Equals("DomainProperties", StringComparison.OrdinalIgnoreCase), "incorrect key name for DomainProperties");
			Guid tenantId = new Guid(g);
			IDictionary<DomainProperty, PropertyValue> dictionary = new Dictionary<DomainProperty, PropertyValue>();
			IDictionary<TenantProperty, PropertyValue> dictionary2 = new Dictionary<TenantProperty, PropertyValue>();
			text7 = text7.Trim(trimChars);
			text8 = text8.Trim(trimChars);
			if (text7 != string.Empty)
			{
				string[] array5 = text7.Split(separator4);
				foreach (string text9 in array5)
				{
					string[] array7 = text9.Split(separator3);
					string name = array7[0];
					string rawStringValue = array7[1];
					TenantProperty tenantProperty = TenantProperty.Get(name);
					dictionary2.Add(tenantProperty, PropertyValue.Create(rawStringValue, tenantProperty));
				}
			}
			if (text8 != string.Empty)
			{
				string[] array8 = text8.Split(separator4);
				foreach (string text10 in array8)
				{
					string[] array10 = text10.Split(separator3);
					string name2 = array10[0];
					string rawStringValue2 = array10[1];
					DomainProperty domainProperty = DomainProperty.Get(name2);
					dictionary.Add(domainProperty, PropertyValue.Create(rawStringValue2, domainProperty));
				}
			}
			findDomainResult = new FindDomainResult("domainName", tenantId, dictionary2, dictionary);
			findDomainsResult = new FindDomainsResult(new FindDomainResult[]
			{
				findDomainResult
			});
			findTenantResult = new FindTenantResult(dictionary2);
		}

		private static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				throw new FormatException(message);
			}
		}

		private const string AppConfigOverrideKeyName = "GlsFindDomainOverride";

		private FindDomainResult findDomainResult;

		private FindDomainsResult findDomainsResult;

		private FindTenantResult findTenantResult;
	}
}
