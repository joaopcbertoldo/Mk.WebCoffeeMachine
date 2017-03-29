using System;

namespace Mkfeina.Domain
{
    public abstract class CommandInterpreter
    {
        public abstract void HandleCommand(ConsoleKeyInfo key);
    }
}