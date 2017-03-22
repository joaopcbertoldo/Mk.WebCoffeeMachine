namespace WebCoffeeMachine.Domain
{
    public enum MakeCoffeeResponseEnum
    {
        Undefined = 0,
        Ok,
        UnkownRecipe,
        NotEnoughIngredients,
        MachineIsDisconnected,
        Busy
    }
}