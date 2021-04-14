using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Internal.ManagedWPP;

namespace Microsoft.Forefront.ActiveDirectoryConnector
{
	internal class ADHelpers
	{
		static ADHelpers()
		{
			Globals.InitializeSinglePerfCounterInstance();
		}

		public static ITopologyConfigurationSession CreateDefaultReadOnlyTopologyConfigurationSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 47, "CreateDefaultReadOnlyTopologyConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Filtering\\src\\platform\\Management\\ADConnector\\impl\\obj\\amd64\\adhelpers.cs");
		}

		public static IEnumerable<ADHelpers.ADRulePackageInfo> GetClassificationRulePackages(string rulePackageSetId, IEnumerable<string> rulePackageIds)
		{
			IConfigurationSession configSession = ADHelpers.GetConfigurationSession(rulePackageSetId);
			foreach (TransportRule rule in ADHelpers.GetRulePackages(configSession, rulePackageIds.ToArray<string>()))
			{
				yield return new ADHelpers.ADRulePackageInfo
				{
					RulePackageSetId = rulePackageSetId,
					RulePackageId = rule.Name,
					Xml = ADHelpers.DecodeClassificationRules(rule.ReplicationSignature),
					ModifiedDate = rule.WhenChangedUTC.Value
				};
			}
			yield break;
		}

		public static void ValidateRulePackageSetId(string rulePackageSetId)
		{
			ADHelpers.GetConfigurationSession(rulePackageSetId);
		}

		private static IConfigurationSession GetConfigurationSession(string rulePackageSetId)
		{
			Guid guid = Guid.ParseExact(rulePackageSetId, "D");
			IConfigurationSession result;
			try
			{
				if (ADHelpers.OutOfBoxPackageSetId == guid)
				{
					result = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 106, "GetConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Filtering\\src\\platform\\Management\\ADConnector\\impl\\obj\\amd64\\adhelpers.cs");
				}
				else
				{
					result = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.FullyConsistent, guid, 113, "GetConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Filtering\\src\\platform\\Management\\ADConnector\\impl\\obj\\amd64\\adhelpers.cs");
				}
			}
			catch (Exception ex)
			{
				if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_s(2, 10, TraceProvider.MakeStringArg(rulePackageSetId));
				}
				throw new COMException(ex.Message, -2147220980);
			}
			return result;
		}

		private static TransportRule[] GetRulePackages(IConfigurationSession configSession, string[] rulePackageIds)
		{
			ADObjectId orgContainerId = configSession.GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId("Transport Settings").GetChildId("Rules").GetChildId("ClassificationDefinitions");
			List<QueryFilter> list = new List<QueryFilter>();
			foreach (string propertyValue in rulePackageIds)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, propertyValue));
			}
			QueryFilter filter = new OrFilter(list.ToArray());
			TransportRule[] array = configSession.Find<TransportRule>(childId, QueryScope.SubTree, filter, null, 0);
			if (rulePackageIds.Length != array.Length)
			{
				string text = string.Format("Not all requested rule package sets were found ({0}/{1} found)", array.Length, rulePackageIds.Length);
				if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_s(2, 11, TraceProvider.MakeStringArg(text));
				}
				throw new COMException(text, -2147220985);
			}
			return array;
		}

		private static string DecodeClassificationRules(byte[] compressedBytes)
		{
			string result;
			using (Stream stream = new MemoryStream(compressedBytes, false))
			{
				using (Package package = Package.Open(stream, FileMode.Open, FileAccess.Read))
				{
					Uri partUri = PackUriHelper.CreatePartUri(new Uri("config", UriKind.Relative));
					PackagePart part = package.GetPart(partUri);
					Stream stream2 = part.GetStream(FileMode.Open, FileAccess.Read);
					using (TextReader textReader = new StreamReader(stream2, Encoding.Unicode))
					{
						result = textReader.ReadToEnd();
					}
				}
			}
			return result;
		}

		private static readonly Guid OutOfBoxPackageSetId = Guid.Empty;

		internal class ADRulePackageInfo
		{
			public string Xml { get; set; }

			public DateTime ModifiedDate { get; set; }

			public string RulePackageSetId { get; set; }

			public string RulePackageId { get; set; }
		}
	}
}
