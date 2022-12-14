using System;
using SException = System.Exception;

namespace BitFramework.PromiseModule
{
    public class ExceptionEventArgs
    {
        public SException exception { get; private set; }

        internal ExceptionEventArgs(SException exception)
        {
            this.exception = exception;
        }
    }
}