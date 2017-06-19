// Copyright João Paulo Casagrande Bertoldo 2017
// USP

#include <ArduinoJson.h>	
#include "MkafeinaTypes.h"

#define REQUEST_BUFFER_SIZE 250

char* RegistrationOrOffsetsRequest(String mac, int msg, String uniqueName, OffsetsObj offsets)
{
	StaticJsonBuffer<400> buffer;
	JsonObject& root = buffer.createObject();
	
	root["mac"] = mac;
	root["msg"] = msg;
	root["un"]  = uniqueName;		
	
	JsonObject& ingredientsSetup = root.createNestedObject("stp");
	
	ingredientsSetup["ca"]   = offsets.CoffeeAvailable;
	ingredientsSetup["ce"]   = offsets.CoffeeEmptyOffset;
	ingredientsSetup["cf"]   = offsets.CoffeeFullOffset;
	
	ingredientsSetup["wa"]   = offsets.WaterAvailable;
	ingredientsSetup["we"]   = offsets.WaterEmptyOffset;
	ingredientsSetup["wf"]   = offsets.WaterFullOffset;
	
	ingredientsSetup["ma"]   = offsets.MilkAvailable;
	ingredientsSetup["me"]   = offsets.MilkEmptyOffset;
	ingredientsSetup["mf"]   = offsets.MilkFullOffset;
	
	ingredientsSetup["sa"]   = offsets.SugarAvailable;
	ingredientsSetup["se"]   = offsets.SugarEmptyOffset;
	ingredientsSetup["sf"]   = offsets.SugarFullOffset;
	
	char request[REQUEST_BUFFER_SIZE];
	root.printTo(request);
	return request;
}

char* SignalsRequest(String mac, float coffee, float water, float milk, float sugar, bool enabled)
{
	StaticJsonBuffer<400> buffer;

	JsonObject& root = buffer.createObject();
	
	root["mac"] = mac;
	root["msg"] = (int)Signals;

	JsonObject& ingredientsSignals = root.createNestedObject("sig");
	
	ingredientsSignals["c"]  = coffee;
	ingredientsSignals["w"]  = water;
	ingredientsSignals["m"]  = milk;
	ingredientsSignals["s"]  = sugar;
	ingredientsSignals["e"]  = enabled;

	char request[REQUEST_BUFFER_SIZE];
	root.printTo(request);
	return request;
}

char* ReadyRequest(String mac, String orderReference)
{
	StaticJsonBuffer<400>  ReadyBuffer;

	JsonObject& root = ReadyBuffer.createObject();

	root["mac"]  = mac;
	root["msg"]  = (int)Ready;
	root["oref"] = orderReference;

	char request[REQUEST_BUFFER_SIZE];
	root.printTo(request);
	return request;
}

char* GeneralRequest(String mac, int msg)
{
	StaticJsonBuffer<100> buffer;
	
	JsonObject& root = buffer.createObject();
	
	root["mac"] = mac;
	root["msg"] = msg;
	
	char request[REQUEST_BUFFER_SIZE];
	root.printTo(request);
	return request;
}

ResponseResults ReadJsonResponse(char* response)
{
	ResponseResults results = ResponseResults_NULL;

	if(DEBUG){
		Serial.println("");
		Serial.println("");
		Serial.println("no json reader");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println(response);
	}
	
	StaticJsonBuffer<150> jsonBuffer;
	JsonObject& root = jsonBuffer.parseObject(response);
	
	results.ResponseCode   = root["rc"].as<int>();
	results.Command        = root["c"].as<int>();
	results.Error          = root["e"].as<int>();
	
	if(DEBUG){
		Serial.print(results.ResponseCode  ); Serial.print("  ");
		Serial.print(results.Command       ); Serial.print("  ");
		Serial.print(results.Error         ); Serial.print("  ");
		Serial.print(results.OrderReference); Serial.print("  ");
		Serial.print(results.Recipe.Coffee);  Serial.print("  ");
		Serial.print(results.Recipe.Watter);  Serial.print("  ");
		Serial.print(results.Recipe.Milk  );  Serial.print("  ");
		Serial.println(results.Recipe.Sugar );
	}
	
	return results;
}

ResponseResults ReadJsonResponseWithOrder(char* response)
{
	ResponseResults results = ResponseResults_NULL;

	if(DEBUG){
		Serial.println("");
		Serial.println("");
		Serial.println("no json reader");//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		Serial.println(response);
	}
	
	StaticJsonBuffer<150> jsonBuffer;
	JsonObject& root = jsonBuffer.parseObject(response);
	
	results.ResponseCode   = root["rc"].as<int>();
	results.Command        = root["c"].as<int>();
	results.Error          = root["e"].as<int>();
	results.OrderReference = root["oref"].as<String>();
	
	JsonObject& recipe = root["rec"].as<JsonObject&>();
	if (recipe != JsonObject::invalid())
	{
		results.Recipe.Coffee = recipe["c"].as<int>();
	    results.Recipe.Watter = recipe["w"].as<int>();
	    results.Recipe.Milk   = recipe["m"].as<int>();
        results.Recipe.Sugar  = recipe["s"].as<int>();
	}
	
	if(DEBUG){
		Serial.print(results.ResponseCode  ); Serial.print("  ");
		Serial.print(results.Command       ); Serial.print("  ");
		Serial.print(results.Error         ); Serial.print("  ");
		Serial.print(results.OrderReference); Serial.print("  ");
		Serial.print(results.Recipe.Coffee);  Serial.print("  ");
		Serial.print(results.Recipe.Watter);  Serial.print("  ");
		Serial.print(results.Recipe.Milk  );  Serial.print("  ");
		Serial.println(results.Recipe.Sugar );
	}
	
	return results;
}



