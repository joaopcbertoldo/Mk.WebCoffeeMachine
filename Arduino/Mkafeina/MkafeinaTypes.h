// Copyright João Paulo Casagrande Bertoldo 2017
// USP

#include <ArduinoJson.h>	

#define REQUEST_BUFFER_SIZE 250

typedef struct
{
	bool  CoffeeAvailable  ;
	float CoffeeEmptyOffset;
	float CoffeeFullOffset ;
	bool  WaterAvailable   ;
	float WaterEmptyOffset ;
	float WaterFullOffset  ;
	bool  MilkAvailable    ;
	float MilkEmptyOffset  ;
	float MilkFullOffset   ;
	bool  SugarAvailable   ;
	float SugarEmptyOffset ;
	float SugarFullOffset  ;
} OffsetsObj;

const OffsetsObj OffsetsObj_NULL = {
    false, 0, 0, false, 0, 0, false, 0, 0, false, 0, 0
};

typedef struct 
{
	int Coffee;
	int Watter;
	int Milk;
	int Sugar;
} RecipeObj;
 
const RecipeObj RecipeObj_NULL = {
    0, 0, 0, 0
};

typedef struct 
{
	int ResponseCode;
	int Command;
	int Error;
	String OrderReference;
	RecipeObj Recipe;
} ResponseResults;

const ResponseResults ResponseResults_NULL = {
    -1, -1, -1, "", RecipeObj_NULL
};

enum MessageEnum
{
	MessageUndef   = 0,
	
	Registration   = 1100,
	Offsets        = 1200,
	Unregistration = 1300,

	Signals        = 2100,
	Disabling      = 2200,
	Reenable       = 2300,

	GiveMeAnOrder  = 3100,
	Ready          = 3200,
	CancelOrders   = 3300
};

enum ResponseCodeEnum
{
	ResponseCodeUndef   = 0,
	OK                  = 200,
	InvalidRequest      = 400,
	InternalServerError = 401
};

enum CommandEnum
{
	CommandUndef = 0,
	CommandVoid  = 100,
	Disable      = 200,
	ProcessOrder = 300,
	Enable       = 400,
	Register     = 500,
	TakeAnOrder  = 600,
	Unregister   = 700
};

enum ErrorEnum
{
	ErrorUndef                         = 0,
	MacAlreadyRegistered               = 75,
	ErrorVoid                          = 76,
	MacNotRegistered                   = 77,
	UnknownMessage                     = 78,
	MissingIngredientsSetup            = 79,
	ShouldNotSentOffsets               = 80,
	MachineDisabledCannotTakeOrders    = 81,
	MachineAskedForOrderButThereIsNone = 82,
	OrderAlreadyTaken                  = 83,
	ShouldNotBeProcessing              = 84,
	ShouldBeAlreadyEnabled             = 85,
	WrongOrderReference                = 86,
	ShouldNotSendSignals               = 87,
	DisabledWithoutWarning             = 88
};