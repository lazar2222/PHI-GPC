#include "define.h"

const byte LCDRS = 47, LCDEN = 49, D4 = 45, D5 = 43, D6 = 41, D7 = 39;
const byte MUXE[] = {42,38,40};
const byte MUXQ[] = {26,32};
const byte MUXAR[] = {A2,A3};
const byte MUXDR[] = {34,36};
const byte MUXDW[] = {4,11,3}; 
const byte ENC[5][2] = {{21,33},{20,31},{19,27},{18,23},{2,46}};
const byte MATRIXB[] = {28,22,30,24};
const byte MATRIXD[] = {50,52,53,51};
const byte JOYX = A15, JOYY = A13, JOYS = A14;
const byte PEDALA = A0, PEDALO = 12, PEDALT = 13;

const byte JOYXI = 16, JOYYI = 17, JOYSI = 64;
const byte PEDALAI = 18, PEDALOI = 65, PEDALTI = 66;
const byte AnalogAlias[] = {15,10,8,13,5,7,2,1,0,4,6,3,11,9,12,14,16,17,18};
const byte ButtonAlias[] = {15,10,8,13,5,7,2,1,0,4,6,3,11,9,12,14};
const byte LedAlias[16] = {};
const byte LedColorAlias[16][3] = {};

LiquidCrystal Lcd(LCDRS, LCDEN, D4, D5, D6, D7);

bool Init = false;
byte ENCPre[5] = {HIGH};
short AnalogPre[19]={0};
short AnalogAvg[19]={0};
byte ButtonDebounce[67]={0};
byte LedValue[16][3]={0};
char LcdBuff[4][21];

//move to persistent
bool KRAFTWERK=true;
byte TRESHOLD=3;
byte DEBOUNCE=3;
byte ERF=80;