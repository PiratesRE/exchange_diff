using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Filtering;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class ScanResultExtensions
	{
		public static string ToClientString(this ClientScanResultStorage storage)
		{
			if (storage == null)
			{
				throw new ArgumentNullException("storage");
			}
			string text = storage.ClassifiedParts.ToClientString();
			string text2 = storage.DlpDetectedClassificationObjects.ToClientString();
			string text3 = storage.DlpDetectedClassificationObjects.ToNamesClientString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(ScanResultExtensions.ClientScanResultStorageToStringFormat, new object[]
			{
				storage.RecoveryOptions.ToString(),
				text,
				text2,
				text3
			});
			return stringBuilder.ToString();
		}

		public static ClientScanResultStorage ToClientScanResultStorage(this string clientData)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			ScanResultExtensions.SplitClientData(clientData, out empty, out empty2);
			string[] dcnames = ScanResultExtensions.GetDCNames(empty2);
			if (string.IsNullOrEmpty(empty))
			{
				throw new ClientScanResultParseException("leftClientData is empty");
			}
			string[] array = empty.Split(new char[]
			{
				'?'
			});
			if (array == null || array.Length != 3)
			{
				throw new ClientScanResultParseException("parts are invalid or don't contain 3 elements");
			}
			RecoveryOptions recoveryOptions;
			if (!Enum.TryParse<RecoveryOptions>(array[0], out recoveryOptions) || !(Enum.IsDefined(typeof(RecoveryOptions), recoveryOptions) | recoveryOptions.ToString().Contains(",")))
			{
				throw new ClientScanResultParseException("recoveryOptions is not a valid RecoveryOptions enum value");
			}
			ClientScanResultStorage clientScanResultStorage = new ClientScanResultStorage();
			clientScanResultStorage.RecoveryOptions = (int)recoveryOptions;
			clientScanResultStorage.ClassifiedParts = array[1].ToClassifiedParts();
			clientScanResultStorage.DlpDetectedClassificationObjects = array[2].ToDlpDetectedClassificationObjects();
			if (dcnames.Length != clientScanResultStorage.DlpDetectedClassificationObjects.Count)
			{
				throw new ClientScanResultParseException("Numberof dcNames don't match the number of DlpDetectedClassificationObjects.");
			}
			for (int i = 0; i < dcnames.Length; i++)
			{
				clientScanResultStorage.DlpDetectedClassificationObjects[i].ClassificationName = dcnames[i];
			}
			return clientScanResultStorage;
		}

		public static string[] GetDCNames(string rightClientData)
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(rightClientData))
			{
				int length = "<DC>".Length;
				int num = "<DC>".Length + "</DC>".Length;
				while (rightClientData.Length > num && rightClientData.StartsWith("<DC>", StringComparison.OrdinalIgnoreCase))
				{
					int num2 = rightClientData.IndexOf("</DC>", StringComparison.OrdinalIgnoreCase);
					int num3 = num2 - length;
					if (num3 >= 0)
					{
						list.Add(rightClientData.Substring(length, num3));
						rightClientData = rightClientData.Substring(num2 + "</DC>".Length);
					}
				}
				if (rightClientData.Length != 0)
				{
					throw new ClientScanResultParseException("DCNames doesn't have valid number of <DC> and </DC> nodes.");
				}
			}
			return list.ToArray();
		}

		public static void SplitClientData(string clientData, out string leftClientData, out string rightClientData)
		{
			leftClientData = string.Empty;
			rightClientData = string.Empty;
			if (!clientData.EndsWith("</DCs>", StringComparison.OrdinalIgnoreCase))
			{
				throw new ClientScanResultParseException("clientData does not end with </DCs>");
			}
			int num = clientData.IndexOf("?<DCs>", StringComparison.OrdinalIgnoreCase);
			if (num < 0)
			{
				throw new ClientScanResultParseException("clientData does not contain ?<DCs>");
			}
			leftClientData = clientData.Substring(0, num);
			int num2 = num + "?<DCs>".Length;
			int num3 = clientData.Length - "</DCs>".Length;
			int num4 = num3 - num2;
			if (num2 >= clientData.Length || num3 <= 0 || num3 >= clientData.Length || num4 < 0)
			{
				throw new ClientScanResultParseException("clientData is invalid as dcsStart and/or dcsEnd are out of range");
			}
			rightClientData = clientData.Substring(num2, num4);
		}

		public static List<DiscoveredDataClassification> ToDlpDetectedClassificationObjects(this string clientDetectionData)
		{
			if (string.IsNullOrEmpty(clientDetectionData))
			{
				return new List<DiscoveredDataClassification>();
			}
			string[] array = clientDetectionData.Split(new char[]
			{
				'|'
			});
			if (array == null)
			{
				throw new ClientScanResultParseException("classifications are invalid or doesn't contain atleast 1 element");
			}
			List<DiscoveredDataClassification> list = new List<DiscoveredDataClassification>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				DiscoveredDataClassification item = array[i].ToDiscoveredDataClassification();
				list.Add(item);
			}
			return list;
		}

		public static string ToClientString(this List<DiscoveredDataClassification> discoveredDataClassifications)
		{
			if (discoveredDataClassifications == null || discoveredDataClassifications.Count == 0)
			{
				return string.Empty;
			}
			return string.Join("|", from discoveredDataClassification in discoveredDataClassifications
			select discoveredDataClassification.ToClientString());
		}

		public static string ToNamesClientString(this List<DiscoveredDataClassification> discoveredDataClassifications)
		{
			if (discoveredDataClassifications == null || discoveredDataClassifications.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (DiscoveredDataClassification discoveredDataClassification in discoveredDataClassifications)
			{
				stringBuilder.AppendFormat("{0}{1}{2}", "<DC>", discoveredDataClassification.ClassificationName ?? string.Empty, "</DC>");
			}
			return stringBuilder.ToString();
		}

		public static DiscoveredDataClassification ToDiscoveredDataClassification(this string classification)
		{
			if (string.IsNullOrEmpty(classification))
			{
				throw new ClientScanResultParseException("classification is null");
			}
			string[] array = classification.Split(new char[]
			{
				'>'
			});
			if (array == null || array.Length != 3)
			{
				throw new ClientScanResultParseException("classificationParts is invalid or doesn't contain 3 elements");
			}
			string text = array[0];
			if (string.IsNullOrEmpty(text))
			{
				throw new ClientScanResultParseException("id is empty");
			}
			uint recommendedMinimumConfidence;
			if (!uint.TryParse(array[1], out recommendedMinimumConfidence))
			{
				throw new ClientScanResultParseException("recMinConfidence is not a uint");
			}
			if (string.IsNullOrEmpty(array[2]))
			{
				throw new ClientScanResultParseException("sourceInfo part of classificationParts is empty");
			}
			return new DiscoveredDataClassification(text, string.Empty, recommendedMinimumConfidence, array[2].ToDataClassificationSourceInfos());
		}

		public static string ToClientString(this DiscoveredDataClassification discoveredDataClassification)
		{
			if (discoveredDataClassification == null)
			{
				throw new ArgumentNullException("discoveredDataClassification");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(ScanResultExtensions.ClassificationStringFormat, discoveredDataClassification.Id, discoveredDataClassification.RecommendedMinimumConfidence, discoveredDataClassification.MatchingSourceInfos.ToClientString());
			return stringBuilder.ToString();
		}

		public static List<DataClassificationSourceInfo> ToDataClassificationSourceInfos(this string sourceInfos)
		{
			if (string.IsNullOrEmpty(sourceInfos))
			{
				throw new ClientScanResultParseException("sourceInfos is null");
			}
			string[] array = sourceInfos.Split(new char[]
			{
				'*'
			});
			if (array.Length < 1)
			{
				throw new ClientScanResultParseException("sourceInfosParts doesn't contain atleast 1 element");
			}
			List<DataClassificationSourceInfo> list = new List<DataClassificationSourceInfo>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				DataClassificationSourceInfo item = array[i].ToDataClassificationSourceInfo();
				list.Add(item);
			}
			return list;
		}

		public static string ToClientString(this List<DataClassificationSourceInfo> sourceInfos)
		{
			if (sourceInfos == null || sourceInfos.Count == 0)
			{
				throw new ArgumentNullException("sourceInfos");
			}
			return string.Join("*", from sourceInfo in sourceInfos
			select sourceInfo.ToClientString());
		}

		public static DataClassificationSourceInfo ToDataClassificationSourceInfo(this string sourceInfo)
		{
			if (string.IsNullOrEmpty(sourceInfo))
			{
				throw new ClientScanResultParseException("sourceInfoParts is null");
			}
			string[] array = sourceInfo.Split(new char[]
			{
				'\\'
			});
			if (array == null || array.Length != 5)
			{
				throw new ClientScanResultParseException(string.Format("sourceInfoParts: {0} doesn't contain 5 elements", sourceInfo));
			}
			int num;
			if (!int.TryParse(array[0], out num))
			{
				throw new ClientScanResultParseException(string.Format("id:{0} is not an int", num));
			}
			string text = array[1];
			if (string.IsNullOrEmpty(text))
			{
				throw new ClientScanResultParseException("name is empty");
			}
			string text2 = array[2];
			if (string.IsNullOrEmpty(text2))
			{
				text2 = text;
			}
			int num2;
			if (!int.TryParse(array[3], out num2))
			{
				throw new ClientScanResultParseException(string.Format("count: {0} is not an int", num2));
			}
			uint num3;
			if (!uint.TryParse(array[4], out num3))
			{
				throw new ClientScanResultParseException(string.Format("confidence: {0} is not a uint", num3));
			}
			return new DataClassificationSourceInfo(num, text, text2, num2, num3);
		}

		public static string ToClientString(this DataClassificationSourceInfo sourceInfo)
		{
			if (sourceInfo == null)
			{
				throw new ArgumentNullException("sourceInfo");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(ScanResultExtensions.SourceInfoStringFormat, new object[]
			{
				sourceInfo.SourceId,
				sourceInfo.SourceName,
				sourceInfo.SourceName.Equals(sourceInfo.TopLevelSourceName, StringComparison.OrdinalIgnoreCase) ? string.Empty : sourceInfo.TopLevelSourceName,
				sourceInfo.Count,
				sourceInfo.ConfidenceLevel
			});
			return stringBuilder.ToString();
		}

		public static List<string> ToClassifiedParts(this string clientPartsData)
		{
			if (string.IsNullOrEmpty(clientPartsData))
			{
				return new List<string>();
			}
			List<string> list = new List<string>();
			string[] array = clientPartsData.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = false;
				if (!string.IsNullOrEmpty(array[i]))
				{
					flag = array[i].Equals(ScanResultStorageProvider.MessageBodyName, StringComparison.OrdinalIgnoreCase);
					if (!flag)
					{
						string[] array2 = array[i].Split(new char[]
						{
							':'
						});
						flag = (array2 != null && array2.Length == 2 && !string.IsNullOrEmpty(array2[0]) && !string.IsNullOrEmpty(array2[1]));
					}
				}
				if (!flag)
				{
					throw new ClientScanResultParseException(string.Format("parts[{0}]: {1} doesn't match either Message Body or attachmentName:attachmentId formats.", i, array[i]));
				}
				list.Add(array[i]);
			}
			return list;
		}

		public static string ToClientString(this List<string> clientParts)
		{
			if (clientParts == null)
			{
				return string.Empty;
			}
			return string.Join("|", clientParts);
		}

		private const string ClientScanResultStorageToStringSeparator = "?";

		private const char ClientScanResultStorageToStringSeparatorChar = '?';

		private const string DataClassificationArrayStartNodeName = "<DCs>";

		private const string DataClassificationArrayEndNodeName = "</DCs>";

		private const string DataClassificationArrayElementStartNodeName = "<DC>";

		private const string DataClassificationArrayElementEndNodeName = "</DC>";

		private const string ClassifiedPartsArraySeparator = "|";

		private const char ClassifiedPartsArraySeparatorChar = '|';

		private const string ClassificationArraySeparator = "|";

		private const char ClassificationArraySeparatorChar = '|';

		private const string ClassificationPropertiesSeparator = ">";

		private const char ClassificationPropertiesSeparatorChar = '>';

		private const string SourceInfoArraySeparator = "*";

		private const char SourceInfoArraySeparatorChar = '*';

		private const string SourceInfoPropertiesSeparator = "\\";

		private const char SourceInfoPropertiesSeparatorChar = '\\';

		private static readonly string ClientScanResultStorageToStringFormat = string.Join<string>("?", new string[]
		{
			"{0}",
			"{1}",
			"{2}",
			string.Format("{0}{1}{2}", "<DCs>", "{3}", "</DCs>")
		});

		private static readonly string ClassificationStringFormat = string.Join(">", new string[]
		{
			"{0}",
			"{1}",
			"{2}"
		});

		private static readonly string SourceInfoStringFormat = string.Join("\\", new string[]
		{
			"{0}",
			"{1}",
			"{2}",
			"{3}",
			"{4}"
		});
	}
}
