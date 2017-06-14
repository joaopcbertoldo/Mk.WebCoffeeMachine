
//Setup Output
int ledPin_3 = 3;

bool foundFlag = false;
const int BUFFERSIZE = 350;

byte inputBuffer[5];

void setup() {
  pinMode(ledPin_3, OUTPUT);
  Serial.begin(9600);
  digitalWrite(ledPin_3, HIGH);
  delay(500);
  digitalWrite(ledPin_3, LOW);
  delay(500);
  
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
  delay(2000);
}

//Main Loop

void loop() {
    if (Serial)
    {
      String request = "generic request 0, generic request 1, generic request 2, generic request 3, generic request 4, generic request 5, generic request 6, generic request 7, generic request 8, generic request 9";
      delay(1000);
      Serial.println(request);
      delay(1000);
      String response = ReadResponse();
      delay(1000);
      Serial.println(response);
    }
}

String ReadResponse(){
    char response[BUFFERSIZE] = "";
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

