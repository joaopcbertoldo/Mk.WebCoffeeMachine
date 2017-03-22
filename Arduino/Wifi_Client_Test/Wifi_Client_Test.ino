/*
 *  Simple HTTP get webclient test
 */

#include <ESP8266WiFi.h>

//const char* ssid     = "Demorados";
//const char* password = "astronauta3132";
const char* hostPtr = "192.168.0.101";


void setup() {
  String ssid;
  String password;
  Serial.begin(115200);
  
  while (true) {
    delay(100);
    
    Serial.println("Please enter the wi-fi name (ssid)");
    while(!Serial.available()){}
    ssid = Serial.readString();

    Serial.println("");
    Serial.println("Please enter the wi-fi password");
    while(!Serial.available()){}
    password = Serial.readString();
    
    WiFi.begin(&ssid[0], &password[0]);
    while (WiFi.status() == WL_IDLE_STATUS) {
      delay(500);
      Serial.print(".");
    }
  
    if(WiFi.status() == WL_CONNECTED){
      Serial.println("");
      Serial.println("WiFi connected");  
      Serial.println("IP address: ");
      Serial.println(WiFi.localIP());
      break;
    }
  }
}


void loop() {
 // Serial.flush();
  //Serial.println("");
 // Serial.println("Please enter the host");
  //while(!Serial.available()){}
  //String host = Serial.readString();
  
 // Serial.flush();
//  Serial.println("");
//  Serial.println("Please enter the port");
 // while(!Serial.available()){}
 // String portStr = Serial.readString();
 // int port = portStr.toInt();

  Serial.println("");
  Serial.print("connecting to ");
  Serial.println(hostPtr);

  // Use WiFiClient class to create TCP connections
  int port = 8081;
  WiFiClient client;

  if (!client.connect(hostPtr, port)) {
    Serial.println("connection failed");
      delay(500);
    return;
  }
  
  // We now create a URI for the request
  Serial.println("");
  Serial.println("Please enter the uri");
  while(!Serial.available()){}
  String uriStr = Serial.readString();

  String htttRequest = String("GET ") + uriStr + " HTTP/1.1\r\n" +
               "Host: " + hostPtr + "\r\n" + 
               "Connection: close\r\n\r\n";
               
  Serial.println("");
  Serial.println("HTTP Request");
  Serial.println(htttRequest);
  
  // This will send the request to the server
  client.print(htttRequest);
  delay(500);

  Serial.println();
  Serial.println("Response");
  // Read all the lines of the reply from server and print them to Serial
  while(client.available()){
    String line = client.readStringUntil('\r');
    Serial.print(line);
  }
  
  Serial.println();
  Serial.println("closing connection");
}
