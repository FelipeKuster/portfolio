#ifndef STARTUP_H_INCLUDED
#define STARTUP_H_INCLUDED

#define BIT_SET(a,b) ((a) |= (1<<(b)))
#define BIT_CLEAR(a,b) ((a) &= ~(1<<(b)))
#define BIT_FLIP(a,b) ((a) ^= (1<<(b)))
#define BIT_CHECK(a,b) ((a) & (1<<(b)))
#define STACK_TOP 0x20008000

extern unsigned long _stext;
extern unsigned long _etext;
extern unsigned long _sdata;
extern unsigned long _edata;
extern unsigned long _sbss;
extern unsigned long _ebss;

//Definições de Funções

extern int main();
extern void GPIOPortF_Handler();
extern void GPIOPortD_Handler();
extern void Timer0A_Handler();
void Reset_Handler(void);
void Default_Handler(void);
void SysTick_Handler(void);


#endif // STARTUP_H_INCLUDED
