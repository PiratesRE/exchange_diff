using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Agent.Hygiene;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Configuration
{
	[Serializable]
	internal class ProtocolAnalysisSrlSettings
	{
		public virtual PropertyBag Fields
		{
			get
			{
				return this.fields;
			}
			set
			{
				this.fields = value;
			}
		}

		public ProtocolAnalysisSrlSettings()
		{
			this.fields = new PropertyBag(75);
		}

		public void InitializeDefaults()
		{
			this.ConfigurationVersion = "1";
			this.LogNumMsgs = -0.035;
			this.ZombNdom = -0.078;
			this.ZombNip = 0.509;
			this.ZombNsegs = 1.532;
			this.NullRdns = 2.645;
			this.LogHeloEmpty = 0.0;
			this.LogNormNmsgNumUniqHelo = -0.537;
			this.LogHeloMatchAll = -0.818;
			this.LogHeloMatch2nd = -1.88;
			this.LogHeloMatchLocal = 0.0;
			this.HeloNoMatch = 0.177;
			this.LogLen0 = 0.0;
			this.LogLen1 = 0.0;
			this.LogLen2 = 0.0;
			this.LogLen3 = 0.0;
			this.LogLen4 = 0.0;
			this.LogLen5 = 0.0;
			this.LogLen6 = 0.0;
			this.LogLen7 = 0.0;
			this.LogLen8 = 0.0;
			this.LogLen9 = 0.0;
			this.LogLen10 = 0.0;
			this.LogLen11 = 0.0;
			this.LogLen12 = 0.0;
			this.LogLen13 = 0.0;
			this.LogLen14 = 0.0;
			this.LogNormNmsgNumValid = -0.054;
			this.LogNormNmsgNumInvalid = 0.0;
			this.LogNormNmsgNumUniqValid = 0.057;
			this.LogNormNmsgNumUniqInvalid = 0.0;
			this.LogValidScl0 = -6.788;
			this.LogValidScl1 = -3.237;
			this.LogValidScl2 = -2.086;
			this.LogValidScl3 = -1.303;
			this.LogValidScl4 = -0.106;
			this.LogValidScl5 = 0.447;
			this.LogValidScl6 = 0.988;
			this.LogValidScl7 = 1.899;
			this.LogValidScl8 = 5.445;
			this.LogValidScl9 = 6.347;
			this.LogInvalidScl0 = 0.0;
			this.LogInvalidScl1 = 0.0;
			this.LogInvalidScl2 = 0.0;
			this.LogInvalidScl3 = 0.0;
			this.LogInvalidScl4 = 0.0;
			this.LogInvalidScl5 = 0.0;
			this.LogInvalidScl6 = 0.0;
			this.LogInvalidScl7 = 0.0;
			this.LogInvalidScl8 = 0.0;
			this.LogInvalidScl9 = 0.0;
			this.LogCallIdValid = 0.0;
			this.LogCallIdInvalid = 0.0;
			this.LogCallIdIndeterminate = 0.0;
			this.LogCallIdEpderror = 0.0;
			this.LogCallIdError = 0.0;
			this.LogCallIdNull = 0.0;
			this.Bias = -0.018;
			this.FeatureThreshold0 = -0.933;
			this.FeatureThreshold1 = -0.069;
			this.FeatureThreshold2 = 0.59;
			this.FeatureThreshold3 = 1.276;
			this.FeatureThreshold4 = 1.654;
			this.FeatureThreshold5 = 2.704;
			this.FeatureThreshold6 = 3.597;
			this.FeatureThreshold7 = 4.304;
			this.FeatureThreshold8 = 5.228;
			this.InitWinLen = 100;
			this.MinWinLen = 20;
			this.MaxWinLen = 500;
			this.WinlenShrinkFactor = 0.25;
			this.WinlenExpandFactor = 4.0;
			this.GoodBehaviorPeriod = 100;
			this.ZombieKeywords = new string[]
			{
				"dsl",
				"client",
				"pool",
				"modem",
				"cable",
				"ppp",
				"dialup",
				"dhcp"
			};
			this.RecommendedDownloadIntervalInMinutes = 30.0;
		}

		public double LogValidScl(int scl)
		{
			switch (scl)
			{
			case 0:
				return this.LogValidScl0;
			case 1:
				return this.LogValidScl1;
			case 2:
				return this.LogValidScl2;
			case 3:
				return this.LogValidScl3;
			case 4:
				return this.LogValidScl4;
			case 5:
				return this.LogValidScl5;
			case 6:
				return this.LogValidScl6;
			case 7:
				return this.LogValidScl7;
			case 8:
				return this.LogValidScl8;
			case 9:
				return this.LogValidScl9;
			default:
				throw new LocalizedException(AgentStrings.InvalidSclLevel(scl));
			}
		}

		public double LogInvalidScl(int scl)
		{
			switch (scl)
			{
			case 0:
				return this.LogInvalidScl0;
			case 1:
				return this.LogInvalidScl1;
			case 2:
				return this.LogInvalidScl2;
			case 3:
				return this.LogInvalidScl3;
			case 4:
				return this.LogInvalidScl4;
			case 5:
				return this.LogInvalidScl5;
			case 6:
				return this.LogInvalidScl6;
			case 7:
				return this.LogInvalidScl7;
			case 8:
				return this.LogInvalidScl8;
			case 9:
				return this.LogInvalidScl9;
			default:
				throw new LocalizedException(AgentStrings.InvalidSclLevel(scl));
			}
		}

		public double LogLength(int length)
		{
			switch (length)
			{
			case 0:
				return this.LogLen0;
			case 1:
				return this.LogLen1;
			case 2:
				return this.LogLen2;
			case 3:
				return this.LogLen3;
			case 4:
				return this.LogLen4;
			case 5:
				return this.LogLen5;
			case 6:
				return this.LogLen6;
			case 7:
				return this.LogLen7;
			case 8:
				return this.LogLen8;
			case 9:
				return this.LogLen9;
			case 10:
				return this.LogLen10;
			case 11:
				return this.LogLen11;
			case 12:
				return this.LogLen12;
			case 13:
				return this.LogLen13;
			case 14:
				return this.LogLen14;
			default:
				throw new LocalizedException(AgentStrings.InvalidLogLength(length));
			}
		}

		public double FeatureThresholds(int srl)
		{
			switch (srl)
			{
			case 0:
				return this.FeatureThreshold0;
			case 1:
				return this.FeatureThreshold1;
			case 2:
				return this.FeatureThreshold2;
			case 3:
				return this.FeatureThreshold3;
			case 4:
				return this.FeatureThreshold4;
			case 5:
				return this.FeatureThreshold5;
			case 6:
				return this.FeatureThreshold6;
			case 7:
				return this.FeatureThreshold7;
			case 8:
				return this.FeatureThreshold8;
			default:
				throw new LocalizedException(AgentStrings.InvalidFeatureThreshold(srl));
			}
		}

		public int MaxFeatureThreshold
		{
			get
			{
				return 9;
			}
		}

		public string ConfigurationVersion
		{
			get
			{
				return (string)this.Fields["ConfigurationVersion"];
			}
			set
			{
				this.Fields["ConfigurationVersion"] = value;
			}
		}

		public double LogNumMsgs
		{
			get
			{
				return (double)this.Fields["LogNumMsgs"];
			}
			set
			{
				this.Fields["LogNumMsgs"] = value;
			}
		}

		public double ZombNdom
		{
			get
			{
				return (double)this.Fields["ZombNdom"];
			}
			set
			{
				this.Fields["ZombNdom"] = value;
			}
		}

		public double ZombNip
		{
			get
			{
				return (double)this.Fields["ZombNip"];
			}
			set
			{
				this.Fields["ZombNip"] = value;
			}
		}

		public double ZombNsegs
		{
			get
			{
				return (double)this.Fields["ZombNsegs"];
			}
			set
			{
				this.Fields["ZombNsegs"] = value;
			}
		}

		public double NullRdns
		{
			get
			{
				return (double)this.Fields["NullRdns"];
			}
			set
			{
				this.Fields["NullRdns"] = value;
			}
		}

		public double LogHeloEmpty
		{
			get
			{
				return (double)this.Fields["LogHeloEmpty"];
			}
			set
			{
				this.Fields["LogHeloEmpty"] = value;
			}
		}

		public double LogNormNmsgNumUniqHelo
		{
			get
			{
				return (double)this.Fields["LogNormNmsgNumUniqHelo"];
			}
			set
			{
				this.Fields["LogNormNmsgNumUniqHelo"] = value;
			}
		}

		public double LogHeloMatchAll
		{
			get
			{
				return (double)this.Fields["LogHeloMatchAll"];
			}
			set
			{
				this.Fields["LogHeloMatchAll"] = value;
			}
		}

		public double LogHeloMatch2nd
		{
			get
			{
				return (double)this.Fields["LogHeloMatch2nd"];
			}
			set
			{
				this.Fields["LogHeloMatch2nd"] = value;
			}
		}

		public double LogHeloMatchLocal
		{
			get
			{
				return (double)this.Fields["LogHeloMatchLocal"];
			}
			set
			{
				this.Fields["LogHeloMatchLocal"] = value;
			}
		}

		public double HeloNoMatch
		{
			get
			{
				return (double)this.Fields["HeloNoMatch"];
			}
			set
			{
				this.Fields["HeloNoMatch"] = value;
			}
		}

		public double LogLen0
		{
			get
			{
				return (double)this.Fields["LogLen0"];
			}
			set
			{
				this.Fields["LogLen0"] = value;
			}
		}

		public double LogLen1
		{
			get
			{
				return (double)this.Fields["LogLen1"];
			}
			set
			{
				this.Fields["LogLen1"] = value;
			}
		}

		public double LogLen2
		{
			get
			{
				return (double)this.Fields["LogLen2"];
			}
			set
			{
				this.Fields["LogLen2"] = value;
			}
		}

		public double LogLen3
		{
			get
			{
				return (double)this.Fields["LogLen3"];
			}
			set
			{
				this.Fields["LogLen3"] = value;
			}
		}

		public double LogLen4
		{
			get
			{
				return (double)this.Fields["LogLen4"];
			}
			set
			{
				this.Fields["LogLen4"] = value;
			}
		}

		public double LogLen5
		{
			get
			{
				return (double)this.Fields["LogLen5"];
			}
			set
			{
				this.Fields["LogLen5"] = value;
			}
		}

		public double LogLen6
		{
			get
			{
				return (double)this.Fields["LogLen6"];
			}
			set
			{
				this.Fields["LogLen6"] = value;
			}
		}

		public double LogLen7
		{
			get
			{
				return (double)this.Fields["LogLen7"];
			}
			set
			{
				this.Fields["LogLen7"] = value;
			}
		}

		public double LogLen8
		{
			get
			{
				return (double)this.Fields["LogLen8"];
			}
			set
			{
				this.Fields["LogLen8"] = value;
			}
		}

		public double LogLen9
		{
			get
			{
				return (double)this.Fields["LogLen9"];
			}
			set
			{
				this.Fields["LogLen9"] = value;
			}
		}

		public double LogLen10
		{
			get
			{
				return (double)this.Fields["LogLen10"];
			}
			set
			{
				this.Fields["LogLen10"] = value;
			}
		}

		public double LogLen11
		{
			get
			{
				return (double)this.Fields["LogLen11"];
			}
			set
			{
				this.Fields["LogLen11"] = value;
			}
		}

		public double LogLen12
		{
			get
			{
				return (double)this.Fields["LogLen12"];
			}
			set
			{
				this.Fields["LogLen12"] = value;
			}
		}

		public double LogLen13
		{
			get
			{
				return (double)this.Fields["LogLen13"];
			}
			set
			{
				this.Fields["LogLen13"] = value;
			}
		}

		public double LogLen14
		{
			get
			{
				return (double)this.Fields["LogLen14"];
			}
			set
			{
				this.Fields["LogLen14"] = value;
			}
		}

		public double LogNormNmsgNumValid
		{
			get
			{
				return (double)this.Fields["LogNormNmsgNumValid"];
			}
			set
			{
				this.Fields["LogNormNmsgNumValid"] = value;
			}
		}

		public double LogNormNmsgNumInvalid
		{
			get
			{
				return (double)this.Fields["LogNormNmsgNumInvalid"];
			}
			set
			{
				this.Fields["LogNormNmsgNumInvalid"] = value;
			}
		}

		public double LogNormNmsgNumUniqValid
		{
			get
			{
				return (double)this.Fields["LogNormNmsgNumUniqValid"];
			}
			set
			{
				this.Fields["LogNormNmsgNumUniqValid"] = value;
			}
		}

		public double LogNormNmsgNumUniqInvalid
		{
			get
			{
				return (double)this.Fields["LogNormNmsgNumUniqInvalid"];
			}
			set
			{
				this.Fields["LogNormNmsgNumUniqInvalid"] = value;
			}
		}

		public double LogValidScl0
		{
			get
			{
				return (double)this.Fields["LogValidScl0"];
			}
			set
			{
				this.Fields["LogValidScl0"] = value;
			}
		}

		public double LogValidScl1
		{
			get
			{
				return (double)this.Fields["LogValidScl1"];
			}
			set
			{
				this.Fields["LogValidScl1"] = value;
			}
		}

		public double LogValidScl2
		{
			get
			{
				return (double)this.Fields["LogValidScl2"];
			}
			set
			{
				this.Fields["LogValidScl2"] = value;
			}
		}

		public double LogValidScl3
		{
			get
			{
				return (double)this.Fields["LogValidScl3"];
			}
			set
			{
				this.Fields["LogValidScl3"] = value;
			}
		}

		public double LogValidScl4
		{
			get
			{
				return (double)this.Fields["LogValidScl4"];
			}
			set
			{
				this.Fields["LogValidScl4"] = value;
			}
		}

		public double LogValidScl5
		{
			get
			{
				return (double)this.Fields["LogValidScl5"];
			}
			set
			{
				this.Fields["LogValidScl5"] = value;
			}
		}

		public double LogValidScl6
		{
			get
			{
				return (double)this.Fields["LogValidScl6"];
			}
			set
			{
				this.Fields["LogValidScl6"] = value;
			}
		}

		public double LogValidScl7
		{
			get
			{
				return (double)this.Fields["LogValidScl7"];
			}
			set
			{
				this.Fields["LogValidScl7"] = value;
			}
		}

		public double LogValidScl8
		{
			get
			{
				return (double)this.Fields["LogValidScl8"];
			}
			set
			{
				this.Fields["LogValidScl8"] = value;
			}
		}

		public double LogValidScl9
		{
			get
			{
				return (double)this.Fields["LogValidScl9"];
			}
			set
			{
				this.Fields["LogValidScl9"] = value;
			}
		}

		public double LogInvalidScl0
		{
			get
			{
				return (double)this.Fields["LogInvalidScl0"];
			}
			set
			{
				this.Fields["LogInvalidScl0"] = value;
			}
		}

		public double LogInvalidScl1
		{
			get
			{
				return (double)this.Fields["LogInvalidScl1"];
			}
			set
			{
				this.Fields["LogInvalidScl1"] = value;
			}
		}

		public double LogInvalidScl2
		{
			get
			{
				return (double)this.Fields["LogInvalidScl2"];
			}
			set
			{
				this.Fields["LogInvalidScl2"] = value;
			}
		}

		public double LogInvalidScl3
		{
			get
			{
				return (double)this.Fields["LogInvalidScl3"];
			}
			set
			{
				this.Fields["LogInvalidScl3"] = value;
			}
		}

		public double LogInvalidScl4
		{
			get
			{
				return (double)this.Fields["LogInvalidScl4"];
			}
			set
			{
				this.Fields["LogInvalidScl4"] = value;
			}
		}

		public double LogInvalidScl5
		{
			get
			{
				return (double)this.Fields["LogInvalidScl5"];
			}
			set
			{
				this.Fields["LogInvalidScl5"] = value;
			}
		}

		public double LogInvalidScl6
		{
			get
			{
				return (double)this.Fields["LogInvalidScl6"];
			}
			set
			{
				this.Fields["LogInvalidScl6"] = value;
			}
		}

		public double LogInvalidScl7
		{
			get
			{
				return (double)this.Fields["LogInvalidScl7"];
			}
			set
			{
				this.Fields["LogInvalidScl7"] = value;
			}
		}

		public double LogInvalidScl8
		{
			get
			{
				return (double)this.Fields["LogInvalidScl8"];
			}
			set
			{
				this.Fields["LogInvalidScl8"] = value;
			}
		}

		public double LogInvalidScl9
		{
			get
			{
				return (double)this.Fields["LogInvalidScl9"];
			}
			set
			{
				this.Fields["LogInvalidScl9"] = value;
			}
		}

		public double LogCallIdValid
		{
			get
			{
				return (double)this.Fields["LogCallIdValid"];
			}
			set
			{
				this.Fields["LogCallIdValid"] = value;
			}
		}

		public double LogCallIdInvalid
		{
			get
			{
				return (double)this.Fields["LogCallIdInvalid"];
			}
			set
			{
				this.Fields["LogCallIdInvalid"] = value;
			}
		}

		public double LogCallIdIndeterminate
		{
			get
			{
				return (double)this.Fields["LogCallIdIndeterminate"];
			}
			set
			{
				this.Fields["LogCallIdIndeterminate"] = value;
			}
		}

		public double LogCallIdEpderror
		{
			get
			{
				return (double)this.Fields["LogCallIdEpderror"];
			}
			set
			{
				this.Fields["LogCallIdEpderror"] = value;
			}
		}

		public double LogCallIdError
		{
			get
			{
				return (double)this.Fields["LogCallIdError"];
			}
			set
			{
				this.Fields["LogCallIdError"] = value;
			}
		}

		public double LogCallIdNull
		{
			get
			{
				return (double)this.Fields["LogCallIdNull"];
			}
			set
			{
				this.Fields["LogCallIdNull"] = value;
			}
		}

		public double Bias
		{
			get
			{
				return (double)this.Fields["Bias"];
			}
			set
			{
				this.Fields["Bias"] = value;
			}
		}

		public double FeatureThreshold0
		{
			get
			{
				return (double)this.Fields["FeatureThreshold0"];
			}
			set
			{
				this.Fields["FeatureThreshold0"] = value;
			}
		}

		public double FeatureThreshold1
		{
			get
			{
				return (double)this.Fields["FeatureThreshold1"];
			}
			set
			{
				this.Fields["FeatureThreshold1"] = value;
			}
		}

		public double FeatureThreshold2
		{
			get
			{
				return (double)this.Fields["FeatureThreshold2"];
			}
			set
			{
				this.Fields["FeatureThreshold2"] = value;
			}
		}

		public double FeatureThreshold3
		{
			get
			{
				return (double)this.Fields["FeatureThreshold3"];
			}
			set
			{
				this.Fields["FeatureThreshold3"] = value;
			}
		}

		public double FeatureThreshold4
		{
			get
			{
				return (double)this.Fields["FeatureThreshold4"];
			}
			set
			{
				this.Fields["FeatureThreshold4"] = value;
			}
		}

		public double FeatureThreshold5
		{
			get
			{
				return (double)this.Fields["FeatureThreshold5"];
			}
			set
			{
				this.Fields["FeatureThreshold5"] = value;
			}
		}

		public double FeatureThreshold6
		{
			get
			{
				return (double)this.Fields["FeatureThreshold6"];
			}
			set
			{
				this.Fields["FeatureThreshold6"] = value;
			}
		}

		public double FeatureThreshold7
		{
			get
			{
				return (double)this.Fields["FeatureThreshold7"];
			}
			set
			{
				this.Fields["FeatureThreshold7"] = value;
			}
		}

		public double FeatureThreshold8
		{
			get
			{
				return (double)this.Fields["FeatureThreshold8"];
			}
			set
			{
				this.Fields["FeatureThreshold8"] = value;
			}
		}

		public int InitWinLen
		{
			get
			{
				return (int)this.Fields["InitWinLen"];
			}
			set
			{
				this.Fields["InitWinLen"] = value;
			}
		}

		public int MinWinLen
		{
			get
			{
				return (int)this.Fields["MinWinLen"];
			}
			set
			{
				this.Fields["MinWinLen"] = value;
			}
		}

		public int MaxWinLen
		{
			get
			{
				return (int)this.Fields["MaxWinLen"];
			}
			set
			{
				this.Fields["MaxWinLen"] = value;
			}
		}

		public double WinlenShrinkFactor
		{
			get
			{
				return (double)this.Fields["WinlenShrinkFactor"];
			}
			set
			{
				this.Fields["WinlenShrinkFactor"] = value;
			}
		}

		public double WinlenExpandFactor
		{
			get
			{
				return (double)this.Fields["WinlenExpandFactor"];
			}
			set
			{
				this.Fields["WinlenExpandFactor"] = value;
			}
		}

		public int GoodBehaviorPeriod
		{
			get
			{
				return (int)this.Fields["GoodBehaviorPeriod"];
			}
			set
			{
				this.Fields["GoodBehaviorPeriod"] = value;
			}
		}

		public string[] ZombieKeywords
		{
			get
			{
				return (string[])this.Fields["ZombieKeywords"];
			}
			set
			{
				this.Fields["ZombieKeywords"] = value;
			}
		}

		public double RecommendedDownloadIntervalInMinutes
		{
			get
			{
				return (double)this.Fields["RecommendedDownloadIntervalInMinutes"];
			}
			set
			{
				this.Fields["RecommendedDownloadIntervalInMinutes"] = value;
			}
		}

		public bool StoreReputationServiceParams(FileStream stream, Trace tracer)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			bool result;
			try
			{
				xmlDocument.Load(stream);
				result = this.ParseXmlAndSetProperties(xmlDocument, tracer);
			}
			catch (XmlException ex)
			{
				tracer.TraceDebug<string>((long)this.GetHashCode(), "Failed to parse the XML downloaded from the web service. Error {0}", ex.Message);
				result = false;
			}
			catch
			{
				tracer.TraceDebug((long)this.GetHashCode(), "Failed to transfer the XML downloaded from the web service to the config object.");
				result = false;
			}
			return result;
		}

		private bool ParseXmlAndSetProperties(XmlDocument doc, Trace tracer)
		{
			lock (this.syncObject)
			{
				XmlNode firstChild = doc.DocumentElement.FirstChild;
				if (firstChild == null)
				{
					tracer.TraceDebug<string>((long)this.GetHashCode(), "Failed to parse the XML downloaded from the web service: no {0} node.", "ProtocolAnalysisSettings");
					return false;
				}
				XmlNode xmlNode = null;
				PropertyBag propertyBag = new PropertyBag();
				StringBuilder stringBuilder = null;
				try
				{
					xmlNode = firstChild.FirstChild;
					while (xmlNode != null)
					{
						string name;
						if ((name = xmlNode.Name) == null)
						{
							goto IL_1EF;
						}
						if (<PrivateImplementationDetails>{F28DE915-E35E-49F1-9F35-FA7FC113BD0D}.$$method0x6000218-1 == null)
						{
							<PrivateImplementationDetails>{F28DE915-E35E-49F1-9F35-FA7FC113BD0D}.$$method0x6000218-1 = new Dictionary<string, int>(6)
							{
								{
									"ConfigurationVersion",
									0
								},
								{
									"ZombieKeywords",
									1
								},
								{
									"MinWinLen",
									2
								},
								{
									"MaxWinLen",
									3
								},
								{
									"GoodBehaviorPeriod",
									4
								},
								{
									"InitWinLen",
									5
								}
							};
						}
						int num;
						if (!<PrivateImplementationDetails>{F28DE915-E35E-49F1-9F35-FA7FC113BD0D}.$$method0x6000218-1.TryGetValue(name, out num))
						{
							goto IL_1EF;
						}
						switch (num)
						{
						case 0:
							propertyBag[xmlNode.Name] = xmlNode.InnerText;
							Database.UpdateConfiguration(xmlNode.Name, xmlNode.InnerText, tracer);
							break;
						case 1:
						{
							ArrayList arrayList = new ArrayList();
							if (stringBuilder == null)
							{
								stringBuilder = new StringBuilder();
							}
							else
							{
								stringBuilder.Clear();
							}
							for (XmlNode xmlNode2 = xmlNode.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
							{
								stringBuilder.Append(xmlNode2.InnerText);
								if (xmlNode2.NextSibling != null)
								{
									stringBuilder.Append(";");
								}
								arrayList.Add(xmlNode2.InnerText);
							}
							if (arrayList.Count != 0)
							{
								propertyBag[xmlNode.Name] = arrayList.ToArray();
								Database.UpdateConfiguration(xmlNode.Name, stringBuilder.ToString(), tracer);
							}
							break;
						}
						case 2:
						case 3:
						case 4:
						case 5:
							propertyBag[xmlNode.Name] = Convert.ToInt32(xmlNode.InnerText, CultureInfo.InvariantCulture);
							Database.UpdateConfiguration(xmlNode.Name, xmlNode.InnerText, tracer);
							break;
						default:
							goto IL_1EF;
						}
						IL_222:
						xmlNode = xmlNode.NextSibling;
						continue;
						IL_1EF:
						propertyBag[xmlNode.Name] = Convert.ToDouble(xmlNode.InnerText, CultureInfo.InvariantCulture);
						Database.UpdateConfiguration(xmlNode.Name, xmlNode.InnerText, tracer);
						goto IL_222;
					}
				}
				catch (Exception ex)
				{
					tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Failed to parse the XML downloaded from the web service: {0} node. Error: {1}", xmlNode.Name, ex.Message);
					return false;
				}
				this.Fields = propertyBag;
			}
			return true;
		}

		private object syncObject = new object();

		private PropertyBag fields;
	}
}
