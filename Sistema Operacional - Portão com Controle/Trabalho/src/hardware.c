#include "hardware.h"
#include "main.h"

void Init_SysTick(bool initInterrupt)
{
    interruptEnabled = initInterrupt;
    second = 0x00F42400;   //16 milhões = 1 segundo;
    h_milis = 0x00186A00;   //1,6 milhão = 100 milis
    SysTick->CTRL = 0;                                               //Desliga o Systick durante o Setup
    if(initInterrupt)
    {
        SysTick->RELOAD_VALUE = 3*h_milis;
        SysTick->VALUE=0;
        SysTick->CTRL = NVIC_ST_CTRL_ENABLE+NVIC_ST_CTRL_CLK_SRC+NVIC_ST_CTRL_INTEN;
    }
    else
    {
        SysTick->RELOAD_VALUE = NVIC_ST_RELOAD_M;                   //Configurando o reload como máximo 100ms
        SysTick->VALUE = 0;                                         //Limpa o valor atual
        SysTick->CTRL = NVIC_ST_CTRL_ENABLE+NVIC_ST_CTRL_CLK_SRC;
    }
}

void Init_Buttons()
{
    SYSCTL_RCGC2_R |= SYSCTL_RCGC2_GPIOF;       //Liga porta F se estiver off
    GPIO_PORTF_LOCK_R = 0x4C4F434B;             //Senha de modificação SW2
    GPIO_PORTF_CR_R |= (0b10001);           //Habilita o acesso aos pinos
    GPIO_PORTF_AMSEL_R &= ~(0b10001);       //Desliga a função analógica dos pinos
    GPIO_PORTF_PCTL_R &= ~(0b10001);
    GPIO_PORTF_DIR_R &= ~(0b10001);
    GPIO_PORTF_AFSEL_R &= ~(0b10001);
    GPIO_PORTF_PUR_R |= 0b10001;           //Liga pull-up em 0 e 4
    GPIO_PORTF_DEN_R |= 0b10001;           //Enable pins 0 & 4

    SYSCTL_RCGC2_R |= SYSCTL_RCGC2_GPIOD;
    GPIO_PORTD_CR_R |= (0b11);           //Habilita o acesso aos pinos
    GPIO_PORTD_AMSEL_R &= ~(0b11);       //Desliga a função analógica dos pinos
    GPIO_PORTD_PCTL_R &= ~(0b11);
    GPIO_PORTD_DIR_R &= ~(0b11);
    GPIO_PORTD_AFSEL_R &= ~(0b11);
    GPIO_PORTD_PUR_R |= 0b11;           //Liga pull-up em 0 e 1
    GPIO_PORTD_DEN_R |= 0b11;           //Enable pins 0 & 1
}

void Init_Interrupts()
{

    //###### Interrupção das chaves: SW1 e SW2  ######

    GPIO_PORTD_IS_R &= ~(0b11);     // (d) PD0 & PD1 are edge-sensitives
    GPIO_PORTD_IBE_R &= ~(0b11);    //     PD0 & PD0 are not both edges
    GPIO_PORTD_IEV_R &= ~(0b11);    //     PD0 & PD1 falling edge event
    GPIO_PORTD_ICR_R |= 0b11;      // (e) clear PD0 & PD1 flags
    GPIO_PORTD_IM_R |= 0b11;      // (f) arm interrupt on PD0 & PD1
    NVIC_PRI7_R = (NVIC_PRI0_R&0x00FFFFFF)|0x40000000; // (g) priority 2
    NVIC_EN0_R |= NVIC_EN0_INT3;      // (h) enable interrupt 30 in NVIC

    //######     Interrupção do TimerA 0        ######
    SYSCTL_RCGC1_R |= SYSCTL_RCGC1_TIMER0; // 0) activate timer0
    TIMER0_CTL_R &= ~0x00000001;     // 1) disable timer0A during setup
    //TIMER0_CFG_R = 0x00000004;       // 2) configure for 16-bit timer mode
    TIMER0_CFG_R = TIMER_CFG_32_BIT_TIMER;  // 32 bits
    TIMER0_TAMR_R = 0x00000002;      // 3) configure for periodic mode
    //TIMER0_TAPR_R = 255;              // 4) pre-scale: 49 = 1 us  - não funciona em 32 bits
    TIMER0_TAILR_R = 80000000;       // 5) 5 s
    TIMER0_ICR_R = 0x00000001;       // 6) clear timer0A timeout flag
    TIMER0_IMR_R |= 0x00000001;      // 7) arm timeout interrupt
    NVIC_PRI4_R = (NVIC_PRI4_R&0x00FFFFFF)|0x40000000; // 8) priority 2
    NVIC_EN0_R |= NVIC_EN0_INT19;    // 9) enable interrupt 19 in NVIC
    //TIMER0_CTL_R |= 0x00000001;      // 10) enable timer0A*/
}


