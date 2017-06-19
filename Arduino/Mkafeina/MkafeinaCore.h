// Copyright João Paulo Casagrande Bertoldo 2017
// USP

#include "MkafeinaCommunications.h"
#include "MkafeinaProtocol.h"	

#define ONOFF_SWITCH    1
#define DISABLE_BUTTON  2
#define OFFSETS_BUTTON  3
#define REENABLE_BUTTON 4

#define REGISTERED_LED  7
#define ENABLED_LED     8

bool DEBUG = false;

// Signals
bool       Registered   = false;
bool       Enabled      = false;
float      Coffee       = 0;
float      Water        = 0;
float      Milk         = 0;
float      Sugar        = 0;
OffsetsObj CMOffsets;

// Registers
int       CommandReg  = (int)CommandVoid;
String    OrderRefReg = "";
RecipeObj RecipeReg   = RecipeObj_NULL;

// Flags
bool OnOffFlag;
bool DisableFlag;
bool OffsetsFlag;
bool ReenableFlag;

const String MyMac = "E841A7FCF5C";
const String MyName = "Prototipo";

void SetIO() 
{
	pinMode(ONOFF_SWITCH, INPUT);
	
	pinMode(DISABLE_BUTTON , INPUT_PULLUP);
	pinMode(OFFSETS_BUTTON , INPUT_PULLUP);
	pinMode(REENABLE_BUTTON, INPUT_PULLUP);
	
	pinMode(REGISTERED_LED, OUTPUT);
	pinMode(ENABLED_LED   , OUTPUT);
	
	digitalWrite(REGISTERED_LED, LOW);
	digitalWrite(ENABLED_LED   , LOW);
	
	//!!!!!!!!!!!!!!!!!!!!!!!!!!
	pinMode(13, OUTPUT);
	digitalWrite(13, LOW);
}

void SetCMOffsets(OffsetsObj offsets)
{
	CMOffsets = offsets;
}

void UpdateFlags()
{
	OnOffFlag    = digitalRead(ONOFF_SWITCH)    == HIGH;
	DisableFlag  = digitalRead(DISABLE_BUTTON)  == LOW;
	OffsetsFlag  = digitalRead(OFFSETS_BUTTON)  == LOW;
	ReenableFlag = digitalRead(REENABLE_BUTTON) == LOW;
}

void UpdateSignals()
{
	Coffee = 1.5;
	Water  = 2.6; 
	Sugar  = 3.2;
}

// AUXILIAR
int GetSize(char* ch)
{
	int tmp=0;
	while (*ch != '\0') {
		*ch++;
		tmp++;
	}
	return tmp + 1; // +1 para copiar o caracter nulo
}

// AUXILIAR
void CopyCharArray(char *original, char *copy)
{
	int size = GetSize(original);
	while(size--) {
		*copy = *original;
		copy++;
		original++;
	}
}

ResponseResults RegisterNoMatterWhat()
{
	ResponseResults results = ResponseResults_NULL;
	while(results.ResponseCode != (int)OK && results.Error != (int)MacAlreadyRegistered){
		OffsetsObj offsets = CMOffsets;
		char * request = RegistrationOrOffsetsRequest(MyMac, (int)Registration, MyName, offsets);		
		char * response = SendRequest(request);
		
		char responseCopy[RESPONSE_BUFFER_SIZE];
		CopyCharArray(response, responseCopy);
		
		if(DEBUG){
			Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			Serial.println("no RegisterNoMatterWhat");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			Serial.println(response);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		}
		
		// depois daqui a response se perder
		results = ReadJsonResponse(response);
		
		if (results.ResponseCode != 0){
			if (DEBUG){
				SendResponseEcho(responseCopy);
			}
		}
	}
	Registered = true;
	Enabled = true;
	return results;
}

bool TryToUnregister()
{
	char * request          = GeneralRequest(MyMac, (int)Unregistration);
	char * response         = SendRequest(request);
	
	char responseCopy[RESPONSE_BUFFER_SIZE];
	CopyCharArray(response, responseCopy);
	
	ResponseResults results = ReadJsonResponse(response);
	
	if (results.ResponseCode != 0){
		if (DEBUG){
			SendResponseEcho(responseCopy);
		}
	}
	
	return results.ResponseCode != 0 && results.Command == (int)Unregister;
}

bool TryToWarnDisabling()
{
	char * request          = GeneralRequest(MyMac, (int)Disabling);
	char * response         = SendRequest(request);
	
	char responseCopy[RESPONSE_BUFFER_SIZE];
	CopyCharArray(response, responseCopy);
	
	ResponseResults results = ReadJsonResponse(response);
	
	if (results.ResponseCode != 0){
		if (DEBUG){
			SendResponseEcho(responseCopy);
		}
		CommandReg = results.Command;
	}
	
	return results.ResponseCode != 0;
}

