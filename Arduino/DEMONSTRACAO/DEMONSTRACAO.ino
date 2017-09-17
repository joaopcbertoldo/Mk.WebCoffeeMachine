//#include "MkafeinaTypes.h"
//#include "MkafeinaCommunications.h"
//#include "MkafeinaProtocol.h"  
#include "MkafeinaCore.h"

bool MONITOR = false;

void setup() 
{
  //pinMode(3,OUTPUT);

  pinMode(US_TRIG_C, OUTPUT);     // Sets the US_TRIG_C pin as an Output
  pinMode(US_ECHO_C, INPUT);      // Sets the US_ECHO_C pin as an Input
  digitalWrite(US_TRIG_C, LOW);

  pinMode(US_TRIG_S, OUTPUT);     // Sets the US_TRIG_S pin as an Output
  pinMode(US_ECHO_S, INPUT);      // Sets the US_ECHO_S pin as an Input
  digitalWrite(US_TRIG_S, LOW);

  pinMode(US_TRIG_W, OUTPUT);     // Sets the US_TRIG_C pin as an Output
  pinMode(US_ECHO_W, INPUT);      // Sets the US_ECHO_C pin as an Input
  digitalWrite(US_TRIG_W, LOW);

  pinMode(HEATER, OUTPUT);         // Sets the HEATER pin as an Output
  digitalWrite(HEATER, HIGH );      // HEATER starts disconnected

  pinMode(PUMP, OUTPUT);         // Sets the PUMP pin as an Output
  digitalWrite(PUMP, HIGH);       // PUMP starts closed

  servoCoffee.attach(SERVO_C);         // Sets the pin for controling servoCoffee
  servoSugar.attach(SERVO_S);           // Sets the pin for controling servoSugar

  servoCoffee.write(coffee_zero);          // Sets the zero positon for servoCoffee
  servoSugar.write(sugar_zero);           // Sets the zero positon for servoSugar

  
  DEBUG = true;
  
  Serial.begin(9600);
  Serial.flush();

  SetIO();

  OffsetsObj offsets = OffsetsObj_NULL;
  
  offsets.CoffeeAvailable   = true;
  offsets.CoffeeEmptyOffset = 0.1;
  offsets.CoffeeFullOffset  = 4.1;
  
  offsets.WaterAvailable    = true;
  offsets.WaterEmptyOffset  = 0.2;
  offsets.WaterFullOffset   = 4.2;
  
  offsets.SugarAvailable    = true;
  offsets.SugarEmptyOffset  = 0.3;
  offsets.SugarFullOffset   = 4.3;
  
  SetCMOffsets(offsets);
  
  ResponseResults res = RegisterNoMatterWhat();
  CommandReg = res.Command;
  //Enabled = false;                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  //Enabled = true;                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  //UpdateSignals();
} 

void loop() 
{
  //delay(1000);
  
  bool ack;

  //UpdateFlags();
  UpdateSignals();
  
  //if(OnOffChecking())
  //{ return; }

  if (Enabled)
  {
    //if(DisableButtonChecking())
    //{ return; }

    //CommandReg = (int)TakeAnOrder;        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11!!!!!!!!!!!!!!!!!!!!!!!!
    //Serial.println(CommandReg);
    switch (CommandReg){

      case (int)CommandVoid:
        TryToReportSignals();
        break;
      
      case (int)TakeAnOrder:
        ack = false;
        for (int i = 0; i < 20 && !ack; i++)
        { ack = TryToTakeOrder(); }
        break;

      case (int)ProcessOrder:
        ProcessingProtocol();
        break;

      case (int)Disable:
        Enabled = false;
        CommandReg = (int)CommandVoid;
        break;

      default:
        CommandReg = (int)CommandVoid;
        break;
    }
  }
  else
  {
    //if(! ReenableButtonChecking())
    //{ TryToReportSignals(); }
  }

  if(MONITOR)
  {
    Serial.println("");
  }
}

bool MakeCoffee(RecipeObj recipe){
  
  //tone(3,600);
  //delay(10000);
  //noTone(3);
  
  //for (int i = recipe.Coffee; i > 0; i--)
  for (int i = 1; i > 0; i--)
  { getCoffee(); }
  
  //for (int i = recipe.Sugar; i > 0; i--)
  for (int i = 1; i > 0; i--)
  { getSugar(); }
    
  heatWater();
  getwater();
  
  //for(int count = 0; count < 10; count++)
  //{
  //digitalWrite(13, HIGH);
  //delay(800);
  //digitalWrite(13, LOW);
  //delay(800);
  //}
  return true;
}

void ProcessingProtocol()
{
  if (OrderRefReg.length() <= 1)
  { CommandReg = (int)TakeAnOrder; }
  else
  {
    bool ack = false;
    ack = MakeCoffee(RecipeReg);
  
    if (ack)
    {
      ack = false;
      
      for (int i = 0; i < 7 && !ack; i++)
      { ack = TryReady(); }
    }
    else
    {
      ack = false;
      
      for (int i = 0; i < 7 && !ack; i++)
      { ack = TryCancelOrders(); }
  
      if (!ack)
      { CommandReg = (int)Disable; }
    }
  }
  OrderRefReg = "";
  RecipeReg   = RecipeObj_NULL;
}

