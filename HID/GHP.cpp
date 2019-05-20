#include "GHP.h"

const byte ANALOG_CHANGE = 'A';
const byte BUTTON_PRESS = 'B';
const byte BUTTON_RELEASE = 'R';
const byte ENCODER_INCREMENT = 'I';
const byte ENCODER_DECREMENT = 'D';
const byte SYSTEM = 'S';
const byte SYSTEM_DISCOVER = 'W';

const byte SET_LED = 'L';
const byte SET_LCD = 'l';
const byte SET_LED_LEN = 5;
const byte SET_LCD_LEN = 22;

byte MsgBuffer[25];
bool DoUpdateLcd=false;

void SendMessage(byte cmd,byte chan,short val)
{
    Serial.write(cmd);
    Serial.write(chan);
    Serial.write(val%255);
    Serial.write(val/255);
}

void SendMessage(byte cmd,byte chan)
{
    if(chan!=-1)
    {
        Serial.write(cmd);
        Serial.write(chan);
    }
}

void ReadSerial()
{
    if(Serial.available()>0)
    {
        MsgBuffer[0]=Serial.read();
        switch(MsgBuffer[0])
        {
            case 'L': {ParseL(); break;}
            case 'l': {Parsel(); break;}
            case 'S': {ParseS(); break;}
            case 'P': {ParseP(); break;}
            default : { break;}
        }
    }
}

byte blockingRead()
{
    while (Serial.available()<1){}
    return Serial.read();
    
}

void blockingRead(byte* A,byte start,byte cnt)
{
    for (byte i = start; i < cnt+start; i++)
    {
        A[i]=blockingRead();
    }
    
}

void ParseL()
{
    blockingRead(MsgBuffer,1,4);
    for (byte i = 0; i < 3; i++)
    {
        LedValue[LedAlias[MsgBuffer[1]]] [LedColorAlias[LedAlias[MsgBuffer[1]]] [i]]=MsgBuffer[i+2];
    }
}

void Parsel()
{
    blockingRead(MsgBuffer,1,21);
    for (byte i = 0; i < 20; i++)
    {
        LcdBuff[MsgBuffer[1]][i]=MsgBuffer[i+2];
        LcdBuff[i][20]=0x00;
    }
    DoUpdateLcd=true;
}

void ParseS()
{
    blockingRead(MsgBuffer,1,1);
    switch(MsgBuffer[1])
    {
        case 'W': {SendMessage(SYSTEM,SYSTEM_DISCOVER); Init=true; break;}
        case 'R': {software_Reset(); break;}
        default : { break;}
    }
}

void ParseP()
{
    blockingRead(MsgBuffer,1,2);
    WritePers(MsgBuffer[1],MsgBuffer[2]);
    RefreshPersistence();
}