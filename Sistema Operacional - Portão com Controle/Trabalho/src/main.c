/*

INSTITUTO FEDERAL DO ESPÍRITO SANTO: IFES
    SISTEMA DE CONTROLE DE PORTÃO POR CONTROLE INFRAVERMELHO
        por André Rozetti
            Felipe Küster


    Este é um trabalho apresentado como requisito parcial de aprovação semestral
    na disciplina de: Tópicos especiais em sistemas digitais e subjetivo à avaliação
    do professor Alexandre Secchin de Melo. A autorização de cópia desse código e/ou
    porções do mesmo fica a julgamento do professor.


*/


#include "main.h"
#include "motor.h"
#include "hardware.h"


int main()
{
    // ###### Iniciando Hardware  #######
    //Init_SysTick(false);
    Init_Buttons();
    Init_Leds();
    Init_ADC();

    // ###### Iniciando Software #######

    pendingEndState = false;
    pendingStartState = false;
    bool SW1Read = false;
    bool SW2Read = false;
    bool SW2CycleState;
    bool SW1CycleState;
    unsigned long analogInput = 0;

    myMotor = InitializeMotorSystem(Closed);

    //####### Iniciado Interrupções ######

    Init_Interrupts();

    //###### LÓGICA DO MOTOR ######

    while(1)
    {
        analogInput = ReadADC0();
        if((analogInput > 2047) &&(!myMotor.GateStatus == Idle))
        {
            Stop(&myMotor,false);
            myMotor.MotorFault = true;
        }
        else if((analogInput < 2047) && myMotor.MotorFault)
        {
            myMotor.MotorFault = false;
            Restart(&myMotor);
        }

        SW1CycleState = DebounceSW1();
        SW2CycleState = DebounceSW2();

        if(!SW2CycleState && !SW2Read )
        {
            SW2Read = true;
            EventInfrared();
        }
        else if(SW2CycleState)
        {
            SW2Read = false;
        }

        if(!SW1CycleState && !SW1Read && SW2CycleState)
        {
            SW1Read = true;
            EventMainClick();
        }
        else if(SW1CycleState)
        {
            SW1Read = false;
        }

        if(pendingEndState)
        {
            if(!DebouncePD0())
            {
                myMotor.EndSensorStatus = true;
                pendingEndState = false;
            }
        }

        if(pendingStartState)
        {
            if(!DebouncePD1())
            {
                myMotor.StartSensorStatus = true;
                pendingStartState = false;
            }
        }

        Update(&myMotor);
        Work(&myMotor);
    }

    return 0;
}


//  DESABILITADA
void GPIOPortF_Handler()
{
    if(PORTF->SW2)
    {
        GPIO_PORTF_ICR_R = 0b00001;
        EventInfrared();
    }
    if(PORTF->SW1)
    {
        GPIO_PORTF_ICR_R = 0b10000;
        EventMainClick();
    }
}

void GPIOPortD_Handler()
{
    if(!PORTD->PD0)
    {
        if(myMotor.GateStatus == Closing)
            pendingEndState = true;  //FECHADO
        GPIO_PORTD_ICR_R = 0b01;
    }
    if(!PORTD->PD1)
    {
        if(myMotor.GateStatus == Opening)
            pendingStartState = true;  //ABERTO
        GPIO_PORTD_ICR_R = 0b10;
    }
}

void Timer0A_Handler()
{
    TIMER0_ICR_R = TIMER_ICR_TATOCINT;   //IDENTIFICA QUE A INTERRUPÇÃO ACONTECEU

    if(myMotor.InfraredFault)
    {
        myMotor.PendingOpening = true;
    }
    else
    {
        myMotor.PendingClosing = true;       //FECHA O PORTÃO
    }

    DisableTimer0();
}

void EventInfrared()
{
    if(myMotor.InfraredFault)
    {
        myMotor.InfraredFault = false;
    }
    else
    {
        myMotor.InfraredFault = true;
        if(myMotor.GateStatus == Closing)
        {
            Stop(&myMotor,true);
        }
    }
}

void EventMainClick()
{
    if(myMotor.GateStatus == Open)
    {
        if(!myMotor.InfraredFault)
            myMotor.PendingClosing = true;
    }
    else if(myMotor.GateStatus == Closed)
    {
        myMotor.PendingOpening = true;
    }
    else if(myMotor.GateStatus == Closing || myMotor.GateStatus == Opening)
    {
        myMotor.PendingStop = true;
    }
    else if(myMotor.GateStatus == Idle)
    {
        myMotor.PendingInversion = true;
    }
}
