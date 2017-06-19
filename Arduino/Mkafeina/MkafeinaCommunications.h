// Copyright João Paulo Casagrande Bertoldo 2017
// USP

#define RESPONSE_BUFFER_SIZE 250
const int RESPONSE_TIMEOUT = 20000;

#define	SEND_START_BYTE    1
#define	SEND_END_BYTE      2
#define	RECEIVE_START_BYTE 3
#define	RECEIVE_END_BYTE   4

void SendStartBytes(){
	Serial.write(byte(SEND_START_BYTE)); delay(100);
	Serial.write(byte(SEND_START_BYTE)); delay(100);
	Serial.write(byte(SEND_START_BYTE)); delay(100);
}

void SendMessage(char request[]){
	Serial.print(request); delay(300);
}

void SendEndBytes(){
	Serial.write(byte(SEND_END_BYTE)); delay(100);
	Serial.write(byte(SEND_END_BYTE)); delay(100);
	Serial.write(byte(SEND_END_BYTE)); delay(100);
}

void ReceiveStartBytes(){
	unsigned long start = millis();
	bool startflag = false;
	bool timeout = false;
	int counter = 0;
	while(!startflag && !timeout){
		
		if(Serial.available() > 0){
			byte b = Serial.read();
			
			if (counter > 0 && b != byte(RECEIVE_START_BYTE)){
				counter = 0;
			}
			
			if (b == byte(RECEIVE_START_BYTE)){
				counter++;
			}
		}
		
		if(counter == 3){
			startflag = true;
		}
		
		if ( (millis()-start) >= RESPONSE_TIMEOUT )
		{
			timeout = true;
		}
	}
}

void ReceiveUntilEndBytes(char response[]){
	unsigned long start = millis();
	bool endflag = false;
	bool timeout = false;
	int counter = 0;
	int index = 0;
	while(!endflag && !timeout){
		delay(50);
		if(Serial.available() > 0){
			byte b = Serial.read();
			
			if (counter > 0 && b != byte(RECEIVE_END_BYTE)){
				counter = 0;
			}
			
			if (b == byte(RECEIVE_END_BYTE)){
				counter++;
			}
			else{
				response[index++] = (char)b;
			}
			
		}
		
		if(counter == 3){
			endflag = true;
		}
		
		if ( (millis()-start) >= RESPONSE_TIMEOUT )
		{
			timeout = true;
		}
	}
	response[index] = (char)'\0';
}

void SendResponseEcho(char response[]){
	SendStartBytes();
	SendMessage(response);
	SendEndBytes();
}

char* SendRequest(char request[]){
	while (Serial.available())
	{ char c = Serial.read(); }
	
	SendStartBytes();
	SendMessage(request);
	SendEndBytes();
	
	delay(3000);
	
	char response[RESPONSE_BUFFER_SIZE];
	ReceiveStartBytes();
	ReceiveUntilEndBytes(response);
	
	while (Serial.available())
	{ char c = Serial.read(); }

	delay(1000);
	
	if(DEBUG){
		Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println("");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println("no SendRequest");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println(response);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	}
	
	return response;
}

