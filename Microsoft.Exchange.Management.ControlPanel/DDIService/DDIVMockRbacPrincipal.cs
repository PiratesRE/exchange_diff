using System;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	internal class DDIVMockRbacPrincipal : RbacPrincipal, IDisposable
	{
		public DDIVMockRbacPrincipal() : base(Activator.CreateInstance(typeof(DDIVMockedExchangeRunspaceConfiguration), true) as ExchangeRunspaceConfiguration, "MockRbacPrincipal")
		{
			HttpContext.Current = new HttpContext(new SimpleWorkerRequest("/", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), string.Empty, string.Empty, new StringWriter()));
			this.originalPrincipal = Thread.CurrentPrincipal;
			this.SetCurrentThreadPrincipal();
		}

		void IDisposable.Dispose()
		{
			Thread.CurrentPrincipal = this.originalPrincipal;
		}

		protected override bool IsInRole(string rbacQuery, out bool canCache, ADRawEntry adRawEntry)
		{
			canCache = true;
			return true;
		}

		private IPrincipal originalPrincipal;
	}
}
