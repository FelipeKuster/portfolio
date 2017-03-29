#include "motor.h"
#include "main.h"
#include "hardware.h"
#include "startup.h"

MotorSystem InitializeMotorSystem(MyMotorStatus defaultStatus)
{
    int startFlag = false;
    int endFlag = false;

    if (defaultStatus == Open)
    {
        startFlag = true;
    }
    else if(defaultStatus == Closed)
    {
        endFlag = true;
    }

    MotorSystem m = {
                .GateStatus = defaultStatus,
                .OldStatus = Idle,
                .MotorFault = false,
                .InfraredFault = false,
                .StartSensorStatus = startFlag,
                .EndSensorStatus = endFlag,
                .PendingClosing = false,
                .PendingOpening = false,
                .PendingInversion = false,
                .PendingStop = false};
    return m;
}

void Update(MotorSystem* motor)
{

    if(motor->EndSensorStatus && motor->GateStatus==Closing)
    {
        motor->GateStatus = Closed;
    }

    if(motor->StartSensorStatus && motor->GateStatus==Opening)
    {
        motor->GateStatus = Open;
    }

    if(motor->PendingStop)
    {
        Stop(motor,true);
        motor->PendingStop = false;
    }

    else if(motor->PendingOpening)
    {
        motor->GateStatus = Opening;
        motor->EndSensorStatus = false;
        motor->PendingOpening = false;
    }

    else if(motor->PendingClosing)
    {
        motor->GateStatus = Closing;
        motor->StartSensorStatus = false;
        motor->PendingClosing = false;
    }

    else if(motor->PendingInversion)
    {
        Invert(motor);
        motor->PendingInversion = false;
    }
}

void Invert(MotorSystem* motor)
{
    DisableTimer0();
    if(motor->GateStatus == Idle)
    {
        if(motor->InfraredFault)
        {
            motor->GateStatus = Opening;
            motor->OldStatus = Idle;
        }
        else
        {
            switch(motor->OldStatus)
            {
                case Closing:
                motor->GateStatus = Opening;
                motor->OldStatus = Idle;
                break;
                case Opening:
                motor->GateStatus = Closing;
                motor->OldStatus = Idle;
                break;
                default:
                break;
            }
        }
     }
}

void Restart(MotorSystem* m)
{
    if(!(m->MotorFault) && (m->GateStatus == Idle))
    {
        m->GateStatus = m->OldStatus;
    }
}

bool IsMoving(MotorSystem* m)
{
    bool result = false;
    if(myMotor.GateStatus == Opening || myMotor.GateStatus == Closing)
    {
        result = true;
    }
    return result;
}

void Work(MotorSystem* m)
{

    if(m->InfraredFault)
    {
        PORTF->RED_LED = true;
    }
    else
    {
        PORTF->RED_LED = false;
    }

    if(m->GateStatus==Idle)
    {
        PORTD->PD2 = true;
        PORTF->BLUE_LED = false;
        PORTF->GREEN_LED = false;
        if(m->MotorFault)
            return;
    }

    else
    {
        PORTD->PD2 = false;
    }

    if(m->GateStatus==Opening)
    {
        PORTF->BLUE_LED = true;
        PORTF->GREEN_LED = false;
    }
    else if(m->GateStatus==Closing)
    {
        PORTF->BLUE_LED = false;
        PORTF->GREEN_LED = true;
    }
    else if(m->GateStatus==Closed)
    {
        PORTF->BLUE_LED = false;
        PORTF->GREEN_LED = false;
    }

    else if(m->GateStatus == Open)
    {
        PORTF->BLUE_LED = true;
        PORTF->GREEN_LED = true;
    }

}

void Stop(MotorSystem* m, bool timer)
{
    m->OldStatus = m->GateStatus;
    m->GateStatus = Idle;
    if(timer)
        EnableTimer0();

}
