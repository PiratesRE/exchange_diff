using System;
using System.Threading;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class GlobalizeCultureScriptResource : GlobalizeScriptResource
	{
		public GlobalizeCultureScriptResource(ResourceTarget.Filter targetFilter, string currentOwaVersion) : base(null, targetFilter, currentOwaVersion, true)
		{
		}

		internal override string ResourceName
		{
			get
			{
				return string.Format("globalize.culture.{0}.js", Thread.CurrentThread.CurrentUICulture.Name);
			}
		}

		private const string GlobalizationScriptFile = "globalize.culture.{0}.js";
	}
}
