TARGET		?=	bin/flash_image
LDSCRIPT	=	src/lm4.ld

SRCS		=   $(wildcard src/*.c)
OBJS		=   $(SRCS:.c=.o)

CC			=	arm-none-eabi-gcc
LD			=	arm-none-eabi-ld
OD			=	arm-none-eabi-objdump
OC			=	arm-none-eabi-objcopy

CCFLAGS		=   -mthumb -mcpu=cortex-m4 -mfloat-abi=hard -mfpu=fpv4-sp-d16 -MD -std=c99 -O1 -Wall -ffunction-sections -fdata-sections
LDFLAGS		=   -T $(LDSCRIPT)
ODFLAGS		=   -d
OCFLAGS		=   -O binary

.PHONY:	all clean install dump

all: $(TARGET)

$(TARGET): $(TARGET).elf
	@echo
	@echo Creating flash image...
	$(OC) $(OCFLAGS) $(TARGET).elf $(TARGET)

$(TARGET).elf: $(OBJS)
	@echo
	@echo linking...
	$(LD) $(LDFLAGS) $(OBJS) -o $(TARGET).elf

%.o: %.c
	@echo
	@echo compiling $<...
	$(CC) $(CCFLAGS) -I inc/ -c $< -o $@

dump:
	$(OD)	$(ODFLAGS) $(TARGET).elf | more 

clean:
	rm -f src/*.o src/*.d $(TARGET) $(TARGET).elf

install:
	lm4flash $(TARGET)
