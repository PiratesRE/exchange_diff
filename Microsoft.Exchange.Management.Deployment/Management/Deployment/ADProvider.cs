using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class ADProvider : IADDataProvider
	{
		internal ADProvider(int sizeLimit, int pageSize, TimeSpan serverTimeLimit)
		{
			if (0 > pageSize || 0 > sizeLimit)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.SizeLimit = sizeLimit;
			this.PageSize = pageSize;
			this.ServerTimeLimit = serverTimeLimit;
		}

		public int SizeLimit { get; set; }

		public int PageSize { get; set; }

		public TimeSpan ServerTimeLimit { get; private set; }

		public SearchResultCollection Run(bool useGC, string directoryEntry, string[] listOfPropertiesToCollect, string filter, SearchScope searchScope)
		{
			DirectorySearcher directorySearcher = new DirectorySearcher();
			SearchResultCollection result;
			try
			{
				directorySearcher.ReferralChasing = ReferralChasingOption.All;
				directorySearcher.SearchRoot = new DirectoryEntry(directoryEntry);
				directorySearcher.Filter = filter;
				directorySearcher.PropertyNamesOnly = false;
				directorySearcher.PropertiesToLoad.AddRange(listOfPropertiesToCollect);
				directorySearcher.ServerTimeLimit = this.ServerTimeLimit;
				directorySearcher.SearchScope = searchScope;
				directorySearcher.CacheResults = useGC;
				directorySearcher.SizeLimit = this.SizeLimit;
				directorySearcher.PageSize = this.PageSize;
				result = directorySearcher.FindAll();
			}
			catch (DirectoryServicesCOMException ex)
			{
				if (ex.ExtendedError != 8333 || (!ex.ExtendedErrorMessage.StartsWith("0000208D: NameErr: DSID-0310020A, problem 2001 (NO_OBJECT), data 0, best match of:") && !ex.ExtendedErrorMessage.StartsWith("0000208D: NameErr: DSID-03100213, problem 2001 (NO_OBJECT), data 0, best match of:")))
				{
					throw;
				}
				result = null;
			}
			catch (COMException ex2)
			{
				if (!ex2.Message.StartsWith("Unknown error (0x80005000)"))
				{
					throw;
				}
				result = null;
			}
			finally
			{
				directorySearcher.Dispose();
			}
			return result;
		}

		public List<string> Run(bool useGC, string directoryEntry)
		{
			DirectoryEntry directoryEntry2 = null;
			List<string> list = new List<string>();
			try
			{
				directoryEntry2 = new DirectoryEntry(directoryEntry);
				list.Add(directoryEntry2.Properties["configurationNamingContext"].Value.ToString());
				list.Add(directoryEntry2.Properties["rootDomainNamingContext"].Value.ToString());
				list.Add(directoryEntry2.Properties["schemaNamingContext"].Value.ToString());
				list.Add(directoryEntry2.Properties["isGlobalCatalogReady"].Value.ToString());
			}
			catch (COMException ex)
			{
				if (ex.Message.StartsWith("Unknown error (0x80005000)"))
				{
					return null;
				}
				throw;
			}
			finally
			{
				directoryEntry2.Dispose();
			}
			return list;
		}

		private const int ExtendedError = 8333;

		private const string ExtendedErrorMessage = "0000208D: NameErr: DSID-0310020A, problem 2001 (NO_OBJECT), data 0, best match of:";

		private const string ExtendedErrorMessageAlt = "0000208D: NameErr: DSID-03100213, problem 2001 (NO_OBJECT), data 0, best match of:";

		private const string ADNotInstalledErrorMessage = "Unknown error (0x80005000)";
	}
}
