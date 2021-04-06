#include <CapacitiveSensor.h>

/*
 * CapitiveSense Library Demo Sketch
 * Paul Badger 2008
 * Uses a high value resistor e.g. 10M between send pin and receive pin
 * Resistor effects sensitivity, experiment with values, 50K - 50M. Larger resistor values yield larger sensor values.
 * Receive pin is the sensor pin - try different amounts of foil/metal on this pin
 */


CapacitiveSensor   s1 = CapacitiveSensor(A0,13);        // 10M resistor between pins 4 & 2, pin 2 is sensor pin, add a wire and or foil if desired
CapacitiveSensor   s2 = CapacitiveSensor(A0,12);        // 10M resistor between pins 4 & 6, pin 6 is sensor pin, add a wire and or foil
CapacitiveSensor   s3 = CapacitiveSensor(A0,11);        // 10M resistor between pins 4 & 8, pin 8 is sensor pin, add a wire and or foil
CapacitiveSensor   s4 = CapacitiveSensor(A0,10); 
CapacitiveSensor   s5 = CapacitiveSensor(A0,9); 
CapacitiveSensor   s6 = CapacitiveSensor(A0,8); 
CapacitiveSensor   s7 = CapacitiveSensor(A0,7); 
CapacitiveSensor   s8 = CapacitiveSensor(A0,6); 
CapacitiveSensor   s9 = CapacitiveSensor(A1,5);
CapacitiveSensor   s10 = CapacitiveSensor(A1,4);
CapacitiveSensor   s11 = CapacitiveSensor(A1,3);
CapacitiveSensor   s12 = CapacitiveSensor(A1,2);
CapacitiveSensor   s13 = CapacitiveSensor(A1,A2);
CapacitiveSensor   s14 = CapacitiveSensor(A1,A3);
CapacitiveSensor   s15 = CapacitiveSensor(A1,A4);
CapacitiveSensor   s16 = CapacitiveSensor(A1,A5);

CapacitiveSensor sns[16] = {s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16};
const int snq = 16;
const int dataSize = 18;
byte outBufMem[dataSize];
//int ir = 0;
void setup()                    
{     // turn off autocalibrate on channel 1 - just as an example
   Serial.begin(115200);
   //pinMode(ir, INPUT);
   for (int i = 0; i < snq; i++)
   {
    sns[i].set_CS_AutocaL_Millis(0xFFFFFFFF);
   }
   
   
}

void loop()                    
{
    
    if(Serial.available() > 0)
    {
      if(Serial.read() == 'a')
      {
        for (int i = 0; i < snq; i++)
        {
          sns[i].reset_CS_AutoCal();
        }  
      }
    }
    byte outBuf[dataSize];
    outBuf[0] = 2;
    outBuf[17] = 3;
    
    for (int i = 0; i < snq; i++)
    {
      if(sns[i].capacitiveSensor(6) > 40)
      {
        outBuf[i+1] = 1;
      }
      else
      {
        outBuf[i+1] = 0;
      }
    

    }

    if (memcmp(outBuf, outBufMem, dataSize) != 0)
    {
      for (int i = 0; i < dataSize; i++)
      {
        Serial.print(outBuf[i], HEX);
      }
      memcpy(outBufMem, outBuf, dataSize); 
    }
    /*int ir_state = 0;
    int ir_check = 0;
    ir_check = 13*pow(analogRead(ir)*(5.0/1023.0), -1);
    if (ir_check > 10 && ir_check <200)
    {
      ir_state = 1;
    }
    
    Serial.println(ir_check);*/
    delay(5);
}
