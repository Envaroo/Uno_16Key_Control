#include <CapacitiveSensor.h>

/*
 * CapitiveSense Library Demo Sketch
 * Paul Badger 2008
 * Uses a high value resistor e.g. 10M between send pin and receive pin
 * Resistor effects sensitivity, experiment with values, 50K - 50M. Larger resistor values yield larger sensor values.
 * Receive pin is the sensor pin - try different amounts of foil/metal on this pin
 */


CapacitiveSensor   s1 = CapacitiveSensor(4,13);        // 10M resistor between pins 4 & 2, pin 2 is sensor pin, add a wire and or foil if desired
CapacitiveSensor   s2 = CapacitiveSensor(4,12);        // 10M resistor between pins 4 & 6, pin 6 is sensor pin, add a wire and or foil
CapacitiveSensor   s3 = CapacitiveSensor(4,11);        // 10M resistor between pins 4 & 8, pin 8 is sensor pin, add a wire and or foil
CapacitiveSensor   s4 = CapacitiveSensor(4,10); 
CapacitiveSensor   s5 = CapacitiveSensor(4,9); 
CapacitiveSensor   s6 = CapacitiveSensor(4,8); 
CapacitiveSensor   s7 = CapacitiveSensor(4,7); 
CapacitiveSensor   s8 = CapacitiveSensor(4,6); 
CapacitiveSensor   s9 = CapacitiveSensor(2,22);
CapacitiveSensor   s10 = CapacitiveSensor(2,24);
CapacitiveSensor   s11 = CapacitiveSensor(2,26);
CapacitiveSensor   s12 = CapacitiveSensor(2,28);
CapacitiveSensor   s13 = CapacitiveSensor(2,30);
CapacitiveSensor   s14 = CapacitiveSensor(2,32);
CapacitiveSensor   s15 = CapacitiveSensor(2,34);
CapacitiveSensor   s16 = CapacitiveSensor(2,36);

CapacitiveSensor sns[16] = {s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16};
int snq = 16;
//int ir = 0;
void setup()                    
{     // turn off autocalibrate on channel 1 - just as an example
   Serial.begin(2000000);
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
    long start = millis();
    long total[snq];
    
    for (int i = 0; i < snq; i++)
    {
    Serial.print(sns[i].capacitiveSensor(8));
    Serial.print('\t');
    }
    /*int ir_state = 0;
    int ir_check = 0;
    ir_check = 13*pow(analogRead(ir)*(5.0/1023.0), -1);
    if (ir_check > 10 && ir_check <200)
    {
      ir_state = 1;
    }
    
    Serial.println(ir_check);*/
    Serial.println('/');
    delay(5);
}
