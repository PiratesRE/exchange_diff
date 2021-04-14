using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class HygieneUtils
	{
		public static bool IsAntispamFilterableLanguage(string code)
		{
			return HygieneUtils.antispamFilterableLanguages.Any((string x) => string.Compare(x, code, StringComparison.OrdinalIgnoreCase) == 0);
		}

		public static bool IsValidIso3166Alpha2Code(string code)
		{
			return code != null && code.Length == 2 && HygieneUtils.iso3166Alpha2Codes.Any((string x) => string.Compare(x, code, StringComparison.OrdinalIgnoreCase) == 0);
		}

		public static T ResolvePolicyObject<T>(Task task, IConfigDataProvider session, ADIdParameter idParameter) where T : IConfigurable, new()
		{
			T result = default(T);
			IEnumerable<T> objects = idParameter.GetObjects<T>(null, session);
			using (IEnumerator<T> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (enumerator.MoveNext())
					{
						task.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorAmbiguousPolicyIdentity(idParameter.RawIdentity)), ErrorCategory.SyntaxError, idParameter);
					}
				}
				else
				{
					task.WriteError(new ManagementObjectNotFoundException(Strings.ErrorPolicyNotFound(idParameter.RawIdentity)), ErrorCategory.ObjectNotFound, idParameter);
				}
			}
			return result;
		}

		public static TransportRule ResolvePolicyRuleObject<T>(T policy, IConfigDataProvider session, string ruleCollectionName) where T : ADObject, new()
		{
			if (typeof(T) != typeof(MalwareFilterPolicy) && typeof(T) != typeof(HostedContentFilterPolicy))
			{
				throw new NotSupportedException();
			}
			try
			{
				ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager(ruleCollectionName, session);
				adruleStorageManager.LoadRuleCollection();
				foreach (Rule rule in adruleStorageManager.GetRuleCollection())
				{
					TransportRule transportRule = (TransportRule)rule;
					if (transportRule.Actions != null && transportRule.Actions.Count > 0 && transportRule.Actions[0].Arguments != null && transportRule.Actions[0].Arguments.Count == 2)
					{
						string strA = transportRule.Actions[0].Arguments[1].GetValue(null) as string;
						if (string.Compare(strA, policy.Name, true) == 0)
						{
							return transportRule;
						}
					}
				}
			}
			catch (RuleCollectionNotInAdException)
			{
				return null;
			}
			return null;
		}

		internal static readonly string[] antispamFilterableLanguages = new string[]
		{
			"af",
			"sq",
			"ar",
			"hy",
			"az",
			"bn",
			"eu",
			"bs",
			"br",
			"bg",
			"be",
			"ca",
			"vi",
			"cy",
			"hr",
			"cs",
			"da",
			"nl",
			"en",
			"eo",
			"et",
			"fo",
			"fa",
			"tl",
			"fi",
			"fr",
			"fy",
			"gl",
			"ka",
			"de",
			"el",
			"kl",
			"gu",
			"ha",
			"he",
			"hi",
			"hu",
			"is",
			"id",
			"ga",
			"it",
			"ja",
			"kn",
			"kk",
			"ky",
			"ko",
			"ku",
			"la",
			"lv",
			"lb",
			"lt",
			"mk",
			"ms",
			"ml",
			"mt",
			"mi",
			"mr",
			"mn",
			"nb",
			"nn",
			"ps",
			"pl",
			"pt",
			"pa",
			"rm",
			"ro",
			"ru",
			"se",
			"sr",
			"sk",
			"sl",
			"zu",
			"es",
			"sw",
			"sv",
			"ta",
			"te",
			"th",
			"tr",
			"uk",
			"ur",
			"uz",
			"yi",
			"wen",
			"zh-cn",
			"zh-tw"
		};

		internal static readonly string[] iso3166Alpha2Codes = new string[]
		{
			"af",
			"ax",
			"al",
			"dz",
			"as",
			"ad",
			"ao",
			"ai",
			"aq",
			"ag",
			"ar",
			"am",
			"aw",
			"au",
			"at",
			"az",
			"bs",
			"bh",
			"bd",
			"bb",
			"by",
			"be",
			"bz",
			"bj",
			"bm",
			"bt",
			"bo",
			"bq",
			"ba",
			"bw",
			"bv",
			"br",
			"io",
			"vg",
			"bn",
			"bg",
			"bf",
			"bi",
			"kh",
			"cm",
			"ca",
			"cv",
			"ky",
			"cf",
			"td",
			"cl",
			"cn",
			"cx",
			"cc",
			"co",
			"km",
			"cg",
			"cd",
			"ck",
			"cr",
			"ci",
			"hr",
			"cu",
			"cw",
			"cy",
			"cz",
			"dk",
			"dj",
			"dm",
			"do",
			"ec",
			"eg",
			"sv",
			"gq",
			"er",
			"ee",
			"et",
			"fk",
			"fo",
			"fj",
			"fi",
			"fr",
			"gf",
			"pf",
			"tf",
			"ga",
			"gm",
			"ge",
			"de",
			"gh",
			"gi",
			"gr",
			"gl",
			"gd",
			"gp",
			"gu",
			"gt",
			"gg",
			"gn",
			"gw",
			"gy",
			"ht",
			"hm",
			"va",
			"hn",
			"hk",
			"hu",
			"is",
			"in",
			"id",
			"ir",
			"iq",
			"ie",
			"im",
			"il",
			"it",
			"jm",
			"xj",
			"jp",
			"je",
			"jo",
			"kz",
			"ke",
			"ki",
			"kr",
			"kw",
			"kg",
			"la",
			"lv",
			"lb",
			"ls",
			"lr",
			"ly",
			"li",
			"lt",
			"lu",
			"mo",
			"mk",
			"mg",
			"mw",
			"my",
			"mv",
			"ml",
			"mt",
			"mh",
			"mq",
			"mr",
			"mu",
			"yt",
			"mx",
			"fm",
			"md",
			"mc",
			"mn",
			"me",
			"ms",
			"ma",
			"mz",
			"mm",
			"na",
			"nr",
			"np",
			"nl",
			"nc",
			"nz",
			"ni",
			"ne",
			"ng",
			"nu",
			"nf",
			"kp",
			"mp",
			"no",
			"om",
			"pk",
			"pw",
			"ps",
			"pa",
			"pg",
			"py",
			"pe",
			"ph",
			"pn",
			"pl",
			"pt",
			"pr",
			"qa",
			"re",
			"ro",
			"ru",
			"rw",
			"xs",
			"bl",
			"sh",
			"kn",
			"lc",
			"mf",
			"pm",
			"vc",
			"ws",
			"sm",
			"st",
			"sa",
			"sn",
			"rs",
			"sc",
			"sl",
			"sg",
			"xe",
			"sx",
			"sk",
			"si",
			"sb",
			"so",
			"za",
			"gs",
			"es",
			"lk",
			"sd",
			"sr",
			"sj",
			"sz",
			"se",
			"ch",
			"sy",
			"tw",
			"tj",
			"tz",
			"th",
			"tl",
			"tg",
			"tk",
			"to",
			"tt",
			"tn",
			"tr",
			"tm",
			"tc",
			"tv",
			"um",
			"vi",
			"ug",
			"ua",
			"ae",
			"gb",
			"us",
			"uy",
			"uz",
			"vu",
			"ve",
			"vn",
			"wf",
			"ye",
			"zm",
			"zw"
		};
	}
}
