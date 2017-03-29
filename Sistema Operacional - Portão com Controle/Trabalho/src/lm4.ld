MEMORY
{
	FLASH	(rx)	: ORIGIN = 0x00000000, LENGTH = 0x00040000
	SRAM	(rwx)	: ORIGIN = 0X20000000, LENGTH = 0x00008000
}

SECTIONS
{
	.text :
	{
		_stext = .;
		KEEP (*(vectors))
		*(.text*)
		*(.rodata*)
		_etext = .;
	} > FLASH

	.data :
	{
		_sdata = .;
		*(.data*)
		_edata = .;
	} > SRAM AT > FLASH

	.bss :
	{
		_sbss = .;
		*(.bss*)
		*(COMMON)
		_ebss = .;
	} > SRAM AT > FLASH
}
