//#include "MkafeinaTypes.h"
//#include "MkafeinaCommunications.h"
//#include "MkafeinaProtocol.h"  
#include "MkafeinaCore.h"

bool MONITOR = false;

void setup() 
{
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
  
  UpdateSignals();
} 

void loop() 
{
  bool ack;

  UpdateFlags();

  //if(OnOffChecking())
  //{ return; }

  if (Enabled)
  {
    if(DisableButtonChecking())
    { return; }

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
    if(! ReenableButtonChecking())
    { TryToReportSignals(); }
  }

  if(MONITOR)
  {
    Serial.println("");
  }
}

