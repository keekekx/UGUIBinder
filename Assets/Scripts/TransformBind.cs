using System;

[AttributeUsage(AttributeTargets.Field)]
public class TransformBind : Attribute
{
    public string Path { get; set; }

    public TransformBind(string path)
    {
        Path = path;
    }
}