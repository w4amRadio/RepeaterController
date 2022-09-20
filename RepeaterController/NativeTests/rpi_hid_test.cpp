#include <linux/platform_device.h>
#include <linux/usb/g_hid.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

#define BUF_LEN 9


//https://docs.kernel.org/usb/gadget_hid.html
int main(int argc, const char* argv[])
{
	const char* filename = NULL;
	fd_set rfds;

	if ((fd = open(filename, O_RDWR, 0666)) == -1)
	{
		perror(filename);
		return 3;
	}
}
