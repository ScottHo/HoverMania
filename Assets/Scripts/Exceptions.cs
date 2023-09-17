using System;

[Serializable]
public class LevelFactoryException : Exception
{
    public LevelFactoryException() : base() { }
    public LevelFactoryException(string message) : base(message) { }
    public LevelFactoryException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class WebRequestException : Exception
{
    public WebRequestException() : base() { }
    public WebRequestException(string message) : base(message) { }
    public WebRequestException(string message, Exception inner) : base(message, inner) { }
}
