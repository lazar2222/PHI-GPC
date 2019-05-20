#ifndef MYPER
#define MYPER

#include <arduino.h>
#include <EEPROM.h>
#include "define.h"

const extern byte PERS_KAFTWERK;
const extern byte PERS_TRESHOLD;
const extern byte PERS_DEBOUNCE;
const extern byte PERS_ERF;

void WritePers(byte ID,byte val);

byte readPers(byte ID);

void RefreshPersistence();

#endif
