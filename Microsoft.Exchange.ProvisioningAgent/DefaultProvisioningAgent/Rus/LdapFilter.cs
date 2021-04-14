using System;
using Microsoft.Exchange.ProvisioningAgent;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal abstract class LdapFilter
	{
		private static bool iswxdigit(char c)
		{
			return "0123456789ABCDEFabcdef".IndexOf(c) != -1;
		}

		protected static bool LdapEscapable(char char1, char char2)
		{
			return LdapFilter.iswxdigit(char1) && LdapFilter.iswxdigit(char2);
		}

		public static LdapFilter Parse(string wsFilter, LdapFilterProvider provider)
		{
			if (string.IsNullOrEmpty(wsFilter))
			{
				throw new ArgumentNullException("wsFilter");
			}
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			LdapFilter ldapFilter = null;
			int num = 0;
			int length = wsFilter.Length;
			int i = num;
			bool flag = false;
			bool flag2 = false;
			int num2 = 0;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			char? c = null;
			bool flag6 = false;
			while (i < wsFilter.Length)
			{
				char c2 = wsFilter[i];
				if (c2 <= ')')
				{
					switch (c2)
					{
					case ' ':
						i++;
						continue;
					case '!':
						flag6 = true;
						if (c == '(')
						{
							int num6 = i + 1;
							while (wsFilter[num6] == ' ')
							{
								num6++;
							}
							if (wsFilter[num6] == '(')
							{
								if (!flag4)
								{
									ldapFilter = new NotLdapFilter();
									flag4 = true;
								}
								flag6 = false;
							}
						}
						break;
					default:
						switch (c2)
						{
						case '&':
							flag6 = true;
							if (c == '(')
							{
								int num7 = i + 1;
								while (wsFilter[num7] == ' ')
								{
									num7++;
								}
								if (wsFilter[num7] == '(')
								{
									if (!flag4)
									{
										ldapFilter = new AndLdapFilter();
										flag4 = true;
									}
									flag6 = false;
								}
							}
							break;
						case '\'':
							goto IL_3B0;
						case '(':
							if (flag6 && !flag2)
							{
								flag2 = true;
								goto IL_3B0;
							}
							if (flag4)
							{
								if (num5 == 0)
								{
									if (flag3 && ldapFilter is NotLdapFilter)
									{
										throw new LdapFilterException(Strings.LdapFilterErrorNotSupportSingleComp);
									}
									num2 = i;
									flag3 = true;
								}
								num5++;
							}
							if (!flag)
							{
								flag = true;
							}
							num4++;
							flag6 = false;
							break;
						case ')':
							if (flag2)
							{
								flag2 = false;
								goto IL_3B0;
							}
							if (!flag6 && c != ')')
							{
								throw new LdapFilterException(Strings.LdapFilterErrorInvalidToken);
							}
							num4--;
							if (num4 == 0)
							{
								flag5 = true;
							}
							else if (num4 < 0)
							{
								throw new LdapFilterException(Strings.LdapFilterErrorBracketMismatch);
							}
							if (flag4)
							{
								if (!flag5)
								{
									num5--;
									if (num5 == 0)
									{
										string wsFilter2 = wsFilter.Substring(num2, i + 1 - num2);
										LdapFilter ldapFilter2 = LdapFilter.Parse(wsFilter2, provider);
										if (ldapFilter2.GetType() == ldapFilter.GetType())
										{
											if (ldapFilter.GetType() == typeof(NotLdapFilter))
											{
												ldapFilter = ((NotLdapFilter)ldapFilter2).SubFilters[0];
											}
											else
											{
												((CompositionLdapFilter)ldapFilter).AddRange(((CompositionLdapFilter)ldapFilter2).SubFilters);
											}
										}
										else
										{
											((CompositionLdapFilter)ldapFilter).Add(ldapFilter2);
										}
									}
								}
							}
							else if (flag5)
							{
								return SinglePropertyLdapFilter.Parse(wsFilter, provider);
							}
							flag6 = false;
							break;
						default:
							goto IL_3B0;
						}
						break;
					}
				}
				else if (c2 != '\\')
				{
					if (c2 != '|')
					{
						goto IL_3B0;
					}
					flag6 = true;
					if (c == '(')
					{
						int num8 = i + 1;
						while (wsFilter[num8] == ' ')
						{
							num8++;
						}
						if (wsFilter[num8] == '(')
						{
							if (!flag4)
							{
								ldapFilter = new OrLdapFilter();
								flag4 = true;
							}
							flag6 = false;
						}
					}
				}
				else
				{
					if (c != '(' && !flag6)
					{
						char? c3 = c;
						int? num9 = (c3 != null) ? new int?((int)c3.GetValueOrDefault()) : null;
						if (num9 != null)
						{
							throw new LdapFilterException(Strings.LdapFilterErrorInvalidToken);
						}
					}
					if (i < length - 2 && LdapFilter.LdapEscapable(wsFilter[i + 1], wsFilter[i + 2]))
					{
						num3 = 2;
					}
					flag6 = true;
				}
				IL_410:
				c = new char?(wsFilter[i]);
				i += num3 + 1;
				num3 = 0;
				continue;
				IL_3B0:
				if (c != '(' && !flag6)
				{
					char? c4 = c;
					int? num10 = (c4 != null) ? new int?((int)c4.GetValueOrDefault()) : null;
					if (num10 != null)
					{
						throw new LdapFilterException(Strings.LdapFilterErrorInvalidToken);
					}
				}
				flag6 = true;
				goto IL_410;
			}
			if (num4 != 0)
			{
				throw new LdapFilterException(Strings.LdapFilterErrorBracketMismatch);
			}
			if ((ldapFilter is AndLdapFilter || ldapFilter is OrLdapFilter) && ((CompositionLdapFilter)ldapFilter).SubFilters.Count == 1)
			{
				ldapFilter = ((CompositionLdapFilter)ldapFilter).SubFilters[0];
			}
			if (!flag)
			{
				ldapFilter = SinglePropertyLdapFilter.Parse(wsFilter, provider);
			}
			return ldapFilter;
		}

		public virtual bool Evaluate(object[] marshalledAttributes)
		{
			return false;
		}
	}
}
