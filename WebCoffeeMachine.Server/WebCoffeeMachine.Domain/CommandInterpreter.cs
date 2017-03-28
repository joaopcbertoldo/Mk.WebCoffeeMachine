using System;

namespace WebCoffeeMachine.Domain
{
    public abstract class CommandInterpreter
    {
        public abstract void HandleCommand(ConsoleKeyInfo key);
    }
}