#ifndef HARDWARE_H_INCLUDED
#define HARDWARE_H_INCLUDED

#include "lm4f120h5qr.h"
#include <stdbool.h>

#define BITBAND(addr,bitnum) ((addr & 0xF0000000)+0x02000000+((addr &0xFFFFF)<<5)+(bitnum<<2))
#define MEM_ADDR(addr) *((volatile unsigned long *) (addr))
#define PORTF_ALIAS ((0x42000000)+ 32*(0x400253FC-0x40000000))
#define PORTD_ALIAS ((0x42000000)+ 32*(0x400073FC-0x40000000))
#define SysTick ((SysTick_Type *) 0xE000E010)
#define PORTD ((PORTD_Type *) PORTD_ALIAS)
#define PORTF ((PORTF_Type *) PORTF_ALIAS)

unsigned long second;
unsigned long h_milis;
bool interruptEnabled;

void Init_SysTick(bool initInterrupt);
void Init_Buttons();
void Init_Leds();
void Init_Interrupts();
void Init_ADC();
unsigned long ReadADC0();
void EnableTimer0();
void DisableTimer0();
bool DebounceSW1();
bool DebounceSW2();
bool DebouncePD0();
bool DebouncePD1();

void SysTick_Delay(unsigned long waitingTime);



typedef struct {
    volatile unsigned long CTRL;
    volatile unsigned long RELOAD_VALUE;
    volatile unsigned long VALUE;
    volatile unsigned long CALIB;
} SysTick_Type;

typedef struct {
    volatile unsigned long SW2;
    volatile unsigned long RED_LED;
    volatile unsigned long BLUE_LED;
    volatile unsigned long GREEN_LED;
    volatile unsigned long SW1;
} PORTF_Type;

typedef struct {
    volatile unsigned long PD0;
    volatile unsigned long PD1;
    volatile unsigned long PD2;
} PORTD_Type;



#endif // HARDWARE_H_INCLUDED
