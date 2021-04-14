using System;

namespace System.Security
{
	[Serializable]
	internal sealed class SecurityDocumentElement : ISecurityElementFactory
	{
		internal SecurityDocumentElement(SecurityDocument document, int position)
		{
			this.m_document = document;
			this.m_position = position;
		}

		SecurityElement ISecurityElementFactory.CreateSecurityElement()
		{
			return this.m_document.GetElement(this.m_position, true);
		}

		object ISecurityElementFactory.Copy()
		{
			return new SecurityDocumentElement(this.m_document, this.m_position);
		}

		string ISecurityElementFactory.GetTag()
		{
			return this.m_document.GetTagForElement(this.m_position);
		}

		string ISecurityElementFactory.Attribute(string attributeName)
		{
			return this.m_document.GetAttributeForElement(this.m_position, attributeName);
		}

		private int m_position;

		private SecurityDocument m_document;
	}
}
