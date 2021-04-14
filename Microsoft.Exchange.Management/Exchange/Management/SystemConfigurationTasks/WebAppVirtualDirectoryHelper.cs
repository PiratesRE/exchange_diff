using System;
using System.Collections;
using System.DirectoryServices;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class WebAppVirtualDirectoryHelper
	{
		protected WebAppVirtualDirectoryHelper()
		{
		}

		internal static void CopyMetabaseProperties(ExchangeWebAppVirtualDirectory target, ExchangeWebAppVirtualDirectory source)
		{
			target.DefaultDomain = source.DefaultDomain;
			target.FormsAuthentication = source.FormsAuthentication;
			target.BasicAuthentication = source.BasicAuthentication;
			target.DigestAuthentication = source.DigestAuthentication;
			target.WindowsAuthentication = source.WindowsAuthentication;
			target.LiveIdAuthentication = source.LiveIdAuthentication;
			target.AdfsAuthentication = source.AdfsAuthentication;
			target.GzipLevel = source.GzipLevel;
			target.WebSite = source.WebSite;
		}

		internal static T FindWebAppVirtualDirectoryInSameWebSite<T>(ExchangeWebAppVirtualDirectory source, IConfigDataProvider dataProvider) where T : ExchangeWebAppVirtualDirectory, new()
		{
			T result = default(T);
			IConfigurable[] array = dataProvider.Find<T>(null, source.Server, true, null);
			if (array != null)
			{
				foreach (ExchangeWebAppVirtualDirectory exchangeWebAppVirtualDirectory in array)
				{
					if (IisUtility.Exists(exchangeWebAppVirtualDirectory.MetabasePath))
					{
						WebAppVirtualDirectoryHelper.UpdateFromMetabase(exchangeWebAppVirtualDirectory);
						if (string.Equals(source.WebSite, exchangeWebAppVirtualDirectory.WebSite, StringComparison.OrdinalIgnoreCase))
						{
							result = (T)((object)exchangeWebAppVirtualDirectory);
							break;
						}
					}
				}
			}
			return result;
		}

		internal static void CheckTwoWebAppVirtualDirectories(ExchangeWebAppVirtualDirectory first, ExchangeWebAppVirtualDirectory second, Action<string> WriteWarning, LocalizedString authMethedNotMatch, LocalizedString urlNotMatch)
		{
			if ((first.IsModified(ExchangeWebAppVirtualDirectorySchema.BasicAuthentication) || first.IsModified(ExchangeWebAppVirtualDirectorySchema.DigestAuthentication) || first.IsModified(ExchangeWebAppVirtualDirectorySchema.FormsAuthentication) || first.IsModified(ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication) || first.IsModified(ExchangeWebAppVirtualDirectorySchema.WindowsAuthentication)) && (first.BasicAuthentication != second.BasicAuthentication || first.DigestAuthentication != second.DigestAuthentication || first.FormsAuthentication != second.FormsAuthentication || first.LiveIdAuthentication != second.LiveIdAuthentication || first.WindowsAuthentication != second.WindowsAuthentication))
			{
				WriteWarning(authMethedNotMatch);
			}
			if ((first.IsModified(ADVirtualDirectorySchema.InternalUrl) && !WebAppVirtualDirectoryHelper.IsUrlConsistent(first.InternalUrl, second.InternalUrl)) || (first.IsModified(ADVirtualDirectorySchema.ExternalUrl) && !WebAppVirtualDirectoryHelper.IsUrlConsistent(first.ExternalUrl, second.ExternalUrl)))
			{
				WriteWarning(urlNotMatch);
			}
		}

		private static bool IsUrlConsistent(Uri url1, Uri url2)
		{
			if (url1 != null && url2 != null)
			{
				return url1.Scheme == url2.Scheme && url1.Host == url2.Host && url1.Port == url2.Port;
			}
			return url1 == url2;
		}

		internal static void UpdateFromMetabase(ExchangeWebAppVirtualDirectory webAppVirtualDirectory)
		{
			try
			{
				DirectoryEntry directoryEntry2;
				DirectoryEntry directoryEntry = directoryEntry2 = IisUtility.CreateIISDirectoryEntry(webAppVirtualDirectory.MetabasePath);
				try
				{
					MetabaseProperty[] properties = IisUtility.GetProperties(directoryEntry);
					webAppVirtualDirectory.DefaultDomain = (string)IisUtility.GetIisPropertyValue("DefaultLogonDomain", properties);
					webAppVirtualDirectory[ExchangeWebAppVirtualDirectorySchema.FormsAuthentication] = IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Fba);
					webAppVirtualDirectory[ExchangeWebAppVirtualDirectorySchema.BasicAuthentication] = IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic);
					webAppVirtualDirectory[ExchangeWebAppVirtualDirectorySchema.DigestAuthentication] = IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Digest);
					webAppVirtualDirectory[ExchangeWebAppVirtualDirectorySchema.WindowsAuthentication] = IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Ntlm);
					if (!IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.None))
					{
						webAppVirtualDirectory[ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication] = false;
					}
					webAppVirtualDirectory.DisplayName = directoryEntry.Name;
					webAppVirtualDirectory.WebSite = IisUtility.GetWebSiteName(directoryEntry.Parent.Path);
				}
				finally
				{
					if (directoryEntry2 != null)
					{
						((IDisposable)directoryEntry2).Dispose();
					}
				}
				webAppVirtualDirectory.GzipLevel = Gzip.GetGzipLevel(webAppVirtualDirectory.MetabasePath);
			}
			catch (IISGeneralCOMException ex)
			{
				if (ex.Code == -2147023174)
				{
					throw new IISNotReachableException(IisUtility.GetHostName(webAppVirtualDirectory.MetabasePath), ex.Message);
				}
				throw;
			}
		}

		internal static void UpdateMetabase(ExchangeWebAppVirtualDirectory webAppVirtualDirectory, string metabasePath, bool enableAnonymous)
		{
			try
			{
				DirectoryEntry directoryEntry2;
				DirectoryEntry directoryEntry = directoryEntry2 = IisUtility.CreateIISDirectoryEntry(webAppVirtualDirectory.MetabasePath);
				try
				{
					ArrayList arrayList = new ArrayList();
					if (webAppVirtualDirectory.DefaultDomain.Length > 0)
					{
						arrayList.Add(new MetabaseProperty("DefaultLogonDomain", webAppVirtualDirectory.DefaultDomain, true));
					}
					else if (webAppVirtualDirectory.DefaultDomain == "")
					{
						directoryEntry.Properties["DefaultLogonDomain"].Clear();
					}
					IisUtility.SetProperties(directoryEntry, arrayList);
					directoryEntry.CommitChanges();
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.None, true);
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic, webAppVirtualDirectory.BasicAuthentication);
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Digest, webAppVirtualDirectory.DigestAuthentication);
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.WindowsIntegrated, webAppVirtualDirectory.WindowsAuthentication);
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.LiveIdFba, webAppVirtualDirectory.LiveIdAuthentication);
					if (webAppVirtualDirectory.FormsAuthentication)
					{
						OwaIsapiFilter.EnableFba(directoryEntry);
					}
					else
					{
						OwaIsapiFilter.DisableFba(directoryEntry);
					}
					IisUtility.SetAuthenticationMethod(directoryEntry, MetabasePropertyTypes.AuthFlags.Anonymous, enableAnonymous);
					directoryEntry.CommitChanges();
				}
				finally
				{
					if (directoryEntry2 != null)
					{
						((IDisposable)directoryEntry2).Dispose();
					}
				}
				GzipLevel gzipLevel = webAppVirtualDirectory.GzipLevel;
				string site = IisUtility.WebSiteFromMetabasePath(webAppVirtualDirectory.MetabasePath);
				Gzip.SetIisGzipLevel(site, GzipLevel.High);
				Gzip.SetVirtualDirectoryGzipLevel(webAppVirtualDirectory.MetabasePath, gzipLevel);
			}
			catch (IISGeneralCOMException ex)
			{
				if (ex.Code == -2147023174)
				{
					throw new IISNotReachableException(IisUtility.GetHostName(webAppVirtualDirectory.MetabasePath), ex.Message);
				}
				throw;
			}
		}
	}
}
