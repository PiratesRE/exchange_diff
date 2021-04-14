using System;
using System.Text;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class AIBreadcrumbs
	{
		private AIBreadcrumbs()
		{
		}

		public string GenerateReport()
		{
			StringBuilder stringBuilder = new StringBuilder(128 * AIBreadcrumbs.allBreadcrumbTrails.Length * 128);
			stringBuilder.AppendLine(string.Format("AI Breadcrumbs. Current Local Time: {0} Current UTC Time: {1}", DateTime.UtcNow.ToLocalTime(), DateTime.UtcNow));
			foreach (BreadcrumbsTrail breadcrumbsTrail in AIBreadcrumbs.allBreadcrumbTrails)
			{
				stringBuilder.AppendLine("Breadcrumbs for " + breadcrumbsTrail.Name);
				stringBuilder.AppendLine(breadcrumbsTrail.ToString());
			}
			return stringBuilder.ToString();
		}

		public static readonly BreadcrumbsTrail ShutdownTrail = new BreadcrumbsTrail("ShutdownTrail", TrailLength.Long);

		public static readonly BreadcrumbsTrail StartupTrail = new BreadcrumbsTrail("StartupTrail", TrailLength.Long);

		public static readonly BreadcrumbsTrail StatusTrail = new BreadcrumbsTrail("StatusTrail", TrailLength.Short);

		public static readonly BreadcrumbsTrail DatabaseStatusTrail = new BreadcrumbsTrail("DatabaseStatusTrail", TrailLength.Short);

		public static readonly AIBreadcrumbs Instance = new AIBreadcrumbs();

		private static BreadcrumbsTrail[] allBreadcrumbTrails = new BreadcrumbsTrail[]
		{
			AIBreadcrumbs.StartupTrail,
			AIBreadcrumbs.DatabaseStatusTrail,
			AIBreadcrumbs.ShutdownTrail,
			AIBreadcrumbs.StatusTrail
		};
	}
}
