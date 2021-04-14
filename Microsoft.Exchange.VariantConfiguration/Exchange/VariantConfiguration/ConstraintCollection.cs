using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using Microsoft.Exchange.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.VariantConfiguration
{
	public class ConstraintCollection : Dictionary<string, string>
	{
		public ConstraintCollection(ConstraintCollection collection) : base(collection, StringComparer.OrdinalIgnoreCase)
		{
		}

		private ConstraintCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
		}

		internal static string Mode
		{
			get
			{
				if (ConstraintCollection.mode == null)
				{
					switch (Datacenter.GetExchangeSku())
					{
					case Datacenter.ExchangeSku.Enterprise:
						ConstraintCollection.mode = "enterprise";
						goto IL_59;
					case Datacenter.ExchangeSku.ExchangeDatacenter:
						ConstraintCollection.mode = "datacenter";
						goto IL_59;
					case Datacenter.ExchangeSku.DatacenterDedicated:
						ConstraintCollection.mode = "dedicated";
						goto IL_59;
					}
					ConstraintCollection.mode = string.Empty;
				}
				IL_59:
				return ConstraintCollection.mode;
			}
		}

		private static ConstraintCollection GlobalConstraints
		{
			get
			{
				if (ConstraintCollection.globalConstraints == null)
				{
					ConstraintCollection constraintCollection = new ConstraintCollection();
					if (!string.IsNullOrEmpty(ConstraintCollection.Mode))
					{
						constraintCollection.Add(VariantType.Mode, ConstraintCollection.Mode);
					}
					constraintCollection.Add(VariantType.Machine, Environment.MachineName);
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\VariantConfiguration", false))
					{
						if (registryKey != null)
						{
							foreach (VariantType variantType in ConstraintCollection.RegistryValueNames)
							{
								object value = registryKey.GetValue(variantType.Name);
								if (value != null)
								{
									constraintCollection.Add(variantType.Name, value.ToString());
								}
							}
						}
					}
					if (ConstraintCollection.Mode.Equals("datacenter"))
					{
						string region = ConstraintCollection.GetRegion();
						if (!string.IsNullOrEmpty(region))
						{
							constraintCollection.Add(VariantType.Region, region);
						}
					}
					constraintCollection.Add(VariantType.Process, ConstraintCollection.GetProcessName());
					lock (ConstraintCollection.lockGlobalConstraints)
					{
						if (ConstraintCollection.globalConstraints == null)
						{
							ConstraintCollection.globalConstraints = constraintCollection;
						}
					}
				}
				return ConstraintCollection.globalConstraints;
			}
		}

		public static void SetGlobalConstraint(string variant, string value)
		{
			lock (ConstraintCollection.lockGlobalConstraints)
			{
				ConstraintCollection.globalConstraints = new ConstraintCollection(ConstraintCollection.GlobalConstraints)
				{
					{
						variant,
						value
					}
				};
			}
		}

		public static ConstraintCollection CreateEmpty()
		{
			return new ConstraintCollection();
		}

		public static ConstraintCollection CreateGlobal()
		{
			return new ConstraintCollection(ConstraintCollection.GlobalConstraints);
		}

		public void Add(ConstraintCollection constraints)
		{
			foreach (KeyValuePair<string, string> keyValuePair in constraints)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public void Add(VariantType variant, string value)
		{
			this.Add(variant.Name, value);
		}

		public void Add(VariantType variant, bool value)
		{
			this.Add(variant.Name, value);
		}

		public void Add(VariantType variant, Guid value)
		{
			this.Add(variant.Name, value.ToString());
		}

		public void Add(VariantType variant, Version value)
		{
			this.Add(variant.Name, string.Format("{0}.{1:00}.{2:0000}.{3:000}", new object[]
			{
				value.Major,
				value.Minor,
				value.Build,
				value.Revision
			}));
		}

		public new void Add(string key, string value)
		{
			base[key] = value;
		}

		public void Add(string key, bool value)
		{
			if (value)
			{
				this.Add(key, bool.TrueString);
				return;
			}
			base.Remove(key);
		}

		private static string GetProcessName()
		{
			string fileNameWithoutExtension;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				fileNameWithoutExtension = Path.GetFileNameWithoutExtension(currentProcess.ProcessName);
			}
			return fileNameWithoutExtension;
		}

		private static string GetRegion()
		{
			string domainName;
			try
			{
				IPGlobalProperties ipglobalProperties = IPGlobalProperties.GetIPGlobalProperties();
				if (ipglobalProperties == null)
				{
					return null;
				}
				domainName = ipglobalProperties.DomainName;
			}
			catch (NetworkInformationException)
			{
				return null;
			}
			if (domainName.Length < 3)
			{
				return null;
			}
			if (domainName.Equals("prod.exchangelabs.com", StringComparison.OrdinalIgnoreCase))
			{
				return "nam";
			}
			return domainName.Substring(0, 3);
		}

		internal const string ModeEnterprise = "enterprise";

		internal const string ModeDedicated = "dedicated";

		internal const string ModeDatacenter = "datacenter";

		private const string VariantConfigurationKeyBasePath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\VariantConfiguration";

		private static readonly VariantType[] RegistryValueNames = new VariantType[]
		{
			VariantType.Dag,
			VariantType.Forest,
			VariantType.Primary,
			VariantType.Pod,
			VariantType.Service,
			VariantType.Test
		};

		private static string mode;

		private static ConstraintCollection globalConstraints;

		private static object lockGlobalConstraints = new object();
	}
}
