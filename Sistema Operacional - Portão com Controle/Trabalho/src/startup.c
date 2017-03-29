#include "lm4f120h5qr.h"
#include "startup.h"
#include "hardware.h"
#include <stdbool.h>

void Default_Handler(void)
{
    PORTF->BLUE_LED = false;
    PORTF->GREEN_LED = false;
    PORTF->RED_LED = false;
    while(1);
}

void Reset_Handler()
{
    unsigned long *src, *dst;

    src = &_etext;
    dst = &_sdata;

    while(dst < &_edata)
    {
        *(dst++) = *(src++);
    }

    dst = &_sbss;
    while (dst < &_ebss)
        *(dst++) = 0;

    main();
}

void SysTick_Handler()
{
    BIT_FLIP(GPIO_PORTF_DATA_R,3);
}


unsigned long *vectorable[] __attribute__
((section("vectors"))) = {
    (unsigned long *) STACK_TOP,
    (unsigned long *) Reset_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) Default_Handler,
    (unsigned long *) SysTick_Handler,
    (unsigned long *) Default_Handler,							    // 16
    (unsigned long *) Default_Handler,								// 17
    (unsigned long *) Default_Handler,								// 18
    (unsigned long *) GPIOPortD_Handler,							// 19
    (unsigned long *) Default_Handler,								// 20
    (unsigned long *) Default_Handler,								// 21
    (unsigned long *) Default_Handler,								// 22
    (unsigned long *) Default_Handler,								// 23
    (unsigned long *) Default_Handler,								// 24
    (unsigned long *) Default_Handler,								// 25
    (unsigned long *) Default_Handler,								// 26
    (unsigned long *) Default_Handler,								// 27
    (unsigned long *) Default_Handler,								// 28
    (unsigned long *) Default_Handler,								// 29
    (unsigned long *) Default_Handler,								// 30
    (unsigned long *) Default_Handler,								// 31
    (unsigned long *) Default_Handler,								// 32
    (unsigned long *) Default_Handler,								// 33
    (unsigned long *) Default_Handler,								// 34
    (unsigned long *) Timer0A_Handler,								// 35
    (unsigned long *) Default_Handler,								// 36
    (unsigned long *) Default_Handler,								// 37
    (unsigned long *) Default_Handler,								// 38
    (unsigned long *) Default_Handler,								// 39
    (unsigned long *) Default_Handler,								// 40
    (unsigned long *) Default_Handler,								// 41
    (unsigned long *) Default_Handler,								// 42
    (unsigned long *) Default_Handler,								// 43
    (unsigned long *) Default_Handler,								// 44
    (unsigned long *) Default_Handler,								// 45
    (unsigned long *) GPIOPortF_Handler		                        // 46  IRQ 30
};


