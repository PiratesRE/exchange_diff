using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class ServiceProviderSettings
	{
		private ServiceProviderSettings(string providerName, string providerUrl, List<IPRange> ipRanges, List<TlsCertificate> tlsCertifcates, string originalExpression)
		{
			this.ProviderName = providerName;
			this.ProviderUrl = providerUrl;
			this.IPRanges = ipRanges;
			this.Certificates = tlsCertifcates;
		}

		public string ProviderName { get; set; }

		public string ProviderUrl { get; set; }

		public List<IPRange> IPRanges { get; set; }

		public List<TlsCertificate> Certificates { get; set; }

		public string Expression
		{
			get
			{
				return this.ToString();
			}
		}

		public override string ToString()
		{
			string format = "{0},{1},{2},{3}";
			object[] array = new object[4];
			array[0] = this.ProviderName;
			array[1] = this.ProviderUrl;
			object[] array2 = array;
			int num = 2;
			object obj;
			if (this.IPRanges == null)
			{
				obj = null;
			}
			else
			{
				obj = string.Join(";", from ipRange in this.IPRanges
				where ipRange != null
				select ipRange.ToString());
			}
			array2[num] = obj;
			object[] array3 = array;
			int num2 = 3;
			object obj2;
			if (this.Certificates == null)
			{
				obj2 = null;
			}
			else
			{
				obj2 = string.Join(";", from tlsCertificate in this.Certificates
				where tlsCertificate != null
				select tlsCertificate.ToString());
			}
			array3[num2] = obj2;
			return string.Format(format, array);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			ServiceProviderSettings serviceProviderSettings = obj as ServiceProviderSettings;
			if (serviceProviderSettings == null)
			{
				return false;
			}
			if (!string.Equals(this.ProviderName, serviceProviderSettings.ProviderName, StringComparison.InvariantCultureIgnoreCase) || !string.Equals(this.ProviderUrl, serviceProviderSettings.ProviderUrl, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			if (this.IPRanges != null && serviceProviderSettings.IPRanges != null)
			{
				if (this.IPRanges.Count != serviceProviderSettings.IPRanges.Count)
				{
					return false;
				}
				using (List<IPRange>.Enumerator enumerator = serviceProviderSettings.IPRanges.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IPRange ipRangeToCompare = enumerator.Current;
						if (!this.IPRanges.Any((IPRange ipRange) => ipRange.Equals(ipRangeToCompare)))
						{
							return false;
						}
					}
				}
			}
			if ((this.IPRanges == null && serviceProviderSettings.IPRanges != null) || (this.IPRanges != null && serviceProviderSettings.IPRanges == null))
			{
				return false;
			}
			if (this.Certificates != null && serviceProviderSettings.Certificates != null)
			{
				if (this.Certificates.Count != serviceProviderSettings.Certificates.Count)
				{
					return false;
				}
				using (List<TlsCertificate>.Enumerator enumerator2 = serviceProviderSettings.Certificates.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TlsCertificate certificateToCompare = enumerator2.Current;
						if (!this.Certificates.Any((TlsCertificate certificate) => certificate.Equals(certificateToCompare)))
						{
							return false;
						}
					}
				}
			}
			return (this.Certificates == null || serviceProviderSettings.Certificates != null) && (this.Certificates != null || serviceProviderSettings.Certificates == null);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public static ServiceProviderSettings Parse(string stringToParse)
		{
			ServiceProviderSettings result;
			string message;
			if (ServiceProviderSettings.TryParse(stringToParse, out result, out message))
			{
				return result;
			}
			throw new FormatException(message);
		}

		private static bool TryParse(string stringToParse, out ServiceProviderSettings settings, out string error)
		{
			settings = null;
			error = null;
			if (string.IsNullOrWhiteSpace(stringToParse))
			{
				error = "String passed is null or empty";
				return false;
			}
			string[] array = stringToParse.Split(new char[]
			{
				','
			});
			if (array.Length != 4)
			{
				error = "We need a CSV with 4 values - Provider name, Provider url, IP ranges and Certificate names";
				return false;
			}
			char[] separator = new char[]
			{
				';'
			};
			List<IPRange> list = null;
			List<TlsCertificate> list2 = null;
			bool flag = true;
			if (!string.IsNullOrWhiteSpace(array[2]))
			{
				list = new List<IPRange>();
				string[] array2 = array[2].Split(separator);
				foreach (string text in array2)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						IPRange item;
						if (IPRange.TryParse(text.Trim(), out item))
						{
							list.Add(item);
						}
						else
						{
							error = string.Format("{0}Invalid IP Range {1}.", error, text);
							flag = false;
						}
					}
				}
			}
			if (!string.IsNullOrWhiteSpace(array[3]))
			{
				list2 = new List<TlsCertificate>();
				string[] array4 = array[3].Split(separator);
				foreach (string text2 in array4)
				{
					if (!string.IsNullOrWhiteSpace(text2))
					{
						TlsCertificate item2;
						if (TlsCertificate.TryParse(text2.Trim(), out item2))
						{
							list2.Add(item2);
						}
						else
						{
							error = string.Format("{0}Invalid Tls certificate {1}.", error, text2);
							flag = false;
						}
					}
				}
			}
			if (flag)
			{
				settings = new ServiceProviderSettings(array[0].Trim(), array[1].Trim(), list, list2, stringToParse);
			}
			return flag;
		}
	}
}
