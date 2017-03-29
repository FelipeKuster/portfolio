#ifndef MAIN_H_INCLUDED
#define MAIN_H_INCLUDED

#include <stdbool.h>
#include "motor.h"


#define BIT_SET(a,b) ((a) |= (1<<(b)))
#define BIT_CLEAR(a,b) ((a) &= ~(1<<(b)))
#define BIT_FLIP(a,b) ((a) ^= (1<<(b)))
#define BIT_CHECK(a,b) ((a) & (1<<(b)))

MotorSystem myMotor;
bool pendingEndState;
bool pendingStartState;


void GPIOPortF_Handler();
void GPIOPortD_Handler();
void Timer0A_Handler();
void EventInfrared();
void EventMainClick();


#endif // MAIN_H_INCLUDED
