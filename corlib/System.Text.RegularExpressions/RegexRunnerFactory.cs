namespace System.Text.RegularExpressions
{
    using System;

    public abstract class RegexRunnerFactory
    {
        protected RegexRunnerFactory()
        {
        }

        protected internal abstract RegexRunner CreateInstance();
    }
}

