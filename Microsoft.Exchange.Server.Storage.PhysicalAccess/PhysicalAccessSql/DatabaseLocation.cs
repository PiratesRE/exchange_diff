using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	internal static class DatabaseLocation
	{
		public static string GetConnectionString(string databaseName)
		{
			databaseName = databaseName.Trim();
			databaseName = databaseName.Replace(';', '_');
			databaseName = databaseName.Replace('[', '_');
			databaseName = databaseName.Replace(']', '_');
			databaseName = databaseName.Replace("--", "_");
			databaseName = databaseName.Substring(0, (databaseName.Length > 128) ? 128 : databaseName.Length);
			return string.Format("Trusted_Connection=yes;Integrated Security=SSPI;packet size=4096;Data Source={0}", ".") + string.Format(";Database={0}", databaseName) + ";Application Name='Exchange';Connect Timeout=6000;Max Pool Size=1000;MultipleActiveResultSets=true";
		}

		public static string GetMasterConnectionString()
		{
			return DatabaseLocation.GetConnectionString("master");
		}

		private const string DefaultDataSource = ".";

		private const string ConnectionStringFormat = "Trusted_Connection=yes;Integrated Security=SSPI;packet size=4096;Data Source={0}";

		private const string DatabaseStringFormat = ";Database={0}";

		private const string DefaultMasterDatabase = "master";
	}
}
