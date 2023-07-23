using System;

[Serializable]
public class SqlException : Exception
{
    public SqlException() : base() { }
    public SqlException(string message) : base(message) { }
    public SqlException(string message, Exception inner) : base(message, inner) { }
}