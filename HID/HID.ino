#include <LiquidCrystal.h>
#include "define.h"
#include "lib.h"
#include "GHP.h"
#include "Persistence.h"

void SnLInit();
void PinModeInit();
void MuxA();
void MatrixB(byte mux);
void MuxB();
void MatrixD(byte mux);
void Enc();
void Joy();
void Pedal();
void LcdUpdate();

void setup() 
{
  SnLInit();
  PinModeInit();
  RefreshPersistence();
}

void loop() 
{
  ReadSerial();
  MuxA();
  MuxB();
  Enc();
  Joy();
  if(KRAFTWERK)
  {
    Pedal();
  }
  LcdUpdate();
}

void SnLInit()
{
  Serial.begin(115200);
  Lcd.begin(20,4); 
}

void PinModeInit()
{
  DoPinMode(MUXE,3,OUTPUT);
  DoPinMode(MUXQ,2,OUTPUT);
  DoPinMode(MUXAR,2,INPUT);
  DoPinMode(MUXDR,2,INPUT);
  DoPinMode(MUXDW,3,OUTPUT);
  for(byte i=0;i<5;i++)
  {
    DoPinMode(ENC[i],2,INPUT_PULLUP);
  }
  DoPinMode(MATRIXB,4,OUTPUT);
  DoPinMode(MATRIXD,4,OUTPUT);
  pinMode(JOYX,INPUT);
  pinMode(JOYY,INPUT);
  pinMode(JOYS,INPUT_PULLUP);
  pinMode(PEDALA,INPUT);
  pinMode(PEDALO,INPUT_PULLUP);
  pinMode(PEDALT,INPUT_PULLUP);
}

void MuxA()
{
  for(byte i=0;i<8;i++)
  {
    SetMux(MUXE,3,i);

    short val = analogRead(MUXAR[0]);
    short avg = (val+AnalogAvg[i])/2;
    AnalogAvg[i]=val;
    if(abs(avg-AnalogPre[i])>TRESHOLD){SendMessage(ANALOG_CHANGE,AnalogAlias[i],avg);AnalogPre[i]=avg;}

    val = analogRead(MUXAR[1]);
    avg = (val+AnalogAvg[i+8])/2;
    AnalogAvg[i+8]=val;
    if(abs(avg-AnalogPre[i+8])>TRESHOLD){SendMessage(ANALOG_CHANGE,AnalogAlias[i+8],avg);AnalogPre[i+8]=avg;}

    MatrixB(i);
  }
}

void MatrixB(byte mux)
{
    for(byte i=0;i<4;i++)
    {
      SetMatrix(MATRIXB,4,i);

      byte ind=i*8+mux;
      if(digitalRead(MUXDR[0])==HIGH && ButtonDebounce[ind]<DEBOUNCE)
      {
        ButtonDebounce[ind]++;
        if(ButtonDebounce[ind]==DEBOUNCE){SendMessage(BUTTON_PRESS,ButtonAlias[ind]);}
      }
      else if(ButtonDebounce[ind]>0)
      {
        ButtonDebounce[ind]--;
        if(ButtonDebounce[ind]==0){SendMessage(BUTTON_RELEASE,ButtonAlias[ind]);}
      }

      ind += 32;
      if(digitalRead(MUXDR[1])==HIGH && ButtonDebounce[ind]<DEBOUNCE)
      {
        ButtonDebounce[ind]++;
        if(ButtonDebounce[ind]==DEBOUNCE){SendMessage(BUTTON_PRESS,ButtonAlias[ind]);}
      }
      else if(ButtonDebounce[ind]>0)
      {
        ButtonDebounce[ind]--;
        if(ButtonDebounce[ind]==0){SendMessage(BUTTON_RELEASE,ButtonAlias[ind]);}
      }
    }
}

void MuxB()
{
  for(byte i=0;i<4;i++)
  {
    SetMux(MUXQ,2,i);

    MatrixD(i);
  }
}

