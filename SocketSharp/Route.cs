using System;

namespace SocketSharp;

public interface Route
{
    public string Name { get; }
    public string[] Methods { get; }

    public string GET();
}