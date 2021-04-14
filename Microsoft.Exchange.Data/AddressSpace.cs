using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class AddressSpace
	{
		public AddressSpace(string addressSpace)
		{
			this.Initialize(addressSpace, true);
		}

		public AddressSpace(string addressSpace, bool includeSubDomain) : this("smtp", addressSpace, 1, includeSubDomain)
		{
		}

		public AddressSpace(string addressType, string addressSpace, int cost) : this(addressType, addressSpace, cost, false)
		{
		}

		public AddressSpace(string addressType, string addressSpace, int cost, bool includeSubDomain)
		{
			this.Initialize(addressType, addressSpace, cost, includeSubDomain, true);
		}

		private AddressSpace(SmtpDomainWithSubdomains obj)
		{
			this.type = "smtp";
			this.cost = 1;
			this.smtpDomainWithSubdomains = obj;
		}

		private AddressSpace()
		{
		}

		public string Address
		{
			get
			{
				if (this.addressSpace == null)
				{
					return this.smtpDomainWithSubdomains.Address;
				}
				return this.addressSpace;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public int Cost
		{
			get
			{
				return this.cost;
			}
			set
			{
				if (value < 1 || value > 100)
				{
					throw new ArgumentOutOfRangeException("Cost", value, DataStrings.AddressSpaceCostOutOfRange(1, 100));
				}
				this.cost = value;
			}
		}

		public bool IncludeSubDomains
		{
			get
			{
				return this.addressSpace == null && this.smtpDomainWithSubdomains.IncludeSubDomains;
			}
		}

		public string Domain
		{
			get
			{
				if (this.addressSpace == null)
				{
					return this.smtpDomainWithSubdomains.Domain;
				}
				return this.addressSpace;
			}
		}

		public bool IsSmtpType
		{
			get
			{
				return "smtp".Equals(this.Type, StringComparison.OrdinalIgnoreCase);
			}
		}

		public bool IsX400Type
		{
			get
			{
				return AddressSpace.IsX400AddressType(this.Type);
			}
		}

		public RoutingX400Address X400Address
		{
			get
			{
				return this.x400Address;
			}
		}

		internal bool IsLocal
		{
			get
			{
				return this.isLocal;
			}
		}

		internal SmtpDomainWithSubdomains DomainWithSubdomains
		{
			get
			{
				if (this.addressSpace == null)
				{
					return this.smtpDomainWithSubdomains;
				}
				return null;
			}
		}

		public bool Equals(AddressSpace value)
		{
			if (object.ReferenceEquals(this, value))
			{
				return true;
			}
			bool flag = false;
			if (value != null)
			{
				flag = (this.cost == value.cost && this.isLocal == value.isLocal && string.Equals(this.type, value.type, StringComparison.OrdinalIgnoreCase));
				if (flag)
				{
					if (this.addressSpace != null)
					{
						flag = this.addressSpace.Equals(value.addressSpace, StringComparison.OrdinalIgnoreCase);
					}
					else
					{
						flag = this.smtpDomainWithSubdomains.Equals(value.smtpDomainWithSubdomains);
					}
				}
			}
			return flag;
		}

		public override bool Equals(object comparand)
		{
			AddressSpace addressSpace = comparand as AddressSpace;
			return addressSpace != null && this.Equals(addressSpace);
		}

		public override int GetHashCode()
		{
			if (this.Domain != null)
			{
				return this.Domain.GetHashCode();
			}
			return 0;
		}

		public override string ToString()
		{
			return this.ToString(false);
		}

		public static AddressSpace Parse(string addressSpace)
		{
			return AddressSpace.Parse(addressSpace, true);
		}

		public static bool TryParse(string address, out AddressSpace addressSpace)
		{
			return AddressSpace.TryParse(address, out addressSpace, true);
		}

		internal static bool TryParse(string address, out AddressSpace addressSpace, bool performFullValidation)
		{
			try
			{
				addressSpace = AddressSpace.Parse(address, performFullValidation);
			}
			catch (FormatException)
			{
				addressSpace = null;
				return false;
			}
			return true;
		}

		internal static AddressSpace ADParse(string address)
		{
			bool flag = false;
			if (address.StartsWith("Local:", StringComparison.OrdinalIgnoreCase))
			{
				address = address.Substring("Local:".Length);
				flag = true;
			}
			AddressSpace addressSpace = AddressSpace.Parse(address, false);
			addressSpace.isLocal = flag;
			return addressSpace;
		}

		internal string ToString(bool includeScope)
		{
			string text = this.addressSpace;
			if (text != null && !this.IsSmtpType && !this.IsX400Type)
			{
				text = this.addressSpace.Replace("@", "(a)");
			}
			if (includeScope && this.isLocal)
			{
				return string.Format("{0}{1}:{2};{3}", new object[]
				{
					"Local:",
					this.type,
					(text != null) ? text : this.smtpDomainWithSubdomains.ToString(),
					this.cost
				});
			}
			return string.Format("{0}:{1};{2}", this.type, (text != null) ? text : this.smtpDomainWithSubdomains.ToString(), this.cost);
		}

		internal string ADToString()
		{
			return this.ToString(true);
		}

		internal AddressSpace ToggleScope()
		{
			return new AddressSpace
			{
				cost = this.cost,
				type = this.type,
				smtpDomainWithSubdomains = this.smtpDomainWithSubdomains,
				addressSpace = this.addressSpace,
				x400Address = this.x400Address,
				isLocal = !this.isLocal
			};
		}

		private void Initialize(string addressType, string addressSpace, int cost, bool includeSubDomain, bool performFullAddressTypeValidation)
		{
			if (string.IsNullOrEmpty(addressType))
			{
				throw new StrongTypeFormatException(DataStrings.InvalidAddressSpaceTypeNullOrEmpty, "Type");
			}
			this.type = addressType;
			this.Cost = cost;
			if (this.IsSmtpType)
			{
				this.smtpDomainWithSubdomains = new SmtpDomainWithSubdomains(addressSpace, includeSubDomain);
				return;
			}
			if (addressSpace != null)
			{
				addressSpace = addressSpace.ToLower();
			}
			if (this.IsX400Type)
			{
				if (!RoutingX400Address.TryParseAddressSpace(addressSpace, this.isLocal, out this.x400Address))
				{
					throw new StrongTypeFormatException(DataStrings.InvalidX400AddressSpace(addressSpace), "Domain");
				}
				this.addressSpace = addressSpace;
				return;
			}
			else
			{
				if (string.IsNullOrEmpty(addressSpace))
				{
					throw new StrongTypeFormatException(DataStrings.InvalidAddressSpaceAddress, "Domain");
				}
				if (performFullAddressTypeValidation && !ProxyAddressPrefix.IsPrefixStringValid(addressType))
				{
					throw new StrongTypeFormatException(DataStrings.InvalidAddressSpaceType(addressType), "Type");
				}
				this.addressSpace = addressSpace.Replace("(a)", "@");
				return;
			}
		}

		private void Initialize(string addressSpace, bool performFullAddressTypeValidation)
		{
			if (addressSpace.StartsWith("Local:", StringComparison.OrdinalIgnoreCase))
			{
				throw new FormatException(DataStrings.InvalidScopedAddressSpace(addressSpace));
			}
			int num = addressSpace.IndexOf(':');
			string addressType;
			if (num == -1 || num == 0)
			{
				addressType = "smtp";
			}
			else
			{
				addressType = addressSpace.Substring(0, num);
			}
			int num2 = addressSpace.LastIndexOf(';');
			if (AddressSpace.IsX400AddressType(addressType) && num2 == addressSpace.Length - 1)
			{
				num2 = -1;
			}
			string text;
			int num3;
			if (num2 == -1 || num2 <= num)
			{
				text = addressSpace.Substring(num + 1);
				num3 = 1;
			}
			else
			{
				text = addressSpace.Substring(num + 1, num2 - num - 1);
				if (num2 == addressSpace.Length - 1)
				{
					num3 = 1;
				}
				else if (!int.TryParse(addressSpace.Substring(num2 + 1), out num3))
				{
					throw new ArgumentException(DataStrings.InvalidAddressSpaceCostFormat(addressSpace.Substring(num2 + 1)), "Cost");
				}
			}
			this.Initialize(addressType, text, num3, false, performFullAddressTypeValidation);
		}

		private static bool IsX400AddressType(string type)
		{
			return "x400".Equals(type, StringComparison.OrdinalIgnoreCase);
		}

		private static AddressSpace Parse(string value, bool performFullValidation)
		{
			AddressSpace addressSpace = new AddressSpace();
			addressSpace.Initialize(value, performFullValidation);
			return addressSpace;
		}

		public const string SmtpAddressType = "smtp";

		public const string X400AddressType = "x400";

		public const int MinCostValue = 1;

		public const int MaxCostValue = 100;

		private const string ScopePrefix = "Local:";

		private const string E12At = "@";

		private const string TiAt = "(a)";

		private string type;

		private string addressSpace;

		private SmtpDomainWithSubdomains smtpDomainWithSubdomains;

		private RoutingX400Address x400Address;

		private int cost;

		private bool isLocal;
	}
}
