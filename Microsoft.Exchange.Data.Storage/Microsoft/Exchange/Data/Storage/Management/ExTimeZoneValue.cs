using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class ExTimeZoneValue : IEquatable<ExTimeZoneValue>, ISerializable
	{
		public ExTimeZone ExTimeZone
		{
			get
			{
				return this.exTimeZone;
			}
			private set
			{
				this.exTimeZone = value;
			}
		}

		public ExTimeZoneValue(ExTimeZone timeZone)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			this.ExTimeZone = timeZone;
		}

		public ExTimeZoneValue(TimeZoneInfo timeZoneInfo)
		{
			foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
			{
				if (string.Equals(exTimeZone.DisplayName, timeZoneInfo.DisplayName, StringComparison.OrdinalIgnoreCase))
				{
					this.ExTimeZone = exTimeZone;
					break;
				}
			}
			if (this.ExTimeZone == null)
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		protected ExTimeZoneValue(SerializationInfo information, StreamingContext context)
		{
			string @string = information.GetString("TimeZoneId");
			this.ExTimeZone = default(ExTimeZoneValue.StringParser).Parse(@string);
		}

		public static ExTimeZoneValue Parse(string timeZoneString)
		{
			return new ExTimeZoneValue(default(ExTimeZoneValue.StringParser).Parse(timeZoneString));
		}

		public static bool operator ==(ExTimeZoneValue left, ExTimeZoneValue right)
		{
			if (left != null)
			{
				return left.Equals(right);
			}
			return right == null;
		}

		public static bool operator !=(ExTimeZoneValue left, ExTimeZoneValue right)
		{
			return !(left == right);
		}

		public static bool TryParse(string timeZoneString, out ExTimeZoneValue instance)
		{
			ExTimeZone timeZone;
			bool flag = default(ExTimeZoneValue.StringParser).TryParse(timeZoneString, out timeZone);
			instance = (flag ? new ExTimeZoneValue(timeZone) : null);
			return flag;
		}

		public override string ToString()
		{
			return this.ExTimeZone.Id;
		}

		public bool Equals(ExTimeZoneValue other)
		{
			return other != null && this.ExTimeZone == other.ExTimeZone;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ExTimeZoneValue);
		}

		public override int GetHashCode()
		{
			return this.ExTimeZone.GetHashCode();
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("TimeZoneId", this.ExTimeZone.Id, typeof(string));
		}

		public const string GMTFormatPrefix = "GMT";

		private const string TimeZoneId = "TimeZoneId";

		[NonSerialized]
		private ExTimeZone exTimeZone;

		private struct StringParser
		{
			internal bool TryParse(string s, out ExTimeZone timeZone)
			{
				this.error = ExTimeZoneValue.StringParser.ParseError.NoError;
				this.multipleMatches = string.Empty;
				if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(s, out timeZone))
				{
					Match match = Regex.Match(s, "^GMT([+|-]?)([^+-]+)");
					if (match.Success)
					{
						string s2 = (match.Groups[1].Value == "-") ? ("-" + match.Groups[2].Value) : match.Groups[2].Value;
						TimeSpan t;
						if (TimeSpan.TryParse(s2, out t))
						{
							List<ExTimeZone> list = new List<ExTimeZone>();
							foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
							{
								if (exTimeZone.TimeZoneInformation.StandardBias == t)
								{
									list.Add(exTimeZone);
								}
							}
							if (list.Count == 1)
							{
								timeZone = list[0];
								goto IL_168;
							}
							if (list.Count == 0)
							{
								this.error = ExTimeZoneValue.StringParser.ParseError.NoGmtMatch;
								goto IL_168;
							}
							this.error = ExTimeZoneValue.StringParser.ParseError.MultipleGmtMatches;
							using (List<ExTimeZone>.Enumerator enumerator2 = list.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									ExTimeZone exTimeZone2 = enumerator2.Current;
									this.multipleMatches = this.multipleMatches + "\n\t" + exTimeZone2.Id;
								}
								goto IL_168;
							}
						}
						this.error = ExTimeZoneValue.StringParser.ParseError.WrongGmtFormat;
					}
					else
					{
						this.error = ExTimeZoneValue.StringParser.ParseError.TimeZoneNotFound;
					}
				}
				IL_168:
				return this.error == ExTimeZoneValue.StringParser.ParseError.NoError;
			}

			internal ExTimeZone Parse(string s)
			{
				ExTimeZone result = null;
				if (!this.TryParse(s, out result))
				{
					switch (this.error)
					{
					case ExTimeZoneValue.StringParser.ParseError.TimeZoneNotFound:
						throw new FormatException(ServerStrings.ErrorExTimeZoneValueTimeZoneNotFound);
					case ExTimeZoneValue.StringParser.ParseError.WrongGmtFormat:
						throw new FormatException(ServerStrings.ErrorExTimeZoneValueWrongGmtFormat);
					case ExTimeZoneValue.StringParser.ParseError.MultipleGmtMatches:
						throw new FormatException(ServerStrings.ErrorExTimeZoneValueMultipleGmtMatches(this.multipleMatches));
					case ExTimeZoneValue.StringParser.ParseError.NoGmtMatch:
						throw new FormatException(ServerStrings.ErrorExTimeZoneValueNoGmtMatch);
					}
				}
				return result;
			}

			private ExTimeZoneValue.StringParser.ParseError error;

			private string multipleMatches;

			private enum ParseError
			{
				NoError,
				TimeZoneNotFound,
				WrongGmtFormat,
				MultipleGmtMatches,
				NoGmtMatch
			}
		}
	}
}
