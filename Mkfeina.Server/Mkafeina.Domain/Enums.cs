using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mkfeina.Domain
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
