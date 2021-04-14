using System;

namespace System.Security
{
	internal interface ISecurityElementFactory
	{
		SecurityElement CreateSecurityElement();

		object Copy();

		string GetTag();

		string Attribute(string attributeName);
	}
}