void Init_Leds()
{
    SYSCTL_RCGC2_R |= SYSCTL_RCGC2_GPIOF;  //Ligar clock na porta F
    GPIO_PORTF_DIR_R |= 0b01110;           //PF4 = SW1, PF3 = Green, PF2= BLUE, PF1 = Red, PF0 = SW2. 0> input; 1>output
    GPIO_PORTF_DEN_R |= 0b01110;           //Enable pins 1-3

    SYSCTL_RCGC2_R |= SYSCTL_RCGC2_GPIOD;
    GPIO_PORTD_DIR_R |= 0b00100;
    GPIO_PORTD_DEN_R |= 0b00100;
}

void Init_ADC()
{
    //Name Num PIN Type Buffer Description
    //AIN9 59 PE4 I Analog Analog-to-digital converter input 9
    //D_out = 4095*V_in/3,3

    int PE4 = 0b10000;   //0x10 = 16

    SYSCTL_RCGC2_R |= SYSCTL_RCGC2_GPIOE;  //2) Habilitar Clock PE
    GPIO_PORTE_DIR_R &= ~(PE4);            //3) Input
    GPIO_PORTE_AFSEL_R |= PE4;             //4) Função alternada
    GPIO_PORTE_DEN_R &= ~(PE4);            //5) Digital off
    GPIO_PORTE_AMSEL_R |= PE4;             //6) Analog on

    SYSCTL_RCGC0_R |= SYSCTL_SCGC0_ADC0;   //7) Habilitar Clock ADC
    SYSCTL_RCGC0_R &= ~0x00000300;         // 8) 125KHz
    ADC0_SSPRI_R = 0x0123;
    ADC0_ACTSS_R &= ~0x0008;               // 9) Desabilitar Seq. 3 na instalação
    ADC0_EMUX_R &= ~0xF000;                // 10) Evento: software trigger
    ADC0_SSMUX3_R &= ~0x000F;              // 11) Selecionar canal
    ADC0_SSMUX3_R += 9;                    //    channel Ain9 (PE4)
    ADC0_SSCTL3_R = 0x0006;                // 12) Setar bits IE0 e END0
    ADC0_ACTSS_R |= 0x0008;                // 13) Ligar amostragem
}

unsigned long ReadADC0()
{

    unsigned long result;
    ADC0_PSSI_R = 0x0008;
    while((ADC0_RIS_R&0x08)==0){};
    result = ADC0_SSFIFO3_R&0xFFF;
    ADC0_ISC_R = 0x0008;
    return result;

}

void DisableTimer0()
{
    TIMER0_CTL_R &= ~0x00000001;
}

void EnableTimer0()
{
    TIMER0_TAILR_R = 80000000;
    TIMER0_CTL_R |= 0x00000001;      // 10) enable timer0A*/
}

void SysTick_Delay(unsigned long waitingTime)
{
    if(!interruptEnabled)
    {
        unsigned long startTime = NVIC_ST_CURRENT_R;
        volatile unsigned long currentTime = 0;

        while(currentTime <= waitingTime)
        {
            currentTime = (startTime - NVIC_ST_CURRENT_R)&NVIC_ST_RELOAD_M;
        }
    }
}

bool DebounceSW1()
{
   bool initial_state = PORTF->SW1;
   bool final_state;
   int count = 0;

   while(count < 1000)
   {
        if((PORTF->SW1) != initial_state)
        {
            initial_state = PORTF->SW1;
            count = 0;
        }
        else
        {
            count++;
        }
   }
   final_state = initial_state;
   return final_state;
}

bool DebounceSW2()
{
   bool final_state;
   bool initial_state = PORTF->SW2;
   int count = 0;

   while(count < 1000)  //9,3 ms
   {
        if((PORTF->SW2) != initial_state)
        {
            initial_state = PORTF->SW2;
            count = 0;
        }
        else
        {
            count++;
        }
   }
   final_state = initial_state;
   return final_state;
}

bool DebouncePD0()
{
   bool final_state;
   bool initial_state = PORTD->PD0;
   int count = 0;

   while(count < 40)  //9,3 ms
   {
        if((PORTD->PD0) != initial_state)
        {
            initial_state = PORTD->PD0;
            count = 0;
        }
        else
        {
            count++;
        }
   }
   final_state = initial_state;
   return final_state;
}

bool DebouncePD1()
{
   bool final_state;
   bool initial_state = PORTD->PD1;
   int count = 0;

   while(count < 40)  //9,3 ms = 150000
   {
        if((PORTD->PD1) != initial_state)
        {
            initial_state = PORTD->PD1;
            count = 0;
        }
        else
        {
            count++;
        }
   }
   final_state = initial_state;
   return final_state;
}
