#ifndef MOTOR_H_INCLUDED
#define MOTOR_H_INCLUDED

#include <stdbool.h>


typedef enum MotorStatus
{
    Idle = 0,
    Closed = 1,
    Open = 2,
    Closing = 3,
    Opening = 4
} MyMotorStatus;

typedef struct
{
    MyMotorStatus GateStatus;
    MyMotorStatus OldStatus;
    bool MotorFault;
    bool InfraredFault;
    bool StartSensorStatus;
    bool EndSensorStatus;
    bool PendingClosing;
    bool PendingOpening;
    bool PendingInversion;
    bool PendingStop;
} MotorSystem;

MotorSystem InitializeMotorSystem(MyMotorStatus defaultStatus);
void Invert(MotorSystem* motor);
void Update(MotorSystem* motor);
void Work(MotorSystem* m);
void Stop(MotorSystem* m, bool timer);
bool IsMoving(MotorSystem *m);
void Restart(MotorSystem* m);



#endif // MOTOR_H_INCLUDED
