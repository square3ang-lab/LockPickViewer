using System;

namespace KeyViewer.API;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class CommentAttribute : Attribute
{
	public string Comment { get; }

	public CommentAttribute(string comment)
	{
		Comment = comment;
	}
}
