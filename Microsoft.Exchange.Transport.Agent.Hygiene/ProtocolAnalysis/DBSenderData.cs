using System;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	[Serializable]
	internal class DBSenderData : SenderData
	{
		public DBSenderData(DateTime tsCreate) : base(tsCreate)
		{
			this.heloUniqCnt = new SUniqueCount();
		}

		public SUniqueCount UniqueHelloCount
		{
			get
			{
				return this.heloUniqCnt;
			}
		}

		public int UniqueHelloDomainCount
		{
			get
			{
				return this.heloUniqCnt.Count();
			}
		}

		public void Merge(DBSenderData source)
		{
			base.Merge(source);
			this.heloUniqCnt.Merge(source.UniqueHelloCount);
		}

		public void Merge(FactorySenderData factoryData, string reverseDns, IPAddress senderIP, AcceptedDomainCollection acceptedDomains)
		{
			base.Merge(factoryData);
			IDictionaryEnumerator helloDomainEnumerator = factoryData.HelloDomainEnumerator;
			while (helloDomainEnumerator.MoveNext())
			{
				this.HelloDomainAnalysis((string)helloDomainEnumerator.Key, (int)helloDomainEnumerator.Value, reverseDns, senderIP, acceptedDomains);
			}
		}

		private static IPAddress GetHeloIPAddress(string helloDomain)
		{
			IPAddress result = IPAddress.Any;
			try
			{
				if (helloDomain.IndexOf(':') > 0)
				{
					result = IPAddress.Parse(helloDomain);
				}
				else if (!Regex.IsMatch(helloDomain, "[a-zA-Z_]"))
				{
					result = IPAddress.Parse(helloDomain);
				}
			}
			catch (FormatException)
			{
				result = IPAddress.Any;
			}
			return result;
		}

		private void HelloDomainAnalysis(string helloDomain, int numMsgs, string reverseDns, IPAddress senderIP, AcceptedDomainCollection acceptedDomains)
		{
			if (string.IsNullOrEmpty(helloDomain))
			{
				this.Helo[1] += numMsgs;
				return;
			}
			this.heloUniqCnt.AddItem(helloDomain);
			IPAddress heloIPAddress = DBSenderData.GetHeloIPAddress(helloDomain);
			if (heloIPAddress.Equals(IPAddress.Any))
			{
				bool flag = false;
				AcceptedDomain acceptedDomain = acceptedDomains.Find(helloDomain);
				if (acceptedDomain != null)
				{
					this.Helo[4] += numMsgs;
					flag = true;
				}
				if (!string.IsNullOrEmpty(reverseDns))
				{
					if (string.Compare(reverseDns, helloDomain, StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.Helo[2] += numMsgs;
						if (flag)
						{
							this.Helo[4] -= numMsgs;
						}
					}
					else
					{
						bool flag2 = false;
						string[] array = reverseDns.Split(new char[]
						{
							'.'
						});
						string[] array2 = helloDomain.Split(new char[]
						{
							'.'
						});
						if (array.Length >= 2 && array2.Length >= 2 && string.Compare(array[array.Length - 1], array2[array2.Length - 1], StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(array[array.Length - 2], array2[array2.Length - 2], StringComparison.OrdinalIgnoreCase) == 0)
						{
							if (array2[array2.Length - 1].Length <= 2)
							{
								if (array.Length >= 3 && array2.Length >= 3 && string.Compare(array[array.Length - 3], array2[array2.Length - 3], StringComparison.OrdinalIgnoreCase) == 0)
								{
									flag2 = true;
								}
							}
							else
							{
								flag2 = true;
							}
						}
						if (flag2)
						{
							this.Helo[3] += numMsgs;
							if (flag)
							{
								this.Helo[4] -= numMsgs;
							}
						}
						else
						{
							this.Helo[5] += numMsgs;
						}
					}
					if (this.Helo[0] != 0)
					{
						if (this.heloUniqCnt.Count() == 1)
						{
							if (this.Helo[2] != 0)
							{
								this.Helo[2] += this.Helo[0];
							}
							else if (this.Helo[3] != 0)
							{
								this.Helo[3] += this.Helo[0];
							}
							else if (this.Helo[5] != 0)
							{
								this.Helo[5] += this.Helo[0];
							}
						}
						this.Helo[0] = 0;
						return;
					}
				}
				else
				{
					this.Helo[0] += numMsgs;
				}
				return;
			}
			if (heloIPAddress.Equals(senderIP))
			{
				this.Helo[2] += numMsgs;
				return;
			}
			this.Helo[5] += numMsgs;
		}

		private SUniqueCount heloUniqCnt;
	}
}
