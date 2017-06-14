// Copyright Benoit Blanchon 2014-2017
// MIT License
//
// Arduino JSON library
// https://bblanchon.github.io/ArduinoJson/
// If you like this project, please add a star!

//#include <ArduinoJson.h>
#include <Mkafeina.h>



void setup() {
  Serial.begin(9600);
  while (!Serial) {
    // wait serial port initialization
  }

  Test();
}

void loop() {
  // not used in this example
}
