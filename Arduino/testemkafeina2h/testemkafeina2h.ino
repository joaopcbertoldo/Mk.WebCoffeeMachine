#include <Mkafeina2.h>

const String mac = "E841A7FCF5C";


void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  //HandShake();
}

String RegistrationRequest2(String mac, String uniqueName, bool coffeeAvailable, float coffeeEmptyOffset, float coffeeFullOffset, bool waterAvailable, float waterEmptyOffset, float waterFullOffset, bool milkAvailable, float milkEmptyOffset, float milkFullOffset, bool sugarAvailable, float sugarEmptyOffset, float sugarFullOffset)
{
  //"name", aJson.createItem("Jack (\"Bee\") Nimble"));
  //aJson.addItemToObject(root, "format", fmt = aJson.createObject());
  //aJson.addNumberToObject(fmt,"width",    1920);
  //aJson.addNumberToObject(fmt,"height",   1080);
  //aJson.addFalseToObject (fmt,"interlace");
  //aJson.addNumberToObject(fmt,"frame rate", 24);
  
  aJsonObject *root, *ingredientsSetup;
  
  root = aJson.createObject();  
  
  aJson.addNumberToObject(root, "msg", (int)Registration);

  
  char buf1[50]; 
  char buf2[50]; 
  mac.toCharArray(buf1, mac.length()+1);
  uniqueName.toCharArray(buf2, uniqueName.length()+1);
  aJson.addStringToObject(root, "mac", buf1);
  aJson.addStringToObject(root, "name", buf2);
  
  aJson.addItemToObject(root, "setup", ingredientsSetup = aJson.createObject());
  
  aJson.addBooleanToObject(ingredientsSetup, "cofAv", coffeeAvailable);
  aJson.addNumberToObject(ingredientsSetup, "cofEmp", coffeeEmptyOffset);
  aJson.addNumberToObject(ingredientsSetup, "cofFul", coffeeFullOffset);
  
  aJson.addBooleanToObject(ingredientsSetup, "watAv", waterAvailable);
  aJson.addNumberToObject(ingredientsSetup, "watEmp", waterEmptyOffset);
  aJson.addNumberToObject(ingredientsSetup, "watFul", waterFullOffset);
  
  aJson.addBooleanToObject(ingredientsSetup, "milAv", milkAvailable);
  aJson.addNumberToObject(ingredientsSetup, "milEmp", milkEmptyOffset);
  aJson.addNumberToObject(ingredientsSetup, "milFul", milkFullOffset);
  
  aJson.addBooleanToObject(ingredientsSetup, "sugAv", sugarAvailable);
  aJson.addNumberToObject(ingredientsSetup, "sugEmp", sugarEmptyOffset);
  aJson.addNumberToObject(ingredientsSetup, "sugFul", sugarFullOffset);
  
  char *output = aJson.print(root);
  return output;
}

void loop() {
   
  String request1 = RegistrationRequest2(mac, "Cafeteira da Maira e do Adriano", true, 0.2, 4.5, true, 0.2, 4.5, false, 0, 0, true, 0.2, 4.5);
  Serial.println(request1);
  Serial.println(request1.length());
  delay(500);
  ReadLine();
  
  String request2 = OffsetsRequest2(mac, true, 1.0, 1.1, true, 1, 1, false, 1, 1, true, 1, 1);
  Serial.println(request2);
  delay(500);
  ReadLine();
  
  String request3  = UnregistrationRequest2(mac);
  Serial.println(request3);
  delay(500);
  ReadLine();
  
  String request4 = SignalsRequest2(mac, 0.5, 1.5, 0.0, 4.0, true);
  Serial.println(request4);
  delay(500);
  ReadLine();
  
  String request5 = DisablingRequest2(mac);
  Serial.println(request5);
  ReadLine();
  
  String request6 = ReenableRequest2(mac);
  Serial.println(request6);
  ReadLine();
  
  String request7 = GiveMeAnOrderRequest2(mac);
  Serial.println(request7);
  ReadLine();
  
  String request8 = ReadyRequest2(mac, "001");
  Serial.println(request8);
  ReadLine();
  
  String request9 = CancelOrdersRequest2(mac);
  Serial.println(request9);
  ReadLine();
    
}


void ReadResponse(String response, ResponseCodeEnum *responseCode, CommandEnum *command, ErrorEnum *error)
{
  StaticJsonBuffer<350> jsonBuffer;

  JsonObject& root = jsonBuffer.parseObject(response);
  *responseCode = (ResponseCodeEnum) root[String("responseCode")].as<int>();
  *command =      (CommandEnum) root[String("command")].as<int>();
  *error =        (ErrorEnum) root[String("error")].as<int>();
}

void ReadGiveMeAnOrderResponse(String response, ResponseCodeEnum *responseCode, CommandEnum *command, ErrorEnum *error, String *orderReference, int *coffee, int *water, int *milk, int *sugar)
{
  StaticJsonBuffer<350> jsonBuffer;

  JsonObject& root = jsonBuffer.parseObject(response);
  *responseCode = (ResponseCodeEnum) root[String("responseCode")].as<int>(); //(ResponseCodeEnum)root[String("responseCode")].as<int>();
  *command      = (CommandEnum) root[String("command")].as<int>();      //(CommandEnum)root[String("command")].as<int>();
  *error        = (ErrorEnum) root[String("error")].as<int>();        //(ErrorEnum)root[String("error")].as<int>();
  
  *orderReference = root[String("orderReference")].as<String>();
  String recipeStr = root[String("recipe")].as<String>();
  JsonObject& recipe = jsonBuffer.parseObject(recipeStr);
  *coffee = recipe["coffee"]; //recipe["coffee"].as<int>;
  *water  = recipe["water"];  // recipe["water"].as<int>;
  *milk   = recipe["milk"];   //  recipe["milk"].as<int>;
  *sugar  = recipe["sugar"];  // recipe["sugar"].as<int>;
}

void HandShake(){
  bool foundFlag = false;
  byte inputBuffer[5];
  while(!foundFlag){
      if (Serial.available() == 5){
        //Read buffer
        inputBuffer[0] = Serial.read(); delay(100);    
        inputBuffer[1] = Serial.read(); delay(100);
        inputBuffer[2] = Serial.read(); delay(100);
        inputBuffer[3] = Serial.read(); delay(100);
        inputBuffer[4] = Serial.read(); delay(100);
  
        if(inputBuffer[0] == 16 && inputBuffer[1] == 128 && inputBuffer[2] == 0 && inputBuffer[3] == 0 && inputBuffer[4] == 4)
        {
            Serial.print("HELLO FROM ARDUINO");
            foundFlag = true;
        }
      }
    }
    Serial.flush();
    ReadLine();
}

String ReadLine(){
    char response[50] = "";
    bool allRead = false;
    int index = 0;
    while (!allRead){
        int inByte;
        if (Serial.available() > 0){
            inByte = Serial.read(); 
            //delay(80);
            if (inByte == '\n')
            {
              allRead = true;
              response[index] = '\0';
            }
            else
            {
              response[index++] = inByte;
            }
            
        }
    }
    String str(response);
    str.trim();
    return str;
}

