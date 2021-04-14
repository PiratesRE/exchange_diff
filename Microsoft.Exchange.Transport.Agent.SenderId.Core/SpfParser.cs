using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class SpfParser
	{
		private SpfParser(string record, bool expEnabled)
		{
			this.s = record;
			this.index = 0;
			this.lookahead = ((this.s.Length > 0) ? this.s[this.index] : 'ÿ');
			this.expEnabled = expEnabled;
			this.parseStatus = SpfParser.ParseStatus.Success;
		}

		public static SpfTerm ParseSpfRecord(SenderIdValidationContext context, string record)
		{
			SpfParser spfParser = new SpfParser(record, false);
			return spfParser.ParseSpfRecord(context);
		}

		public static MacroTermSpfNode ParseExpMacro(string record)
		{
			SpfParser spfParser = new SpfParser(record, true);
			return spfParser.ParseExpMacro();
		}

		private static bool IsLegalUnknownModifierArgument(char c)
		{
			return c != 'ÿ' && c != ' ';
		}

		private static bool SaveMacroLiteral(MacroTermSpfNode p, ref StringBuilder literal)
		{
			if (literal.Length > 0)
			{
				p.Next = new MacroLiteralSpfNode(literal.ToString());
				literal = new StringBuilder();
				return true;
			}
			return false;
		}

		private static bool IsLegalIPAddressCharacter(char c)
		{
			return char.IsDigit(c) || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') || c == '.' || c == ':';
		}

		private static SpfTerm CreateSpfChain(SenderIdValidationContext context, SpfParser.SpfNode ast)
		{
			SpfParser.SpfNode spfNode = ast;
			RedirectSpfModifier redirectSpfModifier = null;
			ExpSpfModifier expSpfModifier = null;
			SpfTerm spfTerm = new UnknownSpfMechanism(context);
			SpfTerm spfTerm2 = spfTerm;
			int num = 0;
			while (spfNode != null)
			{
				if (spfNode.RequiresDns)
				{
					num++;
				}
				if (spfNode.RequiresDns && num > 10)
				{
					spfTerm2 = spfTerm2.Append(new UnknownSpfMechanism(context));
				}
				else if (spfNode.IsMechanism)
				{
					spfTerm2 = spfTerm2.Append(spfNode.CreateMechanism(context));
				}
				else if (spfNode.IsRedirect)
				{
					if (redirectSpfModifier != null)
					{
						context.SetInvalid();
						return null;
					}
					redirectSpfModifier = spfNode.CreateRedirect(context);
				}
				else if (spfNode.IsExp)
				{
					if (expSpfModifier != null)
					{
						context.SetInvalid();
						return null;
					}
					expSpfModifier = spfNode.CreateExp(context);
				}
				spfNode = spfNode.Next;
			}
			if (redirectSpfModifier != null)
			{
				spfTerm2 = spfTerm2.Append(redirectSpfModifier);
			}
			else
			{
				spfTerm2 = spfTerm2.Append(new NoMatch(context));
			}
			context.AddExpModifier(expSpfModifier);
			return spfTerm.Next;
		}

		private SpfTerm ParseSpfRecord(SenderIdValidationContext context)
		{
			SpfParser.SpfNode spfNode = null;
			this.OptionalParseSpaces();
			if (this.lookahead != 'ÿ')
			{
				spfNode = this.ParseSpfTerm();
				SpfParser.SpfNode spfNode2 = spfNode;
				while (this.OptionalParseSpaces() && this.lookahead != 'ÿ')
				{
					spfNode2.Next = this.ParseSpfTerm();
					spfNode2 = spfNode2.Next;
				}
				this.Parse('ÿ');
			}
			if (this.parseStatus == SpfParser.ParseStatus.Success)
			{
				return SpfParser.CreateSpfChain(context, spfNode);
			}
			context.SetInvalid();
			return null;
		}

		private SpfParser.SpfNode ParseSpfTerm()
		{
			bool flag = true;
			SenderIdStatus prefix = SenderIdStatus.Pass;
			char c = this.lookahead;
			switch (c)
			{
			case '+':
				this.Advance();
				prefix = SenderIdStatus.Pass;
				goto IL_5A;
			case ',':
				break;
			case '-':
				this.Advance();
				prefix = SenderIdStatus.Fail;
				goto IL_5A;
			default:
				if (c == '?')
				{
					this.Advance();
					prefix = SenderIdStatus.Neutral;
					goto IL_5A;
				}
				if (c == '~')
				{
					this.Advance();
					prefix = SenderIdStatus.SoftFail;
					goto IL_5A;
				}
				break;
			}
			flag = false;
			IL_5A:
			string text = this.ParseName();
			string key;
			switch (key = text)
			{
			case "all":
				return this.ParseAllMechanism(prefix);
			case "include":
				return this.ParseIncludeMechanism(prefix);
			case "a":
				return this.ParseAMechanism(prefix);
			case "mx":
				return this.ParseMxMechanism(prefix);
			case "ptr":
				return this.ParsePtrMechanism(prefix);
			case "ip4":
				return this.ParseIP4Mechanism(prefix);
			case "ip6":
				return this.ParseIP6Mechanism(prefix);
			case "exists":
				return this.ParseExistsMechanism(prefix);
			case "redirect":
				if (!flag)
				{
					return this.ParseRedirectModifier();
				}
				this.SetError(SpfParser.ParseStatus.PrefixSpecifiedForNonMechanism);
				goto IL_1C5;
			case "exp":
				if (!flag)
				{
					return this.ParseExpModifier();
				}
				this.SetError(SpfParser.ParseStatus.PrefixSpecifiedForNonMechanism);
				goto IL_1C5;
			}
			if (!flag && this.lookahead == '=')
			{
				return this.ParseUnknownModifier();
			}
			this.SetError(SpfParser.ParseStatus.UnrecognizedMechanism);
			IL_1C5:
			this.SetError(SpfParser.ParseStatus.IllegalCharacter);
			return null;
		}

		private MacroTermSpfNode ParseExpMacro()
		{
			MacroTermSpfNode result = this.ParseDomainSpec();
			if (!this.Parse('ÿ'))
			{
				return null;
			}
			return result;
		}

		private string ParseName()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				char c = this.lookahead;
				switch (c)
				{
				case '-':
				case '.':
					break;
				default:
					if (c != '_')
					{
						if (this.lookahead != 'ÿ' && char.IsLetterOrDigit(this.lookahead))
						{
							stringBuilder.Append(this.Advance());
							continue;
						}
						goto IL_5B;
					}
					break;
				}
				stringBuilder.Append(this.Advance());
			}
			IL_5B:
			return stringBuilder.ToString().ToLowerInvariant();
		}

		private SpfParser.SpfNode ParseAllMechanism(SenderIdStatus prefix)
		{
			return new SpfParser.AllSpfMechanismNode(prefix);
		}

		private SpfParser.SpfNode ParseIncludeMechanism(SenderIdStatus prefix)
		{
			this.Parse(':');
			return new SpfParser.IncludeSpfMechanismNode(prefix, this.ParseDomainSpec());
		}

		private SpfParser.SpfNode ParseAMechanism(SenderIdStatus prefix)
		{
			MacroTermSpfNode domainSpec = this.OptionalParse(':') ? this.ParseDomainSpec() : null;
			SpfParser.DualCidrLengthSpfNode dualCidrLength = (this.lookahead == '/') ? this.ParseDualCidrLength() : SpfParser.MaxDualCidrLength;
			return new SpfParser.ASpfMechanismNode(prefix, domainSpec, dualCidrLength);
		}

		private SpfParser.SpfNode ParseMxMechanism(SenderIdStatus prefix)
		{
			MacroTermSpfNode domainSpec = this.OptionalParse(':') ? this.ParseDomainSpec() : null;
			SpfParser.DualCidrLengthSpfNode dualCidrLength = (this.lookahead == '/') ? this.ParseDualCidrLength() : SpfParser.MaxDualCidrLength;
			return new SpfParser.MxSpfMechanismNode(prefix, domainSpec, dualCidrLength);
		}

		private SpfParser.SpfNode ParsePtrMechanism(SenderIdStatus prefix)
		{
			MacroTermSpfNode domainSpec = this.OptionalParse(':') ? this.ParseDomainSpec() : null;
			return new SpfParser.PtrSpfMechanismNode(prefix, domainSpec);
		}

		private SpfParser.SpfNode ParseIP4Mechanism(SenderIdStatus prefix)
		{
			this.Parse(':');
			IPAddress ip4Network = this.ParseIP4Network();
			int ip4CidrLength = (this.lookahead == '/') ? this.ParseIP4CidrLength() : 32;
			return new SpfParser.IP4SpfMechanismNode(prefix, ip4Network, ip4CidrLength);
		}

		private SpfParser.SpfNode ParseIP6Mechanism(SenderIdStatus prefix)
		{
			this.Parse(':');
			IPAddress ip6Network = this.ParseIP6Network();
			int ip6CidrLength = (this.lookahead == '/') ? this.ParseIP6CidrLength() : 128;
			return new SpfParser.IP6SpfMechanismNode(prefix, ip6Network, ip6CidrLength);
		}

		private SpfParser.SpfNode ParseExistsMechanism(SenderIdStatus prefix)
		{
			this.Parse(':');
			return new SpfParser.ExistsSpfMechanismNode(prefix, this.ParseDomainSpec());
		}

		private SpfParser.SpfNode ParseRedirectModifier()
		{
			this.Parse('=');
			return new SpfParser.RedirectSpfNode(this.ParseDomainSpec());
		}

		private SpfParser.SpfNode ParseExpModifier()
		{
			this.Parse('=');
			return new SpfParser.ExpSpfNode(this.ParseDomainSpec());
		}

		private SpfParser.SpfNode ParseUnknownModifier()
		{
			this.Parse('=');
			while (SpfParser.IsLegalUnknownModifierArgument(this.lookahead))
			{
				this.Advance();
			}
			return new SpfParser.UnknownModifierSpfNode();
		}

		private MacroTermSpfNode ParseDomainSpec()
		{
			MacroTermSpfNode macroTermSpfNode = new MacroLiteralSpfNode(string.Empty);
			MacroTermSpfNode macroTermSpfNode2 = macroTermSpfNode;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			while (!flag)
			{
				char c = this.lookahead;
				if (c == '%')
				{
					MacroTermSpfNode macroTermSpfNode3 = this.ParseMacroEscape(stringBuilder);
					if (macroTermSpfNode3 != null)
					{
						if (SpfParser.SaveMacroLiteral(macroTermSpfNode2, ref stringBuilder))
						{
							macroTermSpfNode2 = macroTermSpfNode2.Next;
						}
						macroTermSpfNode2.Next = macroTermSpfNode3;
						macroTermSpfNode2 = macroTermSpfNode2.Next;
					}
				}
				else if (this.IsLegalLiteralCharacter(this.lookahead))
				{
					stringBuilder.Append(this.Advance());
				}
				else
				{
					flag = true;
				}
			}
			SpfParser.SaveMacroLiteral(macroTermSpfNode2, ref stringBuilder);
			if (macroTermSpfNode2.Next != null)
			{
				macroTermSpfNode2 = macroTermSpfNode2.Next;
			}
			if (!macroTermSpfNode2.IsExpand && !this.expEnabled)
			{
				string literal = ((MacroLiteralSpfNode)macroTermSpfNode2).Literal;
				int num = literal.LastIndexOf('.');
				if (num == -1 || num == literal.Length - 1)
				{
					this.SetError(SpfParser.ParseStatus.InvalidDomainSpec);
					return null;
				}
			}
			return macroTermSpfNode.Next;
		}

		private bool IsLegalLiteralCharacter(char c)
		{
			return (c >= '!' && c <= '$') || (c >= '&' && c <= '.') || (c >= '0' && c <= '~') || (this.expEnabled && (c == ' ' || c == '/'));
		}

		private MacroTermSpfNode ParseMacroEscape(StringBuilder literal)
		{
			this.Parse('%');
			char c = char.ToLowerInvariant(this.lookahead);
			if (c <= '-')
			{
				if (c == '%')
				{
					this.Advance();
					literal.Append('%');
					goto IL_FE;
				}
				if (c == '-')
				{
					this.Advance();
					literal.Append("%20");
					goto IL_FE;
				}
			}
			else
			{
				if (c == '_')
				{
					this.Advance();
					literal.Append(' ');
					goto IL_FE;
				}
				if (c == '{')
				{
					this.Advance();
					switch (char.ToLowerInvariant(this.lookahead))
					{
					case 'c':
					case 'd':
					case 'h':
					case 'i':
					case 'l':
					case 'o':
					case 'p':
					case 'r':
					case 's':
					case 't':
					case 'v':
						return this.ParseMacroExpand();
					}
					this.SetError(SpfParser.ParseStatus.IllegalMacroCharacter);
					goto IL_FE;
				}
			}
			this.SetError(SpfParser.ParseStatus.RequiredTokenNotFound);
			IL_FE:
			return null;
		}

		private MacroExpandSpfNode ParseMacroExpand()
		{
			char macroCharacter = this.Advance();
			int transformerDigits = char.IsDigit(this.lookahead) ? this.ParseNumber() : 0;
			bool transformerReverse = this.OptionalParse('r');
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			while (!flag)
			{
				char c = this.lookahead;
				switch (c)
				{
				case '+':
				case ',':
				case '-':
				case '.':
				case '/':
					break;
				default:
					if (c != '=' && c != '_')
					{
						flag = true;
						continue;
					}
					break;
				}
				stringBuilder.Append(this.Advance());
			}
			string text = stringBuilder.ToString();
			if (string.IsNullOrEmpty(text))
			{
				text = ".";
			}
			this.Parse('}');
			return new MacroExpandSpfNode(macroCharacter, transformerDigits, transformerReverse, text);
		}

		private SpfParser.DualCidrLengthSpfNode ParseDualCidrLength()
		{
			int num = 32;
			int ip6CidrLength = 128;
			this.Parse('/');
			if (char.IsDigit(this.lookahead))
			{
				num = this.ParseNumber();
				this.ValidateCidrLength(num, 32);
				if (this.lookahead == '/')
				{
					this.Parse('/');
					ip6CidrLength = this.ParseIP6CidrLength();
				}
			}
			else
			{
				ip6CidrLength = this.ParseIP6CidrLength();
			}
			return new SpfParser.DualCidrLengthSpfNode(num, ip6CidrLength);
		}

		private int ParseIP4CidrLength()
		{
			this.Parse('/');
			int num = this.ParseNumber();
			this.ValidateCidrLength(num, 32);
			return num;
		}

		private int ParseIP6CidrLength()
		{
			this.Parse('/');
			int num = this.ParseNumber();
			this.ValidateCidrLength(num, 128);
			return num;
		}

		private void ValidateCidrLength(int cidrLength, int maxCidrLength)
		{
			if (cidrLength < 0 || cidrLength > maxCidrLength)
			{
				this.SetError(SpfParser.ParseStatus.InvalidCidrLength);
			}
		}

		private IPAddress ParseIP4Network()
		{
			return this.ParseIPAddress(AddressFamily.InterNetwork);
		}

		private IPAddress ParseIP6Network()
		{
			return this.ParseIPAddress(AddressFamily.InterNetworkV6);
		}

		private IPAddress ParseIPAddress(AddressFamily addressFamily)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (SpfParser.IsLegalIPAddressCharacter(this.lookahead))
			{
				stringBuilder.Append(this.Advance());
			}
			IPAddress ipaddress;
			if (IPAddress.TryParse(stringBuilder.ToString(), out ipaddress))
			{
				if (ipaddress.AddressFamily == addressFamily)
				{
					return ipaddress;
				}
				this.SetError(SpfParser.ParseStatus.IncorrectAddressFamily);
			}
			else
			{
				this.SetError(SpfParser.ParseStatus.InvalidIPAddress);
			}
			return null;
		}

		private int ParseNumber()
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (char.IsDigit(this.lookahead))
			{
				stringBuilder.Append(this.Advance());
			}
			int result;
			if (int.TryParse(stringBuilder.ToString(), out result))
			{
				return result;
			}
			this.SetError(SpfParser.ParseStatus.RequiredTokenNotFound);
			return -1;
		}

		private bool Parse(char c)
		{
			if (!this.OptionalParse(c))
			{
				this.SetError(SpfParser.ParseStatus.RequiredTokenNotFound);
				return false;
			}
			return true;
		}

		private bool OptionalParse(char c)
		{
			if (this.lookahead == c)
			{
				this.Advance();
				return true;
			}
			return false;
		}

		private bool OptionalParseSpaces()
		{
			if (this.lookahead == ' ')
			{
				while (this.lookahead == ' ')
				{
					this.Advance();
				}
				return true;
			}
			return false;
		}

		private char Advance()
		{
			char result;
			if (this.parseStatus == SpfParser.ParseStatus.Success && this.index < this.s.Length)
			{
				result = this.lookahead;
				this.index++;
				this.lookahead = ((this.index < this.s.Length) ? this.s[this.index] : 'ÿ');
			}
			else
			{
				result = 'ÿ';
				this.lookahead = 'ÿ';
			}
			return result;
		}

		private void SetError(SpfParser.ParseStatus error)
		{
			if (this.parseStatus == SpfParser.ParseStatus.Success)
			{
				this.parseStatus = error;
				this.lookahead = 'ÿ';
			}
		}

		private const char EOF = 'ÿ';

		private const int IP4MaxCidrLength = 32;

		private const int IP6MaxCidrLength = 128;

		private const int MaxDnsBasedMechanisms = 10;

		private static readonly SpfParser.DualCidrLengthSpfNode MaxDualCidrLength = new SpfParser.DualCidrLengthSpfNode(32, 128);

		private string s;

		private int index;

		private char lookahead;

		private bool expEnabled;

		private SpfParser.ParseStatus parseStatus;

		private enum ParseStatus
		{
			Success = 1,
			UnconsumedCharacters,
			PrefixSpecifiedForNonMechanism,
			IllegalCharacter,
			IllegalMacroCharacter,
			IllegalLiteralCharacter,
			IllegalCidrPrefix,
			IncorrectAddressFamily,
			InvalidIPAddress,
			InvalidCidrLength,
			InvalidDomainSpec,
			RequiredTokenNotFound,
			UnrecognizedMechanism,
			RequiredDomainSpecMissing
		}

		private abstract class SpfNode
		{
			public SpfNode(bool isMechanism, bool isRedirect, bool isExp, bool requiresDns)
			{
				this.IsMechanism = isMechanism;
				this.IsRedirect = isRedirect;
				this.IsExp = isExp;
				this.RequiresDns = requiresDns;
			}

			public virtual SpfMechanism CreateMechanism(SenderIdValidationContext context)
			{
				return null;
			}

			public virtual RedirectSpfModifier CreateRedirect(SenderIdValidationContext context)
			{
				return null;
			}

			public virtual ExpSpfModifier CreateExp(SenderIdValidationContext context)
			{
				return null;
			}

			public SpfParser.SpfNode Next;

			public bool IsMechanism;

			public bool IsRedirect;

			public bool IsExp;

			public bool RequiresDns;
		}

		private abstract class SpfMechanismNode : SpfParser.SpfNode
		{
			public SpfMechanismNode(SenderIdStatus prefix, bool requiresDns) : base(true, false, false, requiresDns)
			{
				this.prefix = prefix;
			}

			protected SenderIdStatus prefix;
		}

		private class AllSpfMechanismNode : SpfParser.SpfMechanismNode
		{
			public AllSpfMechanismNode(SenderIdStatus prefix) : base(prefix, false)
			{
			}

			public override SpfMechanism CreateMechanism(SenderIdValidationContext context)
			{
				return new AllSpfMechanism(context, this.prefix);
			}
		}

		private class IncludeSpfMechanismNode : SpfParser.SpfMechanismNode
		{
			public IncludeSpfMechanismNode(SenderIdStatus prefix, MacroTermSpfNode domainSpec) : base(prefix, true)
			{
				this.DomainSpec = domainSpec;
			}

			public override SpfMechanism CreateMechanism(SenderIdValidationContext context)
			{
				return new IncludeSpfMechanism(context, this.prefix, this.DomainSpec);
			}

			public MacroTermSpfNode DomainSpec;
		}

		private class ASpfMechanismNode : SpfParser.SpfMechanismNode
		{
			public ASpfMechanismNode(SenderIdStatus prefix, MacroTermSpfNode domainSpec, SpfParser.DualCidrLengthSpfNode dualCidrLength) : base(prefix, true)
			{
				this.DomainSpec = domainSpec;
				this.DualCidrLength = dualCidrLength;
			}

			public override SpfMechanism CreateMechanism(SenderIdValidationContext context)
			{
				return new ASpfMechanism(context, this.prefix, this.DomainSpec, this.DualCidrLength.Ip4CidrLength, this.DualCidrLength.Ip6CidrLength);
			}

			public MacroTermSpfNode DomainSpec;

			public SpfParser.DualCidrLengthSpfNode DualCidrLength;
		}

		private class MxSpfMechanismNode : SpfParser.SpfMechanismNode
		{
			public MxSpfMechanismNode(SenderIdStatus prefix, MacroTermSpfNode domainSpec, SpfParser.DualCidrLengthSpfNode dualCidrLength) : base(prefix, true)
			{
				this.DomainSpec = domainSpec;
				this.DualCidrLength = dualCidrLength;
			}

			public override SpfMechanism CreateMechanism(SenderIdValidationContext context)
			{
				return new MxSpfMechanism(context, this.prefix, this.DomainSpec, this.DualCidrLength.Ip4CidrLength, this.DualCidrLength.Ip6CidrLength);
			}

			public MacroTermSpfNode DomainSpec;

			public SpfParser.DualCidrLengthSpfNode DualCidrLength;
		}

		private class PtrSpfMechanismNode : SpfParser.SpfMechanismNode
		{
			public PtrSpfMechanismNode(SenderIdStatus prefix, MacroTermSpfNode domainSpec) : base(prefix, true)
			{
				this.DomainSpec = domainSpec;
			}

			public override SpfMechanism CreateMechanism(SenderIdValidationContext context)
			{
				return new PtrSpfMechanism(context, this.prefix, this.DomainSpec);
			}

			public MacroTermSpfNode DomainSpec;
		}

		private class IP4SpfMechanismNode : SpfParser.SpfMechanismNode
		{
			public IP4SpfMechanismNode(SenderIdStatus prefix, IPAddress ip4Network, int ip4CidrLength) : base(prefix, false)
			{
				this.Ip4Network = ip4Network;
				this.Ip4CidrLength = ip4CidrLength;
			}

			public override SpfMechanism CreateMechanism(SenderIdValidationContext context)
			{
				return new IPSpfMechanism(context, this.prefix, IPNetwork.Create(this.Ip4Network, this.Ip4CidrLength));
			}

			public IPAddress Ip4Network;

			public int Ip4CidrLength;
		}

		private class IP6SpfMechanismNode : SpfParser.SpfMechanismNode
		{
			public IP6SpfMechanismNode(SenderIdStatus prefix, IPAddress ip6Network, int ip6CidrLength) : base(prefix, false)
			{
				this.Ip6Network = ip6Network;
				this.Ip6CidrLength = ip6CidrLength;
			}

			public override SpfMechanism CreateMechanism(SenderIdValidationContext context)
			{
				return new IPSpfMechanism(context, this.prefix, IPNetwork.Create(this.Ip6Network, this.Ip6CidrLength));
			}

			public IPAddress Ip6Network;

			public int Ip6CidrLength;
		}

		private class ExistsSpfMechanismNode : SpfParser.SpfMechanismNode
		{
			public ExistsSpfMechanismNode(SenderIdStatus prefix, MacroTermSpfNode domainSpec) : base(prefix, true)
			{
				this.DomainSpec = domainSpec;
			}

			public override SpfMechanism CreateMechanism(SenderIdValidationContext context)
			{
				return new ExistsSpfMechanism(context, this.prefix, this.DomainSpec);
			}

			public MacroTermSpfNode DomainSpec;
		}

		private class RedirectSpfNode : SpfParser.SpfNode
		{
			public RedirectSpfNode(MacroTermSpfNode domainSpec) : base(false, true, false, true)
			{
				this.DomainSpec = domainSpec;
			}

			public override RedirectSpfModifier CreateRedirect(SenderIdValidationContext context)
			{
				return new RedirectSpfModifier(context, this.DomainSpec);
			}

			public MacroTermSpfNode DomainSpec;
		}

		private class ExpSpfNode : SpfParser.SpfNode
		{
			public ExpSpfNode(MacroTermSpfNode domainSpec) : base(false, false, true, false)
			{
				this.DomainSpec = domainSpec;
			}

			public override ExpSpfModifier CreateExp(SenderIdValidationContext context)
			{
				return new ExpSpfModifier(context, this.DomainSpec);
			}

			public MacroTermSpfNode DomainSpec;
		}

		private class UnknownModifierSpfNode : SpfParser.SpfNode
		{
			public UnknownModifierSpfNode() : base(false, false, false, false)
			{
			}
		}

		private class DualCidrLengthSpfNode
		{
			public DualCidrLengthSpfNode(int ip4CidrLength, int ip6CidrLength)
			{
				this.Ip4CidrLength = ip4CidrLength;
				this.Ip6CidrLength = ip6CidrLength;
			}

			public int Ip4CidrLength;

			public int Ip6CidrLength;
		}
	}
}
