#ifndef MYGHP
#define MYGHP

#include <arduino.h>
#include "define.h"
#include "lib.h"
#include "Persistence.h"

const extern byte ANALOG_CHANGE;
const extern byte BUTTON_PRESS;
const extern byte BUTTON_RELEASE;
const extern byte ENCODER_INCREMENT;
const extern byte ENCODER_DECREMENT;
const extern byte SYSTEM;
const extern byte SYSTEM_DISCOVER;

const extern byte SET_LED;
const extern byte SET_LCD;
const extern byte SET_LED_LEN;
const extern byte SET_LCD_LEN;

extern byte MsgBuffer[25];
extern bool DoUpdateLcd;

void GphInit();

void SendMessage(byte cmd,byte chan,short val);
void SendMessage(byte cmd,byte chan);

void ReadSerial();

byte blockingRead();

void blockingRead(byte* A,byte start,byte cnt);

void ParseL();
void Parsel();
void ParseS();
void ParseP();

#endif
