using System;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class ResourceBase : DataSourceService
	{
		public PowerShellResults<O> GetObject<O>(Identity identity) where O : ResourceConfigurationBase
		{
			identity = Identity.FromExecutingUserId();
			return base.GetObject<O>("Get-CalendarProcessing", identity);
		}

		public PowerShellResults<O> SetObject<O, U>(Identity identity, U properties) where O : ResourceConfigurationBase where U : SetResourceConfigurationBase
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<O> result;
			lock (RbacPrincipal.Current.OwaOptionsLock)
			{
				result = base.SetObject<O, U>("Set-CalendarProcessing", identity, properties);
			}
			return result;
		}

		internal const string GetCmdlet = "Get-CalendarProcessing";

		internal const string SetCmdlet = "Set-CalendarProcessing";

		internal const string Resource = "Resource+";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		internal const string GetObjectRole = "Resource+Get-CalendarProcessing?Identity@R:Self";

		internal const string SetObjectRole = "Resource+Get-CalendarProcessing?Identity@R:Self+Set-CalendarProcessing?Identity@W:Self";
	}
}
