﻿namespace WebCoffeeMachine.Server.Domain.ActionResults
{
    public enum RegistrationResultStatusEnum
    {
        Undefined = 0,
        Ok,
        UniqueNameAlreadyTaken,
        InvalidForms
    }
}