const int DELAY = 5000;
const int BAUDRATE = 115200;
const char format100[] = "{\"registrationMessage\":%d,\"uniqueName\":\"%s\",\"coffeeEmptyOffset\":%d,\"coffeeFullOffset\":%d,\"waterEmptyOffset\":%d,\"waterFullOffset\":%d,\"milkEmptyOffset\":%d,\"milkFullOffset\":%d,\"sugarEmptyOffset\":%d,\"sugarFullOffset\":%d,\"mac\":\"%s\"}";
const char format101[] = "{\"registrationMessage\":%d,\"mac\":\"%s\"}";
      
const char * MakeRegistrationRequest(int registrationMessage, const char * uniqueName, int coffeeEmptyOffset, int coffeeFullOffset, int waterEmptyOffset, int waterFullOffset, int milkEmptyOffset, int milkFullOffset, int sugarEmptyOffset, int sugarFullOffset, const char * mac) {  
  char buf[350];
  switch(registrationMessage) {
    case 100: 
      sprintf(buf, format100, registrationMessage, uniqueName, coffeeEmptyOffset, coffeeFullOffset, waterEmptyOffset, waterFullOffset, milkEmptyOffset, milkFullOffset, sugarEmptyOffset, sugarFullOffset, mac);
      break;
      
    case 101:
      sprintf(buf, format101, registrationMessage, mac);
      break;
      
    case 200:
      break;
    case 300:
      break;
      
  }
  return buf;
}

void setup(){
Serial.begin(BAUDRATE);
}

void loop() {

// RegistrationRequest --> attempt (100)
// Sequencia dos números a serem preenchidos: registrationMessage (INT), uniqueName (str), coffeeEmptyOffset (INT), coffeeFullOffset (INT), waterEmptyOffset (INT), waterFullOffset (INT), milkEmptyOffset(INT), milkFullOffset(INT), sugarEmptyOffset (INT), sugarFullOffset(INT), mac (str)
int registrationMessage = 100;
char uniqueName[] = "CAFETEIRA DA MAIRA E DO ADRIANO";
int coffeeEmptyOffset = 0;
int coffeeFullOffset = 100;
int waterEmptyOffset = 0;
int waterFullOffset = 100;
int milkEmptyOffset = -1;
int milkFullOffset = -1;
int sugarEmptyOffset = 0;
int sugarFullOffset = 100;
char mac[] = "E841A7FCF5C";
char* msg = MakeRegistrationRequest(registrationMessage, uniqueName, coffeeEmptyOffset, coffeeFullOffset, waterEmptyOffset, waterFullOffset,  milkEmptyOffset, milkFullOffset, sugarEmptyOffset, sugarFullOffset, mac);
Serial.println(msg);
delay(DELAY);

// RegistrationRequest --> acceptance (101)
registrationMessage = 101;
msg = MakeRegistrationRequest(registrationMessage, " ", 0, 0, 0, 0,  0, 0, 0, 0, mac);
Serial.println(msg);
delay(DELAY);

// RegistrationRequest --> offsets (200)


// RegistrationRequest --> Unregister (300)


// ReportRequest --> Levels (100) 
//"{\"reportMessage\":100,\"isEnabled\":true,\"coffeeLevel\":100.0,\"waterLevel\":1000.0,\"milkLevel\":-1.0,\"sugarLevel\":-1.0,\"mac\":\"A088B4931364\"}"


// ReportRequest --> DisablingCoffeeMachine (900) 



// OrderRequest --> GiveMeAnOrder = 100   


// OrderRequest --> ProcessingWillStart = 200 


// OrderRequest --> OrderReady = 300  


// OrderRequest --> ProblemOcurredDuringProcessing = 400  

}