void MatrixD(byte mux)
{
    for(byte i=0;i<4;i++)
    {
      SetMatrix(MATRIXD,4,i,true);

      analogWrite(MUXDW[0],LedValue[i*4+mux][0]);
      analogWrite(MUXDW[1],LedValue[i*4+mux][1]);
      analogWrite(MUXDW[2],LedValue[i*4+mux][2]);

      delay(1);
    }
}

void Enc()
{
  for(byte i=0;i<5;i++)
  {
    if(digitalRead(ENC[i][0])==HIGH && ENCPre[i]==LOW)
    {
      if(digitalRead(ENC[i][1])==HIGH)  
      {
        SendMessage(ENCODER_DECREMENT,i);
      }
      else
      {
        SendMessage(ENCODER_INCREMENT,i);
      }
    }
    ENCPre[i]=digitalRead(ENC[i][0]);
  }  
}

void Joy()
{
  short val = analogRead(JOYX);
  short avg = (val+AnalogAvg[JOYXI])/2;
  AnalogAvg[JOYXI]=val;
  if(abs(avg-AnalogPre[JOYXI])>TRESHOLD){SendMessage(ANALOG_CHANGE,AnalogAlias[JOYXI],avg);AnalogPre[JOYXI]=avg;}

  val = analogRead(JOYY);
  avg = (val+AnalogAvg[JOYYI])/2;
  AnalogAvg[JOYYI]=val;
  if(abs(avg-AnalogPre[JOYYI])>TRESHOLD){SendMessage(ANALOG_CHANGE,AnalogAlias[JOYYI],avg);AnalogPre[JOYYI]=avg;}

  if(digitalRead(JOYS)==LOW && ButtonDebounce[JOYSI]<DEBOUNCE)
  {
    ButtonDebounce[JOYSI]++;
    if(ButtonDebounce[JOYSI]==DEBOUNCE){SendMessage(BUTTON_PRESS,ButtonAlias[JOYSI]);}
  }
  else if(ButtonDebounce[JOYSI]>0)
  {
    ButtonDebounce[JOYSI]--;
    if(ButtonDebounce[JOYSI]==0){SendMessage(BUTTON_RELEASE,ButtonAlias[JOYSI]);}
  }
}

void Pedal()
{
  short val = analogRead(PEDALA);
  short avg = (val*ERF)+((1-ERF)*AnalogAvg[PEDALAI]);
  if(avg!=AnalogPre[PEDALAI]){SendMessage(ANALOG_CHANGE,AnalogAlias[PEDALAI],avg);AnalogPre[PEDALAI]=avg;}
  AnalogAvg[PEDALAI]=avg;

  if(digitalRead(PEDALO)==LOW && ButtonDebounce[PEDALOI]<DEBOUNCE)
  {
    ButtonDebounce[PEDALOI]++;
    if(ButtonDebounce[PEDALOI]==DEBOUNCE){SendMessage(BUTTON_PRESS,ButtonAlias[PEDALOI]);}
  }
  else if(ButtonDebounce[PEDALOI]>0)
  {
    ButtonDebounce[PEDALOI]--;
    if(ButtonDebounce[PEDALOI]==0){SendMessage(BUTTON_RELEASE,ButtonAlias[PEDALOI]);}
  }
  if(digitalRead(PEDALT)==LOW && ButtonDebounce[PEDALTI]<DEBOUNCE)
  {
    ButtonDebounce[PEDALTI]++;
    if(ButtonDebounce[PEDALTI]==DEBOUNCE){SendMessage(BUTTON_PRESS,ButtonAlias[PEDALTI]);}
  }
  else if(ButtonDebounce[PEDALTI]>0)
  {
    ButtonDebounce[PEDALTI]--;
    if(ButtonDebounce[PEDALTI]==0){SendMessage(BUTTON_RELEASE,ButtonAlias[PEDALTI]);}
  }
}

void LcdUpdate()
{
  if(DoUpdateLcd){
    Lcd.setCursor(0,0);
    for (byte i = 0; i < 4; i++)
    {
      Lcd.print(LcdBuff[i]);
    }
    DoUpdateLcd=false;
  }
  
}