using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Extensibility.Internal
{
	[Serializable]
	internal static class DsnDefaultMessages
	{
		public static void GetAllDsnCodesAndMessages(out string[] statusCodes, out LocalizedString[] messages)
		{
			Dictionary<string, LocalizedString> dictionary;
			if (DsnVariantConfiguration.SystemMessageOverridesEnabled())
			{
				dictionary = new Dictionary<string, LocalizedString>(DsnDefaultMessages.statusToExplanation, StringComparer.OrdinalIgnoreCase);
				using (Dictionary<string, LocalizedString>.KeyCollection.Enumerator enumerator = DsnDefaultMessages.statusToExplanationDataCenterOverrides.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string key = enumerator.Current;
						dictionary[key] = DsnDefaultMessages.statusToExplanationDataCenterOverrides[key];
					}
					goto IL_62;
				}
			}
			dictionary = DsnDefaultMessages.statusToExplanation;
			IL_62:
			statusCodes = new string[dictionary.Count];
			messages = new LocalizedString[dictionary.Count];
			dictionary.Keys.CopyTo(statusCodes, 0);
			dictionary.Values.CopyTo(messages, 0);
		}

		public static bool TryGetResourceRecipientExplanation(string enhancedStatusCode, out LocalizedString explanation)
		{
			return (DsnVariantConfiguration.SystemMessageOverridesEnabled() && DsnDefaultMessages.statusToExplanationDataCenterOverrides.TryGetValue(enhancedStatusCode, out explanation)) || DsnDefaultMessages.statusToExplanation.TryGetValue(enhancedStatusCode, out explanation);
		}

		private static Dictionary<string, LocalizedString> statusToExplanation = new Dictionary<string, LocalizedString>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"5.0.0",
				SystemMessages.HumanText5_0_0
			},
			{
				"5.1.0",
				SystemMessages.HumanText5_1_0
			},
			{
				"5.1.1",
				SystemMessages.HumanText5_1_1
			},
			{
				"5.1.2",
				SystemMessages.HumanText5_1_2
			},
			{
				"5.1.3",
				SystemMessages.HumanText5_1_3
			},
			{
				"5.1.4",
				SystemMessages.HumanText5_1_4
			},
			{
				"5.1.6",
				SystemMessages.HumanText5_1_6
			},
			{
				"5.1.7",
				SystemMessages.HumanText5_1_7
			},
			{
				"5.1.8",
				SystemMessages.HumanText5_1_8
			},
			{
				"5.2.0",
				SystemMessages.HumanText5_2_0
			},
			{
				"5.2.1",
				SystemMessages.HumanText5_2_1
			},
			{
				"5.2.2",
				SystemMessages.HumanText5_2_2
			},
			{
				"5.2.3",
				SystemMessages.HumanText5_2_3
			},
			{
				"5.2.4",
				SystemMessages.HumanText5_2_4
			},
			{
				"5.3.0",
				SystemMessages.HumanText5_3_0
			},
			{
				"5.3.1",
				SystemMessages.HumanText5_3_1
			},
			{
				"5.3.2",
				SystemMessages.HumanText5_3_2
			},
			{
				"5.3.3",
				SystemMessages.HumanText5_3_3
			},
			{
				"5.3.4",
				SystemMessages.HumanText5_3_4
			},
			{
				"5.3.5",
				SystemMessages.HumanText5_3_5
			},
			{
				"5.4.0",
				SystemMessages.HumanText5_4_0
			},
			{
				"5.4.1",
				SystemMessages.HumanText5_4_1
			},
			{
				"5.4.2",
				SystemMessages.HumanText5_4_2
			},
			{
				"5.4.3",
				SystemMessages.HumanText5_4_3
			},
			{
				"5.4.4",
				SystemMessages.HumanText5_4_4
			},
			{
				"5.4.6",
				SystemMessages.HumanText5_4_6
			},
			{
				"5.4.7",
				SystemMessages.HumanText5_4_7
			},
			{
				"5.4.8",
				SystemMessages.HumanText5_4_8
			},
			{
				"5.5.0",
				SystemMessages.HumanText5_5_0
			},
			{
				"5.5.1",
				SystemMessages.HumanText5_5_1
			},
			{
				"5.5.2",
				SystemMessages.HumanText5_5_2
			},
			{
				"5.5.3",
				SystemMessages.HumanText5_5_3
			},
			{
				"5.5.4",
				SystemMessages.HumanText5_5_4
			},
			{
				"5.5.5",
				SystemMessages.HumanText5_5_5
			},
			{
				"5.5.6",
				SystemMessages.HumanText5_5_6
			},
			{
				"5.6.0",
				SystemMessages.HumanText5_6_0
			},
			{
				"5.6.1",
				SystemMessages.HumanText5_6_1
			},
			{
				"5.6.2",
				SystemMessages.HumanText5_6_2
			},
			{
				"5.6.3",
				SystemMessages.HumanText5_6_3
			},
			{
				"5.6.4",
				SystemMessages.HumanText5_6_4
			},
			{
				"5.6.5",
				SystemMessages.HumanText5_6_5
			},
			{
				"5.7.0",
				SystemMessages.HumanText5_7_0
			},
			{
				"5.7.1",
				SystemMessages.HumanText5_7_1
			},
			{
				"5.7.100",
				SystemMessages.HumanText5_7_100
			},
			{
				"5.7.2",
				SystemMessages.HumanText5_7_2
			},
			{
				"5.7.3",
				SystemMessages.HumanText5_7_3
			},
			{
				"5.7.300",
				SystemMessages.HumanText5_7_300
			},
			{
				"5.7.4",
				SystemMessages.HumanText5_7_4
			},
			{
				"5.7.5",
				SystemMessages.HumanText5_7_5
			},
			{
				"5.7.6",
				SystemMessages.HumanText5_7_6
			},
			{
				"5.7.7",
				SystemMessages.HumanText5_7_7
			},
			{
				"5.7.8",
				SystemMessages.HumanText5_7_8
			},
			{
				"5.7.9",
				SystemMessages.HumanText5_7_9
			},
			{
				"5.7.10",
				SystemMessages.HumanText5_7_10
			}
		};

		private static Dictionary<string, LocalizedString> statusToExplanationDataCenterOverrides = new Dictionary<string, LocalizedString>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"4.4.7",
				SystemMessages.DataCenterHumanText4_4_7
			},
			{
				"5.1.0",
				SystemMessages.DataCenterHumanText5_1_0
			},
			{
				"5.1.1",
				SystemMessages.DataCenterHumanText5_1_1
			},
			{
				"5.4.1",
				SystemMessages.DataCenterHumanText5_4_1
			},
			{
				"5.4.6",
				SystemMessages.DataCenterHumanText5_4_6
			},
			{
				"5.7.1",
				SystemMessages.DataCenterHumanText5_7_1
			}
		};
	}
}