void TryToReportSignals()
{
	char * request  = SignalsRequest(MyMac, Coffee, Water, Milk, Sugar, Enabled);
	char * response = SendRequest(request);
	
	if(DEBUG){
		Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println("no TryToReportSignals");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println(response);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	}
	
	char responseCopy[RESPONSE_BUFFER_SIZE];
	CopyCharArray(response, responseCopy);
	
	ResponseResults results = ReadJsonResponse(response);
	
	if (results.ResponseCode != 0){
		if (DEBUG){
			SendResponseEcho(responseCopy);
		}
		CommandReg = results.Command;
	}
	else{
		CommandReg = (int)CommandUndef;
	}
}

bool TryToTakeOrder()
{
	char * request  = GeneralRequest(MyMac, (int)GiveMeAnOrder);
	char * response = SendRequest(request);
	
	char responseCopy[RESPONSE_BUFFER_SIZE];
	CopyCharArray(response, responseCopy);
	
	if(DEBUG){
		Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println("no try to take order");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println(response);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	}
	
	ResponseResults results = ReadJsonResponseWithOrder(response);
	
	if (results.ResponseCode != 0){
		if (DEBUG){
			Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			Serial.println("echo");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			SendResponseEcho(responseCopy);
		}
		CommandReg  = results.Command;
		OrderRefReg = results.OrderReference;
	    RecipeReg   = results.Recipe;
	}
	else{
		CommandReg = (int)CommandVoid;
		OrderRefReg = "";
	    RecipeReg   = RecipeObj_NULL;
	}
	
	return results.ResponseCode != 0;
}

bool TryReady()
{
	char * request  = ReadyRequest(MyMac, OrderRefReg);
	char * response = SendRequest(request);
	
	char responseCopy[RESPONSE_BUFFER_SIZE];
	CopyCharArray(response, responseCopy);
	
	ResponseResults results = ReadJsonResponse(response);
	
	if (results.ResponseCode != 0){
		if (DEBUG){
			SendResponseEcho(responseCopy);
		}
		CommandReg = results.Command;
	}
	else{
		CommandReg = (int)CommandVoid;
	}
	
	return results.ResponseCode != 0;
}

bool TryCancelOrders()
{
	char * request  = GeneralRequest(MyMac, (int)CancelOrders);
	char * response = SendRequest(request);
	
	char responseCopy[RESPONSE_BUFFER_SIZE];
	CopyCharArray(response, responseCopy);
	
	ResponseResults results = ReadJsonResponse(response);
	
	if (results.ResponseCode != 0){
		if (DEBUG){
			SendResponseEcho(responseCopy);
		}
		CommandReg = results.Command;
	}
	
	return results.ResponseCode != 0;
}

void ReenableNoMatterWhat()
{
	while(!Enabled){
		char * request = GeneralRequest(MyMac, (int)Reenable);		
		char * response = SendRequest(request);
		
		char responseCopy[RESPONSE_BUFFER_SIZE];
		CopyCharArray(response, responseCopy);
		
		// depois daqui a response se perder
		ResponseResults results = ReadJsonResponse(response);
		
		if (results.ResponseCode != 0){
			if (DEBUG){
				SendResponseEcho(responseCopy);
			}
			
			if(results.ResponseCode != (int)OK && results.Command == (int)Enable)
			{
				Enabled = true;
			}
		}
	}
}

bool OnOffChecking()
{
  if (!OnOffFlag)
  {
    bool ack = false;
    for (int i = 0; i < 20 && !ack; i++)
    { ack = TryToUnregister(); }
    
    Registered = false;
    Enabled    = false;

    // Wait for the on/off to become 1 again 
    while (!OnOffFlag) 
    { UpdateFlags(); } 
    
    ResponseResults res = RegisterNoMatterWhat();
    CommandReg = res.Command;
    return true;
  }
  return false;
}

bool DisableButtonChecking()
{
  if (DisableFlag)
  {
    Enabled = false;
    bool ack = false;
    for (int i = 0; i < 20 && !ack; i++)
    { ack = TryToWarnDisabling(); }
    return true;
  }
  return false;
}

bool ReenableButtonChecking()
{
  if(ReenableFlag)
  {
    Enabled = true;
    ReenableNoMatterWhat();
    return true;
  }
  return false;
}


bool MakeCoffee(){
  for(int count = 0; count < 10; count++)
  {
    digitalWrite(13, HIGH);
    delay(800);
    digitalWrite(13, LOW);
    delay(800);
  }
  return true;
  
}

void ProcessingProtocol()
{
  if (OrderRefReg.length() <= 1)
  { CommandReg = (int)TakeAnOrder; }
  else
  {
    bool ack = false;
    ack = MakeCoffee();
  
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



//// not available on the prototipe
//if (_sendOffsetsFlag)
//{
//_sendOffsetsFlag = false;
//ack = false;
//for (var i = 0; i < 20 && !ack; i++)
//{
//ack = _serverCaller.TryToSendNewOffsets(out commandReg);
//}
//continue;
//}
