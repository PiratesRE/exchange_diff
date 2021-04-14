using System;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Test", "NewOrganizationName")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class TestNewOrganizationName : Task
	{
		[Parameter(Mandatory = true)]
		public string Value
		{
			get
			{
				return (string)base.Fields["Value"];
			}
			set
			{
				base.Fields["Value"] = value;
			}
		}

		private string ParameterName
		{
			get
			{
				return "Name";
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (char.IsWhiteSpace(this.Value[0]) || char.IsWhiteSpace(this.Value[this.Value.Length - 1]))
			{
				base.WriteError(new ArgumentException(Strings.ErrorLeadingTrailingWhitespaces(this.ParameterName, this.Value)), ErrorCategory.InvalidArgument, null);
			}
			if (!ADObjectNameHelper.ReservedADNameStringRegex.IsMatch(this.Value) && this.Value.Length > 64)
			{
				base.WriteError(new ArgumentException(Strings.ErrorNameValueStringTooLong(this.ParameterName, 64, this.Value.Length)), ErrorCategory.InvalidArgument, null);
			}
			int startIndex = -1;
			string str = CharacterConstraint.ConstructPattern(new char[]
			{
				'\0',
				'\n'
			}, false);
			if (ADObjectNameHelper.CheckIsUnicodeStringWellFormed(this.Value, out startIndex))
			{
				if (!ADObjectNameHelper.ReservedADNameStringRegex.IsMatch(this.Value) && !Regex.IsMatch(this.Value, "^" + str + "+$"))
				{
					base.WriteError(new ArgumentException(Strings.ErrorInvalidCharactersInParameterValue(this.ParameterName, this.Value, "'\0', '\n'")), ErrorCategory.InvalidArgument, null);
					return;
				}
			}
			else
			{
				base.WriteError(new ArgumentException(Strings.ErrorInvalidCharactersInParameterValue(this.ParameterName, this.Value, this.Value.Substring(startIndex, 1))), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}
